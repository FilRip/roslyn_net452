using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ReferenceDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal readonly KeywordSyntax _referenceKeyword;

		internal readonly StringLiteralTokenSyntax _file;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ReferenceKeyword => _referenceKeyword;

		internal StringLiteralTokenSyntax File => _file;

		internal ReferenceDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax referenceKeyword, StringLiteralTokenSyntax file)
			: base(kind, hashToken)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(referenceKeyword);
			_referenceKeyword = referenceKeyword;
			AdjustFlagsAndWidth(file);
			_file = file;
		}

		internal ReferenceDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax referenceKeyword, StringLiteralTokenSyntax file, ISyntaxFactoryContext context)
			: base(kind, hashToken)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(referenceKeyword);
			_referenceKeyword = referenceKeyword;
			AdjustFlagsAndWidth(file);
			_file = file;
		}

		internal ReferenceDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax referenceKeyword, StringLiteralTokenSyntax file)
			: base(kind, errors, annotations, hashToken)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(referenceKeyword);
			_referenceKeyword = referenceKeyword;
			AdjustFlagsAndWidth(file);
			_file = file;
		}

		internal ReferenceDirectiveTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_referenceKeyword = keywordSyntax;
			}
			StringLiteralTokenSyntax stringLiteralTokenSyntax = (StringLiteralTokenSyntax)reader.ReadValue();
			if (stringLiteralTokenSyntax != null)
			{
				AdjustFlagsAndWidth(stringLiteralTokenSyntax);
				_file = stringLiteralTokenSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_referenceKeyword);
			writer.WriteValue(_file);
		}

		static ReferenceDirectiveTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new ReferenceDirectiveTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ReferenceDirectiveTriviaSyntax), (ObjectReader r) => new ReferenceDirectiveTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ReferenceDirectiveTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _hashToken, 
				1 => _referenceKeyword, 
				2 => _file, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ReferenceDirectiveTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _hashToken, _referenceKeyword, _file);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ReferenceDirectiveTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _hashToken, _referenceKeyword, _file);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitReferenceDirectiveTrivia(this);
		}
	}
}
