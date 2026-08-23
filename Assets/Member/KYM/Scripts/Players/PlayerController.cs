using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.Agents.FSM;
using Member.KYM.Scripts.CoreSystems;
using Member.KYM.Scripts.Players.ClimbingSystems;
using Member.KYM.Scripts.Players.FSM;
using Member.KYM.Scripts.Players.FSM.Interface;
using Member.KYM.Scripts.Players.TraversalSystems;
using UnityEngine;
using StateMachine = Member.KYM.Scripts.Agents.StateMachine;

namespace Member.KYM.Scripts.Players
{
    public class PlayerController : Agent
    {
        [field:SerializeField] public PlayerInputSO PlayerInput { get; private set; }
        [field:SerializeField] public UIInputSO UIInput { get; private set; }
        [SerializeField] private StateListSO playerStates;
        [SerializeField] private float spaceHoldDecisionTime = 0.12f;

        [Header("Stamina Costs")]
        [SerializeField, Min(0f)] private float dodgeStaminaCost = 20f;
        [SerializeField, Min(0f)] private float climbStaminaCost = 35f;
        [SerializeField, Min(0f)] private float traversalJumpStaminaCost = 25f;
        
        public StateMachine StateMachine { get; private set; }
        private PlayerMover _playerMover; //임시
        private StaminaModule _stamina;
        private bool _isSpaceDecisionPending;
        private float _spacePressedTime;

        public PlayerCombatModes CombatMode { get; set; } = PlayerCombatModes.NORMAL;
        
        public ClimbLink CurrentClimbLink { get; private set; }
        public JumpLink CurrentJumpLink { get; private set; }
        
        protected override void InitializeModules()
        {
            base.InitializeModules();
            StateMachine = new StateMachine(this, playerStates.states);
            _playerMover = GetModule<PlayerMover>();
            _stamina = GetModule<StaminaModule>();
            Debug.Assert(_stamina != null, "PlayerController requires StaminaModule.");
        }

        protected override void AfterInitializeModules()
        {
            base.AfterInitializeModules();

            PlayerInput.OnSpaceBarPressed += HandleSpaceBarPressed;
            PlayerInput.OnFKeyPressed += HandleFKeyPressed;
            PlayerInput.OnGuardKeyPressed += HandleGuardKeyPressed;
        }
        private void Start()
        {
            ChangeState(PlayerStateEnum.IDLE);
        }
        
        public void StartClimb(ClimbLink climbLink)
        {
            if (climbLink == null || !climbLink.IsValid)
                return;

            if (StateMachine.CurrentState is PlayerClimbState)
                return;

            if (_stamina != null && !_stamina.TryConsume(climbStaminaCost))
                return;

            CurrentClimbLink = climbLink;
            ChangeState(PlayerStateEnum.CLIMB);
        }

        public void ClearClimbLink()
        {
            CurrentClimbLink = null;
        }

        public void StartTraversalJump(JumpLink jumpLink)
        {
            if (jumpLink == null || !jumpLink.IsValid)
                return;

            if (StateMachine.CurrentState is PlayerJumpState
                or PlayerClimbState)
            {
                return;
            }

            if (StateMachine.CurrentState is not PlayerIdleState
                && StateMachine.CurrentState is not PlayerWalkState
                && StateMachine.CurrentState is not PlayerRunState)
            {
                return;
            }

            if (_stamina != null
                && !_stamina.TryConsume(traversalJumpStaminaCost))
            {
                return;
            }

            CurrentJumpLink = jumpLink;
            ChangeState(PlayerStateEnum.JUMP);
        }

        public void ClearJumpLink()
        {
            CurrentJumpLink = null;
        }
        
        private void HandleGuardKeyPressed(bool value)
        {
            if (value)
                ChangeState(PlayerStateEnum.GUARD);
        }
        private void HandleFKeyPressed()
        {
            if (StateMachine.CurrentState is not ICanModeChangeState) return;
            
            ChangeState(PlayerStateEnum.MODE_CHANGE);
        }

        private void OnDestroy()
        {
            if (PlayerInput != null)
            {
                PlayerInput.OnSpaceBarPressed -= HandleSpaceBarPressed;
                PlayerInput.OnFKeyPressed -= HandleFKeyPressed;
            }
        }

        private void HandleSpaceBarPressed(bool value)
        {
            if (value)
            {
                StartSpaceDecision();
                return;
            }

            FinishSpaceDecisionAsDodge();
        }

        private void StartSpaceDecision()
        {
            if (StateMachine.CurrentState is not ICanDodgeState)
                return;

            _isSpaceDecisionPending = true;
            _spacePressedTime = Time.time;
        }

        private void FinishSpaceDecisionAsDodge()
        {
            if (!_isSpaceDecisionPending)
                return;

            _isSpaceDecisionPending = false;
            
            if (Time.time - _spacePressedTime <= spaceHoldDecisionTime
                && StateMachine.CurrentState is ICanDodgeState
                && (_stamina == null || _stamina.TryConsume(dodgeStaminaCost)))
            {
                ChangeState(PlayerStateEnum.DODGE);
            }
        }

        private void UpdateSpaceHoldDecision()
        {
            if (!_isSpaceDecisionPending)
                return;

            if (!PlayerInput.IsSpaceBarPress)
            {
                _isSpaceDecisionPending = false;
                return;
            }

            if (Time.time - _spacePressedTime < spaceHoldDecisionTime)
                return;

            if (PlayerInput.CurrentMovement.sqrMagnitude <= 0.01f)
                return;
            
            _isSpaceDecisionPending = false;

            if (StateMachine.CurrentState is PlayerWalkState)
            {
                ChangeState(PlayerStateEnum.RUN, 0.2f);
            }
        }

        private void Update()
        {
            UpdateSpaceHoldDecision();
            FallingDetect();
            StateMachine.UpdateMachine();
        }

        private void FallingDetect()
        {
            if (_playerMover.IsGround || !_playerMover.UseGravity)
                return;

            if (_playerMover.VerticalVelocity < -3f && StateMachine.CurrentState is ICanFallState)
            {
                ChangeState(PlayerStateEnum.FALL);
            }
        }
        
        public void ChangeState(PlayerStateEnum newState, float transitionDuration = 0.1f)
            => StateMachine.ChangeState((int)newState, transitionDuration);
    }
}
