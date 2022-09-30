using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedInteractiveInitializerMethod : SynthesizedMethodBase
	{
		internal const string InitializerName = "<Initialize>";

		internal readonly TypeSymbol ResultType;

		internal readonly LocalSymbol FunctionLocal;

		internal readonly LabelSymbol ExitLabel;

		private readonly SyntaxReference _syntaxReference;

		private readonly TypeSymbol _returnType;

		public override string Name => "<Initialize>";

		internal override bool IsScriptInitializer => true;

		public override Accessibility DeclaredAccessibility => Accessibility.Internal;

		public override bool IsAsync => true;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverloads => false;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public override bool IsShared => false;

		public override bool IsSub => _returnType.SpecialType == SpecialType.System_Void;

		public override ImmutableArray<Location> Locations => m_containingType.Locations;

		public override MethodKind MethodKind => MethodKind.Ordinary;

		public override TypeSymbol ReturnType => _returnType;

		internal override bool GenerateDebugInfoImpl => true;

		internal override bool HasSpecialName => true;

		internal override SyntaxNode Syntax => (VisualBasicSyntaxNode)_syntaxReference.GetSyntax();

		internal SynthesizedInteractiveInitializerMethod(SyntaxReference syntaxReference, SourceMemberContainerTypeSymbol containingType, BindingDiagnosticBag diagnostics)
			: base(containingType)
		{
			_syntaxReference = syntaxReference;
			CalculateReturnType(containingType.DeclaringCompilation, diagnostics, ref ResultType, ref _returnType);
			FunctionLocal = new SynthesizedLocal(this, ResultType, SynthesizedLocalKind.FunctionReturnValue, Syntax);
			ExitLabel = new GeneratedLabelSymbol("exit");
		}

		internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
		{
			SyntaxNode syntax = Syntax;
			return new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray.Create(FunctionLocal), ImmutableArray.Create((BoundStatement)new BoundLabelStatement(syntax, ExitLabel)));
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			return ((SourceMemberContainerTypeSymbol)m_containingType).CalculateSyntaxOffsetInSynthesizedConstructor(localPosition, localTree, isShared: false);
		}

		private static void CalculateReturnType(VisualBasicCompilation compilation, BindingDiagnosticBag diagnostics, ref TypeSymbol resultType, ref TypeSymbol returnType)
		{
			Type type = null;
			if (compilation.ScriptCompilationInfo != null)
			{
				type = compilation.ScriptCompilationInfo.ReturnTypeOpt;
			}
			NamedTypeSymbol wellKnownType = compilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task_T);
			diagnostics.Add(wellKnownType.GetUseSiteInfo(), NoLocation.Singleton);
			if ((object)type == null)
			{
				resultType = compilation.GetSpecialType(SpecialType.System_Object);
			}
			else
			{
				resultType = compilation.GetTypeByReflectionType(type);
			}
			returnType = wellKnownType.Construct(resultType);
		}
	}
}
