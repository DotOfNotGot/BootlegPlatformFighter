using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class Knockback : MonoBehaviour
    {
        Rigidbody2D rigidBody;

        public float damageTakenPercent = 0.0f;

        private float weight = 1.0f;


        // Start is called before the first frame update
        void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void KnockBack(Vector2 direction, float baseKnockback, float knockbackScaling ,float damagePercent)
        {
            /*if (damageTakenPercent < 0.2f)
            {*/
                direction = new Vector2(direction.x * (((((damageTakenPercent / 10 + (damageTakenPercent * damagePercent) / 20) * (200 / weight + 100) * 1.4f) + 18)* knockbackScaling) * baseKnockback), direction.y * baseKnockback * 0.2f);
            Debug.Log(direction);
            //}
            /*else
            {
                direction = new Vector2(direction.x * baseKnockback * (damageTakenPercent / 2), direction.y * baseKnockback * (damageTakenPercent / 2));
            }*/
            damageTakenPercent += damagePercent;
            rigidBody.AddForce(direction);
        }
    }
}