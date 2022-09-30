using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EnumBlockSyntax : DeclarationStatementSyntax
	{
		internal readonly EnumStatementSyntax _enumStatement;

		internal readonly GreenNode _members;

		internal readonly EndBlockStatementSyntax _endEnumStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal EnumStatementSyntax EnumStatement => _enumStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Members => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_members);

		internal EndBlockStatementSyntax EndEnumStatement => _endEnumStatement;

		internal EnumBlockSyntax(SyntaxKind kind, EnumStatementSyntax enumStatement, GreenNode members, EndBlockStatementSyntax endEnumStatement)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(enumStatement);
			_enumStatement = enumStatement;
			if (members != null)
			{
				AdjustFlagsAndWidth(members);
				_members = members;
			}
			AdjustFlagsAndWidth(endEnumStatement);
			_endEnumStatement = endEnumStatement;
		}

		internal EnumBlockSyntax(SyntaxKind kind, EnumStatementSyntax enumStatement, GreenNode members, EndBlockStatementSyntax endEnumStatement, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(enumStatement);
			_enumStatement = enumStatement;
			if (members != null)
			{
				AdjustFlagsAndWidth(members);
				_members = members;
			}
			AdjustFlagsAndWidth(endEnumStatement);
			_endEnumStatement = endEnumStatement;
		}

		internal EnumBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, EnumStatementSyntax enumStatement, GreenNode members, EndBlockStatementSyntax endEnumStatement)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(enumStatement);
			_enumStatement = enumStatement;
			if (members != null)
			{
				AdjustFlagsAndWidth(members);
				_members = members;
			}
			AdjustFlagsAndWidth(endEnumStatement);
			_endEnumStatement = endEnumStatement;
		}

		internal EnumBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			EnumStatementSyntax enumStatementSyntax = (EnumStatementSyntax)reader.ReadValue();
			if (enumStatementSyntax != null)
			{
				AdjustFlagsAndWidth(enumStatementSyntax);
				_enumStatement = enumStatementSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_members = greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endEnumStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_enumStatement);
			writer.WriteValue(_members);
			writer.WriteValue(_endEnumStatement);
		}

		static EnumBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new EnumBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(EnumBlockSyntax), (ObjectReader r) => new EnumBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _enumStatement, 
				1 => _members, 
				2 => _endEnumStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new EnumBlockSyntax(base.Kind, newErrors, GetAnnotations(), _enumStatement, _members, _endEnumStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new EnumBlockSyntax(base.Kind, GetDiagnostics(), annotations, _enumStatement, _members, _endEnumStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitEnumBlock(this);
		}
	}
}
