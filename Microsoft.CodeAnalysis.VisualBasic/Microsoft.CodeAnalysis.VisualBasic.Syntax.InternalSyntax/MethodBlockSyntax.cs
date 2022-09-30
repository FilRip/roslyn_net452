using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class MethodBlockSyntax : MethodBlockBaseSyntax
	{
		internal readonly MethodStatementSyntax _subOrFunctionStatement;

		internal readonly EndBlockStatementSyntax _endSubOrFunctionStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal MethodStatementSyntax SubOrFunctionStatement => _subOrFunctionStatement;

		internal EndBlockStatementSyntax EndSubOrFunctionStatement => _endSubOrFunctionStatement;

		public override MethodBaseSyntax Begin => SubOrFunctionStatement;

		public override EndBlockStatementSyntax End => EndSubOrFunctionStatement;

		internal MethodBlockSyntax(SyntaxKind kind, MethodStatementSyntax subOrFunctionStatement, GreenNode statements, EndBlockStatementSyntax endSubOrFunctionStatement)
			: base(kind, statements)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(subOrFunctionStatement);
			_subOrFunctionStatement = subOrFunctionStatement;
			AdjustFlagsAndWidth(endSubOrFunctionStatement);
			_endSubOrFunctionStatement = endSubOrFunctionStatement;
		}

		internal MethodBlockSyntax(SyntaxKind kind, MethodStatementSyntax subOrFunctionStatement, GreenNode statements, EndBlockStatementSyntax endSubOrFunctionStatement, ISyntaxFactoryContext context)
			: base(kind, statements)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(subOrFunctionStatement);
			_subOrFunctionStatement = subOrFunctionStatement;
			AdjustFlagsAndWidth(endSubOrFunctionStatement);
			_endSubOrFunctionStatement = endSubOrFunctionStatement;
		}

		internal MethodBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, MethodStatementSyntax subOrFunctionStatement, GreenNode statements, EndBlockStatementSyntax endSubOrFunctionStatement)
			: base(kind, errors, annotations, statements)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(subOrFunctionStatement);
			_subOrFunctionStatement = subOrFunctionStatement;
			AdjustFlagsAndWidth(endSubOrFunctionStatement);
			_endSubOrFunctionStatement = endSubOrFunctionStatement;
		}

		internal MethodBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			MethodStatementSyntax methodStatementSyntax = (MethodStatementSyntax)reader.ReadValue();
			if (methodStatementSyntax != null)
			{
				AdjustFlagsAndWidth(methodStatementSyntax);
				_subOrFunctionStatement = methodStatementSyntax;
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
			writer.WriteValue(_subOrFunctionStatement);
			writer.WriteValue(_endSubOrFunctionStatement);
		}

		static MethodBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new MethodBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(MethodBlockSyntax), (ObjectReader r) => new MethodBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _subOrFunctionStatement, 
				1 => _statements, 
				2 => _endSubOrFunctionStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new MethodBlockSyntax(base.Kind, newErrors, GetAnnotations(), _subOrFunctionStatement, _statements, _endSubOrFunctionStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new MethodBlockSyntax(base.Kind, GetDiagnostics(), annotations, _subOrFunctionStatement, _statements, _endSubOrFunctionStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitMethodBlock(this);
		}
	}
}
