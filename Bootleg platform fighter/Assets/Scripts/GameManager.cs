using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class GameManager : MonoBehaviour
    {
        public Transform spawnPosition1;
        public Transform spawnPosition2;
        public Transform spawnPosition3;
        public Transform spawnPosition4;


        // Start is called before the first frame update
        void Start()
        {
            // Colliders which handles if player is outside the level region
            GenerateCollidersAcrossScreen();
        }

        HUDAvatar findHudAvatarByIdx(int idx)
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
       public void RespawnPlayer(GameObject player, int index)
        {
            player.transform.position = spawnPosition1.position;
            var hud = findHudAvatarByIdx(index);
            if (!hud){
                Debug.Log("Missing HUD in RespawnPlayer");
                return;
            }
            hud.RemoveOneHeart();
        }

        void GenerateCollidersAcrossScreen()
        {
            // Tweak the corners to adjust when they should be considered <outside>
            Vector2 rTCorner = new Vector2(500, 150);
            Vector2 lBCorner = new Vector2(-500, -50);
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
}