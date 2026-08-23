using KimLIb.AnimatorSystems;
using UnityEngine;

namespace Member.KYM.Scripts.Players.TraversalSystems
{
    public class JumpLink : MonoBehaviour
    {
        [Header("Jump Poses")]
        [Tooltip("플레이어와 가장 가까운 포즈에서 배열의 다음 포즈로 점프합니다.")]
        [SerializeField] private Transform[] jumpPoses;
        [Tooltip("마지막 포즈에서 첫 번째 포즈로 이어지게 합니다.")]
        [SerializeField] private bool loop = true;

        [Header("Alignment")]
        [SerializeField, Min(0.01f)] private float alignmentDuration = 0.12f;

        [Header("Jump")]
        [SerializeField] private AnimParamSO jumpAnimationParam;
        [SerializeField, Min(0.01f)] private float jumpDuration = 0.8f;
        [SerializeField, Min(0f)] private float jumpHeight = 1.2f;
        [SerializeField] private AnimationCurve jumpHeightCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f));

        public bool IsValid => jumpAnimationParam != null
                               && GetValidPoseCount() >= 2;
        public float AlignmentDuration => Mathf.Max(0.01f, alignmentDuration);
        public float JumpDuration => Mathf.Max(0.01f, jumpDuration);
        public float JumpHeight => jumpHeight;
        public AnimationCurve JumpHeightCurve => jumpHeightCurve;
        public int JumpAnimationHash => jumpAnimationParam != null
            ? jumpAnimationParam.ParamHash
            : 0;

        public bool TryGetJumpPoints(
            Vector3 playerPosition,
            out Vector3 startPosition,
            out Vector3 endPosition,
            out Quaternion jumpRotation)
        {
            startPosition = default;
            endPosition = default;
            jumpRotation = Quaternion.identity;

            if (!IsValid)
                return false;

            int startIndex = GetNearestPoseIndex(playerPosition);
            int endIndex = GetNextValidPoseIndex(startIndex);

            if (startIndex < 0 || endIndex < 0 || startIndex == endIndex)
                return false;

            Transform startPose = jumpPoses[startIndex];
            Vector3 playerLocalPosition = transform.InverseTransformPoint(playerPosition);
            Vector3 startLocalPosition = transform.InverseTransformPoint(startPose.position);
            Vector3 endLocalPosition = transform.InverseTransformPoint(
                jumpPoses[endIndex].position);

            // 링크의 진행 방향(Y/Z)은 포즈가 결정하고,
            // 좌우(X)는 플레이어가 상호작용한 위치를 그대로 유지한다.
            startLocalPosition.x = playerLocalPosition.x;
            endLocalPosition.x = playerLocalPosition.x;

            startPosition = transform.TransformPoint(startLocalPosition);
            endPosition = transform.TransformPoint(endLocalPosition);

            Vector3 direction = endPosition - startPosition;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
                jumpRotation = startPose.rotation;
            else
                jumpRotation = Quaternion.LookRotation(
                    direction.normalized,
                    Vector3.up);

            return true;
        }

        private int GetNearestPoseIndex(Vector3 playerPosition)
        {
            int nearestIndex = -1;
            float nearestDistance = float.MaxValue;
            Vector3 playerLocalPosition = transform.InverseTransformPoint(playerPosition);

            for (int i = 0; i < jumpPoses.Length; i++)
            {
                if (jumpPoses[i] == null)
                    continue;

                Vector3 poseLocalPosition = transform.InverseTransformPoint(
                    jumpPoses[i].position);
                Vector3 distanceToPose = poseLocalPosition - playerLocalPosition;
                distanceToPose.x = 0f;
                float distance = distanceToPose.sqrMagnitude;

                if (distance >= nearestDistance)
                    continue;

                nearestDistance = distance;
                nearestIndex = i;
            }

            return nearestIndex;
        }

        private int GetNextValidPoseIndex(int currentIndex)
        {
            if (currentIndex < 0 || jumpPoses == null)
                return -1;

            for (int offset = 1; offset < jumpPoses.Length; offset++)
            {
                int targetIndex = currentIndex + offset;

                if (targetIndex >= jumpPoses.Length)
                {
                    if (!loop)
                        return -1;

                    targetIndex %= jumpPoses.Length;
                }

                if (jumpPoses[targetIndex] != null)
                    return targetIndex;
            }

            return -1;
        }

        private int GetValidPoseCount()
        {
            if (jumpPoses == null)
                return 0;

            int count = 0;

            for (int i = 0; i < jumpPoses.Length; i++)
            {
                if (jumpPoses[i] != null)
                    count++;
            }

            return count;
        }

        private void OnDrawGizmos()
        {
            if (jumpPoses == null || jumpPoses.Length == 0)
                return;

            for (int i = 0; i < jumpPoses.Length; i++)
            {
                if (jumpPoses[i] == null)
                    continue;

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(jumpPoses[i].position, 0.12f);

                int nextIndex = GetNextValidPoseIndex(i);

                if (nextIndex >= 0 && nextIndex != i)
                    DrawJumpArc(jumpPoses[i].position, jumpPoses[nextIndex].position);
            }
        }

        private void DrawJumpArc(Vector3 startPosition, Vector3 endPosition)
        {
            const int segmentCount = 20;
            Vector3 previous = startPosition;

            Gizmos.color = Color.cyan;
            for (int i = 1; i <= segmentCount; i++)
            {
                float t = i / (float)segmentCount;
                Vector3 current = Vector3.Lerp(startPosition, endPosition, t);
                current.y += jumpHeightCurve.Evaluate(t) * jumpHeight;
                Gizmos.DrawLine(previous, current);
                previous = current;
            }
        }
    }
}
