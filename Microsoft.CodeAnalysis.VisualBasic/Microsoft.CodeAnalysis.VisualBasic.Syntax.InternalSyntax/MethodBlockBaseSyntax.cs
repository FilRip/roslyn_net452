using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class MethodBlockBaseSyntax : DeclarationStatementSyntax
	{
		internal readonly GreenNode _statements;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		public abstract MethodBaseSyntax Begin { get; }

		public abstract EndBlockStatementSyntax End { get; }

		internal MethodBlockBaseSyntax(SyntaxKind kind, GreenNode statements)
			: base(kind)
		{
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal MethodBlockBaseSyntax(SyntaxKind kind, GreenNode statements, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal MethodBlockBaseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode statements)
			: base(kind, errors, annotations)
		{
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal MethodBlockBaseSyntax(ObjectReader reader)
			: base(reader)
		{
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
			writer.WriteValue(_statements);
		}
	}
}
