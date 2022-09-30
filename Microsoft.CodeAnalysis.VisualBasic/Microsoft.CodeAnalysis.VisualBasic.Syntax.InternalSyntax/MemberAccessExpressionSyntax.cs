using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class MemberAccessExpressionSyntax : ExpressionSyntax
	{
		internal readonly ExpressionSyntax _expression;

		internal readonly PunctuationSyntax _operatorToken;

		internal readonly SimpleNameSyntax _name;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ExpressionSyntax Expression => _expression;

		internal PunctuationSyntax OperatorToken => _operatorToken;

		internal SimpleNameSyntax Name => _name;

		internal MemberAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, PunctuationSyntax operatorToken, SimpleNameSyntax name)
			: base(kind)
		{
			base._slotCount = 3;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal MemberAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, PunctuationSyntax operatorToken, SimpleNameSyntax name, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal MemberAccessExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax expression, PunctuationSyntax operatorToken, SimpleNameSyntax name)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal MemberAccessExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_operatorToken = punctuationSyntax;
			}
			SimpleNameSyntax simpleNameSyntax = (SimpleNameSyntax)reader.ReadValue();
			if (simpleNameSyntax != null)
			{
				AdjustFlagsAndWidth(simpleNameSyntax);
				_name = simpleNameSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_expression);
			writer.WriteValue(_operatorToken);
			writer.WriteValue(_name);
		}

		static MemberAccessExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new MemberAccessExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(MemberAccessExpressionSyntax), (ObjectReader r) => new MemberAccessExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _expression, 
				1 => _operatorToken, 
				2 => _name, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new MemberAccessExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _expression, _operatorToken, _name);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new MemberAccessExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _expression, _operatorToken, _name);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitMemberAccessExpression(this);
		}
	}
}
