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

        private int playerIndex;


        private void Start()
        {
            characterController = GetComponent<BootlegCharacterController>();
            fighting = GetComponent<Fighting>();
            playerIndex = characterController.playerIndex;
        }

        void FixedUpdate()
        {
            controls.horizontalInput = Input.GetAxisRaw("Horizontal_" + playerIndex);
            controls.verticalInput = Input.GetAxisRaw("Vertical_" + playerIndex);

            controls.jumpButton = Input.GetButton("Jump_" + playerIndex);
            controls.airdashButton = Input.GetButton("Airdash_" + playerIndex);
            controls.normalAttackButton = Input.GetButton("Normal Attack_" + playerIndex);

            controls.SetStateChangeVariables(previousControls);

            characterController.ProcessUpdate(controls);
            fighting.HandleAttackInput(controls);

            previousControls = controls;
        }
    }
}