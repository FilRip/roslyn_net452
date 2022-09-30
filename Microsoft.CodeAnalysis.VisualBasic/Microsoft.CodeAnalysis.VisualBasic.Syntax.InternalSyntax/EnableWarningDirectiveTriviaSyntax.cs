using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EnableWarningDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal readonly KeywordSyntax _enableKeyword;

		internal readonly KeywordSyntax _warningKeyword;

		internal readonly GreenNode _errorCodes;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax EnableKeyword => _enableKeyword;

		internal KeywordSyntax WarningKeyword => _warningKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<IdentifierNameSyntax> ErrorCodes => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<IdentifierNameSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<IdentifierNameSyntax>(_errorCodes));

		internal EnableWarningDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax enableKeyword, KeywordSyntax warningKeyword, GreenNode errorCodes)
			: base(kind, hashToken)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(enableKeyword);
			_enableKeyword = enableKeyword;
			AdjustFlagsAndWidth(warningKeyword);
			_warningKeyword = warningKeyword;
			if (errorCodes != null)
			{
				AdjustFlagsAndWidth(errorCodes);
				_errorCodes = errorCodes;
			}
		}

		internal EnableWarningDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax enableKeyword, KeywordSyntax warningKeyword, GreenNode errorCodes, ISyntaxFactoryContext context)
			: base(kind, hashToken)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(enableKeyword);
			_enableKeyword = enableKeyword;
			AdjustFlagsAndWidth(warningKeyword);
			_warningKeyword = warningKeyword;
			if (errorCodes != null)
			{
				AdjustFlagsAndWidth(errorCodes);
				_errorCodes = errorCodes;
			}
		}

		internal EnableWarningDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax enableKeyword, KeywordSyntax warningKeyword, GreenNode errorCodes)
			: base(kind, errors, annotations, hashToken)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(enableKeyword);
			_enableKeyword = enableKeyword;
			AdjustFlagsAndWidth(warningKeyword);
			_warningKeyword = warningKeyword;
			if (errorCodes != null)
			{
				AdjustFlagsAndWidth(errorCodes);
				_errorCodes = errorCodes;
			}
		}

		internal EnableWarningDirectiveTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_enableKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_warningKeyword = keywordSyntax2;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_errorCodes = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_enableKeyword);
			writer.WriteValue(_warningKeyword);
			writer.WriteValue(_errorCodes);
		}

		static EnableWarningDirectiveTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new EnableWarningDirectiveTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(EnableWarningDirectiveTriviaSyntax), (ObjectReader r) => new EnableWarningDirectiveTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EnableWarningDirectiveTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _hashToken, 
				1 => _enableKeyword, 
				2 => _warningKeyword, 
				3 => _errorCodes, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new EnableWarningDirectiveTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _hashToken, _enableKeyword, _warningKeyword, _errorCodes);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new EnableWarningDirectiveTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _hashToken, _enableKeyword, _warningKeyword, _errorCodes);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitEnableWarningDirectiveTrivia(this);
		}
	}
}
