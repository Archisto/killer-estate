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

        /// <summary>
        /// Initializes the object.
        /// </summary>
        protected virtual void Start()
        {
            _mouse = GameManager.Instance.Mouse;
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        protected override void UpdateObject()
        {
            if (Clicked())
            {
                OnClick();
            }

            base.UpdateObject();
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
