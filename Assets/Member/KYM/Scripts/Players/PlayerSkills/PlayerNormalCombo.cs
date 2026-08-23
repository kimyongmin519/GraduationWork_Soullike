using KimLIb.AnimatorSystems;
using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.CombatSystems;
using UnityEngine;

namespace Member.KYM.Scripts.Players.PlayerSkills
{
    public class PlayerNormalCombo : AbstractPlayerSkill
    {
        [SerializeField] private AnimParamSO[] comboClips;
        
        [SerializeField] private float comboWindow = 0.4f; // 이 시간 안에 다시 입력하면 콤보가 이어진다.
        
        [SerializeField] private float comboInputBufferTime = 0.8f; // 공격 중 미리 입력한 콤보 예약 유지 시간. 0 이하면 현재 공격이 끝날 때까지 유지된다.
        [SerializeField, Min(0f)] private float staminaCost = 15f;

        private AgentTrigger _trigger;
        private StaminaModule _stamina;
        private bool _hasBufferedNextCombo;
        private float _lastBufferedInputTime;
        
        public float _attackSpeed = 1f;
        public int ComboCounter { get; set; } = 0; // 현재 카운터

        public override void InitializeSkill(ISkillModule skillModule)
        {
            base.InitializeSkill(skillModule);
            _trigger = _player.GetModule<AgentTrigger>();
            _stamina = _player.GetModule<StaminaModule>();
            Debug.Assert(_trigger != null, "소드콤보 공격은 AgentTrigger 모듈이 필요합니다.");
            Debug.Assert(_stamina != null, "소드콤보 공격은 StaminaModule이 필요합니다.");
        }

        public override bool CanUseSkill(GameObject target = null)
        {
            if (comboClips == null || comboClips.Length == 0)
                return false;

            if (IsUsing)
                return _stamina == null || _stamina.CanConsume(staminaCost);

            return NormalizedCooldown >= 1f
                   && (_stamina == null || _stamina.CanConsume(staminaCost));
        }

        public override void UseSkill(GameObject target = null)
        {
            if (IsUsing)
            {
                BufferNextComboInput();
                return;
            }

            if (_stamina != null && !_stamina.TryConsume(staminaCost))
                return;

            base.UseSkill(target);

            bool comboCounterOver = ComboCounter >= comboClips.Length;
            bool comboWindowExhausted = Time.time >= _lastUseTime + comboWindow;

            // 콤보 시간이 지났거나 콤보 카운트가 토탈 콤보를 넘어섰다면 0으로 초기화해준다.
            if (comboCounterOver || comboWindowExhausted)
            {
                ComboCounter = 0;
            }
            
            _hasBufferedNextCombo = false;
            _mover.RotateTo(_player.UIInput.GetHorizontalCameraForward());
            _mover.CanManualMove = false;
            PlayCurrentComboClip();
        }
        
        private void PlayCurrentComboClip()
        {
            _renderer.PlayClip(comboClips[ComboCounter].ParamHash);

            _trigger.OnAnimationEnd -= HandleAnimationEnd;
            _trigger.OnAnimationEnd += HandleAnimationEnd;
        }
        
        private void BufferNextComboInput()
        {
            _hasBufferedNextCombo = true;
            _lastBufferedInputTime = Time.time;
        }
        
        private bool HasValidBufferedNextCombo()
        {
            if (!_hasBufferedNextCombo)
                return false;

            return comboInputBufferTime <= 0f || Time.time <= _lastBufferedInputTime + comboInputBufferTime;
        }

        private void HandleDamageCast()
        {
            
        }
        
        private void HandleAnimationEnd()
        {
            if (HasValidBufferedNextCombo())
            {
                if (_stamina != null && !_stamina.TryConsume(staminaCost))
                {
                    StopSkill();
                    return;
                }

                ComboCounter = GetNextComboCounter();
                _hasBufferedNextCombo = false;
                PlayCurrentComboClip();
                return;
            }

            StopSkill();
        }

        public override void StopSkill()
        {
            ComboCounter = GetNextComboCounter();
            _hasBufferedNextCombo = false;
            _mover.CanManualMove = true;
            _trigger.OnAnimationEnd -= HandleAnimationEnd;
            base.StopSkill();
        }

        private int GetNextComboCounter()
        {
            if (comboClips == null || comboClips.Length == 0)
                return 0;

            return (ComboCounter + 1) % comboClips.Length;
        }
    }
}
