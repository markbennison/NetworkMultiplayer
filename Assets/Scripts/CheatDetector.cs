using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class CheatDetector : NetworkBehaviour
{
    string LatestLogEntry;
    void Start()
    {
        LatestLogEntry = DateTime.Now + ": Starting ... \n";
        WriteToCheatLog (LatestLogEntry);
    }


    void FixedUpdate()
    {
        if (isServer)
        {
            string checkLog = GameManager.AmmoCheckLog();
            if (LatestLogEntry != checkLog)
            {
                LatestLogEntry = checkLog;
                WriteToCheatLog(checkLog);
                Debug.Log(checkLog);
            }
        }
    }

    void WriteToCheatLog(string text)
    {
        string path = "Assets/Logs/CheatLog.txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(text);
        writer.Close();
    }

}
