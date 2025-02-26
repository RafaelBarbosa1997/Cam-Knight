using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CamKnight
{
    public class CheckForSword : MonoBehaviour
    {
        private SwordBehavior sword = null;
        private bool containsSword = false;

        public SwordBehavior Sword { get => sword; private set => sword = value; }
        public bool ContainsSword { get => containsSword; private set => containsSword = value; }

        private void OnTriggerEnter(Collider other)
        {
            // When sword enters trigger we know sword is in this zone.
            if(other.tag == "Sword")
            {
                sword = other.transform.parent.GetComponent<SwordBehavior>();
                containsSword = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // When sword exits trigger we know sword is not in this zone.
            if(other.tag == "Sword")
            {
                sword = null;
                containsSword = false;
            }
        }
    }
}
