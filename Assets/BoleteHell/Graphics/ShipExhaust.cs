using System;
using BoleteHell.Input;
using Input;
using UnityEngine;
using Utils;

namespace BoleteHell.Graphics
{
    public class ShipExhaust : MonoBehaviour
    {
        [SerializeField]
        private PlayerMovement movement;    
        
        [SerializeField]
        private GameObject shipExhaust;

        private void Awake()
        {
            this.AssertNotNull(movement);
            this.AssertNotNull(shipExhaust);
        }

        private void Update()
        {
            shipExhaust.SetActive(movement.IsMoving);
        }
    }
}