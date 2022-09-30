using System.Collections.Generic;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Emit
{
    internal abstract class TypeMemberReference : ITypeMemberReference, IReference, INamedEntity
    {
        protected abstract Symbol UnderlyingSymbol { get; }

        string INamedEntity.Name => UnderlyingSymbol.MetadataName;

        public virtual ITypeReference GetContainingType(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate(UnderlyingSymbol.ContainingType, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
        }

        public override string ToString()
        {
            return UnderlyingSymbol.ToDisplayString(SymbolDisplayFormat.ILVisualizationFormat);
        }

        IEnumerable<ICustomAttribute> IReference.GetAttributes(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<ICustomAttribute>();
        }

        public abstract void Dispatch(MetadataVisitor visitor);

        IDefinition IReference.AsDefinition(EmitContext context)
        {
            return null;
        }

        ISymbolInternal IReference.GetInternalSymbol()
        {
            return UnderlyingSymbol;
        }

        public sealed override bool Equals(object obj)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public sealed override int GetHashCode()
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
