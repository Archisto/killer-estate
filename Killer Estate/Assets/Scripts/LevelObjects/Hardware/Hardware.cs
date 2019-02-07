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

        protected MouseController _mouse;
        protected HardwareBase _base;
        protected bool _mouseLeftButtonHeld;
        protected float _mouseDist;
        protected bool _active;
        protected int _ammo;
        protected bool _needsAmmo;

        private float _charge;

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
            _mouse = GameManager.Instance.Mouse;
        }

        public virtual void PlaceOnBase(HardwareBase hardwareBase)
        {
            _base = hardwareBase;
            transform.position = hardwareBase.HardwarePosition;
        }

        protected bool UpdateMouse()
        {
            bool mouseHoveredOnWeapon =
                (!_mouseLeftButtonHeld && MouseWithinClickRange());
            CheckIfMouseLeftButtonDown();
            return mouseHoveredOnWeapon;
        }

        protected virtual void CheckIfMouseLeftButtonDown()
        {
            if (_mouse.LeftButtonDown)
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
            if (!_mouseLeftButtonHeld)
            {
                _mouseLeftButtonHeld = true;
            }
        }

        protected virtual void OnRightMouseButtonDown()
        {
        }

        protected virtual bool ReleaseMouse(bool tryToFire)
        {
            _mouseLeftButtonHeld = false;
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
            return (Vector3.Distance(_mouse.Position, _clickAreaCenter.position) <= _mouseClickRange);
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

            //Health -= amount;
            if (Health <= 0)
            {
                Debug.Log(name + " was destroyed!");
                DestroyObject();
            }
            //else
            //{
            //    Debug.Log(name + " took " + amount + " damage");
            //}
        }

        public override void DestroyObject()
        {
            _base.HandleHardwareDestruction(this);
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
