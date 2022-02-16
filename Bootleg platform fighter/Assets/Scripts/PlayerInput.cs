using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class PlayerInput : MonoBehaviour
    {

        public BootlegCharacterController.Controls controls;
        private BootlegCharacterController.Controls previousControls;


        private BootlegCharacterController characterController;
        private Fighting fighting;


        private void Start()
        {
            characterController = GetComponent<BootlegCharacterController>();
            fighting = GetComponent<Fighting>();
        }

        void FixedUpdate()
        {
            controls.horizontalInput = Input.GetAxisRaw("Horizontal");
            controls.verticalInput = Input.GetAxisRaw("Vertical");

            controls.jumpButton = Input.GetButton("Jump");
            controls.airdashButton = Input.GetButton("Airdash");
            controls.normalAttackButton = Input.GetButton("Normal Attack");

            controls.SetStateChangeVariables(previousControls);

            characterController.ProcessUpdate(controls);
            fighting.HandleAttackInput(controls);

            previousControls = controls;
        }
    }
}