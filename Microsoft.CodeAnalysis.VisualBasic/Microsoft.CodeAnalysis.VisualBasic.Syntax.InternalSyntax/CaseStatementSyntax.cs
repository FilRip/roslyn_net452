using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CaseStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _caseKeyword;

		internal readonly GreenNode _cases;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax CaseKeyword => _caseKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CaseClauseSyntax> Cases => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CaseClauseSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CaseClauseSyntax>(_cases));

		internal CaseStatementSyntax(SyntaxKind kind, KeywordSyntax caseKeyword, GreenNode cases)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(caseKeyword);
			_caseKeyword = caseKeyword;
			if (cases != null)
			{
				AdjustFlagsAndWidth(cases);
				_cases = cases;
			}
		}

		internal CaseStatementSyntax(SyntaxKind kind, KeywordSyntax caseKeyword, GreenNode cases, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(caseKeyword);
			_caseKeyword = caseKeyword;
			if (cases != null)
			{
				AdjustFlagsAndWidth(cases);
				_cases = cases;
			}
		}

		internal CaseStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax caseKeyword, GreenNode cases)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(caseKeyword);
			_caseKeyword = caseKeyword;
			if (cases != null)
			{
				AdjustFlagsAndWidth(cases);
				_cases = cases;
			}
		}

		internal CaseStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_caseKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_cases = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_caseKeyword);
			writer.WriteValue(_cases);
		}

		static CaseStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new CaseStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CaseStatementSyntax), (ObjectReader r) => new CaseStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _caseKeyword, 
				1 => _cases, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CaseStatementSyntax(base.Kind, newErrors, GetAnnotations(), _caseKeyword, _cases);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CaseStatementSyntax(base.Kind, GetDiagnostics(), annotations, _caseKeyword, _cases);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCaseStatement(this);
		}
	}
}
