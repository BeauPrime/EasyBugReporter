using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace EasyBugReporter {

    /// <summary>
    /// Reports loaded scenes and gathers reports from IUnityReportContext components.
    /// </summary>
    public class UnityContext : IReportSystem {
        public bool GatherReports(IReportWriter writer) {
            writer.BeginSection("Loaded Scenes");

            writer.Text("Time Since Startup: " + Time.realtimeSinceStartup.ToString() + " seconds");
            writer.Text("Profiler Memory Usage Report: " + Profiler.GetTotalAllocatedMemoryLong());
            
            HashSet<IUnityReportContext> unityReporters = new HashSet<IUnityReportContext>();
            List<IUnityReportContext> tempUnityReporters = new List<IUnityReportContext>(64);
            int sceneCount = SceneManager.sceneCount;
            for(int i = 0; i < sceneCount; i++) {
                Scene scene = SceneManager.GetSceneAt(i);
                writer.Text("Loaded Scene: " + scene.path);
            }

            writer.EndSection();

            writer.BeginSection("World");

            foreach(var obj in GameObject.FindObjectsOfType<GameObject>()) {
                obj.GetComponentsInChildren<IUnityReportContext>(true, tempUnityReporters);
                foreach(var reporter in tempUnityReporters) {
                    unityReporters.Add(reporter);
                }
            }

            foreach(var reportContext in unityReporters) {
                if (!reportContext.GetType().IsDefined(typeof(AlwaysReportAttribute), true)) {
                    Component c = reportContext as Component;
                    Behaviour b = c as Behaviour;
                    if (b != null && !b.isActiveAndEnabled) {
                        continue;
                    } else if (c != null && !c.gameObject.activeInHierarchy) {
                        continue;
                    }
                }

                try {
                    reportContext.GatherReports(writer);
                }
                catch(Exception e) {
                    UnityEngine.Debug.LogException(e);
                }
            }

            writer.EndSection();

            return true;
        }

        public void Initialize() {
        }

        public void Shutdown() {
        }

        public void Freeze() {
        }

        public void Unfreeze() {
        }
    }

    /// <summary>
    /// Indicates that this MonoBehaviour can report context.
    /// </summary>
    public interface IUnityReportContext : IReportSource {
    }

    /// <summary>
    /// Apply this attribute to a class that inherits from IUnityReportCallback
    /// to allow it to report when the component or gameobject is inactive.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AlwaysReportAttribute : Attribute {
    }
}