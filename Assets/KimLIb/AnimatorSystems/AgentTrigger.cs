using System;
using KimLIb.ModuleSystems;
using UnityEngine;

namespace KimLIb.AnimatorSystems
{
    public class AgentTrigger : MonoBehaviour, IModule
    {
        public event Action OnAnimationEnd;
        public event Action OnAnimationSpecialTrigger;
        public event Action OnDamageCastTrigger;
        private ModuleOwner _owner;
        private int _lastDamageCastFrame = -1;
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
        }
        
        public void InvokeAnimationEnd() => OnAnimationEnd?.Invoke();
        public void InvokeSpecialTrigger() => OnAnimationSpecialTrigger?.Invoke();

        public void InvokeDamageCastTrigger()
        {
            if (_lastDamageCastFrame == Time.frameCount)
                return;

            _lastDamageCastFrame = Time.frameCount;
            OnDamageCastTrigger?.Invoke();
        }
    }
}
