using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BootlegPlatformFighter
{
    public class GameManager : MonoBehaviour
    {
        public Transform spawnPosition1;
        public Transform spawnPosition2;
        public Transform spawnPosition3;
        public Transform spawnPosition4;

        [SerializeField]
        private MultipleTargetCamera multipleTargetCamera;

        [SerializeField]
        private Transform cameraDummyTransform;

        [SerializeField]
        private GameObject explosionPrefab;

        public GameObject ExplosionPrefab { get { return explosionPrefab; } }

        [SerializeField]
        private int invulnerableTimer;

        // Start is called before the first frame update
        void Start()
        {
            // Colliders which handles if player is outside the level region
            GenerateCollidersAcrossScreen();
        }

        public void InitializeCameraTargets(GameObject player, int index)
        {
            multipleTargetCamera.targets[index] = player.transform;
        }

        public void RemoveCameraTarget(int index)
        {
            multipleTargetCamera.targets[index] = cameraDummyTransform;
        }

        public void AddCameraTarget(GameObject player, int index)
        {
            multipleTargetCamera.targets[index] = player.transform;
        }

        public HUDAvatar FindHUDAvatarByIndex(int idx)
        {
            var avatars = GameObject.FindGameObjectsWithTag("HUDAvatar");
            foreach (var avatar in avatars)
            {
                var script = avatar.GetComponent<HUDAvatar>();
                if (idx == script.getCharacterIndex())
                    return script;
            }
            return null;
        }

        public void RemoveHeartFromPlayer(int index)
        {
            RemoveCameraTarget(index);
            var hud = FindHUDAvatarByIndex(index);
            if (!hud)
            {
                Debug.Log("Missing HUD in RespawnPlayer");
                return;
            }
            hud.RemoveOneHeart();
        }

       public void RespawnPlayer(GameObject player, int index)
        {
            AddCameraTarget(player, index);
            Vector3 spawnPosition;
            switch (index)
            {
                case 0:
                    spawnPosition = spawnPosition1.position;
                    break;
                case 1:
                    spawnPosition = spawnPosition2.position;
                    break;
                    case 2:
                    spawnPosition = spawnPosition3.position;
                    break;
                case 3:
                    spawnPosition = spawnPosition4.position;
                    break;
                default:
                    spawnPosition = spawnPosition1.position;
                    break;
            }
            multipleTargetCamera.targets[index] = player.transform;
            player.transform.position = new Vector3(spawnPosition.x, spawnPosition.y + 30);
            player.transform.DOMove(spawnPosition, 1f).SetEase(Ease.OutQuint);
            player.GetComponent<BootlegCharacterController>().SetInvulnerable(false, invulnerableTimer);
            var hud = FindHUDAvatarByIndex(index);
            if (!hud){
                Debug.Log("Missing HUD in RespawnPlayer");
                return;
            }
            hud.SetHealth(0);
        }

        void GenerateCollidersAcrossScreen()
        {
            // Tweak the corners to adjust when they should be considered <outside>
            Vector2 rTCorner = new Vector2(250, 150);
            Vector2 lBCorner = new Vector2(-250, -50);
            Vector2[] colliderpoints;

            EdgeCollider2D upperEdge = new GameObject("upperDeathZone").AddComponent<EdgeCollider2D>();
            colliderpoints = upperEdge.points;
            colliderpoints[0] = new Vector2(lBCorner.x, rTCorner.y);
            colliderpoints[1] = new Vector2(rTCorner.x, rTCorner.y);
            upperEdge.points = colliderpoints;

            EdgeCollider2D lowerEdge = new GameObject("lowerDeathZone").AddComponent<EdgeCollider2D>();
            colliderpoints = lowerEdge.points;
            colliderpoints[0] = new Vector2(lBCorner.x, lBCorner.y);
            colliderpoints[1] = new Vector2(rTCorner.x, lBCorner.y);
            lowerEdge.points = colliderpoints;

            EdgeCollider2D rightEdge = new GameObject("rightDeathZone").AddComponent<EdgeCollider2D>();
            colliderpoints = rightEdge.points;
            colliderpoints[0] = new Vector2(rTCorner.x, rTCorner.y);
            colliderpoints[1] = new Vector2(rTCorner.x, lBCorner.y);
            rightEdge.points = colliderpoints;

            EdgeCollider2D leftEdge = new GameObject("leftDeathZone").AddComponent<EdgeCollider2D>();

            colliderpoints = leftEdge.points;
            colliderpoints[0] = new Vector2(lBCorner.x, rTCorner.y);
            colliderpoints[1] = new Vector2(lBCorner.x, lBCorner.y);
            leftEdge.points = colliderpoints;
        }
    }
    public class Player_t
    {
        public Player_t(string name) {
            this.name = name;
            this.damageCaused = 0f;
            this.damageTaken = 0f;
        }
        public string name;
        public float damageCaused;
        public float damageTaken;
    }
    public static class GameManagerData
    {
        static public Dictionary<int, Player_t> Players = new Dictionary<int, Player_t>();
        static public Player_t LastWinner;

        public static bool GamePaused { get; set; }
    }
}
