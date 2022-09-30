using System;

#nullable enable

namespace Microsoft.CodeAnalysis.Emit
{
    public readonly struct EmitContext
    {
        [Flags()]
        private enum Flags
        {
            None = 0,
            MetadataOnly = 1,
            IncludePrivateMembers = 2
        }

        public readonly CommonPEModuleBuilder Module;

        public readonly SyntaxNode? SyntaxNode;

        public readonly RebuildData? RebuildData;

        public readonly DiagnosticBag Diagnostics;

        private readonly Flags _flags;

        public bool IncludePrivateMembers => (_flags & Flags.IncludePrivateMembers) != 0;

        public bool MetadataOnly => (_flags & Flags.MetadataOnly) != 0;

        public bool IsRefAssembly
        {
            get
            {
                if (MetadataOnly)
                {
                    return !IncludePrivateMembers;
                }
                return false;
            }
        }

        public EmitContext(CommonPEModuleBuilder module, SyntaxNode? syntaxNode, DiagnosticBag diagnostics, bool metadataOnly, bool includePrivateMembers)
            : this(module, diagnostics, metadataOnly, includePrivateMembers, syntaxNode)
        {
        }

        public EmitContext(CommonPEModuleBuilder module, DiagnosticBag diagnostics, bool metadataOnly, bool includePrivateMembers, SyntaxNode? syntaxNode = null, RebuildData? rebuildData = null)
        {
            RebuildData = rebuildData;
            Module = module;
            SyntaxNode = syntaxNode;
            RebuildData = rebuildData;
            Diagnostics = diagnostics;
            Flags flags = Flags.None;
            if (metadataOnly)
            {
                flags |= Flags.MetadataOnly;
            }
            if (includePrivateMembers)
            {
                flags |= Flags.IncludePrivateMembers;
            }
            _flags = flags;
        }
    }
}
