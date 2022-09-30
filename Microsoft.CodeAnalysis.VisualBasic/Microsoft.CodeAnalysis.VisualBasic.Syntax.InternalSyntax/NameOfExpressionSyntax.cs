using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class NameOfExpressionSyntax : ExpressionSyntax
	{
		internal readonly KeywordSyntax _nameOfKeyword;

		internal readonly PunctuationSyntax _openParenToken;

		internal readonly ExpressionSyntax _argument;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax NameOfKeyword => _nameOfKeyword;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal ExpressionSyntax Argument => _argument;

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal NameOfExpressionSyntax(SyntaxKind kind, KeywordSyntax nameOfKeyword, PunctuationSyntax openParenToken, ExpressionSyntax argument, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(nameOfKeyword);
			_nameOfKeyword = nameOfKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(argument);
			_argument = argument;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal NameOfExpressionSyntax(SyntaxKind kind, KeywordSyntax nameOfKeyword, PunctuationSyntax openParenToken, ExpressionSyntax argument, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(nameOfKeyword);
			_nameOfKeyword = nameOfKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(argument);
			_argument = argument;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal NameOfExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax nameOfKeyword, PunctuationSyntax openParenToken, ExpressionSyntax argument, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(nameOfKeyword);
			_nameOfKeyword = nameOfKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(argument);
			_argument = argument;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal NameOfExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_nameOfKeyword = keywordSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_openParenToken = punctuationSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_argument = expressionSyntax;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_closeParenToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_nameOfKeyword);
			writer.WriteValue(_openParenToken);
			writer.WriteValue(_argument);
			writer.WriteValue(_closeParenToken);
		}

		static NameOfExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new NameOfExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(NameOfExpressionSyntax), (ObjectReader r) => new NameOfExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NameOfExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _nameOfKeyword, 
				1 => _openParenToken, 
				2 => _argument, 
				3 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new NameOfExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _nameOfKeyword, _openParenToken, _argument, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new NameOfExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _nameOfKeyword, _openParenToken, _argument, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitNameOfExpression(this);
		}
	}
}
