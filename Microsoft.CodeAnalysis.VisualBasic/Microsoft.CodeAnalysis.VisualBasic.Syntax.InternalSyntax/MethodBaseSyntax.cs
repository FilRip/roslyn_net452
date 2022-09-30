using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class MethodBaseSyntax : DeclarationStatementSyntax
	{
		internal readonly GreenNode _attributeLists;

		internal readonly GreenNode _modifiers;

		internal readonly ParameterListSyntax _parameterList;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(_attributeLists);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_modifiers);

		internal ParameterListSyntax ParameterList => _parameterList;

		internal MethodBaseSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, ParameterListSyntax parameterList)
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
			if (parameterList != null)
			{
				AdjustFlagsAndWidth(parameterList);
				_parameterList = parameterList;
			}
		}

		internal MethodBaseSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, ParameterListSyntax parameterList, ISyntaxFactoryContext context)
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
			if (parameterList != null)
			{
				AdjustFlagsAndWidth(parameterList);
				_parameterList = parameterList;
			}
		}

		internal MethodBaseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, ParameterListSyntax parameterList)
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
			if (parameterList != null)
			{
				AdjustFlagsAndWidth(parameterList);
				_parameterList = parameterList;
			}
		}

		internal MethodBaseSyntax(ObjectReader reader)
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
			ParameterListSyntax parameterListSyntax = (ParameterListSyntax)reader.ReadValue();
			if (parameterListSyntax != null)
			{
				AdjustFlagsAndWidth(parameterListSyntax);
				_parameterList = parameterListSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_attributeLists);
			writer.WriteValue(_modifiers);
			writer.WriteValue(_parameterList);
		}
	}
}
