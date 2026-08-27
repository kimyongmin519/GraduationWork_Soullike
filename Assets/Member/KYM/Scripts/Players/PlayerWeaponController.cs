using System;
using KimLIb.ModuleSystems;
using Member.KYM.Scripts.CombatSystems;
using Member.KYM.Scripts.CombatSystems.DamageSystems;
using Member.KYM.Scripts.CombatSystems.WeaponSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Member.KYM.Scripts.Players
{
    public class PlayerWeaponController : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField] private WeaponDataSO[] testLoadout;
        [SerializeField, Min(0)] private int defaultWeaponIndex;
        
        [Tooltip("인벤토리 연결 전 테스트용입니다. Tab을 누르면 다음 무기로 교체합니다.")]
        [SerializeField] private bool enableTemporaryKeyboardInput = true;

        public event Action<WeaponDataSO> OnWeaponChanged;

        public WeaponDataSO CurrentWeaponData { get; private set; }
        public AbstractWeapon CurrentWeaponInstance { get; private set; }
        public int CurrentWeaponIndex { get; private set; } = -1;

        private ModuleOwner _owner;
        private PlayerWeaponHolder _weaponHolder;

        public void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _weaponHolder = owner.GetModule<PlayerWeaponHolder>();
            Debug.Assert(
                _weaponHolder != null,
                "PlayerWeaponController requires PlayerWeaponHolder.");
        }

        public void AfterInit()
        {
            if (testLoadout == null || testLoadout.Length == 0)
                return;

            int initialIndex = Mathf.Clamp(
                defaultWeaponIndex,
                0,
                testLoadout.Length - 1);
            EquipWeapon(initialIndex);
        }

        private void Update()
        {
            if (!enableTemporaryKeyboardInput
                || Keyboard.current == null
                || !Keyboard.current.tabKey.wasPressedThisFrame)
            {
                return;
            }

            EquipNextWeapon();
        }

        public bool EquipNextWeapon()
        {
            if (testLoadout == null || testLoadout.Length == 0)
                return false;

            int startIndex = CurrentWeaponIndex;

            for (int offset = 1; offset <= testLoadout.Length; offset++)
            {
                int nextIndex = (startIndex + offset + testLoadout.Length)
                                % testLoadout.Length;

                if (testLoadout[nextIndex] != null)
                    return EquipWeapon(nextIndex);
            }

            return false;
        }

        public bool EquipWeapon(int loadoutIndex)
        {
            if (testLoadout == null
                || loadoutIndex < 0
                || loadoutIndex >= testLoadout.Length)
            {
                return false;
            }

            WeaponDataSO weaponData = testLoadout[loadoutIndex];

            if (!EquipWeapon(weaponData))
                return false;

            CurrentWeaponIndex = loadoutIndex;
            return true;
        }

        public bool EquipWeapon(WeaponDataSO weaponData)
        {
            if (_weaponHolder == null || weaponData == null || weaponData.WeaponPrefab == null)
            {
                return false;
            }

            if (CurrentWeaponData == weaponData && CurrentWeaponInstance != null)
                return true;

            AbstractWeapon newWeapon = Instantiate(weaponData.WeaponPrefab);
            newWeapon.name = weaponData.DisplayName;
            newWeapon.InitializeWeapon(_owner);

            Transform previousWeapon = _weaponHolder.CurrentWeapon;
            _weaponHolder.SetWeapon(newWeapon.transform);

            CurrentWeaponData = weaponData;
            CurrentWeaponInstance = newWeapon;

            if (previousWeapon != null && previousWeapon != newWeapon.transform)
            {
                previousWeapon.gameObject.SetActive(false);
                Destroy(previousWeapon.gameObject);
            }

            OnWeaponChanged?.Invoke(weaponData);
            return true;
        }
    }
}
