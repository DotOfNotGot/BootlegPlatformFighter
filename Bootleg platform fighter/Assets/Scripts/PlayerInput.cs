using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class PlayerInput : MonoBehaviour
    {

        public BootlegCharacterController.Controls controls;

        private BootlegCharacterController characterController;

        private int playerIndex;


        private void Start()
        {
            characterController = GetComponent<BootlegCharacterController>();
            playerIndex = characterController.playerIndex;
        }

        void FixedUpdate()
        {
            controls.horizontalInput = Input.GetAxisRaw("Horizontal_" + playerIndex);
            controls.verticalInput = Input.GetAxisRaw("Vertical_" + playerIndex);

            controls.jumpButton = Input.GetButton("Jump_" + playerIndex);

            controls.airdashButton = Input.GetButton("Airdash_" + playerIndex);
            // For controllers
            controls.airdashAxis = Input.GetAxisRaw("Airdash_" + playerIndex);
            characterController.ProcessUpdate(controls);
        }
    }
}