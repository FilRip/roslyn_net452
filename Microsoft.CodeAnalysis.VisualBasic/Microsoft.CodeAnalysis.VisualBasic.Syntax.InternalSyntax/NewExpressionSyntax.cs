using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class NewExpressionSyntax : ExpressionSyntax
	{
		internal readonly KeywordSyntax _newKeyword;

		internal readonly GreenNode _attributeLists;

		internal KeywordSyntax NewKeyword => _newKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(_attributeLists);

		internal NewExpressionSyntax(SyntaxKind kind, KeywordSyntax newKeyword, GreenNode attributeLists)
			: base(kind)
		{
			AdjustFlagsAndWidth(newKeyword);
			_newKeyword = newKeyword;
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
		}

		internal NewExpressionSyntax(SyntaxKind kind, KeywordSyntax newKeyword, GreenNode attributeLists, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
			AdjustFlagsAndWidth(newKeyword);
			_newKeyword = newKeyword;
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
		}

		internal NewExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax newKeyword, GreenNode attributeLists)
			: base(kind, errors, annotations)
		{
			AdjustFlagsAndWidth(newKeyword);
			_newKeyword = newKeyword;
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
		}

		internal NewExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_newKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_attributeLists = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_newKeyword);
			writer.WriteValue(_attributeLists);
		}
	}
}
