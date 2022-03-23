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

        // HUD STUFF
        private GameObject _hudAvatar;
        private TextMeshProUGUI _nameText;
        private TextMeshProUGUI _healthText;
        private GameObject _lifePanel;


        // Start is called before the first frame update
        void Start()
        {
            characterController = GetComponent<BootlegCharacterController>();
            weight = gameObject.GetComponent<Rigidbody2D>().mass;
            rigidBody = gameObject.GetComponent<Rigidbody2D>();

            if (!SetupHUD())
            {
                Debug.LogError("Didn't find any available HUDAvatars");
            }

            _healthText.text = "0%";
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
            _healthText.text = characterController.damageTakenPercent + "%";
            rigidBody.AddForce(direction);
        }

        private bool SetupHUD()
        {
            void makeSetup(GameObject avatar)
            {
                _hudAvatar = avatar;
                _nameText = avatar.transform.Find("NamePanel").Find("NameText").GetComponent<TextMeshProUGUI>();
                _healthText = avatar.transform.Find("DataPanel").Find("HealthText").GetComponent<TextMeshProUGUI>();
                _lifePanel = avatar.transform.Find("DataPanel").Find("LifePanel").gameObject;
            }

            GameObject backupObject = null;
            var avatars = GameObject.FindGameObjectsWithTag("HUDAvatar");
            foreach (var avatar in avatars)
            {
                var hudscript = avatar.GetComponent<HUDAvatar>();
                if (hudscript.getCharacterIndex() == -1)
                {
                    hudscript.setCharacterIndex(characterController.characterIndex);
                    makeSetup(avatar);
                    return true;
                }
                if (hudscript.getCharacterIndex() == characterController.characterIndex)
                    backupObject = avatar;
            }
            if (backupObject != null) {
                makeSetup(backupObject);
                return true;
            }
            return false;
        }
    }
}