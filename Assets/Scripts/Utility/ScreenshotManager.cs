using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Metroidvania
{
    public class ScreenshotManager : SingletonPersistent<ScreenshotManager>
    {
        [SerializeField] private string m_SaveFolder = "Screenshots/screenshot-{ticks}.png";
        [SerializeField] private string m_TakeScreenshotKey = "l";

        private KeyControl _keyControl;

        private void Start()
        {
            _keyControl = Keyboard.current.FindKeyOnCurrentKeyboardLayout(m_TakeScreenshotKey);
        }

        private void Update()
        {
            if (_keyControl.wasPressedThisFrame)
                TakeScreenshot();
        }

        public void TakeScreenshot()
        {
            var fileName = m_SaveFolder.Replace("{ticks}", DateTime.Now.Ticks.ToString());
            ScreenCapture.CaptureScreenshot(fileName);
            Debug.Log($"Screenshot captured at {fileName}");
        }
    }
}
