using System;
using KimLIb.ModuleSystems;
using Member.KYM.Scripts.CombatSystems.DamageSystems;
using UnityEngine;

namespace Member.KYM.Scripts.Agents
{
    public class HealthModule : MonoBehaviour, IModule, IDamageable
    {

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float NormalizeHealth => maxHealth <= 0 ? 0 : currentHealth / maxHealth;

        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;

        private ModuleOwner _owner;

        public event Action OnDeath;
        public event Action<float, float> OnHealthChanged;
        public event Action<DamageData> OnDamaged;

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

        public void ApplyDamage(DamageData damageData)
        {
            if (damageData.DamageAmount <= 0f)
                return;

            ApplyDamage(damageData.DamageAmount);
            OnDamaged?.Invoke(damageData);
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
