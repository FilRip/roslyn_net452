using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ParameterSyntax : VisualBasicSyntaxNode
	{
		internal readonly GreenNode _attributeLists;

		internal readonly GreenNode _modifiers;

		internal readonly ModifiedIdentifierSyntax _identifier;

		internal readonly SimpleAsClauseSyntax _asClause;

		internal readonly EqualsValueSyntax _default;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(_attributeLists);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_modifiers);

		internal ModifiedIdentifierSyntax Identifier => _identifier;

		internal SimpleAsClauseSyntax AsClause => _asClause;

		internal EqualsValueSyntax Default => _default;

		internal ParameterSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, EqualsValueSyntax @default)
			: base(kind)
		{
			base._slotCount = 5;
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
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (@default != null)
			{
				AdjustFlagsAndWidth(@default);
				_default = @default;
			}
		}

		internal ParameterSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, EqualsValueSyntax @default, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 5;
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
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (@default != null)
			{
				AdjustFlagsAndWidth(@default);
				_default = @default;
			}
		}

		internal ParameterSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, EqualsValueSyntax @default)
			: base(kind, errors, annotations)
		{
			base._slotCount = 5;
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
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (@default != null)
			{
				AdjustFlagsAndWidth(@default);
				_default = @default;
			}
		}

		internal ParameterSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
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
			ModifiedIdentifierSyntax modifiedIdentifierSyntax = (ModifiedIdentifierSyntax)reader.ReadValue();
			if (modifiedIdentifierSyntax != null)
			{
				AdjustFlagsAndWidth(modifiedIdentifierSyntax);
				_identifier = modifiedIdentifierSyntax;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)reader.ReadValue();
			if (simpleAsClauseSyntax != null)
			{
				AdjustFlagsAndWidth(simpleAsClauseSyntax);
				_asClause = simpleAsClauseSyntax;
			}
			EqualsValueSyntax equalsValueSyntax = (EqualsValueSyntax)reader.ReadValue();
			if (equalsValueSyntax != null)
			{
				AdjustFlagsAndWidth(equalsValueSyntax);
				_default = equalsValueSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_attributeLists);
			writer.WriteValue(_modifiers);
			writer.WriteValue(_identifier);
			writer.WriteValue(_asClause);
			writer.WriteValue(_default);
		}

		static ParameterSyntax()
		{
			CreateInstance = (ObjectReader o) => new ParameterSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ParameterSyntax), (ObjectReader r) => new ParameterSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _identifier, 
				3 => _asClause, 
				4 => _default, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ParameterSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _identifier, _asClause, _default);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ParameterSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _identifier, _asClause, _default);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitParameter(this);
		}
	}
}
