using System;
using UnityEngine;

namespace ManifestChecker
{
    [Serializable]
    public class SwitchPackage : IPackageTarget
    {
        [Serializable]
        public class PackageReplacement
        {
            [SerializeField] public string name = default;
            [SerializeField] public string version = default;

            public bool IsValid()
            {
                if (string.IsNullOrWhiteSpace(name))
                    return false;
                if (string.IsNullOrWhiteSpace(version))
                    return false;
                return true;
            }
        }

        [SerializeField] public string original = default;
        [SerializeField] public PackageReplacement replacement = default;

        public string Target { get => original; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(original))
                return false;
            return replacement.IsValid();
        }

        public bool TryReplaceManifestText(string text, out string newText)
        {
            if (string.IsNullOrEmpty(text))
                throw new MissingFieldException(nameof(LocalPackage), nameof(text));
            newText = null;
            if (text.Contains(Target))
            {
                if (text.Contains("\"" + replacement.name + "\""))
                    return false;
                try
                {
                    Debug.Log("Switching package " + Target);

                    var target = GetPackageName(text, original);
                    var replace = "\"" + replacement.name;
                    text = text.Replace(target, replace);
                    if (string.IsNullOrWhiteSpace(replacement.version))
                        return true;

                    target = GetPackageVersion(text, replacement.name);
                    replace = "\"" + replacement.version;
                    text = text.Replace(target, replace);
                    return true;
                }
                catch (Exception e) { Debug.LogError($"Failed to switch package {Target}\nError: {e}"); }
            }
            return false;
        }

        private static string GetPackageName(string text, string package)
        {
            var index = text.IndexOf("\"" + package + "\"");
            var length = text.IndexOf('\"', index + 1) - index;
            return text.Substring(index, length);
        }

        private static string GetPackageVersion(string text, string package)
        {
            var index = text.IndexOf("\"" + package + "\"");
            var length = text.IndexOf('\"', index + 1) - index;
            index = text.IndexOf('\"', index + length + 2);
            length = text.IndexOf('\"', index + 1) - index;
            return text.Substring(index, length);
        }
    }
}