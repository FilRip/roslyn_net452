using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EndExternalSourceDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal readonly KeywordSyntax _endKeyword;

		internal readonly KeywordSyntax _externalSourceKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax EndKeyword => _endKeyword;

		internal KeywordSyntax ExternalSourceKeyword => _externalSourceKeyword;

		internal EndExternalSourceDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax externalSourceKeyword)
			: base(kind, hashToken)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(endKeyword);
			_endKeyword = endKeyword;
			AdjustFlagsAndWidth(externalSourceKeyword);
			_externalSourceKeyword = externalSourceKeyword;
		}

		internal EndExternalSourceDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax externalSourceKeyword, ISyntaxFactoryContext context)
			: base(kind, hashToken)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(endKeyword);
			_endKeyword = endKeyword;
			AdjustFlagsAndWidth(externalSourceKeyword);
			_externalSourceKeyword = externalSourceKeyword;
		}

		internal EndExternalSourceDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax externalSourceKeyword)
			: base(kind, errors, annotations, hashToken)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(endKeyword);
			_endKeyword = endKeyword;
			AdjustFlagsAndWidth(externalSourceKeyword);
			_externalSourceKeyword = externalSourceKeyword;
		}

		internal EndExternalSourceDirectiveTriviaSyntax(ObjectReader reader)
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
				_externalSourceKeyword = keywordSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_endKeyword);
			writer.WriteValue(_externalSourceKeyword);
		}

		static EndExternalSourceDirectiveTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new EndExternalSourceDirectiveTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(EndExternalSourceDirectiveTriviaSyntax), (ObjectReader r) => new EndExternalSourceDirectiveTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EndExternalSourceDirectiveTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _hashToken, 
				1 => _endKeyword, 
				2 => _externalSourceKeyword, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new EndExternalSourceDirectiveTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _hashToken, _endKeyword, _externalSourceKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new EndExternalSourceDirectiveTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _hashToken, _endKeyword, _externalSourceKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitEndExternalSourceDirectiveTrivia(this);
		}
	}
}
