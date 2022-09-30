using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class TypeBlockSyntax : DeclarationStatementSyntax
	{
		internal readonly GreenNode _inherits;

		internal readonly GreenNode _implements;

		internal readonly GreenNode _members;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InheritsStatementSyntax> Inherits => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InheritsStatementSyntax>(_inherits);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImplementsStatementSyntax> Implements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImplementsStatementSyntax>(_implements);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Members => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_members);

		public abstract TypeStatementSyntax BlockStatement { get; }

		public abstract EndBlockStatementSyntax EndBlockStatement { get; }

		internal TypeBlockSyntax(SyntaxKind kind, GreenNode inherits, GreenNode implements, GreenNode members)
			: base(kind)
		{
			if (inherits != null)
			{
				AdjustFlagsAndWidth(inherits);
				_inherits = inherits;
			}
			if (implements != null)
			{
				AdjustFlagsAndWidth(implements);
				_implements = implements;
			}
			if (members != null)
			{
				AdjustFlagsAndWidth(members);
				_members = members;
			}
		}

		internal TypeBlockSyntax(SyntaxKind kind, GreenNode inherits, GreenNode implements, GreenNode members, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
			if (inherits != null)
			{
				AdjustFlagsAndWidth(inherits);
				_inherits = inherits;
			}
			if (implements != null)
			{
				AdjustFlagsAndWidth(implements);
				_implements = implements;
			}
			if (members != null)
			{
				AdjustFlagsAndWidth(members);
				_members = members;
			}
		}

		internal TypeBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode inherits, GreenNode implements, GreenNode members)
			: base(kind, errors, annotations)
		{
			if (inherits != null)
			{
				AdjustFlagsAndWidth(inherits);
				_inherits = inherits;
			}
			if (implements != null)
			{
				AdjustFlagsAndWidth(implements);
				_implements = implements;
			}
			if (members != null)
			{
				AdjustFlagsAndWidth(members);
				_members = members;
			}
		}

		internal TypeBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_inherits = greenNode;
			}
			GreenNode greenNode2 = (GreenNode)reader.ReadValue();
			if (greenNode2 != null)
			{
				AdjustFlagsAndWidth(greenNode2);
				_implements = greenNode2;
			}
			GreenNode greenNode3 = (GreenNode)reader.ReadValue();
			if (greenNode3 != null)
			{
				AdjustFlagsAndWidth(greenNode3);
				_members = greenNode3;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_inherits);
			writer.WriteValue(_implements);
			writer.WriteValue(_members);
		}
	}
}
