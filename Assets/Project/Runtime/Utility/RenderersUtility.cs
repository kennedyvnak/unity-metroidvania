using UnityEngine;

namespace Metroidvania
{
    public static class RenderersUtility
    {
        public static void SetAlpha(this SpriteRenderer graphic, float alpha)
        {
            var gCol = graphic.color;
            graphic.color = new Color(gCol.r, gCol.g, gCol.b, alpha);
        }
    }
}