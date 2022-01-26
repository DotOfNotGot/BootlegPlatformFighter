using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class Knockback : MonoBehaviour
    {
        Rigidbody2D rigidBody;

        public float damageTakenPercent = 0.0f;


        // Start is called before the first frame update
        void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void KnockBack(Vector2 direction, float knockbackVelocity, float damagePercent)
        {
            if (damageTakenPercent < 0.2f)
            {
                direction = new Vector2(direction.x * knockbackVelocity * 0.2f, direction.y * knockbackVelocity * 0.2f);
            }
            else
            {
                direction = new Vector2(direction.x * knockbackVelocity * (damageTakenPercent / 2), direction.y * knockbackVelocity * (damageTakenPercent / 2));
            }
            damageTakenPercent += damagePercent;
            rigidBody.AddForce(direction);
        }
    }
}