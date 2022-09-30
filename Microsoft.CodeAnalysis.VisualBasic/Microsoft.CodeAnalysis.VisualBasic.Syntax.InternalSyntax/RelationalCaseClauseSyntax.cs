using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class RelationalCaseClauseSyntax : CaseClauseSyntax
	{
		internal readonly KeywordSyntax _isKeyword;

		internal readonly PunctuationSyntax _operatorToken;

		internal readonly ExpressionSyntax _value;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax IsKeyword => _isKeyword;

		internal PunctuationSyntax OperatorToken => _operatorToken;

		internal ExpressionSyntax Value => _value;

		internal RelationalCaseClauseSyntax(SyntaxKind kind, KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
			: base(kind)
		{
			base._slotCount = 3;
			if (isKeyword != null)
			{
				AdjustFlagsAndWidth(isKeyword);
				_isKeyword = isKeyword;
			}
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal RelationalCaseClauseSyntax(SyntaxKind kind, KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			if (isKeyword != null)
			{
				AdjustFlagsAndWidth(isKeyword);
				_isKeyword = isKeyword;
			}
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal RelationalCaseClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			if (isKeyword != null)
			{
				AdjustFlagsAndWidth(isKeyword);
				_isKeyword = isKeyword;
			}
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal RelationalCaseClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_isKeyword = keywordSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_operatorToken = punctuationSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_value = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_isKeyword);
			writer.WriteValue(_operatorToken);
			writer.WriteValue(_value);
		}

		static RelationalCaseClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new RelationalCaseClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(RelationalCaseClauseSyntax), (ObjectReader r) => new RelationalCaseClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.RelationalCaseClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _isKeyword, 
				1 => _operatorToken, 
				2 => _value, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new RelationalCaseClauseSyntax(base.Kind, newErrors, GetAnnotations(), _isKeyword, _operatorToken, _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new RelationalCaseClauseSyntax(base.Kind, GetDiagnostics(), annotations, _isKeyword, _operatorToken, _value);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitRelationalCaseClause(this);
		}
	}
}
