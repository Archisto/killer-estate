using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public abstract class Hardware : LevelObject
    {
        [Header("Hardware Common")]

        [SerializeField]
        protected HardwareType _type;

        [SerializeField]
        protected int _maxHealth = 100;

        [SerializeField]
        protected GameObject _rotatingObj;

        [SerializeField]
        protected Transform _clickAreaCenter;

        [SerializeField]
        protected float _mouseClickRange = 1f;

        [SerializeField]
        protected float _minMouseDragDist = 1f;

        [SerializeField]
        protected float _maxMouseDragDist = 3f;

        [SerializeField, Range(0f, 1f)]
        protected float _chargeFillAmount = 0.1f;

        [SerializeField, Range(0f, 1f)]
        protected float _chargeDepletionAmount = 0f;

        protected Vector3 _mousePosition;
        protected Vector3 _mousePositionOnButtonDown;
        protected float _mouseDist;
        protected bool _active;
        protected bool _mouseButtonHeld;

        private float _charge;
        private HardwareBase _base;

        public HardwareType Type { get { return _type; } }

        public virtual int Health { get; protected set; }

        public float Charge
        {
            get
            {
                return _charge;
            }
            set
            {
                _charge = Mathf.Clamp01(value);
            }
        }

        protected virtual void Start()
        {
            Health = _maxHealth;
        }

        public virtual void PlaceOnBase(HardwareBase hardwareBase)
        {
            _base = hardwareBase;
            transform.position = hardwareBase.HardwarePosition;
        }

        protected bool UpdateMouse()
        {
            UpdateMousePosition();
            bool mouseHoveredOnWeapon =
                (!_mouseButtonHeld && MouseWithinClickRange());
            CheckIfMouseLeftButtonDown();
            return mouseHoveredOnWeapon;
        }

        protected virtual void UpdateMousePosition()
        {
            _mousePosition = GameManager.Instance.MousePosition;
        }

        protected virtual void CheckIfMouseLeftButtonDown()
        {
            if (Input.GetMouseButton(0))
            {
                OnLeftMouseButtonDown();
            }
            else
            {
                ReleaseMouse(false);
            }
        }

        protected virtual void OnLeftMouseButtonDown()
        {
            if (!_mouseButtonHeld)
            {
                _mouseButtonHeld = true;
                _mousePositionOnButtonDown = _mousePosition;
            }
        }

        protected virtual void OnRightMouseButtonDown()
        {
        }

        protected virtual bool ReleaseMouse(bool tryToFire)
        {
            _mouseButtonHeld = false;
            if (_active)
            {
                _active = false;

                if (tryToFire && MouseReachesMinDragRange())
                {
                    return true;
                }
            }

            return false;
        }

        protected bool MouseWithinClickRange()
        {
            return (Vector3.Distance(_mousePosition, _clickAreaCenter.position) <= _mouseClickRange);
        }

        protected bool MouseReachesMinDragRange()
        {
            return (_mouseDist >= _minMouseDragDist);
        }

        protected bool MouseOutsideDragRange()
        {
            return (_mouseDist > _maxMouseDragDist);
        }

        protected float FillCharge(bool full)
        {
            if (full)
            {
                Charge = 1f;
            }
            else
            {
                Charge += _chargeFillAmount;
            }

            return Charge;
        }

        protected float DepleteCharge(bool empty)
        {
            if (empty)
            {
                Charge = 0f;
            }
            else
            {
                Charge -= _chargeDepletionAmount;
            }

            return Charge;
        }

        public void TakeDamage(int amount)
        {
            if (Health == 0)
            {
                return;
            }

            Health -= amount;
            if (Health <= 0)
            {
                Debug.Log(name + " was destroyed!");
                DestroyObject();
            }
            else
            {
                Debug.Log(name + " took " + amount + " damage");
            }
        }

        public override void DestroyObject()
        {
            // TODO
            _base.SetHardware(null);
            Destroy(gameObject);
            base.DestroyObject();
        }

        public override void ResetObject()
        {
            Health = _maxHealth;
            base.ResetObject();
        }
    }
}
