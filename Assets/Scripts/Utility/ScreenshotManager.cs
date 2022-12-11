using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Metroidvania {
    public class ScreenshotManager : SingletonPersistent<ScreenshotManager> {
        private const string k_SaveFolder = "{dir}/Screenshots/screenshot-{time}.png";
        [SerializeField] private string m_TakeScreenshotKey = "l";

        private KeyControl _keyControl;

        private void Start() {
            _keyControl = Keyboard.current.FindKeyOnCurrentKeyboardLayout(m_TakeScreenshotKey);
        }

        private IEnumerator TakeScreenshot() {
            yield return CoroutinesUtility.GetWaitForEndOfFrame();
            int width = Screen.width;
            int height = Screen.height;

            Texture2D screenshotTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, width, height);
            screenshotTex.ReadPixels(rect, 0, 0);
            screenshotTex.Apply();

            byte[] textureBytes = screenshotTex.EncodeToPNG();

            string filePath = k_SaveFolder.Replace("{time}", System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")).Replace("{dir}", Application.persistentDataPath);
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            File.WriteAllBytes(filePath, textureBytes);

            Debug.Log($"Captured a screenshot at '{filePath}'");
        }

        private void Update() {
            if (_keyControl?.wasPressedThisFrame == true)
                StartCoroutine(TakeScreenshot());
        }
    }
}
