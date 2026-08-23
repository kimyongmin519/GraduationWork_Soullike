using UnityEngine;

namespace Member.KYM.Scripts.CombatSystems
{
    [CreateAssetMenu(fileName = "Weapon data", menuName = "KimSO/Combat/Weapon data", order = 20)]
    public class WeaponDataSO : ScriptableObject
    {
        [field: SerializeField] public string WeaponId { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public string LayerName { get; private set; }
        [field: SerializeField] public WeaponType WeaponType { get; private set; }
        [field: SerializeField] public GameObject WeaponPrefab { get; private set; }
    }
}
