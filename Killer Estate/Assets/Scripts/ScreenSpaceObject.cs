using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class ScreenSpaceObject : MonoBehaviour
    {
        [SerializeField]
        private Vector2 screenSpacePos;

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
            screenSpacePos = GameManager.Instance.UI.
                GetScreenSpacePosition(transform.position, Vector3.zero);
        }
    }
}
