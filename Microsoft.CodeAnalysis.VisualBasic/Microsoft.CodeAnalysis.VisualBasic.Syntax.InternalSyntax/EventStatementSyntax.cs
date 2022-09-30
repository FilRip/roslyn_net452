using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EventStatementSyntax : MethodBaseSyntax
	{
		internal readonly KeywordSyntax _customKeyword;

		internal readonly KeywordSyntax _eventKeyword;

		internal readonly IdentifierTokenSyntax _identifier;

		internal readonly SimpleAsClauseSyntax _asClause;

		internal readonly ImplementsClauseSyntax _implementsClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax CustomKeyword => _customKeyword;

		internal KeywordSyntax EventKeyword => _eventKeyword;

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal SimpleAsClauseSyntax AsClause => _asClause;

		internal ImplementsClauseSyntax ImplementsClause => _implementsClause;

		internal EventStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax customKeyword, KeywordSyntax eventKeyword, IdentifierTokenSyntax identifier, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, ImplementsClauseSyntax implementsClause)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 8;
			if (customKeyword != null)
			{
				AdjustFlagsAndWidth(customKeyword);
				_customKeyword = customKeyword;
			}
			AdjustFlagsAndWidth(eventKeyword);
			_eventKeyword = eventKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (implementsClause != null)
			{
				AdjustFlagsAndWidth(implementsClause);
				_implementsClause = implementsClause;
			}
		}

		internal EventStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax customKeyword, KeywordSyntax eventKeyword, IdentifierTokenSyntax identifier, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, ImplementsClauseSyntax implementsClause, ISyntaxFactoryContext context)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 8;
			SetFactoryContext(context);
			if (customKeyword != null)
			{
				AdjustFlagsAndWidth(customKeyword);
				_customKeyword = customKeyword;
			}
			AdjustFlagsAndWidth(eventKeyword);
			_eventKeyword = eventKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (implementsClause != null)
			{
				AdjustFlagsAndWidth(implementsClause);
				_implementsClause = implementsClause;
			}
		}

		internal EventStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax customKeyword, KeywordSyntax eventKeyword, IdentifierTokenSyntax identifier, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, ImplementsClauseSyntax implementsClause)
			: base(kind, errors, annotations, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 8;
			if (customKeyword != null)
			{
				AdjustFlagsAndWidth(customKeyword);
				_customKeyword = customKeyword;
			}
			AdjustFlagsAndWidth(eventKeyword);
			_eventKeyword = eventKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (implementsClause != null)
			{
				AdjustFlagsAndWidth(implementsClause);
				_implementsClause = implementsClause;
			}
		}

		internal EventStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 8;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_customKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_eventKeyword = keywordSyntax2;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_identifier = identifierTokenSyntax;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)reader.ReadValue();
			if (simpleAsClauseSyntax != null)
			{
				AdjustFlagsAndWidth(simpleAsClauseSyntax);
				_asClause = simpleAsClauseSyntax;
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
			writer.WriteValue(_customKeyword);
			writer.WriteValue(_eventKeyword);
			writer.WriteValue(_identifier);
			writer.WriteValue(_asClause);
			writer.WriteValue(_implementsClause);
		}

		static EventStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new EventStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(EventStatementSyntax), (ObjectReader r) => new EventStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _customKeyword, 
				3 => _eventKeyword, 
				4 => _identifier, 
				5 => _parameterList, 
				6 => _asClause, 
				7 => _implementsClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new EventStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _customKeyword, _eventKeyword, _identifier, _parameterList, _asClause, _implementsClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new EventStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _customKeyword, _eventKeyword, _identifier, _parameterList, _asClause, _implementsClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitEventStatement(this);
		}
	}
}
