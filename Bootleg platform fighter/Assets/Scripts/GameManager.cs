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
            Vector2 lDCorner = Camera.main.ViewportToWorldPoint(new Vector3(-1, -1f, Camera.main.nearClipPlane));
            Vector2 rUCorner = Camera.main.ViewportToWorldPoint(new Vector3(2f, 2f, Camera.main.nearClipPlane));
            Vector2[] colliderpoints;

            EdgeCollider2D upperEdge = new GameObject("upperDeathZone").AddComponent<EdgeCollider2D>();
            colliderpoints = upperEdge.points;
            colliderpoints[0] = new Vector2(lDCorner.x, rUCorner.y);
            colliderpoints[1] = new Vector2(rUCorner.x, rUCorner.y);
            upperEdge.points = colliderpoints;

            EdgeCollider2D lowerEdge = new GameObject("lowerDeathZone").AddComponent<EdgeCollider2D>();
            colliderpoints = lowerEdge.points;
            colliderpoints[0] = new Vector2(lDCorner.x, lDCorner.y);
            colliderpoints[1] = new Vector2(rUCorner.x, lDCorner.y);
            lowerEdge.points = colliderpoints;

            EdgeCollider2D leftEdge = new GameObject("leftDeathZone").AddComponent<EdgeCollider2D>();
            colliderpoints = leftEdge.points;
            colliderpoints[0] = new Vector2(lDCorner.x, lDCorner.y);
            colliderpoints[1] = new Vector2(lDCorner.x, rUCorner.y);
            leftEdge.points = colliderpoints;

            EdgeCollider2D rightEdge = new GameObject("rightDeathZone").AddComponent<EdgeCollider2D>();

            colliderpoints = rightEdge.points;
            colliderpoints[0] = new Vector2(rUCorner.x, rUCorner.y);
            colliderpoints[1] = new Vector2(rUCorner.x, lDCorner.y);
            rightEdge.points = colliderpoints;
        }
    }
}
