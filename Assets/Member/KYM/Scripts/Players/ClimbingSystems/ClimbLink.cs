using KimLIb.AnimatorSystems;
using UnityEngine;
using UnityEngine.Serialization;

namespace Member.KYM.Scripts.Players.ClimbingSystems
{
    public class ClimbLink : MonoBehaviour
    {
        [Header("Link Points")]
        [FormerlySerializedAs("startPoint")]
        [SerializeField] private Transform ledgePoint;
        [SerializeField] private Transform endPoint;

        [Header("Jump To Ledge")]
        [SerializeField] private AnimParamSO jumpAnimationParam;
        [SerializeField, Min(0.01f)] private float alignmentDuration = 0.45f;
        [SerializeField, Min(0.01f)] private float horizontalCorrectionDuration = 0.12f;
        [SerializeField, Min(0f)] private float extraJumpHeight = 0.35f;
        [SerializeField] private AnimationCurve jumpHeightCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f));

        [Tooltip("Player root offset from LedgePoint while hanging")]
        [SerializeField] private Vector3 hangRootOffset = new Vector3(0f, -1.4f, -0.35f);

        public bool IsValid => ledgePoint != null && endPoint != null;

        public int JumpAnimationHash => jumpAnimationParam != null
            ? jumpAnimationParam.ParamHash
            : 0;

        public float AlignmentDuration => alignmentDuration;
        public float HorizontalCorrectionDuration =>
            Mathf.Max(0.01f, horizontalCorrectionDuration);
        public float ExtraJumpHeight => extraJumpHeight;
        public AnimationCurve JumpHeightCurve => jumpHeightCurve;

        public float GetLateralPosition(Vector3 playerWorldPosition)
        {
            return ledgePoint.InverseTransformPoint(playerWorldPosition).x;
        }

        public Vector3 GetHangRootPosition(float lateralPosition)
        {
            Vector3 localPosition = hangRootOffset;
            localPosition.x += lateralPosition;
            return ledgePoint.TransformPoint(localPosition);
        }

        public Vector3 GetEndPosition(float lateralPosition)
        {
            Vector3 localPosition = ledgePoint.InverseTransformPoint(endPoint.position);
            localPosition.x = lateralPosition;
            return ledgePoint.TransformPoint(localPosition);
        }

        public Quaternion GetClimbRotation(
            Vector3 hangRootPosition,
            Vector3 climbEndPosition)
        {
            Vector3 climbDirection = climbEndPosition - hangRootPosition;
            climbDirection.y = 0f;

            if (climbDirection.sqrMagnitude <= 0.0001f)
                return ledgePoint.rotation;

            return Quaternion.LookRotation(climbDirection.normalized, Vector3.up);
        }

        private void OnDrawGizmos()
        {
            if (ledgePoint == null || endPoint == null)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(ledgePoint.position, 0.1f);
            Gizmos.DrawSphere(endPoint.position, 0.1f);
            Gizmos.DrawLine(ledgePoint.position, endPoint.position);

            Gizmos.color = Color.yellow;
            Vector3 hangPosition = GetHangRootPosition(0f);
            Vector3 climbEndPosition = GetEndPosition(0f);
            Gizmos.DrawSphere(hangPosition, 0.12f);
            Gizmos.DrawLine(ledgePoint.position, hangPosition);

            Gizmos.color = Color.green;
            Quaternion climbRotation = GetClimbRotation(hangPosition, climbEndPosition);
            Gizmos.DrawRay(hangPosition, climbRotation * Vector3.forward * 0.6f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(
                ledgePoint.position - ledgePoint.right * 1.5f,
                ledgePoint.position + ledgePoint.right * 1.5f);
        }
    }
}
