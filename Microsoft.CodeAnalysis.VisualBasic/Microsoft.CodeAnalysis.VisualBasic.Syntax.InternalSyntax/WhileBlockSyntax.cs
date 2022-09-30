using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class WhileBlockSyntax : ExecutableStatementSyntax
	{
		internal readonly WhileStatementSyntax _whileStatement;

		internal readonly GreenNode _statements;

		internal readonly EndBlockStatementSyntax _endWhileStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal WhileStatementSyntax WhileStatement => _whileStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal EndBlockStatementSyntax EndWhileStatement => _endWhileStatement;

		internal WhileBlockSyntax(SyntaxKind kind, WhileStatementSyntax whileStatement, GreenNode statements, EndBlockStatementSyntax endWhileStatement)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(whileStatement);
			_whileStatement = whileStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endWhileStatement);
			_endWhileStatement = endWhileStatement;
		}

		internal WhileBlockSyntax(SyntaxKind kind, WhileStatementSyntax whileStatement, GreenNode statements, EndBlockStatementSyntax endWhileStatement, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(whileStatement);
			_whileStatement = whileStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endWhileStatement);
			_endWhileStatement = endWhileStatement;
		}

		internal WhileBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, WhileStatementSyntax whileStatement, GreenNode statements, EndBlockStatementSyntax endWhileStatement)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(whileStatement);
			_whileStatement = whileStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endWhileStatement);
			_endWhileStatement = endWhileStatement;
		}

		internal WhileBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			WhileStatementSyntax whileStatementSyntax = (WhileStatementSyntax)reader.ReadValue();
			if (whileStatementSyntax != null)
			{
				AdjustFlagsAndWidth(whileStatementSyntax);
				_whileStatement = whileStatementSyntax;
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
				_endWhileStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_whileStatement);
			writer.WriteValue(_statements);
			writer.WriteValue(_endWhileStatement);
		}

		static WhileBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new WhileBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(WhileBlockSyntax), (ObjectReader r) => new WhileBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WhileBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _whileStatement, 
				1 => _statements, 
				2 => _endWhileStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new WhileBlockSyntax(base.Kind, newErrors, GetAnnotations(), _whileStatement, _statements, _endWhileStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new WhileBlockSyntax(base.Kind, GetDiagnostics(), annotations, _whileStatement, _statements, _endWhileStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitWhileBlock(this);
		}
	}
}
