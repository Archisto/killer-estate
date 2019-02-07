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

        //[SerializeField]
        //private Hardware _hardware; // testing

        private HardwareManager _hwManager;
        private Vector3 _mousePosition;

        public Hardware Hardware { get; private set; }

        public Vector3 HardwarePosition
        {
            get { return transform.position + _hardwarePosition; }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _hwManager = FindObjectOfType<HardwareManager>();
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            UpdateMousePosition();
            if (Clicked() && Hardware == null)
            {
                SetHardware(_hwManager.GetHardware());
            }

            base.UpdateObject();
        }

        protected virtual void UpdateMousePosition()
        {
            _mousePosition = GameManager.Instance.MousePosition;
        }

        protected virtual bool Clicked()
        {
            return (Input.GetMouseButtonDown(0)
                    && MouseWithinClickRange());
        }

        protected bool MouseWithinClickRange()
        {
            return (Vector3.Distance(_mousePosition, transform.position) <= _mouseClickRange);
        }

        public bool SetHardware(Hardware hardware)
        {
            if (hardware == null)
            {
                WeaponMouse weapon = Hardware as WeaponMouse;
                if (_vital && weapon != null && weapon == _hwManager.CurrentVitalWeapon)
                {
                    GameManager.Instance.EndGame(false);
                }
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
                if (_vital && hardware.Type == HardwareType.Weapon)
                {
                    _hwManager.CurrentVitalWeapon = (Hardware as WeaponMouse);
                }
                return true;
            }
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
