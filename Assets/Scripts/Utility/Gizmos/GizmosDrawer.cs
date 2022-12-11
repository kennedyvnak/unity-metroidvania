#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Metroidvania {
    /// <summary>An shortcut for create gizmos</summary>
    public struct GizmosDrawer {
        /// <summary>A struct for handle colors in UnityEditor.Handles</summary>
        private readonly struct HandlesDrawingScope : IDisposable {
            /// <summary>The color of handles before change</summary>
            private readonly Color _oldColor;

            /// <summary>if true, when the scope ends, the color of the handles reverts to the color before that scope</summary>
            private readonly bool _back;

            /// <summary>A trick to create structs with custom values</summary>
            /// <param name="back">if true, when the scope ends, the color of the handles reverts to the color before that scope</param>
            public HandlesDrawingScope(bool back) {
                _back = back;
                _oldColor = Handles.color;
                Handles.color = Gizmos.color;
            }

            public void Dispose() {
                if (_back)
                    Handles.color = _oldColor;
            }
        }

        /// <summary>Set the color of gizmos</summary>
        /// <param name="color">The color that will be set</param>
        public GizmosDrawer SetColor(Color color) {
            Gizmos.color = color;
            return this;
        }

        /// <summary>Draws the outline of a flat disc in 3D space</summary>
        /// <param name="center">The center of the disc in world space</param>
        /// <param name="radius">The radius of the disc in world space units</param>
        /// <param name="thickness">Line thickness in UI points (zero thickness draws single-pixel line)</param>
        public GizmosDrawer DrawWireDisc(Vector2 center, float radius, float thickness = 0) {
            using (new HandlesDrawingScope(true))
                Handles.DrawWireDisc(center, Vector3.forward, radius, thickness);
            return this;
        }

        /// <summary>Draw a square box with center and size</summary>
        /// <param name="center">The center of the square in world space</param>
        /// <param name="size">The size of the square in world space units</param>
        public GizmosDrawer DrawWireSquare(Vector2 center, Vector2 size) {
            Vector2 vector2 = size * 0.5f;

            Vector2 a = center + new Vector2(-vector2.x, -vector2.y);
            Vector2 b = center + new Vector2(vector2.x, -vector2.y);
            Vector2 c = center + new Vector2(vector2.x, vector2.y);
            Vector2 d = center + new Vector2(-vector2.x, vector2.y);

            Gizmos.DrawLine(d, c);
            Gizmos.DrawLine(c, b);
            Gizmos.DrawLine(b, a);
            Gizmos.DrawLine(d, a);

            return this;
        }

        /// <summary>Draw a square box with center and size</summary>
        /// <param name="rect">The Rect of the square</param>
        public GizmosDrawer DrawWireSquare(Rect rect) {
            DrawWireSquare(rect.center, rect.size);
            return this;
        }

        /// <summary>Draws a ray starting at from to from + direction</summary>
        /// <param name="from">The start point of the ray</param>
        /// <param name="direction">The direction of the ray</param>
        public GizmosDrawer DrawRay(Vector2 from, Vector2 direction) {
            Gizmos.DrawRay(from, direction);
            return this;
        }

        /// <summary>Draws a line starting at from towards to</summary>
        public GizmosDrawer DrawLine(Vector2 from, Vector2 to) {
            Gizmos.DrawLine(from, to);
            return this;
        }

        public GizmosDrawer DrawPath(Pathfinding.Path path) {
            SetColor(GizmosColor.instance.pathfinding.pathColor);
            for (int i = 0; i < path.vectorPath.Count - 1; i++)
                DrawLine(path.vectorPath[i], path.vectorPath[i + 1]);
            return this;
        }
    }
}
#endif