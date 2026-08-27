using Member.KYM.Scripts.CombatSystems.DamageSystems;
using KimLIb.ModuleSystems;
using UnityEngine;

namespace Member.KYM.Scripts.CombatSystems.WeaponSystem
{
    public class MeleeWeapon : AbstractWeapon
    {
        [field: SerializeField]
        public AbstractDamageCaster NormalAttackCasterPrefab { get; private set; }

        public override void InitializeWeapon(ModuleOwner owner)
        {
            base.InitializeWeapon(owner);

            if (NormalAttackCasterPrefab != null)
                NormalAttackCasterPrefab.gameObject.SetActive(false);
        }
    }
}
