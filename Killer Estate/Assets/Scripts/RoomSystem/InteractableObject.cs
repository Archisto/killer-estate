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

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            _mouse = GameManager.Instance.Mouse;
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
            return (Vector3.Distance(position, transform.position) <= _mouseClickRange);
        }

        protected abstract void OnClick();

        protected virtual void OnDrawGizmos()
        {
        }
    }
}
