using KimLIb.ModuleSystems;
using UnityEngine;

namespace Member.KYM.Scripts.Players
{
    public class PlayerWeaponHolder : MonoBehaviour, IModule
    {
        [SerializeField] private Transform weapon;
        [SerializeField] private Transform backSocket;
        [SerializeField] private Transform handSocket;
        public bool IsWeaponInHand { get; private set; }

        public void Initialize(ModuleOwner owner)
        {
            AttachToBack();
        }


        public void AttachToHand()
        {
            Attach(weapon, handSocket);
            IsWeaponInHand = true;
        }

        public void AttachToBack()
        {
            Attach(weapon, backSocket);
            IsWeaponInHand = false;
        }

        private void Attach( Transform target, Transform socket)
        {
            if (target == null || socket == null)
            {
                Debug.LogError($"Weapon or socket is not assigned on {nameof(PlayerWeaponHolder)}.", this);
                return;
            }

            target.SetParent(socket);
            target.localPosition = Vector3.zero;
            target.localRotation = Quaternion.identity;
        }
    }
}
