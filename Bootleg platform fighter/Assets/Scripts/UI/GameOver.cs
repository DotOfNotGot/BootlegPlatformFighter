using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField]
        GameObject resultsCardPrefab;
        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < 5; i++)
                GameManagerData.Players.Add(new Player_t());
            foreach (var player in GameManagerData.Players)
            {
                Instantiate(resultsCardPrefab, transform);
            }
        }
    }
}
