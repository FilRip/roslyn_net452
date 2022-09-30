using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class InterfaceBlockSyntax : TypeBlockSyntax
	{
		internal readonly InterfaceStatementSyntax _interfaceStatement;

		internal readonly EndBlockStatementSyntax _endInterfaceStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal InterfaceStatementSyntax InterfaceStatement => _interfaceStatement;

		internal EndBlockStatementSyntax EndInterfaceStatement => _endInterfaceStatement;

		public override TypeStatementSyntax BlockStatement => InterfaceStatement;

		public override EndBlockStatementSyntax EndBlockStatement => EndInterfaceStatement;

		internal InterfaceBlockSyntax(SyntaxKind kind, InterfaceStatementSyntax interfaceStatement, GreenNode inherits, GreenNode implements, GreenNode members, EndBlockStatementSyntax endInterfaceStatement)
			: base(kind, inherits, implements, members)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(interfaceStatement);
			_interfaceStatement = interfaceStatement;
			AdjustFlagsAndWidth(endInterfaceStatement);
			_endInterfaceStatement = endInterfaceStatement;
		}

		internal InterfaceBlockSyntax(SyntaxKind kind, InterfaceStatementSyntax interfaceStatement, GreenNode inherits, GreenNode implements, GreenNode members, EndBlockStatementSyntax endInterfaceStatement, ISyntaxFactoryContext context)
			: base(kind, inherits, implements, members)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(interfaceStatement);
			_interfaceStatement = interfaceStatement;
			AdjustFlagsAndWidth(endInterfaceStatement);
			_endInterfaceStatement = endInterfaceStatement;
		}

		internal InterfaceBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, InterfaceStatementSyntax interfaceStatement, GreenNode inherits, GreenNode implements, GreenNode members, EndBlockStatementSyntax endInterfaceStatement)
			: base(kind, errors, annotations, inherits, implements, members)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(interfaceStatement);
			_interfaceStatement = interfaceStatement;
			AdjustFlagsAndWidth(endInterfaceStatement);
			_endInterfaceStatement = endInterfaceStatement;
		}

		internal InterfaceBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			InterfaceStatementSyntax interfaceStatementSyntax = (InterfaceStatementSyntax)reader.ReadValue();
			if (interfaceStatementSyntax != null)
			{
				AdjustFlagsAndWidth(interfaceStatementSyntax);
				_interfaceStatement = interfaceStatementSyntax;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endInterfaceStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_interfaceStatement);
			writer.WriteValue(_endInterfaceStatement);
		}

		static InterfaceBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new InterfaceBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(InterfaceBlockSyntax), (ObjectReader r) => new InterfaceBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _interfaceStatement, 
				1 => _inherits, 
				2 => _implements, 
				3 => _members, 
				4 => _endInterfaceStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new InterfaceBlockSyntax(base.Kind, newErrors, GetAnnotations(), _interfaceStatement, _inherits, _implements, _members, _endInterfaceStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new InterfaceBlockSyntax(base.Kind, GetDiagnostics(), annotations, _interfaceStatement, _inherits, _implements, _members, _endInterfaceStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitInterfaceBlock(this);
		}
	}
}
