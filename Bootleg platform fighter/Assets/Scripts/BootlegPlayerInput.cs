using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace BootlegPlatformFighter
{
    public class BootlegPlayerInput : MonoBehaviour
    {

        private PlayerInput playerInput;

        public BootlegCharacterController.Controls controls;
        private BootlegCharacterController.Controls previousControls;

        private int playerIndex;

        private BootlegCharacterController characterController;



        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            playerIndex = playerInput.playerIndex;
            var charControllers = FindObjectsOfType<BootlegCharacterController>();

            characterController = charControllers.FirstOrDefault(c => c.GetPlayerIndex() == playerIndex);

            //characterController = GetComponentInParent<BootlegCharacterController>();
        }

        public void OnMove(CallbackContext context)
        {
            controls.movementHorizontalInput = context.ReadValue<Vector2>().x;
            controls.movementVerticalInput = context.ReadValue<Vector2>().y;
        }
        public void OnNormalAttack(CallbackContext context)
        {
            controls.normalAttackButton = context.ReadValueAsButton();
        }
        public void OnSpecialAttack(CallbackContext context)
        {
            controls.specialAttackButton = context.ReadValueAsButton();
        }
        public void OnJump(CallbackContext context)
        {
            controls.jumpButton = context.ReadValueAsButton();
        }
        public void OnAirdashBlock(CallbackContext context)
        {
            controls.airdashButton = context.ReadValueAsButton();
        }
        public void OnPause(CallbackContext context)
        {

        }

        private void Update()
        {
            //controls.movementHorizontalInput = Input.GetAxisRaw("Movement_Horizontal_" + playerIndex);
            //controls.movementVerticalInput = Input.GetAxisRaw("Movement_Vertical_" + playerIndex);

            //controls.macroHorizontalInput = Input.GetAxisRaw("Macro_Horizontal_" + playerIndex);
            //controls.macroVerticalInput = Input.GetAxisRaw("Macro_Vertical_" + playerIndex);
            //controls.jumpButton = Input.GetButton("Jump_" + playerIndex);
            //controls.airdashButton = Input.GetButton("AirDash_&_Block_" + playerIndex);
            //controls.normalAttackButton = Input.GetButton("Normal_Attack_" + playerIndex);
            //controls.specialAttackButton = Input.GetButton("Special_Attack_" + playerIndex);
            //controls.grabButton = Input.GetButton("Grab_" + playerIndex);
        }

        void FixedUpdate()
        {
            controls.SetStateChangeVariables(previousControls);

            characterController.ProcessUpdate(controls, previousControls);
            previousControls = controls;
        }
    }
}