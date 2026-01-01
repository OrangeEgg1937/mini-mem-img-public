using UnityEngine;

namespace Script.Core.Background
{
    public class BackgroundScaler : MonoBehaviour {
        public void AutoFit() {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;

            // 1. Get the sprite's original width and height in World Units
            // We reset scale to (1,1,1) temporarily to get accurate unscaled bounds
            transform.localScale = Vector3.one;
            float spriteWidth = spriteRenderer.sprite.bounds.size.x;
            float spriteHeight = spriteRenderer.sprite.bounds.size.y;

            // 2. Calculate the World Height and Width of the Camera view
            // Camera.orthographicSize is half the vertical size
            float worldScreenHeight = Camera.main.orthographicSize * 2f;
        
            // Aspect ratio is Screen Width / Screen Height
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            // 3. Calculate the scale factor needed
            // (Desired Size / Current Size)
            Vector3 newScale = transform.localScale;
            newScale.x = worldScreenWidth / spriteWidth;
            newScale.y = worldScreenHeight / spriteHeight;

            // 4. Apply the scale
            transform.localScale = newScale;
        }
    }
}