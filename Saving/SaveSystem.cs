using System;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static bool saveCode(string fileName, string code)
    {
        string path = Application.persistentDataPath + "/source/" + fileName;

        if (!Directory.Exists(path))
            Directory.CreateDirectory(Application.persistentDataPath + "/source");

        try
        {
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                outputFile.WriteLine(code);
            }
            Debug.Log("Saved to " + path);
            return true;
        }
        catch
        {
            Debug.Log("Issue writing to " + path);
            return false;
        }
    }
}
