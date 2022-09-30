using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class FinallyBlockSyntax : VisualBasicSyntaxNode
	{
		internal readonly FinallyStatementSyntax _finallyStatement;

		internal readonly GreenNode _statements;

		internal static Func<ObjectReader, object> CreateInstance;

		internal FinallyStatementSyntax FinallyStatement => _finallyStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal FinallyBlockSyntax(SyntaxKind kind, FinallyStatementSyntax finallyStatement, GreenNode statements)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(finallyStatement);
			_finallyStatement = finallyStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal FinallyBlockSyntax(SyntaxKind kind, FinallyStatementSyntax finallyStatement, GreenNode statements, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(finallyStatement);
			_finallyStatement = finallyStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal FinallyBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, FinallyStatementSyntax finallyStatement, GreenNode statements)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(finallyStatement);
			_finallyStatement = finallyStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
		}

		internal FinallyBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			FinallyStatementSyntax finallyStatementSyntax = (FinallyStatementSyntax)reader.ReadValue();
			if (finallyStatementSyntax != null)
			{
				AdjustFlagsAndWidth(finallyStatementSyntax);
				_finallyStatement = finallyStatementSyntax;
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
			writer.WriteValue(_finallyStatement);
			writer.WriteValue(_statements);
		}

		static FinallyBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new FinallyBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(FinallyBlockSyntax), (ObjectReader r) => new FinallyBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _finallyStatement, 
				1 => _statements, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new FinallyBlockSyntax(base.Kind, newErrors, GetAnnotations(), _finallyStatement, _statements);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new FinallyBlockSyntax(base.Kind, GetDiagnostics(), annotations, _finallyStatement, _statements);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitFinallyBlock(this);
		}
	}
}
