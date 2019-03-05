using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public abstract class InteractableObject : LevelObject
    {
        [Header("Interacting")]

        [SerializeField]
        protected float _mouseClickRange = 1f;

        protected MouseController _mouse;
        protected Room _room;
        protected Vector3 _clickAreaCenter;

        public Room Room { get { return _room; } }

        public virtual bool Interactable { get { return false; } }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            _mouse = GameManager.Instance.Mouse;
            _clickAreaCenter = transform.position;
            _clickAreaCenter.y = _mouse.mouseWorldY;
        }

        public virtual void SetRoom(Room room)
        {
            _room = room;
        }

        protected virtual bool IsInCurrentRoom()
        {
            return GameManager.Instance.CurrentRoom == _room;
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (IsInCurrentRoom())
            {
                UpdateInteraction();
                base.UpdateObject();
            }
        }

        protected virtual void UpdateInteraction()
        {
            if (Clicked())
            {
                OnClick();
            }
        }

        protected virtual bool Clicked()
        {
            return (_mouse.LeftButtonReleased
                    && !_mouse.Dragging
                    && WithinClickRange(_mouse.Position));
        }

        protected virtual bool MouseHovering()
        {
            return (!_mouse.LeftButtonDown
                    && WithinClickRange(_mouse.Position));
        }

        protected bool WithinClickRange(Vector3 position)
        {
            return (Vector3.Distance(position, _clickAreaCenter)
                    <= _mouseClickRange);
        }

        protected abstract void OnClick();

        protected virtual void OnDrawGizmos()
        {
            if (Interactable && Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_clickAreaCenter, _mouseClickRange);
            }
        }
    }
}
