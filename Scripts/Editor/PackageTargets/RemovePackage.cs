using System;
using UnityEngine;

namespace ManifestChecker
{
    [Serializable]
    public class RemovePackage : IPackageTarget
    {
        [SerializeField] public string name = default;

        public string Target { get => name; }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(name))
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
                if (text.Contains("\"" + name + "\"") == false)
                    return false;
                try
                {
                    Debug.Log("Removing package " + Target);

                    var remove = GetPackageSubstring(text, name);
                    text = text.Replace(remove, "");
                    return true;
                }
                catch (Exception e) { Debug.LogError($"Failed to remove package {Target}\nError: {e}"); }
            }
            return false;
        }

        private static string GetPackageSubstring(string text, string package)
        {
            var index = text.IndexOf("\"" + package + "\"");
            index = text.LastIndexOf('\n', index);
            var length = text.IndexOf('\n', text.IndexOf(',', index)) - index;
            return text.Substring(index, length);
        }
    }
}