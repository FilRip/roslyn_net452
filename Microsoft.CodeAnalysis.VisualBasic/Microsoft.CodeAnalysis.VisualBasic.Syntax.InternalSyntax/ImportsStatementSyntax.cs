using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ImportsStatementSyntax : DeclarationStatementSyntax
	{
		internal readonly KeywordSyntax _importsKeyword;

		internal readonly GreenNode _importsClauses;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ImportsKeyword => _importsKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ImportsClauseSyntax> ImportsClauses => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ImportsClauseSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImportsClauseSyntax>(_importsClauses));

		internal ImportsStatementSyntax(SyntaxKind kind, KeywordSyntax importsKeyword, GreenNode importsClauses)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(importsKeyword);
			_importsKeyword = importsKeyword;
			if (importsClauses != null)
			{
				AdjustFlagsAndWidth(importsClauses);
				_importsClauses = importsClauses;
			}
		}

		internal ImportsStatementSyntax(SyntaxKind kind, KeywordSyntax importsKeyword, GreenNode importsClauses, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(importsKeyword);
			_importsKeyword = importsKeyword;
			if (importsClauses != null)
			{
				AdjustFlagsAndWidth(importsClauses);
				_importsClauses = importsClauses;
			}
		}

		internal ImportsStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax importsKeyword, GreenNode importsClauses)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(importsKeyword);
			_importsKeyword = importsKeyword;
			if (importsClauses != null)
			{
				AdjustFlagsAndWidth(importsClauses);
				_importsClauses = importsClauses;
			}
		}

		internal ImportsStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_importsKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_importsClauses = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_importsKeyword);
			writer.WriteValue(_importsClauses);
		}

		static ImportsStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ImportsStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ImportsStatementSyntax), (ObjectReader r) => new ImportsStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _importsKeyword, 
				1 => _importsClauses, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ImportsStatementSyntax(base.Kind, newErrors, GetAnnotations(), _importsKeyword, _importsClauses);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ImportsStatementSyntax(base.Kind, GetDiagnostics(), annotations, _importsKeyword, _importsClauses);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitImportsStatement(this);
		}
	}
}
