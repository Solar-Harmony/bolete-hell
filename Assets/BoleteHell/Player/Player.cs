using System;
using System.Collections.Generic;
using BulletHell.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerControls input;
    [field: SerializeField] public float movementSpeed { get; private set; } = 10;

    
    
}
