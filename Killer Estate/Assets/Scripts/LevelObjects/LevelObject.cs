using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    /// <summary>
    /// A base class for all level objects. Controls their basic behaviour.
    /// </summary>
    public abstract class LevelObject : MonoBehaviour
    {
        // Actions to which any object can listen
        public event Action ObjectUpdated;
        public event Action ObjectDestroyed;
        public event Action ObjectReset;

        protected Vector3 _defaultPosition;

        public bool Destroyed { get; protected set; }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            if (!GameManager.Instance.GamePaused)
            {
                UpdateObject();
            }
        }

        /// <summary>
        /// Updates the object once per frame when the game is not paused.
        /// </summary>
        protected virtual void UpdateObject()
        {
            if (ObjectUpdated != null)
            {
                ObjectUpdated();
            }
        }

        /// <summary>
        /// Destroys the object.
        /// </summary>
        public virtual void DestroyObject()
        {
            if (ObjectDestroyed != null)
            {
                ObjectDestroyed();
            }
        }

        /// <summary>
        /// If <see cref="DestroyObject"/> already handles other
        /// important tasks, this can be called to tell other objects
        /// that the object was destroyed.
        /// </summary>
        protected void RaiseObjectDestroyed()
        {
            if (ObjectDestroyed != null)
            {
                ObjectDestroyed();
            }
        }

        /// <summary>
        /// Resets the object to its default state.
        /// </summary>
        public virtual void ResetObject()
        {
            if (ObjectReset != null)
            {
                ObjectReset();
            }
        }

        /// <summary>
        /// Sets the object to its self-defined default position.
        /// </summary>
        public void SetToDefaultPosition()
        {
            transform.position = _defaultPosition;
        }
    }
}
