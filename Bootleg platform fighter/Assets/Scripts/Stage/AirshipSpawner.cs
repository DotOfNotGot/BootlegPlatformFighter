using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class AirshipSpawner : MonoBehaviour
    {
        public GameObject airShipPrefab;
        
        [SerializeField] private float startDelay = 2;
        [SerializeField] private float spawnInterval = 2.0f;
        // Start is called before the first frame update
        void Start()
        {
            
            InvokeRepeating("SpawnAirship", startDelay, spawnInterval);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void SpawnAirship()
        {
            Instantiate(airShipPrefab, transform);
        }
    }
}
