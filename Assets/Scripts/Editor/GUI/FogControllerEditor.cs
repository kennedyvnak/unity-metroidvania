using Metroidvania;
using Metroidvania.Environment.Fog;
using UnityEditor;
using UnityEngine;

namespace MetroidvaniaEditor {
    [CustomEditor(typeof(FogController))]
    public class FogControllerEditor : Editor {
        private const float k_DotSize = 0.25F;

        private FogController fog { get; set; }

        private bool editPath { get; set; }
        private bool showPoints { get; set; }

        private Vector2 dragStart { get; set; }
        private bool isDragging { get; set; }

        private void OnEnable() {
            fog = (FogController)target;
        }

        public override void OnInspectorGUI() {
            editPath = EditorGUILayout.Toggle("Edit Path", editPath);
            showPoints = EditorGUILayout.Toggle("Show Points", showPoints);
            base.OnInspectorGUI();
        }

        private void OnSceneGUI() {
            FogController.FogPlatform[] platforms = fog.GetPlatforms();

            if (platforms == null)
                fog.SetPlatforms(platforms = new FogController.FogPlatform[0]);

            for (int i = 0; i < platforms.Length; i++) {
                FogController.FogPlatform platform = platforms[i];
                fog.GetPlatformStartEnd(platform, out float startX, out float endX, out float y);

                Handles.color = GizmosColor.instance.fog.main;
                if (showPoints)
                    fog.ForEachPlatformPoint(platform, DrawDot);
                Handles.DrawDottedLine(new Vector3(startX, y), new Vector3(endX, y), 4f);

                Handles.color = GizmosColor.instance.fog.secondary;
                Handles.DrawDottedLine(new Vector3(startX, y), new Vector3(platform.a <= platform.b ? platform.a : platform.b, y), 4f);
                Handles.DrawDottedLine(new Vector3(endX, y), new Vector3(platform.a > platform.b ? platform.a : platform.b, y), 4f);
            }

            if (!editPath)
                return;

            for (int i = 0; i < platforms.Length; i++) {
                FogController.FogPlatform platform = platforms[i];
                fog.GetPlatformStartEnd(platform, out float startX, out float endX, out float y);
                float centerX = (platform.a + platform.b) * 0.5f;

                Handles.color = Handles.xAxisColor;
                platform.a = DoMoveHandle(platform.a, y).x;
                platform.b = DoMoveHandle(platform.b, y).x;

                Handles.color = Handles.yAxisColor;
                Vector2 center = DoMoveHandle(centerX, y);
                platform.y = center.y - fog.yOffset;

                float xMove = center.x - centerX;
                platform.a += xMove;
                platform.b += xMove;

                Undo.RecordObject(fog, "Move Platform Position");
                platforms[i] = platform;
            }

            Handles.color = GizmosColor.instance.fog.main;
            Event evt = Event.current;
            if (evt.shift && evt.type == EventType.MouseDown && evt.button == 0) {
                dragStart = HandleUtility.GUIPointToWorldRay(evt.mousePosition).origin;
                isDragging = true;
            }

            if (isDragging) {
                Handles.DrawDottedLine(dragStart, new Vector3(HandleUtility.GUIPointToWorldRay(evt.mousePosition).origin.x, dragStart.y), 4f);

                if (!evt.shift || evt.type == EventType.MouseUp) {
                    float dragEndX = HandleUtility.GUIPointToWorldRay(evt.mousePosition).origin.x;
                    FogController.FogPlatform newPlatform = new FogController.FogPlatform {
                        a = dragStart.x,
                        b = dragEndX,
                        y = dragStart.y - fog.yOffset
                    };

                    Undo.RecordObject(fog, "Create Platform");
                    ArrayUtility.Add(ref platforms, newPlatform);
                    fog.SetPlatforms(platforms);
                    isDragging = false;
                }
            }

            // This blocks the scene object selection
            HandleUtility.AddDefaultControl(0);
        }

        private void DrawDot(Vector2 position) => Handles.DrawWireDisc(position, Vector3.forward, 0.1f);

        private Vector2 DoMoveHandle(float x, float y) => Handles.FreeMoveHandle(new Vector3(x, y), Quaternion.identity, GetHandleSize(x, y), Vector3.zero, Handles.SphereHandleCap);
        private float GetHandleSize(float x, float y) => HandleUtility.GetHandleSize(new Vector3(x, y)) * k_DotSize;
    }
}
