using KimLIb.ModuleSystems;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Member.KYM.Scripts.Players
{
    public enum IKType
    {
        RightArm,
        LeftArm,
    }
    public class PlayerRiggingController : MonoBehaviour, IModule
    {
        private PlayerController _player;

        [Header("오른팔 IK")]
        [SerializeField] private TwoBoneIKConstraint rightArmIK;
        public void Initialize(ModuleOwner owner)
        {
            _player = owner as PlayerController;
        }

        public void SetIKWeight(IKType type, float weight)
        {
            switch (type)
            {
                case IKType.RightArm:
                    rightArmIK.weight = weight;
                    break;
                case IKType.LeftArm:
                    break;
            }
        }
    }
}
