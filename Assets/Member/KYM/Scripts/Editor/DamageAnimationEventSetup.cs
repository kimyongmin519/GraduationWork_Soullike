using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Member.KYM.Scripts.Editor
{
    [InitializeOnLoad]
    public static class DamageAnimationEventSetup
    {
        private const string DamageEventFunction = "InvokeDamageCastTrigger";
        private const float DamageEventNormalizedTime = 0.45f;

        private static readonly string[] DamageAnimationPaths =
        {
            "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/2_Attacks/2__7Combos/PlayerNormalAttack_1(Re).anim",
            "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/2_Attacks/2__7Combos/PlayerNormalAttack_2(Re).anim",
            "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/2_Attacks/2__7Combos/PlayerStrongAttack(Re).anim",
            "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/3_Skills/PlayerDashAttack(Re).anim",
            "Assets/Member/KYM/05.Animations/WeaponLayers/Katana/Katana_Attack_1.anim",
            "Assets/Member/KYM/05.Animations/WeaponLayers/Katana/Katana_Attack_2.anim",
            "Assets/Member/KYM/05.Animations/WeaponLayers/Katana/Katana_Strong_Attack.anim",
            "Assets/Member/KYM/05.Animations/WeaponLayers/Katana/Katana_Dash_Attack.anim"
        };

        static DamageAnimationEventSetup()
        {
            EditorApplication.delayCall += SetupDamageEvents;
        }

        [MenuItem("Tools/KYM/Setup Damage Animation Events")]
        public static void SetupDamageEvents()
        {
            bool changed = false;

            foreach (string path in DamageAnimationPaths)
            {
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

                if (clip == null)
                    continue;

                AnimationEvent[] currentEvents =
                    AnimationUtility.GetAnimationEvents(clip);
                float eventTime = clip.length * DamageEventNormalizedTime;
                AnimationEvent damageEvent = currentEvents.FirstOrDefault(
                    animationEvent =>
                        animationEvent.functionName == DamageEventFunction);

                if (damageEvent != null
                    && Mathf.Approximately(damageEvent.time, eventTime)
                    && currentEvents.Count(animationEvent =>
                        animationEvent.functionName == DamageEventFunction) == 1)
                {
                    continue;
                }

                List<AnimationEvent> updatedEvents = currentEvents
                    .Where(animationEvent =>
                        animationEvent.functionName != DamageEventFunction)
                    .ToList();
                updatedEvents.Add(new AnimationEvent
                {
                    functionName = DamageEventFunction,
                    time = eventTime
                });
                updatedEvents.Sort((left, right) => left.time.CompareTo(right.time));

                AnimationUtility.SetAnimationEvents(clip, updatedEvents.ToArray());
                EditorUtility.SetDirty(clip);
                changed = true;
            }

            if (changed)
                AssetDatabase.SaveAssets();
        }
    }
}
