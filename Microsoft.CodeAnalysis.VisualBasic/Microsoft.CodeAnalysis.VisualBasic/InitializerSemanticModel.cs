using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class InitializerSemanticModel : MemberSemanticModel
	{
		private InitializerSemanticModel(VisualBasicSyntaxNode root, Binder binder, SyntaxTreeSemanticModel containingSemanticModelOpt = null, SyntaxTreeSemanticModel parentSemanticModelOpt = null, int speculatedPosition = 0, bool ignoreAccessibility = false)
			: base(root, binder, containingSemanticModelOpt, parentSemanticModelOpt, speculatedPosition, ignoreAccessibility)
		{
		}

		internal static InitializerSemanticModel Create(SyntaxTreeSemanticModel containingSemanticModel, DeclarationInitializerBinder binder, bool ignoreAccessibility = false)
		{
			return new InitializerSemanticModel(binder.Root, binder, containingSemanticModel, null, 0, ignoreAccessibility);
		}

		internal static InitializerSemanticModel CreateSpeculative(SyntaxTreeSemanticModel parentSemanticModel, EqualsValueSyntax root, Binder binder, int position)
		{
			return new InitializerSemanticModel(root, binder, null, parentSemanticModel, position);
		}

		internal override BoundNode Bind(Binder binder, SyntaxNode node, BindingDiagnosticBag diagnostics)
		{
			BoundNode boundNode = null;
			switch (VisualBasicExtensions.Kind(node))
			{
			case SyntaxKind.FieldDeclaration:
			{
				if (base.MemberSymbol.Kind == SymbolKind.Field)
				{
					SourceFieldSymbol sourceFieldSymbol = (SourceFieldSymbol)base.MemberSymbol;
					boundNode = BindInitializer(binder, sourceFieldSymbol.EqualsValueOrAsNewInitOpt, diagnostics);
					break;
				}
				VariableDeclaratorSyntax variableDeclaratorSyntax = (VariableDeclaratorSyntax)((ModifiedIdentifierSyntax)((SourcePropertySymbol)base.MemberSymbol).Syntax).Parent;
				VisualBasicSyntaxNode visualBasicSyntaxNode2 = variableDeclaratorSyntax.AsClause;
				if (visualBasicSyntaxNode2 == null || visualBasicSyntaxNode2.Kind() != SyntaxKind.AsNewClause)
				{
					visualBasicSyntaxNode2 = variableDeclaratorSyntax.Initializer;
				}
				boundNode = BindInitializer(binder, visualBasicSyntaxNode2, diagnostics);
				break;
			}
			case SyntaxKind.PropertyStatement:
			{
				PropertyStatementSyntax propertyStatementSyntax = (PropertyStatementSyntax)((SourcePropertySymbol)base.MemberSymbol).DeclarationSyntax;
				VisualBasicSyntaxNode visualBasicSyntaxNode = propertyStatementSyntax.AsClause;
				if (visualBasicSyntaxNode == null || visualBasicSyntaxNode.Kind() != SyntaxKind.AsNewClause)
				{
					visualBasicSyntaxNode = propertyStatementSyntax.Initializer;
				}
				boundNode = BindInitializer(binder, visualBasicSyntaxNode, diagnostics);
				break;
			}
			case SyntaxKind.Parameter:
			{
				ParameterSyntax parameterSyntax = (ParameterSyntax)node;
				boundNode = BindInitializer(binder, parameterSyntax.Default, diagnostics);
				break;
			}
			case SyntaxKind.EnumMemberDeclaration:
			{
				EnumMemberDeclarationSyntax enumMemberDeclarationSyntax = (EnumMemberDeclarationSyntax)node;
				boundNode = BindInitializer(binder, enumMemberDeclarationSyntax.Initializer, diagnostics);
				break;
			}
			case SyntaxKind.AsNewClause:
			case SyntaxKind.EqualsValue:
				boundNode = BindInitializer(binder, node, diagnostics);
				break;
			}
			if (boundNode != null)
			{
				return boundNode;
			}
			return base.Bind(binder, node, diagnostics);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_4_GetInitializedFieldsOrProperties))]
		private IEnumerable<Symbol> GetInitializedFieldsOrProperties(Binder binder)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_4_GetInitializedFieldsOrProperties(-2)
			{
				_0024VB_0024Me = this,
				_0024P_binder = binder
			};
		}

		private BoundNode BindInitializer(Binder binder, SyntaxNode initializer, BindingDiagnosticBag diagnostics)
		{
			BoundNode boundNode = null;
			switch (base.MemberSymbol.Kind)
			{
			case SymbolKind.Field:
				if (base.MemberSymbol is SourceEnumConstantSymbol)
				{
					if (VisualBasicExtensions.Kind(initializer) == SyntaxKind.EqualsValue)
					{
						SourceEnumConstantSymbol fieldSymbol = (SourceEnumConstantSymbol)base.MemberSymbol;
						EqualsValueSyntax equalsValueOrAsNewSyntax = (EqualsValueSyntax)initializer;
						ConstantValue constValue = null;
						boundNode = binder.BindFieldAndEnumConstantInitializer(fieldSymbol, equalsValueOrAsNewSyntax, isEnum: true, diagnostics, out constValue);
					}
				}
				else
				{
					SourceFieldSymbol fieldSymbol2 = (SourceFieldSymbol)base.MemberSymbol;
					ArrayBuilder<BoundInitializer> instance2 = ArrayBuilder<BoundInitializer>.GetInstance();
					if (initializer != null)
					{
						ImmutableArray<FieldSymbol> fieldSymbols = ImmutableArray.CreateRange(GetInitializedFieldsOrProperties(binder).Cast<FieldSymbol>());
						binder.BindFieldInitializer(fieldSymbols, initializer, instance2, diagnostics, bindingForSemanticModel: true);
					}
					else
					{
						binder.BindArrayFieldImplicitInitializer(fieldSymbol2, instance2, diagnostics);
					}
					boundNode = instance2.First();
					instance2.Free();
				}
				if (boundNode is BoundExpression initialValue2)
				{
					return new BoundFieldInitializer(initializer, ImmutableArray.Create((FieldSymbol)base.MemberSymbol), null, initialValue2);
				}
				break;
			case SymbolKind.Property:
			{
				ImmutableArray<PropertySymbol> immutableArray = ImmutableArray.CreateRange(GetInitializedFieldsOrProperties(binder).Cast<PropertySymbol>());
				ArrayBuilder<BoundInitializer> instance = ArrayBuilder<BoundInitializer>.GetInstance();
				binder.BindPropertyInitializer(immutableArray, initializer, instance, diagnostics);
				boundNode = instance.First();
				instance.Free();
				if (boundNode is BoundExpression initialValue)
				{
					return new BoundPropertyInitializer(initializer, immutableArray, null, initialValue);
				}
				break;
			}
			case SymbolKind.Parameter:
				if (VisualBasicExtensions.Kind(initializer) == SyntaxKind.EqualsValue)
				{
					SourceComplexParameterSymbol sourceComplexParameterSymbol = (SourceComplexParameterSymbol)base.RootBinder.ContainingMember;
					TypeSymbol type = sourceComplexParameterSymbol.Type;
					EqualsValueSyntax equalsValueSyntax = (EqualsValueSyntax)initializer;
					ConstantValue constValue = null;
					boundNode = binder.BindParameterDefaultValue(type, equalsValueSyntax, diagnostics, out constValue);
					if (boundNode is BoundExpression value)
					{
						return new BoundParameterEqualsValue(initializer, sourceComplexParameterSymbol, value);
					}
				}
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(base.MemberSymbol.Kind);
			}
			return boundNode;
		}

		internal override BoundNode GetBoundRoot()
		{
			SyntaxNode syntaxNode = base.Root;
			if (VisualBasicExtensions.Kind(syntaxNode) == SyntaxKind.FieldDeclaration)
			{
				if (base.RootBinder.ContainingMember is SourceFieldSymbol sourceFieldSymbol)
				{
					syntaxNode = sourceFieldSymbol.EqualsValueOrAsNewInitOpt ?? sourceFieldSymbol.Syntax;
				}
				else
				{
					VariableDeclaratorSyntax variableDeclaratorSyntax = (VariableDeclaratorSyntax)((ModifiedIdentifierSyntax)(base.RootBinder.ContainingMember as SourcePropertySymbol).Syntax).Parent;
					VisualBasicSyntaxNode visualBasicSyntaxNode = variableDeclaratorSyntax.AsClause;
					if (visualBasicSyntaxNode == null || visualBasicSyntaxNode.Kind() != SyntaxKind.AsNewClause)
					{
						visualBasicSyntaxNode = variableDeclaratorSyntax.Initializer;
					}
					if (visualBasicSyntaxNode != null)
					{
						syntaxNode = visualBasicSyntaxNode;
					}
				}
			}
			else if (VisualBasicExtensions.Kind(syntaxNode) == SyntaxKind.PropertyStatement)
			{
				PropertyStatementSyntax propertyStatementSyntax = (PropertyStatementSyntax)syntaxNode;
				VisualBasicSyntaxNode visualBasicSyntaxNode2 = propertyStatementSyntax.AsClause;
				if (visualBasicSyntaxNode2 == null || visualBasicSyntaxNode2.Kind() != SyntaxKind.AsNewClause)
				{
					visualBasicSyntaxNode2 = propertyStatementSyntax.Initializer;
				}
				if (visualBasicSyntaxNode2 != null)
				{
					syntaxNode = visualBasicSyntaxNode2;
				}
			}
			return GetUpperBoundNode(syntaxNode);
		}

		internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, EqualsValueSyntax initializer, out SemanticModel speculativeModel)
		{
			Binder enclosingBinder = GetEnclosingBinder(position);
			if (enclosingBinder == null)
			{
				speculativeModel = null;
				return false;
			}
			enclosingBinder = SpeculativeBinder.Create(enclosingBinder);
			speculativeModel = CreateSpeculative(parentModel, initializer, enclosingBinder, position);
			return true;
		}

		internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ExecutableStatementSyntax statement, out SemanticModel speculativeModel)
		{
			speculativeModel = null;
			return false;
		}

		internal override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, MethodBlockBaseSyntax body, out SemanticModel speculativeModel)
		{
			speculativeModel = null;
			return false;
		}
	}
}
