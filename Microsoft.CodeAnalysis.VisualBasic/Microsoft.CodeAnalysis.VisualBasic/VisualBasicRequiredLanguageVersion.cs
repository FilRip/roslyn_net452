namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class VisualBasicRequiredLanguageVersion : RequiredLanguageVersion
	{
		internal LanguageVersion Version { get; }

		internal VisualBasicRequiredLanguageVersion(LanguageVersion version)
		{
			Version = version;
		}

		public override string ToString()
		{
			return LanguageVersionFacts.ToDisplayString(Version);
		}
	}
}
