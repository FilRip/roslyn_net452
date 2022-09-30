using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class DelegateStatementSyntax : MethodBaseSyntax
	{
		internal readonly KeywordSyntax _delegateKeyword;

		internal readonly KeywordSyntax _subOrFunctionKeyword;

		internal readonly IdentifierTokenSyntax _identifier;

		internal readonly TypeParameterListSyntax _typeParameterList;

		internal readonly SimpleAsClauseSyntax _asClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax DelegateKeyword => _delegateKeyword;

		internal KeywordSyntax SubOrFunctionKeyword => _subOrFunctionKeyword;

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal TypeParameterListSyntax TypeParameterList => _typeParameterList;

		internal SimpleAsClauseSyntax AsClause => _asClause;

		internal DelegateStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax delegateKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 8;
			AdjustFlagsAndWidth(delegateKeyword);
			_delegateKeyword = delegateKeyword;
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
		}

		internal DelegateStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax delegateKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, ISyntaxFactoryContext context)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 8;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(delegateKeyword);
			_delegateKeyword = delegateKeyword;
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
		}

		internal DelegateStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax delegateKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
			: base(kind, errors, annotations, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 8;
			AdjustFlagsAndWidth(delegateKeyword);
			_delegateKeyword = delegateKeyword;
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
		}

		internal DelegateStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 8;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_delegateKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_subOrFunctionKeyword = keywordSyntax2;
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
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_delegateKeyword);
			writer.WriteValue(_subOrFunctionKeyword);
			writer.WriteValue(_identifier);
			writer.WriteValue(_typeParameterList);
			writer.WriteValue(_asClause);
		}

		static DelegateStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new DelegateStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(DelegateStatementSyntax), (ObjectReader r) => new DelegateStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _delegateKeyword, 
				3 => _subOrFunctionKeyword, 
				4 => _identifier, 
				5 => _typeParameterList, 
				6 => _parameterList, 
				7 => _asClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new DelegateStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _delegateKeyword, _subOrFunctionKeyword, _identifier, _typeParameterList, _parameterList, _asClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new DelegateStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _delegateKeyword, _subOrFunctionKeyword, _identifier, _typeParameterList, _parameterList, _asClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitDelegateStatement(this);
		}
	}
}
