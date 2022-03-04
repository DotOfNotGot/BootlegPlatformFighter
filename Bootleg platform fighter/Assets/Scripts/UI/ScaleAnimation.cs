using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BootlegPlatformFighter
{
    public class ScaleAnimation : MonoBehaviour
    {
        [SerializeField]
        [Range(0f, 1f)]
        float startScale = 0.5f;
        [SerializeField]
        [Range(0.01f, 3f)]
        float animationTime = 0.1f;

        // Start is called before the first frame update
        void Start()
        {
            gameObject.transform.localScale = new Vector3(startScale, startScale);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnEnable()
        {
            gameObject.transform.localScale = new Vector3(startScale, startScale);

            transform.DOScale(Vector2.one, animationTime);
        }
    }
}
