using System;
using KimLIb.ModuleSystems;
using UnityEngine;

namespace Member.KYM.Scripts.Agents
{
    public class HealthModule : MonoBehaviour, IModule
    {

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float NormalizeHealth => maxHealth <= 0 ? 0 : currentHealth / maxHealth;

        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;

        private ModuleOwner _owner;

        public event Action OnDeath;
        public event Action<float, float> OnHealthChanged;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            currentHealth = maxHealth;
            NotifyHealthChanged();
        }

        public void ApplyDamage(float damageAmount)
        {
            currentHealth -= damageAmount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                NotifyHealthChanged();
                OnDeath?.Invoke();
                return;
            }

            NotifyHealthChanged();
        }

        public void SetMaxHealth(float maxHealthValue)
        {
            maxHealth = maxHealthValue;
            currentHealth = maxHealthValue;
            NotifyHealthChanged();
        }

        public bool CanDie(float damageAmount)
        {
            return currentHealth - damageAmount <= 0;
        }

        private void NotifyHealthChanged()
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}