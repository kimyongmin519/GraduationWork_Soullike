using Member.KDH._03.Scripts.Item;
using UnityEngine;

namespace Member.KYM.Scripts.CombatSystems.WeaponSystem
{
    [CreateAssetMenu(fileName = "Weapon data", menuName = "KimSO/Combat/Weapon data", order = 20)]
    public class WeaponDataSO : EquipmentItemDataSO
    {
        [field: SerializeField] public string WeaponId { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public string LayerName { get; private set; }
        [field: SerializeField] public WeaponType WeaponType { get; private set; }
        [field: SerializeField] public AbstractWeapon WeaponPrefab { get; private set; }
        [field: SerializeField, Min(0f)] public float AttackPower { get; private set; }
    }
}
