using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CamKnight
{
    public class Blob
    {
        // The coordinates of the middle pixel of the blob.
        private Vector2 midPoint;

        // Width and height of blob.
        private int width;
        private int height;

        // Used to get width and height.
        public int minX;
        public int maxX;
        public int minY;
        public int maxY;

        // Stores the coordinates of all the pixels in this blob.
        private List<Vector2> pointList;

        /// <summary>
        /// Default constructor takes coordinates to add first pixel to point list.
        /// </summary>
        /// <param name="pointX"></param>
        /// <param name="pointY"></param>
        public Blob(int pointX, int pointY) 
        {
            if(pointList == null) pointList = new List<Vector2>();

            AddToPointList(pointX, pointY);
        }

        public List<Vector2> PointList { get => pointList; private set => pointList = value; }
        public Vector2 MidPoint { get => midPoint; private set => midPoint = value; }
        public int Width { get => width; private set => width = value; }
        public int Height { get => height; private set => height = value; }

        /// <summary>
        /// Add a pixel's coordinates to the point list.
        /// </summary>
        /// <param name="pointX"></param>
        /// <param name="pointY"></param>
        public void AddToPointList(int pointX, int pointY)
        {
            // Add coordinates to point list.
            Vector2 thisPoint = new Vector2(pointX, pointY);

            pointList.Add(thisPoint);

            // Update boundaries.
            CheckNewBoundaries(pointX, pointY);
        }

        /// <summary>
        ///  Set the width, height and midpoint for this blob.
        /// </summary>
        public void SetInfo()
        {
            // Set width and height.
            width = maxX - minX;
            height = maxY - minY;

            // Get X and Y values for midpoint.
            int midIndex = (int)MathF.Floor(pointList.Count / 2);

            // Set midpoint.
            midPoint = pointList[midIndex];
        }

        /// <summary>
        /// Update the min and max values for coordinates to calculate width and height later.
        /// </summary>
        /// <param name="pointX"></param>
        /// <param name="pointY"></param>
        private void CheckNewBoundaries(int pointX, int pointY)
        {
            // Get new min and max for point X.
            if(pointX < minX) minX = pointX;
            else if (pointX > maxX) maxX = pointX;

            // Get new min and max for point Y.
            if(pointY < minY) minY = pointY;
            else if(pointY > maxY) maxY = pointY;
        }
    }
}
