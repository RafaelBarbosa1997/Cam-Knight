using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CamKnight
{
    public class Attacking : IEnemyState
    {
        private int[] attackSequence;

        private int attackIndex;
        private float timer = 0;

        private bool attackStarted;


        public void OnEnter(EnemyController enemy)
        {
            // Create attack sequence array with number of attacks enemy can perform.
            attackSequence = new int[enemy.attackAmount];

            // Randomize enemy attack sequence.
            for (int i = 0; i < attackSequence.Length; i++)
            {
                attackSequence[i] = Random.Range(0, enemy.attackZones.Length - 1);
            }
        }

        public void UpdateState(EnemyController enemy)
        {
            // If enemy health drops to 0 change to dead state.
            if (enemy.health <= 0) enemy.ChangeState(new Dead());

            // If stun max enter stunned.
            if (enemy.stun >= enemy.stunMax) enemy.ChangeState(new Stunned());

            // Attack start behavior.
            if (!attackStarted)
            {
                // Display attack indicator.
                enemy.attackIcons[attackSequence[attackIndex]].gameObject.SetActive(true);

                // Play animation here.
                enemy.animator.Play(enemy.attackStrings[attackSequence[attackIndex]], -1, 0);

                attackStarted = true;
            }

            // Increment timer.
            timer += Time.deltaTime;

            if (timer >= enemy.attackInterval)
            {
                // Set attack indicator off.
                enemy.attackIcons[attackSequence[attackIndex]].gameObject.SetActive(false);

                // Attack code.
                CheckForSword check = enemy.attackZones[attackSequence[attackIndex]].GetComponent<CheckForSword>();
                // Block.
                if (check.ContainsSword && check.Sword.State == SwordBehavior.SwordState.BLOCKING)
                {
                    // Play sound.
                    AudioManager.Instance.PlaySFX("Parry", true);

                    // Increment stun meter.
                    enemy.stun += enemy.blockStun;

                    // Set stun bar.
                    enemy.stunFill.fillAmount = enemy.stun / enemy.stunMax;

                    // Activate block vignette.
                    enemy.blockVignette.ActivateVignette();

                    // If enemy stun reaches max change to stunned state.
                    if (enemy.stun >= enemy.stunMax) enemy.ChangeState(new Stunned());
                }

                // Hit.
                else
                {
                    // Activate hurt vignette.
                    enemy.hurtVignette.ActivateVignette();

                    // Player damage.
                    enemy.gameManager.PlayerTakeDamage();
                }

                // If last attack in sequence.
                if (attackIndex == attackSequence.Length - 1)
                {
                    // Change to idle state.
                    enemy.ChangeState(new Idle());
                }

                // Setup next attack.
                else
                {
                    // Increment attack index.
                    attackIndex++;

                    // Reset timer.
                    timer = 0;

                    // Reset attack start behavior check.
                    attackStarted = false;
                }
            }
        }

        public void OnExit(EnemyController enemy)
        {
            // Set attack indicator off if enemy enters stun or death while is active.
            enemy.attackIcons[attackSequence[attackIndex]].gameObject.SetActive(false);
        }
    }
}
