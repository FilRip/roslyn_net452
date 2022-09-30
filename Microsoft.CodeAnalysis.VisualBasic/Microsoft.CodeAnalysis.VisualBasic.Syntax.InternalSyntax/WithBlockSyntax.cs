using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class WithBlockSyntax : ExecutableStatementSyntax
	{
		internal readonly WithStatementSyntax _withStatement;

		internal readonly GreenNode _statements;

		internal readonly EndBlockStatementSyntax _endWithStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal WithStatementSyntax WithStatement => _withStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal EndBlockStatementSyntax EndWithStatement => _endWithStatement;

		internal WithBlockSyntax(SyntaxKind kind, WithStatementSyntax withStatement, GreenNode statements, EndBlockStatementSyntax endWithStatement)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(withStatement);
			_withStatement = withStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endWithStatement);
			_endWithStatement = endWithStatement;
		}

		internal WithBlockSyntax(SyntaxKind kind, WithStatementSyntax withStatement, GreenNode statements, EndBlockStatementSyntax endWithStatement, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(withStatement);
			_withStatement = withStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endWithStatement);
			_endWithStatement = endWithStatement;
		}

		internal WithBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, WithStatementSyntax withStatement, GreenNode statements, EndBlockStatementSyntax endWithStatement)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(withStatement);
			_withStatement = withStatement;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endWithStatement);
			_endWithStatement = endWithStatement;
		}

		internal WithBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			WithStatementSyntax withStatementSyntax = (WithStatementSyntax)reader.ReadValue();
			if (withStatementSyntax != null)
			{
				AdjustFlagsAndWidth(withStatementSyntax);
				_withStatement = withStatementSyntax;
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
				_endWithStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_withStatement);
			writer.WriteValue(_statements);
			writer.WriteValue(_endWithStatement);
		}

		static WithBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new WithBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(WithBlockSyntax), (ObjectReader r) => new WithBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _withStatement, 
				1 => _statements, 
				2 => _endWithStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new WithBlockSyntax(base.Kind, newErrors, GetAnnotations(), _withStatement, _statements, _endWithStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new WithBlockSyntax(base.Kind, GetDiagnostics(), annotations, _withStatement, _statements, _endWithStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitWithBlock(this);
		}
	}
}
