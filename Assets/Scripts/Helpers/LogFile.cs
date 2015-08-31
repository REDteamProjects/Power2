using System;
using UnityEngine;
using System.Collections;
using System.IO;

public class LogFile
{
    //-------------------------------------------------------------------------------------------------------------------------
    // A simple log to file system.

    //-------------------------------------------------------------------------------------------------------------------------
    internal bool Echo;
    public string FileName;
    private static LogFile _instanse;

    public static LogFile Instanse
    {
        get { return _instanse ?? (_instanse = new LogFile("logfile" + DateTime.Now.ToString("yyyy-MM-dd"), true)); }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public LogFile(string filename, bool echoToDebug)
    {
        FileName = Path.Combine(Application.persistentDataPath, filename);

        using (File.Open(FileName, FileMode.OpenOrCreate)) { }
        Echo = echoToDebug;

    }

    //-------------------------------------------------------------------------------------------------------------------------
    public static void Message(string msg, bool debugMessage = false)
    {
        if (debugMessage && !Debug.isDebugBuild) return;

        using (var stream = new StreamWriter(File.Open(Instanse.FileName, FileMode.Append)))
        {
#if UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1
            stream.WriteLine(DateTime.Now.ToString("u") + "\t" + msg + "\t");
#else
            stream.WriteLine(DateTime.Now.ToString("u") + "\t" + msg + "\t" + Environment.StackTrace);
#endif   
        }

        if (Instanse.Echo)
            Debug.Log(msg);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    //public void Close()
    //{
    //    if (LogStream == null) return;
    //    LogStream.Close();
    //    LogStream = null;
    //    GC.SuppressFinalize(this);
    //}

    //~LogFile()
    //{
    //    if (LogStream == null) return;
    //    LogStream.Close();
    //    LogStream = null;
    //}
}
