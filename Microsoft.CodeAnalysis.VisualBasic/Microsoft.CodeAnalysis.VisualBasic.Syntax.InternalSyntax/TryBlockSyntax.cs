using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class TryBlockSyntax : ExecutableStatementSyntax
	{
		internal readonly TryStatementSyntax _tryStatement;

		internal readonly GreenNode _statements;

		internal readonly GreenNode _catchBlocks;

		internal readonly FinallyBlockSyntax _finallyBlock;

		internal readonly EndBlockStatementSyntax _endTryStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal TryStatementSyntax TryStatement => _tryStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CatchBlockSyntax> CatchBlocks => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CatchBlockSyntax>(_catchBlocks);

		internal FinallyBlockSyntax FinallyBlock => _finallyBlock;

		internal EndBlockStatementSyntax EndTryStatement => _endTryStatement;

		internal TryBlockSyntax(SyntaxKind kind, TryStatementSyntax tryStatement, GreenNode statements, GreenNode catchBlocks, FinallyBlockSyntax finallyBlock, EndBlockStatementSyntax endTryStatement)
			: base(kind)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(tryStatement);
			_tryStatement = tryStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			if (catchBlocks != null)
			{
				AdjustFlagsAndWidth(catchBlocks);
				_catchBlocks = catchBlocks;
			}
			if (finallyBlock != null)
			{
				AdjustFlagsAndWidth(finallyBlock);
				_finallyBlock = finallyBlock;
			}
			AdjustFlagsAndWidth(endTryStatement);
			_endTryStatement = endTryStatement;
		}

		internal TryBlockSyntax(SyntaxKind kind, TryStatementSyntax tryStatement, GreenNode statements, GreenNode catchBlocks, FinallyBlockSyntax finallyBlock, EndBlockStatementSyntax endTryStatement, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(tryStatement);
			_tryStatement = tryStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			if (catchBlocks != null)
			{
				AdjustFlagsAndWidth(catchBlocks);
				_catchBlocks = catchBlocks;
			}
			if (finallyBlock != null)
			{
				AdjustFlagsAndWidth(finallyBlock);
				_finallyBlock = finallyBlock;
			}
			AdjustFlagsAndWidth(endTryStatement);
			_endTryStatement = endTryStatement;
		}

		internal TryBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, TryStatementSyntax tryStatement, GreenNode statements, GreenNode catchBlocks, FinallyBlockSyntax finallyBlock, EndBlockStatementSyntax endTryStatement)
			: base(kind, errors, annotations)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(tryStatement);
			_tryStatement = tryStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			if (catchBlocks != null)
			{
				AdjustFlagsAndWidth(catchBlocks);
				_catchBlocks = catchBlocks;
			}
			if (finallyBlock != null)
			{
				AdjustFlagsAndWidth(finallyBlock);
				_finallyBlock = finallyBlock;
			}
			AdjustFlagsAndWidth(endTryStatement);
			_endTryStatement = endTryStatement;
		}

		internal TryBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			TryStatementSyntax tryStatementSyntax = (TryStatementSyntax)reader.ReadValue();
			if (tryStatementSyntax != null)
			{
				AdjustFlagsAndWidth(tryStatementSyntax);
				_tryStatement = tryStatementSyntax;
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
				_catchBlocks = greenNode2;
			}
			FinallyBlockSyntax finallyBlockSyntax = (FinallyBlockSyntax)reader.ReadValue();
			if (finallyBlockSyntax != null)
			{
				AdjustFlagsAndWidth(finallyBlockSyntax);
				_finallyBlock = finallyBlockSyntax;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endTryStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_tryStatement);
			writer.WriteValue(_statements);
			writer.WriteValue(_catchBlocks);
			writer.WriteValue(_finallyBlock);
			writer.WriteValue(_endTryStatement);
		}

		static TryBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new TryBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(TryBlockSyntax), (ObjectReader r) => new TryBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _tryStatement, 
				1 => _statements, 
				2 => _catchBlocks, 
				3 => _finallyBlock, 
				4 => _endTryStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new TryBlockSyntax(base.Kind, newErrors, GetAnnotations(), _tryStatement, _statements, _catchBlocks, _finallyBlock, _endTryStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new TryBlockSyntax(base.Kind, GetDiagnostics(), annotations, _tryStatement, _statements, _catchBlocks, _finallyBlock, _endTryStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitTryBlock(this);
		}
	}
}
