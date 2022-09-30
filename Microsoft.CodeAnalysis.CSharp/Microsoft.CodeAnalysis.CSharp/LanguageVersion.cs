namespace Microsoft.CodeAnalysis.CSharp
{
    public enum LanguageVersion
    {
        CSharp1 = 1,
        CSharp2 = 2,
        CSharp3 = 3,
        CSharp4 = 4,
        CSharp5 = 5,
        CSharp6 = 6,
        CSharp7 = 7,
        CSharp7_1 = 701,
        CSharp7_2 = 702,
        CSharp7_3 = 703,
        CSharp8 = 800,
        CSharp9 = 900,
        LatestMajor = 2147483645,
        Preview = 2147483646,
        Latest = int.MaxValue,
        Default = 0
    }
}
