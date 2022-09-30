using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class TypeOfExpressionSyntax : ExpressionSyntax
	{
		internal readonly KeywordSyntax _typeOfKeyword;

		internal readonly ExpressionSyntax _expression;

		internal readonly KeywordSyntax _operatorToken;

		internal readonly TypeSyntax _type;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax TypeOfKeyword => _typeOfKeyword;

		internal ExpressionSyntax Expression => _expression;

		internal KeywordSyntax OperatorToken => _operatorToken;

		internal TypeSyntax Type => _type;

		internal TypeOfExpressionSyntax(SyntaxKind kind, KeywordSyntax typeOfKeyword, ExpressionSyntax expression, KeywordSyntax operatorToken, TypeSyntax type)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(typeOfKeyword);
			_typeOfKeyword = typeOfKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(type);
			_type = type;
		}

		internal TypeOfExpressionSyntax(SyntaxKind kind, KeywordSyntax typeOfKeyword, ExpressionSyntax expression, KeywordSyntax operatorToken, TypeSyntax type, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(typeOfKeyword);
			_typeOfKeyword = typeOfKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(type);
			_type = type;
		}

		internal TypeOfExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax typeOfKeyword, ExpressionSyntax expression, KeywordSyntax operatorToken, TypeSyntax type)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(typeOfKeyword);
			_typeOfKeyword = typeOfKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(type);
			_type = type;
		}

		internal TypeOfExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_typeOfKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_operatorToken = keywordSyntax2;
			}
			TypeSyntax typeSyntax = (TypeSyntax)reader.ReadValue();
			if (typeSyntax != null)
			{
				AdjustFlagsAndWidth(typeSyntax);
				_type = typeSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_typeOfKeyword);
			writer.WriteValue(_expression);
			writer.WriteValue(_operatorToken);
			writer.WriteValue(_type);
		}

		static TypeOfExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new TypeOfExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(TypeOfExpressionSyntax), (ObjectReader r) => new TypeOfExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeOfExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _typeOfKeyword, 
				1 => _expression, 
				2 => _operatorToken, 
				3 => _type, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new TypeOfExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _typeOfKeyword, _expression, _operatorToken, _type);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new TypeOfExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _typeOfKeyword, _expression, _operatorToken, _type);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitTypeOfExpression(this);
		}
	}
}
