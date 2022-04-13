using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;

namespace BootlegPlatformFighter
{
    public class BootlegPlayerInput : MonoBehaviour
    {

        private PlayerInput playerInput;

        public BootlegCharacterController.Controls controls;
        private BootlegCharacterController.Controls previousControls;

        private int playerIndex;

        private InGameMenu inGameMenu;

        private BootlegCharacterController characterController;


        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            playerIndex = playerInput.playerIndex;

        }

        public void OnEnterStage()
        {
            var charControllers = FindObjectsOfType<BootlegCharacterController>();

            characterController = charControllers.FirstOrDefault(c => c.GetPlayerIndex() == playerIndex);
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

        public void OnNavigation(CallbackContext context)
        {

        }

        public void OnEnter(CallbackContext context)
        {

        }
        public void OnExit(CallbackContext context)
        {

        }

        void FixedUpdate()
        {
            if (SceneManager.GetActiveScene().name == "WinterStage")
            {
                OnEnterStage();

                controls.SetStateChangeVariables(previousControls);

                characterController.ProcessUpdate(controls, previousControls);
                previousControls = controls;
            }

            
        }
    }
}