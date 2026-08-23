using System;
using System.Threading;
using KimLIb.ModuleSystems;
using UnityEngine;

namespace Member.KYM.Scripts.Agents
{
    public class StaminaModule : MonoBehaviour, IModule
    {
        #region 임시 코드

        [SerializeField] private float maxStamina;
        [SerializeField] private float staminaRegainInterval;
        [SerializeField] private float staminaRegainAmount;

        #endregion
        private ModuleOwner _owner;
        private float _currentStamina;
        private CancellationTokenSource _regainCancellation;

        public event Action<float, float> OnStaminaChanged;

        public float CurrentStamina => _currentStamina;
        public float MaxStamina => maxStamina;
        public float NormalizedStamina => maxStamina <= 0f
            ? 0f
            : _currentStamina / maxStamina;
        public bool IsEmpty => _currentStamina <= 0f;
        
        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            maxStamina = Mathf.Max(0f, maxStamina);
            _currentStamina = maxStamina;
            NotifyStaminaChanged();
        }

        public bool TryConsume(float amount)
        {
            if (amount <= 0f)
                return true;

            if (_currentStamina < amount)
                return false;

            _currentStamina -= amount;
            NotifyStaminaChanged();
            RestartRegain();
            return true;
        }

        public bool CanConsume(float amount)
        {
            return amount <= 0f || _currentStamina >= amount;
        }

        public void SetMaxStamina(float value)
        {
            maxStamina = Mathf.Max(0f, value);
            _currentStamina = Mathf.Clamp(_currentStamina, 0f, maxStamina);
            NotifyStaminaChanged();
        }

        public void RestoreAll()
        {
            _currentStamina = maxStamina;
            NotifyStaminaChanged();
            CancelRegain();
        }

        private void RestartRegain()
        {
            CancelRegain();

            if (_currentStamina >= maxStamina || staminaRegainAmount <= 0f)
                return;

            _regainCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                destroyCancellationToken);

            _ = RegainStaminaAsync(_regainCancellation.Token);
        }

        private async Awaitable RegainStaminaAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (_currentStamina < maxStamina)
                {
                    await Awaitable.WaitForSecondsAsync(
                        Mathf.Max(0f, staminaRegainInterval),
                        cancellationToken);

                    _currentStamina = Mathf.Min(
                        _currentStamina + Mathf.Max(0f, staminaRegainAmount),
                        maxStamina);
                    NotifyStaminaChanged();
                }
            }
            catch (OperationCanceledException)
            {
                // 새로 스태미나를 사용하거나 오브젝트가 파괴되면 정상적으로 취소된다.
            }
        }

        private void CancelRegain()
        {
            if (_regainCancellation == null)
                return;

            _regainCancellation.Cancel();
            _regainCancellation.Dispose();
            _regainCancellation = null;
        }

        private void NotifyStaminaChanged()
        {
            OnStaminaChanged?.Invoke(_currentStamina, maxStamina);
        }

        private void OnDestroy()
        {
            CancelRegain();
        }
    }
}
