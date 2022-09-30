using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class NamespaceBlockSyntax : DeclarationStatementSyntax
	{
		internal readonly NamespaceStatementSyntax _namespaceStatement;

		internal readonly GreenNode _members;

		internal readonly EndBlockStatementSyntax _endNamespaceStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal NamespaceStatementSyntax NamespaceStatement => _namespaceStatement;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Members => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_members);

		internal EndBlockStatementSyntax EndNamespaceStatement => _endNamespaceStatement;

		internal NamespaceBlockSyntax(SyntaxKind kind, NamespaceStatementSyntax namespaceStatement, GreenNode members, EndBlockStatementSyntax endNamespaceStatement)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(namespaceStatement);
			_namespaceStatement = namespaceStatement;
			if (members != null)
			{
				AdjustFlagsAndWidth(members);
				_members = members;
			}
			AdjustFlagsAndWidth(endNamespaceStatement);
			_endNamespaceStatement = endNamespaceStatement;
		}

		internal NamespaceBlockSyntax(SyntaxKind kind, NamespaceStatementSyntax namespaceStatement, GreenNode members, EndBlockStatementSyntax endNamespaceStatement, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(namespaceStatement);
			_namespaceStatement = namespaceStatement;
			if (members != null)
			{
				AdjustFlagsAndWidth(members);
				_members = members;
			}
			AdjustFlagsAndWidth(endNamespaceStatement);
			_endNamespaceStatement = endNamespaceStatement;
		}

		internal NamespaceBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, NamespaceStatementSyntax namespaceStatement, GreenNode members, EndBlockStatementSyntax endNamespaceStatement)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(namespaceStatement);
			_namespaceStatement = namespaceStatement;
			if (members != null)
			{
				AdjustFlagsAndWidth(members);
				_members = members;
			}
			AdjustFlagsAndWidth(endNamespaceStatement);
			_endNamespaceStatement = endNamespaceStatement;
		}

		internal NamespaceBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			NamespaceStatementSyntax namespaceStatementSyntax = (NamespaceStatementSyntax)reader.ReadValue();
			if (namespaceStatementSyntax != null)
			{
				AdjustFlagsAndWidth(namespaceStatementSyntax);
				_namespaceStatement = namespaceStatementSyntax;
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
				_endNamespaceStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_namespaceStatement);
			writer.WriteValue(_members);
			writer.WriteValue(_endNamespaceStatement);
		}

		static NamespaceBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new NamespaceBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(NamespaceBlockSyntax), (ObjectReader r) => new NamespaceBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _namespaceStatement, 
				1 => _members, 
				2 => _endNamespaceStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new NamespaceBlockSyntax(base.Kind, newErrors, GetAnnotations(), _namespaceStatement, _members, _endNamespaceStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new NamespaceBlockSyntax(base.Kind, GetDiagnostics(), annotations, _namespaceStatement, _members, _endNamespaceStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitNamespaceBlock(this);
		}
	}
}
