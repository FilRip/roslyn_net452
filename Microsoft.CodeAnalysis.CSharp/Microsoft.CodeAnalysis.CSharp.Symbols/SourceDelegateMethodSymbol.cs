using System;
using System.Collections.Immutable;
using System.Reflection;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SourceDelegateMethodSymbol : SourceMemberMethodSymbol
    {
        private sealed class Constructor : SourceDelegateMethodSymbol
        {
            public override string Name => ".ctor";

            public override RefKind RefKind => RefKind.None;

            internal Constructor(SourceMemberContainerTypeSymbol delegateType, TypeWithAnnotations voidType, TypeWithAnnotations objectType, TypeWithAnnotations intPtrType, DelegateDeclarationSyntax syntax)
                : base(delegateType, voidType, syntax, MethodKind.Constructor, DeclarationModifiers.Public)
            {
                InitializeParameters(ImmutableArray.Create(SynthesizedParameterSymbol.Create(this, objectType, 0, RefKind.None, "object"), SynthesizedParameterSymbol.Create(this, intPtrType, 1, RefKind.None, "method")));
            }

            internal override OneOrMany<SyntaxList<AttributeListSyntax>> GetReturnTypeAttributeDeclarations()
            {
                return OneOrMany.Create(default(SyntaxList<AttributeListSyntax>));
            }

            internal override LexicalSortKey GetLexicalSortKey()
            {
                return new LexicalSortKey(syntaxReferenceOpt.GetLocation(), DeclaringCompilation);
            }
        }

        private sealed class InvokeMethod : SourceDelegateMethodSymbol
        {
            private readonly RefKind _refKind;

            private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

            public override string Name => "Invoke";

            public override RefKind RefKind => _refKind;

            public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

            internal InvokeMethod(SourceMemberContainerTypeSymbol delegateType, RefKind refKind, TypeWithAnnotations returnType, DelegateDeclarationSyntax syntax, Binder binder, BindingDiagnosticBag diagnostics)
                : base(delegateType, returnType, syntax, MethodKind.DelegateInvoke, DeclarationModifiers.Public | DeclarationModifiers.Virtual)
            {
                _refKind = refKind;
                ImmutableArray<ParameterSymbol> parameters = ParameterHelpers.MakeParameters(binder, this, syntax.ParameterList, out SyntaxToken arglistToken, diagnostics, allowRefOrOut: true, allowThis: false, addRefReadOnlyModifier: true);
                if (arglistToken.Kind() == SyntaxKind.ArgListKeyword)
                {
                    diagnostics.Add(ErrorCode.ERR_IllegalVarArgs, new SourceLocation(in arglistToken));
                }
                if (_refKind == RefKind.In)
                {
                    NamedTypeSymbol wellKnownType = binder.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_InAttribute, diagnostics, syntax.ReturnType);
                    _refCustomModifiers = ImmutableArray.Create(CSharpCustomModifier.CreateRequired(wellKnownType));
                }
                else
                {
                    _refCustomModifiers = ImmutableArray<CustomModifier>.Empty;
                }
                InitializeParameters(parameters);
            }

            internal override LexicalSortKey GetLexicalSortKey()
            {
                return new LexicalSortKey(syntaxReferenceOpt.GetLocation(), DeclaringCompilation);
            }

            internal override void AfterAddingTypeMembersChecks(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
            {
                Location location = ((DelegateDeclarationSyntax)base.SyntaxRef.GetSyntax()).ReturnType.GetLocation();
                CSharpCompilation declaringCompilation = DeclaringCompilation;
                base.AfterAddingTypeMembersChecks(conversions, diagnostics);
                if (_refKind == RefKind.In)
                {
                    declaringCompilation.EnsureIsReadOnlyAttributeExists(diagnostics, location, modifyCompilation: true);
                }
                ParameterHelpers.EnsureIsReadOnlyAttributeExists(declaringCompilation, Parameters, diagnostics, modifyCompilation: true);
                if (base.ReturnType.ContainsNativeInteger())
                {
                    declaringCompilation.EnsureNativeIntegerAttributeExists(diagnostics, location, modifyCompilation: true);
                }
                ParameterHelpers.EnsureNativeIntegerAttributeExists(declaringCompilation, Parameters, diagnostics, modifyCompilation: true);
                if (declaringCompilation.ShouldEmitNullableAttributes(this) && ReturnTypeWithAnnotations.NeedsNullableAttribute())
                {
                    declaringCompilation.EnsureNullableAttributeExists(diagnostics, location, modifyCompilation: true);
                }
                ParameterHelpers.EnsureNullableAttributeExists(declaringCompilation, this, Parameters, diagnostics, modifyCompilation: true);
            }
        }

        private sealed class BeginInvokeMethod : SourceDelegateMethodSymbol
        {
            public override string Name => "BeginInvoke";

            public override RefKind RefKind => RefKind.None;

            internal BeginInvokeMethod(InvokeMethod invoke, TypeWithAnnotations iAsyncResultType, TypeWithAnnotations objectType, TypeWithAnnotations asyncCallbackType, DelegateDeclarationSyntax syntax)
                : base((SourceNamedTypeSymbol)invoke.ContainingType, iAsyncResultType, syntax, MethodKind.Ordinary, DeclarationModifiers.Public | DeclarationModifiers.Virtual)
            {
                ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance();
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = invoke.Parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SourceParameterSymbol sourceParameterSymbol = (SourceParameterSymbol)enumerator.Current;
                    SourceClonedParameterSymbol item = new SourceClonedParameterSymbol(sourceParameterSymbol, this, sourceParameterSymbol.Ordinal, suppressOptional: true);
                    instance.Add(item);
                }
                int parameterCount = invoke.ParameterCount;
                instance.Add(SynthesizedParameterSymbol.Create(this, asyncCallbackType, parameterCount, RefKind.None, GetUniqueParameterName(instance, "callback")));
                instance.Add(SynthesizedParameterSymbol.Create(this, objectType, parameterCount + 1, RefKind.None, GetUniqueParameterName(instance, "object")));
                InitializeParameters(instance.ToImmutableAndFree());
            }

            internal override OneOrMany<SyntaxList<AttributeListSyntax>> GetReturnTypeAttributeDeclarations()
            {
                return OneOrMany.Create(default(SyntaxList<AttributeListSyntax>));
            }
        }

        private sealed class EndInvokeMethod : SourceDelegateMethodSymbol
        {
            private readonly InvokeMethod _invoke;

            protected override SourceMemberMethodSymbol BoundAttributesSource => _invoke;

            public override string Name => "EndInvoke";

            public override RefKind RefKind => _invoke.RefKind;

            public override ImmutableArray<CustomModifier> RefCustomModifiers => _invoke.RefCustomModifiers;

            internal EndInvokeMethod(InvokeMethod invoke, TypeWithAnnotations iAsyncResultType, DelegateDeclarationSyntax syntax)
                : base((SourceNamedTypeSymbol)invoke.ContainingType, invoke.ReturnTypeWithAnnotations, syntax, MethodKind.Ordinary, DeclarationModifiers.Public | DeclarationModifiers.Virtual)
            {
                _invoke = invoke;
                ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance();
                int num = 0;
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = invoke.Parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SourceParameterSymbol sourceParameterSymbol = (SourceParameterSymbol)enumerator.Current;
                    if (sourceParameterSymbol.RefKind != 0)
                    {
                        SourceClonedParameterSymbol item = new SourceClonedParameterSymbol(sourceParameterSymbol, this, num++, suppressOptional: true);
                        instance.Add(item);
                    }
                }
                instance.Add(SynthesizedParameterSymbol.Create(this, iAsyncResultType, num++, RefKind.None, GetUniqueParameterName(instance, "result")));
                InitializeParameters(instance.ToImmutableAndFree());
            }
        }

        private ImmutableArray<ParameterSymbol> _parameters;

        private readonly TypeWithAnnotations _returnType;

        public sealed override bool IsVararg => false;

        public sealed override ImmutableArray<ParameterSymbol> Parameters => _parameters;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        public sealed override TypeWithAnnotations ReturnTypeWithAnnotations => _returnType;

        public sealed override bool IsImplicitlyDeclared => true;

        internal override bool IsExpressionBodied => false;

        internal override bool GenerateDebugInfo => false;

        protected sealed override IAttributeTargetSymbol AttributeOwner => (SourceNamedTypeSymbol)ContainingSymbol;

        internal sealed override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.CodeTypeMask;

        protected SourceDelegateMethodSymbol(SourceMemberContainerTypeSymbol delegateType, TypeWithAnnotations returnType, DelegateDeclarationSyntax syntax, MethodKind methodKind, DeclarationModifiers declarationModifiers)
            : base(delegateType, syntax.GetReference(), syntax.Identifier.GetLocation(), isIterator: false)
        {
            _returnType = returnType;
            MakeFlags(methodKind, declarationModifiers, _returnType.IsVoidType(), isExtensionMethod: false, isNullableAnalysisEnabled: false);
        }

        protected void InitializeParameters(ImmutableArray<ParameterSymbol> parameters)
        {
            _parameters = parameters;
        }

        internal static void AddDelegateMembers(SourceMemberContainerTypeSymbol delegateType, ArrayBuilder<Symbol> symbols, DelegateDeclarationSyntax syntax, BindingDiagnosticBag diagnostics)
        {
            _ = delegateType.DeclaringCompilation;
            Binder binder = delegateType.GetBinder(syntax.ParameterList);
            TypeSyntax typeSyntax = syntax.ReturnType.SkipRef(out RefKind refKind);
            TypeWithAnnotations returnType = binder.BindType(typeSyntax, diagnostics);
            TypeWithAnnotations voidType = TypeWithAnnotations.Create(binder.GetSpecialType(SpecialType.System_Void, diagnostics, syntax));
            TypeWithAnnotations objectType = TypeWithAnnotations.Create(binder.GetSpecialType(SpecialType.System_Object, diagnostics, syntax));
            TypeWithAnnotations intPtrType = TypeWithAnnotations.Create(binder.GetSpecialType(SpecialType.System_IntPtr, diagnostics, syntax));
            if (returnType.IsRestrictedType(ignoreSpanLikeTypes: true))
            {
                diagnostics.Add(ErrorCode.ERR_MethodReturnCantBeRefAny, typeSyntax.Location, returnType.Type);
            }
            InvokeMethod invokeMethod = new InvokeMethod(delegateType, refKind, returnType, syntax, binder, diagnostics);
            invokeMethod.CheckDelegateVarianceSafety(diagnostics);
            symbols.Add(invokeMethod);
            symbols.Add(new Constructor(delegateType, voidType, objectType, intPtrType, syntax));
            if (binder.Compilation.GetSpecialType(SpecialType.System_IAsyncResult).TypeKind != TypeKind.Error && binder.Compilation.GetSpecialType(SpecialType.System_AsyncCallback).TypeKind != TypeKind.Error && !delegateType.IsCompilationOutputWinMdObj())
            {
                TypeWithAnnotations iAsyncResultType = TypeWithAnnotations.Create(binder.GetSpecialType(SpecialType.System_IAsyncResult, diagnostics, syntax));
                TypeWithAnnotations asyncCallbackType = TypeWithAnnotations.Create(binder.GetSpecialType(SpecialType.System_AsyncCallback, diagnostics, syntax));
                symbols.Add(new BeginInvokeMethod(invokeMethod, iAsyncResultType, objectType, asyncCallbackType, syntax));
                symbols.Add(new EndInvokeMethod(invokeMethod, iAsyncResultType, syntax));
            }
            if (delegateType.DeclaredAccessibility <= Accessibility.Private)
            {
                return;
            }
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, delegateType.ContainingAssembly);
            if (!delegateType.IsNoMoreVisibleThan(invokeMethod.ReturnTypeWithAnnotations, ref useSiteInfo))
            {
                diagnostics.Add(ErrorCode.ERR_BadVisDelegateReturn, delegateType.Locations[0], delegateType, invokeMethod.ReturnType);
            }
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = invokeMethod.Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (!current.TypeWithAnnotations.IsAtLeastAsVisibleAs(delegateType, ref useSiteInfo))
                {
                    diagnostics.Add(ErrorCode.ERR_BadVisDelegateParam, delegateType.Locations[0], delegateType, current.Type);
                }
            }
            diagnostics.Add(delegateType.Locations[0], useSiteInfo);
        }

        protected override void MethodChecks(BindingDiagnosticBag diagnostics)
        {
        }

        public override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            return ImmutableArray<ImmutableArray<TypeWithAnnotations>>.Empty;
        }

        public override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            return ImmutableArray<TypeParameterConstraintKind>.Empty;
        }

        internal sealed override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return OneOrMany.Create(((SourceNamedTypeSymbol)ContainingSymbol).GetAttributeDeclarations());
        }

        internal sealed override AttributeTargets GetAttributeTarget()
        {
            return AttributeTargets.Delegate;
        }

        private static string GetUniqueParameterName(ArrayBuilder<ParameterSymbol> currentParameters, string name)
        {
            while (!IsUnique(currentParameters, name))
            {
                name = "__" + name;
            }
            return name;
        }

        private static bool IsUnique(ArrayBuilder<ParameterSymbol> currentParameters, string name)
        {
            ArrayBuilder<ParameterSymbol>.Enumerator enumerator = currentParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (string.CompareOrdinal(enumerator.Current.Name, name) == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
