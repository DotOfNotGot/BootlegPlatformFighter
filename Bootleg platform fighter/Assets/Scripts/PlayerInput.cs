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
            controls.movementHorizontalInput = Input.GetAxisRaw("Movement_Horizontal_" + playerIndex);
            controls.movementVerticalInput = Input.GetAxisRaw("Movement_Vertical_" + playerIndex);
            controls.macroHorizontalInput = Input.GetAxisRaw("Macro_Horizontal_" + playerIndex);
            controls.macroVerticalInput = Input.GetAxisRaw("Macro_Vertical_" + playerIndex);
            controls.jumpButton = Input.GetButton("Jump_" + playerIndex);
            controls.airdashButton = Input.GetButton("AirDash_&_Block_" + playerIndex);
            controls.normalAttackButton = Input.GetButton("Normal_Attack_" + playerIndex);
            controls.specialAttackButton = Input.GetButton("Special_Attack_" + playerIndex);
            controls.grabButton = Input.GetButton("Grab_" + playerIndex);


            controls.SetStateChangeVariables(previousControls);

            characterController.ProcessUpdate(controls, previousControls);
            previousControls = controls;
        }
    }
}