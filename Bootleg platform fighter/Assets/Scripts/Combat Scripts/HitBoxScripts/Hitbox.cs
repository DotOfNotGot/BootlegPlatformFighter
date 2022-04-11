using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class Hitbox : MonoBehaviour
    {
        private Fighting fightingScript;

        [SerializeField] private GameObject mainObject;
        public BootlegCharacterController characterController;
        private HitBoxHandler hitboxHandler;
        public Vector2 direction;


        [Header("Collider")]
        [SerializeField] public float attackAreaRadius;

        [Header("Damage")]
        [SerializeField] public float damage;
        [SerializeField] public float baseKnockback;
        [SerializeField] public float knockbackScaling = 0.1f;
        [SerializeField] [Range(-180, 180)] public float angle;
        [SerializeField] [Range(1, 10)] private int lineLengthDEBUG = 2;
        [SerializeField] public int hitStunFrames = 5;



        void Start()
        {
            hitboxHandler = mainObject.GetComponent<HitBoxHandler>();
            characterController = mainObject.transform.parent.gameObject.GetComponent<BootlegCharacterController>();
            fightingScript = mainObject.GetComponent<Fighting>();
            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            name = transform.parent.name + " " + name;

        }

        void FixedUpdate()
        {
            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            GetComponent<SpriteRenderer>().enabled = mainObject.GetComponent<HitBoxHandler>().visualizeHitboxes;

        }

       

        public bool SendToHitStun(GameObject hitEnemy)
        {
            hitEnemy.GetComponent<Knockback>().StartHitStun(hitStunFrames, this);
            return true;
        }


        private void OnDrawGizmosSelected()
        {

            Gizmos.DrawWireSphere(transform.position, attackAreaRadius);
            Debug.DrawLine(transform.position, new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * lineLengthDEBUG + transform.position.x, Mathf.Sin(angle * Mathf.Deg2Rad) * lineLengthDEBUG + transform.position.y), Color.red);
        }
    }
}
