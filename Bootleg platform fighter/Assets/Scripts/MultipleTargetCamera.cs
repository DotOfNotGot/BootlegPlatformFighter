using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
     [RequireComponent(typeof(Camera))]
    public class MultipleTargetCamera : MonoBehaviour
    {

        public List<Transform> targets;

        [SerializeField]private Vector3 offset;
        [SerializeField] private float smoothTime = .5f;

        private Vector3 velocity;
        private Camera cam;

        [SerializeField]
        private float minPositionY;
        [SerializeField]
        private int maxPositionY;

        [SerializeField]private float minZoom = 40f;
        [SerializeField]private float maxZoom = 10f;
        [SerializeField] private float zoomLimiter = 50f;

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        void LateUpdate()
        {
            if (targets[0] == null) // this check will work tho
                return;


            Move();
            Zoom();
        }

        void Move()
        {
            Vector3 centerPoint = GetCenterPoint();

            Vector3 newPosition = centerPoint + offset;

            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
        }

        void Zoom()
        {
            float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
        }

        float GetGreatestDistance()
        {
            var bounds = new Bounds(targets[0].position, Vector3.zero);
            foreach (var target in targets)
            {
                    bounds.Encapsulate(target.position);
            }

            return bounds.size.x;

        }

        Vector3 GetCenterPoint()
        {
            if (targets.Count == 1)
            {
                return targets[0].position;
            }

            Bounds bounds;

            if (targets[0].position.y < minPositionY)
            {
                bounds = new Bounds(new Vector3(targets[0].position.x, minPositionY), Vector3.zero);
            }
            else
            {
                bounds = new Bounds(targets[0].position, Vector3.zero);
            }
            

            foreach (var target in targets)
            {
                if (target.position.y < minPositionY)
                {
                    bounds.Encapsulate(new Vector3(target.position.x, minPositionY, target.position.z));
                }
                else
                {
                    bounds.Encapsulate(target.position);
                }
            }

            return bounds.center;

        }
    }
}
