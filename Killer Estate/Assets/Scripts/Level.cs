using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KillerEstate
{
    public class Level
    {
        public int Number { get; private set; }
        public string Name { get; private set; }

        public Level(int number, string name)
        {
            Number = number;
            Name = name;
        }

        public override string ToString()
        {
            return Number.ToString();
        }
    }
}
