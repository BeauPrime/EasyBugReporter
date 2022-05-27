using System;
using System.Collections;
using System.IO;
using EasyBugReporter;
using UnityEngine;

public class TestThing : MonoBehaviour {
    private IEnumerator Start() {
        ReporterCollection reporters = new ReporterCollection();
        reporters.Add(new ScreenshotContext());
        reporters.Add(new LogContext());
        reporters.Add(new SystemInfoContext());
        reporters.Add(new UnityContext());
        reporters.Initialize();

        Debug.Log("something happened");
        yield return new WaitForSeconds(0.2f);
        Debug.LogWarning("Another thing!");
        yield return new WaitForSeconds(0.5f);
        Debug.LogError("An ERROR!!!!!");
        yield return null;

        BugReporter.DisplayContext(reporters);
    }
}