using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ConstDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal readonly KeywordSyntax _constKeyword;

		internal readonly IdentifierTokenSyntax _name;

		internal readonly PunctuationSyntax _equalsToken;

		internal readonly ExpressionSyntax _value;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ConstKeyword => _constKeyword;

		internal IdentifierTokenSyntax Name => _name;

		internal PunctuationSyntax EqualsToken => _equalsToken;

		internal ExpressionSyntax Value => _value;

		internal ConstDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax constKeyword, IdentifierTokenSyntax name, PunctuationSyntax equalsToken, ExpressionSyntax value)
			: base(kind, hashToken)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(constKeyword);
			_constKeyword = constKeyword;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal ConstDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax constKeyword, IdentifierTokenSyntax name, PunctuationSyntax equalsToken, ExpressionSyntax value, ISyntaxFactoryContext context)
			: base(kind, hashToken)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(constKeyword);
			_constKeyword = constKeyword;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal ConstDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax constKeyword, IdentifierTokenSyntax name, PunctuationSyntax equalsToken, ExpressionSyntax value)
			: base(kind, errors, annotations, hashToken)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(constKeyword);
			_constKeyword = constKeyword;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal ConstDirectiveTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_constKeyword = keywordSyntax;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_name = identifierTokenSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_equalsToken = punctuationSyntax;
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
			writer.WriteValue(_constKeyword);
			writer.WriteValue(_name);
			writer.WriteValue(_equalsToken);
			writer.WriteValue(_value);
		}

		static ConstDirectiveTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new ConstDirectiveTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ConstDirectiveTriviaSyntax), (ObjectReader r) => new ConstDirectiveTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstDirectiveTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _hashToken, 
				1 => _constKeyword, 
				2 => _name, 
				3 => _equalsToken, 
				4 => _value, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ConstDirectiveTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _hashToken, _constKeyword, _name, _equalsToken, _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ConstDirectiveTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _hashToken, _constKeyword, _name, _equalsToken, _value);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitConstDirectiveTrivia(this);
		}
	}
}
