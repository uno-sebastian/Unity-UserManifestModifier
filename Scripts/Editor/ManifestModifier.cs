using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ManifestChecker
{
    public static class ManifestModifier
    {
        [MenuItem("Assets/Manifest/Display Info", false, 121)]
        private static void DisplayInfo()
        {
            var machineName = Environment.MachineName;
            var userName = Environment.UserName;
            EditorUtility.DisplayDialog("Display Environment Info", $"Your machine's name is '{machineName}' and this user enviorment is '{userName}'.", "Thank you (in Binary)");
        }

        [MenuItem("Assets/Manifest/Manifest Refresh", false, 120)]
        private static void ManifestRefresh()
        {
            var instructions = ManifestInstructions.GatherAll();
            if (instructions == null) return;
            if (instructions.Count < 1) return;
            foreach (var instruction in instructions)
                if (instruction.IsThisEditor())
                {
                    ValidateManifest(instruction);
                    break;
                }
        }
        public static void ValidateManifest(ManifestInstructions instructions)
        {
            if (instructions == false) return;
            var path = GetManifestPath();
            var text = File.ReadAllText(path);
            if (instructions.TryReplacePackages(text, out string newText))
            {
                Debug.Log("Manifest Packages Modified by " + nameof(ManifestModifier));
                File.WriteAllText(path, newText);
                AssetDatabase.Refresh();
            }
        }

        private static string GetManifestPath()
        {
            var manifestPath = Application.dataPath;
            manifestPath = manifestPath.Remove(manifestPath.Length - 6) + "Packages/manifest.json";
            if (string.IsNullOrWhiteSpace(manifestPath))
                throw new Exception("Could not get manifest path!");
            if (File.Exists(manifestPath) == false)
                throw new Exception("Could not find manifest path file!");
            return manifestPath;
        }
    }
}