using KimLIb.ModuleSystems;
using UnityEngine;

namespace Member.KYM.Scripts.CombatSystems.WeaponSystem
{
    public abstract class AbstractWeapon : MonoBehaviour
    {
        [field:SerializeField] public WeaponDataSO WeaponData { get; private set; }
        protected ModuleOwner _owner;

        public virtual void InitializeWeapon(ModuleOwner owner)
        {
            _owner = owner;
        }
    }
}
