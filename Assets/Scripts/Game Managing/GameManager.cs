using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Net.Http.Headers;

namespace CamKnight
{
    public class GameManager : MonoBehaviour
    {
        private int playerHealth = 5;

        [Header("Knight prefab")]
        [SerializeField]
        private GameObject knightPrefab;

        [Header("Heart icons")]
        [SerializeField]
        private GameObject[] hearts;

        [Header("Increment values")]
        [SerializeField]
        private float healthIncrement;
        [SerializeField]
        private float stunIncrement;
        [SerializeField]
        private int attackIncrement;
        private int spawnAmount = 0;

        [Header("Canvases")]
        [SerializeField]
        private GameObject gameplayCanvas;
        [SerializeField]
        private GameObject gameOverCanvas;

        private bool ended;

        [Header("Score")]
        [SerializeField]
        private TMP_Text score;

        [Header("Buttons")]
        [SerializeField]
        private Button retry;
        [SerializeField]
        private Button mainMenu;

        public int PlayerHealth { get => playerHealth; set => playerHealth = value; }
        public float HealthIncrement { get => healthIncrement; private set => healthIncrement = value; }
        public float StunIncrement { get => stunIncrement; private set => stunIncrement = value; }
        public int AttackIncrement { get => attackIncrement; private set => attackIncrement = value; }
        public int SpawnAmount { get => spawnAmount; set => spawnAmount = value; }

        private void Start()
        {
            retry.onClick.AddListener(Retry);
            mainMenu.onClick.AddListener(ReturnToMenu);
        }

        private void Update()
        {
            // When player HP reaches 0 activate game over screen.
            if (playerHealth <= 0 && !ended)
            {
                // Stop music and death sound.
                AudioManager.Instance.StopMusic();
                AudioManager.Instance.PlaySFX("PlayerDeath", false);

                // End game.
                EndGame();
                ended = true;
            }
        }

        /// <summary>
        /// Reduce player health and set UI hearts to represent current health.
        /// </summary>
        public void PlayerTakeDamage()
        {
            // Play sound.
            AudioManager.Instance.PlaySFX("TakeDamage", true);
            AudioManager.Instance.PlaySFX("PlayerGrunt", true);

            // Decrement health.
            playerHealth--;

            // Set UI hearts.
            if(playerHealth >= 0) hearts[playerHealth].SetActive(false);
        }

        /// <summary>
        /// Player recovers one health when enemy is killed.
        /// </summary>
        public void PlayerRecoverHealth()
        {
            if(playerHealth < 5)
            {
                hearts[playerHealth].SetActive(true);
                playerHealth++;
            }
        }

        /// <summary>
        /// Spawn new knight and destroy defeated one.
        /// </summary>
        public void SpawnKnight()
        {
            GameObject currentKnight = GameObject.FindGameObjectWithTag("Knight");
            Destroy(currentKnight);

            Instantiate(knightPrefab);
        }

        /// <summary>
        /// Setup scene for game over.
        /// </summary>
        private void EndGame()
        {
            // Find knight gameobject.
            GameObject knight = GameObject.FindGameObjectWithTag("Knight");

            // Deactivate knight.
            knight.SetActive(false);

            // Switch from gameplay canvas to game over canvas.
            gameplayCanvas.SetActive(false);
            gameOverCanvas.SetActive(true);

            // Set score text.
            score.text = "Enemies killed : " + (spawnAmount - 1).ToString();
        }

        /// <summary>
        /// Reloads scene for retry.
        /// </summary>
        private void Retry()
        {
            AudioManager.Instance.PlaySFX("StartGame", false);
            AudioManager.Instance.PlayMusic("GameplayTheme");

            Scene currentScene = SceneManager.GetActiveScene();

            SceneManager.LoadScene(currentScene.name);
        }

        private void ReturnToMenu()
        {
            AudioManager.Instance.PlaySFX("ButtonPress", false);
            AudioManager.Instance.PlayMusic("MainMenu");

            SceneManager.LoadScene("MainMenu");
        }
    }
}
