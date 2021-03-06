﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VitalWeaponInfo = KillerEstate.UI.VitalWeaponInfo;

namespace KillerEstate
{
    public class HardwareBase : InteractableObject
    {
        [Header("HardwareBase Specific")]

        [SerializeField]
        private bool _vital;

        [SerializeField]
        private Vector3 _hardwarePosition;

        public HardwareManager HardwareManager { get; private set; }

        public Hardware Hardware { get; private set; }

        public bool Vital
        {
            get { return _vital; }
        }

        public Vector3 HardwarePosition
        {
            get { return transform.position + _hardwarePosition; }
        }

        public override bool Interactable { get { return Hardware == null; } }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            HardwareManager = FindObjectOfType<HardwareManager>();
        }

        protected override void OnClick()
        {
            if (Hardware == null)
            {
                Hardware hw = HardwareManager.GetRandomHardware(Vital);
                SetHardware(hw);
                InitVitalWeaponInfo();
            }
        }

        public bool SetHardware(Hardware hardware)
        {
            if (hardware == null)
            {
                Hardware = null;
                return false;
            }
            else if (_vital && hardware.Type != HardwareType.Weapon)
            {
                Debug.LogWarning("Can't put Support Hardware on a Vital Base.");
                return false;
            }
            else
            {
                Hardware = hardware;
                Hardware.PlaceOnBase(this);

                // When a weapon is set on a Vital Base,
                // it immediately becomes the current Vital Weapon
                // (Hardware can't be constructed from outside the room)
                if (_vital && hardware.Type == HardwareType.Weapon)
                {
                    if (HardwareManager.CurrentVitalWeapon != null)
                    {
                        HardwareManager.CurrentVitalWeapon.Info.UpdatePortraitColor(false);
                    }
                    HardwareManager.CurrentVitalWeapon = Hardware as WeaponMouse;
                }

                return true;
            }
        }

        public void InitVitalWeaponInfo()
        {
            if (Vital && Hardware != null)
            {
                WeaponMouse weapon = Hardware as WeaponMouse;
                if (weapon != null)
                {
                    VitalWeaponInfo info = GameManager.Instance.UI.ActivateWeaponInfo(weapon);
                    weapon.SetInfoUI(info);
                }
            }
        }

        public void HandleHardwareDestruction(Hardware hardware)
        {
            HardwareManager.HandleHardwareDestruction(hardware, this);
            SetHardware(null);
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (_vital)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position + Vector3.up * 0.1f - Vector3.right,
                    transform.position + Vector3.up * 0.1f + Vector3.right);
                Gizmos.DrawLine(transform.position + Vector3.up * 0.1f - Vector3.forward,
                    transform.position + Vector3.up * 0.1f + Vector3.forward);
            }
        }
    }
}
