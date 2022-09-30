using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class UsingStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _usingKeyword;

		internal readonly ExpressionSyntax _expression;

		internal readonly GreenNode _variables;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax UsingKeyword => _usingKeyword;

		internal ExpressionSyntax Expression => _expression;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax> Variables => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VariableDeclaratorSyntax>(_variables));

		internal UsingStatementSyntax(SyntaxKind kind, KeywordSyntax usingKeyword, ExpressionSyntax expression, GreenNode variables)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(usingKeyword);
			_usingKeyword = usingKeyword;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
		}

		internal UsingStatementSyntax(SyntaxKind kind, KeywordSyntax usingKeyword, ExpressionSyntax expression, GreenNode variables, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(usingKeyword);
			_usingKeyword = usingKeyword;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
		}

		internal UsingStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax usingKeyword, ExpressionSyntax expression, GreenNode variables)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(usingKeyword);
			_usingKeyword = usingKeyword;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
		}

		internal UsingStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_usingKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_variables = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_usingKeyword);
			writer.WriteValue(_expression);
			writer.WriteValue(_variables);
		}

		static UsingStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new UsingStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(UsingStatementSyntax), (ObjectReader r) => new UsingStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _usingKeyword, 
				1 => _expression, 
				2 => _variables, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new UsingStatementSyntax(base.Kind, newErrors, GetAnnotations(), _usingKeyword, _expression, _variables);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new UsingStatementSyntax(base.Kind, GetDiagnostics(), annotations, _usingKeyword, _expression, _variables);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitUsingStatement(this);
		}
	}
}
