using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FileManagement
{
    public static class FileManager
    {
        public static string[] filePaths
        {
            get
            {
                string[] paths = new string[files.Length];
                for (int i = 0; i < paths.Length; i++)
                    paths[i] = files[i].path;

                return paths;
            }
        }

        public static string[] fileNames
        {
            get
            {
                if (_files == null) findFiles();

                Dictionary<string, File>.KeyCollection keys = _files.Keys;

                List<string> names = new List<string>();
                foreach (string key in keys)
                    names.Add(key);

                return names.ToArray();
            }
        }

        private static Dictionary<string, File> _files = null;
        public static File[] files
        {
            get
            {
                if (_files == null) findFiles();

                File[] files = new File[_files.Count];
                _files.Values.CopyTo(files, 0);
                return files;
            }
        }

        public static void findFiles()
        {
            _files = new Dictionary<string, File>();
            string[] paths = Directory.GetFiles(SaveSystem.sourceFilesPath);

            foreach (string path in paths)
                _files.Add(Path.GetFileName(path), new File(path));
        }

        public class File
        {
            //string fileName; - is key
            public string path;
            //public BlockSave blockSave;
            //public bool open;

            public File(string path)
            {
                this.path = path;
                // open = BlockSave.open;
            }
        }
    }

    public static class SaveSystem
    {
        public static string sourceFilesPath
        {
            get { return makeDirectory(Application.persistentDataPath + "/source"); }
        }
        public static string blockFilesPath
        {
            get { return makeDirectory(Application.persistentDataPath + "/block"); }
        }

        public static string makeDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
            catch { return null; }
        }

        public static bool saveCode(string fileName, string code)
        {
            string filePath = sourceFilesPath + "/" + fileName + ".cs";
            try
            {
                using (StreamWriter outputFile = new StreamWriter(filePath))
                {
                    outputFile.WriteLine(code);
                }
                Debug.Log("Saved to " + filePath);
                return true;
            }
            catch
            {
                Debug.Log("Issue writing to " + filePath);
                return false;
            }
        }
    }
}
