using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class DoLoopBlockSyntax : ExecutableStatementSyntax
	{
		internal readonly DoStatementSyntax _doStatement;

		internal readonly GreenNode _statements;

		internal readonly LoopStatementSyntax _loopStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal DoStatementSyntax DoStatement => _doStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal LoopStatementSyntax LoopStatement => _loopStatement;

		internal DoLoopBlockSyntax(SyntaxKind kind, DoStatementSyntax doStatement, GreenNode statements, LoopStatementSyntax loopStatement)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(doStatement);
			_doStatement = doStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(loopStatement);
			_loopStatement = loopStatement;
		}

		internal DoLoopBlockSyntax(SyntaxKind kind, DoStatementSyntax doStatement, GreenNode statements, LoopStatementSyntax loopStatement, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(doStatement);
			_doStatement = doStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(loopStatement);
			_loopStatement = loopStatement;
		}

		internal DoLoopBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, DoStatementSyntax doStatement, GreenNode statements, LoopStatementSyntax loopStatement)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(doStatement);
			_doStatement = doStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(loopStatement);
			_loopStatement = loopStatement;
		}

		internal DoLoopBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			DoStatementSyntax doStatementSyntax = (DoStatementSyntax)reader.ReadValue();
			if (doStatementSyntax != null)
			{
				AdjustFlagsAndWidth(doStatementSyntax);
				_doStatement = doStatementSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_statements = greenNode;
			}
			LoopStatementSyntax loopStatementSyntax = (LoopStatementSyntax)reader.ReadValue();
			if (loopStatementSyntax != null)
			{
				AdjustFlagsAndWidth(loopStatementSyntax);
				_loopStatement = loopStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_doStatement);
			writer.WriteValue(_statements);
			writer.WriteValue(_loopStatement);
		}

		static DoLoopBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new DoLoopBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(DoLoopBlockSyntax), (ObjectReader r) => new DoLoopBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _doStatement, 
				1 => _statements, 
				2 => _loopStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new DoLoopBlockSyntax(base.Kind, newErrors, GetAnnotations(), _doStatement, _statements, _loopStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new DoLoopBlockSyntax(base.Kind, GetDiagnostics(), annotations, _doStatement, _statements, _loopStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitDoLoopBlock(this);
		}
	}
}
