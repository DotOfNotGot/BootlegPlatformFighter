using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BootlegPlatformFighter
{
    public class Knockback : MonoBehaviour
    {
        [SerializeField] private TMP_Text percentText;

        Rigidbody2D rigidBody;

        public float damageTakenPercent = 0.0f;

        private float weight;


        // Start is called before the first frame update
        void Start()
        {
            weight = gameObject.GetComponent<Rigidbody2D>().mass;
            rigidBody = gameObject.GetComponent<Rigidbody2D>();
            percentText.text = "0%";
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void KnockBack(Vector2 direction, float baseKnockback, float knockbackScaling ,float damagePercent, float angle)
        {
            damageTakenPercent += damagePercent;
            /*if (damageTakenPercent < 0.2f)
            {*/
            //Debug.Log(direction);
            direction = new Vector2(direction.x * (((((damageTakenPercent / 10 + (damageTakenPercent * damagePercent) / 20)
                    * (200 / weight + 100) * 1.4f) + 18)* knockbackScaling) + baseKnockback), 
                    direction.y * (((((damageTakenPercent / 10 + (damageTakenPercent * damagePercent) / 20)
                    * (200 / weight + 100) * 1.4f) + 18) * knockbackScaling) + baseKnockback));
            
            Debug.Log(direction);
            //}
            /*else
            {
                direction = new Vector2(direction.x * baseKnockback * (damageTakenPercent / 2), direction.y * baseKnockback * (damageTakenPercent / 2));
            }*/
            percentText.text = damageTakenPercent + "%";
            rigidBody.AddForce(direction);
        }
    }
}