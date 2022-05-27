using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace EasyBugReporter {
    public class ScreenshotContext : IReportSystem {
        private Texture2D m_Texture;

        public void Freeze() {
            if (m_Texture) {
                GameObject.DestroyImmediate(m_Texture);
                m_Texture = null;
            }
            
            BugReporter.OnEndOfFrame(TakeScreenshot);
        }

        public bool GatherReports(IReportWriter writer) {
            if (!m_Texture) {
                return false;
            }

            writer.BeginSection("Screenshot");
            writer.Image(m_Texture, "current screenshot");
            writer.EndSection();

            return true;
        }

        public void Initialize() {
        }

        public void Shutdown() {
            if (m_Texture) {
                GameObject.DestroyImmediate(m_Texture);
                m_Texture = null;
            }
        }

        public void Unfreeze() {
            if (m_Texture) {
                GameObject.DestroyImmediate(m_Texture);
                m_Texture = null;
            }
        }

        private void TakeScreenshot() {
            m_Texture = ScreenCapture.CaptureScreenshotAsTexture();
        }
    }
}