using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class EarlyWellKnownAttributeBinder : Binder
	{
		private readonly Symbol _owner;

		public override Symbol ContainingMember => _owner ?? base.ContainingMember;

		public override ImmutableArray<Symbol> AdditionalContainingMembers => ImmutableArray<Symbol>.Empty;

		public override BindingLocation BindingLocation => BindingLocation.Attribute;

		internal EarlyWellKnownAttributeBinder(Symbol owner, Binder containingBinder)
			: base(containingBinder, true)
		{
			_owner = owner;
		}

		internal SourceAttributeData GetAttribute(AttributeSyntax node, NamedTypeSymbol boundAttributeType, out bool generatedDiagnostics)
		{
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(withDiagnostics: true, withDependencies: false);
			SourceAttributeData attribute = GetAttribute(node, boundAttributeType, instance);
			generatedDiagnostics = !instance.DiagnosticBag!.IsEmptyWithoutResolution;
			instance.Free();
			return attribute;
		}

		internal static bool CanBeValidAttributeArgument(ExpressionSyntax node, Binder memberAccessBinder)
		{
			switch (node.Kind())
			{
			case SyntaxKind.CharacterLiteralExpression:
			case SyntaxKind.TrueLiteralExpression:
			case SyntaxKind.FalseLiteralExpression:
			case SyntaxKind.NumericLiteralExpression:
			case SyntaxKind.DateLiteralExpression:
			case SyntaxKind.StringLiteralExpression:
			case SyntaxKind.NothingLiteralExpression:
				return true;
			case SyntaxKind.SimpleMemberAccessExpression:
			case SyntaxKind.PredefinedType:
			case SyntaxKind.IdentifierName:
			case SyntaxKind.GlobalName:
				return true;
			case SyntaxKind.ParenthesizedExpression:
				return true;
			case SyntaxKind.CTypeExpression:
			case SyntaxKind.DirectCastExpression:
			case SyntaxKind.TryCastExpression:
			case SyntaxKind.PredefinedCastExpression:
				return true;
			case SyntaxKind.UnaryPlusExpression:
			case SyntaxKind.UnaryMinusExpression:
			case SyntaxKind.NotExpression:
				return true;
			case SyntaxKind.AddExpression:
			case SyntaxKind.SubtractExpression:
			case SyntaxKind.MultiplyExpression:
			case SyntaxKind.DivideExpression:
			case SyntaxKind.IntegerDivideExpression:
			case SyntaxKind.ExponentiateExpression:
			case SyntaxKind.LeftShiftExpression:
			case SyntaxKind.RightShiftExpression:
			case SyntaxKind.ConcatenateExpression:
			case SyntaxKind.ModuloExpression:
			case SyntaxKind.EqualsExpression:
			case SyntaxKind.NotEqualsExpression:
			case SyntaxKind.LessThanExpression:
			case SyntaxKind.LessThanOrEqualExpression:
			case SyntaxKind.GreaterThanOrEqualExpression:
			case SyntaxKind.GreaterThanExpression:
			case SyntaxKind.OrExpression:
			case SyntaxKind.ExclusiveOrExpression:
			case SyntaxKind.AndExpression:
			case SyntaxKind.OrElseExpression:
			case SyntaxKind.AndAlsoExpression:
				return true;
			case SyntaxKind.BinaryConditionalExpression:
			case SyntaxKind.TernaryConditionalExpression:
				return true;
			case SyntaxKind.InvocationExpression:
				if (((InvocationExpressionSyntax)node).Expression is MemberAccessExpressionSyntax node2)
				{
					BoundExpression boundExpression = memberAccessBinder.BindExpression(node2, BindingDiagnosticBag.Discarded);
					if (boundExpression.HasErrors)
					{
						return false;
					}
					if (boundExpression is BoundMethodGroup boundMethodGroup && boundMethodGroup.Methods.Length == 1)
					{
						MethodSymbol methodSymbol = boundMethodGroup.Methods[0];
						VisualBasicCompilation compilation = memberAccessBinder.Compilation;
						if ((object)methodSymbol == compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Strings__ChrWInt32Char) || (object)methodSymbol == compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Strings__ChrInt32Char) || (object)methodSymbol == compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Strings__AscWCharInt32) || (object)methodSymbol == compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Strings__AscCharInt32))
						{
							return true;
						}
					}
				}
				return false;
			case SyntaxKind.GetTypeExpression:
			case SyntaxKind.ArrayCreationExpression:
			case SyntaxKind.CollectionInitializer:
				return true;
			default:
				return false;
			}
		}

		internal override LookupOptions BinderSpecificLookupOptions(LookupOptions options)
		{
			return base.ContainingBinder.BinderSpecificLookupOptions(options) | LookupOptions.IgnoreExtensionMethods;
		}
	}
}
