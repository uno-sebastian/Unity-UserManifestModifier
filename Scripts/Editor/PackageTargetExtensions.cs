using System;
using System.Collections.Generic;
using UnityEngine;

namespace ManifestChecker
{
    public static class PackageTargetExtensions
    {
        public static string[] GetTargetedPackages(this IPackageTarget[] packages)
        {
            if (packages == null) return null;
            var names = new List<string>();
            foreach (var package in packages)
                if (string.IsNullOrWhiteSpace(package.Target))
                    continue;
                else names.Add(package.Target);
            return names.ToArray();
        }

        public static bool TryReplaceManifestText(this IPackageTarget[] packages, string text, out string newText)
        {
            newText = string.Empty;
            if (string.IsNullOrEmpty(text))
                throw new MissingFieldException(nameof(LocalPackage), nameof(text));
            var foundPackage = false;
            var length = packages == null ? 0 : packages.Length;
            for (int i = 0; i < length; i++)
                if (packages[i] != null && packages[i].IsValid())
                    if (packages[i].TryReplaceManifestText(text, out string packageText))
                    {
                        text = packageText;
                        foundPackage = true;
                    }
            if (foundPackage)
                newText = text;
            return foundPackage;
        }
    }
}