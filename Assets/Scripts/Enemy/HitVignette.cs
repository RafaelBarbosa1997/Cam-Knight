using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CamKnight
{
    public class HitVignette : MonoBehaviour
    {
        private SpriteRenderer sprite;

        private float alpha;

        [SerializeField]
        private float decrementRate;

        private void Start()
        {
            sprite = GetComponent<SpriteRenderer>();

            // Set transparent on start.
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
        }

        private void Update()
        {
            // Set opacity.
            if(sprite.color.a > 0) sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);

            // Decrement opacity.
            if (alpha > 0)
            {
                alpha -= decrementRate * Time.deltaTime;
            }
        }

        /// <summary>
        /// Sets opacity to max and begins decrement procedure.
        /// </summary>
        public void ActivateVignette()
        {
            alpha = 1;

            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
        }
    }
}
