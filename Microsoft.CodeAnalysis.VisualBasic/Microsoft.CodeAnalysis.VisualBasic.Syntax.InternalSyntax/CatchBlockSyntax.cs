using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CatchBlockSyntax : VisualBasicSyntaxNode
	{
		internal readonly CatchStatementSyntax _catchStatement;

		internal readonly GreenNode _statements;

		internal static Func<ObjectReader, object> CreateInstance;

		internal CatchStatementSyntax CatchStatement => _catchStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal CatchBlockSyntax(SyntaxKind kind, CatchStatementSyntax catchStatement, GreenNode statements)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(catchStatement);
			_catchStatement = catchStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal CatchBlockSyntax(SyntaxKind kind, CatchStatementSyntax catchStatement, GreenNode statements, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(catchStatement);
			_catchStatement = catchStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal CatchBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, CatchStatementSyntax catchStatement, GreenNode statements)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(catchStatement);
			_catchStatement = catchStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal CatchBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			CatchStatementSyntax catchStatementSyntax = (CatchStatementSyntax)reader.ReadValue();
			if (catchStatementSyntax != null)
			{
				AdjustFlagsAndWidth(catchStatementSyntax);
				_catchStatement = catchStatementSyntax;
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
			writer.WriteValue(_catchStatement);
			writer.WriteValue(_statements);
		}

		static CatchBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new CatchBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CatchBlockSyntax), (ObjectReader r) => new CatchBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _catchStatement, 
				1 => _statements, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CatchBlockSyntax(base.Kind, newErrors, GetAnnotations(), _catchStatement, _statements);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CatchBlockSyntax(base.Kind, GetDiagnostics(), annotations, _catchStatement, _statements);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCatchBlock(this);
		}
	}
}
