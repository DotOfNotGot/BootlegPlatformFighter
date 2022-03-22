using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class GameManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            // Colliders which handles if player is outside the level region
            GenerateCollidersAcrossScreen();
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

            EdgeCollider2D leftEdge = new GameObject("rightDeathZone").AddComponent<EdgeCollider2D>();
            colliderpoints = leftEdge.points;
            colliderpoints[0] = new Vector2(rTCorner.x, rTCorner.y);
            colliderpoints[1] = new Vector2(rTCorner.x, lBCorner.y);
            leftEdge.points = colliderpoints;

            EdgeCollider2D rightEdge = new GameObject("leftDeathZone").AddComponent<EdgeCollider2D>();

            colliderpoints = rightEdge.points;
            colliderpoints[0] = new Vector2(lBCorner.x, rTCorner.y);
            colliderpoints[1] = new Vector2(lBCorner.x, lBCorner.y);
            rightEdge.points = colliderpoints;
        }
    }
}
