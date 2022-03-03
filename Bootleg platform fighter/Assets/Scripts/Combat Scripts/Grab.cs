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


                    if (enemy.gameObject.GetComponent<BootlegCharacterController>().playerIndex != character.gameObject.GetComponent<BootlegCharacterController>().playerIndex)
                    {

                            enemy.gameObject.GetComponent<BootlegCharacterController>().playerState = BootlegCharacterController.PlayerState.Grabbed;
                            characterController.playerState = BootlegCharacterController.PlayerState.Grab;
                        StartCoroutine(GrabTimer(5, enemy));
                        Debug.Log("Grabbed");
                        
                    }
                }
            }
        }

        public void HandleInput(BootlegCharacterController.Controls controls)
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

        IEnumerator GrabTimer(float seconds, Collider2D enemy)
        {
            yield return new WaitForSeconds(seconds);
            character.GetComponent<Animator>().SetBool("isGrabbing", false);
            characterController.playerState = BootlegCharacterController.PlayerState.GroundIdling;
            enemy.gameObject.GetComponent<BootlegCharacterController>().playerState = BootlegCharacterController.PlayerState.GroundIdling;

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, grabAreaRadius);
        }

    }
}
    
