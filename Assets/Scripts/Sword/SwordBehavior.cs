using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CamKnight
{
    public class SwordBehavior : MonoBehaviour
    {
        // SwordTracking reference.
        private SwordTracking tracking;

        // Sword pivot to rotate according to state.
        [Header("Sword Pivot")]
        [SerializeField]
        private Transform swordPivot;

        // Enum for sword states/
        public enum SwordState
        {
            ATTACKING,
            BLOCKING
        }

        // Current sword state.
        private SwordState state;

        // At which rotation point does sword switch states.
        [Header("Change State Treshold")]
        [SerializeField]
        private float stateChangeTreshold;

        // Material and colors to change it according to state.
        [Header("Material colors")]
        [SerializeField]
        private Material swordMaterial;
        [SerializeField]
        private Color attackColor;
        [SerializeField]
        private Color blockColor;

        [Header("Trail")]
        [SerializeField]
        private TrailRenderer trail;

        public SwordState State { get => state; private set => state = value; }

        private void Start()
        {
            // Get SwordTracking reference.
            tracking = GetComponent<SwordTracking>();
        }

        private void Update()
        {
            // Set sword state according to its rotation.
            if (tracking.Direction.y <= stateChangeTreshold) state = SwordState.BLOCKING;

            else state = SwordState.ATTACKING;

            // Block behavior.
            if(state == SwordState.BLOCKING)
            {
                // Rotate sword to face player.
                swordPivot.localRotation = Quaternion.Euler(swordPivot.localRotation.eulerAngles.x, swordPivot.localRotation.eulerAngles.y, -90);

                // Set material color.
                swordMaterial.color = blockColor;

                // Disable trail.
                trail.emitting = false;
            }

            // Attack behavior.
            if(state == SwordState.ATTACKING)
            {
                // Rotate sword to face enemy.
                swordPivot.localRotation = Quaternion.Euler(swordPivot.localRotation.eulerAngles.x, swordPivot.localRotation.eulerAngles.y, -180);

                // Set material color.
                swordMaterial.color = attackColor;

                // Enable trail.
                trail.emitting = true;
            }
        }
    }
}
