using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CamKnight
{
    public class Idle : IEnemyState
    {
        // Wait time to switch from idle to attacking.
        private float attackSwitchTime;
        private float timer;

        public void OnEnter(EnemyController enemy)
        {
            // Set a random wait time to switch to attacking from min to max value.
            attackSwitchTime = Random.Range(enemy.minSwitchAttack, enemy.maxSwitchAttack);

            // Set idle animation.
            enemy.animator.Play("Idle");
        }

        public void UpdateState(EnemyController enemy)
        {
            // If enemy health drops to 0 switch to dead state.
            if (enemy.health <= 0) enemy.ChangeState(new Dead());

            // If enemy stun meter reaches max switch to stunned state.
            if (enemy.stun >= enemy.stunMax) enemy.ChangeState(new Stunned());

            // Increment timer.
            timer += Time.deltaTime;

            // If timer passes max change to attack state.
            if(timer >= attackSwitchTime) enemy.ChangeState(new Attacking());
        }

        public void OnExit(EnemyController enemy)
        {

        }
    }
}
