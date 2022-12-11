using UnityEngine;

namespace Metroidvania {
    /// <summary>Utility class for renderers</summary>
    public static class RenderersUtility {
        /// <summary>Shortcut for change the sprite renderer alpha</summary>
        /// <param name="renderer">Renderer that alpha will be applied</param>
        /// <param name="alpha">New renderer alpha</param>
        public static void SetAlpha(this SpriteRenderer renderer, float alpha) {
            // SpriteRenderer.color is a built-in property (externally run in c++), cache color is good for the performance
            Color gCol = renderer.color;
            renderer.color = new Color(gCol.r, gCol.g, gCol.b, alpha);
        }
    }
}