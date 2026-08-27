using KimLIb.AnimatorSystems;
using Member.KYM.Scripts.Agents;
using Member.KYM.Scripts.CombatSystems;
using Member.KYM.Scripts.CombatSystems.DamageSystems;
using Member.KYM.Scripts.CombatSystems.WeaponSystem;
using UnityEngine;

namespace Member.KYM.Scripts.Players.PlayerSkills
{
    public class PlayerNormalCombo : AbstractPlayerSkill
    {
        [SerializeField] private AnimParamSO[] comboClips;
        
        [Tooltip("비어 있으면 이 스킬 오브젝트의 자식으로 캐스터를 생성합니다.")]
        [SerializeField] private Transform damageCasterRoot;
        
        [SerializeField] private float comboWindow = 0.4f; // 이 시간 안에 다시 입력하면 콤보가 이어진다.
        
        [SerializeField] private float comboInputBufferTime = 0.8f; // 공격 중 미리 입력한 콤보 예약 유지 시간. 0 이하면 현재 공격이 끝날 때까지 유지된다.
        [SerializeField, Min(0f)] private float staminaCost = 15f;

        private AgentTrigger _trigger;
        private StaminaModule _stamina;
        private PlayerWeaponController _weaponController;
        private AbstractDamageCaster _currentDamageCaster;
        private bool _hasBufferedNextCombo;
        private float _lastBufferedInputTime;
        
        public float _attackSpeed = 1f;
        public int ComboCounter { get; set; } = 0; // 현재 카운터

        public override void InitializeSkill(ISkillModule skillModule)
        {
            base.InitializeSkill(skillModule);
            _trigger = _player.GetModule<AgentTrigger>();
            _stamina = _player.GetModule<StaminaModule>();
            _weaponController = _player.GetModule<PlayerWeaponController>();
            Debug.Assert(_trigger != null, "소드콤보 공격은 AgentTrigger 모듈이 필요합니다.");
            Debug.Assert(_stamina != null, "소드콤보 공격은 StaminaModule이 필요합니다.");
            Debug.Assert(_weaponController != null, "평타 스킬은 PlayerWeaponController 모듈이 필요합니다.");

            if (damageCasterRoot == null)
                damageCasterRoot = transform;

            if (_trigger != null)
                _trigger.OnDamageCastTrigger += HandleDamageCast;

            if (_weaponController != null)
            {
                _weaponController.OnWeaponChanged += HandleWeaponChanged;
                RefreshDamageCaster();
            }
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
            if (!IsUsing
                || _currentDamageCaster == null
                || _weaponController == null)
            {
                return;
            }

            _currentDamageCaster.CastAndApply(
                _weaponController.CurrentWeaponData,
                SkillData);
        }

        public void SetDamageCaster(AbstractDamageCaster casterPrefab)
        {
            if (_currentDamageCaster != null)
            {
                Destroy(_currentDamageCaster.gameObject);
                _currentDamageCaster = null;
            }

            if (casterPrefab == null)
                return;

            _currentDamageCaster = Instantiate(
                casterPrefab,
                damageCasterRoot,
                false);
            _currentDamageCaster.gameObject.SetActive(true);
            _currentDamageCaster.Initialize(_player);
        }

        private void HandleWeaponChanged(WeaponDataSO weaponData)
        {
            RefreshDamageCaster();
        }

        private void RefreshDamageCaster()
        {
            AbstractDamageCaster casterPrefab =
                (_weaponController?.CurrentWeaponInstance as MeleeWeapon)
                ?.NormalAttackCasterPrefab;

            SetDamageCaster(casterPrefab);
        }
        
        private void HandleAnimationEnd()
        {
            if (HasValidBufferedNextCombo())
            {
                if (_stamina.TryConsume(staminaCost))
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

        private void OnDestroy()
        {
            if (_trigger != null)
                _trigger.OnDamageCastTrigger -= HandleDamageCast;

            if (_weaponController != null)
                _weaponController.OnWeaponChanged -= HandleWeaponChanged;
        }
    }
}
