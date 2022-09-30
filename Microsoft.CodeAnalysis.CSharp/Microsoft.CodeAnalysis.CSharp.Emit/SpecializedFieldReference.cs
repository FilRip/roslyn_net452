using System.Collections.Immutable;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.CodeAnalysis.CSharp.Emit
{
    internal sealed class SpecializedFieldReference : TypeMemberReference, ISpecializedFieldReference, IFieldReference, ITypeMemberReference, IReference, INamedEntity
    {
        private readonly FieldSymbol _underlyingField;

        protected override Symbol UnderlyingSymbol => _underlyingField;

        IFieldReference ISpecializedFieldReference.UnspecializedVersion => _underlyingField.OriginalDefinition.GetCciAdapter();

        ISpecializedFieldReference IFieldReference.AsSpecializedFieldReference => this;

        bool IFieldReference.IsContextualNamedEntity => false;

        public SpecializedFieldReference(FieldSymbol underlyingField)
        {
            _underlyingField = underlyingField;
        }

        public override void Dispatch(MetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        ITypeReference IFieldReference.GetType(EmitContext context)
        {
            TypeWithAnnotations typeWithAnnotations = _underlyingField.TypeWithAnnotations;
            ImmutableArray<CustomModifier> customModifiers = typeWithAnnotations.CustomModifiers;
            ITypeReference typeReference = ((PEModuleBuilder)context.Module).Translate(typeWithAnnotations.Type, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
            if (customModifiers.Length == 0)
            {
                return typeReference;
            }
            return new ModifiedTypeReference(typeReference, ImmutableArray<ICustomModifier>.CastUp(customModifiers));
        }

        IFieldDefinition IFieldReference.GetResolvedField(EmitContext context)
        {
            return null;
        }
    }
}
