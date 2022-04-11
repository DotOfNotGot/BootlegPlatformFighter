using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class HurtBox : MonoBehaviour
    {
        [SerializeField] public GameObject character;
        public BootlegCharacterController characterController;
        public int characterIndex;

        [Header("HurtBox Settings")]
        public bool canBeGrabbed;


        // Start is called before the first frame update
        void Start()
        {
            characterController = character.transform.parent.gameObject.GetComponentInParent<BootlegCharacterController>();
            characterIndex = characterController.characterIndex;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

    }
}
