using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CamKnight
{
    public class EnemyController : MonoBehaviour
    {
        private IEnemyState currentState;

        internal GameManager gameManager;

        [Header("Stats")]
        [SerializeField]
        internal float healthMax;
        internal float health;
        [SerializeField]
        internal float normalDamage;
        [SerializeField]
        internal float stunnedDamage;
        [SerializeField]
        internal float stun;
        [SerializeField]
        internal float stunMax;
        [SerializeField]
        internal float blockStun;
        [SerializeField]
        internal float attackStun;
        [SerializeField]
        internal float stunTime;
        [SerializeField]
        internal float stunIdleTransitionTime;

        [Header("Animator reference")]
        [SerializeField]
        internal Animator animator;

        [Header("Spawn time")]
        [SerializeField]
        internal float spawnTime;

        [Header("Wait time range to change from idle to attack")]
        [SerializeField]
        internal float minSwitchAttack;
        [SerializeField]
        internal float maxSwitchAttack;

        [Header("Attack sequence settings")]
        [SerializeField]
        internal int attackAmount;
        [SerializeField]
        internal float attackInterval;

        [Header("Attack zones")]
        [SerializeField]
        internal GameObject[] attackZones;
        [SerializeField]
        internal GameObject[] attackIcons;

        [Header("Attack strings for animator")]
        [SerializeField]
        internal string[] attackStrings;

        [Header("Vignettes")]
        [SerializeField]
        internal HitVignette hurtVignette;
        [SerializeField]
        internal HitVignette blockVignette;

        internal Image healthFill;
        internal Image stunFill;

        [Header("Respawn timer")]
        [SerializeField]
        internal float respawnTimer;

        internal string stateString;

        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            healthFill = GameObject.FindGameObjectWithTag("HealthFill").GetComponent<Image>();
            stunFill = GameObject.FindGameObjectWithTag("StunFill").GetComponent<Image>();

            currentState = new Spawning();

            stateString = currentState.ToString();

            currentState.OnEnter(this);
        }

        // Update is called once per frame
        void Update()
        {
            currentState.UpdateState(this);
        }

        public void ChangeState(IEnemyState newState)
        {
            currentState.OnExit(this);

            currentState = newState;

            stateString = newState.ToString();

            currentState.OnEnter(this);
        }
    }

    public interface IEnemyState
    {
        public void OnEnter(EnemyController enemy);
        public void UpdateState(EnemyController enemy);
        public void OnExit(EnemyController enemy);
    }
}
