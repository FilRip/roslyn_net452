using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class DeclareStatementSyntax : MethodBaseSyntax
	{
		internal readonly KeywordSyntax _declareKeyword;

		internal readonly KeywordSyntax _charsetKeyword;

		internal readonly KeywordSyntax _subOrFunctionKeyword;

		internal readonly IdentifierTokenSyntax _identifier;

		internal readonly KeywordSyntax _libKeyword;

		internal readonly LiteralExpressionSyntax _libraryName;

		internal readonly KeywordSyntax _aliasKeyword;

		internal readonly LiteralExpressionSyntax _aliasName;

		internal readonly SimpleAsClauseSyntax _asClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax DeclareKeyword => _declareKeyword;

		internal KeywordSyntax CharsetKeyword => _charsetKeyword;

		internal KeywordSyntax SubOrFunctionKeyword => _subOrFunctionKeyword;

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal KeywordSyntax LibKeyword => _libKeyword;

		internal LiteralExpressionSyntax LibraryName => _libraryName;

		internal KeywordSyntax AliasKeyword => _aliasKeyword;

		internal LiteralExpressionSyntax AliasName => _aliasName;

		internal SimpleAsClauseSyntax AsClause => _asClause;

		internal DeclareStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax declareKeyword, KeywordSyntax charsetKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, KeywordSyntax libKeyword, LiteralExpressionSyntax libraryName, KeywordSyntax aliasKeyword, LiteralExpressionSyntax aliasName, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 12;
			AdjustFlagsAndWidth(declareKeyword);
			_declareKeyword = declareKeyword;
			if (charsetKeyword != null)
			{
				AdjustFlagsAndWidth(charsetKeyword);
				_charsetKeyword = charsetKeyword;
			}
			AdjustFlagsAndWidth(subOrFunctionKeyword);
			_subOrFunctionKeyword = subOrFunctionKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			AdjustFlagsAndWidth(libKeyword);
			_libKeyword = libKeyword;
			AdjustFlagsAndWidth(libraryName);
			_libraryName = libraryName;
			if (aliasKeyword != null)
			{
				AdjustFlagsAndWidth(aliasKeyword);
				_aliasKeyword = aliasKeyword;
			}
			if (aliasName != null)
			{
				AdjustFlagsAndWidth(aliasName);
				_aliasName = aliasName;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal DeclareStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax declareKeyword, KeywordSyntax charsetKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, KeywordSyntax libKeyword, LiteralExpressionSyntax libraryName, KeywordSyntax aliasKeyword, LiteralExpressionSyntax aliasName, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, ISyntaxFactoryContext context)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 12;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(declareKeyword);
			_declareKeyword = declareKeyword;
			if (charsetKeyword != null)
			{
				AdjustFlagsAndWidth(charsetKeyword);
				_charsetKeyword = charsetKeyword;
			}
			AdjustFlagsAndWidth(subOrFunctionKeyword);
			_subOrFunctionKeyword = subOrFunctionKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			AdjustFlagsAndWidth(libKeyword);
			_libKeyword = libKeyword;
			AdjustFlagsAndWidth(libraryName);
			_libraryName = libraryName;
			if (aliasKeyword != null)
			{
				AdjustFlagsAndWidth(aliasKeyword);
				_aliasKeyword = aliasKeyword;
			}
			if (aliasName != null)
			{
				AdjustFlagsAndWidth(aliasName);
				_aliasName = aliasName;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal DeclareStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax declareKeyword, KeywordSyntax charsetKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, KeywordSyntax libKeyword, LiteralExpressionSyntax libraryName, KeywordSyntax aliasKeyword, LiteralExpressionSyntax aliasName, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
			: base(kind, errors, annotations, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 12;
			AdjustFlagsAndWidth(declareKeyword);
			_declareKeyword = declareKeyword;
			if (charsetKeyword != null)
			{
				AdjustFlagsAndWidth(charsetKeyword);
				_charsetKeyword = charsetKeyword;
			}
			AdjustFlagsAndWidth(subOrFunctionKeyword);
			_subOrFunctionKeyword = subOrFunctionKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			AdjustFlagsAndWidth(libKeyword);
			_libKeyword = libKeyword;
			AdjustFlagsAndWidth(libraryName);
			_libraryName = libraryName;
			if (aliasKeyword != null)
			{
				AdjustFlagsAndWidth(aliasKeyword);
				_aliasKeyword = aliasKeyword;
			}
			if (aliasName != null)
			{
				AdjustFlagsAndWidth(aliasName);
				_aliasName = aliasName;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal DeclareStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 12;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_declareKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_charsetKeyword = keywordSyntax2;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax3 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax3);
				_subOrFunctionKeyword = keywordSyntax3;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_identifier = identifierTokenSyntax;
			}
			KeywordSyntax keywordSyntax4 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax4 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax4);
				_libKeyword = keywordSyntax4;
			}
			LiteralExpressionSyntax literalExpressionSyntax = (LiteralExpressionSyntax)reader.ReadValue();
			if (literalExpressionSyntax != null)
			{
				AdjustFlagsAndWidth(literalExpressionSyntax);
				_libraryName = literalExpressionSyntax;
			}
			KeywordSyntax keywordSyntax5 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax5 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax5);
				_aliasKeyword = keywordSyntax5;
			}
			LiteralExpressionSyntax literalExpressionSyntax2 = (LiteralExpressionSyntax)reader.ReadValue();
			if (literalExpressionSyntax2 != null)
			{
				AdjustFlagsAndWidth(literalExpressionSyntax2);
				_aliasName = literalExpressionSyntax2;
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
			writer.WriteValue(_declareKeyword);
			writer.WriteValue(_charsetKeyword);
			writer.WriteValue(_subOrFunctionKeyword);
			writer.WriteValue(_identifier);
			writer.WriteValue(_libKeyword);
			writer.WriteValue(_libraryName);
			writer.WriteValue(_aliasKeyword);
			writer.WriteValue(_aliasName);
			writer.WriteValue(_asClause);
		}

		static DeclareStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new DeclareStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(DeclareStatementSyntax), (ObjectReader r) => new DeclareStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _declareKeyword, 
				3 => _charsetKeyword, 
				4 => _subOrFunctionKeyword, 
				5 => _identifier, 
				6 => _libKeyword, 
				7 => _libraryName, 
				8 => _aliasKeyword, 
				9 => _aliasName, 
				10 => _parameterList, 
				11 => _asClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new DeclareStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _declareKeyword, _charsetKeyword, _subOrFunctionKeyword, _identifier, _libKeyword, _libraryName, _aliasKeyword, _aliasName, _parameterList, _asClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new DeclareStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _declareKeyword, _charsetKeyword, _subOrFunctionKeyword, _identifier, _libKeyword, _libraryName, _aliasKeyword, _aliasName, _parameterList, _asClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitDeclareStatement(this);
		}
	}
}
