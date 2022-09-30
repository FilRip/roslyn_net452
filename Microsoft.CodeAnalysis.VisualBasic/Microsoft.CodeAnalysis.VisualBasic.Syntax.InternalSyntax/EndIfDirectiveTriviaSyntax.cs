using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EndIfDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal readonly KeywordSyntax _endKeyword;

		internal readonly KeywordSyntax _ifKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax EndKeyword => _endKeyword;

		internal KeywordSyntax IfKeyword => _ifKeyword;

		internal EndIfDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax ifKeyword)
			: base(kind, hashToken)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(endKeyword);
			_endKeyword = endKeyword;
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
		}

		internal EndIfDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax ifKeyword, ISyntaxFactoryContext context)
			: base(kind, hashToken)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(endKeyword);
			_endKeyword = endKeyword;
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
		}

		internal EndIfDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax ifKeyword)
			: base(kind, errors, annotations, hashToken)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(endKeyword);
			_endKeyword = endKeyword;
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
		}

		internal EndIfDirectiveTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_endKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_ifKeyword = keywordSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_endKeyword);
			writer.WriteValue(_ifKeyword);
		}

		static EndIfDirectiveTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new EndIfDirectiveTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(EndIfDirectiveTriviaSyntax), (ObjectReader r) => new EndIfDirectiveTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EndIfDirectiveTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _hashToken, 
				1 => _endKeyword, 
				2 => _ifKeyword, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new EndIfDirectiveTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _hashToken, _endKeyword, _ifKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new EndIfDirectiveTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _hashToken, _endKeyword, _ifKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitEndIfDirectiveTrivia(this);
		}
	}
}
