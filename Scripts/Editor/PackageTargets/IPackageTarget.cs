namespace ManifestChecker
{
    public interface IPackageTarget
    {
        string Target { get; }

        bool IsValid();

        bool TryReplaceManifestText(string text, out string newText);
    }
}