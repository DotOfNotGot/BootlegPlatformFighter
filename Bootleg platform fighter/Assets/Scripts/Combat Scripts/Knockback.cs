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

        private BootlegCharacterController characterController;

        private float weight;



        // Start is called before the first frame update
        void Start()
        {
            characterController = GetComponent<BootlegCharacterController>();
            weight = gameObject.GetComponent<Rigidbody2D>().mass;
            rigidBody = gameObject.GetComponent<Rigidbody2D>();
            percentText.text = "0%";
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void KnockBack(Vector2 direction, float baseKnockback, float knockbackScaling ,float damagePercent)
        {
            characterController.damageTakenPercent += damagePercent;
            /*if (damageTakenPercent < 0.2f)
            {*/
            //Debug.Log(direction);
            direction = new Vector2(direction.x * (((((characterController.damageTakenPercent / 10 + (characterController.damageTakenPercent * damagePercent) / 20)
                    * (200 / weight + 100) * 1.4f) + 18)* knockbackScaling) + baseKnockback), 
                    direction.y * (((((characterController.damageTakenPercent / 10 + (characterController.damageTakenPercent * damagePercent) / 20)
                    * (200 / weight + 100) * 1.4f) + 18) * knockbackScaling) + baseKnockback));
            
           //Debug.Log(direction);
            //}
            /*else
            {
                direction = new Vector2(direction.x * baseKnockback * (damageTakenPercent / 2), direction.y * baseKnockback * (damageTakenPercent / 2));
            }*/
            percentText.text = characterController.damageTakenPercent + "%";
            rigidBody.AddForce(direction);
        }
    }
}