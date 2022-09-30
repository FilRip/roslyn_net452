using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class DisableWarningDirectiveTriviaSyntax : DirectiveTriviaSyntax
	{
		internal readonly KeywordSyntax _disableKeyword;

		internal readonly KeywordSyntax _warningKeyword;

		internal readonly GreenNode _errorCodes;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax DisableKeyword => _disableKeyword;

		internal KeywordSyntax WarningKeyword => _warningKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<IdentifierNameSyntax> ErrorCodes => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<IdentifierNameSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<IdentifierNameSyntax>(_errorCodes));

		internal DisableWarningDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax disableKeyword, KeywordSyntax warningKeyword, GreenNode errorCodes)
			: base(kind, hashToken)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(disableKeyword);
			_disableKeyword = disableKeyword;
			AdjustFlagsAndWidth(warningKeyword);
			_warningKeyword = warningKeyword;
			if (errorCodes != null)
			{
				AdjustFlagsAndWidth(errorCodes);
				_errorCodes = errorCodes;
			}
		}

		internal DisableWarningDirectiveTriviaSyntax(SyntaxKind kind, PunctuationSyntax hashToken, KeywordSyntax disableKeyword, KeywordSyntax warningKeyword, GreenNode errorCodes, ISyntaxFactoryContext context)
			: base(kind, hashToken)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(disableKeyword);
			_disableKeyword = disableKeyword;
			AdjustFlagsAndWidth(warningKeyword);
			_warningKeyword = warningKeyword;
			if (errorCodes != null)
			{
				AdjustFlagsAndWidth(errorCodes);
				_errorCodes = errorCodes;
			}
		}

		internal DisableWarningDirectiveTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax hashToken, KeywordSyntax disableKeyword, KeywordSyntax warningKeyword, GreenNode errorCodes)
			: base(kind, errors, annotations, hashToken)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(disableKeyword);
			_disableKeyword = disableKeyword;
			AdjustFlagsAndWidth(warningKeyword);
			_warningKeyword = warningKeyword;
			if (errorCodes != null)
			{
				AdjustFlagsAndWidth(errorCodes);
				_errorCodes = errorCodes;
			}
		}

		internal DisableWarningDirectiveTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_disableKeyword = keywordSyntax;
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
			writer.WriteValue(_disableKeyword);
			writer.WriteValue(_warningKeyword);
			writer.WriteValue(_errorCodes);
		}

		static DisableWarningDirectiveTriviaSyntax()
		{
			CreateInstance = (ObjectReader o) => new DisableWarningDirectiveTriviaSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(DisableWarningDirectiveTriviaSyntax), (ObjectReader r) => new DisableWarningDirectiveTriviaSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _hashToken, 
				1 => _disableKeyword, 
				2 => _warningKeyword, 
				3 => _errorCodes, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new DisableWarningDirectiveTriviaSyntax(base.Kind, newErrors, GetAnnotations(), _hashToken, _disableKeyword, _warningKeyword, _errorCodes);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new DisableWarningDirectiveTriviaSyntax(base.Kind, GetDiagnostics(), annotations, _hashToken, _disableKeyword, _warningKeyword, _errorCodes);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitDisableWarningDirectiveTrivia(this);
		}
	}
}
