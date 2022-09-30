using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class RangeVariableSymbol : Symbol, IRangeVariableSymbol
	{
		private class WithIdentifierToken : RangeVariableSymbol
		{
			private readonly SyntaxToken _identifierToken;

			public override string Name => VisualBasicExtensions.GetIdentifierText(_identifierToken);

			public override VisualBasicSyntaxNode Syntax
			{
				get
				{
					SyntaxToken identifierToken = _identifierToken;
					return (VisualBasicSyntaxNode)identifierToken.Parent;
				}
			}

			public override ImmutableArray<Location> Locations
			{
				get
				{
					SyntaxToken identifierToken = _identifierToken;
					return ImmutableArray.Create(identifierToken.GetLocation());
				}
			}

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
			{
				get
				{
					VisualBasicSyntaxNode visualBasicSyntaxNode = null;
					VisualBasicSyntaxNode visualBasicSyntaxNode2 = null;
					SyntaxToken identifierToken = _identifierToken;
					VisualBasicSyntaxNode visualBasicSyntaxNode3 = (VisualBasicSyntaxNode)identifierToken.Parent;
					if (visualBasicSyntaxNode3 != null)
					{
						visualBasicSyntaxNode = visualBasicSyntaxNode3.Parent;
					}
					if (visualBasicSyntaxNode != null)
					{
						visualBasicSyntaxNode2 = visualBasicSyntaxNode.Parent;
					}
					if (visualBasicSyntaxNode is CollectionRangeVariableSyntax collectionRangeVariableSyntax && _identifierToken == collectionRangeVariableSyntax.Identifier.Identifier)
					{
						return ImmutableArray.Create(collectionRangeVariableSyntax.GetReference());
					}
					if (visualBasicSyntaxNode2 is ExpressionRangeVariableSyntax expressionRangeVariableSyntax && expressionRangeVariableSyntax.NameEquals != null && expressionRangeVariableSyntax.NameEquals.Identifier.Identifier == _identifierToken)
					{
						return ImmutableArray.Create(expressionRangeVariableSyntax.GetReference());
					}
					if (visualBasicSyntaxNode2 is AggregationRangeVariableSyntax aggregationRangeVariableSyntax && aggregationRangeVariableSyntax.NameEquals != null && aggregationRangeVariableSyntax.NameEquals.Identifier.Identifier == _identifierToken)
					{
						return ImmutableArray.Create(aggregationRangeVariableSyntax.GetReference());
					}
					return ImmutableArray<SyntaxReference>.Empty;
				}
			}

			public WithIdentifierToken(Binder binder, SyntaxToken declaringIdentifier, TypeSymbol type)
				: base(binder, type)
			{
				_identifierToken = declaringIdentifier;
			}

			public override bool Equals(object obj)
			{
				WithIdentifierToken withIdentifierToken = obj as WithIdentifierToken;
				if ((object)this == withIdentifierToken)
				{
					return true;
				}
				int result;
				if ((object)withIdentifierToken != null)
				{
					SyntaxToken identifierToken = withIdentifierToken._identifierToken;
					result = (identifierToken.Equals(_identifierToken) ? 1 : 0);
				}
				else
				{
					result = 0;
				}
				return (byte)result != 0;
			}

			public override int GetHashCode()
			{
				SyntaxToken identifierToken = _identifierToken;
				return identifierToken.GetHashCode();
			}
		}

		private class ForErrorRecovery : RangeVariableSymbol
		{
			private readonly VisualBasicSyntaxNode _syntax;

			public override VisualBasicSyntaxNode Syntax => _syntax;

			public override string Name => "$" + _syntax.Position;

			public override ImmutableArray<Location> Locations => ImmutableArray.Create(_syntax.GetLocation());

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

			public ForErrorRecovery(Binder binder, VisualBasicSyntaxNode syntax, TypeSymbol type)
				: base(binder, type)
			{
				_syntax = syntax;
			}
		}

		private class CompilerGenerated : ForErrorRecovery
		{
			private readonly string _name;

			public override string Name => _name;

			public CompilerGenerated(Binder binder, VisualBasicSyntaxNode syntax, string name, TypeSymbol type)
				: base(binder, syntax, type)
			{
				_name = name;
			}
		}

		internal readonly Binder m_Binder;

		private readonly TypeSymbol _type;

		public abstract VisualBasicSyntaxNode Syntax { get; }

		public override SymbolKind Kind => SymbolKind.RangeVariable;

		public override Symbol ContainingSymbol => m_Binder.ContainingMember;

		public virtual TypeSymbol Type => _type;

		public abstract override string Name { get; }

		public abstract override ImmutableArray<Location> Locations { get; }

		public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

		public override bool IsShared => false;

		public override bool IsOverridable => false;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverrides => false;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		private RangeVariableSymbol(Binder binder, TypeSymbol type)
		{
			m_Binder = binder;
			_type = type;
		}

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitRangeVariable(this, arg);
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitRangeVariable(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitRangeVariable(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitRangeVariable(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitRangeVariable(this);
		}

		internal static RangeVariableSymbol Create(Binder binder, SyntaxToken declaringIdentifier, TypeSymbol type)
		{
			return new WithIdentifierToken(binder, declaringIdentifier, type);
		}

		internal static RangeVariableSymbol CreateForErrorRecovery(Binder binder, VisualBasicSyntaxNode syntax, TypeSymbol type)
		{
			return new ForErrorRecovery(binder, syntax, type);
		}

		internal static RangeVariableSymbol CreateCompilerGenerated(Binder binder, VisualBasicSyntaxNode syntax, string name, TypeSymbol type)
		{
			return new CompilerGenerated(binder, syntax, name, type);
		}
	}
}
