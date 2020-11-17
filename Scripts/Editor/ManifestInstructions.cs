using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ManifestChecker
{
    [CreateAssetMenu(fileName = "Manifest Instructions", menuName = "Manifest/Manifest Instructions", order = 120)]
    public class ManifestInstructions : ScriptableObject
    {
        [SerializeField] private string identifier = Environment.UserName;
        [SerializeField] private bool global = false;
        [SerializeField] private AddPackage[] addPackages = null;
        [SerializeField] private RemovePackage[] removePackages = null;
        [SerializeField] private SwitchPackage[] switchPackages = null;
        [SerializeField] private LocalPackage[] localPackages = null;

        public string[] GetTargetedPackages()
        {
            var combine = new List<string>();
            // removePackages
            var names = removePackages.GetTargetedPackages();
            if (names != null)
                combine.AddRange(names);
            // switchPackages
            names = switchPackages.GetTargetedPackages();
            if (names != null)
                combine.AddRange(names);
            // localPackages
            names = localPackages.GetTargetedPackages();
            if (names != null)
                combine.AddRange(names);
            // send
            if (combine.Count > 0)
                return combine.Distinct().ToArray();
            return null;
        }

        public bool IsThisEditor()
        {
            if (global)
                return Environment.MachineName.Equals(identifier, StringComparison.OrdinalIgnoreCase);
            return Environment.UserName.Equals(identifier, StringComparison.OrdinalIgnoreCase);
        }

        public bool TryReplacePackages(string text, out string newText)
        {
            var foundSomething = false;
            // addPackages
            if (addPackages.TryReplaceManifestText(text, out newText))
            {
                text = newText;
                foundSomething = true;
            }
            // removePackages
            if (removePackages.TryReplaceManifestText(text, out newText))
            {
                text = newText;
                foundSomething = true;
            }
            // switchPackages
            if (switchPackages.TryReplaceManifestText(text, out newText))
            {
                text = newText;
                foundSomething = true;
            }
            // localPackages
            if (localPackages.TryReplaceManifestText(text, out newText))
            {
                text = newText;
                foundSomething = true;
            }
            if (foundSomething)
                newText = text;
            else newText = null;
            return foundSomething;
        }

        public static List<ManifestInstructions> GatherAll()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + nameof(ManifestInstructions));
            var length = guids == null ? 0 : guids.Length;
            if (length == 0)
                return null;

            var instructions = new List<ManifestInstructions>();
            for (int i = 0; i < length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var instruction = AssetDatabase.LoadAssetAtPath<ManifestInstructions>(path);
                if (instruction != null)
                    instructions.Add(instruction);
            }
            return instructions;
        }
    }
}