using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class Room : MonoBehaviour
    {
        [SerializeField]
        private List<HardwareBase> _hardwareBases;

        [SerializeField]
        private List<Window> _windows;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
        }

        /// <summary>
        /// Updates the object once per frame.
        /// </summary>
        private void Update()
        {
        }
    }
}
