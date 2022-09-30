using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SynthesizedEntryPointSymbol : SynthesizedMethodBase
	{
		private sealed class ScriptEntryPoint : SynthesizedEntryPointSymbol
		{
			private readonly MethodSymbol _getAwaiterMethod;

			private readonly MethodSymbol _getResultMethod;

			public override string Name => "<Main>";

			public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

			internal ScriptEntryPoint(NamedTypeSymbol containingType, TypeSymbol returnType, MethodSymbol getAwaiterMethod, MethodSymbol getResultMethod)
				: base(containingType, returnType)
			{
				_getAwaiterMethod = getAwaiterMethod;
				_getResultMethod = getResultMethod;
			}

			internal override BoundBlock CreateBody()
			{
				VisualBasicSyntaxNode syntax = GetSyntax();
				SynthesizedConstructorBase scriptConstructor = _containingType.GetScriptConstructor();
				SynthesizedInteractiveInitializerMethod scriptInitializer = _containingType.GetScriptInitializer();
				BoundLocal boundLocal = BoundNodeExtensions.MakeCompilerGenerated(new BoundLocal(syntax, new SynthesizedLocal(this, _containingType, SynthesizedLocalKind.LoweringTemp), _containingType));
				BoundExpressionStatement item = BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(syntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(syntax, boundLocal, BoundNodeExtensions.MakeCompilerGenerated(new BoundObjectCreationExpression(syntax, scriptConstructor, ImmutableArray<BoundExpression>.Empty, null, _containingType)), suppressObjectClone: false))));
				BoundExpressionStatement item2 = BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(syntax, CreateParameterlessCall(syntax, CreateParameterlessCall(syntax, CreateParameterlessCall(syntax, boundLocal, scriptInitializer), _getAwaiterMethod), _getResultMethod)));
				BoundReturnStatement item3 = BoundNodeExtensions.MakeCompilerGenerated(new BoundReturnStatement(syntax, null, null, null));
				return BoundNodeExtensions.MakeCompilerGenerated(new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundStatement)item, (BoundStatement)item2, (BoundStatement)item3)));
			}
		}

		private sealed class SubmissionEntryPoint : SynthesizedEntryPointSymbol
		{
			private readonly ImmutableArray<ParameterSymbol> _parameters;

			public override string Name => "<Factory>";

			public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

			internal SubmissionEntryPoint(NamedTypeSymbol containingType, TypeSymbol returnType, TypeSymbol submissionArrayType)
				: base(containingType, returnType)
			{
				_parameters = ImmutableArray.Create((ParameterSymbol)new SynthesizedParameterSymbol(this, submissionArrayType, 0, isByRef: false, "submissionArray"));
			}

			internal override BoundBlock CreateBody()
			{
				VisualBasicSyntaxNode syntax = GetSyntax();
				SynthesizedConstructorBase scriptConstructor = _containingType.GetScriptConstructor();
				SynthesizedInteractiveInitializerMethod scriptInitializer = _containingType.GetScriptInitializer();
				ParameterSymbol parameterSymbol = _parameters[0];
				BoundParameter item = BoundNodeExtensions.MakeCompilerGenerated(new BoundParameter(syntax, parameterSymbol, isLValue: false, parameterSymbol.Type));
				BoundLocal boundLocal = BoundNodeExtensions.MakeCompilerGenerated(new BoundLocal(syntax, new SynthesizedLocal(this, _containingType, SynthesizedLocalKind.LoweringTemp), _containingType));
				BoundExpressionStatement item2 = BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(syntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(syntax, boundLocal, BoundNodeExtensions.MakeCompilerGenerated(new BoundObjectCreationExpression(syntax, scriptConstructor, ImmutableArray.Create((BoundExpression)item), null, _containingType)), suppressObjectClone: false))));
				BoundReturnStatement item3 = BoundNodeExtensions.MakeCompilerGenerated(new BoundReturnStatement(syntax, CreateParameterlessCall(syntax, boundLocal, scriptInitializer).MakeRValue(), null, null));
				return BoundNodeExtensions.MakeCompilerGenerated(new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundStatement)item2, (BoundStatement)item3)));
			}
		}

		internal const string MainName = "<Main>";

		internal const string FactoryName = "<Factory>";

		private readonly NamedTypeSymbol _containingType;

		private readonly TypeSymbol _returnType;

		public abstract override string Name { get; }

		internal override bool HasSpecialName => false;

		public override bool IsSub => _returnType.SpecialType == SpecialType.System_Void;

		internal override SyntaxNode Syntax => null;

		public override bool IsOverloads => false;

		public override bool IsShared => true;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		public override Accessibility DeclaredAccessibility => Accessibility.Private;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override TypeSymbol ReturnType => _returnType;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override ImmutableArray<TypeSymbol> TypeArguments => ImmutableArray<TypeSymbol>.Empty;

		public override Symbol AssociatedSymbol => null;

		public override MethodKind MethodKind => MethodKind.Ordinary;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

		internal override bool GenerateDebugInfoImpl => false;

		internal static SynthesizedEntryPointSymbol Create(SynthesizedInteractiveInitializerMethod initializerMethod, BindingDiagnosticBag diagnostics)
		{
			NamedTypeSymbol containingType = initializerMethod.ContainingType;
			VisualBasicCompilation declaringCompilation = containingType.DeclaringCompilation;
			if (declaringCompilation.IsSubmission)
			{
				ArrayTypeSymbol arrayTypeSymbol = declaringCompilation.CreateArrayTypeSymbol(declaringCompilation.GetSpecialType(SpecialType.System_Object));
				ReportUseSiteInfo(arrayTypeSymbol, diagnostics);
				return new SubmissionEntryPoint(containingType, initializerMethod.ReturnType, arrayTypeSymbol);
			}
			NamedTypeSymbol wellKnownType = declaringCompilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task);
			ReportUseSiteInfo(wellKnownType, diagnostics);
			MethodSymbol methodSymbol = (TypeSymbolExtensions.IsErrorType(wellKnownType) ? null : GetRequiredMethod(wellKnownType, "GetAwaiter", diagnostics));
			MethodSymbol getResultMethod = (((object)methodSymbol == null) ? null : GetRequiredMethod(methodSymbol.ReturnType, "GetResult", diagnostics));
			return new ScriptEntryPoint(containingType, declaringCompilation.GetSpecialType(SpecialType.System_Void), methodSymbol, getResultMethod);
		}

		private SynthesizedEntryPointSymbol(NamedTypeSymbol containingType, TypeSymbol returnType)
			: base(containingType)
		{
			_containingType = containingType;
			_returnType = returnType;
		}

		internal abstract BoundBlock CreateBody();

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return LexicalSortKey.NotInSource;
		}

		internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			return false;
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}

		private VisualBasicSyntaxNode GetSyntax()
		{
			return VisualBasicSyntaxTree.Dummy.GetRoot();
		}

		private static void ReportUseSiteInfo(Symbol symbol, BindingDiagnosticBag diagnostics)
		{
			diagnostics.Add(symbol.GetUseSiteInfo(), NoLocation.Singleton);
		}

		private static MethodSymbol GetRequiredMethod(TypeSymbol type, string methodName, BindingDiagnosticBag diagnostics)
		{
			MethodSymbol methodSymbol = type.GetMembers(methodName).SingleOrDefault() as MethodSymbol;
			if ((object)methodSymbol == null)
			{
				diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.ERR_MissingRuntimeHelper, type.MetadataName + "." + methodName), NoLocation.Singleton);
			}
			return methodSymbol;
		}

		private static BoundCall CreateParameterlessCall(VisualBasicSyntaxNode syntax, BoundExpression receiver, MethodSymbol method)
		{
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundCall(syntax, method, null, receiver.MakeRValue(), ImmutableArray<BoundExpression>.Empty, null, method.ReturnType));
		}
	}
}
