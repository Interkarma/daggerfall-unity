using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A horizontal progress/indicator bar. Modified to smoothly change
    /// </summary>
    public class HorizontalProgressSmoother : VerticalProgressSmoother
    {
        protected override void DrawProgress()
        {
            Rect srcRect = new Rect(0, 0, 1 * amount, 1);
            Rect dstRect = Rectangle;
            float scaledAmount = Mathf.Round(dstRect.width * amount);
            dstRect.width = scaledAmount;

            if (ProgressTexture)
                DaggerfallUI.DrawTextureWithTexCoords(dstRect, ProgressTexture, srcRect, false);
            else if (colorTexture)
            {
                DaggerfallUI.DrawTextureWithTexCoords(dstRect, colorTexture, srcRect, false, color);
            }
        }
    }
}
