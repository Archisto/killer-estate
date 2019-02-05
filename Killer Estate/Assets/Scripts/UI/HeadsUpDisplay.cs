using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KillerEstate.Localization;

namespace KillerEstate.UI
{
    public class HeadsUpDisplay : Screen
    {
        [SerializeField]
        private LocalizedLabel _scoreLabel;

        protected override void Update()
        {
            // TODO
        }

        public void UpdateScore(int score)
        {
            _scoreLabel.FormatString(score);
        }
    }
}
