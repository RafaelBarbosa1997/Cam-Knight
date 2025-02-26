using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CamKnight
{
    public class Spawning : IEnemyState
    {
        private float timer;

        public void OnEnter(EnemyController enemy)
        {
            // Increment enemy stats according to how many enemies have spawned.
            enemy.healthMax += enemy.gameManager.HealthIncrement * enemy.gameManager.SpawnAmount;
            enemy.stunMax += enemy.gameManager.StunIncrement * enemy.gameManager.SpawnAmount;
            enemy.attackAmount += enemy.gameManager.AttackIncrement * enemy.gameManager.SpawnAmount;

            // Increment spawn amount.
            enemy.gameManager.SpawnAmount++;

            // Set enemy health.
            enemy.health = enemy.healthMax;

            // Set health and stun bars.
            enemy.healthFill.fillAmount = enemy.health / enemy.healthMax;
            enemy.stunFill.fillAmount = enemy.stun / enemy.stunMax;
        }

        public void UpdateState(EnemyController enemy)
        {
            timer += Time.deltaTime;

            if(timer >= enemy.spawnTime)
            {
                enemy.ChangeState(new Idle());
            }
        }

        public void OnExit(EnemyController enemy)
        {

        }
    }
}
