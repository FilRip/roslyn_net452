using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ReDimStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _reDimKeyword;

		internal readonly KeywordSyntax _preserveKeyword;

		internal readonly GreenNode _clauses;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ReDimKeyword => _reDimKeyword;

		internal KeywordSyntax PreserveKeyword => _preserveKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<RedimClauseSyntax> Clauses => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<RedimClauseSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<RedimClauseSyntax>(_clauses));

		internal ReDimStatementSyntax(SyntaxKind kind, KeywordSyntax reDimKeyword, KeywordSyntax preserveKeyword, GreenNode clauses)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(reDimKeyword);
			_reDimKeyword = reDimKeyword;
			if (preserveKeyword != null)
			{
				AdjustFlagsAndWidth(preserveKeyword);
				_preserveKeyword = preserveKeyword;
			}
			if (clauses != null)
			{
				AdjustFlagsAndWidth(clauses);
				_clauses = clauses;
			}
		}

		internal ReDimStatementSyntax(SyntaxKind kind, KeywordSyntax reDimKeyword, KeywordSyntax preserveKeyword, GreenNode clauses, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(reDimKeyword);
			_reDimKeyword = reDimKeyword;
			if (preserveKeyword != null)
			{
				AdjustFlagsAndWidth(preserveKeyword);
				_preserveKeyword = preserveKeyword;
			}
			if (clauses != null)
			{
				AdjustFlagsAndWidth(clauses);
				_clauses = clauses;
			}
		}

		internal ReDimStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax reDimKeyword, KeywordSyntax preserveKeyword, GreenNode clauses)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(reDimKeyword);
			_reDimKeyword = reDimKeyword;
			if (preserveKeyword != null)
			{
				AdjustFlagsAndWidth(preserveKeyword);
				_preserveKeyword = preserveKeyword;
			}
			if (clauses != null)
			{
				AdjustFlagsAndWidth(clauses);
				_clauses = clauses;
			}
		}

		internal ReDimStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_reDimKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_preserveKeyword = keywordSyntax2;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_clauses = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_reDimKeyword);
			writer.WriteValue(_preserveKeyword);
			writer.WriteValue(_clauses);
		}

		static ReDimStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ReDimStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ReDimStatementSyntax), (ObjectReader r) => new ReDimStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ReDimStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _reDimKeyword, 
				1 => _preserveKeyword, 
				2 => _clauses, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ReDimStatementSyntax(base.Kind, newErrors, GetAnnotations(), _reDimKeyword, _preserveKeyword, _clauses);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ReDimStatementSyntax(base.Kind, GetDiagnostics(), annotations, _reDimKeyword, _preserveKeyword, _clauses);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitReDimStatement(this);
		}
	}
}
