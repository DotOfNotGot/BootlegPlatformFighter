using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class CharacterAI : MonoBehaviour
    {
        public BootlegCharacterController.Controls controls;
        private BootlegCharacterController.Controls previousControls;

        private BootlegCharacterController characterController;

        private void Start()
        {
            characterController = GetComponent<BootlegCharacterController>();
        }

        void FixedUpdate()
        {

        }
    }
}