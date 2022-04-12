using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class AirshipMovement : MonoBehaviour
    {
        public float speed = 40.0f;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }

       
    }
}
