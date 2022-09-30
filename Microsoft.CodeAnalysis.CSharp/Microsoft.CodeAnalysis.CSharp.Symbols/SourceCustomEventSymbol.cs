using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SourceCustomEventSymbol : SourceEventSymbol
    {
        private readonly TypeWithAnnotations _type;

        private readonly string _name;

        private readonly SourceEventAccessorSymbol? _addMethod;

        private readonly SourceEventAccessorSymbol? _removeMethod;

        private readonly TypeSymbol _explicitInterfaceType;

        private readonly ImmutableArray<EventSymbol> _explicitInterfaceImplementations;

        public override TypeWithAnnotations TypeWithAnnotations => _type;

        public override string Name => _name;

        public override MethodSymbol? AddMethod => _addMethod;

        public override MethodSymbol? RemoveMethod => _removeMethod;

        protected override AttributeLocation AllowedAttributeLocations => AttributeLocation.Event;

        private ExplicitInterfaceSpecifierSyntax? ExplicitInterfaceSpecifier => ((EventDeclarationSyntax)base.CSharpSyntaxNode).ExplicitInterfaceSpecifier;

        internal override bool IsExplicitInterfaceImplementation => ExplicitInterfaceSpecifier != null;

        public override ImmutableArray<EventSymbol> ExplicitInterfaceImplementations => _explicitInterfaceImplementations;

        internal SourceCustomEventSymbol(SourceMemberContainerTypeSymbol containingType, Binder binder, EventDeclarationSyntax syntax, BindingDiagnosticBag diagnostics)
            : base(containingType, syntax, syntax.Modifiers, isFieldLike: false, syntax.ExplicitInterfaceSpecifier, syntax.Identifier, diagnostics)
        {
            ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier = syntax.ExplicitInterfaceSpecifier;
            SyntaxToken identifier = syntax.Identifier;
            bool flag = explicitInterfaceSpecifier != null;
            _name = ExplicitInterfaceHelpers.GetMemberNameAndInterfaceSymbol(binder, explicitInterfaceSpecifier, identifier.ValueText, diagnostics, out _explicitInterfaceType, out var aliasQualifierOpt);
            _type = BindEventType(binder, syntax.Type, diagnostics);
            EventSymbol eventSymbol = this.FindExplicitlyImplementedEvent(_explicitInterfaceType, identifier.ValueText, explicitInterfaceSpecifier, diagnostics);
            this.FindExplicitlyImplementedMemberVerification(eventSymbol, diagnostics);
            if (!flag)
            {
                if (IsOverride)
                {
                    EventSymbol overriddenEvent = base.OverriddenEvent;
                    if ((object)overriddenEvent != null)
                    {
                        SourceEventSymbol.CopyEventCustomModifiers(overriddenEvent, ref _type, ContainingAssembly);
                    }
                }
            }
            else if ((object)eventSymbol != null)
            {
                SourceEventSymbol.CopyEventCustomModifiers(eventSymbol, ref _type, ContainingAssembly);
            }
            AccessorDeclarationSyntax accessorDeclarationSyntax = null;
            AccessorDeclarationSyntax accessorDeclarationSyntax2 = null;
            if (syntax.AccessorList != null)
            {
                SyntaxList<AccessorDeclarationSyntax>.Enumerator enumerator = syntax.AccessorList!.Accessors.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AccessorDeclarationSyntax current = enumerator.Current;
                    bool flag2 = false;
                    switch (current.Kind())
                    {
                        case SyntaxKind.AddAccessorDeclaration:
                            if (accessorDeclarationSyntax == null)
                            {
                                accessorDeclarationSyntax = current;
                                flag2 = true;
                            }
                            else
                            {
                                diagnostics.Add(ErrorCode.ERR_DuplicateAccessor, current.Keyword.GetLocation());
                            }
                            break;
                        case SyntaxKind.RemoveAccessorDeclaration:
                            if (accessorDeclarationSyntax2 == null)
                            {
                                accessorDeclarationSyntax2 = current;
                                flag2 = true;
                            }
                            else
                            {
                                diagnostics.Add(ErrorCode.ERR_DuplicateAccessor, current.Keyword.GetLocation());
                            }
                            break;
                        case SyntaxKind.GetAccessorDeclaration:
                        case SyntaxKind.SetAccessorDeclaration:
                        case SyntaxKind.InitAccessorDeclaration:
                            diagnostics.Add(ErrorCode.ERR_AddOrRemoveExpected, current.Keyword.GetLocation());
                            break;
                        default:
                            throw ExceptionUtilities.UnexpectedValue(current.Kind());
                        case SyntaxKind.UnknownAccessorDeclaration:
                            break;
                    }
                    if (flag2 && !IsAbstract && current.Body == null && current.ExpressionBody == null && current.SemicolonToken.Kind() == SyntaxKind.SemicolonToken)
                    {
                        diagnostics.Add(ErrorCode.ERR_AddRemoveMustHaveBody, current.SemicolonToken.GetLocation());
                    }
                }
                if (IsAbstract)
                {
                    if (!syntax.AccessorList!.OpenBraceToken.IsMissing)
                    {
                        diagnostics.Add(ErrorCode.ERR_AbstractEventHasAccessors, syntax.AccessorList!.OpenBraceToken.GetLocation(), this);
                    }
                }
                else if ((accessorDeclarationSyntax == null || accessorDeclarationSyntax2 == null) && (!syntax.AccessorList!.OpenBraceToken.IsMissing || !flag))
                {
                    diagnostics.Add(ErrorCode.ERR_EventNeedsBothAccessors, Locations[0], this);
                }
            }
            else if (flag && !IsAbstract)
            {
                diagnostics.Add(ErrorCode.ERR_ExplicitEventFieldImpl, Locations[0]);
            }
            if (flag && IsAbstract && syntax.AccessorList == null)
            {
                Binder.CheckFeatureAvailability(syntax, MessageID.IDS_DefaultInterfaceImplementation, diagnostics, Locations[0]);
                if (!ContainingAssembly.RuntimeSupportsDefaultInterfaceImplementation)
                {
                    diagnostics.Add(ErrorCode.ERR_RuntimeDoesNotSupportDefaultInterfaceImplementation, Locations[0]);
                }
                _addMethod = new SynthesizedEventAccessorSymbol(this, isAdder: true, eventSymbol, aliasQualifierOpt);
                _removeMethod = new SynthesizedEventAccessorSymbol(this, isAdder: false, eventSymbol, aliasQualifierOpt);
            }
            else
            {
                _addMethod = CreateAccessorSymbol(DeclaringCompilation, accessorDeclarationSyntax, eventSymbol, aliasQualifierOpt, diagnostics);
                _removeMethod = CreateAccessorSymbol(DeclaringCompilation, accessorDeclarationSyntax2, eventSymbol, aliasQualifierOpt, diagnostics);
            }
            _explicitInterfaceImplementations = (((object)eventSymbol == null) ? ImmutableArray<EventSymbol>.Empty : ImmutableArray.Create(eventSymbol));
        }

        internal override void AfterAddingTypeMembersChecks(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
        {
            base.AfterAddingTypeMembersChecks(conversions, diagnostics);
            if ((object)_explicitInterfaceType != null)
            {
                ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier = ExplicitInterfaceSpecifier;
                _explicitInterfaceType.CheckAllConstraints(DeclaringCompilation, conversions, new SourceLocation(explicitInterfaceSpecifier.Name), diagnostics);
            }
            if (!_explicitInterfaceImplementations.IsEmpty)
            {
                EventSymbol interfaceMember = _explicitInterfaceImplementations[0];
                TypeSymbol.CheckNullableReferenceTypeMismatchOnImplementingMember(ContainingType, this, interfaceMember, isExplicit: true, diagnostics);
            }
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("syntaxOpt")]
        private SourceCustomEventAccessorSymbol? CreateAccessorSymbol(CSharpCompilation compilation, AccessorDeclarationSyntax? syntaxOpt, EventSymbol? explicitlyImplementedEventOpt, string? aliasQualifierOpt, BindingDiagnosticBag diagnostics)
        {
            if (syntaxOpt == null)
            {
                return null;
            }
            return new SourceCustomEventAccessorSymbol(this, syntaxOpt, explicitlyImplementedEventOpt, aliasQualifierOpt, compilation.IsNullableAnalysisEnabledIn(syntaxOpt), diagnostics);
        }
    }
}
