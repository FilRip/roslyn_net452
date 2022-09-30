using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class MethodStatementSyntax : MethodBaseSyntax
	{
		internal readonly KeywordSyntax _subOrFunctionKeyword;

		internal readonly IdentifierTokenSyntax _identifier;

		internal readonly TypeParameterListSyntax _typeParameterList;

		internal readonly SimpleAsClauseSyntax _asClause;

		internal readonly HandlesClauseSyntax _handlesClause;

		internal readonly ImplementsClauseSyntax _implementsClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax SubOrFunctionKeyword => _subOrFunctionKeyword;

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal TypeParameterListSyntax TypeParameterList => _typeParameterList;

		internal SimpleAsClauseSyntax AsClause => _asClause;

		internal HandlesClauseSyntax HandlesClause => _handlesClause;

		internal ImplementsClauseSyntax ImplementsClause => _implementsClause;

		internal MethodStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, HandlesClauseSyntax handlesClause, ImplementsClauseSyntax implementsClause)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 9;
			AdjustFlagsAndWidth(subOrFunctionKeyword);
			_subOrFunctionKeyword = subOrFunctionKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (typeParameterList != null)
			{
				AdjustFlagsAndWidth(typeParameterList);
				_typeParameterList = typeParameterList;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (handlesClause != null)
			{
				AdjustFlagsAndWidth(handlesClause);
				_handlesClause = handlesClause;
			}
			if (implementsClause != null)
			{
				AdjustFlagsAndWidth(implementsClause);
				_implementsClause = implementsClause;
			}
		}

		internal MethodStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, HandlesClauseSyntax handlesClause, ImplementsClauseSyntax implementsClause, ISyntaxFactoryContext context)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 9;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(subOrFunctionKeyword);
			_subOrFunctionKeyword = subOrFunctionKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (typeParameterList != null)
			{
				AdjustFlagsAndWidth(typeParameterList);
				_typeParameterList = typeParameterList;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (handlesClause != null)
			{
				AdjustFlagsAndWidth(handlesClause);
				_handlesClause = handlesClause;
			}
			if (implementsClause != null)
			{
				AdjustFlagsAndWidth(implementsClause);
				_implementsClause = implementsClause;
			}
		}

		internal MethodStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, HandlesClauseSyntax handlesClause, ImplementsClauseSyntax implementsClause)
			: base(kind, errors, annotations, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 9;
			AdjustFlagsAndWidth(subOrFunctionKeyword);
			_subOrFunctionKeyword = subOrFunctionKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (typeParameterList != null)
			{
				AdjustFlagsAndWidth(typeParameterList);
				_typeParameterList = typeParameterList;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (handlesClause != null)
			{
				AdjustFlagsAndWidth(handlesClause);
				_handlesClause = handlesClause;
			}
			if (implementsClause != null)
			{
				AdjustFlagsAndWidth(implementsClause);
				_implementsClause = implementsClause;
			}
		}

		internal MethodStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 9;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_subOrFunctionKeyword = keywordSyntax;
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
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)reader.ReadValue();
			if (simpleAsClauseSyntax != null)
			{
				AdjustFlagsAndWidth(simpleAsClauseSyntax);
				_asClause = simpleAsClauseSyntax;
			}
			HandlesClauseSyntax handlesClauseSyntax = (HandlesClauseSyntax)reader.ReadValue();
			if (handlesClauseSyntax != null)
			{
				AdjustFlagsAndWidth(handlesClauseSyntax);
				_handlesClause = handlesClauseSyntax;
			}
			ImplementsClauseSyntax implementsClauseSyntax = (ImplementsClauseSyntax)reader.ReadValue();
			if (implementsClauseSyntax != null)
			{
				AdjustFlagsAndWidth(implementsClauseSyntax);
				_implementsClause = implementsClauseSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_subOrFunctionKeyword);
			writer.WriteValue(_identifier);
			writer.WriteValue(_typeParameterList);
			writer.WriteValue(_asClause);
			writer.WriteValue(_handlesClause);
			writer.WriteValue(_implementsClause);
		}

		static MethodStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new MethodStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(MethodStatementSyntax), (ObjectReader r) => new MethodStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _subOrFunctionKeyword, 
				3 => _identifier, 
				4 => _typeParameterList, 
				5 => _parameterList, 
				6 => _asClause, 
				7 => _handlesClause, 
				8 => _implementsClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new MethodStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _subOrFunctionKeyword, _identifier, _typeParameterList, _parameterList, _asClause, _handlesClause, _implementsClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new MethodStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _subOrFunctionKeyword, _identifier, _typeParameterList, _parameterList, _asClause, _handlesClause, _implementsClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitMethodStatement(this);
		}
	}
}
