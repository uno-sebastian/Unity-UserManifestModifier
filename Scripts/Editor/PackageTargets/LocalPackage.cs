using System;
using UnityEngine;

namespace ManifestChecker
{
    [Serializable]
    public class LocalPackage : IPackageTarget
    {
        [SerializeField] public string name = default;
        [SerializeField] public string path = default;

        public string Target { get => name; }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            if (string.IsNullOrWhiteSpace(path))
                return false;
            return true;
        }

        public bool TryReplaceManifestText(string text, out string newText)
        {
            if (string.IsNullOrEmpty(text))
                throw new MissingFieldException(nameof(LocalPackage), nameof(text));
            newText = null;
            if (text.Contains(Target))
            {
                var packagePath = FormatPackagePath(path);
                if (text.Contains(packagePath))
                    return false;
                try
                {
                    Debug.Log("Changed path of local package " + Target);

                    var target = GetLocalPackagePath(text, name);
                    text = text.Replace(target, packagePath);
                    return true;
                }
                catch (Exception e) { Debug.LogError($"Failed to change path of local package {Target}\nError: {e}"); }
            }
            return false;
        }

        private static string GetLocalPackagePath(string text, string package)
        {
            var lineIndex = text.IndexOf("\"" + package + "\"");
            var startingIndex = text.IndexOf("\"file:", lineIndex + 1);
            var replaceLength = text.IndexOf('\"', startingIndex + 1) - startingIndex;
            return text.Substring(startingIndex, replaceLength);
        }

        private static string FormatPackagePath(string path)
        {
            if (path.StartsWith("file:") == false)
                path = "file:" + path;
            return "\"" + path;
        }
    }
}