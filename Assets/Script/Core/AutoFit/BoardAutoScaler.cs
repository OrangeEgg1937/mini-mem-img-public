using UnityEngine;

namespace Script.Core.AutoFit
{
    public class BoardAutoScaler : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Leave as 0 if you want to auto-detect size from this object's SpriteRenderer.")]
        public float manualBoardSize = 0f;
    
        [Tooltip("Padding from the edges of the width (0.5 = 0.5 world units)")]
        public float sidePadding = 0.5f;

        void Start()
        {
            FitBoardToScreen();
        }

        void FitBoardToScreen()
        {
            Camera cam = Camera.main;

            // 1. Get the camera's height and width in World Units
            // Orthographic size is half the height, so we multiply by 2.
            float screenHeight = 2f * cam.orthographicSize;
            float screenWidth = screenHeight * cam.aspect;

            // 2. Define the "Safe Zone" (Middle 70% of the screen)
            // We remove 15% from top and 15% from bottom => 30% removed.
            float safeHeight = screenHeight * 0.7f;

            // 3. Determine the max square size
            // The board must fit within the Width AND the Safe Height.
            // We pick the smaller dimension to ensure it never overflows.
            float targetSize = Mathf.Min(screenWidth - (sidePadding * 2), safeHeight);

            // 4. Get the current unscaled size of the board
            float currentSize = GetCurrentSize();

            // 5. Calculate Scale Factor
            // Formula: Target / Current
            float newScale = targetSize / currentSize;

            // 6. Apply Scale and Center Position
            transform.localScale = new Vector3(newScale, newScale, 1f);
            transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
        }

        // Helper to get the size based on Sprite or Manual Input
        float GetCurrentSize()
        {
            // If user typed a manual size, use that (useful for empty parent objects)
            if (manualBoardSize > 0) 
                return manualBoardSize;

            // Otherwise try to get the size from the sprite renderer
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                return sr.sprite.bounds.size.x; // Assumes sprite is roughly square
            }

            Debug.LogError("No SpriteRenderer found and no Manual Size set!");
            return 1f;
        }
    }
}