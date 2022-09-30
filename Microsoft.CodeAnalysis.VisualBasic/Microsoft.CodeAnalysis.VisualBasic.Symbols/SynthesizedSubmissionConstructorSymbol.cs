using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedSubmissionConstructorSymbol : SynthesizedConstructorBase
	{
		private readonly ImmutableArray<ParameterSymbol> _parameters;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		internal override bool GenerateDebugInfoImpl => false;

		internal SynthesizedSubmissionConstructorSymbol(SyntaxReference syntaxReference, NamedTypeSymbol container, bool isShared, Binder binder, BindingDiagnosticBag diagnostics)
			: base(syntaxReference, container, isShared, binder, diagnostics)
		{
			VisualBasicCompilation declaringCompilation = container.DeclaringCompilation;
			ArrayTypeSymbol arrayTypeSymbol = declaringCompilation.CreateArrayTypeSymbol(declaringCompilation.GetSpecialType(SpecialType.System_Object));
			diagnostics.Add(arrayTypeSymbol.GetUseSiteInfo(), NoLocation.Singleton);
			_parameters = ImmutableArray.Create((ParameterSymbol)new SynthesizedParameterSymbol(this, arrayTypeSymbol, 0, isByRef: false, "submissionArray"));
		}

		internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
		{
			SyntaxNode syntax = base.Syntax;
			return new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundStatement)new BoundReturnStatement(syntax, null, null, null)));
		}

		internal static ImmutableArray<BoundStatement> MakeSubmissionInitialization(SyntaxNode syntax, MethodSymbol constructor, SynthesizedSubmissionFields synthesizedFields, VisualBasicCompilation compilation)
		{
			List<BoundStatement> list = new List<BoundStatement>();
			BoundParameter expression = new BoundParameter(syntax, constructor.Parameters[0], isLValue: false, constructor.Parameters[0].Type);
			compilation.CreateArrayTypeSymbol(compilation.GetSpecialType(SpecialType.System_Object));
			NamedTypeSymbol specialType = compilation.GetSpecialType(SpecialType.System_Int32);
			NamedTypeSymbol specialType2 = compilation.GetSpecialType(SpecialType.System_Object);
			BoundMeReference boundMeReference = new BoundMeReference(syntax, constructor.ContainingType);
			int submissionSlotIndex = compilation.GetSubmissionSlotIndex();
			list.Add(BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(syntax, new BoundAssignmentOperator(syntax, new BoundArrayAccess(syntax, expression, ImmutableArray.Create((BoundExpression)new BoundLiteral(syntax, ConstantValue.Create(submissionSlotIndex), specialType)), isLValue: true, specialType2), new BoundDirectCast(syntax, boundMeReference, ConversionKind.Reference, specialType2), suppressObjectClone: true, specialType2))));
			FieldSymbol hostObjectField = synthesizedFields.GetHostObjectField();
			if ((object)hostObjectField != null)
			{
				list.Add(new BoundExpressionStatement(syntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(syntax, new BoundFieldAccess(syntax, boundMeReference, hostObjectField, isLValue: true, hostObjectField.Type), new BoundDirectCast(syntax, new BoundArrayAccess(syntax, expression, ImmutableArray.Create((BoundExpression)new BoundLiteral(syntax, ConstantValue.Create(0), specialType)), isLValue: false, specialType2), ConversionKind.Reference, hostObjectField.Type), suppressObjectClone: true, hostObjectField.Type))));
			}
			foreach (FieldSymbol fieldSymbol in synthesizedFields.FieldSymbols)
			{
				ImplicitNamedTypeSymbol implicitNamedTypeSymbol = (ImplicitNamedTypeSymbol)fieldSymbol.Type;
				int submissionSlotIndex2 = implicitNamedTypeSymbol.DeclaringCompilation.GetSubmissionSlotIndex();
				list.Add(BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(syntax, new BoundAssignmentOperator(syntax, new BoundFieldAccess(syntax, boundMeReference, fieldSymbol, isLValue: true, implicitNamedTypeSymbol), new BoundDirectCast(syntax, new BoundArrayAccess(syntax, expression, ImmutableArray.Create((BoundExpression)new BoundLiteral(syntax, ConstantValue.Create(submissionSlotIndex2), specialType)), isLValue: false, specialType2), ConversionKind.Reference, implicitNamedTypeSymbol), suppressObjectClone: true, implicitNamedTypeSymbol))));
			}
			return list.AsImmutableOrNull();
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			return ((SourceMemberContainerTypeSymbol)base.ContainingType).CalculateSyntaxOffsetInSynthesizedConstructor(localPosition, localTree, base.IsShared);
		}
	}
}
