using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class TypeStatementSyntax : DeclarationStatementSyntax
	{
		internal readonly GreenNode _attributeLists;

		internal readonly GreenNode _modifiers;

		internal readonly IdentifierTokenSyntax _identifier;

		internal readonly TypeParameterListSyntax _typeParameterList;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(_attributeLists);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_modifiers);

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal TypeParameterListSyntax TypeParameterList => _typeParameterList;

		internal TypeStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
			: base(kind)
		{
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
			if (typeParameterList != null)
			{
				AdjustFlagsAndWidth(typeParameterList);
				_typeParameterList = typeParameterList;
			}
		}

		internal TypeStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ISyntaxFactoryContext context)
			: base(kind)
		{
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
			if (typeParameterList != null)
			{
				AdjustFlagsAndWidth(typeParameterList);
				_typeParameterList = typeParameterList;
			}
		}

		internal TypeStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
			: base(kind, errors, annotations)
		{
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
			if (typeParameterList != null)
			{
				AdjustFlagsAndWidth(typeParameterList);
				_typeParameterList = typeParameterList;
			}
		}

		internal TypeStatementSyntax(ObjectReader reader)
			: base(reader)
		{
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
				_identifier = identifierTokenSyntax;
			}
			TypeParameterListSyntax typeParameterListSyntax = (TypeParameterListSyntax)reader.ReadValue();
			if (typeParameterListSyntax != null)
			{
				AdjustFlagsAndWidth(typeParameterListSyntax);
				_typeParameterList = typeParameterListSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_attributeLists);
			writer.WriteValue(_modifiers);
			writer.WriteValue(_identifier);
			writer.WriteValue(_typeParameterList);
		}
	}
}
