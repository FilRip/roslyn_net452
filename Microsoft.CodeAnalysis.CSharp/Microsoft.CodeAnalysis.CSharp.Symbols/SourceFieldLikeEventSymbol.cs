using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SourceFieldLikeEventSymbol : SourceEventSymbol
    {
        private readonly string _name;

        private readonly TypeWithAnnotations _type;

        private readonly SynthesizedEventAccessorSymbol _addMethod;

        private readonly SynthesizedEventAccessorSymbol _removeMethod;

        internal override FieldSymbol? AssociatedField => AssociatedEventField;

        internal SourceEventFieldSymbol? AssociatedEventField { get; }

        public override string Name => _name;

        public override TypeWithAnnotations TypeWithAnnotations => _type;

        public override MethodSymbol AddMethod => _addMethod;

        public override MethodSymbol RemoveMethod => _removeMethod;

        internal override bool IsExplicitInterfaceImplementation => false;

        protected override AttributeLocation AllowedAttributeLocations
        {
            get
            {
                if ((object)AssociatedEventField == null)
                {
                    return AttributeLocation.Method | AttributeLocation.Event;
                }
                return AttributeLocation.Method | AttributeLocation.Field | AttributeLocation.Event;
            }
        }

        public override ImmutableArray<EventSymbol> ExplicitInterfaceImplementations => ImmutableArray<EventSymbol>.Empty;

        internal SourceFieldLikeEventSymbol(SourceMemberContainerTypeSymbol containingType, Binder binder, SyntaxTokenList modifiers, VariableDeclaratorSyntax declaratorSyntax, BindingDiagnosticBag diagnostics)
            : base(containingType, declaratorSyntax, modifiers, isFieldLike: true, null, declaratorSyntax.Identifier, diagnostics)
        {
            _name = declaratorSyntax.Identifier.ValueText;
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
            VariableDeclarationSyntax variableDeclarationSyntax = (VariableDeclarationSyntax)declaratorSyntax.Parent;
            _type = BindEventType(binder, variableDeclarationSyntax.Type, instance);
            if (IsOverride)
            {
                EventSymbol overriddenEvent = base.OverriddenEvent;
                if ((object)overriddenEvent != null)
                {
                    SourceEventSymbol.CopyEventCustomModifiers(overriddenEvent, ref _type, ContainingAssembly);
                }
            }
            bool flag = declaratorSyntax.Initializer != null;
            bool flag2 = containingType.IsInterfaceType();
            if (flag)
            {
                if (flag2 && !IsStatic)
                {
                    diagnostics.Add(ErrorCode.ERR_InterfaceEventInitializer, Locations[0], this);
                }
                else if (IsAbstract)
                {
                    diagnostics.Add(ErrorCode.ERR_AbstractEventInitializer, Locations[0], this);
                }
                else if (IsExtern)
                {
                    diagnostics.Add(ErrorCode.ERR_ExternEventInitializer, Locations[0], this);
                }
            }
            if (flag || (!IsExtern && !IsAbstract))
            {
                AssociatedEventField = MakeAssociatedField(declaratorSyntax);
            }
            if (!IsStatic && ContainingType.IsReadOnly)
            {
                diagnostics.Add(ErrorCode.ERR_FieldlikeEventsInRoStruct, Locations[0]);
            }
            if (flag2)
            {
                if (IsExtern || IsStatic)
                {
                    if (!ContainingAssembly.RuntimeSupportsDefaultInterfaceImplementation)
                    {
                        diagnostics.Add(ErrorCode.ERR_RuntimeDoesNotSupportDefaultInterfaceImplementation, Locations[0]);
                    }
                }
                else if (!IsAbstract)
                {
                    diagnostics.Add(ErrorCode.ERR_EventNeedsBothAccessors, Locations[0], this);
                }
            }
            _addMethod = new SynthesizedEventAccessorSymbol(this, isAdder: true);
            _removeMethod = new SynthesizedEventAccessorSymbol(this, isAdder: false);
            if (variableDeclarationSyntax.Variables[0] == declaratorSyntax)
            {
                diagnostics.AddRange(instance);
            }
            instance.Free();
        }

        private SourceEventFieldSymbol MakeAssociatedField(VariableDeclaratorSyntax declaratorSyntax)
        {
            return new SourceEventFieldSymbol(this, declaratorSyntax, BindingDiagnosticBag.Discarded);
        }

        internal override void ForceComplete(SourceLocation? locationOpt, CancellationToken cancellationToken)
        {
            if ((object)AssociatedField != null)
            {
                AssociatedField!.ForceComplete(locationOpt, cancellationToken);
            }
            base.ForceComplete(locationOpt, cancellationToken);
        }
    }
}
