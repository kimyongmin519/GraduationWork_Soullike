using System;
using System.Linq;
using KimLIb.ModuleSystems;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Member.KYM.Scripts.Players
{
    public class PlayerAnimationRiggingController : MonoBehaviour,
        IModule
    {
        [Tooltip("가중치를 0으로 초기화할 Animation Rigging Constraint 컴포넌트들")]
        [SerializeField] private MonoBehaviour[] rigConstraints;
        [SerializeField] private Transform[] rigTargets;
        [SerializeField] private bool resetTargetTransforms = true;

        private LocalPose[] _initialTargetPoses = Array.Empty<LocalPose>();

        public void Initialize(ModuleOwner owner)
        {
            PlayerController player = owner as PlayerController;
            Debug.Assert(
                player != null,
                $"{nameof(PlayerAnimationRiggingController)} is player-only.");

            if (rigConstraints == null || rigConstraints.Length == 0)
            {
                rigConstraints = owner
                    .GetComponentsInChildren<MonoBehaviour>(true)
                    .Where(component => component is IRigConstraint)
                    .ToArray();
            }

            if (rigTargets == null)
                rigTargets = Array.Empty<Transform>();

            CacheInitialTargetPoses();
            ResetRigging();
        }

        public void ResetRigging()
        {
            ResetConstraintWeights();

            if (resetTargetTransforms)
                ResetTargetTransforms();
        }

        private void ResetConstraintWeights()
        {
            if (rigConstraints == null)
                return;

            foreach (MonoBehaviour component in rigConstraints)
            {
                if (component is IRigConstraint constraint)
                    constraint.weight = 0f;
            }
        }

        private void CacheInitialTargetPoses()
        {
            _initialTargetPoses = new LocalPose[rigTargets.Length];

            for (int index = 0; index < rigTargets.Length; index++)
            {
                Transform target = rigTargets[index];

                if (target == null)
                    continue;

                _initialTargetPoses[index] = new LocalPose(
                    target.localPosition,
                    target.localRotation);
            }
        }

        private void ResetTargetTransforms()
        {
            int targetCount = Mathf.Min(
                rigTargets.Length,
                _initialTargetPoses.Length);

            for (int index = 0; index < targetCount; index++)
            {
                Transform target = rigTargets[index];

                if (target == null)
                    continue;

                LocalPose initialPose = _initialTargetPoses[index];
                target.SetLocalPositionAndRotation(
                    initialPose.Position,
                    initialPose.Rotation);
            }
        }

        private readonly struct LocalPose
        {
            public Vector3 Position { get; }
            public Quaternion Rotation { get; }

            public LocalPose(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }
    }
}
