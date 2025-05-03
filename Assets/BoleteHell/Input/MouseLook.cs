using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Input
{
    public class MouseLook : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;

        private void Start()
        {
            this.AssertNotNull(mainCamera);
        }

        private void Update()
        {
            
        }
    }
}