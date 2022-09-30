using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedBackingFieldSymbol : FieldSymbolWithAttributesAndModifiers
    {
        private readonly SourcePropertySymbolBase _property;

        private readonly string _name;

        internal bool HasInitializer { get; }

        protected override DeclarationModifiers Modifiers { get; }

        protected override IAttributeTargetSymbol AttributeOwner => _property.AttributesOwner;

        internal override Location ErrorLocation => _property.Location;

        protected override SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList => _property.AttributeDeclarationSyntaxList;

        public override Symbol AssociatedSymbol => _property;

        public override ImmutableArray<Location> Locations => _property.Locations;

        internal override bool HasPointerType => _property.HasPointerType;

        public override string Name => _name;

        public override Symbol ContainingSymbol => _property.ContainingSymbol;

        public override NamedTypeSymbol ContainingType => _property.ContainingType;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        internal override bool HasRuntimeSpecialName => false;

        public override bool IsImplicitlyDeclared => true;

        public SynthesizedBackingFieldSymbol(SourcePropertySymbolBase property, string name, bool isReadOnly, bool isStatic, bool hasInitializer)
        {
            _name = name;
            Modifiers = DeclarationModifiers.Private | (isReadOnly ? DeclarationModifiers.ReadOnly : DeclarationModifiers.None) | (isStatic ? DeclarationModifiers.Static : DeclarationModifiers.None);
            _property = property;
            HasInitializer = hasInitializer;
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            return _property.TypeWithAnnotations;
        }

        internal sealed override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            if (arguments.Attribute.IsTargetAttribute(this, AttributeDescription.FixedBufferAttribute))
            {
                ((BindingDiagnosticBag)arguments.Diagnostics).Add(ErrorCode.ERR_DoNotUseFixedBufferAttrOnProperty, arguments.AttributeSyntaxOpt!.Name.Location);
            }
            else
            {
                base.DecodeWellKnownAttribute(ref arguments);
            }
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            if (!ContainingType.IsImplicitlyDeclared)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
            }
            Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDebuggerBrowsableNeverAttribute());
        }

        internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress, bool earlyDecodingWellKnownAttributes)
        {
            return null;
        }

        internal override void PostDecodeWellKnownAttributes(ImmutableArray<CSharpAttributeData> boundAttributes, ImmutableArray<AttributeSyntax> allAttributeSyntaxNodes, BindingDiagnosticBag diagnostics, AttributeLocation symbolPart, WellKnownAttributeData decodedData)
        {
            base.PostDecodeWellKnownAttributes(boundAttributes, allAttributeSyntaxNodes, diagnostics, symbolPart, decodedData);
            if (!allAttributeSyntaxNodes.IsEmpty && _property.IsAutoPropertyWithGetAccessor)
            {
                CheckForFieldTargetedAttribute(diagnostics);
            }
        }

        private void CheckForFieldTargetedAttribute(BindingDiagnosticBag diagnostics)
        {
            LanguageVersion languageVersion = DeclaringCompilation.LanguageVersion;
            if (languageVersion.AllowAttributesOnBackingFields())
            {
                return;
            }
            SyntaxList<AttributeListSyntax>.Enumerator enumerator = AttributeDeclarationSyntaxList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AttributeListSyntax current = enumerator.Current;
                AttributeTargetSpecifierSyntax? target = current.Target;
                if (target != null && target!.GetAttributeLocation() == AttributeLocation.Field)
                {
                    diagnostics.Add(new CSDiagnosticInfo(ErrorCode.WRN_AttributesOnBackingFieldsNotAvailable, languageVersion.ToDisplayString(), new CSharpRequiredLanguageVersion(MessageID.IDS_FeatureAttributesOnBackingFields.RequiredVersion())), current.Target!.Location);
                }
            }
        }
    }
}
