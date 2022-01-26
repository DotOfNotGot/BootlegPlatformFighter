using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class PlayerInput : MonoBehaviour
    {

        public BootlegCharacterController.Controls controls;

        private BootlegCharacterController characterController;

        private void Start()
        {
            characterController = GetComponent<BootlegCharacterController>();
        }

        void FixedUpdate()
        {

            controls.horizontalInput = Input.GetAxisRaw("Horizontal");
            controls.verticalInput = Input.GetAxisRaw("Vertical");

            controls.jumpButton = Input.GetButton("Jump");

            controls.airdashButton = Input.GetButton("Airdash");

            characterController.ProcessUpdate(controls);
        }
    }
}