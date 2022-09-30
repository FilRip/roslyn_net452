using System;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class UnboundLambdaParameterSymbol : LambdaParameterSymbol
	{
		private readonly ModifiedIdentifierSyntax _identifierSyntax;

		private readonly SyntaxNodeOrToken _typeSyntax;

		public SyntaxToken IdentifierSyntax => _identifierSyntax.Identifier;

		public ModifiedIdentifierSyntax Syntax => _identifierSyntax;

		public SyntaxNodeOrToken TypeSyntax => _typeSyntax;

		public override Symbol ContainingSymbol => null;

		private UnboundLambdaParameterSymbol(string name, int ordinal, TypeSymbol type, Location location, SourceParameterFlags flags, ModifiedIdentifierSyntax identifierSyntax, SyntaxNodeOrToken typeSyntax)
			: base(name, ordinal, type, (flags & SourceParameterFlags.ByRef) != 0, location)
		{
			_identifierSyntax = identifierSyntax;
			_typeSyntax = typeSyntax;
		}

		internal static ParameterSymbol CreateFromSyntax(ParameterSyntax syntax, string name, SourceParameterFlags flags, int ordinal, Binder binder, BindingDiagnosticBag diagBag)
		{
			if ((flags & SourceParameterFlags.ParamArray) != 0)
			{
				Binder.ReportDiagnostic(diagBag, GetModifierToken(syntax.Modifiers, SyntaxKind.ParamArrayKeyword), ERRID.ERR_ParamArrayIllegal1, "Lambda");
			}
			if ((flags & SourceParameterFlags.Optional) != 0)
			{
				Binder.ReportDiagnostic(diagBag, GetModifierToken(syntax.Modifiers, SyntaxKind.OptionalKeyword), ERRID.ERR_OptionalIllegal1, "Lambda");
			}
			if (syntax.AttributeLists.Node != null)
			{
				Binder.ReportDiagnostic(diagBag, syntax.AttributeLists.Node, ERRID.ERR_LambdasCannotHaveAttributes);
			}
			Func<DiagnosticInfo> getRequireTypeDiagnosticInfoFunc = null;
			TypeSymbol typeSymbol = binder.DecodeModifiedIdentifierType(syntax.Identifier, syntax.AsClause, null, getRequireTypeDiagnosticInfoFunc, diagBag, Binder.ModifiedIdentifierTypeDecoderContext.LambdaParameterType);
			if (TypeSymbolExtensions.IsObjectType(typeSymbol) && syntax.AsClause == null)
			{
				typeSymbol = null;
			}
			return new UnboundLambdaParameterSymbol(name, ordinal, typeSymbol, syntax.Identifier.Identifier.GetLocation(), flags, syntax.Identifier, (syntax.AsClause == null) ? ((SyntaxNodeOrToken)syntax.Identifier) : ((SyntaxNodeOrToken)syntax.AsClause.Type));
		}

		private static SyntaxToken GetModifierToken(SyntaxTokenList modifiers, SyntaxKind tokenKind)
		{
			SyntaxTokenList.Enumerator enumerator = modifiers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxToken current = enumerator.Current;
				if (VisualBasicExtensions.Kind(current) == tokenKind)
				{
					return current;
				}
			}
			throw ExceptionUtilities.Unreachable;
		}
	}
}
