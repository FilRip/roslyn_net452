using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class TypeArgumentListSyntax : VisualBasicSyntaxNode
	{
		internal readonly PunctuationSyntax _openParenToken;

		internal readonly KeywordSyntax _ofKeyword;

		internal readonly GreenNode _arguments;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal KeywordSyntax OfKeyword => _ofKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax> Arguments => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeSyntax>(_arguments));

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal TypeArgumentListSyntax(SyntaxKind kind, PunctuationSyntax openParenToken, KeywordSyntax ofKeyword, GreenNode arguments, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(ofKeyword);
			_ofKeyword = ofKeyword;
			if (arguments != null)
			{
				AdjustFlagsAndWidth(arguments);
				_arguments = arguments;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal TypeArgumentListSyntax(SyntaxKind kind, PunctuationSyntax openParenToken, KeywordSyntax ofKeyword, GreenNode arguments, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(ofKeyword);
			_ofKeyword = ofKeyword;
			if (arguments != null)
			{
				AdjustFlagsAndWidth(arguments);
				_arguments = arguments;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal TypeArgumentListSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, KeywordSyntax ofKeyword, GreenNode arguments, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(ofKeyword);
			_ofKeyword = ofKeyword;
			if (arguments != null)
			{
				AdjustFlagsAndWidth(arguments);
				_arguments = arguments;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal TypeArgumentListSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_openParenToken = punctuationSyntax;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_ofKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_arguments = greenNode;
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
			writer.WriteValue(_openParenToken);
			writer.WriteValue(_ofKeyword);
			writer.WriteValue(_arguments);
			writer.WriteValue(_closeParenToken);
		}

		static TypeArgumentListSyntax()
		{
			CreateInstance = (ObjectReader o) => new TypeArgumentListSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(TypeArgumentListSyntax), (ObjectReader r) => new TypeArgumentListSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _openParenToken, 
				1 => _ofKeyword, 
				2 => _arguments, 
				3 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new TypeArgumentListSyntax(base.Kind, newErrors, GetAnnotations(), _openParenToken, _ofKeyword, _arguments, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new TypeArgumentListSyntax(base.Kind, GetDiagnostics(), annotations, _openParenToken, _ofKeyword, _arguments, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitTypeArgumentList(this);
		}
	}
}
