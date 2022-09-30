using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SelectBlockSyntax : ExecutableStatementSyntax
	{
		internal readonly SelectStatementSyntax _selectStatement;

		internal readonly GreenNode _caseBlocks;

		internal readonly EndBlockStatementSyntax _endSelectStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal SelectStatementSyntax SelectStatement => _selectStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CaseBlockSyntax> CaseBlocks => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CaseBlockSyntax>(_caseBlocks);

		internal EndBlockStatementSyntax EndSelectStatement => _endSelectStatement;

		internal SelectBlockSyntax(SyntaxKind kind, SelectStatementSyntax selectStatement, GreenNode caseBlocks, EndBlockStatementSyntax endSelectStatement)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(selectStatement);
			_selectStatement = selectStatement;
			if (caseBlocks != null)
			{
				AdjustFlagsAndWidth(caseBlocks);
				_caseBlocks = caseBlocks;
			}
			AdjustFlagsAndWidth(endSelectStatement);
			_endSelectStatement = endSelectStatement;
		}

		internal SelectBlockSyntax(SyntaxKind kind, SelectStatementSyntax selectStatement, GreenNode caseBlocks, EndBlockStatementSyntax endSelectStatement, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(selectStatement);
			_selectStatement = selectStatement;
			if (caseBlocks != null)
			{
				AdjustFlagsAndWidth(caseBlocks);
				_caseBlocks = caseBlocks;
			}
			AdjustFlagsAndWidth(endSelectStatement);
			_endSelectStatement = endSelectStatement;
		}

		internal SelectBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SelectStatementSyntax selectStatement, GreenNode caseBlocks, EndBlockStatementSyntax endSelectStatement)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(selectStatement);
			_selectStatement = selectStatement;
			if (caseBlocks != null)
			{
				AdjustFlagsAndWidth(caseBlocks);
				_caseBlocks = caseBlocks;
			}
			AdjustFlagsAndWidth(endSelectStatement);
			_endSelectStatement = endSelectStatement;
		}

		internal SelectBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			SelectStatementSyntax selectStatementSyntax = (SelectStatementSyntax)reader.ReadValue();
			if (selectStatementSyntax != null)
			{
				AdjustFlagsAndWidth(selectStatementSyntax);
				_selectStatement = selectStatementSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_caseBlocks = greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endSelectStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_selectStatement);
			writer.WriteValue(_caseBlocks);
			writer.WriteValue(_endSelectStatement);
		}

		static SelectBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new SelectBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SelectBlockSyntax), (ObjectReader r) => new SelectBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _selectStatement, 
				1 => _caseBlocks, 
				2 => _endSelectStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SelectBlockSyntax(base.Kind, newErrors, GetAnnotations(), _selectStatement, _caseBlocks, _endSelectStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SelectBlockSyntax(base.Kind, GetDiagnostics(), annotations, _selectStatement, _caseBlocks, _endSelectStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSelectBlock(this);
		}
	}
}
