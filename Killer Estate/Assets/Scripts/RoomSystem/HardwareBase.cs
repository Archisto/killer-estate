using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class HardwareBase : LevelObject
    {
        [SerializeField]
        private bool _vital;

        [SerializeField]
        private Vector3 _hardwarePosition;

        [SerializeField]
        protected float _mouseClickRange = 1f;

        private MouseController _mouse;

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

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            HardwareManager = FindObjectOfType<HardwareManager>();
            _mouse = GameManager.Instance.Mouse;
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (Clicked() && Hardware == null)
            {
                SetHardware(HardwareManager.GetRandomHardware(Vital));
            }

            base.UpdateObject();
        }

        protected virtual bool Clicked()
        {
            return (_mouse.LeftButtonReleased
                    && MouseWithinClickRange());
        }

        protected bool MouseWithinClickRange()
        {
            return (Vector3.Distance(_mouse.Position, transform.position) <= _mouseClickRange);
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
                    HardwareManager.CurrentVitalWeapon = Hardware as WeaponMouse;
                }

                return true;
            }
        }

        public void HandleHardwareDestruction(Hardware hardware)
        {
            HardwareManager.HandleHardwareDestruction(hardware, this);
            SetHardware(null);
        }

        private void OnDrawGizmos()
        {
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
