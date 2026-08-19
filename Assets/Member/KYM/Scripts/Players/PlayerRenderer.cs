using Member.KYM.Scripts.Agents;

namespace Member.KYM.Scripts.Players
{
    public class PlayerRenderer : AgentRenderer
    {
        /*[SerializeField] private AnimParamSO isCombatParam;

        private PlayerController _player;
        private float _targetCombatValue;

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
            _player = owner as PlayerController;
            Debug.Assert(_player != null, "플레이어 렌더러는 플레이어 전용 입니다");
        }
        public void AfterInit()
        {
            _player.OnCombatModeChange += HandleCombatModeChange;
        }

        private void OnDestroy()
        {
            _player.OnCombatModeChange -= HandleCombatModeChange;
        }

        private void HandleCombatModeChange(float value)
        {
            _targetCombatValue = value;
        }

        private void Update()
        {
            Animator.SetFloat(isCombatParam.ParamHash, _targetCombatValue, 0.2f, Time.deltaTime);
        }*/
    }
}