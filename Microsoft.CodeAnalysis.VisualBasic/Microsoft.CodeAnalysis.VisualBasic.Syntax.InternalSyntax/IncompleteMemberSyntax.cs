using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class IncompleteMemberSyntax : DeclarationStatementSyntax
	{
		internal readonly GreenNode _attributeLists;

		internal readonly GreenNode _modifiers;

		internal readonly IdentifierTokenSyntax _missingIdentifier;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(_attributeLists);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_modifiers);

		internal IdentifierTokenSyntax MissingIdentifier => _missingIdentifier;

		internal IncompleteMemberSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, IdentifierTokenSyntax missingIdentifier)
			: base(kind)
		{
			base._slotCount = 3;
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
			if (modifiers != null)
			{
				AdjustFlagsAndWidth(modifiers);
				_modifiers = modifiers;
			}
			if (missingIdentifier != null)
			{
				AdjustFlagsAndWidth(missingIdentifier);
				_missingIdentifier = missingIdentifier;
			}
		}

		internal IncompleteMemberSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, IdentifierTokenSyntax missingIdentifier, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
			if (modifiers != null)
			{
				AdjustFlagsAndWidth(modifiers);
				_modifiers = modifiers;
			}
			if (missingIdentifier != null)
			{
				AdjustFlagsAndWidth(missingIdentifier);
				_missingIdentifier = missingIdentifier;
			}
		}

		internal IncompleteMemberSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, IdentifierTokenSyntax missingIdentifier)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
			if (modifiers != null)
			{
				AdjustFlagsAndWidth(modifiers);
				_modifiers = modifiers;
			}
			if (missingIdentifier != null)
			{
				AdjustFlagsAndWidth(missingIdentifier);
				_missingIdentifier = missingIdentifier;
			}
		}

		internal IncompleteMemberSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_attributeLists = greenNode;
			}
			GreenNode greenNode2 = (GreenNode)reader.ReadValue();
			if (greenNode2 != null)
			{
				AdjustFlagsAndWidth(greenNode2);
				_modifiers = greenNode2;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_missingIdentifier = identifierTokenSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_attributeLists);
			writer.WriteValue(_modifiers);
			writer.WriteValue(_missingIdentifier);
		}

		static IncompleteMemberSyntax()
		{
			CreateInstance = (ObjectReader o) => new IncompleteMemberSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(IncompleteMemberSyntax), (ObjectReader r) => new IncompleteMemberSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _missingIdentifier, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new IncompleteMemberSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _missingIdentifier);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new IncompleteMemberSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _missingIdentifier);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitIncompleteMember(this);
		}
	}
}
