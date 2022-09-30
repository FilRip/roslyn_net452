using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class PropertyStatementSyntax : MethodBaseSyntax
	{
		internal readonly KeywordSyntax _propertyKeyword;

		internal readonly IdentifierTokenSyntax _identifier;

		internal readonly AsClauseSyntax _asClause;

		internal readonly EqualsValueSyntax _initializer;

		internal readonly ImplementsClauseSyntax _implementsClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax PropertyKeyword => _propertyKeyword;

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal AsClauseSyntax AsClause => _asClause;

		internal EqualsValueSyntax Initializer => _initializer;

		internal ImplementsClauseSyntax ImplementsClause => _implementsClause;

		internal PropertyStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax propertyKeyword, IdentifierTokenSyntax identifier, ParameterListSyntax parameterList, AsClauseSyntax asClause, EqualsValueSyntax initializer, ImplementsClauseSyntax implementsClause)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 8;
			AdjustFlagsAndWidth(propertyKeyword);
			_propertyKeyword = propertyKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (initializer != null)
			{
				AdjustFlagsAndWidth(initializer);
				_initializer = initializer;
			}
			if (implementsClause != null)
			{
				AdjustFlagsAndWidth(implementsClause);
				_implementsClause = implementsClause;
			}
		}

		internal PropertyStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax propertyKeyword, IdentifierTokenSyntax identifier, ParameterListSyntax parameterList, AsClauseSyntax asClause, EqualsValueSyntax initializer, ImplementsClauseSyntax implementsClause, ISyntaxFactoryContext context)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 8;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(propertyKeyword);
			_propertyKeyword = propertyKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (initializer != null)
			{
				AdjustFlagsAndWidth(initializer);
				_initializer = initializer;
			}
			if (implementsClause != null)
			{
				AdjustFlagsAndWidth(implementsClause);
				_implementsClause = implementsClause;
			}
		}

		internal PropertyStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax propertyKeyword, IdentifierTokenSyntax identifier, ParameterListSyntax parameterList, AsClauseSyntax asClause, EqualsValueSyntax initializer, ImplementsClauseSyntax implementsClause)
			: base(kind, errors, annotations, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 8;
			AdjustFlagsAndWidth(propertyKeyword);
			_propertyKeyword = propertyKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (initializer != null)
			{
				AdjustFlagsAndWidth(initializer);
				_initializer = initializer;
			}
			if (implementsClause != null)
			{
				AdjustFlagsAndWidth(implementsClause);
				_implementsClause = implementsClause;
			}
		}

		internal PropertyStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 8;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_propertyKeyword = keywordSyntax;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_identifier = identifierTokenSyntax;
			}
			AsClauseSyntax asClauseSyntax = (AsClauseSyntax)reader.ReadValue();
			if (asClauseSyntax != null)
			{
				AdjustFlagsAndWidth(asClauseSyntax);
				_asClause = asClauseSyntax;
			}
			EqualsValueSyntax equalsValueSyntax = (EqualsValueSyntax)reader.ReadValue();
			if (equalsValueSyntax != null)
			{
				AdjustFlagsAndWidth(equalsValueSyntax);
				_initializer = equalsValueSyntax;
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
			writer.WriteValue(_propertyKeyword);
			writer.WriteValue(_identifier);
			writer.WriteValue(_asClause);
			writer.WriteValue(_initializer);
			writer.WriteValue(_implementsClause);
		}

		static PropertyStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new PropertyStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(PropertyStatementSyntax), (ObjectReader r) => new PropertyStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _propertyKeyword, 
				3 => _identifier, 
				4 => _parameterList, 
				5 => _asClause, 
				6 => _initializer, 
				7 => _implementsClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new PropertyStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _propertyKeyword, _identifier, _parameterList, _asClause, _initializer, _implementsClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new PropertyStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _propertyKeyword, _identifier, _parameterList, _asClause, _initializer, _implementsClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitPropertyStatement(this);
		}
	}
}
