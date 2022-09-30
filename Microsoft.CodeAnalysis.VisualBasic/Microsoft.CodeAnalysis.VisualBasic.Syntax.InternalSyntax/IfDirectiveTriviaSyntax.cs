using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class IfDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal readonly KeywordSyntax _elseKeyword;

		internal readonly KeywordSyntax _ifOrElseIfKeyword;

		internal readonly ExpressionSyntax _condition;

		internal readonly KeywordSyntax _thenKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ElseKeyword => _elseKeyword;

		internal KeywordSyntax IfOrElseIfKeyword => _ifOrElseIfKeyword;

		internal ExpressionSyntax Condition => _condition;

		internal KeywordSyntax ThenKeyword => _thenKeyword;

		internal IfDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax elseKeyword, KeywordSyntax ifOrElseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
			: base(kind, hashToken)
		{
			base._slotCount = 5;
			if (elseKeyword != null)
			{
				AdjustFlagsAndWidth(elseKeyword);
				_elseKeyword = elseKeyword;
			}
			AdjustFlagsAndWidth(ifOrElseIfKeyword);
			_ifOrElseIfKeyword = ifOrElseIfKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			if (thenKeyword != null)
			{
				AdjustFlagsAndWidth(thenKeyword);
				_thenKeyword = thenKeyword;
			}
		}

		internal IfDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax elseKeyword, KeywordSyntax ifOrElseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword, ISyntaxFactoryContext context)
			: base(kind, hashToken)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			if (elseKeyword != null)
			{
				AdjustFlagsAndWidth(elseKeyword);
				_elseKeyword = elseKeyword;
			}
			AdjustFlagsAndWidth(ifOrElseIfKeyword);
			_ifOrElseIfKeyword = ifOrElseIfKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			if (thenKeyword != null)
			{
				AdjustFlagsAndWidth(thenKeyword);
				_thenKeyword = thenKeyword;
			}
		}

		internal IfDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax elseKeyword, KeywordSyntax ifOrElseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
			: base(kind, errors, annotations, hashToken)
		{
			base._slotCount = 5;
			if (elseKeyword != null)
			{
				AdjustFlagsAndWidth(elseKeyword);
				_elseKeyword = elseKeyword;
			}
			AdjustFlagsAndWidth(ifOrElseIfKeyword);
			_ifOrElseIfKeyword = ifOrElseIfKeyword;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			if (thenKeyword != null)
			{
				AdjustFlagsAndWidth(thenKeyword);
				_thenKeyword = thenKeyword;
			}
		}

		internal IfDirectiveTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_elseKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_ifOrElseIfKeyword = keywordSyntax2;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_condition = expressionSyntax;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax3 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax3);
				_thenKeyword = keywordSyntax3;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_elseKeyword);
			writer.WriteValue(_ifOrElseIfKeyword);
			writer.WriteValue(_condition);
			writer.WriteValue(_thenKeyword);
		}

		static IfDirectiveTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new IfDirectiveTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(IfDirectiveTriviaSyntax), (ObjectReader r) => new IfDirectiveTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.IfDirectiveTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _hashToken, 
				1 => _elseKeyword, 
				2 => _ifOrElseIfKeyword, 
				3 => _condition, 
				4 => _thenKeyword, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new IfDirectiveTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _hashToken, _elseKeyword, _ifOrElseIfKeyword, _condition, _thenKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new IfDirectiveTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _hashToken, _elseKeyword, _ifOrElseIfKeyword, _condition, _thenKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitIfDirectiveTrivia(this);
		}
	}
}
