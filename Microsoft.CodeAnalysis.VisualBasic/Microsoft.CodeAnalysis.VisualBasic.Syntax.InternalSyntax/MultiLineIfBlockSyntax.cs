using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class MultiLineIfBlockSyntax : ExecutableStatementSyntax
	{
		internal readonly IfStatementSyntax _ifStatement;

		internal readonly GreenNode _statements;

		internal readonly GreenNode _elseIfBlocks;

		internal readonly ElseBlockSyntax _elseBlock;

		internal readonly EndBlockStatementSyntax _endIfStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal IfStatementSyntax IfStatement => _ifStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ElseIfBlockSyntax> ElseIfBlocks => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ElseIfBlockSyntax>(_elseIfBlocks);

		internal ElseBlockSyntax ElseBlock => _elseBlock;

		internal EndBlockStatementSyntax EndIfStatement => _endIfStatement;

		internal MultiLineIfBlockSyntax(SyntaxKind kind, IfStatementSyntax ifStatement, GreenNode statements, GreenNode elseIfBlocks, ElseBlockSyntax elseBlock, EndBlockStatementSyntax endIfStatement)
			: base(kind)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(ifStatement);
			_ifStatement = ifStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			if (elseIfBlocks != null)
			{
				AdjustFlagsAndWidth(elseIfBlocks);
				_elseIfBlocks = elseIfBlocks;
			}
			if (elseBlock != null)
			{
				AdjustFlagsAndWidth(elseBlock);
				_elseBlock = elseBlock;
			}
			AdjustFlagsAndWidth(endIfStatement);
			_endIfStatement = endIfStatement;
		}

		internal MultiLineIfBlockSyntax(SyntaxKind kind, IfStatementSyntax ifStatement, GreenNode statements, GreenNode elseIfBlocks, ElseBlockSyntax elseBlock, EndBlockStatementSyntax endIfStatement, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(ifStatement);
			_ifStatement = ifStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			if (elseIfBlocks != null)
			{
				AdjustFlagsAndWidth(elseIfBlocks);
				_elseIfBlocks = elseIfBlocks;
			}
			if (elseBlock != null)
			{
				AdjustFlagsAndWidth(elseBlock);
				_elseBlock = elseBlock;
			}
			AdjustFlagsAndWidth(endIfStatement);
			_endIfStatement = endIfStatement;
		}

		internal MultiLineIfBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IfStatementSyntax ifStatement, GreenNode statements, GreenNode elseIfBlocks, ElseBlockSyntax elseBlock, EndBlockStatementSyntax endIfStatement)
			: base(kind, errors, annotations)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(ifStatement);
			_ifStatement = ifStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			if (elseIfBlocks != null)
			{
				AdjustFlagsAndWidth(elseIfBlocks);
				_elseIfBlocks = elseIfBlocks;
			}
			if (elseBlock != null)
			{
				AdjustFlagsAndWidth(elseBlock);
				_elseBlock = elseBlock;
			}
			AdjustFlagsAndWidth(endIfStatement);
			_endIfStatement = endIfStatement;
		}

		internal MultiLineIfBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			IfStatementSyntax ifStatementSyntax = (IfStatementSyntax)reader.ReadValue();
			if (ifStatementSyntax != null)
			{
				AdjustFlagsAndWidth(ifStatementSyntax);
				_ifStatement = ifStatementSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_statements = greenNode;
			}
			GreenNode greenNode2 = (GreenNode)reader.ReadValue();
			if (greenNode2 != null)
			{
				AdjustFlagsAndWidth(greenNode2);
				_elseIfBlocks = greenNode2;
			}
			ElseBlockSyntax elseBlockSyntax = (ElseBlockSyntax)reader.ReadValue();
			if (elseBlockSyntax != null)
			{
				AdjustFlagsAndWidth(elseBlockSyntax);
				_elseBlock = elseBlockSyntax;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endIfStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_ifStatement);
			writer.WriteValue(_statements);
			writer.WriteValue(_elseIfBlocks);
			writer.WriteValue(_elseBlock);
			writer.WriteValue(_endIfStatement);
		}

		static MultiLineIfBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new MultiLineIfBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(MultiLineIfBlockSyntax), (ObjectReader r) => new MultiLineIfBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _ifStatement, 
				1 => _statements, 
				2 => _elseIfBlocks, 
				3 => _elseBlock, 
				4 => _endIfStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new MultiLineIfBlockSyntax(base.Kind, newErrors, GetAnnotations(), _ifStatement, _statements, _elseIfBlocks, _elseBlock, _endIfStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new MultiLineIfBlockSyntax(base.Kind, GetDiagnostics(), annotations, _ifStatement, _statements, _elseIfBlocks, _elseBlock, _endIfStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitMultiLineIfBlock(this);
		}
	}
}
