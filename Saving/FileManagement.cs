using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UCompile;

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



        private static bool prettyPrint = true;

        public static Workspace activeWorkspace = null;

        public static void loadWorkspace(string name)
        {
            string workspacePath = DirectoryManager.makeDirectory(DirectoryManager.workspacesPath + '/' + name);

            // vv allows people to exit and re-enter workspace without reloading
            if (activeWorkspace == null || activeWorkspace.path != workspacePath)
            {
                WindowManager.destroyFileWindows();
                activeWorkspace = new Workspace(workspacePath);
            }
        }
        


        public static ReferenceTypeS getSourceFile(string name)
        {
            return activeWorkspace._sourceFiles[name];
        }

        public static bool saveSourceFile(string name)
        {
            return activeWorkspace.saveSourceFile(name);
        }

        public static ReferenceTypeS createSourceFile(string name, bool isClass)
        {
            return activeWorkspace.createSourceFile(name, isClass);
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

        public static string loadSourceCode(ReferenceTypeS rTS)
        {
            string path = rTS.path + '/' + rTS.name + ".cs";
            if (!File.Exists(path))
            {
                Debug.Log("Code file " + rTS.name + ".cs does not exist");
                return null;
            }

            try
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string code = r.ReadToEnd();
                    return code;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Issue reading code " + rTS.name + ".cs");
                return null;
            }
        }





        public class Workspace
        {
            public string path;
            public Dictionary<string, ReferenceTypeS> _sourceFiles = null;

            public Workspace(string path)
            {
                this.path = path;
                findSourceFiles();

                if (_sourceFiles.Count == 0) // if this is a new workspace
                {
                    // create the example cube class
                    try
                    {
                        string name = "Cube";

                        // JSON
                        string json = CompilationHelper.singleton.cubeJSON;
                        ReferenceTypeS rTS = JsonUtility.FromJson<ReferenceTypeS>(json);
                        rTS.path = path + '/' + name;
                        _sourceFiles.Add(name, rTS);

                        // TODO: CS automatically?
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Issue with the formatting of cube json or cs");
                    }
                }
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
                                //Debug.Log("Loaded source file " + Path.GetFileName(file));
                            }
                            break;
                        }
                    }
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

            public bool saveAllFiles()
            {
                foreach (string file in _sourceFiles.Keys)
                    if (!saveSourceFile(file)) return false;
                return true;
            }

            public ReferenceTypeS createSourceFile(string name, bool isClass)
            {
                if (_sourceFiles.ContainsKey(name))
                {
                    Debug.Log("File already exists");
                    return null;
                }

                ReferenceTypeS rTS = new ReferenceTypeS(path + '/' + name, name, isClass);
                _sourceFiles.Add(name, rTS);
                return rTS;
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

    public static class CompilationManager
    {
        private static CSScriptEngineRemote _engineRemote;
        private static CSScriptEngineRemote engineRemote
        {
            get
            {
                if (_engineRemote == null)
                {
                    _engineRemote = new CSScriptEngineRemote();
                    _engineRemote.AddOnCompilationFailedHandler(OnCompilationFailedAction);
                    _engineRemote.AddOnCompilationSucceededHandler(OnCompilationSucceededAction);
                }
                return _engineRemote;
            }
        }

        private static int compiled;
        private static int failed;
        private static bool code;

        public static void CompileActiveWorkspace()
        {
            code = false;
            compiled = 0; failed = 0;

            // disposes of current remote AppDomain, if it exists
            UnloadDomain();

            // loads new remote AppDomain
            engineRemote.LoadDomain();
            engineRemote.AddUsings("using UnityEngine;");


            Dictionary<string, ReferenceTypeS>.ValueCollection values = FileManager.activeWorkspace._sourceFiles.Values;
            foreach (ReferenceTypeS rTS in values)
            {
                string code = FileManager.loadSourceCode(rTS);
                if (code == null) continue;

                engineRemote.CompileType(rTS.name, code);
            }


            string summary = "Compiled: " + compiled + " \nFailed: " + failed;
            WindowManager.getWindowWithComponent<ToolsWindow>().setTitleTextMessage(summary, true);
        }

        public static void ExecuteClasslessCode(string classlessCode)
        {
            code = true;

            Window w = WindowManager.getWindowWithComponent<CodeWindow>();

            try
            {
                engineRemote.CompileCode(classlessCode);
                w.setTitleTextMessage("", false);
            }
            catch
            {
                w.setTitleTextMessage("ERR", true);
            }
        }

        public static void UnloadDomain()
        {
            engineRemote.UnloadDomain();
        }

        private static void OnCompilationFailedAction(CompilerOutput output)
        {
            for (int i = 0; i < output.Errors.Count; i++)
                Debug.Log(output.Errors[i]);
            for (int i = 0; i < output.Warnings.Count; i++)
                Debug.Log(output.Warnings[i]);

            if (!code) failed++;
        }

        private static void OnCompilationSucceededAction(CompilerOutput output)
        {
            for (int i = 0; i < output.Warnings.Count; i++)
                Debug.Log(output.Warnings[i]);

            if (code) engineRemote.ExecuteLastCompiledCode();
            else compiled++;
        }
    }
}
