using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class DebugCommands 
{
    [QFSW.QC.Command("open-log-file")]
    public static void OpenLogFile()
    {
        var log_path = CombinePaths(Environment.GetEnvironmentVariable("AppData"), "..", "LocalLow", Application.companyName, Application.productName, "Player.log");

        string notepadPath = "C:/Program Files/Notepad++/notepad++.exe";

        ProcessStartInfo startInfo = new ProcessStartInfo(notepadPath, log_path);
        Process.Start(startInfo);
    }

    public static string CombinePaths(string path1, params string[] paths)
    {
        if (path1 == null)
        {
            throw new ArgumentNullException("path1");
        }
        if (paths == null)
        {
            throw new ArgumentNullException("paths");
        }
        return paths.Aggregate(path1, (acc, p) => Path.Combine(acc, p));
    }
}
