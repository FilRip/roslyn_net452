using System;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class EarlyWellKnownAttributeBinder : Binder
    {
        internal EarlyWellKnownAttributeBinder(Binder enclosing)
            : base(enclosing, enclosing.Flags | BinderFlags.EarlyAttributeBinding)
        {
        }

        internal CSharpAttributeData GetAttribute(AttributeSyntax node, NamedTypeSymbol boundAttributeType, out bool generatedDiagnostics)
        {
            BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag(DiagnosticBag.GetInstance());
            CSharpAttributeData attribute = base.GetAttribute(node, boundAttributeType, bindingDiagnosticBag);
            generatedDiagnostics = !bindingDiagnosticBag.DiagnosticBag!.IsEmptyWithoutResolution;
            bindingDiagnosticBag.Free();
            return attribute;
        }

        [Obsolete("EarlyWellKnownAttributeBinder has a better overload - GetAttribute(AttributeSyntax, NamedTypeSymbol, out bool)", true)]
        internal new CSharpAttributeData GetAttribute(AttributeSyntax node, NamedTypeSymbol boundAttributeType, BindingDiagnosticBag diagnostics)
        {
            diagnostics.Add(ErrorCode.ERR_InternalError, node.Location);
            return base.GetAttribute(node, boundAttributeType, diagnostics);
        }

        internal static bool CanBeValidAttributeArgument(ExpressionSyntax node, Binder typeBinder)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.ImplicitObjectCreationExpression:
                    {
                        BaseObjectCreationExpressionSyntax baseObjectCreationExpressionSyntax = (BaseObjectCreationExpressionSyntax)node;
                        if (baseObjectCreationExpressionSyntax.Initializer == null)
                        {
                            return (baseObjectCreationExpressionSyntax.ArgumentList?.Arguments.Count ?? 0) == 0;
                        }
                        return false;
                    }
                case SyntaxKind.IdentifierName:
                case SyntaxKind.QualifiedName:
                case SyntaxKind.GenericName:
                case SyntaxKind.AliasQualifiedName:
                case SyntaxKind.PredefinedType:
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.ConditionalExpression:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.CastExpression:
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.CharacterLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.TypeOfExpression:
                case SyntaxKind.SizeOfExpression:
                case SyntaxKind.CheckedExpression:
                case SyntaxKind.UncheckedExpression:
                case SyntaxKind.DefaultExpression:
                    return true;
                default:
                    return false;
            }
        }
    }
}
