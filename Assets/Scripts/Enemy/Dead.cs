using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CamKnight
{
    public class Dead : IEnemyState
    {
        private float timer;

        public void OnEnter(EnemyController enemy)
        {
            AudioManager.Instance.PlaySFX("EnemyDeath", true);

            enemy.animator.Play("Death");

            enemy.gameManager.PlayerRecoverHealth();
        }

        public void UpdateState(EnemyController enemy)
        {
            timer += Time.deltaTime;

            // Spawn new enemy.
            if(timer >= enemy.respawnTimer)
            {
                enemy.gameManager.SpawnKnight();
            }
        }

        public void OnExit(EnemyController enemy)
        {

        }
    }
}
