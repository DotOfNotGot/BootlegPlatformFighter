using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class Grab : MonoBehaviour
    {

        private Fighting fightingScript;
        public BootlegCharacterController.Controls controls;

        [SerializeField] private GameObject character;
        private BootlegCharacterController characterController;
        [SerializeField] private LayerMask characterLayers;
        private HitBoxHandler hitboxHandler;

        [Header("Collider")]
        [SerializeField] public float grabAreaRadius;


        void Start()
        {
            hitboxHandler = character.GetComponent<HitBoxHandler>();
            characterController = character.GetComponent<BootlegCharacterController>();
            fightingScript = character.GetComponent<Fighting>();
        }

        private void FixedUpdate()
        {
            HandleInput(controls);
        }

        public void FindEnemyInArea()
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, grabAreaRadius, characterLayers);

            if (hitEnemies.Length > 0)
            {
                foreach (Collider2D enemy in hitEnemies)
                {
                    Fighting enemyFighting = enemy.gameObject.GetComponent<Fighting>();


                    if (enemy.gameObject.GetComponent<BootlegCharacterController>().playerIndex != gameObject.GetComponent<BootlegCharacterController>().playerIndex)
                    {

                            enemy.gameObject.GetComponent<BootlegCharacterController>().playerState = BootlegCharacterController.PlayerState.Grabbed;
                            gameObject.GetComponent<BootlegCharacterController>().playerState = BootlegCharacterController.PlayerState.Grab;
                        Debug.Log("Grabbed");
                        
                    }
                }
            }
        }

        private void HandleInput(BootlegCharacterController.Controls controls)
        {
            switch (characterController.playerState)
            {
                case BootlegCharacterController.PlayerState.GroundIdling:
                    if (controls.grabButtonPressed)
                    {
                        FindEnemyInArea();
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, grabAreaRadius);
        }

    }
}
    
