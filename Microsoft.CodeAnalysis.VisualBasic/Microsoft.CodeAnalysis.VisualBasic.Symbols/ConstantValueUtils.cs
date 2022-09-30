using System.Diagnostics;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class ConstantValueUtils
	{
		[DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
		internal struct FieldInfo
		{
			public readonly SourceFieldSymbol Field;

			public readonly bool StartsCycle;

			public FieldInfo(SourceFieldSymbol field, bool startsCycle)
			{
				this = default(FieldInfo);
				Field = field;
				StartsCycle = startsCycle;
			}

			private string GetDebuggerDisplay()
			{
				string text = Field.ToString();
				if (StartsCycle)
				{
					text += " [cycle]";
				}
				return text;
			}
		}

		public static EvaluatedConstant EvaluateFieldConstant(SourceFieldSymbol field, SyntaxReference equalsValueOrAsNewNodeRef, ConstantFieldsInProgress.Dependencies dependencies, BindingDiagnosticBag diagnostics)
		{
			Binder binder = BinderBuilder.CreateBinderForType(field.ContainingSourceType.ContainingSourceModule, equalsValueOrAsNewNodeRef.SyntaxTree, field.ContainingSourceType);
			VisualBasicSyntaxNode visualBasicSyntax = VisualBasicExtensions.GetVisualBasicSyntax(equalsValueOrAsNewNodeRef);
			ConstantFieldsInProgressBinder binder2 = new ConstantFieldsInProgressBinder(new ConstantFieldsInProgress(field, dependencies), binder, field);
			ConstantValue constValue = null;
			BoundExpression boundExpression = BindFieldOrEnumInitializer(binder2, field, visualBasicSyntax, diagnostics, out constValue);
			TypeSymbol type = ((!BoundExpressionExtensions.IsNothingLiteral(boundExpression)) ? boundExpression.Type : binder.GetSpecialType(SpecialType.System_Object, visualBasicSyntax, diagnostics));
			ConstantValue value = constValue ?? ConstantValue.Bad;
			return new EvaluatedConstant(value, type);
		}

		private static BoundExpression BindFieldOrEnumInitializer(Binder binder, FieldSymbol fieldOrEnumSymbol, VisualBasicSyntaxNode equalsValueOrAsNewSyntax, BindingDiagnosticBag diagnostics, out ConstantValue constValue)
		{
			if (fieldOrEnumSymbol is SourceEnumConstantSymbol fieldSymbol)
			{
				return binder.BindFieldAndEnumConstantInitializer(fieldSymbol, equalsValueOrAsNewSyntax, isEnum: true, diagnostics, out constValue);
			}
			SourceFieldSymbol fieldSymbol2 = (SourceFieldSymbol)fieldOrEnumSymbol;
			return binder.BindFieldAndEnumConstantInitializer(fieldSymbol2, equalsValueOrAsNewSyntax, isEnum: false, diagnostics, out constValue);
		}
	}
}
