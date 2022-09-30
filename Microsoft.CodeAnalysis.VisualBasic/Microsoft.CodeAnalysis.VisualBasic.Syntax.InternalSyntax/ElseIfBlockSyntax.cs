using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ElseIfBlockSyntax : VisualBasicSyntaxNode
	{
		internal readonly ElseIfStatementSyntax _elseIfStatement;

		internal readonly GreenNode _statements;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ElseIfStatementSyntax ElseIfStatement => _elseIfStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal ElseIfBlockSyntax(SyntaxKind kind, ElseIfStatementSyntax elseIfStatement, GreenNode statements)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(elseIfStatement);
			_elseIfStatement = elseIfStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal ElseIfBlockSyntax(SyntaxKind kind, ElseIfStatementSyntax elseIfStatement, GreenNode statements, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(elseIfStatement);
			_elseIfStatement = elseIfStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal ElseIfBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ElseIfStatementSyntax elseIfStatement, GreenNode statements)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(elseIfStatement);
			_elseIfStatement = elseIfStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal ElseIfBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			ElseIfStatementSyntax elseIfStatementSyntax = (ElseIfStatementSyntax)reader.ReadValue();
			if (elseIfStatementSyntax != null)
			{
				AdjustFlagsAndWidth(elseIfStatementSyntax);
				_elseIfStatement = elseIfStatementSyntax;
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
			writer.WriteValue(_elseIfStatement);
			writer.WriteValue(_statements);
		}

		static ElseIfBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new ElseIfBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ElseIfBlockSyntax), (ObjectReader r) => new ElseIfBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _elseIfStatement, 
				1 => _statements, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ElseIfBlockSyntax(base.Kind, newErrors, GetAnnotations(), _elseIfStatement, _statements);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ElseIfBlockSyntax(base.Kind, GetDiagnostics(), annotations, _elseIfStatement, _statements);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitElseIfBlock(this);
		}
	}
}
