using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


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

        private void ZeroControls()
        {
            controls.airdashButton = false;
            controls.jumpButton = false;
            controls.movementHorizontalInput = 0f;
            controls.movementVerticalInput = 0f;
            controls.normalAttackButton = false;
            controls.specialAttackButton = false;
            controls.airdashButton = false;
        }

        public void OnEnterStage()
        {
            var charControllers = FindObjectsOfType<BootlegCharacterController>();

            characterController = charControllers.FirstOrDefault(c => c.GetPlayerIndex() == playerIndex);
        }

        public void OnMove(CallbackContext context)
        {
            if (GameManagerData.GamePaused)
            {
                ZeroControls();
                return;
            }
            controls.movementHorizontalInput = context.ReadValue<Vector2>().x;
            controls.movementVerticalInput = context.ReadValue<Vector2>().y;
        }
        public void OnNormalAttack(CallbackContext context)
        {
            if (GameManagerData.GamePaused)
            {
                ZeroControls();
                return;
            }
            controls.normalAttackButton = context.ReadValueAsButton();
        }
        public void OnSpecialAttack(CallbackContext context)
        {
            if (GameManagerData.GamePaused)
            {
                ZeroControls();
                return;
            }
            controls.specialAttackButton = context.ReadValueAsButton();
        }
        public void OnJump(CallbackContext context)
        {
            if (GameManagerData.GamePaused)
            {
                ZeroControls();
                return;
            }
            controls.jumpButton = context.ReadValueAsButton();
        }
        public void OnAirdashBlock(CallbackContext context)
        {
            if (GameManagerData.GamePaused)
            {
                ZeroControls();
                return;
            }
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