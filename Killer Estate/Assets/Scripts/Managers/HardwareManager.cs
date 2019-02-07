using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KillerEstate
{
    public enum HardwareType
    {
        Weapon,
        Support
    }

    public class HardwareManager : MonoBehaviour
    {
        [SerializeField]
        private List<WeaponMouse> _weaponPrefabs;

        [SerializeField]
        private List<Support> _supportPrefabs;

        [SerializeField]
        private HitScanProjectile _projectilePrefab;

        [SerializeField]
        protected int _maxProjectileCount = 10;

        private List<Hardware> _hardwarePrefabs;
        protected Pool<HitScanProjectile> _projectiles;
        private Room[] _rooms;
        private WeaponMouse _currentVitalWeapon;

        public WeaponMouse CurrentVitalWeapon
        {
            get
            {
                return _currentVitalWeapon;
            }
            set
            {
                _currentVitalWeapon = value;
                Debug.Log("Vital Weapon set: " + value.name);
            }
        }

        private void Start()
        {
            _hardwarePrefabs = new List<Hardware>();
            foreach (Hardware hw in _weaponPrefabs)
            {
                _hardwarePrefabs.Add(hw);
            }
            foreach (Hardware hw in _supportPrefabs)
            {
                _hardwarePrefabs.Add(hw);
            }

            _projectiles = new Pool<HitScanProjectile>
                (_maxProjectileCount, false, _projectilePrefab);
            _rooms = FindObjectsOfType<Room>();
        }

        public Hardware GetRandomHardware(bool mustBeWeapon)
        {
            Hardware hw;
            int randomIndex;
            if (mustBeWeapon)
            {
                randomIndex = Random.Range(0, _weaponPrefabs.Count);
                hw = Instantiate(_weaponPrefabs[randomIndex]);
            }
            else
            {
                randomIndex = Random.Range(0, _hardwarePrefabs.Count);
                hw = Instantiate(_hardwarePrefabs[randomIndex]);
            }

            return hw;
        }

        public HitScanProjectile GetProjectile()
        {
            return _projectiles.GetPooledObject();
        }

        public void HandleHardwareDestruction(Hardware hw, HardwareBase hwBase)
        {
            WeaponMouse weapon = hw as WeaponMouse;
            if (hwBase.Vital && weapon != null)
            {
                if (weapon == CurrentVitalWeapon)
                {
                    GameManager.Instance.EndGame(false);
                }
            }
        }
    }
}
