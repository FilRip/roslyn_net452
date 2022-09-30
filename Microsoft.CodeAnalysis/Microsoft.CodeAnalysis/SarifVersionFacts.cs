namespace Microsoft.CodeAnalysis
{
    public static class SarifVersionFacts
    {
        public static bool TryParse(string version, out SarifVersion result)
        {
            if (version == null)
            {
                result = SarifVersion.Sarif1;
                return true;
            }
            switch (CaseInsensitiveComparison.ToLower(version))
            {
                case "default":
                    result = SarifVersion.Sarif1;
                    return true;
                case "latest":
                    result = SarifVersion.Latest;
                    return true;
                case "1":
                case "1.0":
                    result = SarifVersion.Sarif1;
                    return true;
                case "2":
                case "2.1":
                    result = SarifVersion.Sarif2;
                    return true;
                default:
                    result = SarifVersion.Sarif1;
                    return false;
            }
        }
    }
}
