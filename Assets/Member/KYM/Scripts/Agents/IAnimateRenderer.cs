using UnityEngine;

namespace Member.KYM.Scripts.Agents
{
    public interface IAnimateRenderer
    {
        Animator Animator { get; }
        void PlayClip(int clipHash, float crossFadeDuration = 0.1f, int layerIndex = 0, float normalizedTime = 0);
    }
}