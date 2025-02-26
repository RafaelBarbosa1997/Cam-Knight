using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace CamKnight
{
    public class MainMenuButtons : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField]
        private Button play;
        [SerializeField]
        private Button calibration;
        [SerializeField]
        private Button back;

        [Header("Canvases")]
        [SerializeField]
        private GameObject menuCanvas;
        [SerializeField]
        private GameObject calibrationCanvas;

        private GameObject cam;
        private MeshRenderer camRenderer;
        private BlobDetection blobs;

        private void Start()
        {
            // Get references.
            cam = GameObject.FindGameObjectWithTag("Cam");
            camRenderer = cam.GetComponent<MeshRenderer>();
            blobs = cam.GetComponent<BlobDetection>();

            // Add button listeners.
            play.onClick.AddListener(StartGame);
            calibration.onClick.AddListener(EnterCalibration);
            back.onClick.AddListener(ExitCalibration);

            // Start music.
            AudioManager.Instance.PlayMusic("MainMenu");
        }

        /// <summary>
        /// Start game when pressing start button.
        /// </summary>
        private void StartGame()
        {
            // Play sound.
            AudioManager.Instance.PlaySFX("StartGame", false);

            // Start gameplay music.
            AudioManager.Instance.PlayMusic("GameplayTheme");

            // Load gameplay scene.
            SceneManager.LoadScene("Gameplay");
        }

        /// <summary>
        /// Enter calibration menu when pressing calibration button.
        /// </summary>
        private void EnterCalibration()
        {
            // Play sound.
            AudioManager.Instance.PlaySFX("ButtonPress", false);

            // Switch canvases.
            menuCanvas.SetActive(false);
            calibrationCanvas.SetActive(true);

            // Enable cam renderer.
            camRenderer.enabled = true;
        }

        /// <summary>
        /// Exit calibration menu when pressing back button.
        /// </summary>
        private void ExitCalibration()
        {
            // Play sound.
            AudioManager.Instance.PlaySFX("ButtonPress", false);

            // Switch canvases.
            calibrationCanvas.SetActive(false);
            menuCanvas.SetActive(true);

            // Set cam settings.
            camRenderer.enabled = false;
            if(!blobs.ImageProcessing) blobs.ImageProcessing = true;
        }
    }
}
