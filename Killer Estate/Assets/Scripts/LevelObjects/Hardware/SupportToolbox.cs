using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class SupportToolbox : Support
    {
        [SerializeField]
        private float _windowBoardingTimeFactor = 0.8f;

        public float WindowBoardingTimeFactor
        {
            get { return _windowBoardingTimeFactor; }
        }

        protected override bool CanUseEffect()
        {
            return false;
        }

        protected override void UseEffect()
        {
        }
    }
}
