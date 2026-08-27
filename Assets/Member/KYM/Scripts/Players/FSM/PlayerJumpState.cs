using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.Agents.FSM;
using Member.KYM.Scripts.Players.TraversalSystems;
using UnityEngine;

namespace Member.KYM.Scripts.Players.FSM
{
    public class PlayerJumpState : AbstractPlayerState
    {
        private enum JumpPhase
        {
            Alignment,
            Jump
        }

        private JumpLink _jumpLink;
        private JumpPhase _phase;
        private float _phaseTimer;
        private Vector3 _alignmentStartPosition;
        private Quaternion _alignmentStartRotation;
        private Vector3 _jumpStartPosition;
        private Vector3 _jumpEndPosition;
        private Quaternion _jumpRotation;

        private bool _previousCanManualMove;
        private bool _previousUseRootMotion;
        private bool _previousUseGravity;
        private bool _isTraversalActive;

        private PlayerWeaponHolder _weaponHolder;
        private bool _restoreWeaponAfterJump;

        public PlayerJumpState(Agent agent, int stateClipHash)
            : base(agent, stateClipHash)
        {
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            ResetAnimationRigging();
            _isTraversalActive = false;
            _jumpLink = _player.CurrentJumpLink;

            if (_jumpLink == null || !_jumpLink.IsValid)
            {
                Debug.LogError("Traversal jump requires a valid JumpLink.");
                _player.ChangeState(PlayerStateEnum.IDLE);
                return;
            }

            if (!_jumpLink.TryGetJumpPoints(
                    _player.transform.position,
                    out _jumpStartPosition,
                    out _jumpEndPosition,
                    out _jumpRotation))
            {
                Debug.LogError("Traversal jump could not select the next jump pose.");
                _player.ChangeState(PlayerStateEnum.IDLE);
                return;
            }

            _previousCanManualMove = _mover.CanManualMove;
            _previousUseRootMotion = _mover.UseRootMotion;
            _previousUseGravity = _mover.UseGravity;
            _isTraversalActive = true;

            _mover.CanManualMove = false;
            _mover.UseRootMotion = false;
            _mover.UseGravity = false;

            _phase = JumpPhase.Alignment;
            _phaseTimer = 0f;
            _alignmentStartPosition = _player.transform.position;
            _alignmentStartRotation = _player.transform.rotation;

            if (_weaponHolder == null)
                _weaponHolder = _player.GetModule<PlayerWeaponHolder>();

            _restoreWeaponAfterJump = _weaponHolder != null
                                      && _weaponHolder.IsWeaponInHand;

            if (_restoreWeaponAfterJump)
                _weaponHolder.AttachToBack();
        }

        public override void Update()
        {
            base.Update();

            switch (_phase)
            {
                case JumpPhase.Alignment:
                    UpdateAlignment();
                    break;
                case JumpPhase.Jump:
                    UpdateJump();
                    break;
            }
        }

        private void UpdateAlignment()
        {
            _phaseTimer += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(
                _phaseTimer / _jumpLink.AlignmentDuration);
            float easedTime = Mathf.SmoothStep(0f, 1f, normalizedTime);

            Vector3 position = Vector3.Lerp(
                _alignmentStartPosition,
                _jumpStartPosition,
                easedTime);
            Quaternion rotation = Quaternion.Slerp(
                _alignmentStartRotation,
                _jumpRotation,
                easedTime);

            _player.transform.SetPositionAndRotation(position, rotation);

            if (normalizedTime < 1f)
                return;

            _phase = JumpPhase.Jump;
            _phaseTimer = 0f;
            _player.transform.SetPositionAndRotation(
                _jumpStartPosition,
                _jumpRotation);
            _renderer.PlayClip(_jumpLink.JumpAnimationHash);
        }

        private void UpdateJump()
        {
            _phaseTimer += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(
                _phaseTimer / _jumpLink.JumpDuration);
            float easedTime = Mathf.SmoothStep(0f, 1f, normalizedTime);

            Vector3 position = Vector3.Lerp(
                _jumpStartPosition,
                _jumpEndPosition,
                easedTime);
            position.y += _jumpLink.JumpHeightCurve.Evaluate(normalizedTime)
                          * _jumpLink.JumpHeight;

            _player.transform.SetPositionAndRotation(position, _jumpRotation);

            if (normalizedTime >= 1f)
                _player.ChangeState(PlayerStateEnum.IDLE);
        }

        public override void Exit()
        {
            if (_isTraversalActive)
            {
                _mover.CanManualMove = _previousCanManualMove;
                _mover.UseRootMotion = _previousUseRootMotion;
                _mover.UseGravity = _previousUseGravity;

                if (_restoreWeaponAfterJump && _weaponHolder != null)
                    _weaponHolder.AttachToHand();
            }

            _player.ClearJumpLink();
            _jumpLink = null;
            _isTraversalActive = false;
            _restoreWeaponAfterJump = false;
            base.Exit();
        }
    }
}
