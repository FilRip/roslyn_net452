using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ElseBlockSyntax : VisualBasicSyntaxNode
	{
		internal readonly ElseStatementSyntax _elseStatement;

		internal readonly GreenNode _statements;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ElseStatementSyntax ElseStatement => _elseStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal ElseBlockSyntax(SyntaxKind kind, ElseStatementSyntax elseStatement, GreenNode statements)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(elseStatement);
			_elseStatement = elseStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal ElseBlockSyntax(SyntaxKind kind, ElseStatementSyntax elseStatement, GreenNode statements, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(elseStatement);
			_elseStatement = elseStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal ElseBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ElseStatementSyntax elseStatement, GreenNode statements)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(elseStatement);
			_elseStatement = elseStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal ElseBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			ElseStatementSyntax elseStatementSyntax = (ElseStatementSyntax)reader.ReadValue();
			if (elseStatementSyntax != null)
			{
				AdjustFlagsAndWidth(elseStatementSyntax);
				_elseStatement = elseStatementSyntax;
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
			writer.WriteValue(_elseStatement);
			writer.WriteValue(_statements);
		}

		static ElseBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new ElseBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ElseBlockSyntax), (ObjectReader r) => new ElseBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _elseStatement, 
				1 => _statements, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ElseBlockSyntax(base.Kind, newErrors, GetAnnotations(), _elseStatement, _statements);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ElseBlockSyntax(base.Kind, GetDiagnostics(), annotations, _elseStatement, _statements);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitElseBlock(this);
		}
	}
}
