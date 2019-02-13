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
        private Transform _cameraPosition;

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
                VitalHardwareBase.InitVitalWeaponInfo();
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
            if (GameManager.Instance.CurrentRoom != null)
            {
                GameManager.Instance.CurrentRoom.Exit();
            }

            GameManager.Instance.CurrentRoom = this;
            _camera.GoTo(_cameraPosition);

            WeaponMouse vitalWeapon =
                VitalHardwareBase.Hardware as WeaponMouse;
            if (vitalWeapon != null)
            {
                if (_hwManager.CurrentVitalWeapon != vitalWeapon)
                {
                    SetCurrentVitalWeapon(vitalWeapon);
                }

                vitalWeapon.Info.ShowSelectionIcon();
            }

            Debug.Log("Entered room " + name);
        }

        public void Exit()
        {
            WeaponMouse vitalWeapon =
                VitalHardwareBase.Hardware as WeaponMouse;
            if (vitalWeapon != null)
            {
                vitalWeapon.Info.HideSideIcon();
            }
        }

        private void SetCurrentVitalWeapon(WeaponMouse weapon)
        {
            if (weapon != null)
            {
                _hwManager.CurrentVitalWeapon.Info.UpdatePortraitColor(false);
                _hwManager.CurrentVitalWeapon = weapon;
                _hwManager.CurrentVitalWeapon.Info.UpdatePortraitColor(true);
            }
            else
            {
                Debug.LogError("The Hardware is null or not a weapon.");
            }
        }
    }
}
