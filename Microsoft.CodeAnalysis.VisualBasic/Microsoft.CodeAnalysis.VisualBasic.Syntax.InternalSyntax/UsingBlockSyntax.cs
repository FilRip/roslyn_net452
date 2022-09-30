using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class UsingBlockSyntax : ExecutableStatementSyntax
	{
		internal readonly UsingStatementSyntax _usingStatement;

		internal readonly GreenNode _statements;

		internal readonly EndBlockStatementSyntax _endUsingStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal UsingStatementSyntax UsingStatement => _usingStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal EndBlockStatementSyntax EndUsingStatement => _endUsingStatement;

		internal UsingBlockSyntax(SyntaxKind kind, UsingStatementSyntax usingStatement, GreenNode statements, EndBlockStatementSyntax endUsingStatement)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(usingStatement);
			_usingStatement = usingStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endUsingStatement);
			_endUsingStatement = endUsingStatement;
		}

		internal UsingBlockSyntax(SyntaxKind kind, UsingStatementSyntax usingStatement, GreenNode statements, EndBlockStatementSyntax endUsingStatement, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(usingStatement);
			_usingStatement = usingStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endUsingStatement);
			_endUsingStatement = endUsingStatement;
		}

		internal UsingBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, UsingStatementSyntax usingStatement, GreenNode statements, EndBlockStatementSyntax endUsingStatement)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(usingStatement);
			_usingStatement = usingStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endUsingStatement);
			_endUsingStatement = endUsingStatement;
		}

		internal UsingBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			UsingStatementSyntax usingStatementSyntax = (UsingStatementSyntax)reader.ReadValue();
			if (usingStatementSyntax != null)
			{
				AdjustFlagsAndWidth(usingStatementSyntax);
				_usingStatement = usingStatementSyntax;
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
				_endUsingStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_usingStatement);
			writer.WriteValue(_statements);
			writer.WriteValue(_endUsingStatement);
		}

		static UsingBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new UsingBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(UsingBlockSyntax), (ObjectReader r) => new UsingBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _usingStatement, 
				1 => _statements, 
				2 => _endUsingStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new UsingBlockSyntax(base.Kind, newErrors, GetAnnotations(), _usingStatement, _statements, _endUsingStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new UsingBlockSyntax(base.Kind, GetDiagnostics(), annotations, _usingStatement, _statements, _endUsingStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitUsingBlock(this);
		}
	}
}
