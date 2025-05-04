using BoleteHell.Input;
using BoleteHell.Utils;
using UnityEngine;

namespace BoleteHell.Graphics
{
    public class ShipExhaust : MonoBehaviour
    {
        [SerializeField]
        private PlayerMovement movement;    
        
        [SerializeField]
        private GameObject shipExhaust;
        
        private void Update()
        {
            shipExhaust.SetActive(movement.IsMoving);
        }
    }
}