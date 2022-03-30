using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BootlegPlatformFighter
{
    public class Knockback : MonoBehaviour
    {
        Rigidbody2D rigidBody;

        private BootlegCharacterController characterController;

        private float weight;

        private HUDAvatar _HUDAvatar;


        // Start is called before the first frame update
        void Start()
        {
            characterController = GetComponentInParent<BootlegCharacterController>();
            weight = gameObject.GetComponentInParent<Rigidbody2D>().mass;
            rigidBody = gameObject.GetComponentInParent<Rigidbody2D>();

            if (!SetupHUD())
            {
                Debug.LogError("Knockback.cs: Didn't find any available HUDAvatars");
            }

            _HUDAvatar.SetHealth(0);
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
            _HUDAvatar.SetHealth(characterController.damageTakenPercent);
            rigidBody.AddForce(direction);
        }

        private bool SetupHUD()
        {
            HUDAvatar backupObject = null;
            var avatars = GameObject.FindGameObjectsWithTag("HUDAvatar");
            foreach (var avatar in avatars)
            {
                var hudscript = avatar.GetComponent<HUDAvatar>();
                if (hudscript.getCharacterIndex() == -1)
                {
                    hudscript.setCharacterIndex(characterController.characterIndex);
                    _HUDAvatar = hudscript;
                    return true;
                }
                if (hudscript.getCharacterIndex() == characterController.characterIndex)
                    backupObject = hudscript;
            }
            if (backupObject != null) {
                _HUDAvatar = backupObject;
                return true;
            }
            return false;
        }
    }
}