using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class RegionDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal readonly KeywordSyntax _regionKeyword;

		internal readonly StringLiteralTokenSyntax _name;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax RegionKeyword => _regionKeyword;

		internal StringLiteralTokenSyntax Name => _name;

		internal RegionDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax regionKeyword, StringLiteralTokenSyntax name)
			: base(kind, hashToken)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(regionKeyword);
			_regionKeyword = regionKeyword;
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal RegionDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax regionKeyword, StringLiteralTokenSyntax name, ISyntaxFactoryContext context)
			: base(kind, hashToken)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(regionKeyword);
			_regionKeyword = regionKeyword;
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal RegionDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax regionKeyword, StringLiteralTokenSyntax name)
			: base(kind, errors, annotations, hashToken)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(regionKeyword);
			_regionKeyword = regionKeyword;
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal RegionDirectiveTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_regionKeyword = keywordSyntax;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax = (StringLiteralTokenSyntax)reader.ReadValue();
			if (stringLiteralTokenSyntax != null)
			{
				AdjustFlagsAndWidth(stringLiteralTokenSyntax);
				_name = stringLiteralTokenSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_regionKeyword);
			writer.WriteValue(_name);
		}

		static RegionDirectiveTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new RegionDirectiveTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(RegionDirectiveTriviaSyntax), (ObjectReader r) => new RegionDirectiveTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.RegionDirectiveTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _hashToken, 
				1 => _regionKeyword, 
				2 => _name, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new RegionDirectiveTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _hashToken, _regionKeyword, _name);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new RegionDirectiveTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _hashToken, _regionKeyword, _name);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitRegionDirectiveTrivia(this);
		}
	}
}
