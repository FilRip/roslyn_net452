namespace Microsoft.CodeAnalysis.Symbols
{
    public abstract class CommonAnonymousTypeManager
    {
        private ThreeState _templatesSealed = ThreeState.False;

        public bool AreTemplatesSealed => _templatesSealed == ThreeState.True;

        protected void SealTemplates()
        {
            _templatesSealed = ThreeState.True;
        }
    }
}
