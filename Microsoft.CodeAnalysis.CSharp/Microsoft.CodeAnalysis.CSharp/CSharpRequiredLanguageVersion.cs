namespace Microsoft.CodeAnalysis.CSharp
{
    public class CSharpRequiredLanguageVersion : RequiredLanguageVersion
    {
        internal LanguageVersion Version { get; }

        internal CSharpRequiredLanguageVersion(LanguageVersion version)
        {
            Version = version;
        }

        public override string ToString()
        {
            return Version.ToDisplayString();
        }
    }
}
