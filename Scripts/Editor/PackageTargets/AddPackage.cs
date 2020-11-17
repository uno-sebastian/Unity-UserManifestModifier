using System;
using UnityEngine;

namespace ManifestChecker
{
    [Serializable]
    public class AddPackage : IPackageTarget
    {
        [SerializeField] public string name = default;
        [SerializeField] public string version = default;

        public string Target { get => null; }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            if (string.IsNullOrWhiteSpace(version))
                return false;
            return true;
        }

        public bool TryReplaceManifestText(string text, out string newText)
        {
            if (string.IsNullOrEmpty(text))
                throw new MissingFieldException(nameof(LocalPackage), nameof(text));
            newText = null;
            if (text.Contains("\"" + name + "\""))
                return false;
            try
            {
                Debug.Log("Adding package " + Target);

                var dependenciesIndex = GetDependenciesIndex(text);
                text = text.Insert(dependenciesIndex, $"\n\t\"{name}\": \"{version}\",");
                return true;
            }
            catch (Exception e) { Debug.LogError($"Failed trying add package {Target}\nError: {e}"); }
            return false;
        }

        private static int GetDependenciesIndex(string text)
        {
            var dependenciesIndex = text.IndexOf("\"dependencies\"");
            return text.IndexOf('{', dependenciesIndex) + 1;
        }
    }
}