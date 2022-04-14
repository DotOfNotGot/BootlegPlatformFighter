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
        private GameManager gameManager;

        private AnimationHandler animHandler;
        [SerializeField] private int hitStunTimer = -1;
        public bool isHitStunned;
        public Hitbox hitbox;

        private int lastAttackerIndex = -1;


        // Start is called before the first frame update
        void Start()
        {
            characterController = GetComponentInParent<BootlegCharacterController>();
            weight = gameObject.GetComponentInParent<Rigidbody2D>().mass;
            rigidBody = gameObject.GetComponentInParent<Rigidbody2D>();
            gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>( );
            animHandler = gameObject.GetComponent<AnimationHandler>();

            if (!gameManager)
                Debug.LogError("Missing GameManager object, did you forget to add it to the scene?");

            if (!SetupHUD())
            {
                Debug.LogError("Knockback.cs: Didn't find any available HUDAvatars");
            }

            _HUDAvatar?.SetHealth(0);
        }

        public void FixedUpdate()
        {
            if (hitStunTimer > 0)
            {
                hitStunTimer--;
            }
            else if (hitStunTimer == 0)
            {
                EndHitStun();
                hitStunTimer = -1;

            }
        }

        // Sends force to player and returns percentage total damage taken
        public float KnockBack(Vector2 direction, float baseKnockback, float knockbackScaling ,float damagePercent)
        {
            EnterTumble();
            characterController.damageTakenPercent += damagePercent;
            direction = new Vector2(direction.x * (((((characterController.damageTakenPercent / 10 + (characterController.damageTakenPercent * damagePercent) / 20)
                    * (200 / weight + 100) * 1.4f) + 18)* knockbackScaling) + baseKnockback), 
                    direction.y * (((((characterController.damageTakenPercent / 10 + (characterController.damageTakenPercent * damagePercent) / 20)
                    * (200 / weight + 100) * 1.4f) + 18) * knockbackScaling) + baseKnockback));
            Debug.Log(characterController.damageTakenPercent);
            Debug.Log("direction = " + direction);
            Debug.Log("baseKnockback = " + baseKnockback);
            Debug.Log("knockbackScaling = " + knockbackScaling);
            Debug.Log("damagePercent = " + damagePercent);

            _HUDAvatar.SetHealth(characterController.damageTakenPercent);
            rigidBody.AddForce(direction, ForceMode2D.Impulse);
            return characterController.damageTakenPercent;
        }

        IEnumerator DelayedDeath()
        {
            yield return new WaitForSeconds(0.3f);
            Instantiate(gameManager.ExplosionPrefab, transform.position, gameManager.ExplosionPrefab.transform.rotation);
            // Probably disables wrong children
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);
            StartCoroutine(characterController.DelayRespawn());
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

        public void StartHitStun(int stunTimer, Hitbox hBox, int attackerIndex)
        {
            hitbox = hBox;
            hitStunTimer = stunTimer;
            isHitStunned = true;
            characterController.canMove = false;
            characterController.previousPlayerState = characterController.playerState;
            characterController.playerState = BootlegCharacterController.PlayerState.HitStun;
            lastAttackerIndex = attackerIndex;
        }

        public void EndHitStun()
        {
            characterController.canMove = true;
            isHitStunned = false;
            var dmg = KnockBack(new Vector2(transform.parent.position.x - hitbox.mainObject.transform.position.x, 1) * hitbox.direction, hitbox.baseKnockback, hitbox.knockbackScaling, hitbox.damage);
            GameManagerData.Players[lastAttackerIndex].damageCaused = dmg;
        }

        public void EnterTumble()
        {
            characterController.previousPlayerState = characterController.playerState;
            characterController.playerState = BootlegCharacterController.PlayerState.Tumble;
        }

    }
}