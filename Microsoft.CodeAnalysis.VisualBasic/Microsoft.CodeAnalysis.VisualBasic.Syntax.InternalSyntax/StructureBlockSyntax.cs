using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class StructureBlockSyntax : TypeBlockSyntax
	{
		internal readonly StructureStatementSyntax _structureStatement;

		internal readonly EndBlockStatementSyntax _endStructureStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal StructureStatementSyntax StructureStatement => _structureStatement;

		internal EndBlockStatementSyntax EndStructureStatement => _endStructureStatement;

		public override TypeStatementSyntax BlockStatement => StructureStatement;

		public override EndBlockStatementSyntax EndBlockStatement => EndStructureStatement;

		internal StructureBlockSyntax(SyntaxKind kind, StructureStatementSyntax structureStatement, GreenNode inherits, GreenNode implements, GreenNode members, EndBlockStatementSyntax endStructureStatement)
			: base(kind, inherits, implements, members)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(structureStatement);
			_structureStatement = structureStatement;
			AdjustFlagsAndWidth(endStructureStatement);
			_endStructureStatement = endStructureStatement;
		}

		internal StructureBlockSyntax(SyntaxKind kind, StructureStatementSyntax structureStatement, GreenNode inherits, GreenNode implements, GreenNode members, EndBlockStatementSyntax endStructureStatement, ISyntaxFactoryContext context)
			: base(kind, inherits, implements, members)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(structureStatement);
			_structureStatement = structureStatement;
			AdjustFlagsAndWidth(endStructureStatement);
			_endStructureStatement = endStructureStatement;
		}

		internal StructureBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, StructureStatementSyntax structureStatement, GreenNode inherits, GreenNode implements, GreenNode members, EndBlockStatementSyntax endStructureStatement)
			: base(kind, errors, annotations, inherits, implements, members)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(structureStatement);
			_structureStatement = structureStatement;
			AdjustFlagsAndWidth(endStructureStatement);
			_endStructureStatement = endStructureStatement;
		}

		internal StructureBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			StructureStatementSyntax structureStatementSyntax = (StructureStatementSyntax)reader.ReadValue();
			if (structureStatementSyntax != null)
			{
				AdjustFlagsAndWidth(structureStatementSyntax);
				_structureStatement = structureStatementSyntax;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endStructureStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_structureStatement);
			writer.WriteValue(_endStructureStatement);
		}

		static StructureBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new StructureBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(StructureBlockSyntax), (ObjectReader r) => new StructureBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _structureStatement, 
				1 => _inherits, 
				2 => _implements, 
				3 => _members, 
				4 => _endStructureStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new StructureBlockSyntax(base.Kind, newErrors, GetAnnotations(), _structureStatement, _inherits, _implements, _members, _endStructureStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new StructureBlockSyntax(base.Kind, GetDiagnostics(), annotations, _structureStatement, _inherits, _implements, _members, _endStructureStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitStructureBlock(this);
		}
	}
}
