using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private List<Hardware> _hardwarePrefabs;

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
            }
        }

        private void Start()
        {
            _rooms = FindObjectsOfType<Room>();
        }

        public Hardware GetHardware()
        {
            int randomIndex = Random.Range(0, _hardwarePrefabs.Count);
            Hardware hw = Instantiate(_hardwarePrefabs[randomIndex]);
            return hw;
        }
    }
}
