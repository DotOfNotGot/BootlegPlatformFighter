using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class Fighting : MonoBehaviour
    {
        public BootlegCharacterController.Controls controls;

        private BootlegCharacterController characterController;
        private BoxCollider2D attackBox;
        private float horizontalInput;
        private float verticalInput;
       

        [SerializeField] Collider2D playerCollider;

        public Transform attackPoint;
        public float attackRange = 0.5f;
        public LayerMask characterLayers;

        [SerializeField] private Vector2 direction;


        // Start is called before the first frame update
        void Start()
        {
            characterController = gameObject.GetComponentInParent<BootlegCharacterController>();
            attackPoint.localScale = new Vector3(8 * attackRange, 8 * attackRange, 8 * attackRange);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            /*This gets the character's direction into 2 variables, so that can be used to
             * determine the Vector2 of the attackPoint.*/

            Debug.DrawRay(transform.position, new Vector2((direction.x * 5) * horizontalInput, direction.y * 5), Color.green);
            horizontalInput = characterController.moveVector.x;
            verticalInput = characterController.moveVector.y;
            UpdateAttackPoint(new Vector2(horizontalInput, verticalInput));

        }

        public void UpdateAttackPoint(Vector2 direction, float xOffset = 0.85f, float yOffset = 1.5f)
        {
            if (direction.y != 0)
            {
                attackPoint.localPosition = new Vector2(0, yOffset * direction.y);
            }
            else
            {
                attackPoint.localPosition = new Vector2(xOffset, 0);
            }

        }

    }

    public static class WaitFor
    {
        public static IEnumerator Frames(int frameCount)
        {
            while (frameCount > 0)
            {
                frameCount--;
                yield return null;

            }
        }
    }
}
