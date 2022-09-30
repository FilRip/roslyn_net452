using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class OperatorStatementSyntax : MethodBaseSyntax
	{
		internal readonly KeywordSyntax _operatorKeyword;

		internal readonly SyntaxToken _operatorToken;

		internal readonly SimpleAsClauseSyntax _asClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax OperatorKeyword => _operatorKeyword;

		internal SyntaxToken OperatorToken => _operatorToken;

		internal SimpleAsClauseSyntax AsClause => _asClause;

		internal OperatorStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax operatorKeyword, SyntaxToken operatorToken, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 6;
			AdjustFlagsAndWidth(operatorKeyword);
			_operatorKeyword = operatorKeyword;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal OperatorStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax operatorKeyword, SyntaxToken operatorToken, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, ISyntaxFactoryContext context)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 6;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(operatorKeyword);
			_operatorKeyword = operatorKeyword;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal OperatorStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax operatorKeyword, SyntaxToken operatorToken, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
			: base(kind, errors, annotations, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 6;
			AdjustFlagsAndWidth(operatorKeyword);
			_operatorKeyword = operatorKeyword;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal OperatorStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 6;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_operatorKeyword = keywordSyntax;
			}
			SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
			if (syntaxToken != null)
			{
				AdjustFlagsAndWidth(syntaxToken);
				_operatorToken = syntaxToken;
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
			writer.WriteValue(_operatorKeyword);
			writer.WriteValue(_operatorToken);
			writer.WriteValue(_asClause);
		}

		static OperatorStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new OperatorStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(OperatorStatementSyntax), (ObjectReader r) => new OperatorStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _operatorKeyword, 
				3 => _operatorToken, 
				4 => _parameterList, 
				5 => _asClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new OperatorStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _operatorKeyword, _operatorToken, _parameterList, _asClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new OperatorStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _operatorKeyword, _operatorToken, _parameterList, _asClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitOperatorStatement(this);
		}
	}
}
