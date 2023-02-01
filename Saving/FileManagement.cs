using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FileManagement
{
    public static class FileManager
    {
        public static string[] workspaceNames
        {
            get
            {
                string[] paths = Directory.GetDirectories(DirectoryManager.workspacesPath);
                string[] names = new string[paths.Length];
                for (int i = 0; i < paths.Length; i++)
                    names[i] = Path.GetFileName(paths[i]);
                return names;
            }
        }

        public static string[] sourceFileNames
        {
            get
            {
                Dictionary<string, ReferenceTypeS>.KeyCollection keys = activeWorkspace._sourceFiles.Keys;

                string[] names = new string[keys.Count];
                keys.CopyTo(names, 0);
                return names;
            }
        }

        public static string[] sourceFileTypes
        {
            get
            {
                Dictionary<string, ReferenceTypeS>.ValueCollection values = activeWorkspace._sourceFiles.Values;
                
                List<string> types = new List<string>();
                foreach (ReferenceTypeS rTS in values)
                    types.Add(rTS.isClass ? "Class" : "Interface");
                return types.ToArray();
            }
        }



        private static bool prettyPrint = false;

        private static Workspace activeWorkspace = null;

        public static void loadWorkspace(string name)
        {
            string workspacePath = DirectoryManager.makeDirectory(DirectoryManager.workspacesPath + '/' + name);

            // vv allows people to exit and re-enter workspace without reloading
            if (activeWorkspace == null || activeWorkspace.path != workspacePath)
                activeWorkspace = new Workspace(workspacePath);
        }
        


        public static ReferenceTypeS getSourceFile(string name)
        {
            return activeWorkspace._sourceFiles[name];
        }

        public static bool saveSourceFile(string name)
        {
            return activeWorkspace.saveSourceFile(name);
        }

        public static ReferenceTypeS createSourceFile(string name)
        {
            return activeWorkspace.createSourceFile(name);
        }

        public static bool saveAllFiles()
        {
            return activeWorkspace.saveAllFiles();
        }



        public static List<BlockVariantS> loadCustomBlockVariants()
        {
            return activeWorkspace.loadCustomBlockVariants();
        }

        public static void saveCustomBlockVariants()
        {
            activeWorkspace.saveCustomBlockVariants();
        }



        public class Workspace
        {
            public string path;
            public Dictionary<string, ReferenceTypeS> _sourceFiles = null;

            public Workspace(string path)
            {
                this.path = path;
                findSourceFiles();
            }

            public void findSourceFiles()
            {
                _sourceFiles = new Dictionary<string, ReferenceTypeS>();

                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                {
                    string[] files = Directory.GetFiles(directory);
                    foreach (string file in files)
                    {
                        if (file.Contains(".json"))
                        {
                            ReferenceTypeS f = loadSourceFile(file);
                            // dictionary key is file name
                            if (f != null)
                            {
                                _sourceFiles.Add(f.name, f);
                                Debug.Log("Loaded source file " + Path.GetFileName(file)); // TEMP
                            }
                            break;
                        }
                    }
                }
            }



            public static ReferenceTypeS loadSourceFile(string path)
            {
                if (!File.Exists(path))
                {
                    Debug.Log("Path does not exist " + path);
                    return null;
                }

                try
                {
                    using (StreamReader r = new StreamReader(path))
                    {
                        string json = r.ReadToEnd();
                        ReferenceTypeS sourceFile = JsonUtility.FromJson<ReferenceTypeS>(json);
                        return sourceFile;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Issue with the formatting of text file " + path);
                    return null;
                }
            }

            public bool saveSourceFile(string name)
            {
                try
                {
                    if (!_sourceFiles.ContainsKey(name))
                    {
                        Debug.Log("Source file " + name + " does not exist");
                        return false;
                    }
                    // get ref to source file object
                    ReferenceTypeS sourceFile = _sourceFiles[name];
                    sourceFile.save();

                    // check there is a directory
                    DirectoryManager.makeDirectory(sourceFile.path);

                    // save the source code
                    using (StreamWriter w = new StreamWriter(sourceFile.path + '/' + sourceFile.name + ".cs"))
                        w.WriteLine(sourceFile.getCode());
                    // save the block structure
                    using (StreamWriter w = new StreamWriter(sourceFile.path + '/' + sourceFile.name + ".json"))
                        w.WriteLine(JsonUtility.ToJson(sourceFile, prettyPrint));

                    return true;
                }
                catch (Exception e)
                {
                    Debug.Log(e.StackTrace);
                    Debug.Log("Issue saving source file " + name + '.');
                    return false;
                }
            }

            public ReferenceTypeS createSourceFile(string name)
            {
                if (_sourceFiles.ContainsKey(name))
                {
                    Debug.Log("File already exists");
                    return null;
                }

                ReferenceTypeS rTS = new ReferenceTypeS(path + '/' + name, name);
                _sourceFiles.Add(name, rTS);
                return rTS;
            }

            public bool saveAllFiles()
            {
                foreach (string file in _sourceFiles.Keys)
                    if (!saveSourceFile(file)) return false;
                return true;
            }



            public List<BlockVariantS> loadCustomBlockVariants()
            {
                string path = this.path + "/Workspace.json";
                if (!File.Exists(path)) return null;

                try
                {
                    using (StreamReader r = new StreamReader(path))
                    {
                        string json = r.ReadToEnd();
                        WorkspaceS workspaceFile = JsonUtility.FromJson<WorkspaceS>(json);
                        return workspaceFile.customBlockVariants;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Issue with the formatting of text file " + path);
                    return null;
                }
            }

            public void saveCustomBlockVariants()
            {
                try
                {
                    // create a workspace save json
                    WorkspaceS wS = new WorkspaceS();
                    string json = JsonUtility.ToJson(wS, prettyPrint);

                    // save the source code
                    using (StreamWriter w = new StreamWriter(path + "/Workspace.json"))
                        w.WriteLine(json);
                }
                catch (Exception e)
                {
                    Debug.Log(e.StackTrace);
                    Debug.Log("Issue converting Workspace to json");
                }
            }
        }
    }

    public static class DirectoryManager
    {
        public static string workspacesPath
        {
            get { return makeDirectory(Application.persistentDataPath + "/Workspaces"); }
        }

        public static string makeDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
            catch
            {
                Debug.Log("Err making directory " + path);
                return null;
            }
        }
    }
}
