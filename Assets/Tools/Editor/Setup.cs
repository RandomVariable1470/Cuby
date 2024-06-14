using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using static UnityEditor.AssetDatabase;

public static class Setup
{
    [MenuItem("Tools/Setup/Create Default Folders")]
    public static void CreateDefaultFolders()
    {
        Folders.CreateDefault("_Project", "Resources/Art", "Resources/ScriptableObjects", "Resources/Plugin",
            "Resources/Materials", "Resources/Shaders", "Resources/Animations", "_Scripts", "Scenes", "Audio","Prefabs","Sprites");
        Refresh();
    }

    [MenuItem("Tools/Setup/Install Open Source")]
    public static void InstallOpenSource() 
    {
        Packages.InstallPackages(new[] 
        {
            "git+https://github.com/kirevdokimov/Unity-UI-Rounded-Corners.git",
            "git+https://github.com/starikcetin/Eflatun.SceneReference.git#3.1.1",
            "git+https://github.com/adammyhre/Unity-Improved-Timers.git"
        });
    }
    
    [MenuItem("Tools/Setup/Install GOAP Package")]
    public static void InstallAIPackage()
    {
        Packages.InstallPackages(new[]
        {
            "git+https://github.com/crashkonijn/GOAP.git?path=/Package#2.1.21"
        });
    }

    [MenuItem("Tools/Setup/Install Unity Packages")]
    public static void InstallUnityPackages()
    {
        Packages.InstallPackages(new []
        {
            "com.unity.ai.navigation",
            "com.unity.cinemachine",
            "com.unity.device-simulator.devices",
            "com.unity.inputsystem",
            "com.unity.probuilder",
            "com.unity.textmeshpro",
            "com.unity.shadergraph",
            "com.unity.splines",
            "com.unity.visualeffectgraph"
        });
    }

    static class Folders
    {
        public static void CreateDefault(string root, params string[] folders) 
        {
            var fullpath = Path.Combine(Application.dataPath, root);
            if (!Directory.Exists(fullpath))
            {
                Directory.CreateDirectory(fullpath);
            }
            foreach (var folder in folders)
            {
                CreateSubFolders(fullpath, folder);
            }
        }

        private static void CreateSubFolders(string rootPath, string folderHierarchy) 
        {
            var folders = folderHierarchy.Split('/');
            var currentPath = rootPath;
            foreach (var folder in folders) 
            {
                currentPath = Path.Combine(currentPath, folder);
                if (!Directory.Exists(currentPath)) 
                {
                    Directory.CreateDirectory(currentPath);
                }
            }
        }
    }

    static class Packages 
    {
        static AddRequest Request;
        static Queue<string> PackagesToInstall = new();

        public static void InstallPackages(string[] packages) 
        {
            foreach (var package in packages)
            {
                PackagesToInstall.Enqueue(package);
            }
            
            if (PackagesToInstall.Count > 0) 
            {
                Request = Client.Add(PackagesToInstall.Dequeue());
                EditorApplication.update += Progress;
            }
        }

        static void Progress()
        {
            if (Request.IsCompleted) 
            {
                if (Request.Status == StatusCode.Success)
                {
                    Debug.Log("Installed: " + Request.Result.packageId);
                }
                else if (Request.Status >= StatusCode.Failure)
                {
                    Debug.LogAssertion(Request.Error.message);
                }

                EditorApplication.update -= Progress;
                
                if (PackagesToInstall.Count > 0)
                {
                    Request = Client.Add(PackagesToInstall.Dequeue());
                    EditorApplication.update += Progress;
                }
            }
        }
    }
}