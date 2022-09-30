using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SynthesizedEntryPointSymbol : MethodSymbol
    {
        internal sealed class AsyncForwardEntryPoint : SynthesizedEntryPointSymbol
        {
            private readonly CSharpSyntaxNode _userMainReturnTypeSyntax;

            private readonly BoundExpression _getAwaiterGetResultCall;

            private readonly ImmutableArray<ParameterSymbol> _parameters;

            internal readonly MethodSymbol UserMain;

            public override string Name => "<Main>";

            public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

            public override TypeWithAnnotations ReturnTypeWithAnnotations => TypeWithAnnotations.Create(_getAwaiterGetResultCall.Type);

            internal AsyncForwardEntryPoint(CSharpCompilation compilation, NamedTypeSymbol containingType, MethodSymbol userMain)
                : base(containingType)
            {
                UserMain = userMain;
                _userMainReturnTypeSyntax = userMain.ExtractReturnTypeSyntax();
                Binder binder = compilation.GetBinder(_userMainReturnTypeSyntax);
                _parameters = SynthesizedParameterSymbol.DeriveParameters(userMain, this);
                BoundCall expression = new BoundCall(arguments: Parameters.SelectAsArray((Func<ParameterSymbol, CSharpSyntaxNode, BoundExpression>)((ParameterSymbol p, CSharpSyntaxNode s) => new BoundParameter(s, p, p.Type)), _userMainReturnTypeSyntax), syntax: _userMainReturnTypeSyntax, receiverOpt: null, method: userMain, argumentNamesOpt: default(ImmutableArray<string>), argumentRefKindsOpt: default(ImmutableArray<RefKind>), isDelegateCall: false, expanded: false, invokedAsExtensionMethod: false, argsToParamsOpt: default(ImmutableArray<int>), defaultArguments: default(BitVector), resultKind: LookupResultKind.Viable, type: userMain.ReturnType)
                {
                    WasCompilerGenerated = true
                };
                binder.GetAwaitableExpressionInfo(expression, out _getAwaiterGetResultCall, _userMainReturnTypeSyntax, BindingDiagnosticBag.Discarded);
            }

            internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
            {
                base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
                Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeDebuggerStepThroughAttribute());
            }

            internal override BoundBlock CreateBody(BindingDiagnosticBag diagnostics)
            {
                CSharpSyntaxNode userMainReturnTypeSyntax = _userMainReturnTypeSyntax;
                if (ReturnsVoid)
                {
                    return new BoundBlock(userMainReturnTypeSyntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(new BoundExpressionStatement(userMainReturnTypeSyntax, _getAwaiterGetResultCall)
                    {
                        WasCompilerGenerated = true
                    }, (BoundStatement)new BoundReturnStatement(userMainReturnTypeSyntax, RefKind.None, null)
                    {
                        WasCompilerGenerated = true
                    }))
                    {
                        WasCompilerGenerated = true
                    };
                }
                return new BoundBlock(userMainReturnTypeSyntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundStatement)new BoundReturnStatement(userMainReturnTypeSyntax, RefKind.None, _getAwaiterGetResultCall)))
                {
                    WasCompilerGenerated = true
                };
            }
        }

        private sealed class ScriptEntryPoint : SynthesizedEntryPointSymbol
        {
            private readonly TypeWithAnnotations _returnType;

            public override string Name => "<Main>";

            public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

            public override TypeWithAnnotations ReturnTypeWithAnnotations => _returnType;

            internal ScriptEntryPoint(NamedTypeSymbol containingType, TypeWithAnnotations returnType)
                : base(containingType)
            {
                _returnType = returnType;
            }

            internal override BoundBlock CreateBody(BindingDiagnosticBag diagnostics)
            {
                CSharpSyntaxNode cSharpSyntaxNode = DummySyntax();
                CSharpCompilation declaringCompilation = _containingType.DeclaringCompilation;
                Binder next = WithUsingNamespacesAndTypesBinder.Create(declaringCompilation.GlobalImports, new BuckStopsHereBinder(declaringCompilation), withImportChainEntry: true);
                next = new InContainerBinder(declaringCompilation.GlobalNamespace, next);
                SynthesizedInstanceConstructor scriptConstructor = _containingType.GetScriptConstructor();
                SynthesizedInteractiveInitializerMethod scriptInitializer = _containingType.GetScriptInitializer();
                BoundLocal boundLocal = new BoundLocal(cSharpSyntaxNode, new SynthesizedLocal(this, TypeWithAnnotations.Create(_containingType), SynthesizedLocalKind.LoweringTemp), null, _containingType)
                {
                    WasCompilerGenerated = true
                };
                BoundCall expression = CreateParameterlessCall(cSharpSyntaxNode, boundLocal, scriptInitializer);
                if (!next.GetAwaitableExpressionInfo(expression, out var getAwaiterGetResultCall, cSharpSyntaxNode, diagnostics))
                {
                    return new BoundBlock(cSharpSyntaxNode, ImmutableArray<LocalSymbol>.Empty, ImmutableArray<BoundStatement>.Empty, hasErrors: true);
                }
                return new BoundBlock(cSharpSyntaxNode, ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create(new BoundExpressionStatement(cSharpSyntaxNode, new BoundAssignmentOperator(cSharpSyntaxNode, boundLocal, new BoundObjectCreationExpression(cSharpSyntaxNode, scriptConstructor)
                {
                    WasCompilerGenerated = true
                }, _containingType)
                {
                    WasCompilerGenerated = true
                })
                {
                    WasCompilerGenerated = true
                }, new BoundExpressionStatement(cSharpSyntaxNode, getAwaiterGetResultCall)
                {
                    WasCompilerGenerated = true
                }, (BoundStatement)new BoundReturnStatement(cSharpSyntaxNode, RefKind.None, null)
                {
                    WasCompilerGenerated = true
                }))
                {
                    WasCompilerGenerated = true
                };
            }
        }

        private sealed class SubmissionEntryPoint : SynthesizedEntryPointSymbol
        {
            private readonly ImmutableArray<ParameterSymbol> _parameters;

            private readonly TypeWithAnnotations _returnType;

            public override string Name => "<Factory>";

            public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

            public override TypeWithAnnotations ReturnTypeWithAnnotations => _returnType;

            internal SubmissionEntryPoint(NamedTypeSymbol containingType, TypeWithAnnotations returnType, TypeSymbol submissionArrayType)
                : base(containingType)
            {
                _parameters = ImmutableArray.Create(SynthesizedParameterSymbol.Create(this, TypeWithAnnotations.Create(submissionArrayType), 0, RefKind.None, "submissionArray"));
                _returnType = returnType;
            }

            internal override BoundBlock CreateBody(BindingDiagnosticBag diagnostics)
            {
                CSharpSyntaxNode syntax = DummySyntax();
                SynthesizedInstanceConstructor scriptConstructor = _containingType.GetScriptConstructor();
                SynthesizedInteractiveInitializerMethod scriptInitializer = _containingType.GetScriptInitializer();
                BoundParameter item = new BoundParameter(syntax, _parameters[0])
                {
                    WasCompilerGenerated = true
                };
                BoundLocal boundLocal = new BoundLocal(syntax, new SynthesizedLocal(this, TypeWithAnnotations.Create(_containingType), SynthesizedLocalKind.LoweringTemp), null, _containingType)
                {
                    WasCompilerGenerated = true
                };
                BoundExpressionStatement item2 = new BoundExpressionStatement(syntax, new BoundAssignmentOperator(syntax, boundLocal, new BoundObjectCreationExpression(syntax, scriptConstructor, ImmutableArray.Create((BoundExpression)item), default(ImmutableArray<string>), default(ImmutableArray<RefKind>), expanded: false, default(ImmutableArray<int>), default(BitVector), null, null, _containingType)
                {
                    WasCompilerGenerated = true
                }, _containingType)
                {
                    WasCompilerGenerated = true
                })
                {
                    WasCompilerGenerated = true
                };
                BoundCall expressionOpt = CreateParameterlessCall(syntax, boundLocal, scriptInitializer);
                BoundReturnStatement item3 = new BoundReturnStatement(syntax, RefKind.None, expressionOpt)
                {
                    WasCompilerGenerated = true
                };
                return new BoundBlock(syntax, ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create(item2, (BoundStatement)item3))
                {
                    WasCompilerGenerated = true
                };
            }
        }

        internal const string MainName = "<Main>";

        internal const string FactoryName = "<Factory>";

        private readonly NamedTypeSymbol _containingType;

        internal override bool GenerateDebugInfo => false;

        public override Symbol ContainingSymbol => _containingType;

        public abstract override string Name { get; }

        internal override bool HasSpecialName => true;

        internal override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.IL;

        internal override bool RequiresSecurityObject => false;

        public override bool IsVararg => false;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        public override Accessibility DeclaredAccessibility => Accessibility.Private;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override RefKind RefKind => RefKind.None;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        public override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotations => ImmutableArray<TypeWithAnnotations>.Empty;

        public override Symbol AssociatedSymbol => null;

        public override int Arity => 0;

        public override bool ReturnsVoid => base.ReturnType.IsVoidType();

        public sealed override FlowAnalysisAnnotations ReturnTypeFlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public sealed override ImmutableHashSet<string> ReturnNotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

        public sealed override FlowAnalysisAnnotations FlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public override MethodKind MethodKind => MethodKind.Ordinary;

        public override bool IsExtern => false;

        public override bool IsSealed => false;

        public override bool IsAbstract => false;

        public override bool IsOverride => false;

        public override bool IsVirtual => false;

        public override bool IsStatic => true;

        public override bool IsAsync => false;

        public override bool HidesBaseMethodsByName => false;

        public override bool IsExtensionMethod => false;

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal override CallingConvention CallingConvention => CallingConvention.Default;

        internal override bool IsExplicitInterfaceImplementation => false;

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

        internal sealed override bool IsDeclaredReadOnly => false;

        internal sealed override bool IsInitOnly => false;

        internal override bool IsMetadataFinal => false;

        public override bool IsImplicitlyDeclared => true;

        public sealed override bool AreLocalsZeroed => ContainingType.AreLocalsZeroed;

        internal override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation => null;

        internal override bool HasDeclarativeSecurity => false;

        internal static SynthesizedEntryPointSymbol Create(SynthesizedInteractiveInitializerMethod initializerMethod, BindingDiagnosticBag diagnostics)
        {
            NamedTypeSymbol containingType = initializerMethod.ContainingType;
            CSharpCompilation declaringCompilation = containingType.DeclaringCompilation;
            if (declaringCompilation.IsSubmission)
            {
                NamedTypeSymbol specialType = Binder.GetSpecialType(declaringCompilation, SpecialType.System_Object, DummySyntax(), diagnostics);
                ArrayTypeSymbol arrayTypeSymbol = declaringCompilation.CreateArrayTypeSymbol(specialType);
                diagnostics.ReportUseSite(arrayTypeSymbol, NoLocation.Singleton);
                return new SubmissionEntryPoint(containingType, initializerMethod.ReturnTypeWithAnnotations, arrayTypeSymbol);
            }
            NamedTypeSymbol specialType2 = Binder.GetSpecialType(declaringCompilation, SpecialType.System_Void, DummySyntax(), diagnostics);
            return new ScriptEntryPoint(containingType, TypeWithAnnotations.Create(specialType2));
        }

        private SynthesizedEntryPointSymbol(NamedTypeSymbol containingType)
        {
            _containingType = containingType;
        }

        internal abstract BoundBlock CreateBody(BindingDiagnosticBag diagnostics);

        internal sealed override UnmanagedCallersOnlyAttributeData GetUnmanagedCallersOnlyAttributeData(bool forceComplete)
        {
            return null;
        }

        internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        public override DllImportData GetDllImportData()
        {
            return null;
        }

        internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal sealed override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return ImmutableArray<string>.Empty;
        }

        internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            throw ExceptionUtilities.Unreachable;
        }

        private static CSharpSyntaxNode DummySyntax()
        {
            return (CSharpSyntaxNode)CSharpSyntaxTree.Dummy.GetRoot();
        }

        private static BoundCall CreateParameterlessCall(CSharpSyntaxNode syntax, BoundExpression receiver, MethodSymbol method)
        {
            return new BoundCall(syntax, receiver, method, ImmutableArray<BoundExpression>.Empty, default(ImmutableArray<string>), default(ImmutableArray<RefKind>), isDelegateCall: false, expanded: false, invokedAsExtensionMethod: false, default(ImmutableArray<int>), default(BitVector), LookupResultKind.Viable, method.ReturnType)
            {
                WasCompilerGenerated = true
            };
        }

        internal sealed override bool IsNullableAnalysisEnabled()
        {
            return false;
        }
    }
}
