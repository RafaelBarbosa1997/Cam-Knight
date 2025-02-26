using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CamKnight
{
    public class CalibrationSettings : MonoBehaviour
    {
        private BlobDetection blobs;
        private SwordTracking sword;

        [Header("Back button")]
        [SerializeField]
        private Button back;

        [Header("Camera buttons")]
        [SerializeField]
        private Button previousCam;
        [SerializeField]
        private Button nextCam;

        [Header("Process button")]
        [SerializeField]
        private Button process;

        [Header("Color buttons")]
        [SerializeField]
        private Button tipColor;
        [SerializeField]
        private Button baseColor;
        [SerializeField]
        private GameObject tipIndicator;
        [SerializeField]
        private GameObject baseIndicator;

        [Header("Tip slider")]
        [SerializeField]
        private TMP_Text tipValue;
        [SerializeField]
        private Slider tipSlider;

        [Header("Base slider")]
        [SerializeField]
        private TMP_Text baseValue;
        [SerializeField]
        private Slider baseSlider;

        [Header("Filter slider")]
        [SerializeField]
        private TMP_Text filterValue;
        [SerializeField]
        private Slider filterSlider;

        private Image tipButtonImage;
        private Image baseButtonImage;

        private bool selectingTip;
        private bool selectingBase;

        private void Start()
        {
            // Get blob detection reference.
            blobs = FindObjectOfType<BlobDetection>();

            // Get sword tracking reference.
            sword = FindObjectOfType<SwordTracking>();

            // Get button images to change button color.
            tipButtonImage = tipColor.GetComponent<Image>();
            baseButtonImage = baseColor.GetComponent<Image>();

            // Set initial button color.
            tipButtonImage.color = blobs.TipColor;
            baseButtonImage.color = blobs.BaseColor;

            // Add listeners to buttons.
            tipColor.onClick.AddListener(SetTipColor);
            baseColor.onClick.AddListener(SetBaseColor);
            process.onClick.AddListener(ToggleImageProcessing);
            nextCam.onClick.AddListener(SetNextCam);
            previousCam.onClick.AddListener(SetPrevCam);

            // Tip slider.
            tipValue.text = blobs.TipColorThreshold.ToString();
            tipSlider.value = blobs.TipColorThreshold;
            tipSlider.onValueChanged.AddListener(delegate { ChangeTipSlider(); });

            // Base slider.
            baseValue.text = blobs.BaseColorTreshold.ToString();
            baseSlider.value = blobs.BaseColorTreshold;
            baseSlider.onValueChanged.AddListener(delegate { ChangeBaseSlider(); });

            // Filter slider.
            filterValue.text = sword.FilterTreshold.ToString();
            filterSlider.value = sword.FilterTreshold;
            filterSlider.onValueChanged.AddListener(delegate { ChangeFilterSlider(); });
        }

        private void Update()
        {
            // Select tip color.
            if (Input.GetMouseButtonDown(0) && selectingTip)
            {
                Color color = SelectColor();
                if (color != new Color(0, 0, 0))
                {
                    blobs.TipColor = color;
                    tipButtonImage.color = color;
                }


                selectingTip = false;
                tipIndicator.SetActive(false);

                EnableUI();
            }

            // Select base color.
            if (Input.GetMouseButtonDown(0) && selectingBase)
            {
                Color color = SelectColor();
                if (color != new Color(0, 0, 0))
                {
                    blobs.BaseColor = color;
                    baseButtonImage.color = color;
                }

                selectingBase = false;
                baseIndicator.SetActive(false);

                EnableUI();
            }

            // Cancel color selection.
            if (Input.GetMouseButtonDown(1))
            {
                selectingTip = false;
                selectingBase = false;

                tipIndicator.SetActive(false);
                baseIndicator.SetActive(false);

                EnableUI();
            }
        }

        /// <summary>
        /// Toggles image processing.
        /// </summary>
        private void ToggleImageProcessing()
        {
            blobs.ImageProcessing = !blobs.ImageProcessing;
        }

        private void SetNextCam()
        {
            WebCamDevice[] cams = WebCamTexture.devices;
            WebCamTexture currentCam = blobs.GetCam();
            
            int index = 0;
            for (int i = 0; i < cams.Length; i++)
            {
                if (cams[i].name == currentCam.deviceName)
                {
                    index = i;
                    break;
                }
            }

            if (index + 1 >= cams.Length) index = 0;

            else index++;    

            blobs.SetCam(cams[index]);
        }

        private void SetPrevCam()
        {
            WebCamDevice[] cams = WebCamTexture.devices;
            WebCamTexture currentCam = blobs.GetCam();

            int index = 0;
            for (int i = 0; i < cams.Length; i++)
            {
                if (cams[i].name == currentCam.deviceName)
                {
                    index = i;
                    break;
                }
            }

            if (index - 1 < 0) index = cams.Length - 1;
            else index--;

            blobs.SetCam(cams[index]);
        }

        /// <summary>
        /// Begin color selection for tip color.
        /// </summary>
        private void SetTipColor()
        {
            if (blobs.ImageProcessing) return;

            selectingTip = true;

            tipIndicator.SetActive(true);

            DisableUI();
        }

        /// <summary>
        /// Begin color selection for base color.
        /// </summary>
        private void SetBaseColor()
        {
            if (blobs.ImageProcessing) return;

            selectingBase = true;

            baseIndicator.SetActive(true);

            DisableUI();
        }

        /// <summary>
        /// Change tip color treshold with slider.
        /// </summary>
        private void ChangeTipSlider()
        {
            tipValue.text = tipSlider.value.ToString();

            blobs.TipColorThreshold = tipSlider.value;
        }

        /// <summary>
        /// Change base color treshold with slider.
        /// </summary>
        private void ChangeBaseSlider()
        {
            baseValue.text = baseSlider.value.ToString();

            blobs.BaseColorTreshold = baseSlider.value;
        }

        /// <summary>
        /// Change filter treshold with slider.
        /// </summary>
        private void ChangeFilterSlider()
        {
            filterValue.text = filterSlider.value.ToString();

            sword.FilterTreshold = filterSlider.value;
        }

        /// <summary>
        /// Disable UI elements when selection color.
        /// </summary>
        private void DisableUI()
        {
            process.enabled = false;

            tipColor.enabled = false;
            baseColor.enabled = false;

            back.enabled = false;

            tipSlider.enabled = false;
            baseSlider.enabled = false;
            filterSlider.enabled = false;
        }

        /// <summary>
        /// Enable UI elements when done selecting color.
        /// </summary>
        private void EnableUI()
        {
            process.enabled = true;

            tipColor.enabled = true;
            baseColor.enabled = true;

            back.enabled = true;

            tipSlider.enabled = true;
            baseSlider.enabled = true;
            filterSlider.enabled = true;
        }

        /// <summary>
        /// Get color of clicked pixel from raycast.
        /// </summary>
        /// <returns></returns>
        private Color SelectColor()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.tag == "Cam")
                {
                    Renderer renderer = hit.collider.GetComponent<MeshRenderer>();
                    WebCamTexture texture = renderer.material.mainTexture as WebCamTexture;
                    Vector2 coordinates = hit.textureCoord;

                    coordinates.x *= texture.width;
                    coordinates.y *= texture.height;

                    Vector2 tiling = renderer.material.mainTextureScale;
                    Color color = texture.GetPixel(Mathf.FloorToInt(coordinates.x * tiling.x), Mathf.FloorToInt(coordinates.y * tiling.y));

                    return color;
                }
            }

            return new Color(0, 0, 0);
        }
    }
}
