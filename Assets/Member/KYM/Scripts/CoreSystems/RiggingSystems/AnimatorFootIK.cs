using UnityEngine;

namespace Member.KYM.Scripts.CoreSystems.RiggingSystems
{
    public class AnimatorFootIK : MonoBehaviour
    {
        [Header("Ground")]
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private float rayUpOffSet = 0.5f;
        [SerializeField] private float rayLength = 1.5f;
        [SerializeField] private float footHeight = 0.12f; //발목의 위치, 발이 파묻히는 일이 없도록

        [Header("weights")] [Range(0, 1f), SerializeField]
        private float masterWeight = 1f;

        [SerializeField] private string leftParam = "IKLeftFoot";
        [SerializeField] private string rightParam = "IKRightFoot";
        
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null) return;
            
            SolveFoot(AvatarIKGoal.LeftFoot, leftParam);
            SolveFoot(AvatarIKGoal.RightFoot, rightParam);
        }

        private void SolveFoot(AvatarIKGoal goal, string weightParam)
        {
            Vector3 animPos = _animator.GetIKPosition(goal);
            Quaternion animRot = _animator.GetIKRotation(goal);

            float w = masterWeight * GetParam(weightParam, 0f);
            if (w <= 0)
            {
                _animator.SetIKPositionWeight(goal, 0f);
                _animator.SetIKRotationWeight(goal, 0f);
                return;
            }

            Vector3 origin = animPos + Vector3.up * rayUpOffSet;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit,
                    rayUpOffSet + rayLength, groundMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 target = new Vector3(animPos.x, hit.point.y + footHeight, animPos.z);
                Quaternion aligend = Quaternion.FromToRotation(Vector3.up, hit.normal) * animRot;
                
                _animator.SetIKRotation(goal, aligend);
                _animator.SetIKRotationWeight(goal, w);
                
                _animator.SetIKPosition(goal, target);
                _animator.SetIKPositionWeight(goal, w);
            }
            else
            {
                _animator.SetIKPositionWeight(goal, 0f);
                _animator.SetIKRotationWeight(goal, 0f);
            }
        }

        private float GetParam(string weightParam, float fallback)
        {
            if (string.IsNullOrEmpty(weightParam)) return fallback;

            foreach (AnimatorControllerParameter param in _animator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Float && param.name == weightParam)
                    return _animator.GetFloat(param.name);
            }
            
            return fallback;
        }
    }
}