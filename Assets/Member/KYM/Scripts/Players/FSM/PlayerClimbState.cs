using KimLIb.AnimatorSystems;
using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.Agents.FSM;
using Member.KYM.Scripts.Players.ClimbingSystems;
using UnityEngine;

namespace Member.KYM.Scripts.Players.FSM
{
    public class PlayerClimbState : AbstractPlayerState
    {
        private enum ClimbPhase
        {
            JumpToLedge,
            ClimbUp,
            HorizontalCorrection
        }

        private AgentTrigger _trigger;
        private ClimbLink _climbLink;
        private ClimbPhase _phase;
        private float _alignmentTimer;
        private Vector3 _jumpStartPosition;
        private Quaternion _jumpStartRotation;
        private Vector3 _hangRootPosition;
        private Vector3 _climbEndPosition;
        private Quaternion _climbRotation;
        private bool _isJumpAnimationEnded;
        private bool _isAlignmentCompleted;
        private float _horizontalCorrectionTimer;
        private Vector3 _horizontalCorrectionStart;
        private Vector3 _horizontalCorrectionTarget;

        public PlayerClimbState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            ResetAnimationRigging();
            _climbLink = _player.CurrentClimbLink;

            if (_climbLink == null || !_climbLink.IsValid)
            {
                Debug.LogError("Climb state requires a valid ClimbLink with ledge and end points.");
                _player.ChangeState(PlayerStateEnum.IDLE);
                return;
            }

            if (_trigger == null)
                _trigger = _player.GetModule<AgentTrigger>();

            Debug.Assert(_trigger != null, "Player climb state requires AgentTrigger.");

            _mover.CanManualMove = false;
            _mover.UseRootMotion = false;
            _mover.UseGravity = false;

            _phase = ClimbPhase.JumpToLedge;
            _alignmentTimer = 0f;
            _jumpStartPosition = _player.transform.position;
            _jumpStartRotation = _player.transform.rotation;

            float lateralPosition = _climbLink.GetLateralPosition(_jumpStartPosition);
            _hangRootPosition = _climbLink.GetHangRootPosition(lateralPosition);
            _climbEndPosition = _climbLink.GetEndPosition(lateralPosition);
            _climbRotation = _climbLink.GetClimbRotation(
                _hangRootPosition,
                _climbEndPosition);

            _isJumpAnimationEnded = false;
            _isAlignmentCompleted = false;

            _trigger.OnAnimationEnd -= HandleJumpAnimationEnd;
            _trigger.OnAnimationEnd += HandleJumpAnimationEnd;

            _renderer.PlayClip(_climbLink.JumpAnimationHash, transitionDuration, layerIndex);

        }

        public override void Update()
        {
            base.Update();

            if (_climbLink == null)
                return;

            if (_phase == ClimbPhase.HorizontalCorrection)
            {
                UpdateHorizontalCorrection();
                return;
            }

            if (_phase != ClimbPhase.JumpToLedge)
                return;

            _alignmentTimer += Time.deltaTime;

            float normalizedTime = Mathf.Clamp01(
                _alignmentTimer / _climbLink.AlignmentDuration);
            float easedTime = Mathf.SmoothStep(0f, 1f, normalizedTime);

            Vector3 position = Vector3.Lerp(
                _jumpStartPosition,
                _hangRootPosition,
                easedTime);

            position.y += _climbLink.JumpHeightCurve.Evaluate(normalizedTime)
                          * _climbLink.ExtraJumpHeight;

            Quaternion rotation = Quaternion.Slerp(
                _jumpStartRotation,
                _climbRotation,
                easedTime);

            _player.transform.SetPositionAndRotation(position, rotation);

            if (normalizedTime >= 1f)
            {
                _isAlignmentCompleted = true;
                TryStartClimbUp();
            }
        }

        private void HandleJumpAnimationEnd()
        {
            _isJumpAnimationEnded = true;
            TryStartClimbUp();
        }

        private void TryStartClimbUp()
        {
            if (_phase != ClimbPhase.JumpToLedge)
                return;

            if (!_isJumpAnimationEnded || !_isAlignmentCompleted)
                return;

            StartClimbUp();
        }

        private void StartClimbUp()
        {
            _phase = ClimbPhase.ClimbUp;

            _trigger.OnAnimationEnd -= HandleJumpAnimationEnd;

            _player.transform.SetPositionAndRotation(
                _hangRootPosition,
                _climbRotation);

            _mover.UseRootMotion = true;
            _mover.IgnoreRootMotionCollision = true;

            _trigger.OnAnimationEnd -= HandleAnimationEnd;
            _trigger.OnAnimationEnd += HandleAnimationEnd;

            _renderer.PlayClip(_stateClipHash);
        }

        private void HandleAnimationEnd()
        {
            if (_climbLink == null || _phase != ClimbPhase.ClimbUp)
                return;

            _phase = ClimbPhase.HorizontalCorrection;
            _horizontalCorrectionTimer = 0f;
            _horizontalCorrectionStart = _player.transform.position;
            _horizontalCorrectionTarget = new Vector3(
                _climbEndPosition.x,
                _horizontalCorrectionStart.y,
                _climbEndPosition.z);

            _mover.UseRootMotion = false;
        }

        private void UpdateHorizontalCorrection()
        {
            _horizontalCorrectionTimer += Time.deltaTime;

            float normalizedTime = Mathf.Clamp01(
                _horizontalCorrectionTimer /
                _climbLink.HorizontalCorrectionDuration);
            float easedTime = Mathf.SmoothStep(0f, 1f, normalizedTime);

            Vector3 position = Vector3.Lerp(
                _horizontalCorrectionStart,
                _horizontalCorrectionTarget,
                easedTime);

            _player.transform.SetPositionAndRotation(position, _climbRotation);

            if (normalizedTime < 1f)
                return;

            _player.ChangeState(PlayerStateEnum.IDLE);
        }

        public override void Exit()
        {
            if (_trigger != null)
            {
                _trigger.OnAnimationEnd -= HandleJumpAnimationEnd;
                _trigger.OnAnimationEnd -= HandleAnimationEnd;
            }

            _mover.CanManualMove = true;
            _mover.UseRootMotion = true;
            _mover.UseGravity = true;
            _mover.IgnoreRootMotionCollision = false;
            _player.ClearClimbLink();
            _climbLink = null;

            base.Exit();
        }
    }
}
