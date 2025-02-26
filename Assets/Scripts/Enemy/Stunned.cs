using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CamKnight
{
    public class Stunned : IEnemyState
    {
        private float timer = 0;

        private bool exiting = false;

        public void OnEnter(EnemyController enemy)
        {
            enemy.animator.Play("Parried");
        }

        public void UpdateState(EnemyController enemy)
        {
            // If enemy HP drops to 0 change to dead state.
            if (enemy.health <= 0) enemy.ChangeState(new Dead());

            // Increment timer.
            timer += Time.deltaTime;

            // Start exiting stun.
            if(timer >= enemy.stunTime && !exiting)
            {
                enemy.animator.Play("ExitStun");

                exiting = true;
            }

            // Transition from stun to idle.
            if(timer >= enemy.stunTime + enemy.stunIdleTransitionTime)
            {
                enemy.stun = 0;

                enemy.stunFill.fillAmount = 0;

                enemy.ChangeState(new Idle());
            }
        }

        public void OnExit(EnemyController enemy)
        {

        }
    }
}
