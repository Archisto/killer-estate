using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class Room : MonoBehaviour
    {
        [SerializeField]
        private bool _startingRoom;

        [SerializeField]
        private List<HardwareBase> _hardwareBases;

        [SerializeField]
        private List<Window> _windows;

        private CameraController _camera;
        private HardwareManager _hwManager;

        public HardwareBase VitalHardwareBase { get; private set; }

        public bool StartingRoom { get { return _startingRoom; } }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _camera = FindObjectOfType<CameraController>();
            _hwManager = FindObjectOfType<HardwareManager>();
            InitHardwareBases();
            InitWindows();

            if (StartingRoom)
            {
                Enter();
                VitalHardwareBase.SetHardware(_hwManager.GetRandomHardware(true));
            }
        }

        private void InitHardwareBases()
        {
            if (_hardwareBases.Count > 0)
            {
                foreach (HardwareBase hwBase in _hardwareBases)
                {
                    hwBase.SetRoom(this);
                }

                VitalHardwareBase = _hardwareBases.FirstOrDefault(hwb => hwb.Vital);
                if (VitalHardwareBase == null)
                {
                    Debug.LogError("The Room doesn't have a Vital Hardware Base.");
                }
            }
            else
            {
                Debug.LogError("The Room doesn't any Hardware Bases.");
            }
        }

        private void InitWindows()
        {
            if (_windows.Count > 0)
            {
                foreach (Window window in _windows)
                {
                    window.SetRoom(this);
                }
            }
        }

        public void Enter()
        {
            _camera.GoTo(transform);
            GameManager.Instance.CurrentRoom = this;

            WeaponMouse vitalWeapon =
                VitalHardwareBase.Hardware as WeaponMouse;
            if (vitalWeapon != null &&
                _hwManager.CurrentVitalWeapon != vitalWeapon)
            {
                SetCurrentVitalWeapon(vitalWeapon);
            }
            
            Debug.Log("Entered room " + name);
        }

        private void SetCurrentVitalWeapon(WeaponMouse weapon)
        {
            if (weapon != null)
            {
                _hwManager.CurrentVitalWeapon = weapon;
            }
            else
            {
                Debug.LogError("The Hardware is null or not a weapon.");
            }
        }
    }
}
