using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SyncLockBlockSyntax : ExecutableStatementSyntax
	{
		internal readonly SyncLockStatementSyntax _syncLockStatement;

		internal readonly GreenNode _statements;

		internal readonly EndBlockStatementSyntax _endSyncLockStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal SyncLockStatementSyntax SyncLockStatement => _syncLockStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal EndBlockStatementSyntax EndSyncLockStatement => _endSyncLockStatement;

		internal SyncLockBlockSyntax(SyntaxKind kind, SyncLockStatementSyntax syncLockStatement, GreenNode statements, EndBlockStatementSyntax endSyncLockStatement)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(syncLockStatement);
			_syncLockStatement = syncLockStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endSyncLockStatement);
			_endSyncLockStatement = endSyncLockStatement;
		}

		internal SyncLockBlockSyntax(SyntaxKind kind, SyncLockStatementSyntax syncLockStatement, GreenNode statements, EndBlockStatementSyntax endSyncLockStatement, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(syncLockStatement);
			_syncLockStatement = syncLockStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endSyncLockStatement);
			_endSyncLockStatement = endSyncLockStatement;
		}

		internal SyncLockBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyncLockStatementSyntax syncLockStatement, GreenNode statements, EndBlockStatementSyntax endSyncLockStatement)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(syncLockStatement);
			_syncLockStatement = syncLockStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endSyncLockStatement);
			_endSyncLockStatement = endSyncLockStatement;
		}

		internal SyncLockBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			SyncLockStatementSyntax syncLockStatementSyntax = (SyncLockStatementSyntax)reader.ReadValue();
			if (syncLockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(syncLockStatementSyntax);
				_syncLockStatement = syncLockStatementSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_statements = greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endSyncLockStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_syncLockStatement);
			writer.WriteValue(_statements);
			writer.WriteValue(_endSyncLockStatement);
		}

		static SyncLockBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new SyncLockBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SyncLockBlockSyntax), (ObjectReader r) => new SyncLockBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _syncLockStatement, 
				1 => _statements, 
				2 => _endSyncLockStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SyncLockBlockSyntax(base.Kind, newErrors, GetAnnotations(), _syncLockStatement, _statements, _endSyncLockStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SyncLockBlockSyntax(base.Kind, GetDiagnostics(), annotations, _syncLockStatement, _statements, _endSyncLockStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSyncLockBlock(this);
		}
	}
}
