using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class ForOrForEachBlockSyntax : ExecutableStatementSyntax
	{
		internal readonly GreenNode _statements;

		internal readonly NextStatementSyntax _nextStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal NextStatementSyntax NextStatement => _nextStatement;

		internal ForOrForEachBlockSyntax(SyntaxKind kind, GreenNode statements, NextStatementSyntax nextStatement)
			: base(kind)
		{
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			if (nextStatement != null)
			{
				AdjustFlagsAndWidth(nextStatement);
				_nextStatement = nextStatement;
			}
		}

		internal ForOrForEachBlockSyntax(SyntaxKind kind, GreenNode statements, NextStatementSyntax nextStatement, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			if (nextStatement != null)
			{
				AdjustFlagsAndWidth(nextStatement);
				_nextStatement = nextStatement;
			}
		}

		internal ForOrForEachBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode statements, NextStatementSyntax nextStatement)
			: base(kind, errors, annotations)
		{
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			if (nextStatement != null)
			{
				AdjustFlagsAndWidth(nextStatement);
				_nextStatement = nextStatement;
			}
		}

		internal ForOrForEachBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_statements = greenNode;
			}
			NextStatementSyntax nextStatementSyntax = (NextStatementSyntax)reader.ReadValue();
			if (nextStatementSyntax != null)
			{
				AdjustFlagsAndWidth(nextStatementSyntax);
				_nextStatement = nextStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_statements);
			writer.WriteValue(_nextStatement);
		}
	}
}
