using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CamKnight
{
    public class SwordTracking : MonoBehaviour
    {
        // Reference to blob detection script.
        [Header("Blob detection reference")]
        [SerializeField]
        private BlobDetection blobs;

        // Corner positions of plane where cam texture is renderer.
        [Header("Plane corners")]
        [SerializeField]
        private Transform bottomLeft;
        [SerializeField]
        private Transform topRight;

        // Reference to sword pivot to move sword model.
        [Header("Sword pivot")]
        [SerializeField]
        private Transform swordPivot;

        // Stores the position for the sword's base and tip.
        private Vector3 tipPosition;
        private Vector3 basePosition;

        // Direction of sword to determine if blocking or attacking.
        private Vector3 direction;

        // Value to define what values pass through high pass filter.
        [Header("High pass filter")]
        [SerializeField]
        private float filterTreshold;

        // Previous position for high pass filter.
        private Vector3 previousBase;
        private Vector3 previousTip;

        // Ignore filter on first iteration.
        private bool firstIteration;

        public Vector3 Direction { get => direction; private set => direction = value; }
        public Vector3 TipPosition { get => tipPosition; private set => tipPosition = value; }
        public float FilterTreshold { get => filterTreshold; set => filterTreshold = value; }

        private void Start()
        {
            // Set first iteration to true.
            firstIteration = true;
        }

        private void Update()
        {
            if (blobs.TipMidPoint != null && blobs.BaseMidPoint != null)
            {
                // Get sword tip's tracked position.
                tipPosition = TrackPosition(blobs.TipMidPoint.x, blobs.TipMidPoint.y);

                if (tipPosition == new Vector3(-100, -100, -100)) return;

                // Get sword base's tracked position.
                basePosition = TrackPosition(blobs.BaseMidPoint.x, blobs.BaseMidPoint.y);

                if (basePosition == new Vector3(-100, -100, -100)) return;

                // Set sword position.
                if ((Mathf.Abs(previousBase.x - tipPosition.x) > filterTreshold && Mathf.Abs(previousBase.y - tipPosition.y) > filterTreshold) || firstIteration)
                {
                    swordPivot.position = basePosition;
                    previousBase = basePosition;
                }

                // Set sword rotation.
                if ((Mathf.Abs(previousTip.x - basePosition.x) > filterTreshold && Mathf.Abs(previousTip.y - basePosition.y) > filterTreshold) || firstIteration)
                {
                    direction = (tipPosition - basePosition).normalized;
                    swordPivot.forward = -direction;
                    previousTip = tipPosition;
                }

                // Set values after first iteration.
                if (firstIteration)
                {
                    previousBase = basePosition;
                    previousTip = tipPosition;
                    firstIteration = false;
                }
            }

        }

        /// <summary>
        /// Gets the blob positions from blob detection script and converts it so we can track objects in local space.
        /// </summary>
        /// <param name="baseX"></param>
        /// <param name="baseY"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private Vector3 TrackPosition(float baseX, float baseY)
        {
            if(blobs.TextureTarget != null)
            {
                // Normalized coordinates.
                Vector3 uvCoordinates = new Vector3(baseX / blobs.TextureTarget.width, baseY / blobs.TextureTarget.height, -1);

                // Position lerped from texture corners with normalized coordinates values.
                Vector3 position = new Vector3(Mathf.Lerp(topRight.position.x, bottomLeft.position.x, uvCoordinates.x), Mathf.Lerp(bottomLeft.position.y, topRight.position.y, uvCoordinates.y));

                return position;
            }

            return new Vector3(-100, -100, -100);
        }


    }
}
