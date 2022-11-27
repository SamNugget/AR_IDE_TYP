using System;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static void saveCode(string fileName, string code)
    {
        string path = Application.persistentDataPath + "/" + fileName;

        try
        {
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                outputFile.WriteLine(code);
            }
            Debug.Log("Saved to " + path);
        }
        catch
        {
            Debug.Log("Issue writing to " + path);
            return;
        }
    }
}
