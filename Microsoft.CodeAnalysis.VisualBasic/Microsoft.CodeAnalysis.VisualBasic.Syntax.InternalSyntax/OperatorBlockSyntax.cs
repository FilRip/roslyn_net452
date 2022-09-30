using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class OperatorBlockSyntax : MethodBlockBaseSyntax
	{
		internal readonly OperatorStatementSyntax _operatorStatement;

		internal readonly EndBlockStatementSyntax _endOperatorStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal OperatorStatementSyntax OperatorStatement => _operatorStatement;

		internal EndBlockStatementSyntax EndOperatorStatement => _endOperatorStatement;

		public override MethodBaseSyntax Begin => OperatorStatement;

		public override EndBlockStatementSyntax End => EndOperatorStatement;

		internal OperatorBlockSyntax(SyntaxKind kind, OperatorStatementSyntax operatorStatement, GreenNode statements, EndBlockStatementSyntax endOperatorStatement)
			: base(kind, statements)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(operatorStatement);
			_operatorStatement = operatorStatement;
			AdjustFlagsAndWidth(endOperatorStatement);
			_endOperatorStatement = endOperatorStatement;
		}

		internal OperatorBlockSyntax(SyntaxKind kind, OperatorStatementSyntax operatorStatement, GreenNode statements, EndBlockStatementSyntax endOperatorStatement, ISyntaxFactoryContext context)
			: base(kind, statements)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(operatorStatement);
			_operatorStatement = operatorStatement;
			AdjustFlagsAndWidth(endOperatorStatement);
			_endOperatorStatement = endOperatorStatement;
		}

		internal OperatorBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, OperatorStatementSyntax operatorStatement, GreenNode statements, EndBlockStatementSyntax endOperatorStatement)
			: base(kind, errors, annotations, statements)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(operatorStatement);
			_operatorStatement = operatorStatement;
			AdjustFlagsAndWidth(endOperatorStatement);
			_endOperatorStatement = endOperatorStatement;
		}

		internal OperatorBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			OperatorStatementSyntax operatorStatementSyntax = (OperatorStatementSyntax)reader.ReadValue();
			if (operatorStatementSyntax != null)
			{
				AdjustFlagsAndWidth(operatorStatementSyntax);
				_operatorStatement = operatorStatementSyntax;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endOperatorStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_operatorStatement);
			writer.WriteValue(_endOperatorStatement);
		}

		static OperatorBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new OperatorBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(OperatorBlockSyntax), (ObjectReader r) => new OperatorBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _operatorStatement, 
				1 => _statements, 
				2 => _endOperatorStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new OperatorBlockSyntax(base.Kind, newErrors, GetAnnotations(), _operatorStatement, _statements, _endOperatorStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new OperatorBlockSyntax(base.Kind, GetDiagnostics(), annotations, _operatorStatement, _statements, _endOperatorStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitOperatorBlock(this);
		}
	}
}
