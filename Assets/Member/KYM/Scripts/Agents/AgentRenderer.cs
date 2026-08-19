using KimLIb.ModuleSystems;
using UnityEngine;

namespace Member.KYM.Scripts.Agents
{
    public class AgentRenderer : MonoBehaviour, IModule, IAnimateRenderer
    {
        public Animator Animator { get; private set; }
        private IMover _mover;
        private ModuleOwner _owner;
        
        [Header("루트모션 관련")]
        [SerializeField] private bool useRootMotion;
        [SerializeField] private bool useRootMotionRotation;
        public virtual void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _mover = owner.GetModule<IMover>();
            Debug.Assert(_mover != null, "에이전트 렌더러는 무버를 필요로합니다!");
            Animator = GetComponent<Animator>();
        }
        private void OnAnimatorMove()
        {
            if (!useRootMotion)
                return;

            if (useRootMotionRotation)
            {
                _mover.ApplyRootMotion(Animator.deltaPosition, Animator.deltaRotation);
            }
            else
            {
                _mover.ApplyRootMotion(Animator.deltaPosition, Quaternion.identity);
            }
        }

        public void PlayClip(int clipHash, float crossFadeDuration = 0.1f, int layerIndex = 0, float normalizedTime = 0)
        {
            Animator.CrossFadeInFixedTime(clipHash, crossFadeDuration, layerIndex, normalizedTime);
        }
        
    }
}