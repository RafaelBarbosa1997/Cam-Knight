using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
//using Unity.Android.Types;
using UnityEngine;

namespace CamKnight
{
    public class BlobDetection : MonoBehaviour
    {
        // Webcam feed.
        private static WebCamTexture cam;

        // Turn off image processing for testing.
        [Header("Turn processing on and off")]
        [SerializeField]
        private bool imageProcessing;

        [Header("Run every X frames")]
        [SerializeField]
        private float frameLimit;
        private float frames;

        // Colors we are looking for.
        [Header("Colors to look for")]
        [SerializeField]
        private Color tipColor;
        [SerializeField]
        private Color baseColor;

        // Colors for binarization.
        private Color tipPixelColor;
        private Color basePixelColor;
        private Color offPixelColor;

        // Threshold for finding same color in pixels.
        [Header("Similarity treshold")]
        [SerializeField]
        private float tipColorThreshold;
        [SerializeField]
        private float baseColorTreshold;

        // Pixel color array to create processed image.
        private Color32[] pixelTarget;

        // Texture2D to get processed image.
        private Texture2D textureTarget;

        // Erosion kernel.
        private bool[] erosionKernel;

        // Number of times erosion is performed.
        private int erosionAmount = 0;

        // Blob lists.
        private List<Blob> tipBlobs;
        private List<Blob> baseBlobs;

        // Blobs to be used to tracking.
        private Blob mainTipBlob;
        private Blob mainBaseBlob;

        // Distance for points to be considered part of the same blob.
        [Header("Pixel distance to be considered blob")]
        [SerializeField]
        private int blobDistance;

        // Renderer on gameObject.
        private Renderer planeRenderer;

        // Midpoints for each blob.
        private Vector2 tipMidPoint;
        private Vector2 baseMidPoint;

        // Minimum required pixel count for blobs.
        [Header("Minimum blob size")]
        [SerializeField]
        private int minBlobSize;

        private static BlobDetection activeInstance;

        public Vector2 TipMidPoint { get => tipMidPoint; private set => tipMidPoint = value; }
        public Vector2 BaseMidPoint { get => baseMidPoint; private set => baseMidPoint = value; }
        public Texture2D TextureTarget { get => textureTarget; private set => textureTarget = value; }
        public bool ImageProcessing { get => imageProcessing; set => imageProcessing = value; }
        public Color TipColor { get => tipColor; set => tipColor = value; }
        public Color BaseColor { get => baseColor; set => baseColor = value; }
        public float TipColorThreshold { get => tipColorThreshold; set => tipColorThreshold = value; }
        public float BaseColorTreshold { get => baseColorTreshold; set => baseColorTreshold = value; }


