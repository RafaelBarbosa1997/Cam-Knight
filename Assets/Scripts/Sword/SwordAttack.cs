using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CamKnight
{
    public class SwordAttack : MonoBehaviour
    {
        private EnemyController enemyController;

        private SwordBehavior sword;

        [Header("Hit effect")]
        [SerializeField]
        private GameObject attackIcon;
        private HitVignette hitVignette;

        [Header("Hit effect positions")]
        [SerializeField]
        private Transform standingPos;
        [SerializeField]
        private Transform crouchingPos;

        private void Start()
        {
            sword = GameObject.FindGameObjectWithTag("SwordBehavior").GetComponent<SwordBehavior>();

            enemyController = GetComponent<EnemyController>();

            hitVignette = attackIcon.GetComponent<HitVignette>();
        }

        private void OnTriggerEnter(Collider other)
        {
            // When sword slashes enemy.
            if(other.tag == "Sword" && sword.State == SwordBehavior.SwordState.ATTACKING)
            {
                // Normal attack values.
                if(enemyController.stateString == "CamKnight.Idle" || enemyController.stateString == "CamKnight.Attacking")
                {
                    PerformAttack(enemyController.normalDamage, standingPos, false);
                }

                // Stunned attack values.
                if(enemyController.stateString == "CamKnight.Stunned")
                {
                    PerformAttack(enemyController.stunnedDamage, crouchingPos, true);
                }
            }
        }

        /// <summary>
        /// Deals damage to enemy when player slashes them with sword.
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="pos"></param>
        /// <param name="stunned"></param>
        private void PerformAttack(float damage, Transform pos, bool stunned)
        {
            // Reduce enemy HP.
            enemyController.health -= damage;

            // Set enemy health bar.
            enemyController.healthFill.fillAmount = enemyController.health / enemyController.healthMax;

            // If enemy isn't stunned, apply hit stun.
            if (!stunned)
            {
                enemyController.stun += enemyController.attackStun;
                enemyController.stunFill.fillAmount = enemyController.stun / enemyController.stunMax;
            }

            // Enable hit effect.
            attackIcon.transform.position = pos.position;
            hitVignette.ActivateVignette();

            // Play sound.
            AudioManager.Instance.PlaySFX("DealDamage", true);
        }
    }
}
