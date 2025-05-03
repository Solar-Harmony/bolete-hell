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
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            // mouseWorldPos.z = 0;

            Vector3 dir = mouseWorldPos - transform.position;
            dir.z = 0;
            dir.Normalize();
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}