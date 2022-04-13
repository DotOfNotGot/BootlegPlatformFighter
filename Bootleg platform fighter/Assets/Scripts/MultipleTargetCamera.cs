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

        [SerializeField]private float minZoom = 40f;
        [SerializeField]private float maxZoom = 10f;
        [SerializeField] private float zoomLimiter = 50f;

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        void LateUpdate()
        {
            if (targets.Count == 0) // will be false because you've set size to 2 in the editor
            {
                return;
            }

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

            var bounds = new Bounds(targets[0].position, Vector3.zero);
            foreach (var target in targets)
            {
                bounds.Encapsulate(target.position);
            }

            return bounds.center;

        }
    }
}