        private void Awake()
        {
            // If there is no active cam instance this becomes it.
            if (activeInstance == null)
            {
                activeInstance = this;
                DontDestroyOnLoad(gameObject);
            }

            // If there is already an active instance get rid of any other cams in the scene.
            else
            {
                if (this != activeInstance)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void Start()
        {
            // Set pixel colors.
            tipPixelColor = Color.green;
            basePixelColor = Color.blue;
            offPixelColor = new Color(0, 0, 0, 0);

            // Initialize blob lists.
            tipBlobs = new List<Blob>();
            baseBlobs = new List<Blob>();

            // Set up erosion kernel.
            erosionKernel = new bool[9]
            {
            false, true, false,
            true, true, true,
            false, false, false
            };

            // Get renderer.
            planeRenderer = GetComponent<Renderer>();

            // Create webcam texture if it's null.
            if (cam == null) cam = new WebCamTexture(300, 300);

            // If webcam isn't playing play it.
            if (cam.isPlaying == false)
            {
                cam.Play();
            }
        }

        private void Update()
        {
            if (!cam.isPlaying) cam.Play();

            frames += Time.deltaTime;

            if (frames >= frameLimit)
            {
                // Apply non-processed image to material's main texture, for testing purposes.
                if (!imageProcessing) planeRenderer.material.SetTexture("_MainTex", cam);

                // Image processing.
                else
                {
                    // Process image with binarization, erosion and dilation.
                    ProcessImage();

                    // Set texture pixels.
                    textureTarget.SetPixels32(pixelTarget);


                    // Get blob list for each pixel color.
                    tipBlobs = GetBlobs(tipPixelColor);
                    baseBlobs = GetBlobs(basePixelColor);

                    // For tip blobs.
                    if (tipBlobs.Count > 0 && GetBiggestBlog(tipBlobs) != null)
                    {
                        // Get the biggest blob for each pixel color.
                        mainTipBlob = GetBiggestBlog(tipBlobs);

                        // Set up mid point and dimensions for biggest blobs.
                        mainTipBlob.SetInfo();

                        // Set midpoints.
                        tipMidPoint = new Vector2(mainTipBlob.MidPoint.X, mainTipBlob.MidPoint.Y);
                    }

                    // For base blobs.
                    if (baseBlobs.Count > 0 && GetBiggestBlog(baseBlobs) != null)
                    {
                        // Get the biggest blob for each pixel color.
                        mainBaseBlob = GetBiggestBlog(baseBlobs);

                        // Set up mid point and dimensions for biggest blobs.
                        mainBaseBlob.SetInfo();

                        // Set midpoints.
                        baseMidPoint = new Vector2(mainBaseBlob.MidPoint.X, mainBaseBlob.MidPoint.Y);
                    }

                    // TESTING TRACKING IN TEXTURE.
                    //for (int i = 0; i < textureTarget.width; i++)
                    //{
                    //    textureTarget.SetPixel(i, (int)mainTipBlob.MidPoint.Y, Color.white);
                    //}

                    //for (int j = 0; j < textureTarget.height; j++)
                    //{
                    //    textureTarget.SetPixel((int)mainTipBlob.MidPoint.X, j, Color.white);
                    //}

                    //for (int i = 0; i < textureTarget.width; i++)
                    //{
                    //    textureTarget.SetPixel(i, (int)mainBaseBlob.MidPoint.Y, Color.yellow);
                    //}

                    //for (int j = 0; j < textureTarget.height; j++)
                    //{
                    //    textureTarget.SetPixel((int)mainBaseBlob.MidPoint.X, j, Color.yellow);
                    //}


                    // Apply SetPixels call.
                    textureTarget.Apply();

                    // Set material main texture to processed image.
                    planeRenderer.material.SetTexture("_MainTex", textureTarget);
                }

                frames = 0;
            }
        }

        public WebCamTexture GetCam()
        {
            return cam;
        }

        public void SetCam(WebCamDevice newCam)
        {
            cam.Stop();

            cam.deviceName = newCam.name;

            cam.Play();
        }

        /// <summary>
        /// Process image to return Binarized image, including erosion and dilation.
        /// </summary>
        /// <returns></returns>
        private void ProcessImage()
        {
            // Create texture.
            textureTarget = new Texture2D(cam.width, cam.height);

            // Make pixelTarget an empty array.
            pixelTarget = new Color32[cam.width * cam.height];

            // Binarize image.
            Binarize();

            // Perform erosion cycles.
            for (int i = 0; i < erosionAmount; i++)
            {
                Erosion();
            }
        }

        /// <summary>
        ///  Binarizes image.
        /// </summary>
        private void Binarize()
        {
            // Get cam pixel color array.
            Color32[] pixels = cam.GetPixels32();

            // Loop through cam pixel color array.
            for (int i = 0; i < textureTarget.width * textureTarget.height; i++)
            {
                // Set pixels that match tip color.
                if (CompareColor(pixels[i], tipColor, tipColorThreshold))
                {
                    pixelTarget[i] = tipPixelColor;
                }

                // Set pixels that match base color.
                else if (CompareColor(pixels[i], baseColor, baseColorTreshold))
                {
                    pixelTarget[i] = basePixelColor;
                }

                // Set off pixels.
                else
                {
                    pixelTarget[i] = offPixelColor;
                }
            }
        }

        /// <summary>
        ///  Performs erosion operation on image.
        ///  NOT WORKING RIGHT NOW :(.
        /// </summary>
        private void Erosion()
        {
            for (int i = 0; i < textureTarget.width * textureTarget.height; i++)
            {

                bool erode = false;

                if (pixelTarget[i] != offPixelColor && i - 4 > 0 && i + 4 < textureTarget.width * textureTarget.height)
                {
                    int index = i - 4;
                    for (int j = 0; j < erosionKernel.Length; j++)
                    {
                        if (erosionKernel[j] == true)
                        {
                            if (pixelTarget[index] == offPixelColor)
                            {
                                erode = true;
                                break;
                            }
                        }

                        index++;
                    }

                    if (erode) pixelTarget[i] = offPixelColor;
                }
            }
        }

        /// <summary>
        /// Returns a list with all the blobs belonging to a specified pixel color.
        /// </summary>
        /// <returns></returns>
        private List<Blob> GetBlobs(Color pixelColor)
        {
            // Create blob list.
            List<Blob> blobs = new List<Blob>();

            // Loop through pixel color array.
            for (int i = 0; i < pixelTarget.Length; i++)
            {
                // Only check pixel if it belongs to specified pixel color.
                if (pixelTarget[i] == pixelColor)
                {
                    // Get the coordinates for this pixel.
                    Vector2 thisPoints = GetPixelCoordinates(i, textureTarget.width, textureTarget.height);
                    System.Numerics.Vector2 thisPointsSystem = new System.Numerics.Vector2(thisPoints.x, thisPoints.y);

                    // If there aren't any blobs yet.
                    if (!blobs.Any())
                    {
                        // Create new blob and add to blob list.
                        Blob blob = new Blob((int)thisPoints.x, (int)thisPoints.y);
                        blobs.Add(blob);
                    }

                    // If there already blobs in the blob list.
                    else
                    {
                        bool partOfBlob = false;

                        // Check if this pixel belongs to any existing blobs.
                        // Based on distance to other pixels in existing blobs.
                        foreach (Blob blob in blobs)
                        {
                            if (IsPartOfBlob(blob, thisPointsSystem))
                            {
                                // Add this pixels coordinates to existing blob point list.
                                blob.AddToPointList((int)thisPoints.x, (int)thisPoints.y);
                                partOfBlob = true;
                                break;
                            }
                        }

                        // If pixel isn't part of any blob create new blob and add pixel to point list.
                        if (!partOfBlob)
                        {
                            Blob blob = new Blob((int)thisPoints.x, (int)thisPoints.y);
                            blobs.Add(blob);
                        }
                    }
                }
            }

            // Return blob list.
            return blobs;
        }

        /// <summary>
        /// Checks blob list for blob with biggest pixel count and returns it.
        /// </summary>
        /// <param name="blobs"></param>
        /// <returns></returns>
        private Blob GetBiggestBlog(List<Blob> blobs)
        {
            int biggest = 0;
            Blob selectedBlob = null;
            foreach (Blob blob in blobs)
            {
                if (blob.PointList.Count > biggest && blob.PointList.Count > minBlobSize)
                {
                    selectedBlob = blob;
                    biggest = blob.PointList.Count;
                }
            }

            return selectedBlob;
        }

        /// <summary>
        /// Checks if a pixel is part of a blob by comparing the coordinates of this pixel with the one's in blob's point list.
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsPartOfBlob(Blob blob, System.Numerics.Vector2 position)
        {
            bool closeTo = false;

            // Check if pixels are close enough to be part of blob, according to treshold.
            foreach (System.Numerics.Vector2 points in blob.PointList)
            {
                float differenceX = Mathf.Abs(points.X - position.X);
                float differenceY = Mathf.Abs(points.Y - position.Y);

                if (differenceX <= blobDistance && differenceY <= blobDistance)
                {
                    closeTo = true;
                    break;
                }
            }

            return closeTo;
        }

        /// <summary>
        /// Compares two colors.
        /// Returns true if color similarity is within treshold, false if not.
        /// </summary>
        /// <param name="pixelColor"></param>
        /// <param name="targetColor"></param>
        /// <param name="treshold"></param>
        /// <returns></returns>
        private bool CompareColor(Color pixelColor, Color targetColor, float treshold)
        {
            // Convert pixel color to HSV.
            float hP;
            float sP;
            float vP;
            Color.RGBToHSV(pixelColor, out hP, out sP, out vP);

            // Convert target color to HSV
            float hT;
            float sT;
            float vT;
            Color.RGBToHSV(targetColor, out hT, out sT, out vT);

            // Get difference of each value.
            float hDifference = Mathf.Abs(hP - hT);
            float sDifference = Mathf.Abs(sP - sT);
            float vDifference = Mathf.Abs(vP - vT);

            // Return true if all differences are within treshold.
            if (hDifference < treshold && sDifference < treshold && vDifference < treshold)
            {
                return true;
            }

            // Else return false.
            else return false;
        }

        /// <summary>
        /// Gets pixel coordinates of flattened array from index and dimensions.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Vector2 GetPixelCoordinates(int index, int width, int height)
        {
            int y = (int)Mathf.Floor(index / width);
            int x = index % width;

            return new Vector2(x, y);
        }
    }
}
