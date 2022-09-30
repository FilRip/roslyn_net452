using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class InferredFieldInitializerSyntax : FieldInitializerSyntax
	{
		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ExpressionSyntax Expression => _expression;

		internal InferredFieldInitializerSyntax(SyntaxKind kind, KeywordSyntax keyKeyword, ExpressionSyntax expression)
			: base(kind, keyKeyword)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal InferredFieldInitializerSyntax(SyntaxKind kind, KeywordSyntax keyKeyword, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind, keyKeyword)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal InferredFieldInitializerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyKeyword, ExpressionSyntax expression)
			: base(kind, errors, annotations, keyKeyword)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal InferredFieldInitializerSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_expression);
		}

		static InferredFieldInitializerSyntax()
		{
			CreateInstance = (ObjectReader o) => new InferredFieldInitializerSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(InferredFieldInitializerSyntax), (ObjectReader r) => new InferredFieldInitializerSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InferredFieldInitializerSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _keyKeyword, 
				1 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new InferredFieldInitializerSyntax(base.Kind, newErrors, GetAnnotations(), _keyKeyword, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new InferredFieldInitializerSyntax(base.Kind, GetDiagnostics(), annotations, _keyKeyword, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitInferredFieldInitializer(this);
		}
	}
}
