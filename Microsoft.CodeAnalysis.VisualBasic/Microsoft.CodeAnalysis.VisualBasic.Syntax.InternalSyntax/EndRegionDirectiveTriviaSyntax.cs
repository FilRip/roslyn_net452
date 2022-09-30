using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EndRegionDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal readonly KeywordSyntax _endKeyword;

		internal readonly KeywordSyntax _regionKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax EndKeyword => _endKeyword;

		internal KeywordSyntax RegionKeyword => _regionKeyword;

		internal EndRegionDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax regionKeyword)
			: base(kind, hashToken)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(endKeyword);
			_endKeyword = endKeyword;
			AdjustFlagsAndWidth(regionKeyword);
			_regionKeyword = regionKeyword;
		}

		internal EndRegionDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax regionKeyword, ISyntaxFactoryContext context)
			: base(kind, hashToken)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(endKeyword);
			_endKeyword = endKeyword;
			AdjustFlagsAndWidth(regionKeyword);
			_regionKeyword = regionKeyword;
		}

		internal EndRegionDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax regionKeyword)
			: base(kind, errors, annotations, hashToken)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(endKeyword);
			_endKeyword = endKeyword;
			AdjustFlagsAndWidth(regionKeyword);
			_regionKeyword = regionKeyword;
		}

		internal EndRegionDirectiveTriviaSyntax(ObjectReader reader)
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
				_regionKeyword = keywordSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_endKeyword);
			writer.WriteValue(_regionKeyword);
		}

		static EndRegionDirectiveTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new EndRegionDirectiveTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(EndRegionDirectiveTriviaSyntax), (ObjectReader r) => new EndRegionDirectiveTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EndRegionDirectiveTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _hashToken, 
				1 => _endKeyword, 
				2 => _regionKeyword, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new EndRegionDirectiveTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _hashToken, _endKeyword, _regionKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new EndRegionDirectiveTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _hashToken, _endKeyword, _regionKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitEndRegionDirectiveTrivia(this);
		}
	}
}
