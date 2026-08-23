using System;
using UnityEngine;

namespace Member.KYM.Scripts.CombatSystems
{
    [Serializable]
    public struct WeaponAnimationClipOverride
    {
        [field: SerializeField] public AnimationClip OriginalClip { get; private set; }
        [field: SerializeField] public AnimationClip OverrideClip { get; private set; }

        public bool IsValid => OriginalClip != null && OverrideClip != null;
    }

    [CreateAssetMenu(
        fileName = "Weapon animation profile",
        menuName = "KimSO/Combat/Weapon animation profile",
        order = 21)]
    public class WeaponAnimationProfileSO : ScriptableObject
    {
        [field: SerializeField] public WeaponType WeaponType { get; private set; }
        [field: SerializeField] public WeaponAnimationClipOverride[] ClipOverrides { get; private set; }
    }
}
