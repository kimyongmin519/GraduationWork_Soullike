using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Member.KYM.Scripts.Editor
{
    [InitializeOnLoad]
    public static class WeaponAnimatorLayerSetup
    {
        private const string ControllerPath =
            "Assets/Member/KYM/05.Animations/PlayerController.controller";
        private const string OutputFolder =
            "Assets/Member/KYM/05.Animations/WeaponLayers/Katana";
        private const string GreatSwordLayerName = "GreatSword";
        private const string KatanaLayerName = "Katana";
        private const string GeneratedTreeSuffix = " [Katana Layer]";

        private readonly struct ClipMapping
        {
            public readonly string OriginalPath;
            public readonly string KatanaSourcePath;
            public readonly string OutputName;

            public ClipMapping(
                string originalPath,
                string katanaSourcePath,
                string outputName)
            {
                OriginalPath = originalPath;
                KatanaSourcePath = katanaSourcePath;
                OutputName = outputName;
            }
        }

        private static readonly ClipMapping[] ClipMappings =
        {
            new(
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/1_Movements/1__Idle/PlayerIdle(Re).anim",
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Katana_Blade/1_Movements/1__Idle/M_katana_Blade@Idle_ver_A.FBX",
                "Katana_Idle.anim"),
            new(
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/1_Movements/3__Jogging/B/PlayerWalk(Re).anim",
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Katana_Blade/1_Movements/3__Jogging/A/M_katana_Blade@Jogging_8Way_verA_F.FBX",
                "Katana_Walk.anim"),
            new(
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/1_Movements/8__Dodge/Player_FrontDodge(Re).anim",
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Katana_Blade/1_Movements/8__Dodge/M_katana_Blade@Dodge_Front.FBX",
                "Katana_Dodge_Front.anim"),
            new(
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/1_Movements/8__Dodge/Player_BackDodge(Re).anim",
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Katana_Blade/1_Movements/8__Dodge/M_katana_Blade@Dodge_Back.FBX",
                "Katana_Dodge_Back.anim"),
            new(
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/1_Movements/8__Dodge/Player_LeftDodge(Re).anim",
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Katana_Blade/1_Movements/8__Dodge/M_katana_Blade@Dodge_Left.FBX",
                "Katana_Dodge_Left.anim"),
            new(
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/1_Movements/8__Dodge/Player_RightDodge(Re).anim",
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Katana_Blade/1_Movements/8__Dodge/M_katana_Blade@Dodge_Right.FBX",
                "Katana_Dodge_Right.anim"),
            new(
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/2_Attacks/2__7Combos/PlayerNormalAttack_1(Re).anim",
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Katana_Blade/2_Attacks/2__5Combos/M_katana_Blade@Attack_5Combo_1.FBX",
                "Katana_Attack_1.anim"),
            new(
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/2_Attacks/2__7Combos/PlayerNormalAttack_2(Re).anim",
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Katana_Blade/2_Attacks/2__5Combos/M_katana_Blade@Attack_5Combo_2.FBX",
                "Katana_Attack_2.anim"),
            new(
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/2_Attacks/2__7Combos/PlayerStrongAttack(Re).anim",
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Katana_Blade/2_Attacks/5__Upper_Attack/M_katana_Blade@UpperAttack.FBX",
                "Katana_Strong_Attack.anim"),
            new(
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/3_Skills/PlayerDashAttack(Re).anim",
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Katana_Blade/2_Attacks/3__Dash_Attack/M_katana_Blade@Dash_Attack_ver_A.FBX",
                "Katana_Dash_Attack.anim"),
            new(
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Big_Sword/5_Revenges/Guard_Revenges/PlayerGuard(Re).anim",
                "Assets/Member/KYM/01.Graphics/Grruzam Powerful Sword Animation(Great Sword, Katana)/Animation/M_Katana_Blade/5_Revenges/Guard_Revenges/M_katana_Blade@Revenge_Guard_Loop.FBX",
                "Katana_Guard.anim")
        };

        static WeaponAnimatorLayerSetup()
        {
            EditorApplication.delayCall += SetupIfMissing;
        }

        [MenuItem("Tools/KYM/Setup Weapon Animator Layers")]
        public static void SetupWeaponLayers()
        {
            AnimatorController controller =
                AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);

            if (controller == null)
            {
                Debug.LogError($"Animator controller was not found: {ControllerPath}");
                return;
            }

            RemoveGeneratedLayers(controller);
            RemoveGeneratedBlendTrees(controller);

            Dictionary<Motion, Motion> replacements = CreateKatanaClips();
            AddSyncedLayers(controller, replacements);
            DamageAnimationEventSetup.SetupDamageEvents();

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("GreatSword and Katana synced animator layers were created.");
        }

        private static void SetupIfMissing()
        {
            AnimatorController controller =
                AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);

            if (controller == null)
                return;

            bool hasGreatSword = controller.layers.Any(
                layer => layer.name == GreatSwordLayerName);
            bool hasKatana = controller.layers.Any(
                layer => layer.name == KatanaLayerName);
            bool hasKatanaWalk = AssetDatabase.LoadAssetAtPath<AnimationClip>(
                $"{OutputFolder}/Katana_Walk.anim") != null;

            if (!hasGreatSword || !hasKatana || !hasKatanaWalk)
                SetupWeaponLayers();
        }

        private static void RemoveGeneratedLayers(AnimatorController controller)
        {
            controller.layers = controller.layers
                .Where(layer => layer.name != GreatSwordLayerName
                                && layer.name != KatanaLayerName)
                .ToArray();
        }

        private static void RemoveGeneratedBlendTrees(AnimatorController controller)
        {
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(ControllerPath);

            foreach (BlendTree tree in subAssets
                         .OfType<BlendTree>()
                         .Where(tree => tree.name.EndsWith(GeneratedTreeSuffix)))
            {
                Object.DestroyImmediate(tree, true);
            }
        }

        private static Dictionary<Motion, Motion> CreateKatanaClips()
        {
            Dictionary<Motion, Motion> replacements = new();

            foreach (ClipMapping mapping in ClipMappings)
            {
                AnimationClip original =
                    AssetDatabase.LoadAssetAtPath<AnimationClip>(mapping.OriginalPath);
                AnimationClip source = LoadAnimationClip(mapping.KatanaSourcePath);

                if (original == null || source == null)
                {
                    Debug.LogError(
                        $"Weapon animation mapping is missing. Original: " +
                        $"{mapping.OriginalPath}, Katana: {mapping.KatanaSourcePath}");
                    continue;
                }

                string outputPath = $"{OutputFolder}/{mapping.OutputName}";
                AssetDatabase.DeleteAsset(outputPath);

                AnimationClip katanaClip = Object.Instantiate(source);
                katanaClip.name = System.IO.Path.GetFileNameWithoutExtension(
                    mapping.OutputName);
                AnimationUtility.SetAnimationEvents(
                    katanaClip,
                    AnimationUtility.GetAnimationEvents(original));
                AssetDatabase.CreateAsset(katanaClip, outputPath);
                replacements[original] = katanaClip;
            }

            return replacements;
        }

        private static AnimationClip LoadAnimationClip(string assetPath)
        {
            return AssetDatabase.LoadAllAssetsAtPath(assetPath)
                .OfType<AnimationClip>()
                .FirstOrDefault(clip => !clip.name.StartsWith("__preview__"));
        }

        private static void AddSyncedLayers(
            AnimatorController controller,
            IReadOnlyDictionary<Motion, Motion> replacements)
        {
            AnimatorControllerLayer baseLayer = controller.layers[0];
            AnimatorControllerLayer greatSwordLayer = CreateSyncedLayer(
                GreatSwordLayerName,
                baseLayer);
            AnimatorControllerLayer katanaLayer = CreateSyncedLayer(
                KatanaLayerName,
                baseLayer);

            List<AnimatorControllerLayer> layers = controller.layers.ToList();
            layers.Add(greatSwordLayer);
            layers.Add(katanaLayer);
            controller.layers = layers.ToArray();

            int katanaLayerIndex = controller.layers.Length - 1;
            AnimatorControllerLayer[] updatedLayers = controller.layers;
            AnimatorControllerLayer updatedKatanaLayer =
                updatedLayers[katanaLayerIndex];

            foreach (AnimatorState state in GetStates(baseLayer.stateMachine))
            {
                Motion katanaMotion = CreateKatanaMotion(
                    controller,
                    state.motion,
                    replacements);

                if (katanaMotion != state.motion)
                    updatedKatanaLayer.SetOverrideMotion(state, katanaMotion);
            }

            updatedLayers[katanaLayerIndex] = updatedKatanaLayer;
            controller.layers = updatedLayers;
        }

        private static AnimatorControllerLayer CreateSyncedLayer(
            string name,
            AnimatorControllerLayer baseLayer)
        {
            return new AnimatorControllerLayer
            {
                name = name,
                stateMachine = baseLayer.stateMachine,
                blendingMode = AnimatorLayerBlendingMode.Override,
                defaultWeight = 0f,
                syncedLayerIndex = 0,
                syncedLayerAffectsTiming = true
            };
        }

        private static IEnumerable<AnimatorState> GetStates(
            AnimatorStateMachine stateMachine)
        {
            foreach (ChildAnimatorState childState in stateMachine.states)
                yield return childState.state;

            foreach (ChildAnimatorStateMachine childMachine
                     in stateMachine.stateMachines)
            {
                foreach (AnimatorState state in GetStates(childMachine.stateMachine))
                    yield return state;
            }
        }

        private static Motion CreateKatanaMotion(
            AnimatorController controller,
            Motion source,
            IReadOnlyDictionary<Motion, Motion> replacements)
        {
            if (source == null)
                return null;

            if (replacements.TryGetValue(source, out Motion replacement))
                return replacement;

            if (source is not BlendTree sourceTree)
                return source;

            ChildMotion[] children = sourceTree.children;
            bool hasReplacement = false;

            for (int index = 0; index < children.Length; index++)
            {
                Motion childMotion = CreateKatanaMotion(
                    controller,
                    children[index].motion,
                    replacements);

                if (childMotion == children[index].motion)
                    continue;

                children[index].motion = childMotion;
                hasReplacement = true;
            }

            if (!hasReplacement)
                return sourceTree;

            BlendTree katanaTree = new BlendTree();
            EditorUtility.CopySerialized(sourceTree, katanaTree);
            katanaTree.name = sourceTree.name + GeneratedTreeSuffix;
            katanaTree.children = children;
            AssetDatabase.AddObjectToAsset(katanaTree, controller);
            return katanaTree;
        }
    }
}
