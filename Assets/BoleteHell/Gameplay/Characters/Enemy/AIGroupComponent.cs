using System;
using UnityEngine;

namespace BoleteHell.Gameplay.Characters.Enemy
{
    public class AIGroupComponent : MonoBehaviour
    {
        [NonSerialized]
        public int GroupID = -1;
        
        [NonSerialized]
        public GameObject TargetOverride;
    }
}