using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SynthesizedMainTypeEntryPoint : SynthesizedRegularMethodBase
	{
		public override Accessibility DeclaredAccessibility => Accessibility.Public;

		public override bool IsSub => true;

		public override TypeSymbol ReturnType => ContainingAssembly.GetSpecialType(SpecialType.System_Void);

		internal override bool GenerateDebugInfoImpl => false;

		public SynthesizedMainTypeEntryPoint(VisualBasicSyntaxNode syntaxNode, SourceNamedTypeSymbol container)
			: base(syntaxNode, container, "Main", isShared: true)
		{
		}

		internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, out Binder methodBodyBinder = null)
		{
			methodBodyBinder = null;
			SyntaxNode syntax = base.Syntax;
			SourceNamedTypeSymbol sourceNamedTypeSymbol = (SourceNamedTypeSymbol)base.ContainingSymbol;
			Binder binder = BinderBuilder.CreateBinderForType(sourceNamedTypeSymbol.ContainingSourceModule, syntax.SyntaxTree, sourceNamedTypeSymbol);
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(withDiagnostics: false, diagnostics.AccumulatesDependencies);
			BoundExpression boundExpression = binder.TryDefaultInstanceProperty(new BoundTypeExpression(syntax, sourceNamedTypeSymbol), instance);
			if (boundExpression == null)
			{
				boundExpression = binder.BindObjectCreationExpression(syntax, sourceNamedTypeSymbol, ImmutableArray<BoundExpression>.Empty, diagnostics);
			}
			else
			{
				diagnostics.AddDependencies(instance);
			}
			instance.Free();
			UseSiteInfo<AssemblySymbol> useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
			MethodSymbol item = (MethodSymbol)Binder.GetWellKnownTypeMember(sourceNamedTypeSymbol.DeclaringCompilation, WellKnownMember.System_Windows_Forms_Application__RunForm, out useSiteInfo);
			return new BoundBlock(statements: ImmutableArray.Create(Binder.ReportUseSite(diagnostics, syntax, useSiteInfo) ? ((BoundStatement)new BoundBadStatement(syntax, ImmutableArray<BoundNode>.Empty, hasErrors: true)) : ((BoundStatement)BoundExpressionExtensions.ToStatement(binder.BindInvocationExpression(syntax, syntax, TypeCharacter.None, new BoundMethodGroup(syntax, null, ImmutableArray.Create(item), LookupResultKind.Good, null, QualificationKind.QualifiedViaTypeName), ImmutableArray.Create(boundExpression), default(ImmutableArray<string>), diagnostics, null))), new BoundReturnStatement(syntax, null, null, null)), syntax: syntax, statementListSyntax: default(SyntaxList<StatementSyntax>), locals: ImmutableArray<LocalSymbol>.Empty);
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_STAThreadAttribute__ctor));
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
