using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class AccessorBlockSyntax : MethodBlockBaseSyntax
	{
		internal readonly AccessorStatementSyntax _accessorStatement;

		internal readonly EndBlockStatementSyntax _endAccessorStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal AccessorStatementSyntax AccessorStatement => _accessorStatement;

		internal EndBlockStatementSyntax EndAccessorStatement => _endAccessorStatement;

		public override MethodBaseSyntax Begin => AccessorStatement;

		public override EndBlockStatementSyntax End => EndAccessorStatement;

		internal AccessorBlockSyntax(SyntaxKind kind, AccessorStatementSyntax accessorStatement, GreenNode statements, EndBlockStatementSyntax endAccessorStatement)
			: base(kind, statements)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(accessorStatement);
			_accessorStatement = accessorStatement;
			AdjustFlagsAndWidth(endAccessorStatement);
			_endAccessorStatement = endAccessorStatement;
		}

		internal AccessorBlockSyntax(SyntaxKind kind, AccessorStatementSyntax accessorStatement, GreenNode statements, EndBlockStatementSyntax endAccessorStatement, ISyntaxFactoryContext context)
			: base(kind, statements)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(accessorStatement);
			_accessorStatement = accessorStatement;
			AdjustFlagsAndWidth(endAccessorStatement);
			_endAccessorStatement = endAccessorStatement;
		}

		internal AccessorBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, AccessorStatementSyntax accessorStatement, GreenNode statements, EndBlockStatementSyntax endAccessorStatement)
			: base(kind, errors, annotations, statements)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(accessorStatement);
			_accessorStatement = accessorStatement;
			AdjustFlagsAndWidth(endAccessorStatement);
			_endAccessorStatement = endAccessorStatement;
		}

		internal AccessorBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			AccessorStatementSyntax accessorStatementSyntax = (AccessorStatementSyntax)reader.ReadValue();
			if (accessorStatementSyntax != null)
			{
				AdjustFlagsAndWidth(accessorStatementSyntax);
				_accessorStatement = accessorStatementSyntax;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endAccessorStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_accessorStatement);
			writer.WriteValue(_endAccessorStatement);
		}

		static AccessorBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new AccessorBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(AccessorBlockSyntax), (ObjectReader r) => new AccessorBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _accessorStatement, 
				1 => _statements, 
				2 => _endAccessorStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new AccessorBlockSyntax(base.Kind, newErrors, GetAnnotations(), _accessorStatement, _statements, _endAccessorStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new AccessorBlockSyntax(base.Kind, GetDiagnostics(), annotations, _accessorStatement, _statements, _endAccessorStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitAccessorBlock(this);
		}
	}
}
