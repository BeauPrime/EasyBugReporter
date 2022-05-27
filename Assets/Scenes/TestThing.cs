using System;
using System.Collections;
using System.IO;
using EasyBugReporter;
using UnityEngine;

public class TestThing : MonoBehaviour {
    private IEnumerator Start() {
        DumpSourceCollection sources = new DumpSourceCollection();
        sources.Add(new ScreenshotContext());
        sources.Add(new LogContext());
        sources.Add(new SystemInfoContext());
        sources.Add(new UnityContext());
        sources.Initialize();

        Debug.Log("something happened");
        yield return new WaitForSeconds(0.2f);
        Debug.LogWarning("Another thing!");
        yield return new WaitForSeconds(0.5f);
        Debug.LogError("An ERROR!!!!!");
        yield return null;

        BugReporter.DumpContext(sources);
    }
}