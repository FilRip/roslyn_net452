using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CaseBlockSyntax : VisualBasicSyntaxNode
	{
		internal readonly CaseStatementSyntax _caseStatement;

		internal readonly GreenNode _statements;

		internal static Func<ObjectReader, object> CreateInstance;

		internal CaseStatementSyntax CaseStatement => _caseStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal CaseBlockSyntax(SyntaxKind kind, CaseStatementSyntax caseStatement, GreenNode statements)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(caseStatement);
			_caseStatement = caseStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal CaseBlockSyntax(SyntaxKind kind, CaseStatementSyntax caseStatement, GreenNode statements, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(caseStatement);
			_caseStatement = caseStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal CaseBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, CaseStatementSyntax caseStatement, GreenNode statements)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(caseStatement);
			_caseStatement = caseStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal CaseBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			CaseStatementSyntax caseStatementSyntax = (CaseStatementSyntax)reader.ReadValue();
			if (caseStatementSyntax != null)
			{
				AdjustFlagsAndWidth(caseStatementSyntax);
				_caseStatement = caseStatementSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_statements = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_caseStatement);
			writer.WriteValue(_statements);
		}

		static CaseBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new CaseBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CaseBlockSyntax), (ObjectReader r) => new CaseBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CaseBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _caseStatement, 
				1 => _statements, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CaseBlockSyntax(base.Kind, newErrors, GetAnnotations(), _caseStatement, _statements);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CaseBlockSyntax(base.Kind, GetDiagnostics(), annotations, _caseStatement, _statements);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCaseBlock(this);
		}
	}
}
