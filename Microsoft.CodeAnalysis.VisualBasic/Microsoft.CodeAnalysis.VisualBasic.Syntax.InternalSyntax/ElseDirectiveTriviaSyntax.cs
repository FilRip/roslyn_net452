using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ElseDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal readonly KeywordSyntax _elseKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ElseKeyword => _elseKeyword;

		internal ElseDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax elseKeyword)
			: base(kind, hashToken)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(elseKeyword);
			_elseKeyword = elseKeyword;
		}

		internal ElseDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax elseKeyword, ISyntaxFactoryContext context)
			: base(kind, hashToken)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(elseKeyword);
			_elseKeyword = elseKeyword;
		}

		internal ElseDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax elseKeyword)
			: base(kind, errors, annotations, hashToken)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(elseKeyword);
			_elseKeyword = elseKeyword;
		}

		internal ElseDirectiveTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_elseKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_elseKeyword);
		}

		static ElseDirectiveTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new ElseDirectiveTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ElseDirectiveTriviaSyntax), (ObjectReader r) => new ElseDirectiveTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseDirectiveTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _hashToken, 
				1 => _elseKeyword, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ElseDirectiveTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _hashToken, _elseKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ElseDirectiveTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _hashToken, _elseKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitElseDirectiveTrivia(this);
		}
	}
}
