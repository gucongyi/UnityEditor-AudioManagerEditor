using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileHelper : MonoBehaviour {

    public static void createOrWriteJsonFile(string jsonString)
    {
        string AudioJsonDir = Path.Combine(Application.persistentDataPath, @"AudioJson");
        if (!Directory.Exists(AudioJsonDir))
        {
            // Create the directory it does not exist.
            Directory.CreateDirectory(AudioJsonDir);
        }
        var fullFileName = Path.Combine(AudioJsonDir, "AudioJson.json");
        Stream fileHandler = null;
        if (File.Exists(fullFileName))
            fileHandler = File.Open(fullFileName, FileMode.Create);//复写方式
        else
            fileHandler = File.Create(fullFileName);
        if (fileHandler == null)
        {
            Debug.Assert(false);
            return;
        }

        using (var streamWriter = new StreamWriter(fileHandler))
        {
            streamWriter.Write(jsonString);
            streamWriter.Flush();
        }
        fileHandler.Close();
        fileHandler.Dispose();
        fileHandler = null;
    }

   

    public static string LoadAndJsonNameFile()
    {
        string JsonDir = Path.Combine(Application.persistentDataPath, @"AudioJson");
        if (!Directory.Exists(JsonDir))
        {
            // Create the directory it does not exist.
            Directory.CreateDirectory(JsonDir);
            return string.Empty;
        }
        var fullFileName = Path.Combine(JsonDir, "AudioJson.json");
        if (!File.Exists(fullFileName))
        {
            return string.Empty;
        }

        using (var fileHandler = File.Open(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            if (fileHandler == null)
                return string.Empty;
            using (var fileReader = new StreamReader(fileHandler))
            {
                string jsonString = string.Empty;
                jsonString=fileReader.ReadToEnd();
                return jsonString;
            }
        }
        return string.Empty;
    }
    
}
