using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace EasyBugReporter {
    public interface IReportSource {
        bool GatherReports(IReportWriter gatherer);
    }

    public interface IReportSystem : IReportSource {
        void Initialize();
        void Freeze();
        void Unfreeze();
        void Shutdown();
    }

    public class ReporterCollection : ICollection<IReportSource> {
        protected readonly HashSet<IReportSource> m_Sources = new HashSet<IReportSource>(); 
        protected readonly HashSet<IReportSystem> m_Systems = new HashSet<IReportSystem>();

        private bool m_InitializeState = false;
        private bool m_FrozenState = false;

        public void Initialize() {
            if (!m_InitializeState) {
                m_InitializeState = true;
                foreach(var sys in m_Systems) {
                    sys.Initialize();
                }
            }
        }

        public void PreReport() {
            if (!m_InitializeState) {
                throw new InvalidOperationException("Must call Initialize before PreReport");
            }
            if (m_FrozenState) {
                throw new InvalidOperationException("Must call PostReport after any PreReport call before calling PreReport again");
            }
            
            m_FrozenState = true;
            foreach(var sys in m_Systems) {
                sys.Freeze();
            }
        }

        public void GatherReports(IReportWriter writer) {
            if (!m_InitializeState || !m_FrozenState) {
                throw new InvalidOperationException("Cannot gather reports if Initialize or PreReport have not been called");
            }

            foreach(var source in m_Sources) {
                source.GatherReports(writer);
            }
        }

        public void PostReport() {
            if (!m_FrozenState) {
                throw new InvalidOperationException("Must call PostReport after PreReport");
            }
            
            m_FrozenState = false;
            foreach(var sys in m_Systems) {
                sys.Unfreeze();
            }
        }

        public void Shutdown() {
            if (m_FrozenState) {
                throw new InvalidOperationException("Must call PostReport before Shutdown if PreReport was called");
            }

            if (m_InitializeState) {
                m_InitializeState = false;
                foreach(var sys in m_Systems) {
                    Shutdown();
                }
            }
        }

        #region ICollection

        public int Count { get { return m_Sources.Count; } }

        public bool IsReadOnly  { get { return false; } }

        public void Add(IReportSource item) {
            if (m_Sources.Add(item)) {
                IReportSystem sys = item as IReportSystem;
                if (sys != null && m_Systems.Add(sys)) {
                    if (m_InitializeState) {
                        sys.Initialize();
                    }
                    if (m_FrozenState) {
                        sys.Freeze();
                    }
                }
            }
        }

        public void Clear() {
            m_Sources.Clear();
            if (m_InitializeState) {
                foreach(var system in m_Systems) {
                    system.Shutdown();
                }
            }
            m_Systems.Clear();
        }

        public bool Contains(IReportSource item) {
            return m_Sources.Contains(item);
        }

        public void CopyTo(IReportSource[] array, int arrayIndex) {
            m_Sources.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IReportSource> GetEnumerator() {
            return m_Sources.GetEnumerator();
        }

        public bool Remove(IReportSource item) {
            if (m_Sources.Remove(item)) {
                IReportSystem sys = item as IReportSystem;
                if (sys != null && m_Systems.Contains(sys)) {
                    if (m_FrozenState) {
                        sys.Freeze();
                    }
                    if (m_InitializeState) {
                        sys.Shutdown();
                    }
                    m_Systems.Remove(sys);
                }
                return true;
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion // ICollection
    }
}