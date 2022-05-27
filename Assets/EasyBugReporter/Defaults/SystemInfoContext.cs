using UnityEngine;

namespace EasyBugReporter {

    /// <summary>
    /// Reports system stats as according to SystemInfo.
    /// </summary>
    public class SystemInfoContext : IReportSystem {
        public bool GatherReports(IReportWriter writer) {
            writer.BeginSection("System Information", false);
            writer.Text(SystemInfoExt.Report(false));
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
}