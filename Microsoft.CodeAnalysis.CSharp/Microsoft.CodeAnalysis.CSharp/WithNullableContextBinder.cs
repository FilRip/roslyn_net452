namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class WithNullableContextBinder : Binder
    {
        private readonly SyntaxTree _syntaxTree;

        private readonly int _position;

        internal WithNullableContextBinder(SyntaxTree syntaxTree, int position, Binder next)
            : base(next)
        {
            _syntaxTree = syntaxTree;
            _position = position;
        }

        internal override bool AreNullableAnnotationsGloballyEnabled()
        {
            return base.Next!.AreNullableAnnotationsEnabled(_syntaxTree, _position);
        }
    }
}
