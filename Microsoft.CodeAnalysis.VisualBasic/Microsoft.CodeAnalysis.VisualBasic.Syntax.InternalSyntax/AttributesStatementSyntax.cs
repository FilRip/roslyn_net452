using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class AttributesStatementSyntax : DeclarationStatementSyntax
	{
		internal readonly GreenNode _attributeLists;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(_attributeLists);

		internal AttributesStatementSyntax(SyntaxKind kind, GreenNode attributeLists)
			: base(kind)
		{
			base._slotCount = 1;
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
		}

		internal AttributesStatementSyntax(SyntaxKind kind, GreenNode attributeLists, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
		}

		internal AttributesStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
		}

		internal AttributesStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_attributeLists = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_attributeLists);
		}

		static AttributesStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new AttributesStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(AttributesStatementSyntax), (ObjectReader r) => new AttributesStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributesStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _attributeLists;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new AttributesStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new AttributesStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitAttributesStatement(this);
		}
	}
}
