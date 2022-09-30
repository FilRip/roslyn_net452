using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class MultiLineLambdaExpressionSyntax : LambdaExpressionSyntax
	{
		internal readonly GreenNode _statements;

		internal readonly EndBlockStatementSyntax _endSubOrFunctionStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_statements);

		internal EndBlockStatementSyntax EndSubOrFunctionStatement => _endSubOrFunctionStatement;

		internal MultiLineLambdaExpressionSyntax(SyntaxKind kind, LambdaHeaderSyntax subOrFunctionHeader, GreenNode statements, EndBlockStatementSyntax endSubOrFunctionStatement)
			: base(kind, subOrFunctionHeader)
		{
			base._slotCount = 3;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endSubOrFunctionStatement);
			_endSubOrFunctionStatement = endSubOrFunctionStatement;
		}

		internal MultiLineLambdaExpressionSyntax(SyntaxKind kind, LambdaHeaderSyntax subOrFunctionHeader, GreenNode statements, EndBlockStatementSyntax endSubOrFunctionStatement, ISyntaxFactoryContext context)
			: base(kind, subOrFunctionHeader)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endSubOrFunctionStatement);
			_endSubOrFunctionStatement = endSubOrFunctionStatement;
		}

		internal MultiLineLambdaExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, LambdaHeaderSyntax subOrFunctionHeader, GreenNode statements, EndBlockStatementSyntax endSubOrFunctionStatement)
			: base(kind, errors, annotations, subOrFunctionHeader)
		{
			base._slotCount = 3;
			if (statements != null)
			{
				AdjustFlagsAndWidth(statements);
				_statements = statements;
			}
			AdjustFlagsAndWidth(endSubOrFunctionStatement);
			_endSubOrFunctionStatement = endSubOrFunctionStatement;
		}

		internal MultiLineLambdaExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
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
				_endSubOrFunctionStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_statements);
			writer.WriteValue(_endSubOrFunctionStatement);
		}

		static MultiLineLambdaExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new MultiLineLambdaExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(MultiLineLambdaExpressionSyntax), (ObjectReader r) => new MultiLineLambdaExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _subOrFunctionHeader, 
				1 => _statements, 
				2 => _endSubOrFunctionStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new MultiLineLambdaExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _subOrFunctionHeader, _statements, _endSubOrFunctionStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new MultiLineLambdaExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _subOrFunctionHeader, _statements, _endSubOrFunctionStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitMultiLineLambdaExpression(this);
		}
	}
}
