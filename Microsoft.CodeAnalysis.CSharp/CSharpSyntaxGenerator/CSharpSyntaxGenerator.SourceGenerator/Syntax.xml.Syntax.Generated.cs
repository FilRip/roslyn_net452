#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{


    /// <summary>Provides the base class from which the classes that represent name syntax nodes are derived. This is an abstract class.</summary>
    public abstract partial class NameSyntax : TypeSyntax
    {
        internal NameSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    /// <summary>Provides the base class from which the classes that represent simple name syntax nodes are derived. This is an abstract class.</summary>
    public abstract partial class SimpleNameSyntax : NameSyntax
    {
        internal SimpleNameSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the identifier of the simple name.</summary>
        public abstract SyntaxToken Identifier { get; }
        public SimpleNameSyntax WithIdentifier(SyntaxToken identifier) => WithIdentifierCore(identifier);
        internal abstract SimpleNameSyntax WithIdentifierCore(SyntaxToken identifier);
    }

    /// <summary>Class which represents the syntax node for identifier name.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.IdentifierName"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class IdentifierNameSyntax : SimpleNameSyntax
    {

        internal IdentifierNameSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the keyword for the kind of the identifier name.</summary>
        public override SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.IdentifierNameSyntax)this.Green).identifier, Position, 0);

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitIdentifierName(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitIdentifierName(this);

        public IdentifierNameSyntax Update(SyntaxToken identifier)
        {
            if (identifier != this.Identifier)
            {
                var newNode = SyntaxFactory.IdentifierName(identifier);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override SimpleNameSyntax WithIdentifierCore(SyntaxToken identifier) => WithIdentifier(identifier);
        public new IdentifierNameSyntax WithIdentifier(SyntaxToken identifier) => Update(identifier);
    }

    /// <summary>Class which represents the syntax node for qualified name.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.QualifiedName"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class QualifiedNameSyntax : NameSyntax
    {
        private NameSyntax? left;
        private SimpleNameSyntax? right;

        internal QualifiedNameSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>NameSyntax node representing the name on the left side of the dot token of the qualified name.</summary>
        public NameSyntax Left => GetRedAtZero(ref this.left)!;

        /// <summary>SyntaxToken representing the dot.</summary>
        public SyntaxToken DotToken => new SyntaxToken(this, ((Syntax.InternalSyntax.QualifiedNameSyntax)this.Green).dotToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>SimpleNameSyntax node representing the name on the right side of the dot token of the qualified name.</summary>
        public SimpleNameSyntax Right => GetRed(ref this.right, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.left)!,
                2 => GetRed(ref this.right, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.left,
                2 => this.right,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitQualifiedName(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitQualifiedName(this);

        public QualifiedNameSyntax Update(NameSyntax left, SyntaxToken dotToken, SimpleNameSyntax right)
        {
            if (left != this.Left || dotToken != this.DotToken || right != this.Right)
            {
                var newNode = SyntaxFactory.QualifiedName(left, dotToken, right);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public QualifiedNameSyntax WithLeft(NameSyntax left) => Update(left, this.DotToken, this.Right);
        public QualifiedNameSyntax WithDotToken(SyntaxToken dotToken) => Update(this.Left, dotToken, this.Right);
        public QualifiedNameSyntax WithRight(SimpleNameSyntax right) => Update(this.Left, this.DotToken, right);
    }

    /// <summary>Class which represents the syntax node for generic name.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.GenericName"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class GenericNameSyntax : SimpleNameSyntax
    {
        private TypeArgumentListSyntax? typeArgumentList;

        internal GenericNameSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the name of the identifier of the generic name.</summary>
        public override SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.GenericNameSyntax)this.Green).identifier, Position, 0);

        /// <summary>TypeArgumentListSyntax node representing the list of type arguments of the generic name.</summary>
        public TypeArgumentListSyntax TypeArgumentList => GetRed(ref this.typeArgumentList, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.typeArgumentList, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.typeArgumentList : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitGenericName(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitGenericName(this);

        public GenericNameSyntax Update(SyntaxToken identifier, TypeArgumentListSyntax typeArgumentList)
        {
            if (identifier != this.Identifier || typeArgumentList != this.TypeArgumentList)
            {
                var newNode = SyntaxFactory.GenericName(identifier, typeArgumentList);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override SimpleNameSyntax WithIdentifierCore(SyntaxToken identifier) => WithIdentifier(identifier);
        public new GenericNameSyntax WithIdentifier(SyntaxToken identifier) => Update(identifier, this.TypeArgumentList);
        public GenericNameSyntax WithTypeArgumentList(TypeArgumentListSyntax typeArgumentList) => Update(this.Identifier, typeArgumentList);

        public GenericNameSyntax AddTypeArgumentListArguments(params TypeSyntax[] items) => WithTypeArgumentList(this.TypeArgumentList.WithArguments(this.TypeArgumentList.Arguments.AddRange(items)));
    }

    /// <summary>Class which represents the syntax node for type argument list.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.TypeArgumentList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class TypeArgumentListSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? arguments;

        internal TypeArgumentListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing less than.</summary>
        public SyntaxToken LessThanToken => new SyntaxToken(this, ((Syntax.InternalSyntax.TypeArgumentListSyntax)this.Green).lessThanToken, Position, 0);

        /// <summary>SeparatedSyntaxList of TypeSyntax node representing the type arguments.</summary>
        public SeparatedSyntaxList<TypeSyntax> Arguments
        {
            get
            {
                var red = GetRed(ref this.arguments, 1);
                return red != null ? new SeparatedSyntaxList<TypeSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>SyntaxToken representing greater than.</summary>
        public SyntaxToken GreaterThanToken => new SyntaxToken(this, ((Syntax.InternalSyntax.TypeArgumentListSyntax)this.Green).greaterThanToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.arguments, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.arguments : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitTypeArgumentList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeArgumentList(this);

        public TypeArgumentListSyntax Update(SyntaxToken lessThanToken, SeparatedSyntaxList<TypeSyntax> arguments, SyntaxToken greaterThanToken)
        {
            if (lessThanToken != this.LessThanToken || arguments != this.Arguments || greaterThanToken != this.GreaterThanToken)
            {
                var newNode = SyntaxFactory.TypeArgumentList(lessThanToken, arguments, greaterThanToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public TypeArgumentListSyntax WithLessThanToken(SyntaxToken lessThanToken) => Update(lessThanToken, this.Arguments, this.GreaterThanToken);
        public TypeArgumentListSyntax WithArguments(SeparatedSyntaxList<TypeSyntax> arguments) => Update(this.LessThanToken, arguments, this.GreaterThanToken);
        public TypeArgumentListSyntax WithGreaterThanToken(SyntaxToken greaterThanToken) => Update(this.LessThanToken, this.Arguments, greaterThanToken);

        public TypeArgumentListSyntax AddArguments(params TypeSyntax[] items) => WithArguments(this.Arguments.AddRange(items));
    }

    /// <summary>Class which represents the syntax node for alias qualified name.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.AliasQualifiedName"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class AliasQualifiedNameSyntax : NameSyntax
    {
        private IdentifierNameSyntax? alias;
        private SimpleNameSyntax? name;

        internal AliasQualifiedNameSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>IdentifierNameSyntax node representing the name of the alias</summary>
        public IdentifierNameSyntax Alias => GetRedAtZero(ref this.alias)!;

        /// <summary>SyntaxToken representing colon colon.</summary>
        public SyntaxToken ColonColonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.AliasQualifiedNameSyntax)this.Green).colonColonToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>SimpleNameSyntax node representing the name that is being alias qualified.</summary>
        public SimpleNameSyntax Name => GetRed(ref this.name, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.alias)!,
                2 => GetRed(ref this.name, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.alias,
                2 => this.name,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitAliasQualifiedName(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAliasQualifiedName(this);

        public AliasQualifiedNameSyntax Update(IdentifierNameSyntax alias, SyntaxToken colonColonToken, SimpleNameSyntax name)
        {
            if (alias != this.Alias || colonColonToken != this.ColonColonToken || name != this.Name)
            {
                var newNode = SyntaxFactory.AliasQualifiedName(alias, colonColonToken, name);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public AliasQualifiedNameSyntax WithAlias(IdentifierNameSyntax alias) => Update(alias, this.ColonColonToken, this.Name);
        public AliasQualifiedNameSyntax WithColonColonToken(SyntaxToken colonColonToken) => Update(this.Alias, colonColonToken, this.Name);
        public AliasQualifiedNameSyntax WithName(SimpleNameSyntax name) => Update(this.Alias, this.ColonColonToken, name);
    }

    /// <summary>Provides the base class from which the classes that represent type syntax nodes are derived. This is an abstract class.</summary>
    public abstract partial class TypeSyntax : ExpressionSyntax
    {
        internal TypeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    /// <summary>Class which represents the syntax node for predefined types.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.PredefinedType"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class PredefinedTypeSyntax : TypeSyntax
    {

        internal PredefinedTypeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken which represents the keyword corresponding to the predefined type.</summary>
        public SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.PredefinedTypeSyntax)this.Green).keyword, Position, 0);

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitPredefinedType(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPredefinedType(this);

        public PredefinedTypeSyntax Update(SyntaxToken keyword)
        {
            if (keyword != this.Keyword)
            {
                var newNode = SyntaxFactory.PredefinedType(keyword);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public PredefinedTypeSyntax WithKeyword(SyntaxToken keyword) => Update(keyword);
    }

    /// <summary>Class which represents the syntax node for the array type.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ArrayType"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ArrayTypeSyntax : TypeSyntax
    {
        private TypeSyntax? elementType;
        private SyntaxNode? rankSpecifiers;

        internal ArrayTypeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>TypeSyntax node representing the type of the element of the array.</summary>
        public TypeSyntax ElementType => GetRedAtZero(ref this.elementType)!;

        /// <summary>SyntaxList of ArrayRankSpecifierSyntax nodes representing the list of rank specifiers for the array.</summary>
        public SyntaxList<ArrayRankSpecifierSyntax> RankSpecifiers => new SyntaxList<ArrayRankSpecifierSyntax>(GetRed(ref this.rankSpecifiers, 1));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.elementType)!,
                1 => GetRed(ref this.rankSpecifiers, 1)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.elementType,
                1 => this.rankSpecifiers,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitArrayType(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitArrayType(this);

        public ArrayTypeSyntax Update(TypeSyntax elementType, SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers)
        {
            if (elementType != this.ElementType || rankSpecifiers != this.RankSpecifiers)
            {
                var newNode = SyntaxFactory.ArrayType(elementType, rankSpecifiers);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ArrayTypeSyntax WithElementType(TypeSyntax elementType) => Update(elementType, this.RankSpecifiers);
        public ArrayTypeSyntax WithRankSpecifiers(SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers) => Update(this.ElementType, rankSpecifiers);

        public ArrayTypeSyntax AddRankSpecifiers(params ArrayRankSpecifierSyntax[] items) => WithRankSpecifiers(this.RankSpecifiers.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ArrayRankSpecifier"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ArrayRankSpecifierSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? sizes;

        internal ArrayRankSpecifierSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ArrayRankSpecifierSyntax)this.Green).openBracketToken, Position, 0);

        public SeparatedSyntaxList<ExpressionSyntax> Sizes
        {
            get
            {
                var red = GetRed(ref this.sizes, 1);
                return red != null ? new SeparatedSyntaxList<ExpressionSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ArrayRankSpecifierSyntax)this.Green).closeBracketToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.sizes, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.sizes : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitArrayRankSpecifier(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitArrayRankSpecifier(this);

        public ArrayRankSpecifierSyntax Update(SyntaxToken openBracketToken, SeparatedSyntaxList<ExpressionSyntax> sizes, SyntaxToken closeBracketToken)
        {
            if (openBracketToken != this.OpenBracketToken || sizes != this.Sizes || closeBracketToken != this.CloseBracketToken)
            {
                var newNode = SyntaxFactory.ArrayRankSpecifier(openBracketToken, sizes, closeBracketToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ArrayRankSpecifierSyntax WithOpenBracketToken(SyntaxToken openBracketToken) => Update(openBracketToken, this.Sizes, this.CloseBracketToken);
        public ArrayRankSpecifierSyntax WithSizes(SeparatedSyntaxList<ExpressionSyntax> sizes) => Update(this.OpenBracketToken, sizes, this.CloseBracketToken);
        public ArrayRankSpecifierSyntax WithCloseBracketToken(SyntaxToken closeBracketToken) => Update(this.OpenBracketToken, this.Sizes, closeBracketToken);

        public ArrayRankSpecifierSyntax AddSizes(params ExpressionSyntax[] items) => WithSizes(this.Sizes.AddRange(items));
    }

    /// <summary>Class which represents the syntax node for pointer type.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.PointerType"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class PointerTypeSyntax : TypeSyntax
    {
        private TypeSyntax? elementType;

        internal PointerTypeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>TypeSyntax node that represents the element type of the pointer.</summary>
        public TypeSyntax ElementType => GetRedAtZero(ref this.elementType)!;

        /// <summary>SyntaxToken representing the asterisk.</summary>
        public SyntaxToken AsteriskToken => new SyntaxToken(this, ((Syntax.InternalSyntax.PointerTypeSyntax)this.Green).asteriskToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.elementType)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.elementType : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitPointerType(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPointerType(this);

        public PointerTypeSyntax Update(TypeSyntax elementType, SyntaxToken asteriskToken)
        {
            if (elementType != this.ElementType || asteriskToken != this.AsteriskToken)
            {
                var newNode = SyntaxFactory.PointerType(elementType, asteriskToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public PointerTypeSyntax WithElementType(TypeSyntax elementType) => Update(elementType, this.AsteriskToken);
        public PointerTypeSyntax WithAsteriskToken(SyntaxToken asteriskToken) => Update(this.ElementType, asteriskToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.FunctionPointerType"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class FunctionPointerTypeSyntax : TypeSyntax
    {
        private FunctionPointerCallingConventionSyntax? callingConvention;
        private FunctionPointerParameterListSyntax? parameterList;

        internal FunctionPointerTypeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the delegate keyword.</summary>
        public SyntaxToken DelegateKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.FunctionPointerTypeSyntax)this.Green).delegateKeyword, Position, 0);

        /// <summary>SyntaxToken representing the asterisk.</summary>
        public SyntaxToken AsteriskToken => new SyntaxToken(this, ((Syntax.InternalSyntax.FunctionPointerTypeSyntax)this.Green).asteriskToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>Node representing the optional calling convention.</summary>
        public FunctionPointerCallingConventionSyntax? CallingConvention => GetRed(ref this.callingConvention, 2);

        /// <summary>List of the parameter types and return type of the function pointer.</summary>
        public FunctionPointerParameterListSyntax ParameterList => GetRed(ref this.parameterList, 3)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                2 => GetRed(ref this.callingConvention, 2),
                3 => GetRed(ref this.parameterList, 3)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                2 => this.callingConvention,
                3 => this.parameterList,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitFunctionPointerType(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionPointerType(this);

        public FunctionPointerTypeSyntax Update(SyntaxToken delegateKeyword, SyntaxToken asteriskToken, FunctionPointerCallingConventionSyntax? callingConvention, FunctionPointerParameterListSyntax parameterList)
        {
            if (delegateKeyword != this.DelegateKeyword || asteriskToken != this.AsteriskToken || callingConvention != this.CallingConvention || parameterList != this.ParameterList)
            {
                var newNode = SyntaxFactory.FunctionPointerType(delegateKeyword, asteriskToken, callingConvention, parameterList);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public FunctionPointerTypeSyntax WithDelegateKeyword(SyntaxToken delegateKeyword) => Update(delegateKeyword, this.AsteriskToken, this.CallingConvention, this.ParameterList);
        public FunctionPointerTypeSyntax WithAsteriskToken(SyntaxToken asteriskToken) => Update(this.DelegateKeyword, asteriskToken, this.CallingConvention, this.ParameterList);
        public FunctionPointerTypeSyntax WithCallingConvention(FunctionPointerCallingConventionSyntax? callingConvention) => Update(this.DelegateKeyword, this.AsteriskToken, callingConvention, this.ParameterList);
        public FunctionPointerTypeSyntax WithParameterList(FunctionPointerParameterListSyntax parameterList) => Update(this.DelegateKeyword, this.AsteriskToken, this.CallingConvention, parameterList);

        public FunctionPointerTypeSyntax AddParameterListParameters(params FunctionPointerParameterSyntax[] items) => WithParameterList(this.ParameterList.WithParameters(this.ParameterList.Parameters.AddRange(items)));
    }

    /// <summary>Function pointer parameter list syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.FunctionPointerParameterList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class FunctionPointerParameterListSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? parameters;

        internal FunctionPointerParameterListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the less than token.</summary>
        public SyntaxToken LessThanToken => new SyntaxToken(this, ((Syntax.InternalSyntax.FunctionPointerParameterListSyntax)this.Green).lessThanToken, Position, 0);

        /// <summary>SeparatedSyntaxList of ParameterSyntaxes representing the list of parameters and return type.</summary>
        public SeparatedSyntaxList<FunctionPointerParameterSyntax> Parameters
        {
            get
            {
                var red = GetRed(ref this.parameters, 1);
                return red != null ? new SeparatedSyntaxList<FunctionPointerParameterSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>SyntaxToken representing the greater than token.</summary>
        public SyntaxToken GreaterThanToken => new SyntaxToken(this, ((Syntax.InternalSyntax.FunctionPointerParameterListSyntax)this.Green).greaterThanToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.parameters, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.parameters : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitFunctionPointerParameterList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionPointerParameterList(this);

        public FunctionPointerParameterListSyntax Update(SyntaxToken lessThanToken, SeparatedSyntaxList<FunctionPointerParameterSyntax> parameters, SyntaxToken greaterThanToken)
        {
            if (lessThanToken != this.LessThanToken || parameters != this.Parameters || greaterThanToken != this.GreaterThanToken)
            {
                var newNode = SyntaxFactory.FunctionPointerParameterList(lessThanToken, parameters, greaterThanToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public FunctionPointerParameterListSyntax WithLessThanToken(SyntaxToken lessThanToken) => Update(lessThanToken, this.Parameters, this.GreaterThanToken);
        public FunctionPointerParameterListSyntax WithParameters(SeparatedSyntaxList<FunctionPointerParameterSyntax> parameters) => Update(this.LessThanToken, parameters, this.GreaterThanToken);
        public FunctionPointerParameterListSyntax WithGreaterThanToken(SyntaxToken greaterThanToken) => Update(this.LessThanToken, this.Parameters, greaterThanToken);

        public FunctionPointerParameterListSyntax AddParameters(params FunctionPointerParameterSyntax[] items) => WithParameters(this.Parameters.AddRange(items));
    }

    /// <summary>Function pointer calling convention syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.FunctionPointerCallingConvention"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class FunctionPointerCallingConventionSyntax : CSharpSyntaxNode
    {
        private FunctionPointerUnmanagedCallingConventionListSyntax? unmanagedCallingConventionList;

        internal FunctionPointerCallingConventionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing whether the calling convention is managed or unmanaged.</summary>
        public SyntaxToken ManagedOrUnmanagedKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.FunctionPointerCallingConventionSyntax)this.Green).managedOrUnmanagedKeyword, Position, 0);

        /// <summary>Optional list of identifiers that will contribute to an unmanaged calling convention.</summary>
        public FunctionPointerUnmanagedCallingConventionListSyntax? UnmanagedCallingConventionList => GetRed(ref this.unmanagedCallingConventionList, 1);

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.unmanagedCallingConventionList, 1) : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.unmanagedCallingConventionList : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitFunctionPointerCallingConvention(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionPointerCallingConvention(this);

        public FunctionPointerCallingConventionSyntax Update(SyntaxToken managedOrUnmanagedKeyword, FunctionPointerUnmanagedCallingConventionListSyntax? unmanagedCallingConventionList)
        {
            if (managedOrUnmanagedKeyword != this.ManagedOrUnmanagedKeyword || unmanagedCallingConventionList != this.UnmanagedCallingConventionList)
            {
                var newNode = SyntaxFactory.FunctionPointerCallingConvention(managedOrUnmanagedKeyword, unmanagedCallingConventionList);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public FunctionPointerCallingConventionSyntax WithManagedOrUnmanagedKeyword(SyntaxToken managedOrUnmanagedKeyword) => Update(managedOrUnmanagedKeyword, this.UnmanagedCallingConventionList);
        public FunctionPointerCallingConventionSyntax WithUnmanagedCallingConventionList(FunctionPointerUnmanagedCallingConventionListSyntax? unmanagedCallingConventionList) => Update(this.ManagedOrUnmanagedKeyword, unmanagedCallingConventionList);

        public FunctionPointerCallingConventionSyntax AddUnmanagedCallingConventionListCallingConventions(params FunctionPointerUnmanagedCallingConventionSyntax[] items)
        {
            var unmanagedCallingConventionList = this.UnmanagedCallingConventionList ?? SyntaxFactory.FunctionPointerUnmanagedCallingConventionList();
            return WithUnmanagedCallingConventionList(unmanagedCallingConventionList.WithCallingConventions(unmanagedCallingConventionList.CallingConventions.AddRange(items)));
        }
    }

    /// <summary>Function pointer calling convention syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.FunctionPointerUnmanagedCallingConventionList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class FunctionPointerUnmanagedCallingConventionListSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? callingConventions;

        internal FunctionPointerUnmanagedCallingConventionListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing open bracket.</summary>
        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.FunctionPointerUnmanagedCallingConventionListSyntax)this.Green).openBracketToken, Position, 0);

        /// <summary>SeparatedSyntaxList of calling convention identifiers.</summary>
        public SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax> CallingConventions
        {
            get
            {
                var red = GetRed(ref this.callingConventions, 1);
                return red != null ? new SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>SyntaxToken representing close bracket.</summary>
        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.FunctionPointerUnmanagedCallingConventionListSyntax)this.Green).closeBracketToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.callingConventions, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.callingConventions : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitFunctionPointerUnmanagedCallingConventionList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionPointerUnmanagedCallingConventionList(this);

        public FunctionPointerUnmanagedCallingConventionListSyntax Update(SyntaxToken openBracketToken, SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax> callingConventions, SyntaxToken closeBracketToken)
        {
            if (openBracketToken != this.OpenBracketToken || callingConventions != this.CallingConventions || closeBracketToken != this.CloseBracketToken)
            {
                var newNode = SyntaxFactory.FunctionPointerUnmanagedCallingConventionList(openBracketToken, callingConventions, closeBracketToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public FunctionPointerUnmanagedCallingConventionListSyntax WithOpenBracketToken(SyntaxToken openBracketToken) => Update(openBracketToken, this.CallingConventions, this.CloseBracketToken);
        public FunctionPointerUnmanagedCallingConventionListSyntax WithCallingConventions(SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax> callingConventions) => Update(this.OpenBracketToken, callingConventions, this.CloseBracketToken);
        public FunctionPointerUnmanagedCallingConventionListSyntax WithCloseBracketToken(SyntaxToken closeBracketToken) => Update(this.OpenBracketToken, this.CallingConventions, closeBracketToken);

        public FunctionPointerUnmanagedCallingConventionListSyntax AddCallingConventions(params FunctionPointerUnmanagedCallingConventionSyntax[] items) => WithCallingConventions(this.CallingConventions.AddRange(items));
    }

    /// <summary>Individual function pointer unmanaged calling convention.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.FunctionPointerUnmanagedCallingConvention"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class FunctionPointerUnmanagedCallingConventionSyntax : CSharpSyntaxNode
    {

        internal FunctionPointerUnmanagedCallingConventionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the calling convention identifier.</summary>
        public SyntaxToken Name => new SyntaxToken(this, ((Syntax.InternalSyntax.FunctionPointerUnmanagedCallingConventionSyntax)this.Green).name, Position, 0);

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitFunctionPointerUnmanagedCallingConvention(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionPointerUnmanagedCallingConvention(this);

        public FunctionPointerUnmanagedCallingConventionSyntax Update(SyntaxToken name)
        {
            if (name != this.Name)
            {
                var newNode = SyntaxFactory.FunctionPointerUnmanagedCallingConvention(name);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public FunctionPointerUnmanagedCallingConventionSyntax WithName(SyntaxToken name) => Update(name);
    }

    /// <summary>Class which represents the syntax node for a nullable type.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.NullableType"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class NullableTypeSyntax : TypeSyntax
    {
        private TypeSyntax? elementType;

        internal NullableTypeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>TypeSyntax node representing the type of the element.</summary>
        public TypeSyntax ElementType => GetRedAtZero(ref this.elementType)!;

        /// <summary>SyntaxToken representing the question mark.</summary>
        public SyntaxToken QuestionToken => new SyntaxToken(this, ((Syntax.InternalSyntax.NullableTypeSyntax)this.Green).questionToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.elementType)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.elementType : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitNullableType(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitNullableType(this);

        public NullableTypeSyntax Update(TypeSyntax elementType, SyntaxToken questionToken)
        {
            if (elementType != this.ElementType || questionToken != this.QuestionToken)
            {
                var newNode = SyntaxFactory.NullableType(elementType, questionToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public NullableTypeSyntax WithElementType(TypeSyntax elementType) => Update(elementType, this.QuestionToken);
        public NullableTypeSyntax WithQuestionToken(SyntaxToken questionToken) => Update(this.ElementType, questionToken);
    }

    /// <summary>Class which represents the syntax node for tuple type.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.TupleType"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class TupleTypeSyntax : TypeSyntax
    {
        private SyntaxNode? elements;

        internal TupleTypeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the open parenthesis.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.TupleTypeSyntax)this.Green).openParenToken, Position, 0);

        public SeparatedSyntaxList<TupleElementSyntax> Elements
        {
            get
            {
                var red = GetRed(ref this.elements, 1);
                return red != null ? new SeparatedSyntaxList<TupleElementSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>SyntaxToken representing the close parenthesis.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.TupleTypeSyntax)this.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.elements, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.elements : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitTupleType(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTupleType(this);

        public TupleTypeSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<TupleElementSyntax> elements, SyntaxToken closeParenToken)
        {
            if (openParenToken != this.OpenParenToken || elements != this.Elements || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.TupleType(openParenToken, elements, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public TupleTypeSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(openParenToken, this.Elements, this.CloseParenToken);
        public TupleTypeSyntax WithElements(SeparatedSyntaxList<TupleElementSyntax> elements) => Update(this.OpenParenToken, elements, this.CloseParenToken);
        public TupleTypeSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.OpenParenToken, this.Elements, closeParenToken);

        public TupleTypeSyntax AddElements(params TupleElementSyntax[] items) => WithElements(this.Elements.AddRange(items));
    }

    /// <summary>Tuple type element.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.TupleElement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class TupleElementSyntax : CSharpSyntaxNode
    {
        private TypeSyntax? type;

        internal TupleElementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the type of the tuple element.</summary>
        public TypeSyntax Type => GetRedAtZero(ref this.type)!;

        /// <summary>Gets the name of the tuple element.</summary>
        public SyntaxToken Identifier
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.TupleElementSyntax)this.Green).identifier;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.type)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.type : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitTupleElement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTupleElement(this);

        public TupleElementSyntax Update(TypeSyntax type, SyntaxToken identifier)
        {
            if (type != this.Type || identifier != this.Identifier)
            {
                var newNode = SyntaxFactory.TupleElement(type, identifier);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public TupleElementSyntax WithType(TypeSyntax type) => Update(type, this.Identifier);
        public TupleElementSyntax WithIdentifier(SyntaxToken identifier) => Update(this.Type, identifier);
    }

    /// <summary>Class which represents a placeholder in the type argument list of an unbound generic type.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.OmittedTypeArgument"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class OmittedTypeArgumentSyntax : TypeSyntax
    {

        internal OmittedTypeArgumentSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the omitted type argument.</summary>
        public SyntaxToken OmittedTypeArgumentToken => new SyntaxToken(this, ((Syntax.InternalSyntax.OmittedTypeArgumentSyntax)this.Green).omittedTypeArgumentToken, Position, 0);

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitOmittedTypeArgument(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitOmittedTypeArgument(this);

        public OmittedTypeArgumentSyntax Update(SyntaxToken omittedTypeArgumentToken)
        {
            if (omittedTypeArgumentToken != this.OmittedTypeArgumentToken)
            {
                var newNode = SyntaxFactory.OmittedTypeArgument(omittedTypeArgumentToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public OmittedTypeArgumentSyntax WithOmittedTypeArgumentToken(SyntaxToken omittedTypeArgumentToken) => Update(omittedTypeArgumentToken);
    }

    /// <summary>The ref modifier of a method's return value or a local.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.RefType"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class RefTypeSyntax : TypeSyntax
    {
        private TypeSyntax? type;

        internal RefTypeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken RefKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.RefTypeSyntax)this.Green).refKeyword, Position, 0);

        /// <summary>Gets the optional "readonly" keyword.</summary>
        public SyntaxToken ReadOnlyKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.RefTypeSyntax)this.Green).readOnlyKeyword;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public TypeSyntax Type => GetRed(ref this.type, 2)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.type, 2)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.type : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitRefType(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRefType(this);

        public RefTypeSyntax Update(SyntaxToken refKeyword, SyntaxToken readOnlyKeyword, TypeSyntax type)
        {
            if (refKeyword != this.RefKeyword || readOnlyKeyword != this.ReadOnlyKeyword || type != this.Type)
            {
                var newNode = SyntaxFactory.RefType(refKeyword, readOnlyKeyword, type);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public RefTypeSyntax WithRefKeyword(SyntaxToken refKeyword) => Update(refKeyword, this.ReadOnlyKeyword, this.Type);
        public RefTypeSyntax WithReadOnlyKeyword(SyntaxToken readOnlyKeyword) => Update(this.RefKeyword, readOnlyKeyword, this.Type);
        public RefTypeSyntax WithType(TypeSyntax type) => Update(this.RefKeyword, this.ReadOnlyKeyword, type);
    }

    public abstract partial class ExpressionOrPatternSyntax : CSharpSyntaxNode
    {
        internal ExpressionOrPatternSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    /// <summary>Provides the base class from which the classes that represent expression syntax nodes are derived. This is an abstract class.</summary>
    public abstract partial class ExpressionSyntax : ExpressionOrPatternSyntax
    {
        internal ExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    /// <summary>Class which represents the syntax node for parenthesized expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ParenthesizedExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        internal ParenthesizedExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the open parenthesis.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ParenthesizedExpressionSyntax)this.Green).openParenToken, Position, 0);

        /// <summary>ExpressionSyntax node representing the expression enclosed within the parenthesis.</summary>
        public ExpressionSyntax Expression => GetRed(ref this.expression, 1)!;

        /// <summary>SyntaxToken representing the close parenthesis.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ParenthesizedExpressionSyntax)this.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.expression, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.expression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitParenthesizedExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitParenthesizedExpression(this);

        public ParenthesizedExpressionSyntax Update(SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            if (openParenToken != this.OpenParenToken || expression != this.Expression || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.ParenthesizedExpression(openParenToken, expression, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ParenthesizedExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(openParenToken, this.Expression, this.CloseParenToken);
        public ParenthesizedExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(this.OpenParenToken, expression, this.CloseParenToken);
        public ParenthesizedExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.OpenParenToken, this.Expression, closeParenToken);
    }

    /// <summary>Class which represents the syntax node for tuple expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.TupleExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class TupleExpressionSyntax : ExpressionSyntax
    {
        private SyntaxNode? arguments;

        internal TupleExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the open parenthesis.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.TupleExpressionSyntax)this.Green).openParenToken, Position, 0);

        /// <summary>SeparatedSyntaxList of ArgumentSyntax representing the list of arguments.</summary>
        public SeparatedSyntaxList<ArgumentSyntax> Arguments
        {
            get
            {
                var red = GetRed(ref this.arguments, 1);
                return red != null ? new SeparatedSyntaxList<ArgumentSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>SyntaxToken representing the close parenthesis.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.TupleExpressionSyntax)this.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.arguments, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.arguments : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitTupleExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTupleExpression(this);

        public TupleExpressionSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<ArgumentSyntax> arguments, SyntaxToken closeParenToken)
        {
            if (openParenToken != this.OpenParenToken || arguments != this.Arguments || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.TupleExpression(openParenToken, arguments, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public TupleExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(openParenToken, this.Arguments, this.CloseParenToken);
        public TupleExpressionSyntax WithArguments(SeparatedSyntaxList<ArgumentSyntax> arguments) => Update(this.OpenParenToken, arguments, this.CloseParenToken);
        public TupleExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.OpenParenToken, this.Arguments, closeParenToken);

        public TupleExpressionSyntax AddArguments(params ArgumentSyntax[] items) => WithArguments(this.Arguments.AddRange(items));
    }

    /// <summary>Class which represents the syntax node for prefix unary expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.UnaryPlusExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.UnaryMinusExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.BitwiseNotExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.LogicalNotExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.PreIncrementExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.PreDecrementExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.AddressOfExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.PointerIndirectionExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.IndexExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class PrefixUnaryExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? operand;

        internal PrefixUnaryExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the kind of the operator of the prefix unary expression.</summary>
        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Syntax.InternalSyntax.PrefixUnaryExpressionSyntax)this.Green).operatorToken, Position, 0);

        /// <summary>ExpressionSyntax representing the operand of the prefix unary expression.</summary>
        public ExpressionSyntax Operand => GetRed(ref this.operand, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.operand, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.operand : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitPrefixUnaryExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPrefixUnaryExpression(this);

        public PrefixUnaryExpressionSyntax Update(SyntaxToken operatorToken, ExpressionSyntax operand)
        {
            if (operatorToken != this.OperatorToken || operand != this.Operand)
            {
                var newNode = SyntaxFactory.PrefixUnaryExpression(this.Kind(), operatorToken, operand);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public PrefixUnaryExpressionSyntax WithOperatorToken(SyntaxToken operatorToken) => Update(operatorToken, this.Operand);
        public PrefixUnaryExpressionSyntax WithOperand(ExpressionSyntax operand) => Update(this.OperatorToken, operand);
    }

    /// <summary>Class which represents the syntax node for an "await" expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.AwaitExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class AwaitExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        internal AwaitExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the kind "await" keyword.</summary>
        public SyntaxToken AwaitKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.AwaitExpressionSyntax)this.Green).awaitKeyword, Position, 0);

        /// <summary>ExpressionSyntax representing the operand of the "await" operator.</summary>
        public ExpressionSyntax Expression => GetRed(ref this.expression, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.expression, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.expression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitAwaitExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAwaitExpression(this);

        public AwaitExpressionSyntax Update(SyntaxToken awaitKeyword, ExpressionSyntax expression)
        {
            if (awaitKeyword != this.AwaitKeyword || expression != this.Expression)
            {
                var newNode = SyntaxFactory.AwaitExpression(awaitKeyword, expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public AwaitExpressionSyntax WithAwaitKeyword(SyntaxToken awaitKeyword) => Update(awaitKeyword, this.Expression);
        public AwaitExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(this.AwaitKeyword, expression);
    }

    /// <summary>Class which represents the syntax node for postfix unary expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.PostIncrementExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.PostDecrementExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.SuppressNullableWarningExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class PostfixUnaryExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? operand;

        internal PostfixUnaryExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>ExpressionSyntax representing the operand of the postfix unary expression.</summary>
        public ExpressionSyntax Operand => GetRedAtZero(ref this.operand)!;

        /// <summary>SyntaxToken representing the kind of the operator of the postfix unary expression.</summary>
        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Syntax.InternalSyntax.PostfixUnaryExpressionSyntax)this.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.operand)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.operand : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitPostfixUnaryExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPostfixUnaryExpression(this);

        public PostfixUnaryExpressionSyntax Update(ExpressionSyntax operand, SyntaxToken operatorToken)
        {
            if (operand != this.Operand || operatorToken != this.OperatorToken)
            {
                var newNode = SyntaxFactory.PostfixUnaryExpression(this.Kind(), operand, operatorToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public PostfixUnaryExpressionSyntax WithOperand(ExpressionSyntax operand) => Update(operand, this.OperatorToken);
        public PostfixUnaryExpressionSyntax WithOperatorToken(SyntaxToken operatorToken) => Update(this.Operand, operatorToken);
    }

    /// <summary>Class which represents the syntax node for member access expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.SimpleMemberAccessExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.PointerMemberAccessExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class MemberAccessExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;
        private SimpleNameSyntax? name;

        internal MemberAccessExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>ExpressionSyntax node representing the object that the member belongs to.</summary>
        public ExpressionSyntax Expression => GetRedAtZero(ref this.expression)!;

        /// <summary>SyntaxToken representing the kind of the operator in the member access expression.</summary>
        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Syntax.InternalSyntax.MemberAccessExpressionSyntax)this.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>SimpleNameSyntax node representing the member being accessed.</summary>
        public SimpleNameSyntax Name => GetRed(ref this.name, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.expression)!,
                2 => GetRed(ref this.name, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.expression,
                2 => this.name,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitMemberAccessExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitMemberAccessExpression(this);

        public MemberAccessExpressionSyntax Update(ExpressionSyntax expression, SyntaxToken operatorToken, SimpleNameSyntax name)
        {
            if (expression != this.Expression || operatorToken != this.OperatorToken || name != this.Name)
            {
                var newNode = SyntaxFactory.MemberAccessExpression(this.Kind(), expression, operatorToken, name);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public MemberAccessExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(expression, this.OperatorToken, this.Name);
        public MemberAccessExpressionSyntax WithOperatorToken(SyntaxToken operatorToken) => Update(this.Expression, operatorToken, this.Name);
        public MemberAccessExpressionSyntax WithName(SimpleNameSyntax name) => Update(this.Expression, this.OperatorToken, name);
    }

    /// <summary>Class which represents the syntax node for conditional access expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ConditionalAccessExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ConditionalAccessExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;
        private ExpressionSyntax? whenNotNull;

        internal ConditionalAccessExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>ExpressionSyntax node representing the object conditionally accessed.</summary>
        public ExpressionSyntax Expression => GetRedAtZero(ref this.expression)!;

        /// <summary>SyntaxToken representing the question mark.</summary>
        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ConditionalAccessExpressionSyntax)this.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>ExpressionSyntax node representing the access expression to be executed when the object is not null.</summary>
        public ExpressionSyntax WhenNotNull => GetRed(ref this.whenNotNull, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.expression)!,
                2 => GetRed(ref this.whenNotNull, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.expression,
                2 => this.whenNotNull,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitConditionalAccessExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitConditionalAccessExpression(this);

        public ConditionalAccessExpressionSyntax Update(ExpressionSyntax expression, SyntaxToken operatorToken, ExpressionSyntax whenNotNull)
        {
            if (expression != this.Expression || operatorToken != this.OperatorToken || whenNotNull != this.WhenNotNull)
            {
                var newNode = SyntaxFactory.ConditionalAccessExpression(expression, operatorToken, whenNotNull);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ConditionalAccessExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(expression, this.OperatorToken, this.WhenNotNull);
        public ConditionalAccessExpressionSyntax WithOperatorToken(SyntaxToken operatorToken) => Update(this.Expression, operatorToken, this.WhenNotNull);
        public ConditionalAccessExpressionSyntax WithWhenNotNull(ExpressionSyntax whenNotNull) => Update(this.Expression, this.OperatorToken, whenNotNull);
    }

    /// <summary>Class which represents the syntax node for member binding expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.MemberBindingExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class MemberBindingExpressionSyntax : ExpressionSyntax
    {
        private SimpleNameSyntax? name;

        internal MemberBindingExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing dot.</summary>
        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Syntax.InternalSyntax.MemberBindingExpressionSyntax)this.Green).operatorToken, Position, 0);

        /// <summary>SimpleNameSyntax node representing the member being bound to.</summary>
        public SimpleNameSyntax Name => GetRed(ref this.name, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.name, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.name : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitMemberBindingExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitMemberBindingExpression(this);

        public MemberBindingExpressionSyntax Update(SyntaxToken operatorToken, SimpleNameSyntax name)
        {
            if (operatorToken != this.OperatorToken || name != this.Name)
            {
                var newNode = SyntaxFactory.MemberBindingExpression(operatorToken, name);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public MemberBindingExpressionSyntax WithOperatorToken(SyntaxToken operatorToken) => Update(operatorToken, this.Name);
        public MemberBindingExpressionSyntax WithName(SimpleNameSyntax name) => Update(this.OperatorToken, name);
    }

    /// <summary>Class which represents the syntax node for element binding expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ElementBindingExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ElementBindingExpressionSyntax : ExpressionSyntax
    {
        private BracketedArgumentListSyntax? argumentList;

        internal ElementBindingExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>BracketedArgumentListSyntax node representing the list of arguments of the element binding expression.</summary>
        public BracketedArgumentListSyntax ArgumentList => GetRedAtZero(ref this.argumentList)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.argumentList)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.argumentList : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitElementBindingExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitElementBindingExpression(this);

        public ElementBindingExpressionSyntax Update(BracketedArgumentListSyntax argumentList)
        {
            if (argumentList != this.ArgumentList)
            {
                var newNode = SyntaxFactory.ElementBindingExpression(argumentList);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ElementBindingExpressionSyntax WithArgumentList(BracketedArgumentListSyntax argumentList) => Update(argumentList);

        public ElementBindingExpressionSyntax AddArgumentListArguments(params ArgumentSyntax[] items) => WithArgumentList(this.ArgumentList.WithArguments(this.ArgumentList.Arguments.AddRange(items)));
    }

    /// <summary>Class which represents the syntax node for a range expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.RangeExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class RangeExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? leftOperand;
        private ExpressionSyntax? rightOperand;

        internal RangeExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>ExpressionSyntax node representing the expression on the left of the range operator.</summary>
        public ExpressionSyntax? LeftOperand => GetRedAtZero(ref this.leftOperand);

        /// <summary>SyntaxToken representing the operator of the range expression.</summary>
        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Syntax.InternalSyntax.RangeExpressionSyntax)this.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>ExpressionSyntax node representing the expression on the right of the range operator.</summary>
        public ExpressionSyntax? RightOperand => GetRed(ref this.rightOperand, 2);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.leftOperand),
                2 => GetRed(ref this.rightOperand, 2),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.leftOperand,
                2 => this.rightOperand,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitRangeExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRangeExpression(this);

        public RangeExpressionSyntax Update(ExpressionSyntax? leftOperand, SyntaxToken operatorToken, ExpressionSyntax? rightOperand)
        {
            if (leftOperand != this.LeftOperand || operatorToken != this.OperatorToken || rightOperand != this.RightOperand)
            {
                var newNode = SyntaxFactory.RangeExpression(leftOperand, operatorToken, rightOperand);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public RangeExpressionSyntax WithLeftOperand(ExpressionSyntax? leftOperand) => Update(leftOperand, this.OperatorToken, this.RightOperand);
        public RangeExpressionSyntax WithOperatorToken(SyntaxToken operatorToken) => Update(this.LeftOperand, operatorToken, this.RightOperand);
        public RangeExpressionSyntax WithRightOperand(ExpressionSyntax? rightOperand) => Update(this.LeftOperand, this.OperatorToken, rightOperand);
    }

    /// <summary>Class which represents the syntax node for implicit element access expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ImplicitElementAccess"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ImplicitElementAccessSyntax : ExpressionSyntax
    {
        private BracketedArgumentListSyntax? argumentList;

        internal ImplicitElementAccessSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>BracketedArgumentListSyntax node representing the list of arguments of the implicit element access expression.</summary>
        public BracketedArgumentListSyntax ArgumentList => GetRedAtZero(ref this.argumentList)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.argumentList)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.argumentList : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitImplicitElementAccess(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitImplicitElementAccess(this);

        public ImplicitElementAccessSyntax Update(BracketedArgumentListSyntax argumentList)
        {
            if (argumentList != this.ArgumentList)
            {
                var newNode = SyntaxFactory.ImplicitElementAccess(argumentList);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ImplicitElementAccessSyntax WithArgumentList(BracketedArgumentListSyntax argumentList) => Update(argumentList);

        public ImplicitElementAccessSyntax AddArgumentListArguments(params ArgumentSyntax[] items) => WithArgumentList(this.ArgumentList.WithArguments(this.ArgumentList.Arguments.AddRange(items)));
    }

    /// <summary>Class which represents an expression that has a binary operator.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.AddExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.SubtractExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.MultiplyExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.DivideExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.ModuloExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.LeftShiftExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.RightShiftExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.LogicalOrExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.LogicalAndExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.BitwiseOrExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.BitwiseAndExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.ExclusiveOrExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.EqualsExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.NotEqualsExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.LessThanExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.LessThanOrEqualExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.GreaterThanExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.GreaterThanOrEqualExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.IsExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.AsExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.CoalesceExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class BinaryExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? left;
        private ExpressionSyntax? right;

        internal BinaryExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>ExpressionSyntax node representing the expression on the left of the binary operator.</summary>
        public ExpressionSyntax Left => GetRedAtZero(ref this.left)!;

        /// <summary>SyntaxToken representing the operator of the binary expression.</summary>
        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Syntax.InternalSyntax.BinaryExpressionSyntax)this.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>ExpressionSyntax node representing the expression on the right of the binary operator.</summary>
        public ExpressionSyntax Right => GetRed(ref this.right, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.left)!,
                2 => GetRed(ref this.right, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.left,
                2 => this.right,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitBinaryExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBinaryExpression(this);

        public BinaryExpressionSyntax Update(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            if (left != this.Left || operatorToken != this.OperatorToken || right != this.Right)
            {
                var newNode = SyntaxFactory.BinaryExpression(this.Kind(), left, operatorToken, right);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public BinaryExpressionSyntax WithLeft(ExpressionSyntax left) => Update(left, this.OperatorToken, this.Right);
        public BinaryExpressionSyntax WithOperatorToken(SyntaxToken operatorToken) => Update(this.Left, operatorToken, this.Right);
        public BinaryExpressionSyntax WithRight(ExpressionSyntax right) => Update(this.Left, this.OperatorToken, right);
    }

    /// <summary>Class which represents an expression that has an assignment operator.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.SimpleAssignmentExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.AddAssignmentExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.SubtractAssignmentExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.MultiplyAssignmentExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.DivideAssignmentExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.ModuloAssignmentExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.AndAssignmentExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.ExclusiveOrAssignmentExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.OrAssignmentExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.LeftShiftAssignmentExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.RightShiftAssignmentExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.CoalesceAssignmentExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class AssignmentExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? left;
        private ExpressionSyntax? right;

        internal AssignmentExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>ExpressionSyntax node representing the expression on the left of the assignment operator.</summary>
        public ExpressionSyntax Left => GetRedAtZero(ref this.left)!;

        /// <summary>SyntaxToken representing the operator of the assignment expression.</summary>
        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Syntax.InternalSyntax.AssignmentExpressionSyntax)this.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>ExpressionSyntax node representing the expression on the right of the assignment operator.</summary>
        public ExpressionSyntax Right => GetRed(ref this.right, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.left)!,
                2 => GetRed(ref this.right, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.left,
                2 => this.right,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitAssignmentExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAssignmentExpression(this);

        public AssignmentExpressionSyntax Update(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            if (left != this.Left || operatorToken != this.OperatorToken || right != this.Right)
            {
                var newNode = SyntaxFactory.AssignmentExpression(this.Kind(), left, operatorToken, right);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public AssignmentExpressionSyntax WithLeft(ExpressionSyntax left) => Update(left, this.OperatorToken, this.Right);
        public AssignmentExpressionSyntax WithOperatorToken(SyntaxToken operatorToken) => Update(this.Left, operatorToken, this.Right);
        public AssignmentExpressionSyntax WithRight(ExpressionSyntax right) => Update(this.Left, this.OperatorToken, right);
    }

    /// <summary>Class which represents the syntax node for conditional expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ConditionalExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ConditionalExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? condition;
        private ExpressionSyntax? whenTrue;
        private ExpressionSyntax? whenFalse;

        internal ConditionalExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>ExpressionSyntax node representing the condition of the conditional expression.</summary>
        public ExpressionSyntax Condition => GetRedAtZero(ref this.condition)!;

        /// <summary>SyntaxToken representing the question mark.</summary>
        public SyntaxToken QuestionToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ConditionalExpressionSyntax)this.Green).questionToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>ExpressionSyntax node representing the expression to be executed when the condition is true.</summary>
        public ExpressionSyntax WhenTrue => GetRed(ref this.whenTrue, 2)!;

        /// <summary>SyntaxToken representing the colon.</summary>
        public SyntaxToken ColonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ConditionalExpressionSyntax)this.Green).colonToken, GetChildPosition(3), GetChildIndex(3));

        /// <summary>ExpressionSyntax node representing the expression to be executed when the condition is false.</summary>
        public ExpressionSyntax WhenFalse => GetRed(ref this.whenFalse, 4)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.condition)!,
                2 => GetRed(ref this.whenTrue, 2)!,
                4 => GetRed(ref this.whenFalse, 4)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.condition,
                2 => this.whenTrue,
                4 => this.whenFalse,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitConditionalExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitConditionalExpression(this);

        public ConditionalExpressionSyntax Update(ExpressionSyntax condition, SyntaxToken questionToken, ExpressionSyntax whenTrue, SyntaxToken colonToken, ExpressionSyntax whenFalse)
        {
            if (condition != this.Condition || questionToken != this.QuestionToken || whenTrue != this.WhenTrue || colonToken != this.ColonToken || whenFalse != this.WhenFalse)
            {
                var newNode = SyntaxFactory.ConditionalExpression(condition, questionToken, whenTrue, colonToken, whenFalse);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ConditionalExpressionSyntax WithCondition(ExpressionSyntax condition) => Update(condition, this.QuestionToken, this.WhenTrue, this.ColonToken, this.WhenFalse);
        public ConditionalExpressionSyntax WithQuestionToken(SyntaxToken questionToken) => Update(this.Condition, questionToken, this.WhenTrue, this.ColonToken, this.WhenFalse);
        public ConditionalExpressionSyntax WithWhenTrue(ExpressionSyntax whenTrue) => Update(this.Condition, this.QuestionToken, whenTrue, this.ColonToken, this.WhenFalse);
        public ConditionalExpressionSyntax WithColonToken(SyntaxToken colonToken) => Update(this.Condition, this.QuestionToken, this.WhenTrue, colonToken, this.WhenFalse);
        public ConditionalExpressionSyntax WithWhenFalse(ExpressionSyntax whenFalse) => Update(this.Condition, this.QuestionToken, this.WhenTrue, this.ColonToken, whenFalse);
    }

    /// <summary>Provides the base class from which the classes that represent instance expression syntax nodes are derived. This is an abstract class.</summary>
    public abstract partial class InstanceExpressionSyntax : ExpressionSyntax
    {
        internal InstanceExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    /// <summary>Class which represents the syntax node for a this expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ThisExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ThisExpressionSyntax : InstanceExpressionSyntax
    {

        internal ThisExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the this keyword.</summary>
        public SyntaxToken Token => new SyntaxToken(this, ((Syntax.InternalSyntax.ThisExpressionSyntax)this.Green).token, Position, 0);

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitThisExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitThisExpression(this);

        public ThisExpressionSyntax Update(SyntaxToken token)
        {
            if (token != this.Token)
            {
                var newNode = SyntaxFactory.ThisExpression(token);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ThisExpressionSyntax WithToken(SyntaxToken token) => Update(token);
    }

    /// <summary>Class which represents the syntax node for a base expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.BaseExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class BaseExpressionSyntax : InstanceExpressionSyntax
    {

        internal BaseExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the base keyword.</summary>
        public SyntaxToken Token => new SyntaxToken(this, ((Syntax.InternalSyntax.BaseExpressionSyntax)this.Green).token, Position, 0);

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitBaseExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBaseExpression(this);

        public BaseExpressionSyntax Update(SyntaxToken token)
        {
            if (token != this.Token)
            {
                var newNode = SyntaxFactory.BaseExpression(token);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public BaseExpressionSyntax WithToken(SyntaxToken token) => Update(token);
    }

    /// <summary>Class which represents the syntax node for a literal expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ArgListExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.NumericLiteralExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.StringLiteralExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.CharacterLiteralExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.TrueLiteralExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.FalseLiteralExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.NullLiteralExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.DefaultLiteralExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class LiteralExpressionSyntax : ExpressionSyntax
    {

        internal LiteralExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the keyword corresponding to the kind of the literal expression.</summary>
        public SyntaxToken Token => new SyntaxToken(this, ((Syntax.InternalSyntax.LiteralExpressionSyntax)this.Green).token, Position, 0);

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitLiteralExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLiteralExpression(this);

        public LiteralExpressionSyntax Update(SyntaxToken token)
        {
            if (token != this.Token)
            {
                var newNode = SyntaxFactory.LiteralExpression(this.Kind(), token);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public LiteralExpressionSyntax WithToken(SyntaxToken token) => Update(token);
    }

    /// <summary>Class which represents the syntax node for MakeRef expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.MakeRefExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class MakeRefExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        internal MakeRefExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the MakeRefKeyword.</summary>
        public SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.MakeRefExpressionSyntax)this.Green).keyword, Position, 0);

        /// <summary>SyntaxToken representing open parenthesis.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.MakeRefExpressionSyntax)this.Green).openParenToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>Argument of the primary function.</summary>
        public ExpressionSyntax Expression => GetRed(ref this.expression, 2)!;

        /// <summary>SyntaxToken representing close parenthesis.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.MakeRefExpressionSyntax)this.Green).closeParenToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.expression, 2)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.expression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitMakeRefExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitMakeRefExpression(this);

        public MakeRefExpressionSyntax Update(SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            if (keyword != this.Keyword || openParenToken != this.OpenParenToken || expression != this.Expression || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.MakeRefExpression(keyword, openParenToken, expression, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public MakeRefExpressionSyntax WithKeyword(SyntaxToken keyword) => Update(keyword, this.OpenParenToken, this.Expression, this.CloseParenToken);
        public MakeRefExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.Keyword, openParenToken, this.Expression, this.CloseParenToken);
        public MakeRefExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(this.Keyword, this.OpenParenToken, expression, this.CloseParenToken);
        public MakeRefExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.Keyword, this.OpenParenToken, this.Expression, closeParenToken);
    }

    /// <summary>Class which represents the syntax node for RefType expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.RefTypeExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class RefTypeExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        internal RefTypeExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the RefTypeKeyword.</summary>
        public SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.RefTypeExpressionSyntax)this.Green).keyword, Position, 0);

        /// <summary>SyntaxToken representing open parenthesis.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.RefTypeExpressionSyntax)this.Green).openParenToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>Argument of the primary function.</summary>
        public ExpressionSyntax Expression => GetRed(ref this.expression, 2)!;

        /// <summary>SyntaxToken representing close parenthesis.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.RefTypeExpressionSyntax)this.Green).closeParenToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.expression, 2)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.expression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitRefTypeExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRefTypeExpression(this);

        public RefTypeExpressionSyntax Update(SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            if (keyword != this.Keyword || openParenToken != this.OpenParenToken || expression != this.Expression || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.RefTypeExpression(keyword, openParenToken, expression, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public RefTypeExpressionSyntax WithKeyword(SyntaxToken keyword) => Update(keyword, this.OpenParenToken, this.Expression, this.CloseParenToken);
        public RefTypeExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.Keyword, openParenToken, this.Expression, this.CloseParenToken);
        public RefTypeExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(this.Keyword, this.OpenParenToken, expression, this.CloseParenToken);
        public RefTypeExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.Keyword, this.OpenParenToken, this.Expression, closeParenToken);
    }

    /// <summary>Class which represents the syntax node for RefValue expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.RefValueExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class RefValueExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;
        private TypeSyntax? type;

        internal RefValueExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the RefValueKeyword.</summary>
        public SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.RefValueExpressionSyntax)this.Green).keyword, Position, 0);

        /// <summary>SyntaxToken representing open parenthesis.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.RefValueExpressionSyntax)this.Green).openParenToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>Typed reference expression.</summary>
        public ExpressionSyntax Expression => GetRed(ref this.expression, 2)!;

        /// <summary>Comma separating the arguments.</summary>
        public SyntaxToken Comma => new SyntaxToken(this, ((Syntax.InternalSyntax.RefValueExpressionSyntax)this.Green).comma, GetChildPosition(3), GetChildIndex(3));

        /// <summary>The type of the value.</summary>
        public TypeSyntax Type => GetRed(ref this.type, 4)!;

        /// <summary>SyntaxToken representing close parenthesis.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.RefValueExpressionSyntax)this.Green).closeParenToken, GetChildPosition(5), GetChildIndex(5));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                2 => GetRed(ref this.expression, 2)!,
                4 => GetRed(ref this.type, 4)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                2 => this.expression,
                4 => this.type,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitRefValueExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRefValueExpression(this);

        public RefValueExpressionSyntax Update(SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken comma, TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword != this.Keyword || openParenToken != this.OpenParenToken || expression != this.Expression || comma != this.Comma || type != this.Type || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.RefValueExpression(keyword, openParenToken, expression, comma, type, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public RefValueExpressionSyntax WithKeyword(SyntaxToken keyword) => Update(keyword, this.OpenParenToken, this.Expression, this.Comma, this.Type, this.CloseParenToken);
        public RefValueExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.Keyword, openParenToken, this.Expression, this.Comma, this.Type, this.CloseParenToken);
        public RefValueExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(this.Keyword, this.OpenParenToken, expression, this.Comma, this.Type, this.CloseParenToken);
        public RefValueExpressionSyntax WithComma(SyntaxToken comma) => Update(this.Keyword, this.OpenParenToken, this.Expression, comma, this.Type, this.CloseParenToken);
        public RefValueExpressionSyntax WithType(TypeSyntax type) => Update(this.Keyword, this.OpenParenToken, this.Expression, this.Comma, type, this.CloseParenToken);
        public RefValueExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.Keyword, this.OpenParenToken, this.Expression, this.Comma, this.Type, closeParenToken);
    }

    /// <summary>Class which represents the syntax node for Checked or Unchecked expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.CheckedExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.UncheckedExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class CheckedExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        internal CheckedExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the checked or unchecked keyword.</summary>
        public SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.CheckedExpressionSyntax)this.Green).keyword, Position, 0);

        /// <summary>SyntaxToken representing open parenthesis.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CheckedExpressionSyntax)this.Green).openParenToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>Argument of the primary function.</summary>
        public ExpressionSyntax Expression => GetRed(ref this.expression, 2)!;

        /// <summary>SyntaxToken representing close parenthesis.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CheckedExpressionSyntax)this.Green).closeParenToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.expression, 2)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.expression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitCheckedExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCheckedExpression(this);

        public CheckedExpressionSyntax Update(SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            if (keyword != this.Keyword || openParenToken != this.OpenParenToken || expression != this.Expression || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.CheckedExpression(this.Kind(), keyword, openParenToken, expression, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public CheckedExpressionSyntax WithKeyword(SyntaxToken keyword) => Update(keyword, this.OpenParenToken, this.Expression, this.CloseParenToken);
        public CheckedExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.Keyword, openParenToken, this.Expression, this.CloseParenToken);
        public CheckedExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(this.Keyword, this.OpenParenToken, expression, this.CloseParenToken);
        public CheckedExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.Keyword, this.OpenParenToken, this.Expression, closeParenToken);
    }

    /// <summary>Class which represents the syntax node for Default expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.DefaultExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class DefaultExpressionSyntax : ExpressionSyntax
    {
        private TypeSyntax? type;

        internal DefaultExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the DefaultKeyword.</summary>
        public SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.DefaultExpressionSyntax)this.Green).keyword, Position, 0);

        /// <summary>SyntaxToken representing open parenthesis.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.DefaultExpressionSyntax)this.Green).openParenToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>Argument of the primary function.</summary>
        public TypeSyntax Type => GetRed(ref this.type, 2)!;

        /// <summary>SyntaxToken representing close parenthesis.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.DefaultExpressionSyntax)this.Green).closeParenToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.type, 2)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.type : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitDefaultExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDefaultExpression(this);

        public DefaultExpressionSyntax Update(SyntaxToken keyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword != this.Keyword || openParenToken != this.OpenParenToken || type != this.Type || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.DefaultExpression(keyword, openParenToken, type, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public DefaultExpressionSyntax WithKeyword(SyntaxToken keyword) => Update(keyword, this.OpenParenToken, this.Type, this.CloseParenToken);
        public DefaultExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.Keyword, openParenToken, this.Type, this.CloseParenToken);
        public DefaultExpressionSyntax WithType(TypeSyntax type) => Update(this.Keyword, this.OpenParenToken, type, this.CloseParenToken);
        public DefaultExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.Keyword, this.OpenParenToken, this.Type, closeParenToken);
    }

    /// <summary>Class which represents the syntax node for TypeOf expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.TypeOfExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class TypeOfExpressionSyntax : ExpressionSyntax
    {
        private TypeSyntax? type;

        internal TypeOfExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the TypeOfKeyword.</summary>
        public SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.TypeOfExpressionSyntax)this.Green).keyword, Position, 0);

        /// <summary>SyntaxToken representing open parenthesis.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.TypeOfExpressionSyntax)this.Green).openParenToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>The expression to return type of.</summary>
        public TypeSyntax Type => GetRed(ref this.type, 2)!;

        /// <summary>SyntaxToken representing close parenthesis.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.TypeOfExpressionSyntax)this.Green).closeParenToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.type, 2)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.type : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitTypeOfExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeOfExpression(this);

        public TypeOfExpressionSyntax Update(SyntaxToken keyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword != this.Keyword || openParenToken != this.OpenParenToken || type != this.Type || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.TypeOfExpression(keyword, openParenToken, type, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public TypeOfExpressionSyntax WithKeyword(SyntaxToken keyword) => Update(keyword, this.OpenParenToken, this.Type, this.CloseParenToken);
        public TypeOfExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.Keyword, openParenToken, this.Type, this.CloseParenToken);
        public TypeOfExpressionSyntax WithType(TypeSyntax type) => Update(this.Keyword, this.OpenParenToken, type, this.CloseParenToken);
        public TypeOfExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.Keyword, this.OpenParenToken, this.Type, closeParenToken);
    }

    /// <summary>Class which represents the syntax node for SizeOf expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.SizeOfExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class SizeOfExpressionSyntax : ExpressionSyntax
    {
        private TypeSyntax? type;

        internal SizeOfExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the SizeOfKeyword.</summary>
        public SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.SizeOfExpressionSyntax)this.Green).keyword, Position, 0);

        /// <summary>SyntaxToken representing open parenthesis.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.SizeOfExpressionSyntax)this.Green).openParenToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>Argument of the primary function.</summary>
        public TypeSyntax Type => GetRed(ref this.type, 2)!;

        /// <summary>SyntaxToken representing close parenthesis.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.SizeOfExpressionSyntax)this.Green).closeParenToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.type, 2)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.type : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitSizeOfExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSizeOfExpression(this);

        public SizeOfExpressionSyntax Update(SyntaxToken keyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken)
        {
            if (keyword != this.Keyword || openParenToken != this.OpenParenToken || type != this.Type || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.SizeOfExpression(keyword, openParenToken, type, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public SizeOfExpressionSyntax WithKeyword(SyntaxToken keyword) => Update(keyword, this.OpenParenToken, this.Type, this.CloseParenToken);
        public SizeOfExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.Keyword, openParenToken, this.Type, this.CloseParenToken);
        public SizeOfExpressionSyntax WithType(TypeSyntax type) => Update(this.Keyword, this.OpenParenToken, type, this.CloseParenToken);
        public SizeOfExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.Keyword, this.OpenParenToken, this.Type, closeParenToken);
    }

    /// <summary>Class which represents the syntax node for invocation expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.InvocationExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class InvocationExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;
        private ArgumentListSyntax? argumentList;

        internal InvocationExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>ExpressionSyntax node representing the expression part of the invocation.</summary>
        public ExpressionSyntax Expression => GetRedAtZero(ref this.expression)!;

        /// <summary>ArgumentListSyntax node representing the list of arguments of the invocation expression.</summary>
        public ArgumentListSyntax ArgumentList => GetRed(ref this.argumentList, 1)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.expression)!,
                1 => GetRed(ref this.argumentList, 1)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.expression,
                1 => this.argumentList,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitInvocationExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitInvocationExpression(this);

        public InvocationExpressionSyntax Update(ExpressionSyntax expression, ArgumentListSyntax argumentList)
        {
            if (expression != this.Expression || argumentList != this.ArgumentList)
            {
                var newNode = SyntaxFactory.InvocationExpression(expression, argumentList);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public InvocationExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(expression, this.ArgumentList);
        public InvocationExpressionSyntax WithArgumentList(ArgumentListSyntax argumentList) => Update(this.Expression, argumentList);

        public InvocationExpressionSyntax AddArgumentListArguments(params ArgumentSyntax[] items) => WithArgumentList(this.ArgumentList.WithArguments(this.ArgumentList.Arguments.AddRange(items)));
    }

    /// <summary>Class which represents the syntax node for element access expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ElementAccessExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ElementAccessExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;
        private BracketedArgumentListSyntax? argumentList;

        internal ElementAccessExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>ExpressionSyntax node representing the expression which is accessing the element.</summary>
        public ExpressionSyntax Expression => GetRedAtZero(ref this.expression)!;

        /// <summary>BracketedArgumentListSyntax node representing the list of arguments of the element access expression.</summary>
        public BracketedArgumentListSyntax ArgumentList => GetRed(ref this.argumentList, 1)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.expression)!,
                1 => GetRed(ref this.argumentList, 1)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.expression,
                1 => this.argumentList,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitElementAccessExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitElementAccessExpression(this);

        public ElementAccessExpressionSyntax Update(ExpressionSyntax expression, BracketedArgumentListSyntax argumentList)
        {
            if (expression != this.Expression || argumentList != this.ArgumentList)
            {
                var newNode = SyntaxFactory.ElementAccessExpression(expression, argumentList);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ElementAccessExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(expression, this.ArgumentList);
        public ElementAccessExpressionSyntax WithArgumentList(BracketedArgumentListSyntax argumentList) => Update(this.Expression, argumentList);

        public ElementAccessExpressionSyntax AddArgumentListArguments(params ArgumentSyntax[] items) => WithArgumentList(this.ArgumentList.WithArguments(this.ArgumentList.Arguments.AddRange(items)));
    }

    /// <summary>Provides the base class from which the classes that represent argument list syntax nodes are derived. This is an abstract class.</summary>
    public abstract partial class BaseArgumentListSyntax : CSharpSyntaxNode
    {
        internal BaseArgumentListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SeparatedSyntaxList of ArgumentSyntax nodes representing the list of arguments.</summary>
        public abstract SeparatedSyntaxList<ArgumentSyntax> Arguments { get; }
        public BaseArgumentListSyntax WithArguments(SeparatedSyntaxList<ArgumentSyntax> arguments) => WithArgumentsCore(arguments);
        internal abstract BaseArgumentListSyntax WithArgumentsCore(SeparatedSyntaxList<ArgumentSyntax> arguments);

        public BaseArgumentListSyntax AddArguments(params ArgumentSyntax[] items) => AddArgumentsCore(items);
        internal abstract BaseArgumentListSyntax AddArgumentsCore(params ArgumentSyntax[] items);
    }

    /// <summary>Class which represents the syntax node for the list of arguments.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ArgumentList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ArgumentListSyntax : BaseArgumentListSyntax
    {
        private SyntaxNode? arguments;

        internal ArgumentListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing open parenthesis.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ArgumentListSyntax)this.Green).openParenToken, Position, 0);

        /// <summary>SeparatedSyntaxList of ArgumentSyntax representing the list of arguments.</summary>
        public override SeparatedSyntaxList<ArgumentSyntax> Arguments
        {
            get
            {
                var red = GetRed(ref this.arguments, 1);
                return red != null ? new SeparatedSyntaxList<ArgumentSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>SyntaxToken representing close parenthesis.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ArgumentListSyntax)this.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.arguments, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.arguments : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitArgumentList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitArgumentList(this);

        public ArgumentListSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<ArgumentSyntax> arguments, SyntaxToken closeParenToken)
        {
            if (openParenToken != this.OpenParenToken || arguments != this.Arguments || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.ArgumentList(openParenToken, arguments, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ArgumentListSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(openParenToken, this.Arguments, this.CloseParenToken);
        internal override BaseArgumentListSyntax WithArgumentsCore(SeparatedSyntaxList<ArgumentSyntax> arguments) => WithArguments(arguments);
        public new ArgumentListSyntax WithArguments(SeparatedSyntaxList<ArgumentSyntax> arguments) => Update(this.OpenParenToken, arguments, this.CloseParenToken);
        public ArgumentListSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.OpenParenToken, this.Arguments, closeParenToken);

        internal override BaseArgumentListSyntax AddArgumentsCore(params ArgumentSyntax[] items) => AddArguments(items);
        public new ArgumentListSyntax AddArguments(params ArgumentSyntax[] items) => WithArguments(this.Arguments.AddRange(items));
    }

    /// <summary>Class which represents the syntax node for bracketed argument list.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.BracketedArgumentList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class BracketedArgumentListSyntax : BaseArgumentListSyntax
    {
        private SyntaxNode? arguments;

        internal BracketedArgumentListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing open bracket.</summary>
        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.BracketedArgumentListSyntax)this.Green).openBracketToken, Position, 0);

        /// <summary>SeparatedSyntaxList of ArgumentSyntax representing the list of arguments.</summary>
        public override SeparatedSyntaxList<ArgumentSyntax> Arguments
        {
            get
            {
                var red = GetRed(ref this.arguments, 1);
                return red != null ? new SeparatedSyntaxList<ArgumentSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>SyntaxToken representing close bracket.</summary>
        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.BracketedArgumentListSyntax)this.Green).closeBracketToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.arguments, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.arguments : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitBracketedArgumentList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBracketedArgumentList(this);

        public BracketedArgumentListSyntax Update(SyntaxToken openBracketToken, SeparatedSyntaxList<ArgumentSyntax> arguments, SyntaxToken closeBracketToken)
        {
            if (openBracketToken != this.OpenBracketToken || arguments != this.Arguments || closeBracketToken != this.CloseBracketToken)
            {
                var newNode = SyntaxFactory.BracketedArgumentList(openBracketToken, arguments, closeBracketToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public BracketedArgumentListSyntax WithOpenBracketToken(SyntaxToken openBracketToken) => Update(openBracketToken, this.Arguments, this.CloseBracketToken);
        internal override BaseArgumentListSyntax WithArgumentsCore(SeparatedSyntaxList<ArgumentSyntax> arguments) => WithArguments(arguments);
        public new BracketedArgumentListSyntax WithArguments(SeparatedSyntaxList<ArgumentSyntax> arguments) => Update(this.OpenBracketToken, arguments, this.CloseBracketToken);
        public BracketedArgumentListSyntax WithCloseBracketToken(SyntaxToken closeBracketToken) => Update(this.OpenBracketToken, this.Arguments, closeBracketToken);

        internal override BaseArgumentListSyntax AddArgumentsCore(params ArgumentSyntax[] items) => AddArguments(items);
        public new BracketedArgumentListSyntax AddArguments(params ArgumentSyntax[] items) => WithArguments(this.Arguments.AddRange(items));
    }

    /// <summary>Class which represents the syntax node for argument.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.Argument"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ArgumentSyntax : CSharpSyntaxNode
    {
        private NameColonSyntax? nameColon;
        private ExpressionSyntax? expression;

        internal ArgumentSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>NameColonSyntax node representing the optional name arguments.</summary>
        public NameColonSyntax? NameColon => GetRedAtZero(ref this.nameColon);

        /// <summary>SyntaxToken representing the optional ref or out keyword.</summary>
        public SyntaxToken RefKindKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.ArgumentSyntax)this.Green).refKindKeyword;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>ExpressionSyntax node representing the argument.</summary>
        public ExpressionSyntax Expression => GetRed(ref this.expression, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.nameColon),
                2 => GetRed(ref this.expression, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.nameColon,
                2 => this.expression,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitArgument(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitArgument(this);

        public ArgumentSyntax Update(NameColonSyntax? nameColon, SyntaxToken refKindKeyword, ExpressionSyntax expression)
        {
            if (nameColon != this.NameColon || refKindKeyword != this.RefKindKeyword || expression != this.Expression)
            {
                var newNode = SyntaxFactory.Argument(nameColon, refKindKeyword, expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ArgumentSyntax WithNameColon(NameColonSyntax? nameColon) => Update(nameColon, this.RefKindKeyword, this.Expression);
        public ArgumentSyntax WithRefKindKeyword(SyntaxToken refKindKeyword) => Update(this.NameColon, refKindKeyword, this.Expression);
        public ArgumentSyntax WithExpression(ExpressionSyntax expression) => Update(this.NameColon, this.RefKindKeyword, expression);
    }

    /// <summary>Class which represents the syntax node for name colon syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.NameColon"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class NameColonSyntax : CSharpSyntaxNode
    {
        private IdentifierNameSyntax? name;

        internal NameColonSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>IdentifierNameSyntax representing the identifier name.</summary>
        public IdentifierNameSyntax Name => GetRedAtZero(ref this.name)!;

        /// <summary>SyntaxToken representing colon.</summary>
        public SyntaxToken ColonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.NameColonSyntax)this.Green).colonToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.name)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.name : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitNameColon(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitNameColon(this);

        public NameColonSyntax Update(IdentifierNameSyntax name, SyntaxToken colonToken)
        {
            if (name != this.Name || colonToken != this.ColonToken)
            {
                var newNode = SyntaxFactory.NameColon(name, colonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public NameColonSyntax WithName(IdentifierNameSyntax name) => Update(name, this.ColonToken);
        public NameColonSyntax WithColonToken(SyntaxToken colonToken) => Update(this.Name, colonToken);
    }

    /// <summary>Class which represents the syntax node for the variable declaration in an out var declaration or a deconstruction declaration.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.DeclarationExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class DeclarationExpressionSyntax : ExpressionSyntax
    {
        private TypeSyntax? type;
        private VariableDesignationSyntax? designation;

        internal DeclarationExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public TypeSyntax Type => GetRedAtZero(ref this.type)!;

        /// <summary>Declaration representing the variable declared in an out parameter or deconstruction.</summary>
        public VariableDesignationSyntax Designation => GetRed(ref this.designation, 1)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.type)!,
                1 => GetRed(ref this.designation, 1)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.type,
                1 => this.designation,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitDeclarationExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDeclarationExpression(this);

        public DeclarationExpressionSyntax Update(TypeSyntax type, VariableDesignationSyntax designation)
        {
            if (type != this.Type || designation != this.Designation)
            {
                var newNode = SyntaxFactory.DeclarationExpression(type, designation);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public DeclarationExpressionSyntax WithType(TypeSyntax type) => Update(type, this.Designation);
        public DeclarationExpressionSyntax WithDesignation(VariableDesignationSyntax designation) => Update(this.Type, designation);
    }

    /// <summary>Class which represents the syntax node for cast expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.CastExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class CastExpressionSyntax : ExpressionSyntax
    {
        private TypeSyntax? type;
        private ExpressionSyntax? expression;

        internal CastExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the open parenthesis.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CastExpressionSyntax)this.Green).openParenToken, Position, 0);

        /// <summary>TypeSyntax node representing the type to which the expression is being cast.</summary>
        public TypeSyntax Type => GetRed(ref this.type, 1)!;

        /// <summary>SyntaxToken representing the close parenthesis.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CastExpressionSyntax)this.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        /// <summary>ExpressionSyntax node representing the expression that is being casted.</summary>
        public ExpressionSyntax Expression => GetRed(ref this.expression, 3)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.type, 1)!,
                3 => GetRed(ref this.expression, 3)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.type,
                3 => this.expression,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitCastExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCastExpression(this);

        public CastExpressionSyntax Update(SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken, ExpressionSyntax expression)
        {
            if (openParenToken != this.OpenParenToken || type != this.Type || closeParenToken != this.CloseParenToken || expression != this.Expression)
            {
                var newNode = SyntaxFactory.CastExpression(openParenToken, type, closeParenToken, expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public CastExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(openParenToken, this.Type, this.CloseParenToken, this.Expression);
        public CastExpressionSyntax WithType(TypeSyntax type) => Update(this.OpenParenToken, type, this.CloseParenToken, this.Expression);
        public CastExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.OpenParenToken, this.Type, closeParenToken, this.Expression);
        public CastExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(this.OpenParenToken, this.Type, this.CloseParenToken, expression);
    }

    /// <summary>Provides the base class from which the classes that represent anonymous function expressions are derived.</summary>
    public abstract partial class AnonymousFunctionExpressionSyntax : ExpressionSyntax
    {
        internal AnonymousFunctionExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public abstract SyntaxTokenList Modifiers { get; }
        public AnonymousFunctionExpressionSyntax WithModifiers(SyntaxTokenList modifiers) => WithModifiersCore(modifiers);
        internal abstract AnonymousFunctionExpressionSyntax WithModifiersCore(SyntaxTokenList modifiers);

        public AnonymousFunctionExpressionSyntax AddModifiers(params SyntaxToken[] items) => AddModifiersCore(items);
        internal abstract AnonymousFunctionExpressionSyntax AddModifiersCore(params SyntaxToken[] items);

        /// <summary>
        /// BlockSyntax node representing the body of the anonymous function.
        /// Only one of Block or ExpressionBody will be non-null.
        /// </summary>
        public abstract BlockSyntax? Block { get; }
        public AnonymousFunctionExpressionSyntax WithBlock(BlockSyntax? block) => WithBlockCore(block);
        internal abstract AnonymousFunctionExpressionSyntax WithBlockCore(BlockSyntax? block);

        public AnonymousFunctionExpressionSyntax AddBlockAttributeLists(params AttributeListSyntax[] items) => AddBlockAttributeListsCore(items);
        internal abstract AnonymousFunctionExpressionSyntax AddBlockAttributeListsCore(params AttributeListSyntax[] items);

        public AnonymousFunctionExpressionSyntax AddBlockStatements(params StatementSyntax[] items) => AddBlockStatementsCore(items);
        internal abstract AnonymousFunctionExpressionSyntax AddBlockStatementsCore(params StatementSyntax[] items);

        /// <summary>
        /// ExpressionSyntax node representing the body of the anonymous function.
        /// Only one of Block or ExpressionBody will be non-null.
        /// </summary>
        public abstract ExpressionSyntax? ExpressionBody { get; }
        public AnonymousFunctionExpressionSyntax WithExpressionBody(ExpressionSyntax? expressionBody) => WithExpressionBodyCore(expressionBody);
        internal abstract AnonymousFunctionExpressionSyntax WithExpressionBodyCore(ExpressionSyntax? expressionBody);
    }

    /// <summary>Class which represents the syntax node for anonymous method expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.AnonymousMethodExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class AnonymousMethodExpressionSyntax : AnonymousFunctionExpressionSyntax
    {
        private ParameterListSyntax? parameterList;
        private BlockSyntax? block;
        private ExpressionSyntax? expressionBody;

        internal AnonymousMethodExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(0);
                return slot != null ? new SyntaxTokenList(this, slot, Position, 0) : default;
            }
        }

        /// <summary>SyntaxToken representing the delegate keyword.</summary>
        public SyntaxToken DelegateKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.AnonymousMethodExpressionSyntax)this.Green).delegateKeyword, GetChildPosition(1), GetChildIndex(1));

        /// <summary>List of parameters of the anonymous method expression, or null if there no parameters are specified.</summary>
        public ParameterListSyntax? ParameterList => GetRed(ref this.parameterList, 2);

        /// <summary>
        /// BlockSyntax node representing the body of the anonymous function.
        /// This will never be null.
        /// </summary>
        public override BlockSyntax Block => GetRed(ref this.block, 3)!;

        /// <summary>
        /// Inherited from AnonymousFunctionExpressionSyntax, but not used for
        /// AnonymousMethodExpressionSyntax.  This will always be null.
        /// </summary>
        public override ExpressionSyntax? ExpressionBody => GetRed(ref this.expressionBody, 4);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                2 => GetRed(ref this.parameterList, 2),
                3 => GetRed(ref this.block, 3)!,
                4 => GetRed(ref this.expressionBody, 4),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                2 => this.parameterList,
                3 => this.block,
                4 => this.expressionBody,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitAnonymousMethodExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAnonymousMethodExpression(this);

        public AnonymousMethodExpressionSyntax Update(SyntaxTokenList modifiers, SyntaxToken delegateKeyword, ParameterListSyntax? parameterList, BlockSyntax block, ExpressionSyntax? expressionBody)
        {
            if (modifiers != this.Modifiers || delegateKeyword != this.DelegateKeyword || parameterList != this.ParameterList || block != this.Block || expressionBody != this.ExpressionBody)
            {
                var newNode = SyntaxFactory.AnonymousMethodExpression(modifiers, delegateKeyword, parameterList, block, expressionBody);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override AnonymousFunctionExpressionSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new AnonymousMethodExpressionSyntax WithModifiers(SyntaxTokenList modifiers) => Update(modifiers, this.DelegateKeyword, this.ParameterList, this.Block, this.ExpressionBody);
        public AnonymousMethodExpressionSyntax WithDelegateKeyword(SyntaxToken delegateKeyword) => Update(this.Modifiers, delegateKeyword, this.ParameterList, this.Block, this.ExpressionBody);
        public AnonymousMethodExpressionSyntax WithParameterList(ParameterListSyntax? parameterList) => Update(this.Modifiers, this.DelegateKeyword, parameterList, this.Block, this.ExpressionBody);
        internal override AnonymousFunctionExpressionSyntax WithBlockCore(BlockSyntax? block) => WithBlock(block ?? throw new ArgumentNullException(nameof(block)));
        public new AnonymousMethodExpressionSyntax WithBlock(BlockSyntax block) => Update(this.Modifiers, this.DelegateKeyword, this.ParameterList, block, this.ExpressionBody);
        internal override AnonymousFunctionExpressionSyntax WithExpressionBodyCore(ExpressionSyntax? expressionBody) => WithExpressionBody(expressionBody);
        public new AnonymousMethodExpressionSyntax WithExpressionBody(ExpressionSyntax? expressionBody) => Update(this.Modifiers, this.DelegateKeyword, this.ParameterList, this.Block, expressionBody);

        internal override AnonymousFunctionExpressionSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new AnonymousMethodExpressionSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        public AnonymousMethodExpressionSyntax AddParameterListParameters(params ParameterSyntax[] items)
        {
            var parameterList = this.ParameterList ?? SyntaxFactory.ParameterList();
            return WithParameterList(parameterList.WithParameters(parameterList.Parameters.AddRange(items)));
        }
        internal override AnonymousFunctionExpressionSyntax AddBlockAttributeListsCore(params AttributeListSyntax[] items) => AddBlockAttributeLists(items);
        public new AnonymousMethodExpressionSyntax AddBlockAttributeLists(params AttributeListSyntax[] items) => WithBlock(this.Block.WithAttributeLists(this.Block.AttributeLists.AddRange(items)));
        internal override AnonymousFunctionExpressionSyntax AddBlockStatementsCore(params StatementSyntax[] items) => AddBlockStatements(items);
        public new AnonymousMethodExpressionSyntax AddBlockStatements(params StatementSyntax[] items) => WithBlock(this.Block.WithStatements(this.Block.Statements.AddRange(items)));
    }

    /// <summary>Provides the base class from which the classes that represent lambda expressions are derived.</summary>
    public abstract partial class LambdaExpressionSyntax : AnonymousFunctionExpressionSyntax
    {
        internal LambdaExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public abstract SyntaxList<AttributeListSyntax> AttributeLists { get; }
        public LambdaExpressionSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeListsCore(attributeLists);
        internal abstract LambdaExpressionSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists);

        public LambdaExpressionSyntax AddAttributeLists(params AttributeListSyntax[] items) => AddAttributeListsCore(items);
        internal abstract LambdaExpressionSyntax AddAttributeListsCore(params AttributeListSyntax[] items);

        /// <summary>SyntaxToken representing equals greater than.</summary>
        public abstract SyntaxToken ArrowToken { get; }
        public LambdaExpressionSyntax WithArrowToken(SyntaxToken arrowToken) => WithArrowTokenCore(arrowToken);
        internal abstract LambdaExpressionSyntax WithArrowTokenCore(SyntaxToken arrowToken);

        public new LambdaExpressionSyntax WithModifiers(SyntaxTokenList modifiers) => (LambdaExpressionSyntax)WithModifiersCore(modifiers);
        public new LambdaExpressionSyntax WithBlock(BlockSyntax? block) => (LambdaExpressionSyntax)WithBlockCore(block);
        public new LambdaExpressionSyntax WithExpressionBody(ExpressionSyntax? expressionBody) => (LambdaExpressionSyntax)WithExpressionBodyCore(expressionBody);

        public new LambdaExpressionSyntax AddModifiers(params SyntaxToken[] items) => (LambdaExpressionSyntax)AddModifiersCore(items);

        public new AnonymousFunctionExpressionSyntax AddBlockAttributeLists(params AttributeListSyntax[] items) => AddBlockAttributeListsCore(items);

        public new AnonymousFunctionExpressionSyntax AddBlockStatements(params StatementSyntax[] items) => AddBlockStatementsCore(items);
    }

    /// <summary>Class which represents the syntax node for a simple lambda expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.SimpleLambdaExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class SimpleLambdaExpressionSyntax : LambdaExpressionSyntax
    {
        private SyntaxNode? attributeLists;
        private ParameterSyntax? parameter;
        private BlockSyntax? block;
        private ExpressionSyntax? expressionBody;

        internal SimpleLambdaExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>ParameterSyntax node representing the parameter of the lambda expression.</summary>
        public ParameterSyntax Parameter => GetRed(ref this.parameter, 2)!;

        /// <summary>SyntaxToken representing equals greater than.</summary>
        public override SyntaxToken ArrowToken => new SyntaxToken(this, ((Syntax.InternalSyntax.SimpleLambdaExpressionSyntax)this.Green).arrowToken, GetChildPosition(3), GetChildIndex(3));

        /// <summary>
        /// BlockSyntax node representing the body of the lambda.
        /// Only one of Block or ExpressionBody will be non-null.
        /// </summary>
        public override BlockSyntax? Block => GetRed(ref this.block, 4);

        /// <summary>
        /// ExpressionSyntax node representing the body of the lambda.
        /// Only one of Block or ExpressionBody will be non-null.
        /// </summary>
        public override ExpressionSyntax? ExpressionBody => GetRed(ref this.expressionBody, 5);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.parameter, 2)!,
                4 => GetRed(ref this.block, 4),
                5 => GetRed(ref this.expressionBody, 5),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.parameter,
                4 => this.block,
                5 => this.expressionBody,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitSimpleLambdaExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSimpleLambdaExpression(this);

        public SimpleLambdaExpressionSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, ParameterSyntax parameter, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || parameter != this.Parameter || arrowToken != this.ArrowToken || block != this.Block || expressionBody != this.ExpressionBody)
            {
                var newNode = SyntaxFactory.SimpleLambdaExpression(attributeLists, modifiers, parameter, arrowToken, block, expressionBody);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override LambdaExpressionSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new SimpleLambdaExpressionSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Parameter, this.ArrowToken, this.Block, this.ExpressionBody);
        internal override AnonymousFunctionExpressionSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new SimpleLambdaExpressionSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Parameter, this.ArrowToken, this.Block, this.ExpressionBody);
        public SimpleLambdaExpressionSyntax WithParameter(ParameterSyntax parameter) => Update(this.AttributeLists, this.Modifiers, parameter, this.ArrowToken, this.Block, this.ExpressionBody);
        internal override LambdaExpressionSyntax WithArrowTokenCore(SyntaxToken arrowToken) => WithArrowToken(arrowToken);
        public new SimpleLambdaExpressionSyntax WithArrowToken(SyntaxToken arrowToken) => Update(this.AttributeLists, this.Modifiers, this.Parameter, arrowToken, this.Block, this.ExpressionBody);
        internal override AnonymousFunctionExpressionSyntax WithBlockCore(BlockSyntax? block) => WithBlock(block);
        public new SimpleLambdaExpressionSyntax WithBlock(BlockSyntax? block) => Update(this.AttributeLists, this.Modifiers, this.Parameter, this.ArrowToken, block, this.ExpressionBody);
        internal override AnonymousFunctionExpressionSyntax WithExpressionBodyCore(ExpressionSyntax? expressionBody) => WithExpressionBody(expressionBody);
        public new SimpleLambdaExpressionSyntax WithExpressionBody(ExpressionSyntax? expressionBody) => Update(this.AttributeLists, this.Modifiers, this.Parameter, this.ArrowToken, this.Block, expressionBody);

        internal override LambdaExpressionSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new SimpleLambdaExpressionSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override AnonymousFunctionExpressionSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new SimpleLambdaExpressionSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        public SimpleLambdaExpressionSyntax AddParameterAttributeLists(params AttributeListSyntax[] items) => WithParameter(this.Parameter.WithAttributeLists(this.Parameter.AttributeLists.AddRange(items)));
        public SimpleLambdaExpressionSyntax AddParameterModifiers(params SyntaxToken[] items) => WithParameter(this.Parameter.WithModifiers(this.Parameter.Modifiers.AddRange(items)));
        internal override AnonymousFunctionExpressionSyntax AddBlockAttributeListsCore(params AttributeListSyntax[] items) => AddBlockAttributeLists(items);
        public new SimpleLambdaExpressionSyntax AddBlockAttributeLists(params AttributeListSyntax[] items)
        {
            var block = this.Block ?? SyntaxFactory.Block();
            return WithBlock(block.WithAttributeLists(block.AttributeLists.AddRange(items)));
        }
        internal override AnonymousFunctionExpressionSyntax AddBlockStatementsCore(params StatementSyntax[] items) => AddBlockStatements(items);
        public new SimpleLambdaExpressionSyntax AddBlockStatements(params StatementSyntax[] items)
        {
            var block = this.Block ?? SyntaxFactory.Block();
            return WithBlock(block.WithStatements(block.Statements.AddRange(items)));
        }
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.RefExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class RefExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        internal RefExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken RefKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.RefExpressionSyntax)this.Green).refKeyword, Position, 0);

        public ExpressionSyntax Expression => GetRed(ref this.expression, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.expression, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.expression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitRefExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRefExpression(this);

        public RefExpressionSyntax Update(SyntaxToken refKeyword, ExpressionSyntax expression)
        {
            if (refKeyword != this.RefKeyword || expression != this.Expression)
            {
                var newNode = SyntaxFactory.RefExpression(refKeyword, expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public RefExpressionSyntax WithRefKeyword(SyntaxToken refKeyword) => Update(refKeyword, this.Expression);
        public RefExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(this.RefKeyword, expression);
    }

    /// <summary>Class which represents the syntax node for parenthesized lambda expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ParenthesizedLambdaExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ParenthesizedLambdaExpressionSyntax : LambdaExpressionSyntax
    {
        private SyntaxNode? attributeLists;
        private ParameterListSyntax? parameterList;
        private BlockSyntax? block;
        private ExpressionSyntax? expressionBody;

        internal ParenthesizedLambdaExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>ParameterListSyntax node representing the list of parameters for the lambda expression.</summary>
        public ParameterListSyntax ParameterList => GetRed(ref this.parameterList, 2)!;

        /// <summary>SyntaxToken representing equals greater than.</summary>
        public override SyntaxToken ArrowToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ParenthesizedLambdaExpressionSyntax)this.Green).arrowToken, GetChildPosition(3), GetChildIndex(3));

        /// <summary>
        /// BlockSyntax node representing the body of the lambda.
        /// Only one of Block or ExpressionBody will be non-null.
        /// </summary>
        public override BlockSyntax? Block => GetRed(ref this.block, 4);

        /// <summary>
        /// ExpressionSyntax node representing the body of the lambda.
        /// Only one of Block or ExpressionBody will be non-null.
        /// </summary>
        public override ExpressionSyntax? ExpressionBody => GetRed(ref this.expressionBody, 5);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.parameterList, 2)!,
                4 => GetRed(ref this.block, 4),
                5 => GetRed(ref this.expressionBody, 5),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.parameterList,
                4 => this.block,
                5 => this.expressionBody,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitParenthesizedLambdaExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitParenthesizedLambdaExpression(this);

        public ParenthesizedLambdaExpressionSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, ParameterListSyntax parameterList, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || parameterList != this.ParameterList || arrowToken != this.ArrowToken || block != this.Block || expressionBody != this.ExpressionBody)
            {
                var newNode = SyntaxFactory.ParenthesizedLambdaExpression(attributeLists, modifiers, parameterList, arrowToken, block, expressionBody);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override LambdaExpressionSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new ParenthesizedLambdaExpressionSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.ParameterList, this.ArrowToken, this.Block, this.ExpressionBody);
        internal override AnonymousFunctionExpressionSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new ParenthesizedLambdaExpressionSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.ParameterList, this.ArrowToken, this.Block, this.ExpressionBody);
        public ParenthesizedLambdaExpressionSyntax WithParameterList(ParameterListSyntax parameterList) => Update(this.AttributeLists, this.Modifiers, parameterList, this.ArrowToken, this.Block, this.ExpressionBody);
        internal override LambdaExpressionSyntax WithArrowTokenCore(SyntaxToken arrowToken) => WithArrowToken(arrowToken);
        public new ParenthesizedLambdaExpressionSyntax WithArrowToken(SyntaxToken arrowToken) => Update(this.AttributeLists, this.Modifiers, this.ParameterList, arrowToken, this.Block, this.ExpressionBody);
        internal override AnonymousFunctionExpressionSyntax WithBlockCore(BlockSyntax? block) => WithBlock(block);
        public new ParenthesizedLambdaExpressionSyntax WithBlock(BlockSyntax? block) => Update(this.AttributeLists, this.Modifiers, this.ParameterList, this.ArrowToken, block, this.ExpressionBody);
        internal override AnonymousFunctionExpressionSyntax WithExpressionBodyCore(ExpressionSyntax? expressionBody) => WithExpressionBody(expressionBody);
        public new ParenthesizedLambdaExpressionSyntax WithExpressionBody(ExpressionSyntax? expressionBody) => Update(this.AttributeLists, this.Modifiers, this.ParameterList, this.ArrowToken, this.Block, expressionBody);

        internal override LambdaExpressionSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new ParenthesizedLambdaExpressionSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override AnonymousFunctionExpressionSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new ParenthesizedLambdaExpressionSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        public ParenthesizedLambdaExpressionSyntax AddParameterListParameters(params ParameterSyntax[] items) => WithParameterList(this.ParameterList.WithParameters(this.ParameterList.Parameters.AddRange(items)));
        internal override AnonymousFunctionExpressionSyntax AddBlockAttributeListsCore(params AttributeListSyntax[] items) => AddBlockAttributeLists(items);
        public new ParenthesizedLambdaExpressionSyntax AddBlockAttributeLists(params AttributeListSyntax[] items)
        {
            var block = this.Block ?? SyntaxFactory.Block();
            return WithBlock(block.WithAttributeLists(block.AttributeLists.AddRange(items)));
        }
        internal override AnonymousFunctionExpressionSyntax AddBlockStatementsCore(params StatementSyntax[] items) => AddBlockStatements(items);
        public new ParenthesizedLambdaExpressionSyntax AddBlockStatements(params StatementSyntax[] items)
        {
            var block = this.Block ?? SyntaxFactory.Block();
            return WithBlock(block.WithStatements(block.Statements.AddRange(items)));
        }
    }

    /// <summary>Class which represents the syntax node for initializer expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ObjectInitializerExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.CollectionInitializerExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.ArrayInitializerExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.ComplexElementInitializerExpression"/></description></item>
    /// <item><description><see cref="SyntaxKind.WithInitializerExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class InitializerExpressionSyntax : ExpressionSyntax
    {
        private SyntaxNode? expressions;

        internal InitializerExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the open brace.</summary>
        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.InitializerExpressionSyntax)this.Green).openBraceToken, Position, 0);

        /// <summary>SeparatedSyntaxList of ExpressionSyntax representing the list of expressions in the initializer expression.</summary>
        public SeparatedSyntaxList<ExpressionSyntax> Expressions
        {
            get
            {
                var red = GetRed(ref this.expressions, 1);
                return red != null ? new SeparatedSyntaxList<ExpressionSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>SyntaxToken representing the close brace.</summary>
        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.InitializerExpressionSyntax)this.Green).closeBraceToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.expressions, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.expressions : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitInitializerExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitInitializerExpression(this);

        public InitializerExpressionSyntax Update(SyntaxToken openBraceToken, SeparatedSyntaxList<ExpressionSyntax> expressions, SyntaxToken closeBraceToken)
        {
            if (openBraceToken != this.OpenBraceToken || expressions != this.Expressions || closeBraceToken != this.CloseBraceToken)
            {
                var newNode = SyntaxFactory.InitializerExpression(this.Kind(), openBraceToken, expressions, closeBraceToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public InitializerExpressionSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(openBraceToken, this.Expressions, this.CloseBraceToken);
        public InitializerExpressionSyntax WithExpressions(SeparatedSyntaxList<ExpressionSyntax> expressions) => Update(this.OpenBraceToken, expressions, this.CloseBraceToken);
        public InitializerExpressionSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.OpenBraceToken, this.Expressions, closeBraceToken);

        public InitializerExpressionSyntax AddExpressions(params ExpressionSyntax[] items) => WithExpressions(this.Expressions.AddRange(items));
    }

    public abstract partial class BaseObjectCreationExpressionSyntax : ExpressionSyntax
    {
        internal BaseObjectCreationExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the new keyword.</summary>
        public abstract SyntaxToken NewKeyword { get; }
        public BaseObjectCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword) => WithNewKeywordCore(newKeyword);
        internal abstract BaseObjectCreationExpressionSyntax WithNewKeywordCore(SyntaxToken newKeyword);

        /// <summary>ArgumentListSyntax representing the list of arguments passed as part of the object creation expression.</summary>
        public abstract ArgumentListSyntax? ArgumentList { get; }
        public BaseObjectCreationExpressionSyntax WithArgumentList(ArgumentListSyntax? argumentList) => WithArgumentListCore(argumentList);
        internal abstract BaseObjectCreationExpressionSyntax WithArgumentListCore(ArgumentListSyntax? argumentList);

        public BaseObjectCreationExpressionSyntax AddArgumentListArguments(params ArgumentSyntax[] items) => AddArgumentListArgumentsCore(items);
        internal abstract BaseObjectCreationExpressionSyntax AddArgumentListArgumentsCore(params ArgumentSyntax[] items);

        /// <summary>InitializerExpressionSyntax representing the initializer expression for the object being created.</summary>
        public abstract InitializerExpressionSyntax? Initializer { get; }
        public BaseObjectCreationExpressionSyntax WithInitializer(InitializerExpressionSyntax? initializer) => WithInitializerCore(initializer);
        internal abstract BaseObjectCreationExpressionSyntax WithInitializerCore(InitializerExpressionSyntax? initializer);
    }

    /// <summary>Class which represents the syntax node for implicit object creation expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ImplicitObjectCreationExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ImplicitObjectCreationExpressionSyntax : BaseObjectCreationExpressionSyntax
    {
        private ArgumentListSyntax? argumentList;
        private InitializerExpressionSyntax? initializer;

        internal ImplicitObjectCreationExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the new keyword.</summary>
        public override SyntaxToken NewKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ImplicitObjectCreationExpressionSyntax)this.Green).newKeyword, Position, 0);

        /// <summary>ArgumentListSyntax representing the list of arguments passed as part of the object creation expression.</summary>
        public override ArgumentListSyntax ArgumentList => GetRed(ref this.argumentList, 1)!;

        /// <summary>InitializerExpressionSyntax representing the initializer expression for the object being created.</summary>
        public override InitializerExpressionSyntax? Initializer => GetRed(ref this.initializer, 2);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.argumentList, 1)!,
                2 => GetRed(ref this.initializer, 2),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.argumentList,
                2 => this.initializer,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitImplicitObjectCreationExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitImplicitObjectCreationExpression(this);

        public ImplicitObjectCreationExpressionSyntax Update(SyntaxToken newKeyword, ArgumentListSyntax argumentList, InitializerExpressionSyntax? initializer)
        {
            if (newKeyword != this.NewKeyword || argumentList != this.ArgumentList || initializer != this.Initializer)
            {
                var newNode = SyntaxFactory.ImplicitObjectCreationExpression(newKeyword, argumentList, initializer);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override BaseObjectCreationExpressionSyntax WithNewKeywordCore(SyntaxToken newKeyword) => WithNewKeyword(newKeyword);
        public new ImplicitObjectCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword) => Update(newKeyword, this.ArgumentList, this.Initializer);
        internal override BaseObjectCreationExpressionSyntax WithArgumentListCore(ArgumentListSyntax? argumentList) => WithArgumentList(argumentList ?? throw new ArgumentNullException(nameof(argumentList)));
        public new ImplicitObjectCreationExpressionSyntax WithArgumentList(ArgumentListSyntax argumentList) => Update(this.NewKeyword, argumentList, this.Initializer);
        internal override BaseObjectCreationExpressionSyntax WithInitializerCore(InitializerExpressionSyntax? initializer) => WithInitializer(initializer);
        public new ImplicitObjectCreationExpressionSyntax WithInitializer(InitializerExpressionSyntax? initializer) => Update(this.NewKeyword, this.ArgumentList, initializer);

        internal override BaseObjectCreationExpressionSyntax AddArgumentListArgumentsCore(params ArgumentSyntax[] items) => AddArgumentListArguments(items);
        public new ImplicitObjectCreationExpressionSyntax AddArgumentListArguments(params ArgumentSyntax[] items) => WithArgumentList(this.ArgumentList.WithArguments(this.ArgumentList.Arguments.AddRange(items)));
    }

    /// <summary>Class which represents the syntax node for object creation expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ObjectCreationExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ObjectCreationExpressionSyntax : BaseObjectCreationExpressionSyntax
    {
        private TypeSyntax? type;
        private ArgumentListSyntax? argumentList;
        private InitializerExpressionSyntax? initializer;

        internal ObjectCreationExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the new keyword.</summary>
        public override SyntaxToken NewKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ObjectCreationExpressionSyntax)this.Green).newKeyword, Position, 0);

        /// <summary>TypeSyntax representing the type of the object being created.</summary>
        public TypeSyntax Type => GetRed(ref this.type, 1)!;

        /// <summary>ArgumentListSyntax representing the list of arguments passed as part of the object creation expression.</summary>
        public override ArgumentListSyntax? ArgumentList => GetRed(ref this.argumentList, 2);

        /// <summary>InitializerExpressionSyntax representing the initializer expression for the object being created.</summary>
        public override InitializerExpressionSyntax? Initializer => GetRed(ref this.initializer, 3);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.type, 1)!,
                2 => GetRed(ref this.argumentList, 2),
                3 => GetRed(ref this.initializer, 3),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.type,
                2 => this.argumentList,
                3 => this.initializer,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitObjectCreationExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitObjectCreationExpression(this);

        public ObjectCreationExpressionSyntax Update(SyntaxToken newKeyword, TypeSyntax type, ArgumentListSyntax? argumentList, InitializerExpressionSyntax? initializer)
        {
            if (newKeyword != this.NewKeyword || type != this.Type || argumentList != this.ArgumentList || initializer != this.Initializer)
            {
                var newNode = SyntaxFactory.ObjectCreationExpression(newKeyword, type, argumentList, initializer);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override BaseObjectCreationExpressionSyntax WithNewKeywordCore(SyntaxToken newKeyword) => WithNewKeyword(newKeyword);
        public new ObjectCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword) => Update(newKeyword, this.Type, this.ArgumentList, this.Initializer);
        public ObjectCreationExpressionSyntax WithType(TypeSyntax type) => Update(this.NewKeyword, type, this.ArgumentList, this.Initializer);
        internal override BaseObjectCreationExpressionSyntax WithArgumentListCore(ArgumentListSyntax? argumentList) => WithArgumentList(argumentList);
        public new ObjectCreationExpressionSyntax WithArgumentList(ArgumentListSyntax? argumentList) => Update(this.NewKeyword, this.Type, argumentList, this.Initializer);
        internal override BaseObjectCreationExpressionSyntax WithInitializerCore(InitializerExpressionSyntax? initializer) => WithInitializer(initializer);
        public new ObjectCreationExpressionSyntax WithInitializer(InitializerExpressionSyntax? initializer) => Update(this.NewKeyword, this.Type, this.ArgumentList, initializer);

        internal override BaseObjectCreationExpressionSyntax AddArgumentListArgumentsCore(params ArgumentSyntax[] items) => AddArgumentListArguments(items);
        public new ObjectCreationExpressionSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
        {
            var argumentList = this.ArgumentList ?? SyntaxFactory.ArgumentList();
            return WithArgumentList(argumentList.WithArguments(argumentList.Arguments.AddRange(items)));
        }
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.WithExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class WithExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;
        private InitializerExpressionSyntax? initializer;

        internal WithExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public ExpressionSyntax Expression => GetRedAtZero(ref this.expression)!;

        public SyntaxToken WithKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.WithExpressionSyntax)this.Green).withKeyword, GetChildPosition(1), GetChildIndex(1));

        /// <summary>InitializerExpressionSyntax representing the initializer expression for the with expression.</summary>
        public InitializerExpressionSyntax Initializer => GetRed(ref this.initializer, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.expression)!,
                2 => GetRed(ref this.initializer, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.expression,
                2 => this.initializer,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitWithExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitWithExpression(this);

        public WithExpressionSyntax Update(ExpressionSyntax expression, SyntaxToken withKeyword, InitializerExpressionSyntax initializer)
        {
            if (expression != this.Expression || withKeyword != this.WithKeyword || initializer != this.Initializer)
            {
                var newNode = SyntaxFactory.WithExpression(expression, withKeyword, initializer);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public WithExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(expression, this.WithKeyword, this.Initializer);
        public WithExpressionSyntax WithWithKeyword(SyntaxToken withKeyword) => Update(this.Expression, withKeyword, this.Initializer);
        public WithExpressionSyntax WithInitializer(InitializerExpressionSyntax initializer) => Update(this.Expression, this.WithKeyword, initializer);

        public WithExpressionSyntax AddInitializerExpressions(params ExpressionSyntax[] items) => WithInitializer(this.Initializer.WithExpressions(this.Initializer.Expressions.AddRange(items)));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.AnonymousObjectMemberDeclarator"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class AnonymousObjectMemberDeclaratorSyntax : CSharpSyntaxNode
    {
        private NameEqualsSyntax? nameEquals;
        private ExpressionSyntax? expression;

        internal AnonymousObjectMemberDeclaratorSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>NameEqualsSyntax representing the optional name of the member being initialized.</summary>
        public NameEqualsSyntax? NameEquals => GetRedAtZero(ref this.nameEquals);

        /// <summary>ExpressionSyntax representing the value the member is initialized with.</summary>
        public ExpressionSyntax Expression => GetRed(ref this.expression, 1)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.nameEquals),
                1 => GetRed(ref this.expression, 1)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.nameEquals,
                1 => this.expression,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitAnonymousObjectMemberDeclarator(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAnonymousObjectMemberDeclarator(this);

        public AnonymousObjectMemberDeclaratorSyntax Update(NameEqualsSyntax? nameEquals, ExpressionSyntax expression)
        {
            if (nameEquals != this.NameEquals || expression != this.Expression)
            {
                var newNode = SyntaxFactory.AnonymousObjectMemberDeclarator(nameEquals, expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public AnonymousObjectMemberDeclaratorSyntax WithNameEquals(NameEqualsSyntax? nameEquals) => Update(nameEquals, this.Expression);
        public AnonymousObjectMemberDeclaratorSyntax WithExpression(ExpressionSyntax expression) => Update(this.NameEquals, expression);
    }

    /// <summary>Class which represents the syntax node for anonymous object creation expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.AnonymousObjectCreationExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class AnonymousObjectCreationExpressionSyntax : ExpressionSyntax
    {
        private SyntaxNode? initializers;

        internal AnonymousObjectCreationExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the new keyword.</summary>
        public SyntaxToken NewKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax)this.Green).newKeyword, Position, 0);

        /// <summary>SyntaxToken representing the open brace.</summary>
        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax)this.Green).openBraceToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>SeparatedSyntaxList of AnonymousObjectMemberDeclaratorSyntax representing the list of object member initializers.</summary>
        public SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax> Initializers
        {
            get
            {
                var red = GetRed(ref this.initializers, 2);
                return red != null ? new SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax>(red, GetChildIndex(2)) : default;
            }
        }

        /// <summary>SyntaxToken representing the close brace.</summary>
        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax)this.Green).closeBraceToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.initializers, 2)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.initializers : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitAnonymousObjectCreationExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAnonymousObjectCreationExpression(this);

        public AnonymousObjectCreationExpressionSyntax Update(SyntaxToken newKeyword, SyntaxToken openBraceToken, SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax> initializers, SyntaxToken closeBraceToken)
        {
            if (newKeyword != this.NewKeyword || openBraceToken != this.OpenBraceToken || initializers != this.Initializers || closeBraceToken != this.CloseBraceToken)
            {
                var newNode = SyntaxFactory.AnonymousObjectCreationExpression(newKeyword, openBraceToken, initializers, closeBraceToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public AnonymousObjectCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword) => Update(newKeyword, this.OpenBraceToken, this.Initializers, this.CloseBraceToken);
        public AnonymousObjectCreationExpressionSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(this.NewKeyword, openBraceToken, this.Initializers, this.CloseBraceToken);
        public AnonymousObjectCreationExpressionSyntax WithInitializers(SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax> initializers) => Update(this.NewKeyword, this.OpenBraceToken, initializers, this.CloseBraceToken);
        public AnonymousObjectCreationExpressionSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.NewKeyword, this.OpenBraceToken, this.Initializers, closeBraceToken);

        public AnonymousObjectCreationExpressionSyntax AddInitializers(params AnonymousObjectMemberDeclaratorSyntax[] items) => WithInitializers(this.Initializers.AddRange(items));
    }

    /// <summary>Class which represents the syntax node for array creation expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ArrayCreationExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ArrayCreationExpressionSyntax : ExpressionSyntax
    {
        private ArrayTypeSyntax? type;
        private InitializerExpressionSyntax? initializer;

        internal ArrayCreationExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the new keyword.</summary>
        public SyntaxToken NewKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ArrayCreationExpressionSyntax)this.Green).newKeyword, Position, 0);

        /// <summary>ArrayTypeSyntax node representing the type of the array.</summary>
        public ArrayTypeSyntax Type => GetRed(ref this.type, 1)!;

        /// <summary>InitializerExpressionSyntax node representing the initializer of the array creation expression.</summary>
        public InitializerExpressionSyntax? Initializer => GetRed(ref this.initializer, 2);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.type, 1)!,
                2 => GetRed(ref this.initializer, 2),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.type,
                2 => this.initializer,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitArrayCreationExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitArrayCreationExpression(this);

        public ArrayCreationExpressionSyntax Update(SyntaxToken newKeyword, ArrayTypeSyntax type, InitializerExpressionSyntax? initializer)
        {
            if (newKeyword != this.NewKeyword || type != this.Type || initializer != this.Initializer)
            {
                var newNode = SyntaxFactory.ArrayCreationExpression(newKeyword, type, initializer);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ArrayCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword) => Update(newKeyword, this.Type, this.Initializer);
        public ArrayCreationExpressionSyntax WithType(ArrayTypeSyntax type) => Update(this.NewKeyword, type, this.Initializer);
        public ArrayCreationExpressionSyntax WithInitializer(InitializerExpressionSyntax? initializer) => Update(this.NewKeyword, this.Type, initializer);

        public ArrayCreationExpressionSyntax AddTypeRankSpecifiers(params ArrayRankSpecifierSyntax[] items) => WithType(this.Type.WithRankSpecifiers(this.Type.RankSpecifiers.AddRange(items)));
    }

    /// <summary>Class which represents the syntax node for implicit array creation expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ImplicitArrayCreationExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ImplicitArrayCreationExpressionSyntax : ExpressionSyntax
    {
        private InitializerExpressionSyntax? initializer;

        internal ImplicitArrayCreationExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the new keyword.</summary>
        public SyntaxToken NewKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ImplicitArrayCreationExpressionSyntax)this.Green).newKeyword, Position, 0);

        /// <summary>SyntaxToken representing the open bracket.</summary>
        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ImplicitArrayCreationExpressionSyntax)this.Green).openBracketToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>SyntaxList of SyntaxToken representing the commas in the implicit array creation expression.</summary>
        public SyntaxTokenList Commas
        {
            get
            {
                var slot = this.Green.GetSlot(2);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(2), GetChildIndex(2)) : default;
            }
        }

        /// <summary>SyntaxToken representing the close bracket.</summary>
        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ImplicitArrayCreationExpressionSyntax)this.Green).closeBracketToken, GetChildPosition(3), GetChildIndex(3));

        /// <summary>InitializerExpressionSyntax representing the initializer expression of the implicit array creation expression.</summary>
        public InitializerExpressionSyntax Initializer => GetRed(ref this.initializer, 4)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 4 ? GetRed(ref this.initializer, 4)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 4 ? this.initializer : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitImplicitArrayCreationExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitImplicitArrayCreationExpression(this);

        public ImplicitArrayCreationExpressionSyntax Update(SyntaxToken newKeyword, SyntaxToken openBracketToken, SyntaxTokenList commas, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer)
        {
            if (newKeyword != this.NewKeyword || openBracketToken != this.OpenBracketToken || commas != this.Commas || closeBracketToken != this.CloseBracketToken || initializer != this.Initializer)
            {
                var newNode = SyntaxFactory.ImplicitArrayCreationExpression(newKeyword, openBracketToken, commas, closeBracketToken, initializer);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ImplicitArrayCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword) => Update(newKeyword, this.OpenBracketToken, this.Commas, this.CloseBracketToken, this.Initializer);
        public ImplicitArrayCreationExpressionSyntax WithOpenBracketToken(SyntaxToken openBracketToken) => Update(this.NewKeyword, openBracketToken, this.Commas, this.CloseBracketToken, this.Initializer);
        public ImplicitArrayCreationExpressionSyntax WithCommas(SyntaxTokenList commas) => Update(this.NewKeyword, this.OpenBracketToken, commas, this.CloseBracketToken, this.Initializer);
        public ImplicitArrayCreationExpressionSyntax WithCloseBracketToken(SyntaxToken closeBracketToken) => Update(this.NewKeyword, this.OpenBracketToken, this.Commas, closeBracketToken, this.Initializer);
        public ImplicitArrayCreationExpressionSyntax WithInitializer(InitializerExpressionSyntax initializer) => Update(this.NewKeyword, this.OpenBracketToken, this.Commas, this.CloseBracketToken, initializer);

        public ImplicitArrayCreationExpressionSyntax AddCommas(params SyntaxToken[] items) => WithCommas(this.Commas.AddRange(items));
        public ImplicitArrayCreationExpressionSyntax AddInitializerExpressions(params ExpressionSyntax[] items) => WithInitializer(this.Initializer.WithExpressions(this.Initializer.Expressions.AddRange(items)));
    }

    /// <summary>Class which represents the syntax node for stackalloc array creation expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.StackAllocArrayCreationExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class StackAllocArrayCreationExpressionSyntax : ExpressionSyntax
    {
        private TypeSyntax? type;
        private InitializerExpressionSyntax? initializer;

        internal StackAllocArrayCreationExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the stackalloc keyword.</summary>
        public SyntaxToken StackAllocKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.StackAllocArrayCreationExpressionSyntax)this.Green).stackAllocKeyword, Position, 0);

        /// <summary>TypeSyntax node representing the type of the stackalloc array.</summary>
        public TypeSyntax Type => GetRed(ref this.type, 1)!;

        /// <summary>InitializerExpressionSyntax node representing the initializer of the stackalloc array creation expression.</summary>
        public InitializerExpressionSyntax? Initializer => GetRed(ref this.initializer, 2);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.type, 1)!,
                2 => GetRed(ref this.initializer, 2),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.type,
                2 => this.initializer,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitStackAllocArrayCreationExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitStackAllocArrayCreationExpression(this);

        public StackAllocArrayCreationExpressionSyntax Update(SyntaxToken stackAllocKeyword, TypeSyntax type, InitializerExpressionSyntax? initializer)
        {
            if (stackAllocKeyword != this.StackAllocKeyword || type != this.Type || initializer != this.Initializer)
            {
                var newNode = SyntaxFactory.StackAllocArrayCreationExpression(stackAllocKeyword, type, initializer);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public StackAllocArrayCreationExpressionSyntax WithStackAllocKeyword(SyntaxToken stackAllocKeyword) => Update(stackAllocKeyword, this.Type, this.Initializer);
        public StackAllocArrayCreationExpressionSyntax WithType(TypeSyntax type) => Update(this.StackAllocKeyword, type, this.Initializer);
        public StackAllocArrayCreationExpressionSyntax WithInitializer(InitializerExpressionSyntax? initializer) => Update(this.StackAllocKeyword, this.Type, initializer);
    }

    /// <summary>Class which represents the syntax node for implicit stackalloc array creation expression.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ImplicitStackAllocArrayCreationExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ImplicitStackAllocArrayCreationExpressionSyntax : ExpressionSyntax
    {
        private InitializerExpressionSyntax? initializer;

        internal ImplicitStackAllocArrayCreationExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the stackalloc keyword.</summary>
        public SyntaxToken StackAllocKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ImplicitStackAllocArrayCreationExpressionSyntax)this.Green).stackAllocKeyword, Position, 0);

        /// <summary>SyntaxToken representing the open bracket.</summary>
        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ImplicitStackAllocArrayCreationExpressionSyntax)this.Green).openBracketToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>SyntaxToken representing the close bracket.</summary>
        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ImplicitStackAllocArrayCreationExpressionSyntax)this.Green).closeBracketToken, GetChildPosition(2), GetChildIndex(2));

        /// <summary>InitializerExpressionSyntax representing the initializer expression of the implicit stackalloc array creation expression.</summary>
        public InitializerExpressionSyntax Initializer => GetRed(ref this.initializer, 3)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 3 ? GetRed(ref this.initializer, 3)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 3 ? this.initializer : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitImplicitStackAllocArrayCreationExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitImplicitStackAllocArrayCreationExpression(this);

        public ImplicitStackAllocArrayCreationExpressionSyntax Update(SyntaxToken stackAllocKeyword, SyntaxToken openBracketToken, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer)
        {
            if (stackAllocKeyword != this.StackAllocKeyword || openBracketToken != this.OpenBracketToken || closeBracketToken != this.CloseBracketToken || initializer != this.Initializer)
            {
                var newNode = SyntaxFactory.ImplicitStackAllocArrayCreationExpression(stackAllocKeyword, openBracketToken, closeBracketToken, initializer);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ImplicitStackAllocArrayCreationExpressionSyntax WithStackAllocKeyword(SyntaxToken stackAllocKeyword) => Update(stackAllocKeyword, this.OpenBracketToken, this.CloseBracketToken, this.Initializer);
        public ImplicitStackAllocArrayCreationExpressionSyntax WithOpenBracketToken(SyntaxToken openBracketToken) => Update(this.StackAllocKeyword, openBracketToken, this.CloseBracketToken, this.Initializer);
        public ImplicitStackAllocArrayCreationExpressionSyntax WithCloseBracketToken(SyntaxToken closeBracketToken) => Update(this.StackAllocKeyword, this.OpenBracketToken, closeBracketToken, this.Initializer);
        public ImplicitStackAllocArrayCreationExpressionSyntax WithInitializer(InitializerExpressionSyntax initializer) => Update(this.StackAllocKeyword, this.OpenBracketToken, this.CloseBracketToken, initializer);

        public ImplicitStackAllocArrayCreationExpressionSyntax AddInitializerExpressions(params ExpressionSyntax[] items) => WithInitializer(this.Initializer.WithExpressions(this.Initializer.Expressions.AddRange(items)));
    }

    public abstract partial class QueryClauseSyntax : CSharpSyntaxNode
    {
        internal QueryClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    public abstract partial class SelectOrGroupClauseSyntax : CSharpSyntaxNode
    {
        internal SelectOrGroupClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.QueryExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class QueryExpressionSyntax : ExpressionSyntax
    {
        private FromClauseSyntax? fromClause;
        private QueryBodySyntax? body;

        internal QueryExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public FromClauseSyntax FromClause => GetRedAtZero(ref this.fromClause)!;

        public QueryBodySyntax Body => GetRed(ref this.body, 1)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.fromClause)!,
                1 => GetRed(ref this.body, 1)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.fromClause,
                1 => this.body,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitQueryExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitQueryExpression(this);

        public QueryExpressionSyntax Update(FromClauseSyntax fromClause, QueryBodySyntax body)
        {
            if (fromClause != this.FromClause || body != this.Body)
            {
                var newNode = SyntaxFactory.QueryExpression(fromClause, body);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public QueryExpressionSyntax WithFromClause(FromClauseSyntax fromClause) => Update(fromClause, this.Body);
        public QueryExpressionSyntax WithBody(QueryBodySyntax body) => Update(this.FromClause, body);

        public QueryExpressionSyntax AddBodyClauses(params QueryClauseSyntax[] items) => WithBody(this.Body.WithClauses(this.Body.Clauses.AddRange(items)));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.QueryBody"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class QueryBodySyntax : CSharpSyntaxNode
    {
        private SyntaxNode? clauses;
        private SelectOrGroupClauseSyntax? selectOrGroup;
        private QueryContinuationSyntax? continuation;

        internal QueryBodySyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxList<QueryClauseSyntax> Clauses => new SyntaxList<QueryClauseSyntax>(GetRed(ref this.clauses, 0));

        public SelectOrGroupClauseSyntax SelectOrGroup => GetRed(ref this.selectOrGroup, 1)!;

        public QueryContinuationSyntax? Continuation => GetRed(ref this.continuation, 2);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.clauses)!,
                1 => GetRed(ref this.selectOrGroup, 1)!,
                2 => GetRed(ref this.continuation, 2),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.clauses,
                1 => this.selectOrGroup,
                2 => this.continuation,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitQueryBody(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitQueryBody(this);

        public QueryBodySyntax Update(SyntaxList<QueryClauseSyntax> clauses, SelectOrGroupClauseSyntax selectOrGroup, QueryContinuationSyntax? continuation)
        {
            if (clauses != this.Clauses || selectOrGroup != this.SelectOrGroup || continuation != this.Continuation)
            {
                var newNode = SyntaxFactory.QueryBody(clauses, selectOrGroup, continuation);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public QueryBodySyntax WithClauses(SyntaxList<QueryClauseSyntax> clauses) => Update(clauses, this.SelectOrGroup, this.Continuation);
        public QueryBodySyntax WithSelectOrGroup(SelectOrGroupClauseSyntax selectOrGroup) => Update(this.Clauses, selectOrGroup, this.Continuation);
        public QueryBodySyntax WithContinuation(QueryContinuationSyntax? continuation) => Update(this.Clauses, this.SelectOrGroup, continuation);

        public QueryBodySyntax AddClauses(params QueryClauseSyntax[] items) => WithClauses(this.Clauses.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.FromClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class FromClauseSyntax : QueryClauseSyntax
    {
        private TypeSyntax? type;
        private ExpressionSyntax? expression;

        internal FromClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken FromKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.FromClauseSyntax)this.Green).fromKeyword, Position, 0);

        public TypeSyntax? Type => GetRed(ref this.type, 1);

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.FromClauseSyntax)this.Green).identifier, GetChildPosition(2), GetChildIndex(2));

        public SyntaxToken InKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.FromClauseSyntax)this.Green).inKeyword, GetChildPosition(3), GetChildIndex(3));

        public ExpressionSyntax Expression => GetRed(ref this.expression, 4)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.type, 1),
                4 => GetRed(ref this.expression, 4)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.type,
                4 => this.expression,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitFromClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFromClause(this);

        public FromClauseSyntax Update(SyntaxToken fromKeyword, TypeSyntax? type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression)
        {
            if (fromKeyword != this.FromKeyword || type != this.Type || identifier != this.Identifier || inKeyword != this.InKeyword || expression != this.Expression)
            {
                var newNode = SyntaxFactory.FromClause(fromKeyword, type, identifier, inKeyword, expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public FromClauseSyntax WithFromKeyword(SyntaxToken fromKeyword) => Update(fromKeyword, this.Type, this.Identifier, this.InKeyword, this.Expression);
        public FromClauseSyntax WithType(TypeSyntax? type) => Update(this.FromKeyword, type, this.Identifier, this.InKeyword, this.Expression);
        public FromClauseSyntax WithIdentifier(SyntaxToken identifier) => Update(this.FromKeyword, this.Type, identifier, this.InKeyword, this.Expression);
        public FromClauseSyntax WithInKeyword(SyntaxToken inKeyword) => Update(this.FromKeyword, this.Type, this.Identifier, inKeyword, this.Expression);
        public FromClauseSyntax WithExpression(ExpressionSyntax expression) => Update(this.FromKeyword, this.Type, this.Identifier, this.InKeyword, expression);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.LetClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class LetClauseSyntax : QueryClauseSyntax
    {
        private ExpressionSyntax? expression;

        internal LetClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken LetKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.LetClauseSyntax)this.Green).letKeyword, Position, 0);

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.LetClauseSyntax)this.Green).identifier, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken EqualsToken => new SyntaxToken(this, ((Syntax.InternalSyntax.LetClauseSyntax)this.Green).equalsToken, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax Expression => GetRed(ref this.expression, 3)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 3 ? GetRed(ref this.expression, 3)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 3 ? this.expression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitLetClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLetClause(this);

        public LetClauseSyntax Update(SyntaxToken letKeyword, SyntaxToken identifier, SyntaxToken equalsToken, ExpressionSyntax expression)
        {
            if (letKeyword != this.LetKeyword || identifier != this.Identifier || equalsToken != this.EqualsToken || expression != this.Expression)
            {
                var newNode = SyntaxFactory.LetClause(letKeyword, identifier, equalsToken, expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public LetClauseSyntax WithLetKeyword(SyntaxToken letKeyword) => Update(letKeyword, this.Identifier, this.EqualsToken, this.Expression);
        public LetClauseSyntax WithIdentifier(SyntaxToken identifier) => Update(this.LetKeyword, identifier, this.EqualsToken, this.Expression);
        public LetClauseSyntax WithEqualsToken(SyntaxToken equalsToken) => Update(this.LetKeyword, this.Identifier, equalsToken, this.Expression);
        public LetClauseSyntax WithExpression(ExpressionSyntax expression) => Update(this.LetKeyword, this.Identifier, this.EqualsToken, expression);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.JoinClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class JoinClauseSyntax : QueryClauseSyntax
    {
        private TypeSyntax? type;
        private ExpressionSyntax? inExpression;
        private ExpressionSyntax? leftExpression;
        private ExpressionSyntax? rightExpression;
        private JoinIntoClauseSyntax? into;

        internal JoinClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken JoinKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.JoinClauseSyntax)this.Green).joinKeyword, Position, 0);

        public TypeSyntax? Type => GetRed(ref this.type, 1);

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.JoinClauseSyntax)this.Green).identifier, GetChildPosition(2), GetChildIndex(2));

        public SyntaxToken InKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.JoinClauseSyntax)this.Green).inKeyword, GetChildPosition(3), GetChildIndex(3));

        public ExpressionSyntax InExpression => GetRed(ref this.inExpression, 4)!;

        public SyntaxToken OnKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.JoinClauseSyntax)this.Green).onKeyword, GetChildPosition(5), GetChildIndex(5));

        public ExpressionSyntax LeftExpression => GetRed(ref this.leftExpression, 6)!;

        public SyntaxToken EqualsKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.JoinClauseSyntax)this.Green).equalsKeyword, GetChildPosition(7), GetChildIndex(7));

        public ExpressionSyntax RightExpression => GetRed(ref this.rightExpression, 8)!;

        public JoinIntoClauseSyntax? Into => GetRed(ref this.into, 9);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.type, 1),
                4 => GetRed(ref this.inExpression, 4)!,
                6 => GetRed(ref this.leftExpression, 6)!,
                8 => GetRed(ref this.rightExpression, 8)!,
                9 => GetRed(ref this.into, 9),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.type,
                4 => this.inExpression,
                6 => this.leftExpression,
                8 => this.rightExpression,
                9 => this.into,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitJoinClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitJoinClause(this);

        public JoinClauseSyntax Update(SyntaxToken joinKeyword, TypeSyntax? type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax inExpression, SyntaxToken onKeyword, ExpressionSyntax leftExpression, SyntaxToken equalsKeyword, ExpressionSyntax rightExpression, JoinIntoClauseSyntax? into)
        {
            if (joinKeyword != this.JoinKeyword || type != this.Type || identifier != this.Identifier || inKeyword != this.InKeyword || inExpression != this.InExpression || onKeyword != this.OnKeyword || leftExpression != this.LeftExpression || equalsKeyword != this.EqualsKeyword || rightExpression != this.RightExpression || into != this.Into)
            {
                var newNode = SyntaxFactory.JoinClause(joinKeyword, type, identifier, inKeyword, inExpression, onKeyword, leftExpression, equalsKeyword, rightExpression, into);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public JoinClauseSyntax WithJoinKeyword(SyntaxToken joinKeyword) => Update(joinKeyword, this.Type, this.Identifier, this.InKeyword, this.InExpression, this.OnKeyword, this.LeftExpression, this.EqualsKeyword, this.RightExpression, this.Into);
        public JoinClauseSyntax WithType(TypeSyntax? type) => Update(this.JoinKeyword, type, this.Identifier, this.InKeyword, this.InExpression, this.OnKeyword, this.LeftExpression, this.EqualsKeyword, this.RightExpression, this.Into);
        public JoinClauseSyntax WithIdentifier(SyntaxToken identifier) => Update(this.JoinKeyword, this.Type, identifier, this.InKeyword, this.InExpression, this.OnKeyword, this.LeftExpression, this.EqualsKeyword, this.RightExpression, this.Into);
        public JoinClauseSyntax WithInKeyword(SyntaxToken inKeyword) => Update(this.JoinKeyword, this.Type, this.Identifier, inKeyword, this.InExpression, this.OnKeyword, this.LeftExpression, this.EqualsKeyword, this.RightExpression, this.Into);
        public JoinClauseSyntax WithInExpression(ExpressionSyntax inExpression) => Update(this.JoinKeyword, this.Type, this.Identifier, this.InKeyword, inExpression, this.OnKeyword, this.LeftExpression, this.EqualsKeyword, this.RightExpression, this.Into);
        public JoinClauseSyntax WithOnKeyword(SyntaxToken onKeyword) => Update(this.JoinKeyword, this.Type, this.Identifier, this.InKeyword, this.InExpression, onKeyword, this.LeftExpression, this.EqualsKeyword, this.RightExpression, this.Into);
        public JoinClauseSyntax WithLeftExpression(ExpressionSyntax leftExpression) => Update(this.JoinKeyword, this.Type, this.Identifier, this.InKeyword, this.InExpression, this.OnKeyword, leftExpression, this.EqualsKeyword, this.RightExpression, this.Into);
        public JoinClauseSyntax WithEqualsKeyword(SyntaxToken equalsKeyword) => Update(this.JoinKeyword, this.Type, this.Identifier, this.InKeyword, this.InExpression, this.OnKeyword, this.LeftExpression, equalsKeyword, this.RightExpression, this.Into);
        public JoinClauseSyntax WithRightExpression(ExpressionSyntax rightExpression) => Update(this.JoinKeyword, this.Type, this.Identifier, this.InKeyword, this.InExpression, this.OnKeyword, this.LeftExpression, this.EqualsKeyword, rightExpression, this.Into);
        public JoinClauseSyntax WithInto(JoinIntoClauseSyntax? into) => Update(this.JoinKeyword, this.Type, this.Identifier, this.InKeyword, this.InExpression, this.OnKeyword, this.LeftExpression, this.EqualsKeyword, this.RightExpression, into);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.JoinIntoClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class JoinIntoClauseSyntax : CSharpSyntaxNode
    {

        internal JoinIntoClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken IntoKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.JoinIntoClauseSyntax)this.Green).intoKeyword, Position, 0);

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.JoinIntoClauseSyntax)this.Green).identifier, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitJoinIntoClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitJoinIntoClause(this);

        public JoinIntoClauseSyntax Update(SyntaxToken intoKeyword, SyntaxToken identifier)
        {
            if (intoKeyword != this.IntoKeyword || identifier != this.Identifier)
            {
                var newNode = SyntaxFactory.JoinIntoClause(intoKeyword, identifier);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public JoinIntoClauseSyntax WithIntoKeyword(SyntaxToken intoKeyword) => Update(intoKeyword, this.Identifier);
        public JoinIntoClauseSyntax WithIdentifier(SyntaxToken identifier) => Update(this.IntoKeyword, identifier);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.WhereClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class WhereClauseSyntax : QueryClauseSyntax
    {
        private ExpressionSyntax? condition;

        internal WhereClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken WhereKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.WhereClauseSyntax)this.Green).whereKeyword, Position, 0);

        public ExpressionSyntax Condition => GetRed(ref this.condition, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.condition, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.condition : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitWhereClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitWhereClause(this);

        public WhereClauseSyntax Update(SyntaxToken whereKeyword, ExpressionSyntax condition)
        {
            if (whereKeyword != this.WhereKeyword || condition != this.Condition)
            {
                var newNode = SyntaxFactory.WhereClause(whereKeyword, condition);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public WhereClauseSyntax WithWhereKeyword(SyntaxToken whereKeyword) => Update(whereKeyword, this.Condition);
        public WhereClauseSyntax WithCondition(ExpressionSyntax condition) => Update(this.WhereKeyword, condition);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.OrderByClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class OrderByClauseSyntax : QueryClauseSyntax
    {
        private SyntaxNode? orderings;

        internal OrderByClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken OrderByKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.OrderByClauseSyntax)this.Green).orderByKeyword, Position, 0);

        public SeparatedSyntaxList<OrderingSyntax> Orderings
        {
            get
            {
                var red = GetRed(ref this.orderings, 1);
                return red != null ? new SeparatedSyntaxList<OrderingSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.orderings, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.orderings : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitOrderByClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitOrderByClause(this);

        public OrderByClauseSyntax Update(SyntaxToken orderByKeyword, SeparatedSyntaxList<OrderingSyntax> orderings)
        {
            if (orderByKeyword != this.OrderByKeyword || orderings != this.Orderings)
            {
                var newNode = SyntaxFactory.OrderByClause(orderByKeyword, orderings);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public OrderByClauseSyntax WithOrderByKeyword(SyntaxToken orderByKeyword) => Update(orderByKeyword, this.Orderings);
        public OrderByClauseSyntax WithOrderings(SeparatedSyntaxList<OrderingSyntax> orderings) => Update(this.OrderByKeyword, orderings);

        public OrderByClauseSyntax AddOrderings(params OrderingSyntax[] items) => WithOrderings(this.Orderings.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.AscendingOrdering"/></description></item>
    /// <item><description><see cref="SyntaxKind.DescendingOrdering"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class OrderingSyntax : CSharpSyntaxNode
    {
        private ExpressionSyntax? expression;

        internal OrderingSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public ExpressionSyntax Expression => GetRedAtZero(ref this.expression)!;

        public SyntaxToken AscendingOrDescendingKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.OrderingSyntax)this.Green).ascendingOrDescendingKeyword;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.expression)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.expression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitOrdering(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitOrdering(this);

        public OrderingSyntax Update(ExpressionSyntax expression, SyntaxToken ascendingOrDescendingKeyword)
        {
            if (expression != this.Expression || ascendingOrDescendingKeyword != this.AscendingOrDescendingKeyword)
            {
                var newNode = SyntaxFactory.Ordering(this.Kind(), expression, ascendingOrDescendingKeyword);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public OrderingSyntax WithExpression(ExpressionSyntax expression) => Update(expression, this.AscendingOrDescendingKeyword);
        public OrderingSyntax WithAscendingOrDescendingKeyword(SyntaxToken ascendingOrDescendingKeyword) => Update(this.Expression, ascendingOrDescendingKeyword);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.SelectClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class SelectClauseSyntax : SelectOrGroupClauseSyntax
    {
        private ExpressionSyntax? expression;

        internal SelectClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken SelectKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.SelectClauseSyntax)this.Green).selectKeyword, Position, 0);

        public ExpressionSyntax Expression => GetRed(ref this.expression, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.expression, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.expression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitSelectClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSelectClause(this);

        public SelectClauseSyntax Update(SyntaxToken selectKeyword, ExpressionSyntax expression)
        {
            if (selectKeyword != this.SelectKeyword || expression != this.Expression)
            {
                var newNode = SyntaxFactory.SelectClause(selectKeyword, expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public SelectClauseSyntax WithSelectKeyword(SyntaxToken selectKeyword) => Update(selectKeyword, this.Expression);
        public SelectClauseSyntax WithExpression(ExpressionSyntax expression) => Update(this.SelectKeyword, expression);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.GroupClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class GroupClauseSyntax : SelectOrGroupClauseSyntax
    {
        private ExpressionSyntax? groupExpression;
        private ExpressionSyntax? byExpression;

        internal GroupClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken GroupKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.GroupClauseSyntax)this.Green).groupKeyword, Position, 0);

        public ExpressionSyntax GroupExpression => GetRed(ref this.groupExpression, 1)!;

        public SyntaxToken ByKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.GroupClauseSyntax)this.Green).byKeyword, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax ByExpression => GetRed(ref this.byExpression, 3)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.groupExpression, 1)!,
                3 => GetRed(ref this.byExpression, 3)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.groupExpression,
                3 => this.byExpression,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitGroupClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitGroupClause(this);

        public GroupClauseSyntax Update(SyntaxToken groupKeyword, ExpressionSyntax groupExpression, SyntaxToken byKeyword, ExpressionSyntax byExpression)
        {
            if (groupKeyword != this.GroupKeyword || groupExpression != this.GroupExpression || byKeyword != this.ByKeyword || byExpression != this.ByExpression)
            {
                var newNode = SyntaxFactory.GroupClause(groupKeyword, groupExpression, byKeyword, byExpression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public GroupClauseSyntax WithGroupKeyword(SyntaxToken groupKeyword) => Update(groupKeyword, this.GroupExpression, this.ByKeyword, this.ByExpression);
        public GroupClauseSyntax WithGroupExpression(ExpressionSyntax groupExpression) => Update(this.GroupKeyword, groupExpression, this.ByKeyword, this.ByExpression);
        public GroupClauseSyntax WithByKeyword(SyntaxToken byKeyword) => Update(this.GroupKeyword, this.GroupExpression, byKeyword, this.ByExpression);
        public GroupClauseSyntax WithByExpression(ExpressionSyntax byExpression) => Update(this.GroupKeyword, this.GroupExpression, this.ByKeyword, byExpression);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.QueryContinuation"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class QueryContinuationSyntax : CSharpSyntaxNode
    {
        private QueryBodySyntax? body;

        internal QueryContinuationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken IntoKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.QueryContinuationSyntax)this.Green).intoKeyword, Position, 0);

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.QueryContinuationSyntax)this.Green).identifier, GetChildPosition(1), GetChildIndex(1));

        public QueryBodySyntax Body => GetRed(ref this.body, 2)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.body, 2)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.body : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitQueryContinuation(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitQueryContinuation(this);

        public QueryContinuationSyntax Update(SyntaxToken intoKeyword, SyntaxToken identifier, QueryBodySyntax body)
        {
            if (intoKeyword != this.IntoKeyword || identifier != this.Identifier || body != this.Body)
            {
                var newNode = SyntaxFactory.QueryContinuation(intoKeyword, identifier, body);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public QueryContinuationSyntax WithIntoKeyword(SyntaxToken intoKeyword) => Update(intoKeyword, this.Identifier, this.Body);
        public QueryContinuationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.IntoKeyword, identifier, this.Body);
        public QueryContinuationSyntax WithBody(QueryBodySyntax body) => Update(this.IntoKeyword, this.Identifier, body);

        public QueryContinuationSyntax AddBodyClauses(params QueryClauseSyntax[] items) => WithBody(this.Body.WithClauses(this.Body.Clauses.AddRange(items)));
    }

    /// <summary>Class which represents a placeholder in an array size list.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.OmittedArraySizeExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class OmittedArraySizeExpressionSyntax : ExpressionSyntax
    {

        internal OmittedArraySizeExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the omitted array size expression.</summary>
        public SyntaxToken OmittedArraySizeExpressionToken => new SyntaxToken(this, ((Syntax.InternalSyntax.OmittedArraySizeExpressionSyntax)this.Green).omittedArraySizeExpressionToken, Position, 0);

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitOmittedArraySizeExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitOmittedArraySizeExpression(this);

        public OmittedArraySizeExpressionSyntax Update(SyntaxToken omittedArraySizeExpressionToken)
        {
            if (omittedArraySizeExpressionToken != this.OmittedArraySizeExpressionToken)
            {
                var newNode = SyntaxFactory.OmittedArraySizeExpression(omittedArraySizeExpressionToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public OmittedArraySizeExpressionSyntax WithOmittedArraySizeExpressionToken(SyntaxToken omittedArraySizeExpressionToken) => Update(omittedArraySizeExpressionToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.InterpolatedStringExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class InterpolatedStringExpressionSyntax : ExpressionSyntax
    {
        private SyntaxNode? contents;

        internal InterpolatedStringExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>The first part of an interpolated string, $" or $@"</summary>
        public SyntaxToken StringStartToken => new SyntaxToken(this, ((Syntax.InternalSyntax.InterpolatedStringExpressionSyntax)this.Green).stringStartToken, Position, 0);

        /// <summary>List of parts of the interpolated string, each one is either a literal part or an interpolation.</summary>
        public SyntaxList<InterpolatedStringContentSyntax> Contents => new SyntaxList<InterpolatedStringContentSyntax>(GetRed(ref this.contents, 1));

        /// <summary>The closing quote of the interpolated string.</summary>
        public SyntaxToken StringEndToken => new SyntaxToken(this, ((Syntax.InternalSyntax.InterpolatedStringExpressionSyntax)this.Green).stringEndToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.contents, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.contents : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitInterpolatedStringExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitInterpolatedStringExpression(this);

        public InterpolatedStringExpressionSyntax Update(SyntaxToken stringStartToken, SyntaxList<InterpolatedStringContentSyntax> contents, SyntaxToken stringEndToken)
        {
            if (stringStartToken != this.StringStartToken || contents != this.Contents || stringEndToken != this.StringEndToken)
            {
                var newNode = SyntaxFactory.InterpolatedStringExpression(stringStartToken, contents, stringEndToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public InterpolatedStringExpressionSyntax WithStringStartToken(SyntaxToken stringStartToken) => Update(stringStartToken, this.Contents, this.StringEndToken);
        public InterpolatedStringExpressionSyntax WithContents(SyntaxList<InterpolatedStringContentSyntax> contents) => Update(this.StringStartToken, contents, this.StringEndToken);
        public InterpolatedStringExpressionSyntax WithStringEndToken(SyntaxToken stringEndToken) => Update(this.StringStartToken, this.Contents, stringEndToken);

        public InterpolatedStringExpressionSyntax AddContents(params InterpolatedStringContentSyntax[] items) => WithContents(this.Contents.AddRange(items));
    }

    /// <summary>Class which represents a simple pattern-matching expression using the "is" keyword.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.IsPatternExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class IsPatternExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;
        private PatternSyntax? pattern;

        internal IsPatternExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>ExpressionSyntax node representing the expression on the left of the "is" operator.</summary>
        public ExpressionSyntax Expression => GetRedAtZero(ref this.expression)!;

        public SyntaxToken IsKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.IsPatternExpressionSyntax)this.Green).isKeyword, GetChildPosition(1), GetChildIndex(1));

        /// <summary>PatternSyntax node representing the pattern on the right of the "is" operator.</summary>
        public PatternSyntax Pattern => GetRed(ref this.pattern, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.expression)!,
                2 => GetRed(ref this.pattern, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.expression,
                2 => this.pattern,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitIsPatternExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitIsPatternExpression(this);

        public IsPatternExpressionSyntax Update(ExpressionSyntax expression, SyntaxToken isKeyword, PatternSyntax pattern)
        {
            if (expression != this.Expression || isKeyword != this.IsKeyword || pattern != this.Pattern)
            {
                var newNode = SyntaxFactory.IsPatternExpression(expression, isKeyword, pattern);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public IsPatternExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(expression, this.IsKeyword, this.Pattern);
        public IsPatternExpressionSyntax WithIsKeyword(SyntaxToken isKeyword) => Update(this.Expression, isKeyword, this.Pattern);
        public IsPatternExpressionSyntax WithPattern(PatternSyntax pattern) => Update(this.Expression, this.IsKeyword, pattern);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ThrowExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ThrowExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? expression;

        internal ThrowExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken ThrowKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ThrowExpressionSyntax)this.Green).throwKeyword, Position, 0);

        public ExpressionSyntax Expression => GetRed(ref this.expression, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.expression, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.expression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitThrowExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitThrowExpression(this);

        public ThrowExpressionSyntax Update(SyntaxToken throwKeyword, ExpressionSyntax expression)
        {
            if (throwKeyword != this.ThrowKeyword || expression != this.Expression)
            {
                var newNode = SyntaxFactory.ThrowExpression(throwKeyword, expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ThrowExpressionSyntax WithThrowKeyword(SyntaxToken throwKeyword) => Update(throwKeyword, this.Expression);
        public ThrowExpressionSyntax WithExpression(ExpressionSyntax expression) => Update(this.ThrowKeyword, expression);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.WhenClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class WhenClauseSyntax : CSharpSyntaxNode
    {
        private ExpressionSyntax? condition;

        internal WhenClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken WhenKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.WhenClauseSyntax)this.Green).whenKeyword, Position, 0);

        public ExpressionSyntax Condition => GetRed(ref this.condition, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.condition, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.condition : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitWhenClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitWhenClause(this);

        public WhenClauseSyntax Update(SyntaxToken whenKeyword, ExpressionSyntax condition)
        {
            if (whenKeyword != this.WhenKeyword || condition != this.Condition)
            {
                var newNode = SyntaxFactory.WhenClause(whenKeyword, condition);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public WhenClauseSyntax WithWhenKeyword(SyntaxToken whenKeyword) => Update(whenKeyword, this.Condition);
        public WhenClauseSyntax WithCondition(ExpressionSyntax condition) => Update(this.WhenKeyword, condition);
    }

    public abstract partial class PatternSyntax : ExpressionOrPatternSyntax
    {
        internal PatternSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.DiscardPattern"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class DiscardPatternSyntax : PatternSyntax
    {

        internal DiscardPatternSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken UnderscoreToken => new SyntaxToken(this, ((Syntax.InternalSyntax.DiscardPatternSyntax)this.Green).underscoreToken, Position, 0);

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitDiscardPattern(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDiscardPattern(this);

        public DiscardPatternSyntax Update(SyntaxToken underscoreToken)
        {
            if (underscoreToken != this.UnderscoreToken)
            {
                var newNode = SyntaxFactory.DiscardPattern(underscoreToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public DiscardPatternSyntax WithUnderscoreToken(SyntaxToken underscoreToken) => Update(underscoreToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.DeclarationPattern"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class DeclarationPatternSyntax : PatternSyntax
    {
        private TypeSyntax? type;
        private VariableDesignationSyntax? designation;

        internal DeclarationPatternSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public TypeSyntax Type => GetRedAtZero(ref this.type)!;

        public VariableDesignationSyntax Designation => GetRed(ref this.designation, 1)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.type)!,
                1 => GetRed(ref this.designation, 1)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.type,
                1 => this.designation,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitDeclarationPattern(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDeclarationPattern(this);

        public DeclarationPatternSyntax Update(TypeSyntax type, VariableDesignationSyntax designation)
        {
            if (type != this.Type || designation != this.Designation)
            {
                var newNode = SyntaxFactory.DeclarationPattern(type, designation);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public DeclarationPatternSyntax WithType(TypeSyntax type) => Update(type, this.Designation);
        public DeclarationPatternSyntax WithDesignation(VariableDesignationSyntax designation) => Update(this.Type, designation);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.VarPattern"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class VarPatternSyntax : PatternSyntax
    {
        private VariableDesignationSyntax? designation;

        internal VarPatternSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken VarKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.VarPatternSyntax)this.Green).varKeyword, Position, 0);

        public VariableDesignationSyntax Designation => GetRed(ref this.designation, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.designation, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.designation : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitVarPattern(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitVarPattern(this);

        public VarPatternSyntax Update(SyntaxToken varKeyword, VariableDesignationSyntax designation)
        {
            if (varKeyword != this.VarKeyword || designation != this.Designation)
            {
                var newNode = SyntaxFactory.VarPattern(varKeyword, designation);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public VarPatternSyntax WithVarKeyword(SyntaxToken varKeyword) => Update(varKeyword, this.Designation);
        public VarPatternSyntax WithDesignation(VariableDesignationSyntax designation) => Update(this.VarKeyword, designation);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.RecursivePattern"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class RecursivePatternSyntax : PatternSyntax
    {
        private TypeSyntax? type;
        private PositionalPatternClauseSyntax? positionalPatternClause;
        private PropertyPatternClauseSyntax? propertyPatternClause;
        private VariableDesignationSyntax? designation;

        internal RecursivePatternSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public TypeSyntax? Type => GetRedAtZero(ref this.type);

        public PositionalPatternClauseSyntax? PositionalPatternClause => GetRed(ref this.positionalPatternClause, 1);

        public PropertyPatternClauseSyntax? PropertyPatternClause => GetRed(ref this.propertyPatternClause, 2);

        public VariableDesignationSyntax? Designation => GetRed(ref this.designation, 3);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.type),
                1 => GetRed(ref this.positionalPatternClause, 1),
                2 => GetRed(ref this.propertyPatternClause, 2),
                3 => GetRed(ref this.designation, 3),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.type,
                1 => this.positionalPatternClause,
                2 => this.propertyPatternClause,
                3 => this.designation,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitRecursivePattern(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRecursivePattern(this);

        public RecursivePatternSyntax Update(TypeSyntax? type, PositionalPatternClauseSyntax? positionalPatternClause, PropertyPatternClauseSyntax? propertyPatternClause, VariableDesignationSyntax? designation)
        {
            if (type != this.Type || positionalPatternClause != this.PositionalPatternClause || propertyPatternClause != this.PropertyPatternClause || designation != this.Designation)
            {
                var newNode = SyntaxFactory.RecursivePattern(type, positionalPatternClause, propertyPatternClause, designation);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public RecursivePatternSyntax WithType(TypeSyntax? type) => Update(type, this.PositionalPatternClause, this.PropertyPatternClause, this.Designation);
        public RecursivePatternSyntax WithPositionalPatternClause(PositionalPatternClauseSyntax? positionalPatternClause) => Update(this.Type, positionalPatternClause, this.PropertyPatternClause, this.Designation);
        public RecursivePatternSyntax WithPropertyPatternClause(PropertyPatternClauseSyntax? propertyPatternClause) => Update(this.Type, this.PositionalPatternClause, propertyPatternClause, this.Designation);
        public RecursivePatternSyntax WithDesignation(VariableDesignationSyntax? designation) => Update(this.Type, this.PositionalPatternClause, this.PropertyPatternClause, designation);

        public RecursivePatternSyntax AddPositionalPatternClauseSubpatterns(params SubpatternSyntax[] items)
        {
            var positionalPatternClause = this.PositionalPatternClause ?? SyntaxFactory.PositionalPatternClause();
            return WithPositionalPatternClause(positionalPatternClause.WithSubpatterns(positionalPatternClause.Subpatterns.AddRange(items)));
        }
        public RecursivePatternSyntax AddPropertyPatternClauseSubpatterns(params SubpatternSyntax[] items)
        {
            var propertyPatternClause = this.PropertyPatternClause ?? SyntaxFactory.PropertyPatternClause();
            return WithPropertyPatternClause(propertyPatternClause.WithSubpatterns(propertyPatternClause.Subpatterns.AddRange(items)));
        }
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.PositionalPatternClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class PositionalPatternClauseSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? subpatterns;

        internal PositionalPatternClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.PositionalPatternClauseSyntax)this.Green).openParenToken, Position, 0);

        public SeparatedSyntaxList<SubpatternSyntax> Subpatterns
        {
            get
            {
                var red = GetRed(ref this.subpatterns, 1);
                return red != null ? new SeparatedSyntaxList<SubpatternSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.PositionalPatternClauseSyntax)this.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.subpatterns, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.subpatterns : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitPositionalPatternClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPositionalPatternClause(this);

        public PositionalPatternClauseSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<SubpatternSyntax> subpatterns, SyntaxToken closeParenToken)
        {
            if (openParenToken != this.OpenParenToken || subpatterns != this.Subpatterns || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.PositionalPatternClause(openParenToken, subpatterns, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public PositionalPatternClauseSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(openParenToken, this.Subpatterns, this.CloseParenToken);
        public PositionalPatternClauseSyntax WithSubpatterns(SeparatedSyntaxList<SubpatternSyntax> subpatterns) => Update(this.OpenParenToken, subpatterns, this.CloseParenToken);
        public PositionalPatternClauseSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.OpenParenToken, this.Subpatterns, closeParenToken);

        public PositionalPatternClauseSyntax AddSubpatterns(params SubpatternSyntax[] items) => WithSubpatterns(this.Subpatterns.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.PropertyPatternClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class PropertyPatternClauseSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? subpatterns;

        internal PropertyPatternClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.PropertyPatternClauseSyntax)this.Green).openBraceToken, Position, 0);

        public SeparatedSyntaxList<SubpatternSyntax> Subpatterns
        {
            get
            {
                var red = GetRed(ref this.subpatterns, 1);
                return red != null ? new SeparatedSyntaxList<SubpatternSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.PropertyPatternClauseSyntax)this.Green).closeBraceToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.subpatterns, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.subpatterns : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitPropertyPatternClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPropertyPatternClause(this);

        public PropertyPatternClauseSyntax Update(SyntaxToken openBraceToken, SeparatedSyntaxList<SubpatternSyntax> subpatterns, SyntaxToken closeBraceToken)
        {
            if (openBraceToken != this.OpenBraceToken || subpatterns != this.Subpatterns || closeBraceToken != this.CloseBraceToken)
            {
                var newNode = SyntaxFactory.PropertyPatternClause(openBraceToken, subpatterns, closeBraceToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public PropertyPatternClauseSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(openBraceToken, this.Subpatterns, this.CloseBraceToken);
        public PropertyPatternClauseSyntax WithSubpatterns(SeparatedSyntaxList<SubpatternSyntax> subpatterns) => Update(this.OpenBraceToken, subpatterns, this.CloseBraceToken);
        public PropertyPatternClauseSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.OpenBraceToken, this.Subpatterns, closeBraceToken);

        public PropertyPatternClauseSyntax AddSubpatterns(params SubpatternSyntax[] items) => WithSubpatterns(this.Subpatterns.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.Subpattern"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class SubpatternSyntax : CSharpSyntaxNode
    {
        private NameColonSyntax? nameColon;
        private PatternSyntax? pattern;

        internal SubpatternSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public NameColonSyntax? NameColon => GetRedAtZero(ref this.nameColon);

        public PatternSyntax Pattern => GetRed(ref this.pattern, 1)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.nameColon),
                1 => GetRed(ref this.pattern, 1)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.nameColon,
                1 => this.pattern,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitSubpattern(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSubpattern(this);

        public SubpatternSyntax Update(NameColonSyntax? nameColon, PatternSyntax pattern)
        {
            if (nameColon != this.NameColon || pattern != this.Pattern)
            {
                var newNode = SyntaxFactory.Subpattern(nameColon, pattern);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public SubpatternSyntax WithNameColon(NameColonSyntax? nameColon) => Update(nameColon, this.Pattern);
        public SubpatternSyntax WithPattern(PatternSyntax pattern) => Update(this.NameColon, pattern);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ConstantPattern"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ConstantPatternSyntax : PatternSyntax
    {
        private ExpressionSyntax? expression;

        internal ConstantPatternSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>ExpressionSyntax node representing the constant expression.</summary>
        public ExpressionSyntax Expression => GetRedAtZero(ref this.expression)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.expression)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.expression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitConstantPattern(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitConstantPattern(this);

        public ConstantPatternSyntax Update(ExpressionSyntax expression)
        {
            if (expression != this.Expression)
            {
                var newNode = SyntaxFactory.ConstantPattern(expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ConstantPatternSyntax WithExpression(ExpressionSyntax expression) => Update(expression);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ParenthesizedPattern"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ParenthesizedPatternSyntax : PatternSyntax
    {
        private PatternSyntax? pattern;

        internal ParenthesizedPatternSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ParenthesizedPatternSyntax)this.Green).openParenToken, Position, 0);

        public PatternSyntax Pattern => GetRed(ref this.pattern, 1)!;

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ParenthesizedPatternSyntax)this.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.pattern, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.pattern : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitParenthesizedPattern(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitParenthesizedPattern(this);

        public ParenthesizedPatternSyntax Update(SyntaxToken openParenToken, PatternSyntax pattern, SyntaxToken closeParenToken)
        {
            if (openParenToken != this.OpenParenToken || pattern != this.Pattern || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.ParenthesizedPattern(openParenToken, pattern, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ParenthesizedPatternSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(openParenToken, this.Pattern, this.CloseParenToken);
        public ParenthesizedPatternSyntax WithPattern(PatternSyntax pattern) => Update(this.OpenParenToken, pattern, this.CloseParenToken);
        public ParenthesizedPatternSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.OpenParenToken, this.Pattern, closeParenToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.RelationalPattern"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class RelationalPatternSyntax : PatternSyntax
    {
        private ExpressionSyntax? expression;

        internal RelationalPatternSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the operator of the relational pattern.</summary>
        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Syntax.InternalSyntax.RelationalPatternSyntax)this.Green).operatorToken, Position, 0);

        public ExpressionSyntax Expression => GetRed(ref this.expression, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.expression, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.expression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitRelationalPattern(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRelationalPattern(this);

        public RelationalPatternSyntax Update(SyntaxToken operatorToken, ExpressionSyntax expression)
        {
            if (operatorToken != this.OperatorToken || expression != this.Expression)
            {
                var newNode = SyntaxFactory.RelationalPattern(operatorToken, expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public RelationalPatternSyntax WithOperatorToken(SyntaxToken operatorToken) => Update(operatorToken, this.Expression);
        public RelationalPatternSyntax WithExpression(ExpressionSyntax expression) => Update(this.OperatorToken, expression);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.TypePattern"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class TypePatternSyntax : PatternSyntax
    {
        private TypeSyntax? type;

        internal TypePatternSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>The type for the type pattern.</summary>
        public TypeSyntax Type => GetRedAtZero(ref this.type)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.type)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.type : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitTypePattern(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypePattern(this);

        public TypePatternSyntax Update(TypeSyntax type)
        {
            if (type != this.Type)
            {
                var newNode = SyntaxFactory.TypePattern(type);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public TypePatternSyntax WithType(TypeSyntax type) => Update(type);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.OrPattern"/></description></item>
    /// <item><description><see cref="SyntaxKind.AndPattern"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class BinaryPatternSyntax : PatternSyntax
    {
        private PatternSyntax? left;
        private PatternSyntax? right;

        internal BinaryPatternSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public PatternSyntax Left => GetRedAtZero(ref this.left)!;

        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Syntax.InternalSyntax.BinaryPatternSyntax)this.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        public PatternSyntax Right => GetRed(ref this.right, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.left)!,
                2 => GetRed(ref this.right, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.left,
                2 => this.right,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitBinaryPattern(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBinaryPattern(this);

        public BinaryPatternSyntax Update(PatternSyntax left, SyntaxToken operatorToken, PatternSyntax right)
        {
            if (left != this.Left || operatorToken != this.OperatorToken || right != this.Right)
            {
                var newNode = SyntaxFactory.BinaryPattern(this.Kind(), left, operatorToken, right);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public BinaryPatternSyntax WithLeft(PatternSyntax left) => Update(left, this.OperatorToken, this.Right);
        public BinaryPatternSyntax WithOperatorToken(SyntaxToken operatorToken) => Update(this.Left, operatorToken, this.Right);
        public BinaryPatternSyntax WithRight(PatternSyntax right) => Update(this.Left, this.OperatorToken, right);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.NotPattern"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class UnaryPatternSyntax : PatternSyntax
    {
        private PatternSyntax? pattern;

        internal UnaryPatternSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Syntax.InternalSyntax.UnaryPatternSyntax)this.Green).operatorToken, Position, 0);

        public PatternSyntax Pattern => GetRed(ref this.pattern, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.pattern, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.pattern : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitUnaryPattern(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitUnaryPattern(this);

        public UnaryPatternSyntax Update(SyntaxToken operatorToken, PatternSyntax pattern)
        {
            if (operatorToken != this.OperatorToken || pattern != this.Pattern)
            {
                var newNode = SyntaxFactory.UnaryPattern(operatorToken, pattern);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public UnaryPatternSyntax WithOperatorToken(SyntaxToken operatorToken) => Update(operatorToken, this.Pattern);
        public UnaryPatternSyntax WithPattern(PatternSyntax pattern) => Update(this.OperatorToken, pattern);
    }

    public abstract partial class InterpolatedStringContentSyntax : CSharpSyntaxNode
    {
        internal InterpolatedStringContentSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.InterpolatedStringText"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class InterpolatedStringTextSyntax : InterpolatedStringContentSyntax
    {

        internal InterpolatedStringTextSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>The text contents of a part of the interpolated string.</summary>
        public SyntaxToken TextToken => new SyntaxToken(this, ((Syntax.InternalSyntax.InterpolatedStringTextSyntax)this.Green).textToken, Position, 0);

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitInterpolatedStringText(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitInterpolatedStringText(this);

        public InterpolatedStringTextSyntax Update(SyntaxToken textToken)
        {
            if (textToken != this.TextToken)
            {
                var newNode = SyntaxFactory.InterpolatedStringText(textToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public InterpolatedStringTextSyntax WithTextToken(SyntaxToken textToken) => Update(textToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.Interpolation"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class InterpolationSyntax : InterpolatedStringContentSyntax
    {
        private ExpressionSyntax? expression;
        private InterpolationAlignmentClauseSyntax? alignmentClause;
        private InterpolationFormatClauseSyntax? formatClause;

        internal InterpolationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.InterpolationSyntax)this.Green).openBraceToken, Position, 0);

        public ExpressionSyntax Expression => GetRed(ref this.expression, 1)!;

        public InterpolationAlignmentClauseSyntax? AlignmentClause => GetRed(ref this.alignmentClause, 2);

        public InterpolationFormatClauseSyntax? FormatClause => GetRed(ref this.formatClause, 3);

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.InterpolationSyntax)this.Green).closeBraceToken, GetChildPosition(4), GetChildIndex(4));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.expression, 1)!,
                2 => GetRed(ref this.alignmentClause, 2),
                3 => GetRed(ref this.formatClause, 3),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.expression,
                2 => this.alignmentClause,
                3 => this.formatClause,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitInterpolation(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitInterpolation(this);

        public InterpolationSyntax Update(SyntaxToken openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax? alignmentClause, InterpolationFormatClauseSyntax? formatClause, SyntaxToken closeBraceToken)
        {
            if (openBraceToken != this.OpenBraceToken || expression != this.Expression || alignmentClause != this.AlignmentClause || formatClause != this.FormatClause || closeBraceToken != this.CloseBraceToken)
            {
                var newNode = SyntaxFactory.Interpolation(openBraceToken, expression, alignmentClause, formatClause, closeBraceToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public InterpolationSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(openBraceToken, this.Expression, this.AlignmentClause, this.FormatClause, this.CloseBraceToken);
        public InterpolationSyntax WithExpression(ExpressionSyntax expression) => Update(this.OpenBraceToken, expression, this.AlignmentClause, this.FormatClause, this.CloseBraceToken);
        public InterpolationSyntax WithAlignmentClause(InterpolationAlignmentClauseSyntax? alignmentClause) => Update(this.OpenBraceToken, this.Expression, alignmentClause, this.FormatClause, this.CloseBraceToken);
        public InterpolationSyntax WithFormatClause(InterpolationFormatClauseSyntax? formatClause) => Update(this.OpenBraceToken, this.Expression, this.AlignmentClause, formatClause, this.CloseBraceToken);
        public InterpolationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.OpenBraceToken, this.Expression, this.AlignmentClause, this.FormatClause, closeBraceToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.InterpolationAlignmentClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class InterpolationAlignmentClauseSyntax : CSharpSyntaxNode
    {
        private ExpressionSyntax? value;

        internal InterpolationAlignmentClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken CommaToken => new SyntaxToken(this, ((Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax)this.Green).commaToken, Position, 0);

        public ExpressionSyntax Value => GetRed(ref this.value, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.value, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.value : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitInterpolationAlignmentClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitInterpolationAlignmentClause(this);

        public InterpolationAlignmentClauseSyntax Update(SyntaxToken commaToken, ExpressionSyntax value)
        {
            if (commaToken != this.CommaToken || value != this.Value)
            {
                var newNode = SyntaxFactory.InterpolationAlignmentClause(commaToken, value);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public InterpolationAlignmentClauseSyntax WithCommaToken(SyntaxToken commaToken) => Update(commaToken, this.Value);
        public InterpolationAlignmentClauseSyntax WithValue(ExpressionSyntax value) => Update(this.CommaToken, value);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.InterpolationFormatClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class InterpolationFormatClauseSyntax : CSharpSyntaxNode
    {

        internal InterpolationFormatClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken ColonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.InterpolationFormatClauseSyntax)this.Green).colonToken, Position, 0);

        /// <summary>The text contents of the format specifier for an interpolation.</summary>
        public SyntaxToken FormatStringToken => new SyntaxToken(this, ((Syntax.InternalSyntax.InterpolationFormatClauseSyntax)this.Green).formatStringToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitInterpolationFormatClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitInterpolationFormatClause(this);

        public InterpolationFormatClauseSyntax Update(SyntaxToken colonToken, SyntaxToken formatStringToken)
        {
            if (colonToken != this.ColonToken || formatStringToken != this.FormatStringToken)
            {
                var newNode = SyntaxFactory.InterpolationFormatClause(colonToken, formatStringToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public InterpolationFormatClauseSyntax WithColonToken(SyntaxToken colonToken) => Update(colonToken, this.FormatStringToken);
        public InterpolationFormatClauseSyntax WithFormatStringToken(SyntaxToken formatStringToken) => Update(this.ColonToken, formatStringToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.GlobalStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class GlobalStatementSyntax : MemberDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private StatementSyntax? statement;

        internal GlobalStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public StatementSyntax Statement => GetRed(ref this.statement, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.statement, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.statement,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitGlobalStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitGlobalStatement(this);

        public GlobalStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, StatementSyntax statement)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || statement != this.Statement)
            {
                var newNode = SyntaxFactory.GlobalStatement(attributeLists, modifiers, statement);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new GlobalStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Statement);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new GlobalStatementSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Statement);
        public GlobalStatementSyntax WithStatement(StatementSyntax statement) => Update(this.AttributeLists, this.Modifiers, statement);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new GlobalStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new GlobalStatementSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
    }

    /// <summary>Represents the base class for all statements syntax classes.</summary>
    public abstract partial class StatementSyntax : CSharpSyntaxNode
    {
        internal StatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public abstract SyntaxList<AttributeListSyntax> AttributeLists { get; }
        public StatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeListsCore(attributeLists);
        internal abstract StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists);

        public StatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => AddAttributeListsCore(items);
        internal abstract StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.Block"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class BlockSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private SyntaxNode? statements;

        internal BlockSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.BlockSyntax)this.Green).openBraceToken, GetChildPosition(1), GetChildIndex(1));

        public SyntaxList<StatementSyntax> Statements => new SyntaxList<StatementSyntax>(GetRed(ref this.statements, 2));

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.BlockSyntax)this.Green).closeBraceToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.statements, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.statements,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitBlock(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBlock(this);

        public BlockSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken openBraceToken, SyntaxList<StatementSyntax> statements, SyntaxToken closeBraceToken)
        {
            if (attributeLists != this.AttributeLists || openBraceToken != this.OpenBraceToken || statements != this.Statements || closeBraceToken != this.CloseBraceToken)
            {
                var newNode = SyntaxFactory.Block(attributeLists, openBraceToken, statements, closeBraceToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new BlockSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.OpenBraceToken, this.Statements, this.CloseBraceToken);
        public BlockSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(this.AttributeLists, openBraceToken, this.Statements, this.CloseBraceToken);
        public BlockSyntax WithStatements(SyntaxList<StatementSyntax> statements) => Update(this.AttributeLists, this.OpenBraceToken, statements, this.CloseBraceToken);
        public BlockSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.AttributeLists, this.OpenBraceToken, this.Statements, closeBraceToken);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new BlockSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        public BlockSyntax AddStatements(params StatementSyntax[] items) => WithStatements(this.Statements.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.LocalFunctionStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class LocalFunctionStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeSyntax? returnType;
        private TypeParameterListSyntax? typeParameterList;
        private ParameterListSyntax? parameterList;
        private SyntaxNode? constraintClauses;
        private BlockSyntax? body;
        private ArrowExpressionClauseSyntax? expressionBody;

        internal LocalFunctionStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public TypeSyntax ReturnType => GetRed(ref this.returnType, 2)!;

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.LocalFunctionStatementSyntax)this.Green).identifier, GetChildPosition(3), GetChildIndex(3));

        public TypeParameterListSyntax? TypeParameterList => GetRed(ref this.typeParameterList, 4);

        public ParameterListSyntax ParameterList => GetRed(ref this.parameterList, 5)!;

        public SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new SyntaxList<TypeParameterConstraintClauseSyntax>(GetRed(ref this.constraintClauses, 6));

        public BlockSyntax? Body => GetRed(ref this.body, 7);

        public ArrowExpressionClauseSyntax? ExpressionBody => GetRed(ref this.expressionBody, 8);

        /// <summary>Gets the optional semicolon token.</summary>
        public SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.LocalFunctionStatementSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(9), GetChildIndex(9)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.returnType, 2)!,
                4 => GetRed(ref this.typeParameterList, 4),
                5 => GetRed(ref this.parameterList, 5)!,
                6 => GetRed(ref this.constraintClauses, 6)!,
                7 => GetRed(ref this.body, 7),
                8 => GetRed(ref this.expressionBody, 8),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.returnType,
                4 => this.typeParameterList,
                5 => this.parameterList,
                6 => this.constraintClauses,
                7 => this.body,
                8 => this.expressionBody,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitLocalFunctionStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLocalFunctionStatement(this);

        public LocalFunctionStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax returnType, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || returnType != this.ReturnType || identifier != this.Identifier || typeParameterList != this.TypeParameterList || parameterList != this.ParameterList || constraintClauses != this.ConstraintClauses || body != this.Body || expressionBody != this.ExpressionBody || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.LocalFunctionStatement(attributeLists, modifiers, returnType, identifier, typeParameterList, parameterList, constraintClauses, body, expressionBody, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new LocalFunctionStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.ReturnType, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        public LocalFunctionStatementSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.ReturnType, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        public LocalFunctionStatementSyntax WithReturnType(TypeSyntax returnType) => Update(this.AttributeLists, this.Modifiers, returnType, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        public LocalFunctionStatementSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        public LocalFunctionStatementSyntax WithTypeParameterList(TypeParameterListSyntax? typeParameterList) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.Identifier, typeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        public LocalFunctionStatementSyntax WithParameterList(ParameterListSyntax parameterList) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.Identifier, this.TypeParameterList, parameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        public LocalFunctionStatementSyntax WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.Identifier, this.TypeParameterList, this.ParameterList, constraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        public LocalFunctionStatementSyntax WithBody(BlockSyntax? body) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, body, this.ExpressionBody, this.SemicolonToken);
        public LocalFunctionStatementSyntax WithExpressionBody(ArrowExpressionClauseSyntax? expressionBody) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, expressionBody, this.SemicolonToken);
        public LocalFunctionStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, semicolonToken);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new LocalFunctionStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        public LocalFunctionStatementSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        public LocalFunctionStatementSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
        {
            var typeParameterList = this.TypeParameterList ?? SyntaxFactory.TypeParameterList();
            return WithTypeParameterList(typeParameterList.WithParameters(typeParameterList.Parameters.AddRange(items)));
        }
        public LocalFunctionStatementSyntax AddParameterListParameters(params ParameterSyntax[] items) => WithParameterList(this.ParameterList.WithParameters(this.ParameterList.Parameters.AddRange(items)));
        public LocalFunctionStatementSyntax AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items) => WithConstraintClauses(this.ConstraintClauses.AddRange(items));
        public LocalFunctionStatementSyntax AddBodyAttributeLists(params AttributeListSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithAttributeLists(body.AttributeLists.AddRange(items)));
        }
        public LocalFunctionStatementSyntax AddBodyStatements(params StatementSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithStatements(body.Statements.AddRange(items)));
        }
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.LocalDeclarationStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class LocalDeclarationStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private VariableDeclarationSyntax? declaration;

        internal LocalDeclarationStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken AwaitKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.LocalDeclarationStatementSyntax)this.Green).awaitKeyword;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public SyntaxToken UsingKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.LocalDeclarationStatementSyntax)this.Green).usingKeyword;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(2), GetChildIndex(2)) : default;
            }
        }

        /// <summary>Gets the modifier list.</summary>
        public SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(3);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(3), GetChildIndex(3)) : default;
            }
        }

        public VariableDeclarationSyntax Declaration => GetRed(ref this.declaration, 4)!;

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.LocalDeclarationStatementSyntax)this.Green).semicolonToken, GetChildPosition(5), GetChildIndex(5));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                4 => GetRed(ref this.declaration, 4)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                4 => this.declaration,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitLocalDeclarationStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLocalDeclarationStatement(this);

        public LocalDeclarationStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken usingKeyword, SyntaxTokenList modifiers, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || awaitKeyword != this.AwaitKeyword || usingKeyword != this.UsingKeyword || modifiers != this.Modifiers || declaration != this.Declaration || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.LocalDeclarationStatement(attributeLists, awaitKeyword, usingKeyword, modifiers, declaration, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new LocalDeclarationStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.AwaitKeyword, this.UsingKeyword, this.Modifiers, this.Declaration, this.SemicolonToken);
        public LocalDeclarationStatementSyntax WithAwaitKeyword(SyntaxToken awaitKeyword) => Update(this.AttributeLists, awaitKeyword, this.UsingKeyword, this.Modifiers, this.Declaration, this.SemicolonToken);
        public LocalDeclarationStatementSyntax WithUsingKeyword(SyntaxToken usingKeyword) => Update(this.AttributeLists, this.AwaitKeyword, usingKeyword, this.Modifiers, this.Declaration, this.SemicolonToken);
        public LocalDeclarationStatementSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, this.AwaitKeyword, this.UsingKeyword, modifiers, this.Declaration, this.SemicolonToken);
        public LocalDeclarationStatementSyntax WithDeclaration(VariableDeclarationSyntax declaration) => Update(this.AttributeLists, this.AwaitKeyword, this.UsingKeyword, this.Modifiers, declaration, this.SemicolonToken);
        public LocalDeclarationStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.AwaitKeyword, this.UsingKeyword, this.Modifiers, this.Declaration, semicolonToken);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new LocalDeclarationStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        public LocalDeclarationStatementSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        public LocalDeclarationStatementSyntax AddDeclarationVariables(params VariableDeclaratorSyntax[] items) => WithDeclaration(this.Declaration.WithVariables(this.Declaration.Variables.AddRange(items)));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.VariableDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class VariableDeclarationSyntax : CSharpSyntaxNode
    {
        private TypeSyntax? type;
        private SyntaxNode? variables;

        internal VariableDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public TypeSyntax Type => GetRedAtZero(ref this.type)!;

        public SeparatedSyntaxList<VariableDeclaratorSyntax> Variables
        {
            get
            {
                var red = GetRed(ref this.variables, 1);
                return red != null ? new SeparatedSyntaxList<VariableDeclaratorSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.type)!,
                1 => GetRed(ref this.variables, 1)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.type,
                1 => this.variables,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitVariableDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitVariableDeclaration(this);

        public VariableDeclarationSyntax Update(TypeSyntax type, SeparatedSyntaxList<VariableDeclaratorSyntax> variables)
        {
            if (type != this.Type || variables != this.Variables)
            {
                var newNode = SyntaxFactory.VariableDeclaration(type, variables);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public VariableDeclarationSyntax WithType(TypeSyntax type) => Update(type, this.Variables);
        public VariableDeclarationSyntax WithVariables(SeparatedSyntaxList<VariableDeclaratorSyntax> variables) => Update(this.Type, variables);

        public VariableDeclarationSyntax AddVariables(params VariableDeclaratorSyntax[] items) => WithVariables(this.Variables.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.VariableDeclarator"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class VariableDeclaratorSyntax : CSharpSyntaxNode
    {
        private BracketedArgumentListSyntax? argumentList;
        private EqualsValueClauseSyntax? initializer;

        internal VariableDeclaratorSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.VariableDeclaratorSyntax)this.Green).identifier, Position, 0);

        public BracketedArgumentListSyntax? ArgumentList => GetRed(ref this.argumentList, 1);

        public EqualsValueClauseSyntax? Initializer => GetRed(ref this.initializer, 2);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.argumentList, 1),
                2 => GetRed(ref this.initializer, 2),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.argumentList,
                2 => this.initializer,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitVariableDeclarator(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitVariableDeclarator(this);

        public VariableDeclaratorSyntax Update(SyntaxToken identifier, BracketedArgumentListSyntax? argumentList, EqualsValueClauseSyntax? initializer)
        {
            if (identifier != this.Identifier || argumentList != this.ArgumentList || initializer != this.Initializer)
            {
                var newNode = SyntaxFactory.VariableDeclarator(identifier, argumentList, initializer);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public VariableDeclaratorSyntax WithIdentifier(SyntaxToken identifier) => Update(identifier, this.ArgumentList, this.Initializer);
        public VariableDeclaratorSyntax WithArgumentList(BracketedArgumentListSyntax? argumentList) => Update(this.Identifier, argumentList, this.Initializer);
        public VariableDeclaratorSyntax WithInitializer(EqualsValueClauseSyntax? initializer) => Update(this.Identifier, this.ArgumentList, initializer);

        public VariableDeclaratorSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
        {
            var argumentList = this.ArgumentList ?? SyntaxFactory.BracketedArgumentList();
            return WithArgumentList(argumentList.WithArguments(argumentList.Arguments.AddRange(items)));
        }
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.EqualsValueClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class EqualsValueClauseSyntax : CSharpSyntaxNode
    {
        private ExpressionSyntax? value;

        internal EqualsValueClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken EqualsToken => new SyntaxToken(this, ((Syntax.InternalSyntax.EqualsValueClauseSyntax)this.Green).equalsToken, Position, 0);

        public ExpressionSyntax Value => GetRed(ref this.value, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.value, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.value : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitEqualsValueClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEqualsValueClause(this);

        public EqualsValueClauseSyntax Update(SyntaxToken equalsToken, ExpressionSyntax value)
        {
            if (equalsToken != this.EqualsToken || value != this.Value)
            {
                var newNode = SyntaxFactory.EqualsValueClause(equalsToken, value);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public EqualsValueClauseSyntax WithEqualsToken(SyntaxToken equalsToken) => Update(equalsToken, this.Value);
        public EqualsValueClauseSyntax WithValue(ExpressionSyntax value) => Update(this.EqualsToken, value);
    }

    public abstract partial class VariableDesignationSyntax : CSharpSyntaxNode
    {
        internal VariableDesignationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.SingleVariableDesignation"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class SingleVariableDesignationSyntax : VariableDesignationSyntax
    {

        internal SingleVariableDesignationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.SingleVariableDesignationSyntax)this.Green).identifier, Position, 0);

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitSingleVariableDesignation(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSingleVariableDesignation(this);

        public SingleVariableDesignationSyntax Update(SyntaxToken identifier)
        {
            if (identifier != this.Identifier)
            {
                var newNode = SyntaxFactory.SingleVariableDesignation(identifier);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public SingleVariableDesignationSyntax WithIdentifier(SyntaxToken identifier) => Update(identifier);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.DiscardDesignation"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class DiscardDesignationSyntax : VariableDesignationSyntax
    {

        internal DiscardDesignationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken UnderscoreToken => new SyntaxToken(this, ((Syntax.InternalSyntax.DiscardDesignationSyntax)this.Green).underscoreToken, Position, 0);

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitDiscardDesignation(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDiscardDesignation(this);

        public DiscardDesignationSyntax Update(SyntaxToken underscoreToken)
        {
            if (underscoreToken != this.UnderscoreToken)
            {
                var newNode = SyntaxFactory.DiscardDesignation(underscoreToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public DiscardDesignationSyntax WithUnderscoreToken(SyntaxToken underscoreToken) => Update(underscoreToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ParenthesizedVariableDesignation"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ParenthesizedVariableDesignationSyntax : VariableDesignationSyntax
    {
        private SyntaxNode? variables;

        internal ParenthesizedVariableDesignationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ParenthesizedVariableDesignationSyntax)this.Green).openParenToken, Position, 0);

        public SeparatedSyntaxList<VariableDesignationSyntax> Variables
        {
            get
            {
                var red = GetRed(ref this.variables, 1);
                return red != null ? new SeparatedSyntaxList<VariableDesignationSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ParenthesizedVariableDesignationSyntax)this.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.variables, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.variables : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitParenthesizedVariableDesignation(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitParenthesizedVariableDesignation(this);

        public ParenthesizedVariableDesignationSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<VariableDesignationSyntax> variables, SyntaxToken closeParenToken)
        {
            if (openParenToken != this.OpenParenToken || variables != this.Variables || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.ParenthesizedVariableDesignation(openParenToken, variables, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ParenthesizedVariableDesignationSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(openParenToken, this.Variables, this.CloseParenToken);
        public ParenthesizedVariableDesignationSyntax WithVariables(SeparatedSyntaxList<VariableDesignationSyntax> variables) => Update(this.OpenParenToken, variables, this.CloseParenToken);
        public ParenthesizedVariableDesignationSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.OpenParenToken, this.Variables, closeParenToken);

        public ParenthesizedVariableDesignationSyntax AddVariables(params VariableDesignationSyntax[] items) => WithVariables(this.Variables.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ExpressionStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ExpressionStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private ExpressionSyntax? expression;

        internal ExpressionStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public ExpressionSyntax Expression => GetRed(ref this.expression, 1)!;

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ExpressionStatementSyntax)this.Green).semicolonToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                1 => GetRed(ref this.expression, 1)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                1 => this.expression,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitExpressionStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitExpressionStatement(this);

        public ExpressionStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || expression != this.Expression || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.ExpressionStatement(attributeLists, expression, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new ExpressionStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Expression, this.SemicolonToken);
        public ExpressionStatementSyntax WithExpression(ExpressionSyntax expression) => Update(this.AttributeLists, expression, this.SemicolonToken);
        public ExpressionStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Expression, semicolonToken);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new ExpressionStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.EmptyStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class EmptyStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        internal EmptyStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.EmptyStatementSyntax)this.Green).semicolonToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.attributeLists)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.attributeLists : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitEmptyStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEmptyStatement(this);

        public EmptyStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.EmptyStatement(attributeLists, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new EmptyStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.SemicolonToken);
        public EmptyStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, semicolonToken);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new EmptyStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <summary>Represents a labeled statement syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.LabeledStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class LabeledStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private StatementSyntax? statement;

        internal LabeledStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.LabeledStatementSyntax)this.Green).identifier, GetChildPosition(1), GetChildIndex(1));

        /// <summary>Gets a SyntaxToken that represents the colon following the statement's label.</summary>
        public SyntaxToken ColonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.LabeledStatementSyntax)this.Green).colonToken, GetChildPosition(2), GetChildIndex(2));

        public StatementSyntax Statement => GetRed(ref this.statement, 3)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.statement, 3)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.statement,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitLabeledStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLabeledStatement(this);

        public LabeledStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken identifier, SyntaxToken colonToken, StatementSyntax statement)
        {
            if (attributeLists != this.AttributeLists || identifier != this.Identifier || colonToken != this.ColonToken || statement != this.Statement)
            {
                var newNode = SyntaxFactory.LabeledStatement(attributeLists, identifier, colonToken, statement);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new LabeledStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Identifier, this.ColonToken, this.Statement);
        public LabeledStatementSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, identifier, this.ColonToken, this.Statement);
        public LabeledStatementSyntax WithColonToken(SyntaxToken colonToken) => Update(this.AttributeLists, this.Identifier, colonToken, this.Statement);
        public LabeledStatementSyntax WithStatement(StatementSyntax statement) => Update(this.AttributeLists, this.Identifier, this.ColonToken, statement);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new LabeledStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <summary>
    /// Represents a goto statement syntax
    /// </summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.GotoStatement"/></description></item>
    /// <item><description><see cref="SyntaxKind.GotoCaseStatement"/></description></item>
    /// <item><description><see cref="SyntaxKind.GotoDefaultStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class GotoStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private ExpressionSyntax? expression;

        internal GotoStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        /// <summary>
        /// Gets a SyntaxToken that represents the goto keyword.
        /// </summary>
        public SyntaxToken GotoKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.GotoStatementSyntax)this.Green).gotoKeyword, GetChildPosition(1), GetChildIndex(1));

        /// <summary>
        /// Gets a SyntaxToken that represents the case or default keywords if any exists.
        /// </summary>
        public SyntaxToken CaseOrDefaultKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.GotoStatementSyntax)this.Green).caseOrDefaultKeyword;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(2), GetChildIndex(2)) : default;
            }
        }

        /// <summary>
        /// Gets a constant expression for a goto case statement.
        /// </summary>
        public ExpressionSyntax? Expression => GetRed(ref this.expression, 3);

        /// <summary>
        /// Gets a SyntaxToken that represents the semi-colon at the end of the statement.
        /// </summary>
        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.GotoStatementSyntax)this.Green).semicolonToken, GetChildPosition(4), GetChildIndex(4));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.expression, 3),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.expression,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitGotoStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitGotoStatement(this);

        public GotoStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken gotoKeyword, SyntaxToken caseOrDefaultKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || gotoKeyword != this.GotoKeyword || caseOrDefaultKeyword != this.CaseOrDefaultKeyword || expression != this.Expression || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.GotoStatement(this.Kind(), attributeLists, gotoKeyword, caseOrDefaultKeyword, expression, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new GotoStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.GotoKeyword, this.CaseOrDefaultKeyword, this.Expression, this.SemicolonToken);
        public GotoStatementSyntax WithGotoKeyword(SyntaxToken gotoKeyword) => Update(this.AttributeLists, gotoKeyword, this.CaseOrDefaultKeyword, this.Expression, this.SemicolonToken);
        public GotoStatementSyntax WithCaseOrDefaultKeyword(SyntaxToken caseOrDefaultKeyword) => Update(this.AttributeLists, this.GotoKeyword, caseOrDefaultKeyword, this.Expression, this.SemicolonToken);
        public GotoStatementSyntax WithExpression(ExpressionSyntax? expression) => Update(this.AttributeLists, this.GotoKeyword, this.CaseOrDefaultKeyword, expression, this.SemicolonToken);
        public GotoStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.GotoKeyword, this.CaseOrDefaultKeyword, this.Expression, semicolonToken);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new GotoStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.BreakStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class BreakStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        internal BreakStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken BreakKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.BreakStatementSyntax)this.Green).breakKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.BreakStatementSyntax)this.Green).semicolonToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.attributeLists)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.attributeLists : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitBreakStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBreakStatement(this);

        public BreakStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken breakKeyword, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || breakKeyword != this.BreakKeyword || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.BreakStatement(attributeLists, breakKeyword, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new BreakStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.BreakKeyword, this.SemicolonToken);
        public BreakStatementSyntax WithBreakKeyword(SyntaxToken breakKeyword) => Update(this.AttributeLists, breakKeyword, this.SemicolonToken);
        public BreakStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.BreakKeyword, semicolonToken);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new BreakStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ContinueStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ContinueStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        internal ContinueStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken ContinueKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ContinueStatementSyntax)this.Green).continueKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ContinueStatementSyntax)this.Green).semicolonToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.attributeLists)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.attributeLists : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitContinueStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitContinueStatement(this);

        public ContinueStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken continueKeyword, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || continueKeyword != this.ContinueKeyword || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.ContinueStatement(attributeLists, continueKeyword, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new ContinueStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.ContinueKeyword, this.SemicolonToken);
        public ContinueStatementSyntax WithContinueKeyword(SyntaxToken continueKeyword) => Update(this.AttributeLists, continueKeyword, this.SemicolonToken);
        public ContinueStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.ContinueKeyword, semicolonToken);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new ContinueStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ReturnStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ReturnStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private ExpressionSyntax? expression;

        internal ReturnStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken ReturnKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ReturnStatementSyntax)this.Green).returnKeyword, GetChildPosition(1), GetChildIndex(1));

        public ExpressionSyntax? Expression => GetRed(ref this.expression, 2);

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ReturnStatementSyntax)this.Green).semicolonToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.expression, 2),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.expression,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitReturnStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitReturnStatement(this);

        public ReturnStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken returnKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || returnKeyword != this.ReturnKeyword || expression != this.Expression || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.ReturnStatement(attributeLists, returnKeyword, expression, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new ReturnStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.ReturnKeyword, this.Expression, this.SemicolonToken);
        public ReturnStatementSyntax WithReturnKeyword(SyntaxToken returnKeyword) => Update(this.AttributeLists, returnKeyword, this.Expression, this.SemicolonToken);
        public ReturnStatementSyntax WithExpression(ExpressionSyntax? expression) => Update(this.AttributeLists, this.ReturnKeyword, expression, this.SemicolonToken);
        public ReturnStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.ReturnKeyword, this.Expression, semicolonToken);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new ReturnStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ThrowStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ThrowStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private ExpressionSyntax? expression;

        internal ThrowStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken ThrowKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ThrowStatementSyntax)this.Green).throwKeyword, GetChildPosition(1), GetChildIndex(1));

        public ExpressionSyntax? Expression => GetRed(ref this.expression, 2);

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ThrowStatementSyntax)this.Green).semicolonToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.expression, 2),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.expression,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitThrowStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitThrowStatement(this);

        public ThrowStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken throwKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || throwKeyword != this.ThrowKeyword || expression != this.Expression || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.ThrowStatement(attributeLists, throwKeyword, expression, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new ThrowStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.ThrowKeyword, this.Expression, this.SemicolonToken);
        public ThrowStatementSyntax WithThrowKeyword(SyntaxToken throwKeyword) => Update(this.AttributeLists, throwKeyword, this.Expression, this.SemicolonToken);
        public ThrowStatementSyntax WithExpression(ExpressionSyntax? expression) => Update(this.AttributeLists, this.ThrowKeyword, expression, this.SemicolonToken);
        public ThrowStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.ThrowKeyword, this.Expression, semicolonToken);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new ThrowStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.YieldReturnStatement"/></description></item>
    /// <item><description><see cref="SyntaxKind.YieldBreakStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class YieldStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private ExpressionSyntax? expression;

        internal YieldStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken YieldKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.YieldStatementSyntax)this.Green).yieldKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken ReturnOrBreakKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.YieldStatementSyntax)this.Green).returnOrBreakKeyword, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax? Expression => GetRed(ref this.expression, 3);

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.YieldStatementSyntax)this.Green).semicolonToken, GetChildPosition(4), GetChildIndex(4));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.expression, 3),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.expression,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitYieldStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitYieldStatement(this);

        public YieldStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || yieldKeyword != this.YieldKeyword || returnOrBreakKeyword != this.ReturnOrBreakKeyword || expression != this.Expression || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.YieldStatement(this.Kind(), attributeLists, yieldKeyword, returnOrBreakKeyword, expression, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new YieldStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.YieldKeyword, this.ReturnOrBreakKeyword, this.Expression, this.SemicolonToken);
        public YieldStatementSyntax WithYieldKeyword(SyntaxToken yieldKeyword) => Update(this.AttributeLists, yieldKeyword, this.ReturnOrBreakKeyword, this.Expression, this.SemicolonToken);
        public YieldStatementSyntax WithReturnOrBreakKeyword(SyntaxToken returnOrBreakKeyword) => Update(this.AttributeLists, this.YieldKeyword, returnOrBreakKeyword, this.Expression, this.SemicolonToken);
        public YieldStatementSyntax WithExpression(ExpressionSyntax? expression) => Update(this.AttributeLists, this.YieldKeyword, this.ReturnOrBreakKeyword, expression, this.SemicolonToken);
        public YieldStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.YieldKeyword, this.ReturnOrBreakKeyword, this.Expression, semicolonToken);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new YieldStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.WhileStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class WhileStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private ExpressionSyntax? condition;
        private StatementSyntax? statement;

        internal WhileStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken WhileKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.WhileStatementSyntax)this.Green).whileKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.WhileStatementSyntax)this.Green).openParenToken, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax Condition => GetRed(ref this.condition, 3)!;

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.WhileStatementSyntax)this.Green).closeParenToken, GetChildPosition(4), GetChildIndex(4));

        public StatementSyntax Statement => GetRed(ref this.statement, 5)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.condition, 3)!,
                5 => GetRed(ref this.statement, 5)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.condition,
                5 => this.statement,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitWhileStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitWhileStatement(this);

        public WhileStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != this.AttributeLists || whileKeyword != this.WhileKeyword || openParenToken != this.OpenParenToken || condition != this.Condition || closeParenToken != this.CloseParenToken || statement != this.Statement)
            {
                var newNode = SyntaxFactory.WhileStatement(attributeLists, whileKeyword, openParenToken, condition, closeParenToken, statement);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new WhileStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.WhileKeyword, this.OpenParenToken, this.Condition, this.CloseParenToken, this.Statement);
        public WhileStatementSyntax WithWhileKeyword(SyntaxToken whileKeyword) => Update(this.AttributeLists, whileKeyword, this.OpenParenToken, this.Condition, this.CloseParenToken, this.Statement);
        public WhileStatementSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.AttributeLists, this.WhileKeyword, openParenToken, this.Condition, this.CloseParenToken, this.Statement);
        public WhileStatementSyntax WithCondition(ExpressionSyntax condition) => Update(this.AttributeLists, this.WhileKeyword, this.OpenParenToken, condition, this.CloseParenToken, this.Statement);
        public WhileStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.AttributeLists, this.WhileKeyword, this.OpenParenToken, this.Condition, closeParenToken, this.Statement);
        public WhileStatementSyntax WithStatement(StatementSyntax statement) => Update(this.AttributeLists, this.WhileKeyword, this.OpenParenToken, this.Condition, this.CloseParenToken, statement);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new WhileStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.DoStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class DoStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private StatementSyntax? statement;
        private ExpressionSyntax? condition;

        internal DoStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken DoKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.DoStatementSyntax)this.Green).doKeyword, GetChildPosition(1), GetChildIndex(1));

        public StatementSyntax Statement => GetRed(ref this.statement, 2)!;

        public SyntaxToken WhileKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.DoStatementSyntax)this.Green).whileKeyword, GetChildPosition(3), GetChildIndex(3));

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.DoStatementSyntax)this.Green).openParenToken, GetChildPosition(4), GetChildIndex(4));

        public ExpressionSyntax Condition => GetRed(ref this.condition, 5)!;

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.DoStatementSyntax)this.Green).closeParenToken, GetChildPosition(6), GetChildIndex(6));

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.DoStatementSyntax)this.Green).semicolonToken, GetChildPosition(7), GetChildIndex(7));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.statement, 2)!,
                5 => GetRed(ref this.condition, 5)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.statement,
                5 => this.condition,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitDoStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDoStatement(this);

        public DoStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken doKeyword, StatementSyntax statement, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || doKeyword != this.DoKeyword || statement != this.Statement || whileKeyword != this.WhileKeyword || openParenToken != this.OpenParenToken || condition != this.Condition || closeParenToken != this.CloseParenToken || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.DoStatement(attributeLists, doKeyword, statement, whileKeyword, openParenToken, condition, closeParenToken, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new DoStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.DoKeyword, this.Statement, this.WhileKeyword, this.OpenParenToken, this.Condition, this.CloseParenToken, this.SemicolonToken);
        public DoStatementSyntax WithDoKeyword(SyntaxToken doKeyword) => Update(this.AttributeLists, doKeyword, this.Statement, this.WhileKeyword, this.OpenParenToken, this.Condition, this.CloseParenToken, this.SemicolonToken);
        public DoStatementSyntax WithStatement(StatementSyntax statement) => Update(this.AttributeLists, this.DoKeyword, statement, this.WhileKeyword, this.OpenParenToken, this.Condition, this.CloseParenToken, this.SemicolonToken);
        public DoStatementSyntax WithWhileKeyword(SyntaxToken whileKeyword) => Update(this.AttributeLists, this.DoKeyword, this.Statement, whileKeyword, this.OpenParenToken, this.Condition, this.CloseParenToken, this.SemicolonToken);
        public DoStatementSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.AttributeLists, this.DoKeyword, this.Statement, this.WhileKeyword, openParenToken, this.Condition, this.CloseParenToken, this.SemicolonToken);
        public DoStatementSyntax WithCondition(ExpressionSyntax condition) => Update(this.AttributeLists, this.DoKeyword, this.Statement, this.WhileKeyword, this.OpenParenToken, condition, this.CloseParenToken, this.SemicolonToken);
        public DoStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.AttributeLists, this.DoKeyword, this.Statement, this.WhileKeyword, this.OpenParenToken, this.Condition, closeParenToken, this.SemicolonToken);
        public DoStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.DoKeyword, this.Statement, this.WhileKeyword, this.OpenParenToken, this.Condition, this.CloseParenToken, semicolonToken);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new DoStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ForStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ForStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private VariableDeclarationSyntax? declaration;
        private SyntaxNode? initializers;
        private ExpressionSyntax? condition;
        private SyntaxNode? incrementors;
        private StatementSyntax? statement;

        internal ForStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken ForKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ForStatementSyntax)this.Green).forKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ForStatementSyntax)this.Green).openParenToken, GetChildPosition(2), GetChildIndex(2));

        public VariableDeclarationSyntax? Declaration => GetRed(ref this.declaration, 3);

        public SeparatedSyntaxList<ExpressionSyntax> Initializers
        {
            get
            {
                var red = GetRed(ref this.initializers, 4);
                return red != null ? new SeparatedSyntaxList<ExpressionSyntax>(red, GetChildIndex(4)) : default;
            }
        }

        public SyntaxToken FirstSemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ForStatementSyntax)this.Green).firstSemicolonToken, GetChildPosition(5), GetChildIndex(5));

        public ExpressionSyntax? Condition => GetRed(ref this.condition, 6);

        public SyntaxToken SecondSemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ForStatementSyntax)this.Green).secondSemicolonToken, GetChildPosition(7), GetChildIndex(7));

        public SeparatedSyntaxList<ExpressionSyntax> Incrementors
        {
            get
            {
                var red = GetRed(ref this.incrementors, 8);
                return red != null ? new SeparatedSyntaxList<ExpressionSyntax>(red, GetChildIndex(8)) : default;
            }
        }

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ForStatementSyntax)this.Green).closeParenToken, GetChildPosition(9), GetChildIndex(9));

        public StatementSyntax Statement => GetRed(ref this.statement, 10)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.declaration, 3),
                4 => GetRed(ref this.initializers, 4)!,
                6 => GetRed(ref this.condition, 6),
                8 => GetRed(ref this.incrementors, 8)!,
                10 => GetRed(ref this.statement, 10)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.declaration,
                4 => this.initializers,
                6 => this.condition,
                8 => this.incrementors,
                10 => this.statement,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitForStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitForStatement(this);

        public ForStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken forKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, SeparatedSyntaxList<ExpressionSyntax> initializers, SyntaxToken firstSemicolonToken, ExpressionSyntax? condition, SyntaxToken secondSemicolonToken, SeparatedSyntaxList<ExpressionSyntax> incrementors, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != this.AttributeLists || forKeyword != this.ForKeyword || openParenToken != this.OpenParenToken || declaration != this.Declaration || initializers != this.Initializers || firstSemicolonToken != this.FirstSemicolonToken || condition != this.Condition || secondSemicolonToken != this.SecondSemicolonToken || incrementors != this.Incrementors || closeParenToken != this.CloseParenToken || statement != this.Statement)
            {
                var newNode = SyntaxFactory.ForStatement(attributeLists, forKeyword, openParenToken, declaration, initializers, firstSemicolonToken, condition, secondSemicolonToken, incrementors, closeParenToken, statement);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new ForStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.ForKeyword, this.OpenParenToken, this.Declaration, this.Initializers, this.FirstSemicolonToken, this.Condition, this.SecondSemicolonToken, this.Incrementors, this.CloseParenToken, this.Statement);
        public ForStatementSyntax WithForKeyword(SyntaxToken forKeyword) => Update(this.AttributeLists, forKeyword, this.OpenParenToken, this.Declaration, this.Initializers, this.FirstSemicolonToken, this.Condition, this.SecondSemicolonToken, this.Incrementors, this.CloseParenToken, this.Statement);
        public ForStatementSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.AttributeLists, this.ForKeyword, openParenToken, this.Declaration, this.Initializers, this.FirstSemicolonToken, this.Condition, this.SecondSemicolonToken, this.Incrementors, this.CloseParenToken, this.Statement);
        public ForStatementSyntax WithDeclaration(VariableDeclarationSyntax? declaration) => Update(this.AttributeLists, this.ForKeyword, this.OpenParenToken, declaration, this.Initializers, this.FirstSemicolonToken, this.Condition, this.SecondSemicolonToken, this.Incrementors, this.CloseParenToken, this.Statement);
        public ForStatementSyntax WithInitializers(SeparatedSyntaxList<ExpressionSyntax> initializers) => Update(this.AttributeLists, this.ForKeyword, this.OpenParenToken, this.Declaration, initializers, this.FirstSemicolonToken, this.Condition, this.SecondSemicolonToken, this.Incrementors, this.CloseParenToken, this.Statement);
        public ForStatementSyntax WithFirstSemicolonToken(SyntaxToken firstSemicolonToken) => Update(this.AttributeLists, this.ForKeyword, this.OpenParenToken, this.Declaration, this.Initializers, firstSemicolonToken, this.Condition, this.SecondSemicolonToken, this.Incrementors, this.CloseParenToken, this.Statement);
        public ForStatementSyntax WithCondition(ExpressionSyntax? condition) => Update(this.AttributeLists, this.ForKeyword, this.OpenParenToken, this.Declaration, this.Initializers, this.FirstSemicolonToken, condition, this.SecondSemicolonToken, this.Incrementors, this.CloseParenToken, this.Statement);
        public ForStatementSyntax WithSecondSemicolonToken(SyntaxToken secondSemicolonToken) => Update(this.AttributeLists, this.ForKeyword, this.OpenParenToken, this.Declaration, this.Initializers, this.FirstSemicolonToken, this.Condition, secondSemicolonToken, this.Incrementors, this.CloseParenToken, this.Statement);
        public ForStatementSyntax WithIncrementors(SeparatedSyntaxList<ExpressionSyntax> incrementors) => Update(this.AttributeLists, this.ForKeyword, this.OpenParenToken, this.Declaration, this.Initializers, this.FirstSemicolonToken, this.Condition, this.SecondSemicolonToken, incrementors, this.CloseParenToken, this.Statement);
        public ForStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.AttributeLists, this.ForKeyword, this.OpenParenToken, this.Declaration, this.Initializers, this.FirstSemicolonToken, this.Condition, this.SecondSemicolonToken, this.Incrementors, closeParenToken, this.Statement);
        public ForStatementSyntax WithStatement(StatementSyntax statement) => Update(this.AttributeLists, this.ForKeyword, this.OpenParenToken, this.Declaration, this.Initializers, this.FirstSemicolonToken, this.Condition, this.SecondSemicolonToken, this.Incrementors, this.CloseParenToken, statement);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new ForStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        public ForStatementSyntax AddInitializers(params ExpressionSyntax[] items) => WithInitializers(this.Initializers.AddRange(items));
        public ForStatementSyntax AddIncrementors(params ExpressionSyntax[] items) => WithIncrementors(this.Incrementors.AddRange(items));
    }

    public abstract partial class CommonForEachStatementSyntax : StatementSyntax
    {
        internal CommonForEachStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public abstract SyntaxToken AwaitKeyword { get; }
        public CommonForEachStatementSyntax WithAwaitKeyword(SyntaxToken awaitKeyword) => WithAwaitKeywordCore(awaitKeyword);
        internal abstract CommonForEachStatementSyntax WithAwaitKeywordCore(SyntaxToken awaitKeyword);

        public abstract SyntaxToken ForEachKeyword { get; }
        public CommonForEachStatementSyntax WithForEachKeyword(SyntaxToken forEachKeyword) => WithForEachKeywordCore(forEachKeyword);
        internal abstract CommonForEachStatementSyntax WithForEachKeywordCore(SyntaxToken forEachKeyword);

        public abstract SyntaxToken OpenParenToken { get; }
        public CommonForEachStatementSyntax WithOpenParenToken(SyntaxToken openParenToken) => WithOpenParenTokenCore(openParenToken);
        internal abstract CommonForEachStatementSyntax WithOpenParenTokenCore(SyntaxToken openParenToken);

        public abstract SyntaxToken InKeyword { get; }
        public CommonForEachStatementSyntax WithInKeyword(SyntaxToken inKeyword) => WithInKeywordCore(inKeyword);
        internal abstract CommonForEachStatementSyntax WithInKeywordCore(SyntaxToken inKeyword);

        public abstract ExpressionSyntax Expression { get; }
        public CommonForEachStatementSyntax WithExpression(ExpressionSyntax expression) => WithExpressionCore(expression);
        internal abstract CommonForEachStatementSyntax WithExpressionCore(ExpressionSyntax expression);

        public abstract SyntaxToken CloseParenToken { get; }
        public CommonForEachStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken) => WithCloseParenTokenCore(closeParenToken);
        internal abstract CommonForEachStatementSyntax WithCloseParenTokenCore(SyntaxToken closeParenToken);

        public abstract StatementSyntax Statement { get; }
        public CommonForEachStatementSyntax WithStatement(StatementSyntax statement) => WithStatementCore(statement);
        internal abstract CommonForEachStatementSyntax WithStatementCore(StatementSyntax statement);

        public new CommonForEachStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => (CommonForEachStatementSyntax)WithAttributeListsCore(attributeLists);

        public new CommonForEachStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => (CommonForEachStatementSyntax)AddAttributeListsCore(items);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ForEachStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ForEachStatementSyntax : CommonForEachStatementSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeSyntax? type;
        private ExpressionSyntax? expression;
        private StatementSyntax? statement;

        internal ForEachStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxToken AwaitKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.ForEachStatementSyntax)this.Green).awaitKeyword;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public override SyntaxToken ForEachKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ForEachStatementSyntax)this.Green).forEachKeyword, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ForEachStatementSyntax)this.Green).openParenToken, GetChildPosition(3), GetChildIndex(3));

        public TypeSyntax Type => GetRed(ref this.type, 4)!;

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.ForEachStatementSyntax)this.Green).identifier, GetChildPosition(5), GetChildIndex(5));

        public override SyntaxToken InKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ForEachStatementSyntax)this.Green).inKeyword, GetChildPosition(6), GetChildIndex(6));

        public override ExpressionSyntax Expression => GetRed(ref this.expression, 7)!;

        public override SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ForEachStatementSyntax)this.Green).closeParenToken, GetChildPosition(8), GetChildIndex(8));

        public override StatementSyntax Statement => GetRed(ref this.statement, 9)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                4 => GetRed(ref this.type, 4)!,
                7 => GetRed(ref this.expression, 7)!,
                9 => GetRed(ref this.statement, 9)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                4 => this.type,
                7 => this.expression,
                9 => this.statement,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitForEachStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitForEachStatement(this);

        public ForEachStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != this.AttributeLists || awaitKeyword != this.AwaitKeyword || forEachKeyword != this.ForEachKeyword || openParenToken != this.OpenParenToken || type != this.Type || identifier != this.Identifier || inKeyword != this.InKeyword || expression != this.Expression || closeParenToken != this.CloseParenToken || statement != this.Statement)
            {
                var newNode = SyntaxFactory.ForEachStatement(attributeLists, awaitKeyword, forEachKeyword, openParenToken, type, identifier, inKeyword, expression, closeParenToken, statement);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new ForEachStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.AwaitKeyword, this.ForEachKeyword, this.OpenParenToken, this.Type, this.Identifier, this.InKeyword, this.Expression, this.CloseParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithAwaitKeywordCore(SyntaxToken awaitKeyword) => WithAwaitKeyword(awaitKeyword);
        public new ForEachStatementSyntax WithAwaitKeyword(SyntaxToken awaitKeyword) => Update(this.AttributeLists, awaitKeyword, this.ForEachKeyword, this.OpenParenToken, this.Type, this.Identifier, this.InKeyword, this.Expression, this.CloseParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithForEachKeywordCore(SyntaxToken forEachKeyword) => WithForEachKeyword(forEachKeyword);
        public new ForEachStatementSyntax WithForEachKeyword(SyntaxToken forEachKeyword) => Update(this.AttributeLists, this.AwaitKeyword, forEachKeyword, this.OpenParenToken, this.Type, this.Identifier, this.InKeyword, this.Expression, this.CloseParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithOpenParenTokenCore(SyntaxToken openParenToken) => WithOpenParenToken(openParenToken);
        public new ForEachStatementSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.AttributeLists, this.AwaitKeyword, this.ForEachKeyword, openParenToken, this.Type, this.Identifier, this.InKeyword, this.Expression, this.CloseParenToken, this.Statement);
        public ForEachStatementSyntax WithType(TypeSyntax type) => Update(this.AttributeLists, this.AwaitKeyword, this.ForEachKeyword, this.OpenParenToken, type, this.Identifier, this.InKeyword, this.Expression, this.CloseParenToken, this.Statement);
        public ForEachStatementSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.AwaitKeyword, this.ForEachKeyword, this.OpenParenToken, this.Type, identifier, this.InKeyword, this.Expression, this.CloseParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithInKeywordCore(SyntaxToken inKeyword) => WithInKeyword(inKeyword);
        public new ForEachStatementSyntax WithInKeyword(SyntaxToken inKeyword) => Update(this.AttributeLists, this.AwaitKeyword, this.ForEachKeyword, this.OpenParenToken, this.Type, this.Identifier, inKeyword, this.Expression, this.CloseParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithExpressionCore(ExpressionSyntax expression) => WithExpression(expression);
        public new ForEachStatementSyntax WithExpression(ExpressionSyntax expression) => Update(this.AttributeLists, this.AwaitKeyword, this.ForEachKeyword, this.OpenParenToken, this.Type, this.Identifier, this.InKeyword, expression, this.CloseParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithCloseParenTokenCore(SyntaxToken closeParenToken) => WithCloseParenToken(closeParenToken);
        public new ForEachStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.AttributeLists, this.AwaitKeyword, this.ForEachKeyword, this.OpenParenToken, this.Type, this.Identifier, this.InKeyword, this.Expression, closeParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithStatementCore(StatementSyntax statement) => WithStatement(statement);
        public new ForEachStatementSyntax WithStatement(StatementSyntax statement) => Update(this.AttributeLists, this.AwaitKeyword, this.ForEachKeyword, this.OpenParenToken, this.Type, this.Identifier, this.InKeyword, this.Expression, this.CloseParenToken, statement);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new ForEachStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ForEachVariableStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ForEachVariableStatementSyntax : CommonForEachStatementSyntax
    {
        private SyntaxNode? attributeLists;
        private ExpressionSyntax? variable;
        private ExpressionSyntax? expression;
        private StatementSyntax? statement;

        internal ForEachVariableStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxToken AwaitKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.ForEachVariableStatementSyntax)this.Green).awaitKeyword;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public override SyntaxToken ForEachKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ForEachVariableStatementSyntax)this.Green).forEachKeyword, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ForEachVariableStatementSyntax)this.Green).openParenToken, GetChildPosition(3), GetChildIndex(3));

        /// <summary>
        /// The variable(s) of the loop. In correct code this is a tuple
        /// literal, declaration expression with a tuple designator, or
        /// a discard syntax in the form of a simple identifier. In broken
        /// code it could be something else.
        /// </summary>
        public ExpressionSyntax Variable => GetRed(ref this.variable, 4)!;

        public override SyntaxToken InKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ForEachVariableStatementSyntax)this.Green).inKeyword, GetChildPosition(5), GetChildIndex(5));

        public override ExpressionSyntax Expression => GetRed(ref this.expression, 6)!;

        public override SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ForEachVariableStatementSyntax)this.Green).closeParenToken, GetChildPosition(7), GetChildIndex(7));

        public override StatementSyntax Statement => GetRed(ref this.statement, 8)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                4 => GetRed(ref this.variable, 4)!,
                6 => GetRed(ref this.expression, 6)!,
                8 => GetRed(ref this.statement, 8)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                4 => this.variable,
                6 => this.expression,
                8 => this.statement,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitForEachVariableStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitForEachVariableStatement(this);

        public ForEachVariableStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, ExpressionSyntax variable, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != this.AttributeLists || awaitKeyword != this.AwaitKeyword || forEachKeyword != this.ForEachKeyword || openParenToken != this.OpenParenToken || variable != this.Variable || inKeyword != this.InKeyword || expression != this.Expression || closeParenToken != this.CloseParenToken || statement != this.Statement)
            {
                var newNode = SyntaxFactory.ForEachVariableStatement(attributeLists, awaitKeyword, forEachKeyword, openParenToken, variable, inKeyword, expression, closeParenToken, statement);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new ForEachVariableStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.AwaitKeyword, this.ForEachKeyword, this.OpenParenToken, this.Variable, this.InKeyword, this.Expression, this.CloseParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithAwaitKeywordCore(SyntaxToken awaitKeyword) => WithAwaitKeyword(awaitKeyword);
        public new ForEachVariableStatementSyntax WithAwaitKeyword(SyntaxToken awaitKeyword) => Update(this.AttributeLists, awaitKeyword, this.ForEachKeyword, this.OpenParenToken, this.Variable, this.InKeyword, this.Expression, this.CloseParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithForEachKeywordCore(SyntaxToken forEachKeyword) => WithForEachKeyword(forEachKeyword);
        public new ForEachVariableStatementSyntax WithForEachKeyword(SyntaxToken forEachKeyword) => Update(this.AttributeLists, this.AwaitKeyword, forEachKeyword, this.OpenParenToken, this.Variable, this.InKeyword, this.Expression, this.CloseParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithOpenParenTokenCore(SyntaxToken openParenToken) => WithOpenParenToken(openParenToken);
        public new ForEachVariableStatementSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.AttributeLists, this.AwaitKeyword, this.ForEachKeyword, openParenToken, this.Variable, this.InKeyword, this.Expression, this.CloseParenToken, this.Statement);
        public ForEachVariableStatementSyntax WithVariable(ExpressionSyntax variable) => Update(this.AttributeLists, this.AwaitKeyword, this.ForEachKeyword, this.OpenParenToken, variable, this.InKeyword, this.Expression, this.CloseParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithInKeywordCore(SyntaxToken inKeyword) => WithInKeyword(inKeyword);
        public new ForEachVariableStatementSyntax WithInKeyword(SyntaxToken inKeyword) => Update(this.AttributeLists, this.AwaitKeyword, this.ForEachKeyword, this.OpenParenToken, this.Variable, inKeyword, this.Expression, this.CloseParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithExpressionCore(ExpressionSyntax expression) => WithExpression(expression);
        public new ForEachVariableStatementSyntax WithExpression(ExpressionSyntax expression) => Update(this.AttributeLists, this.AwaitKeyword, this.ForEachKeyword, this.OpenParenToken, this.Variable, this.InKeyword, expression, this.CloseParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithCloseParenTokenCore(SyntaxToken closeParenToken) => WithCloseParenToken(closeParenToken);
        public new ForEachVariableStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.AttributeLists, this.AwaitKeyword, this.ForEachKeyword, this.OpenParenToken, this.Variable, this.InKeyword, this.Expression, closeParenToken, this.Statement);
        internal override CommonForEachStatementSyntax WithStatementCore(StatementSyntax statement) => WithStatement(statement);
        public new ForEachVariableStatementSyntax WithStatement(StatementSyntax statement) => Update(this.AttributeLists, this.AwaitKeyword, this.ForEachKeyword, this.OpenParenToken, this.Variable, this.InKeyword, this.Expression, this.CloseParenToken, statement);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new ForEachVariableStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.UsingStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class UsingStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private VariableDeclarationSyntax? declaration;
        private ExpressionSyntax? expression;
        private StatementSyntax? statement;

        internal UsingStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken AwaitKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.UsingStatementSyntax)this.Green).awaitKeyword;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public SyntaxToken UsingKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.UsingStatementSyntax)this.Green).usingKeyword, GetChildPosition(2), GetChildIndex(2));

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.UsingStatementSyntax)this.Green).openParenToken, GetChildPosition(3), GetChildIndex(3));

        public VariableDeclarationSyntax? Declaration => GetRed(ref this.declaration, 4);

        public ExpressionSyntax? Expression => GetRed(ref this.expression, 5);

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.UsingStatementSyntax)this.Green).closeParenToken, GetChildPosition(6), GetChildIndex(6));

        public StatementSyntax Statement => GetRed(ref this.statement, 7)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                4 => GetRed(ref this.declaration, 4),
                5 => GetRed(ref this.expression, 5),
                7 => GetRed(ref this.statement, 7)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                4 => this.declaration,
                5 => this.expression,
                7 => this.statement,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitUsingStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitUsingStatement(this);

        public UsingStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken usingKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, ExpressionSyntax? expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != this.AttributeLists || awaitKeyword != this.AwaitKeyword || usingKeyword != this.UsingKeyword || openParenToken != this.OpenParenToken || declaration != this.Declaration || expression != this.Expression || closeParenToken != this.CloseParenToken || statement != this.Statement)
            {
                var newNode = SyntaxFactory.UsingStatement(attributeLists, awaitKeyword, usingKeyword, openParenToken, declaration, expression, closeParenToken, statement);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new UsingStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.AwaitKeyword, this.UsingKeyword, this.OpenParenToken, this.Declaration, this.Expression, this.CloseParenToken, this.Statement);
        public UsingStatementSyntax WithAwaitKeyword(SyntaxToken awaitKeyword) => Update(this.AttributeLists, awaitKeyword, this.UsingKeyword, this.OpenParenToken, this.Declaration, this.Expression, this.CloseParenToken, this.Statement);
        public UsingStatementSyntax WithUsingKeyword(SyntaxToken usingKeyword) => Update(this.AttributeLists, this.AwaitKeyword, usingKeyword, this.OpenParenToken, this.Declaration, this.Expression, this.CloseParenToken, this.Statement);
        public UsingStatementSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.AttributeLists, this.AwaitKeyword, this.UsingKeyword, openParenToken, this.Declaration, this.Expression, this.CloseParenToken, this.Statement);
        public UsingStatementSyntax WithDeclaration(VariableDeclarationSyntax? declaration) => Update(this.AttributeLists, this.AwaitKeyword, this.UsingKeyword, this.OpenParenToken, declaration, this.Expression, this.CloseParenToken, this.Statement);
        public UsingStatementSyntax WithExpression(ExpressionSyntax? expression) => Update(this.AttributeLists, this.AwaitKeyword, this.UsingKeyword, this.OpenParenToken, this.Declaration, expression, this.CloseParenToken, this.Statement);
        public UsingStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.AttributeLists, this.AwaitKeyword, this.UsingKeyword, this.OpenParenToken, this.Declaration, this.Expression, closeParenToken, this.Statement);
        public UsingStatementSyntax WithStatement(StatementSyntax statement) => Update(this.AttributeLists, this.AwaitKeyword, this.UsingKeyword, this.OpenParenToken, this.Declaration, this.Expression, this.CloseParenToken, statement);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new UsingStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.FixedStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class FixedStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private VariableDeclarationSyntax? declaration;
        private StatementSyntax? statement;

        internal FixedStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken FixedKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.FixedStatementSyntax)this.Green).fixedKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.FixedStatementSyntax)this.Green).openParenToken, GetChildPosition(2), GetChildIndex(2));

        public VariableDeclarationSyntax Declaration => GetRed(ref this.declaration, 3)!;

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.FixedStatementSyntax)this.Green).closeParenToken, GetChildPosition(4), GetChildIndex(4));

        public StatementSyntax Statement => GetRed(ref this.statement, 5)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.declaration, 3)!,
                5 => GetRed(ref this.statement, 5)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.declaration,
                5 => this.statement,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitFixedStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFixedStatement(this);

        public FixedStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken fixedKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax declaration, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != this.AttributeLists || fixedKeyword != this.FixedKeyword || openParenToken != this.OpenParenToken || declaration != this.Declaration || closeParenToken != this.CloseParenToken || statement != this.Statement)
            {
                var newNode = SyntaxFactory.FixedStatement(attributeLists, fixedKeyword, openParenToken, declaration, closeParenToken, statement);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new FixedStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.FixedKeyword, this.OpenParenToken, this.Declaration, this.CloseParenToken, this.Statement);
        public FixedStatementSyntax WithFixedKeyword(SyntaxToken fixedKeyword) => Update(this.AttributeLists, fixedKeyword, this.OpenParenToken, this.Declaration, this.CloseParenToken, this.Statement);
        public FixedStatementSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.AttributeLists, this.FixedKeyword, openParenToken, this.Declaration, this.CloseParenToken, this.Statement);
        public FixedStatementSyntax WithDeclaration(VariableDeclarationSyntax declaration) => Update(this.AttributeLists, this.FixedKeyword, this.OpenParenToken, declaration, this.CloseParenToken, this.Statement);
        public FixedStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.AttributeLists, this.FixedKeyword, this.OpenParenToken, this.Declaration, closeParenToken, this.Statement);
        public FixedStatementSyntax WithStatement(StatementSyntax statement) => Update(this.AttributeLists, this.FixedKeyword, this.OpenParenToken, this.Declaration, this.CloseParenToken, statement);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new FixedStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        public FixedStatementSyntax AddDeclarationVariables(params VariableDeclaratorSyntax[] items) => WithDeclaration(this.Declaration.WithVariables(this.Declaration.Variables.AddRange(items)));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.CheckedStatement"/></description></item>
    /// <item><description><see cref="SyntaxKind.UncheckedStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class CheckedStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private BlockSyntax? block;

        internal CheckedStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.CheckedStatementSyntax)this.Green).keyword, GetChildPosition(1), GetChildIndex(1));

        public BlockSyntax Block => GetRed(ref this.block, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.block, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.block,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitCheckedStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCheckedStatement(this);

        public CheckedStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken keyword, BlockSyntax block)
        {
            if (attributeLists != this.AttributeLists || keyword != this.Keyword || block != this.Block)
            {
                var newNode = SyntaxFactory.CheckedStatement(this.Kind(), attributeLists, keyword, block);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new CheckedStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Keyword, this.Block);
        public CheckedStatementSyntax WithKeyword(SyntaxToken keyword) => Update(this.AttributeLists, keyword, this.Block);
        public CheckedStatementSyntax WithBlock(BlockSyntax block) => Update(this.AttributeLists, this.Keyword, block);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new CheckedStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        public CheckedStatementSyntax AddBlockAttributeLists(params AttributeListSyntax[] items) => WithBlock(this.Block.WithAttributeLists(this.Block.AttributeLists.AddRange(items)));
        public CheckedStatementSyntax AddBlockStatements(params StatementSyntax[] items) => WithBlock(this.Block.WithStatements(this.Block.Statements.AddRange(items)));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.UnsafeStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class UnsafeStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private BlockSyntax? block;

        internal UnsafeStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken UnsafeKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.UnsafeStatementSyntax)this.Green).unsafeKeyword, GetChildPosition(1), GetChildIndex(1));

        public BlockSyntax Block => GetRed(ref this.block, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.block, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.block,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitUnsafeStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitUnsafeStatement(this);

        public UnsafeStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken unsafeKeyword, BlockSyntax block)
        {
            if (attributeLists != this.AttributeLists || unsafeKeyword != this.UnsafeKeyword || block != this.Block)
            {
                var newNode = SyntaxFactory.UnsafeStatement(attributeLists, unsafeKeyword, block);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new UnsafeStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.UnsafeKeyword, this.Block);
        public UnsafeStatementSyntax WithUnsafeKeyword(SyntaxToken unsafeKeyword) => Update(this.AttributeLists, unsafeKeyword, this.Block);
        public UnsafeStatementSyntax WithBlock(BlockSyntax block) => Update(this.AttributeLists, this.UnsafeKeyword, block);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new UnsafeStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        public UnsafeStatementSyntax AddBlockAttributeLists(params AttributeListSyntax[] items) => WithBlock(this.Block.WithAttributeLists(this.Block.AttributeLists.AddRange(items)));
        public UnsafeStatementSyntax AddBlockStatements(params StatementSyntax[] items) => WithBlock(this.Block.WithStatements(this.Block.Statements.AddRange(items)));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.LockStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class LockStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private ExpressionSyntax? expression;
        private StatementSyntax? statement;

        internal LockStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken LockKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.LockStatementSyntax)this.Green).lockKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.LockStatementSyntax)this.Green).openParenToken, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax Expression => GetRed(ref this.expression, 3)!;

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.LockStatementSyntax)this.Green).closeParenToken, GetChildPosition(4), GetChildIndex(4));

        public StatementSyntax Statement => GetRed(ref this.statement, 5)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.expression, 3)!,
                5 => GetRed(ref this.statement, 5)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.expression,
                5 => this.statement,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitLockStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLockStatement(this);

        public LockStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken lockKeyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != this.AttributeLists || lockKeyword != this.LockKeyword || openParenToken != this.OpenParenToken || expression != this.Expression || closeParenToken != this.CloseParenToken || statement != this.Statement)
            {
                var newNode = SyntaxFactory.LockStatement(attributeLists, lockKeyword, openParenToken, expression, closeParenToken, statement);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new LockStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.LockKeyword, this.OpenParenToken, this.Expression, this.CloseParenToken, this.Statement);
        public LockStatementSyntax WithLockKeyword(SyntaxToken lockKeyword) => Update(this.AttributeLists, lockKeyword, this.OpenParenToken, this.Expression, this.CloseParenToken, this.Statement);
        public LockStatementSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.AttributeLists, this.LockKeyword, openParenToken, this.Expression, this.CloseParenToken, this.Statement);
        public LockStatementSyntax WithExpression(ExpressionSyntax expression) => Update(this.AttributeLists, this.LockKeyword, this.OpenParenToken, expression, this.CloseParenToken, this.Statement);
        public LockStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.AttributeLists, this.LockKeyword, this.OpenParenToken, this.Expression, closeParenToken, this.Statement);
        public LockStatementSyntax WithStatement(StatementSyntax statement) => Update(this.AttributeLists, this.LockKeyword, this.OpenParenToken, this.Expression, this.CloseParenToken, statement);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new LockStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <summary>
    /// Represents an if statement syntax.
    /// </summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.IfStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class IfStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private ExpressionSyntax? condition;
        private StatementSyntax? statement;
        private ElseClauseSyntax? @else;

        internal IfStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        /// <summary>
        /// Gets a SyntaxToken that represents the if keyword.
        /// </summary>
        public SyntaxToken IfKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.IfStatementSyntax)this.Green).ifKeyword, GetChildPosition(1), GetChildIndex(1));

        /// <summary>
        /// Gets a SyntaxToken that represents the open parenthesis before the if statement's condition expression.
        /// </summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.IfStatementSyntax)this.Green).openParenToken, GetChildPosition(2), GetChildIndex(2));

        /// <summary>
        /// Gets an ExpressionSyntax that represents the condition of the if statement.
        /// </summary>
        public ExpressionSyntax Condition => GetRed(ref this.condition, 3)!;

        /// <summary>
        /// Gets a SyntaxToken that represents the close parenthesis after the if statement's condition expression.
        /// </summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.IfStatementSyntax)this.Green).closeParenToken, GetChildPosition(4), GetChildIndex(4));

        /// <summary>
        /// Gets a StatementSyntax the represents the statement to be executed when the condition is true.
        /// </summary>
        public StatementSyntax Statement => GetRed(ref this.statement, 5)!;

        /// <summary>
        /// Gets an ElseClauseSyntax that represents the statement to be executed when the condition is false if such statement exists.
        /// </summary>
        public ElseClauseSyntax? Else => GetRed(ref this.@else, 6);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.condition, 3)!,
                5 => GetRed(ref this.statement, 5)!,
                6 => GetRed(ref this.@else, 6),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.condition,
                5 => this.statement,
                6 => this.@else,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitIfStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitIfStatement(this);

        public IfStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken ifKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement, ElseClauseSyntax? @else)
        {
            if (attributeLists != this.AttributeLists || ifKeyword != this.IfKeyword || openParenToken != this.OpenParenToken || condition != this.Condition || closeParenToken != this.CloseParenToken || statement != this.Statement || @else != this.Else)
            {
                var newNode = SyntaxFactory.IfStatement(attributeLists, ifKeyword, openParenToken, condition, closeParenToken, statement, @else);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new IfStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.IfKeyword, this.OpenParenToken, this.Condition, this.CloseParenToken, this.Statement, this.Else);
        public IfStatementSyntax WithIfKeyword(SyntaxToken ifKeyword) => Update(this.AttributeLists, ifKeyword, this.OpenParenToken, this.Condition, this.CloseParenToken, this.Statement, this.Else);
        public IfStatementSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.AttributeLists, this.IfKeyword, openParenToken, this.Condition, this.CloseParenToken, this.Statement, this.Else);
        public IfStatementSyntax WithCondition(ExpressionSyntax condition) => Update(this.AttributeLists, this.IfKeyword, this.OpenParenToken, condition, this.CloseParenToken, this.Statement, this.Else);
        public IfStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.AttributeLists, this.IfKeyword, this.OpenParenToken, this.Condition, closeParenToken, this.Statement, this.Else);
        public IfStatementSyntax WithStatement(StatementSyntax statement) => Update(this.AttributeLists, this.IfKeyword, this.OpenParenToken, this.Condition, this.CloseParenToken, statement, this.Else);
        public IfStatementSyntax WithElse(ElseClauseSyntax? @else) => Update(this.AttributeLists, this.IfKeyword, this.OpenParenToken, this.Condition, this.CloseParenToken, this.Statement, @else);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new IfStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <summary>Represents an else statement syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ElseClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ElseClauseSyntax : CSharpSyntaxNode
    {
        private StatementSyntax? statement;

        internal ElseClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>
        /// Gets a syntax token
        /// </summary>
        public SyntaxToken ElseKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ElseClauseSyntax)this.Green).elseKeyword, Position, 0);

        public StatementSyntax Statement => GetRed(ref this.statement, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.statement, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.statement : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitElseClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitElseClause(this);

        public ElseClauseSyntax Update(SyntaxToken elseKeyword, StatementSyntax statement)
        {
            if (elseKeyword != this.ElseKeyword || statement != this.Statement)
            {
                var newNode = SyntaxFactory.ElseClause(elseKeyword, statement);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ElseClauseSyntax WithElseKeyword(SyntaxToken elseKeyword) => Update(elseKeyword, this.Statement);
        public ElseClauseSyntax WithStatement(StatementSyntax statement) => Update(this.ElseKeyword, statement);
    }

    /// <summary>Represents a switch statement syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.SwitchStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class SwitchStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private ExpressionSyntax? expression;
        private SyntaxNode? sections;

        internal SwitchStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        /// <summary>
        /// Gets a SyntaxToken that represents the switch keyword.
        /// </summary>
        public SyntaxToken SwitchKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.SwitchStatementSyntax)this.Green).switchKeyword, GetChildPosition(1), GetChildIndex(1));

        /// <summary>
        /// Gets a SyntaxToken that represents the open parenthesis preceding the switch governing expression.
        /// </summary>
        public SyntaxToken OpenParenToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.SwitchStatementSyntax)this.Green).openParenToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(2), GetChildIndex(2)) : default;
            }
        }

        /// <summary>
        /// Gets an ExpressionSyntax representing the expression of the switch statement.
        /// </summary>
        public ExpressionSyntax Expression => GetRed(ref this.expression, 3)!;

        /// <summary>
        /// Gets a SyntaxToken that represents the close parenthesis following the switch governing expression.
        /// </summary>
        public SyntaxToken CloseParenToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.SwitchStatementSyntax)this.Green).closeParenToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(4), GetChildIndex(4)) : default;
            }
        }

        /// <summary>
        /// Gets a SyntaxToken that represents the open braces preceding the switch sections.
        /// </summary>
        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.SwitchStatementSyntax)this.Green).openBraceToken, GetChildPosition(5), GetChildIndex(5));

        /// <summary>
        /// Gets a SyntaxList of SwitchSectionSyntax's that represents the switch sections of the switch statement.
        /// </summary>
        public SyntaxList<SwitchSectionSyntax> Sections => new SyntaxList<SwitchSectionSyntax>(GetRed(ref this.sections, 6));

        /// <summary>
        /// Gets a SyntaxToken that represents the open braces following the switch sections.
        /// </summary>
        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.SwitchStatementSyntax)this.Green).closeBraceToken, GetChildPosition(7), GetChildIndex(7));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.expression, 3)!,
                6 => GetRed(ref this.sections, 6)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.expression,
                6 => this.sections,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitSwitchStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSwitchStatement(this);

        public SwitchStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken switchKeyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken, SyntaxToken openBraceToken, SyntaxList<SwitchSectionSyntax> sections, SyntaxToken closeBraceToken)
        {
            if (attributeLists != this.AttributeLists || switchKeyword != this.SwitchKeyword || openParenToken != this.OpenParenToken || expression != this.Expression || closeParenToken != this.CloseParenToken || openBraceToken != this.OpenBraceToken || sections != this.Sections || closeBraceToken != this.CloseBraceToken)
            {
                var newNode = SyntaxFactory.SwitchStatement(attributeLists, switchKeyword, openParenToken, expression, closeParenToken, openBraceToken, sections, closeBraceToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new SwitchStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.SwitchKeyword, this.OpenParenToken, this.Expression, this.CloseParenToken, this.OpenBraceToken, this.Sections, this.CloseBraceToken);
        public SwitchStatementSyntax WithSwitchKeyword(SyntaxToken switchKeyword) => Update(this.AttributeLists, switchKeyword, this.OpenParenToken, this.Expression, this.CloseParenToken, this.OpenBraceToken, this.Sections, this.CloseBraceToken);
        public SwitchStatementSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.AttributeLists, this.SwitchKeyword, openParenToken, this.Expression, this.CloseParenToken, this.OpenBraceToken, this.Sections, this.CloseBraceToken);
        public SwitchStatementSyntax WithExpression(ExpressionSyntax expression) => Update(this.AttributeLists, this.SwitchKeyword, this.OpenParenToken, expression, this.CloseParenToken, this.OpenBraceToken, this.Sections, this.CloseBraceToken);
        public SwitchStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.AttributeLists, this.SwitchKeyword, this.OpenParenToken, this.Expression, closeParenToken, this.OpenBraceToken, this.Sections, this.CloseBraceToken);
        public SwitchStatementSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(this.AttributeLists, this.SwitchKeyword, this.OpenParenToken, this.Expression, this.CloseParenToken, openBraceToken, this.Sections, this.CloseBraceToken);
        public SwitchStatementSyntax WithSections(SyntaxList<SwitchSectionSyntax> sections) => Update(this.AttributeLists, this.SwitchKeyword, this.OpenParenToken, this.Expression, this.CloseParenToken, this.OpenBraceToken, sections, this.CloseBraceToken);
        public SwitchStatementSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.AttributeLists, this.SwitchKeyword, this.OpenParenToken, this.Expression, this.CloseParenToken, this.OpenBraceToken, this.Sections, closeBraceToken);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new SwitchStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        public SwitchStatementSyntax AddSections(params SwitchSectionSyntax[] items) => WithSections(this.Sections.AddRange(items));
    }

    /// <summary>Represents a switch section syntax of a switch statement.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.SwitchSection"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class SwitchSectionSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? labels;
        private SyntaxNode? statements;

        internal SwitchSectionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>
        /// Gets a SyntaxList of SwitchLabelSyntax's the represents the possible labels that control can transfer to within the section.
        /// </summary>
        public SyntaxList<SwitchLabelSyntax> Labels => new SyntaxList<SwitchLabelSyntax>(GetRed(ref this.labels, 0));

        /// <summary>
        /// Gets a SyntaxList of StatementSyntax's the represents the statements to be executed when control transfer to a label the belongs to the section.
        /// </summary>
        public SyntaxList<StatementSyntax> Statements => new SyntaxList<StatementSyntax>(GetRed(ref this.statements, 1));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.labels)!,
                1 => GetRed(ref this.statements, 1)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.labels,
                1 => this.statements,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitSwitchSection(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSwitchSection(this);

        public SwitchSectionSyntax Update(SyntaxList<SwitchLabelSyntax> labels, SyntaxList<StatementSyntax> statements)
        {
            if (labels != this.Labels || statements != this.Statements)
            {
                var newNode = SyntaxFactory.SwitchSection(labels, statements);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public SwitchSectionSyntax WithLabels(SyntaxList<SwitchLabelSyntax> labels) => Update(labels, this.Statements);
        public SwitchSectionSyntax WithStatements(SyntaxList<StatementSyntax> statements) => Update(this.Labels, statements);

        public SwitchSectionSyntax AddLabels(params SwitchLabelSyntax[] items) => WithLabels(this.Labels.AddRange(items));
        public SwitchSectionSyntax AddStatements(params StatementSyntax[] items) => WithStatements(this.Statements.AddRange(items));
    }

    /// <summary>Represents a switch label within a switch statement.</summary>
    public abstract partial class SwitchLabelSyntax : CSharpSyntaxNode
    {
        internal SwitchLabelSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>
        /// Gets a SyntaxToken that represents a case or default keyword that belongs to a switch label.
        /// </summary>
        public abstract SyntaxToken Keyword { get; }
        public SwitchLabelSyntax WithKeyword(SyntaxToken keyword) => WithKeywordCore(keyword);
        internal abstract SwitchLabelSyntax WithKeywordCore(SyntaxToken keyword);

        /// <summary>
        /// Gets a SyntaxToken that represents the colon that terminates the switch label.
        /// </summary>
        public abstract SyntaxToken ColonToken { get; }
        public SwitchLabelSyntax WithColonToken(SyntaxToken colonToken) => WithColonTokenCore(colonToken);
        internal abstract SwitchLabelSyntax WithColonTokenCore(SyntaxToken colonToken);
    }

    /// <summary>Represents a case label within a switch statement.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.CasePatternSwitchLabel"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class CasePatternSwitchLabelSyntax : SwitchLabelSyntax
    {
        private PatternSyntax? pattern;
        private WhenClauseSyntax? whenClause;

        internal CasePatternSwitchLabelSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the case keyword token.</summary>
        public override SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.CasePatternSwitchLabelSyntax)this.Green).keyword, Position, 0);

        /// <summary>
        /// Gets a PatternSyntax that represents the pattern that gets matched for the case label.
        /// </summary>
        public PatternSyntax Pattern => GetRed(ref this.pattern, 1)!;

        public WhenClauseSyntax? WhenClause => GetRed(ref this.whenClause, 2);

        public override SyntaxToken ColonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CasePatternSwitchLabelSyntax)this.Green).colonToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.pattern, 1)!,
                2 => GetRed(ref this.whenClause, 2),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.pattern,
                2 => this.whenClause,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitCasePatternSwitchLabel(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCasePatternSwitchLabel(this);

        public CasePatternSwitchLabelSyntax Update(SyntaxToken keyword, PatternSyntax pattern, WhenClauseSyntax? whenClause, SyntaxToken colonToken)
        {
            if (keyword != this.Keyword || pattern != this.Pattern || whenClause != this.WhenClause || colonToken != this.ColonToken)
            {
                var newNode = SyntaxFactory.CasePatternSwitchLabel(keyword, pattern, whenClause, colonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override SwitchLabelSyntax WithKeywordCore(SyntaxToken keyword) => WithKeyword(keyword);
        public new CasePatternSwitchLabelSyntax WithKeyword(SyntaxToken keyword) => Update(keyword, this.Pattern, this.WhenClause, this.ColonToken);
        public CasePatternSwitchLabelSyntax WithPattern(PatternSyntax pattern) => Update(this.Keyword, pattern, this.WhenClause, this.ColonToken);
        public CasePatternSwitchLabelSyntax WithWhenClause(WhenClauseSyntax? whenClause) => Update(this.Keyword, this.Pattern, whenClause, this.ColonToken);
        internal override SwitchLabelSyntax WithColonTokenCore(SyntaxToken colonToken) => WithColonToken(colonToken);
        public new CasePatternSwitchLabelSyntax WithColonToken(SyntaxToken colonToken) => Update(this.Keyword, this.Pattern, this.WhenClause, colonToken);
    }

    /// <summary>Represents a case label within a switch statement.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.CaseSwitchLabel"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class CaseSwitchLabelSyntax : SwitchLabelSyntax
    {
        private ExpressionSyntax? value;

        internal CaseSwitchLabelSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the case keyword token.</summary>
        public override SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.CaseSwitchLabelSyntax)this.Green).keyword, Position, 0);

        /// <summary>
        /// Gets an ExpressionSyntax that represents the constant expression that gets matched for the case label.
        /// </summary>
        public ExpressionSyntax Value => GetRed(ref this.value, 1)!;

        public override SyntaxToken ColonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CaseSwitchLabelSyntax)this.Green).colonToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.value, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.value : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitCaseSwitchLabel(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCaseSwitchLabel(this);

        public CaseSwitchLabelSyntax Update(SyntaxToken keyword, ExpressionSyntax value, SyntaxToken colonToken)
        {
            if (keyword != this.Keyword || value != this.Value || colonToken != this.ColonToken)
            {
                var newNode = SyntaxFactory.CaseSwitchLabel(keyword, value, colonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override SwitchLabelSyntax WithKeywordCore(SyntaxToken keyword) => WithKeyword(keyword);
        public new CaseSwitchLabelSyntax WithKeyword(SyntaxToken keyword) => Update(keyword, this.Value, this.ColonToken);
        public CaseSwitchLabelSyntax WithValue(ExpressionSyntax value) => Update(this.Keyword, value, this.ColonToken);
        internal override SwitchLabelSyntax WithColonTokenCore(SyntaxToken colonToken) => WithColonToken(colonToken);
        public new CaseSwitchLabelSyntax WithColonToken(SyntaxToken colonToken) => Update(this.Keyword, this.Value, colonToken);
    }

    /// <summary>Represents a default label within a switch statement.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.DefaultSwitchLabel"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class DefaultSwitchLabelSyntax : SwitchLabelSyntax
    {

        internal DefaultSwitchLabelSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the default keyword token.</summary>
        public override SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.DefaultSwitchLabelSyntax)this.Green).keyword, Position, 0);

        public override SyntaxToken ColonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.DefaultSwitchLabelSyntax)this.Green).colonToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitDefaultSwitchLabel(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDefaultSwitchLabel(this);

        public DefaultSwitchLabelSyntax Update(SyntaxToken keyword, SyntaxToken colonToken)
        {
            if (keyword != this.Keyword || colonToken != this.ColonToken)
            {
                var newNode = SyntaxFactory.DefaultSwitchLabel(keyword, colonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override SwitchLabelSyntax WithKeywordCore(SyntaxToken keyword) => WithKeyword(keyword);
        public new DefaultSwitchLabelSyntax WithKeyword(SyntaxToken keyword) => Update(keyword, this.ColonToken);
        internal override SwitchLabelSyntax WithColonTokenCore(SyntaxToken colonToken) => WithColonToken(colonToken);
        public new DefaultSwitchLabelSyntax WithColonToken(SyntaxToken colonToken) => Update(this.Keyword, colonToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.SwitchExpression"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class SwitchExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? governingExpression;
        private SyntaxNode? arms;

        internal SwitchExpressionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public ExpressionSyntax GoverningExpression => GetRedAtZero(ref this.governingExpression)!;

        public SyntaxToken SwitchKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.SwitchExpressionSyntax)this.Green).switchKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.SwitchExpressionSyntax)this.Green).openBraceToken, GetChildPosition(2), GetChildIndex(2));

        public SeparatedSyntaxList<SwitchExpressionArmSyntax> Arms
        {
            get
            {
                var red = GetRed(ref this.arms, 3);
                return red != null ? new SeparatedSyntaxList<SwitchExpressionArmSyntax>(red, GetChildIndex(3)) : default;
            }
        }

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.SwitchExpressionSyntax)this.Green).closeBraceToken, GetChildPosition(4), GetChildIndex(4));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.governingExpression)!,
                3 => GetRed(ref this.arms, 3)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.governingExpression,
                3 => this.arms,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitSwitchExpression(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSwitchExpression(this);

        public SwitchExpressionSyntax Update(ExpressionSyntax governingExpression, SyntaxToken switchKeyword, SyntaxToken openBraceToken, SeparatedSyntaxList<SwitchExpressionArmSyntax> arms, SyntaxToken closeBraceToken)
        {
            if (governingExpression != this.GoverningExpression || switchKeyword != this.SwitchKeyword || openBraceToken != this.OpenBraceToken || arms != this.Arms || closeBraceToken != this.CloseBraceToken)
            {
                var newNode = SyntaxFactory.SwitchExpression(governingExpression, switchKeyword, openBraceToken, arms, closeBraceToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public SwitchExpressionSyntax WithGoverningExpression(ExpressionSyntax governingExpression) => Update(governingExpression, this.SwitchKeyword, this.OpenBraceToken, this.Arms, this.CloseBraceToken);
        public SwitchExpressionSyntax WithSwitchKeyword(SyntaxToken switchKeyword) => Update(this.GoverningExpression, switchKeyword, this.OpenBraceToken, this.Arms, this.CloseBraceToken);
        public SwitchExpressionSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(this.GoverningExpression, this.SwitchKeyword, openBraceToken, this.Arms, this.CloseBraceToken);
        public SwitchExpressionSyntax WithArms(SeparatedSyntaxList<SwitchExpressionArmSyntax> arms) => Update(this.GoverningExpression, this.SwitchKeyword, this.OpenBraceToken, arms, this.CloseBraceToken);
        public SwitchExpressionSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.GoverningExpression, this.SwitchKeyword, this.OpenBraceToken, this.Arms, closeBraceToken);

        public SwitchExpressionSyntax AddArms(params SwitchExpressionArmSyntax[] items) => WithArms(this.Arms.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.SwitchExpressionArm"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class SwitchExpressionArmSyntax : CSharpSyntaxNode
    {
        private PatternSyntax? pattern;
        private WhenClauseSyntax? whenClause;
        private ExpressionSyntax? expression;

        internal SwitchExpressionArmSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public PatternSyntax Pattern => GetRedAtZero(ref this.pattern)!;

        public WhenClauseSyntax? WhenClause => GetRed(ref this.whenClause, 1);

        public SyntaxToken EqualsGreaterThanToken => new SyntaxToken(this, ((Syntax.InternalSyntax.SwitchExpressionArmSyntax)this.Green).equalsGreaterThanToken, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax Expression => GetRed(ref this.expression, 3)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.pattern)!,
                1 => GetRed(ref this.whenClause, 1),
                3 => GetRed(ref this.expression, 3)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.pattern,
                1 => this.whenClause,
                3 => this.expression,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitSwitchExpressionArm(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSwitchExpressionArm(this);

        public SwitchExpressionArmSyntax Update(PatternSyntax pattern, WhenClauseSyntax? whenClause, SyntaxToken equalsGreaterThanToken, ExpressionSyntax expression)
        {
            if (pattern != this.Pattern || whenClause != this.WhenClause || equalsGreaterThanToken != this.EqualsGreaterThanToken || expression != this.Expression)
            {
                var newNode = SyntaxFactory.SwitchExpressionArm(pattern, whenClause, equalsGreaterThanToken, expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public SwitchExpressionArmSyntax WithPattern(PatternSyntax pattern) => Update(pattern, this.WhenClause, this.EqualsGreaterThanToken, this.Expression);
        public SwitchExpressionArmSyntax WithWhenClause(WhenClauseSyntax? whenClause) => Update(this.Pattern, whenClause, this.EqualsGreaterThanToken, this.Expression);
        public SwitchExpressionArmSyntax WithEqualsGreaterThanToken(SyntaxToken equalsGreaterThanToken) => Update(this.Pattern, this.WhenClause, equalsGreaterThanToken, this.Expression);
        public SwitchExpressionArmSyntax WithExpression(ExpressionSyntax expression) => Update(this.Pattern, this.WhenClause, this.EqualsGreaterThanToken, expression);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.TryStatement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class TryStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;
        private BlockSyntax? block;
        private SyntaxNode? catches;
        private FinallyClauseSyntax? @finally;

        internal TryStatementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken TryKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.TryStatementSyntax)this.Green).tryKeyword, GetChildPosition(1), GetChildIndex(1));

        public BlockSyntax Block => GetRed(ref this.block, 2)!;

        public SyntaxList<CatchClauseSyntax> Catches => new SyntaxList<CatchClauseSyntax>(GetRed(ref this.catches, 3));

        public FinallyClauseSyntax? Finally => GetRed(ref this.@finally, 4);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.block, 2)!,
                3 => GetRed(ref this.catches, 3)!,
                4 => GetRed(ref this.@finally, 4),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.block,
                3 => this.catches,
                4 => this.@finally,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitTryStatement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTryStatement(this);

        public TryStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken tryKeyword, BlockSyntax block, SyntaxList<CatchClauseSyntax> catches, FinallyClauseSyntax? @finally)
        {
            if (attributeLists != this.AttributeLists || tryKeyword != this.TryKeyword || block != this.Block || catches != this.Catches || @finally != this.Finally)
            {
                var newNode = SyntaxFactory.TryStatement(attributeLists, tryKeyword, block, catches, @finally);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new TryStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.TryKeyword, this.Block, this.Catches, this.Finally);
        public TryStatementSyntax WithTryKeyword(SyntaxToken tryKeyword) => Update(this.AttributeLists, tryKeyword, this.Block, this.Catches, this.Finally);
        public TryStatementSyntax WithBlock(BlockSyntax block) => Update(this.AttributeLists, this.TryKeyword, block, this.Catches, this.Finally);
        public TryStatementSyntax WithCatches(SyntaxList<CatchClauseSyntax> catches) => Update(this.AttributeLists, this.TryKeyword, this.Block, catches, this.Finally);
        public TryStatementSyntax WithFinally(FinallyClauseSyntax? @finally) => Update(this.AttributeLists, this.TryKeyword, this.Block, this.Catches, @finally);

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new TryStatementSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        public TryStatementSyntax AddBlockAttributeLists(params AttributeListSyntax[] items) => WithBlock(this.Block.WithAttributeLists(this.Block.AttributeLists.AddRange(items)));
        public TryStatementSyntax AddBlockStatements(params StatementSyntax[] items) => WithBlock(this.Block.WithStatements(this.Block.Statements.AddRange(items)));
        public TryStatementSyntax AddCatches(params CatchClauseSyntax[] items) => WithCatches(this.Catches.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.CatchClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class CatchClauseSyntax : CSharpSyntaxNode
    {
        private CatchDeclarationSyntax? declaration;
        private CatchFilterClauseSyntax? filter;
        private BlockSyntax? block;

        internal CatchClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken CatchKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.CatchClauseSyntax)this.Green).catchKeyword, Position, 0);

        public CatchDeclarationSyntax? Declaration => GetRed(ref this.declaration, 1);

        public CatchFilterClauseSyntax? Filter => GetRed(ref this.filter, 2);

        public BlockSyntax Block => GetRed(ref this.block, 3)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.declaration, 1),
                2 => GetRed(ref this.filter, 2),
                3 => GetRed(ref this.block, 3)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.declaration,
                2 => this.filter,
                3 => this.block,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitCatchClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCatchClause(this);

        public CatchClauseSyntax Update(SyntaxToken catchKeyword, CatchDeclarationSyntax? declaration, CatchFilterClauseSyntax? filter, BlockSyntax block)
        {
            if (catchKeyword != this.CatchKeyword || declaration != this.Declaration || filter != this.Filter || block != this.Block)
            {
                var newNode = SyntaxFactory.CatchClause(catchKeyword, declaration, filter, block);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public CatchClauseSyntax WithCatchKeyword(SyntaxToken catchKeyword) => Update(catchKeyword, this.Declaration, this.Filter, this.Block);
        public CatchClauseSyntax WithDeclaration(CatchDeclarationSyntax? declaration) => Update(this.CatchKeyword, declaration, this.Filter, this.Block);
        public CatchClauseSyntax WithFilter(CatchFilterClauseSyntax? filter) => Update(this.CatchKeyword, this.Declaration, filter, this.Block);
        public CatchClauseSyntax WithBlock(BlockSyntax block) => Update(this.CatchKeyword, this.Declaration, this.Filter, block);

        public CatchClauseSyntax AddBlockAttributeLists(params AttributeListSyntax[] items) => WithBlock(this.Block.WithAttributeLists(this.Block.AttributeLists.AddRange(items)));
        public CatchClauseSyntax AddBlockStatements(params StatementSyntax[] items) => WithBlock(this.Block.WithStatements(this.Block.Statements.AddRange(items)));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.CatchDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class CatchDeclarationSyntax : CSharpSyntaxNode
    {
        private TypeSyntax? type;

        internal CatchDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CatchDeclarationSyntax)this.Green).openParenToken, Position, 0);

        public TypeSyntax Type => GetRed(ref this.type, 1)!;

        public SyntaxToken Identifier
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.CatchDeclarationSyntax)this.Green).identifier;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(2), GetChildIndex(2)) : default;
            }
        }

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CatchDeclarationSyntax)this.Green).closeParenToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.type, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.type : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitCatchDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCatchDeclaration(this);

        public CatchDeclarationSyntax Update(SyntaxToken openParenToken, TypeSyntax type, SyntaxToken identifier, SyntaxToken closeParenToken)
        {
            if (openParenToken != this.OpenParenToken || type != this.Type || identifier != this.Identifier || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.CatchDeclaration(openParenToken, type, identifier, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public CatchDeclarationSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(openParenToken, this.Type, this.Identifier, this.CloseParenToken);
        public CatchDeclarationSyntax WithType(TypeSyntax type) => Update(this.OpenParenToken, type, this.Identifier, this.CloseParenToken);
        public CatchDeclarationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.OpenParenToken, this.Type, identifier, this.CloseParenToken);
        public CatchDeclarationSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.OpenParenToken, this.Type, this.Identifier, closeParenToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.CatchFilterClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class CatchFilterClauseSyntax : CSharpSyntaxNode
    {
        private ExpressionSyntax? filterExpression;

        internal CatchFilterClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken WhenKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.CatchFilterClauseSyntax)this.Green).whenKeyword, Position, 0);

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CatchFilterClauseSyntax)this.Green).openParenToken, GetChildPosition(1), GetChildIndex(1));

        public ExpressionSyntax FilterExpression => GetRed(ref this.filterExpression, 2)!;

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CatchFilterClauseSyntax)this.Green).closeParenToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.filterExpression, 2)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.filterExpression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitCatchFilterClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCatchFilterClause(this);

        public CatchFilterClauseSyntax Update(SyntaxToken whenKeyword, SyntaxToken openParenToken, ExpressionSyntax filterExpression, SyntaxToken closeParenToken)
        {
            if (whenKeyword != this.WhenKeyword || openParenToken != this.OpenParenToken || filterExpression != this.FilterExpression || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.CatchFilterClause(whenKeyword, openParenToken, filterExpression, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public CatchFilterClauseSyntax WithWhenKeyword(SyntaxToken whenKeyword) => Update(whenKeyword, this.OpenParenToken, this.FilterExpression, this.CloseParenToken);
        public CatchFilterClauseSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.WhenKeyword, openParenToken, this.FilterExpression, this.CloseParenToken);
        public CatchFilterClauseSyntax WithFilterExpression(ExpressionSyntax filterExpression) => Update(this.WhenKeyword, this.OpenParenToken, filterExpression, this.CloseParenToken);
        public CatchFilterClauseSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.WhenKeyword, this.OpenParenToken, this.FilterExpression, closeParenToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.FinallyClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class FinallyClauseSyntax : CSharpSyntaxNode
    {
        private BlockSyntax? block;

        internal FinallyClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken FinallyKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.FinallyClauseSyntax)this.Green).finallyKeyword, Position, 0);

        public BlockSyntax Block => GetRed(ref this.block, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.block, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.block : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitFinallyClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFinallyClause(this);

        public FinallyClauseSyntax Update(SyntaxToken finallyKeyword, BlockSyntax block)
        {
            if (finallyKeyword != this.FinallyKeyword || block != this.Block)
            {
                var newNode = SyntaxFactory.FinallyClause(finallyKeyword, block);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public FinallyClauseSyntax WithFinallyKeyword(SyntaxToken finallyKeyword) => Update(finallyKeyword, this.Block);
        public FinallyClauseSyntax WithBlock(BlockSyntax block) => Update(this.FinallyKeyword, block);

        public FinallyClauseSyntax AddBlockAttributeLists(params AttributeListSyntax[] items) => WithBlock(this.Block.WithAttributeLists(this.Block.AttributeLists.AddRange(items)));
        public FinallyClauseSyntax AddBlockStatements(params StatementSyntax[] items) => WithBlock(this.Block.WithStatements(this.Block.Statements.AddRange(items)));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.CompilationUnit"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class CompilationUnitSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? externs;
        private SyntaxNode? usings;
        private SyntaxNode? attributeLists;
        private SyntaxNode? members;

        internal CompilationUnitSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxList<ExternAliasDirectiveSyntax> Externs => new SyntaxList<ExternAliasDirectiveSyntax>(GetRed(ref this.externs, 0));

        public SyntaxList<UsingDirectiveSyntax> Usings => new SyntaxList<UsingDirectiveSyntax>(GetRed(ref this.usings, 1));

        /// <summary>Gets the attribute declaration list.</summary>
        public SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 2));

        public SyntaxList<MemberDeclarationSyntax> Members => new SyntaxList<MemberDeclarationSyntax>(GetRed(ref this.members, 3));

        public SyntaxToken EndOfFileToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CompilationUnitSyntax)this.Green).endOfFileToken, GetChildPosition(4), GetChildIndex(4));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.externs)!,
                1 => GetRed(ref this.usings, 1)!,
                2 => GetRed(ref this.attributeLists, 2)!,
                3 => GetRed(ref this.members, 3)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.externs,
                1 => this.usings,
                2 => this.attributeLists,
                3 => this.members,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitCompilationUnit(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCompilationUnit(this);

        public CompilationUnitSyntax Update(SyntaxList<ExternAliasDirectiveSyntax> externs, SyntaxList<UsingDirectiveSyntax> usings, SyntaxList<AttributeListSyntax> attributeLists, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken endOfFileToken)
        {
            if (externs != this.Externs || usings != this.Usings || attributeLists != this.AttributeLists || members != this.Members || endOfFileToken != this.EndOfFileToken)
            {
                var newNode = SyntaxFactory.CompilationUnit(externs, usings, attributeLists, members, endOfFileToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public CompilationUnitSyntax WithExterns(SyntaxList<ExternAliasDirectiveSyntax> externs) => Update(externs, this.Usings, this.AttributeLists, this.Members, this.EndOfFileToken);
        public CompilationUnitSyntax WithUsings(SyntaxList<UsingDirectiveSyntax> usings) => Update(this.Externs, usings, this.AttributeLists, this.Members, this.EndOfFileToken);
        public CompilationUnitSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(this.Externs, this.Usings, attributeLists, this.Members, this.EndOfFileToken);
        public CompilationUnitSyntax WithMembers(SyntaxList<MemberDeclarationSyntax> members) => Update(this.Externs, this.Usings, this.AttributeLists, members, this.EndOfFileToken);
        public CompilationUnitSyntax WithEndOfFileToken(SyntaxToken endOfFileToken) => Update(this.Externs, this.Usings, this.AttributeLists, this.Members, endOfFileToken);

        public CompilationUnitSyntax AddExterns(params ExternAliasDirectiveSyntax[] items) => WithExterns(this.Externs.AddRange(items));
        public CompilationUnitSyntax AddUsings(params UsingDirectiveSyntax[] items) => WithUsings(this.Usings.AddRange(items));
        public CompilationUnitSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        public CompilationUnitSyntax AddMembers(params MemberDeclarationSyntax[] items) => WithMembers(this.Members.AddRange(items));
    }

    /// <summary>
    /// Represents an ExternAlias directive syntax, e.g. "extern alias MyAlias;" with specifying "/r:MyAlias=SomeAssembly.dll " on the compiler command line.
    /// </summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ExternAliasDirective"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ExternAliasDirectiveSyntax : CSharpSyntaxNode
    {

        internal ExternAliasDirectiveSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>SyntaxToken representing the extern keyword.</summary>
        public SyntaxToken ExternKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ExternAliasDirectiveSyntax)this.Green).externKeyword, Position, 0);

        /// <summary>SyntaxToken representing the alias keyword.</summary>
        public SyntaxToken AliasKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ExternAliasDirectiveSyntax)this.Green).aliasKeyword, GetChildPosition(1), GetChildIndex(1));

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.ExternAliasDirectiveSyntax)this.Green).identifier, GetChildPosition(2), GetChildIndex(2));

        /// <summary>SyntaxToken representing the semicolon token.</summary>
        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ExternAliasDirectiveSyntax)this.Green).semicolonToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitExternAliasDirective(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitExternAliasDirective(this);

        public ExternAliasDirectiveSyntax Update(SyntaxToken externKeyword, SyntaxToken aliasKeyword, SyntaxToken identifier, SyntaxToken semicolonToken)
        {
            if (externKeyword != this.ExternKeyword || aliasKeyword != this.AliasKeyword || identifier != this.Identifier || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.ExternAliasDirective(externKeyword, aliasKeyword, identifier, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ExternAliasDirectiveSyntax WithExternKeyword(SyntaxToken externKeyword) => Update(externKeyword, this.AliasKeyword, this.Identifier, this.SemicolonToken);
        public ExternAliasDirectiveSyntax WithAliasKeyword(SyntaxToken aliasKeyword) => Update(this.ExternKeyword, aliasKeyword, this.Identifier, this.SemicolonToken);
        public ExternAliasDirectiveSyntax WithIdentifier(SyntaxToken identifier) => Update(this.ExternKeyword, this.AliasKeyword, identifier, this.SemicolonToken);
        public ExternAliasDirectiveSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.ExternKeyword, this.AliasKeyword, this.Identifier, semicolonToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.UsingDirective"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class UsingDirectiveSyntax : CSharpSyntaxNode
    {
        private NameEqualsSyntax? alias;
        private NameSyntax? name;

        internal UsingDirectiveSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken GlobalKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.UsingDirectiveSyntax)this.Green).globalKeyword;
                return slot != null ? new SyntaxToken(this, slot, Position, 0) : default;
            }
        }

        public SyntaxToken UsingKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.UsingDirectiveSyntax)this.Green).usingKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken StaticKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.UsingDirectiveSyntax)this.Green).staticKeyword;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(2), GetChildIndex(2)) : default;
            }
        }

        public NameEqualsSyntax? Alias => GetRed(ref this.alias, 3);

        public NameSyntax Name => GetRed(ref this.name, 4)!;

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.UsingDirectiveSyntax)this.Green).semicolonToken, GetChildPosition(5), GetChildIndex(5));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                3 => GetRed(ref this.alias, 3),
                4 => GetRed(ref this.name, 4)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                3 => this.alias,
                4 => this.name,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitUsingDirective(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitUsingDirective(this);

        public UsingDirectiveSyntax Update(SyntaxToken globalKeyword, SyntaxToken usingKeyword, SyntaxToken staticKeyword, NameEqualsSyntax? alias, NameSyntax name, SyntaxToken semicolonToken)
        {
            if (globalKeyword != this.GlobalKeyword || usingKeyword != this.UsingKeyword || staticKeyword != this.StaticKeyword || alias != this.Alias || name != this.Name || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.UsingDirective(globalKeyword, usingKeyword, staticKeyword, alias, name, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public UsingDirectiveSyntax WithGlobalKeyword(SyntaxToken globalKeyword) => Update(globalKeyword, this.UsingKeyword, this.StaticKeyword, this.Alias, this.Name, this.SemicolonToken);
        public UsingDirectiveSyntax WithUsingKeyword(SyntaxToken usingKeyword) => Update(this.GlobalKeyword, usingKeyword, this.StaticKeyword, this.Alias, this.Name, this.SemicolonToken);
        public UsingDirectiveSyntax WithStaticKeyword(SyntaxToken staticKeyword) => Update(this.GlobalKeyword, this.UsingKeyword, staticKeyword, this.Alias, this.Name, this.SemicolonToken);
        public UsingDirectiveSyntax WithAlias(NameEqualsSyntax? alias) => Update(this.GlobalKeyword, this.UsingKeyword, this.StaticKeyword, alias, this.Name, this.SemicolonToken);
        public UsingDirectiveSyntax WithName(NameSyntax name) => Update(this.GlobalKeyword, this.UsingKeyword, this.StaticKeyword, this.Alias, name, this.SemicolonToken);
        public UsingDirectiveSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.GlobalKeyword, this.UsingKeyword, this.StaticKeyword, this.Alias, this.Name, semicolonToken);
    }

    /// <summary>Member declaration syntax.</summary>
    public abstract partial class MemberDeclarationSyntax : CSharpSyntaxNode
    {
        internal MemberDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the attribute declaration list.</summary>
        public abstract SyntaxList<AttributeListSyntax> AttributeLists { get; }
        public MemberDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeListsCore(attributeLists);
        internal abstract MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists);

        public MemberDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => AddAttributeListsCore(items);
        internal abstract MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items);

        /// <summary>Gets the modifier list.</summary>
        public abstract SyntaxTokenList Modifiers { get; }
        public MemberDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => WithModifiersCore(modifiers);
        internal abstract MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers);

        public MemberDeclarationSyntax AddModifiers(params SyntaxToken[] items) => AddModifiersCore(items);
        internal abstract MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.NamespaceDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class NamespaceDeclarationSyntax : MemberDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private NameSyntax? name;
        private SyntaxNode? externs;
        private SyntaxNode? usings;
        private SyntaxNode? members;

        internal NamespaceDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public SyntaxToken NamespaceKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.NamespaceDeclarationSyntax)this.Green).namespaceKeyword, GetChildPosition(2), GetChildIndex(2));

        public NameSyntax Name => GetRed(ref this.name, 3)!;

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.NamespaceDeclarationSyntax)this.Green).openBraceToken, GetChildPosition(4), GetChildIndex(4));

        public SyntaxList<ExternAliasDirectiveSyntax> Externs => new SyntaxList<ExternAliasDirectiveSyntax>(GetRed(ref this.externs, 5));

        public SyntaxList<UsingDirectiveSyntax> Usings => new SyntaxList<UsingDirectiveSyntax>(GetRed(ref this.usings, 6));

        public SyntaxList<MemberDeclarationSyntax> Members => new SyntaxList<MemberDeclarationSyntax>(GetRed(ref this.members, 7));

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.NamespaceDeclarationSyntax)this.Green).closeBraceToken, GetChildPosition(8), GetChildIndex(8));

        /// <summary>Gets the optional semicolon token.</summary>
        public SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.NamespaceDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(9), GetChildIndex(9)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.name, 3)!,
                5 => GetRed(ref this.externs, 5)!,
                6 => GetRed(ref this.usings, 6)!,
                7 => GetRed(ref this.members, 7)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.name,
                5 => this.externs,
                6 => this.usings,
                7 => this.members,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitNamespaceDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitNamespaceDeclaration(this);

        public NamespaceDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken namespaceKeyword, NameSyntax name, SyntaxToken openBraceToken, SyntaxList<ExternAliasDirectiveSyntax> externs, SyntaxList<UsingDirectiveSyntax> usings, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || namespaceKeyword != this.NamespaceKeyword || name != this.Name || openBraceToken != this.OpenBraceToken || externs != this.Externs || usings != this.Usings || members != this.Members || closeBraceToken != this.CloseBraceToken || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.NamespaceDeclaration(attributeLists, modifiers, namespaceKeyword, name, openBraceToken, externs, usings, members, closeBraceToken, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new NamespaceDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.NamespaceKeyword, this.Name, this.OpenBraceToken, this.Externs, this.Usings, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new NamespaceDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.NamespaceKeyword, this.Name, this.OpenBraceToken, this.Externs, this.Usings, this.Members, this.CloseBraceToken, this.SemicolonToken);
        public NamespaceDeclarationSyntax WithNamespaceKeyword(SyntaxToken namespaceKeyword) => Update(this.AttributeLists, this.Modifiers, namespaceKeyword, this.Name, this.OpenBraceToken, this.Externs, this.Usings, this.Members, this.CloseBraceToken, this.SemicolonToken);
        public NamespaceDeclarationSyntax WithName(NameSyntax name) => Update(this.AttributeLists, this.Modifiers, this.NamespaceKeyword, name, this.OpenBraceToken, this.Externs, this.Usings, this.Members, this.CloseBraceToken, this.SemicolonToken);
        public NamespaceDeclarationSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(this.AttributeLists, this.Modifiers, this.NamespaceKeyword, this.Name, openBraceToken, this.Externs, this.Usings, this.Members, this.CloseBraceToken, this.SemicolonToken);
        public NamespaceDeclarationSyntax WithExterns(SyntaxList<ExternAliasDirectiveSyntax> externs) => Update(this.AttributeLists, this.Modifiers, this.NamespaceKeyword, this.Name, this.OpenBraceToken, externs, this.Usings, this.Members, this.CloseBraceToken, this.SemicolonToken);
        public NamespaceDeclarationSyntax WithUsings(SyntaxList<UsingDirectiveSyntax> usings) => Update(this.AttributeLists, this.Modifiers, this.NamespaceKeyword, this.Name, this.OpenBraceToken, this.Externs, usings, this.Members, this.CloseBraceToken, this.SemicolonToken);
        public NamespaceDeclarationSyntax WithMembers(SyntaxList<MemberDeclarationSyntax> members) => Update(this.AttributeLists, this.Modifiers, this.NamespaceKeyword, this.Name, this.OpenBraceToken, this.Externs, this.Usings, members, this.CloseBraceToken, this.SemicolonToken);
        public NamespaceDeclarationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.AttributeLists, this.Modifiers, this.NamespaceKeyword, this.Name, this.OpenBraceToken, this.Externs, this.Usings, this.Members, closeBraceToken, this.SemicolonToken);
        public NamespaceDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.NamespaceKeyword, this.Name, this.OpenBraceToken, this.Externs, this.Usings, this.Members, this.CloseBraceToken, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new NamespaceDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new NamespaceDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        public NamespaceDeclarationSyntax AddExterns(params ExternAliasDirectiveSyntax[] items) => WithExterns(this.Externs.AddRange(items));
        public NamespaceDeclarationSyntax AddUsings(params UsingDirectiveSyntax[] items) => WithUsings(this.Usings.AddRange(items));
        public NamespaceDeclarationSyntax AddMembers(params MemberDeclarationSyntax[] items) => WithMembers(this.Members.AddRange(items));
    }

    /// <summary>Class representing one or more attributes applied to a language construct.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.AttributeList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class AttributeListSyntax : CSharpSyntaxNode
    {
        private AttributeTargetSpecifierSyntax? target;
        private SyntaxNode? attributes;

        internal AttributeListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the open bracket token.</summary>
        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.AttributeListSyntax)this.Green).openBracketToken, Position, 0);

        /// <summary>Gets the optional construct targeted by the attribute.</summary>
        public AttributeTargetSpecifierSyntax? Target => GetRed(ref this.target, 1);

        /// <summary>Gets the attribute declaration list.</summary>
        public SeparatedSyntaxList<AttributeSyntax> Attributes
        {
            get
            {
                var red = GetRed(ref this.attributes, 2);
                return red != null ? new SeparatedSyntaxList<AttributeSyntax>(red, GetChildIndex(2)) : default;
            }
        }

        /// <summary>Gets the close bracket token.</summary>
        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.AttributeListSyntax)this.Green).closeBracketToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.target, 1),
                2 => GetRed(ref this.attributes, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.target,
                2 => this.attributes,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitAttributeList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAttributeList(this);

        public AttributeListSyntax Update(SyntaxToken openBracketToken, AttributeTargetSpecifierSyntax? target, SeparatedSyntaxList<AttributeSyntax> attributes, SyntaxToken closeBracketToken)
        {
            if (openBracketToken != this.OpenBracketToken || target != this.Target || attributes != this.Attributes || closeBracketToken != this.CloseBracketToken)
            {
                var newNode = SyntaxFactory.AttributeList(openBracketToken, target, attributes, closeBracketToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public AttributeListSyntax WithOpenBracketToken(SyntaxToken openBracketToken) => Update(openBracketToken, this.Target, this.Attributes, this.CloseBracketToken);
        public AttributeListSyntax WithTarget(AttributeTargetSpecifierSyntax? target) => Update(this.OpenBracketToken, target, this.Attributes, this.CloseBracketToken);
        public AttributeListSyntax WithAttributes(SeparatedSyntaxList<AttributeSyntax> attributes) => Update(this.OpenBracketToken, this.Target, attributes, this.CloseBracketToken);
        public AttributeListSyntax WithCloseBracketToken(SyntaxToken closeBracketToken) => Update(this.OpenBracketToken, this.Target, this.Attributes, closeBracketToken);

        public AttributeListSyntax AddAttributes(params AttributeSyntax[] items) => WithAttributes(this.Attributes.AddRange(items));
    }

    /// <summary>Class representing what language construct an attribute targets.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.AttributeTargetSpecifier"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class AttributeTargetSpecifierSyntax : CSharpSyntaxNode
    {

        internal AttributeTargetSpecifierSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.AttributeTargetSpecifierSyntax)this.Green).identifier, Position, 0);

        /// <summary>Gets the colon token.</summary>
        public SyntaxToken ColonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.AttributeTargetSpecifierSyntax)this.Green).colonToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitAttributeTargetSpecifier(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAttributeTargetSpecifier(this);

        public AttributeTargetSpecifierSyntax Update(SyntaxToken identifier, SyntaxToken colonToken)
        {
            if (identifier != this.Identifier || colonToken != this.ColonToken)
            {
                var newNode = SyntaxFactory.AttributeTargetSpecifier(identifier, colonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public AttributeTargetSpecifierSyntax WithIdentifier(SyntaxToken identifier) => Update(identifier, this.ColonToken);
        public AttributeTargetSpecifierSyntax WithColonToken(SyntaxToken colonToken) => Update(this.Identifier, colonToken);
    }

    /// <summary>Attribute syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.Attribute"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class AttributeSyntax : CSharpSyntaxNode
    {
        private NameSyntax? name;
        private AttributeArgumentListSyntax? argumentList;

        internal AttributeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the name.</summary>
        public NameSyntax Name => GetRedAtZero(ref this.name)!;

        public AttributeArgumentListSyntax? ArgumentList => GetRed(ref this.argumentList, 1);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.name)!,
                1 => GetRed(ref this.argumentList, 1),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.name,
                1 => this.argumentList,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitAttribute(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAttribute(this);

        public AttributeSyntax Update(NameSyntax name, AttributeArgumentListSyntax? argumentList)
        {
            if (name != this.Name || argumentList != this.ArgumentList)
            {
                var newNode = SyntaxFactory.Attribute(name, argumentList);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public AttributeSyntax WithName(NameSyntax name) => Update(name, this.ArgumentList);
        public AttributeSyntax WithArgumentList(AttributeArgumentListSyntax? argumentList) => Update(this.Name, argumentList);

        public AttributeSyntax AddArgumentListArguments(params AttributeArgumentSyntax[] items)
        {
            var argumentList = this.ArgumentList ?? SyntaxFactory.AttributeArgumentList();
            return WithArgumentList(argumentList.WithArguments(argumentList.Arguments.AddRange(items)));
        }
    }

    /// <summary>Attribute argument list syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.AttributeArgumentList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class AttributeArgumentListSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? arguments;

        internal AttributeArgumentListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the open paren token.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.AttributeArgumentListSyntax)this.Green).openParenToken, Position, 0);

        /// <summary>Gets the arguments syntax list.</summary>
        public SeparatedSyntaxList<AttributeArgumentSyntax> Arguments
        {
            get
            {
                var red = GetRed(ref this.arguments, 1);
                return red != null ? new SeparatedSyntaxList<AttributeArgumentSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the close paren token.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.AttributeArgumentListSyntax)this.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.arguments, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.arguments : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitAttributeArgumentList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAttributeArgumentList(this);

        public AttributeArgumentListSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<AttributeArgumentSyntax> arguments, SyntaxToken closeParenToken)
        {
            if (openParenToken != this.OpenParenToken || arguments != this.Arguments || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.AttributeArgumentList(openParenToken, arguments, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public AttributeArgumentListSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(openParenToken, this.Arguments, this.CloseParenToken);
        public AttributeArgumentListSyntax WithArguments(SeparatedSyntaxList<AttributeArgumentSyntax> arguments) => Update(this.OpenParenToken, arguments, this.CloseParenToken);
        public AttributeArgumentListSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.OpenParenToken, this.Arguments, closeParenToken);

        public AttributeArgumentListSyntax AddArguments(params AttributeArgumentSyntax[] items) => WithArguments(this.Arguments.AddRange(items));
    }

    /// <summary>Attribute argument syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.AttributeArgument"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class AttributeArgumentSyntax : CSharpSyntaxNode
    {
        private NameEqualsSyntax? nameEquals;
        private NameColonSyntax? nameColon;
        private ExpressionSyntax? expression;

        internal AttributeArgumentSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public NameEqualsSyntax? NameEquals => GetRedAtZero(ref this.nameEquals);

        public NameColonSyntax? NameColon => GetRed(ref this.nameColon, 1);

        /// <summary>Gets the expression.</summary>
        public ExpressionSyntax Expression => GetRed(ref this.expression, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.nameEquals),
                1 => GetRed(ref this.nameColon, 1),
                2 => GetRed(ref this.expression, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.nameEquals,
                1 => this.nameColon,
                2 => this.expression,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitAttributeArgument(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAttributeArgument(this);

        public AttributeArgumentSyntax Update(NameEqualsSyntax? nameEquals, NameColonSyntax? nameColon, ExpressionSyntax expression)
        {
            if (nameEquals != this.NameEquals || nameColon != this.NameColon || expression != this.Expression)
            {
                var newNode = SyntaxFactory.AttributeArgument(nameEquals, nameColon, expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public AttributeArgumentSyntax WithNameEquals(NameEqualsSyntax? nameEquals) => Update(nameEquals, this.NameColon, this.Expression);
        public AttributeArgumentSyntax WithNameColon(NameColonSyntax? nameColon) => Update(this.NameEquals, nameColon, this.Expression);
        public AttributeArgumentSyntax WithExpression(ExpressionSyntax expression) => Update(this.NameEquals, this.NameColon, expression);
    }

    /// <summary>Class representing an identifier name followed by an equals token.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.NameEquals"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class NameEqualsSyntax : CSharpSyntaxNode
    {
        private IdentifierNameSyntax? name;

        internal NameEqualsSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the identifier name.</summary>
        public IdentifierNameSyntax Name => GetRedAtZero(ref this.name)!;

        public SyntaxToken EqualsToken => new SyntaxToken(this, ((Syntax.InternalSyntax.NameEqualsSyntax)this.Green).equalsToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.name)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.name : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitNameEquals(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitNameEquals(this);

        public NameEqualsSyntax Update(IdentifierNameSyntax name, SyntaxToken equalsToken)
        {
            if (name != this.Name || equalsToken != this.EqualsToken)
            {
                var newNode = SyntaxFactory.NameEquals(name, equalsToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public NameEqualsSyntax WithName(IdentifierNameSyntax name) => Update(name, this.EqualsToken);
        public NameEqualsSyntax WithEqualsToken(SyntaxToken equalsToken) => Update(this.Name, equalsToken);
    }

    /// <summary>Type parameter list syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.TypeParameterList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class TypeParameterListSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? parameters;

        internal TypeParameterListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the &lt; token.</summary>
        public SyntaxToken LessThanToken => new SyntaxToken(this, ((Syntax.InternalSyntax.TypeParameterListSyntax)this.Green).lessThanToken, Position, 0);

        /// <summary>Gets the parameter list.</summary>
        public SeparatedSyntaxList<TypeParameterSyntax> Parameters
        {
            get
            {
                var red = GetRed(ref this.parameters, 1);
                return red != null ? new SeparatedSyntaxList<TypeParameterSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the &gt; token.</summary>
        public SyntaxToken GreaterThanToken => new SyntaxToken(this, ((Syntax.InternalSyntax.TypeParameterListSyntax)this.Green).greaterThanToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.parameters, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.parameters : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitTypeParameterList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeParameterList(this);

        public TypeParameterListSyntax Update(SyntaxToken lessThanToken, SeparatedSyntaxList<TypeParameterSyntax> parameters, SyntaxToken greaterThanToken)
        {
            if (lessThanToken != this.LessThanToken || parameters != this.Parameters || greaterThanToken != this.GreaterThanToken)
            {
                var newNode = SyntaxFactory.TypeParameterList(lessThanToken, parameters, greaterThanToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public TypeParameterListSyntax WithLessThanToken(SyntaxToken lessThanToken) => Update(lessThanToken, this.Parameters, this.GreaterThanToken);
        public TypeParameterListSyntax WithParameters(SeparatedSyntaxList<TypeParameterSyntax> parameters) => Update(this.LessThanToken, parameters, this.GreaterThanToken);
        public TypeParameterListSyntax WithGreaterThanToken(SyntaxToken greaterThanToken) => Update(this.LessThanToken, this.Parameters, greaterThanToken);

        public TypeParameterListSyntax AddParameters(params TypeParameterSyntax[] items) => WithParameters(this.Parameters.AddRange(items));
    }

    /// <summary>Type parameter syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.TypeParameter"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class TypeParameterSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? attributeLists;

        internal TypeParameterSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the attribute declaration list.</summary>
        public SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public SyntaxToken VarianceKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.TypeParameterSyntax)this.Green).varianceKeyword;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.TypeParameterSyntax)this.Green).identifier, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.attributeLists)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.attributeLists : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitTypeParameter(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeParameter(this);

        public TypeParameterSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken varianceKeyword, SyntaxToken identifier)
        {
            if (attributeLists != this.AttributeLists || varianceKeyword != this.VarianceKeyword || identifier != this.Identifier)
            {
                var newNode = SyntaxFactory.TypeParameter(attributeLists, varianceKeyword, identifier);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public TypeParameterSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.VarianceKeyword, this.Identifier);
        public TypeParameterSyntax WithVarianceKeyword(SyntaxToken varianceKeyword) => Update(this.AttributeLists, varianceKeyword, this.Identifier);
        public TypeParameterSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.VarianceKeyword, identifier);

        public TypeParameterSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
    }

    /// <summary>Base class for type declaration syntax.</summary>
    public abstract partial class BaseTypeDeclarationSyntax : MemberDeclarationSyntax
    {
        internal BaseTypeDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the identifier.</summary>
        public abstract SyntaxToken Identifier { get; }
        public BaseTypeDeclarationSyntax WithIdentifier(SyntaxToken identifier) => WithIdentifierCore(identifier);
        internal abstract BaseTypeDeclarationSyntax WithIdentifierCore(SyntaxToken identifier);

        /// <summary>Gets the base type list.</summary>
        public abstract BaseListSyntax? BaseList { get; }
        public BaseTypeDeclarationSyntax WithBaseList(BaseListSyntax? baseList) => WithBaseListCore(baseList);
        internal abstract BaseTypeDeclarationSyntax WithBaseListCore(BaseListSyntax? baseList);

        public BaseTypeDeclarationSyntax AddBaseListTypes(params BaseTypeSyntax[] items) => AddBaseListTypesCore(items);
        internal abstract BaseTypeDeclarationSyntax AddBaseListTypesCore(params BaseTypeSyntax[] items);

        /// <summary>Gets the open brace token.</summary>
        public abstract SyntaxToken OpenBraceToken { get; }
        public BaseTypeDeclarationSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => WithOpenBraceTokenCore(openBraceToken);
        internal abstract BaseTypeDeclarationSyntax WithOpenBraceTokenCore(SyntaxToken openBraceToken);

        /// <summary>Gets the close brace token.</summary>
        public abstract SyntaxToken CloseBraceToken { get; }
        public BaseTypeDeclarationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => WithCloseBraceTokenCore(closeBraceToken);
        internal abstract BaseTypeDeclarationSyntax WithCloseBraceTokenCore(SyntaxToken closeBraceToken);

        /// <summary>Gets the optional semicolon token.</summary>
        public abstract SyntaxToken SemicolonToken { get; }
        public BaseTypeDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => WithSemicolonTokenCore(semicolonToken);
        internal abstract BaseTypeDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken);

        public new BaseTypeDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => (BaseTypeDeclarationSyntax)WithAttributeListsCore(attributeLists);
        public new BaseTypeDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => (BaseTypeDeclarationSyntax)WithModifiersCore(modifiers);

        public new BaseTypeDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => (BaseTypeDeclarationSyntax)AddAttributeListsCore(items);

        public new BaseTypeDeclarationSyntax AddModifiers(params SyntaxToken[] items) => (BaseTypeDeclarationSyntax)AddModifiersCore(items);
    }

    /// <summary>Base class for type declaration syntax (class, struct, interface, record).</summary>
    public abstract partial class TypeDeclarationSyntax : BaseTypeDeclarationSyntax
    {
        internal TypeDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the type keyword token ("class", "struct", "interface", "record").</summary>
        public abstract SyntaxToken Keyword { get; }
        public TypeDeclarationSyntax WithKeyword(SyntaxToken keyword) => WithKeywordCore(keyword);
        internal abstract TypeDeclarationSyntax WithKeywordCore(SyntaxToken keyword);

        public abstract TypeParameterListSyntax? TypeParameterList { get; }
        public TypeDeclarationSyntax WithTypeParameterList(TypeParameterListSyntax? typeParameterList) => WithTypeParameterListCore(typeParameterList);
        internal abstract TypeDeclarationSyntax WithTypeParameterListCore(TypeParameterListSyntax? typeParameterList);

        public TypeDeclarationSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items) => AddTypeParameterListParametersCore(items);
        internal abstract TypeDeclarationSyntax AddTypeParameterListParametersCore(params TypeParameterSyntax[] items);

        /// <summary>Gets the type constraint list.</summary>
        public abstract SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses { get; }
        public TypeDeclarationSyntax WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => WithConstraintClausesCore(constraintClauses);
        internal abstract TypeDeclarationSyntax WithConstraintClausesCore(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses);

        public TypeDeclarationSyntax AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items) => AddConstraintClausesCore(items);
        internal abstract TypeDeclarationSyntax AddConstraintClausesCore(params TypeParameterConstraintClauseSyntax[] items);

        /// <summary>Gets the member declarations.</summary>
        public abstract SyntaxList<MemberDeclarationSyntax> Members { get; }
        public TypeDeclarationSyntax WithMembers(SyntaxList<MemberDeclarationSyntax> members) => WithMembersCore(members);
        internal abstract TypeDeclarationSyntax WithMembersCore(SyntaxList<MemberDeclarationSyntax> members);

        public TypeDeclarationSyntax AddMembers(params MemberDeclarationSyntax[] items) => AddMembersCore(items);
        internal abstract TypeDeclarationSyntax AddMembersCore(params MemberDeclarationSyntax[] items);

        public new TypeDeclarationSyntax WithIdentifier(SyntaxToken identifier) => (TypeDeclarationSyntax)WithIdentifierCore(identifier);
        public new TypeDeclarationSyntax WithBaseList(BaseListSyntax? baseList) => (TypeDeclarationSyntax)WithBaseListCore(baseList);
        public new TypeDeclarationSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => (TypeDeclarationSyntax)WithOpenBraceTokenCore(openBraceToken);
        public new TypeDeclarationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => (TypeDeclarationSyntax)WithCloseBraceTokenCore(closeBraceToken);
        public new TypeDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => (TypeDeclarationSyntax)WithSemicolonTokenCore(semicolonToken);

        public new BaseTypeDeclarationSyntax AddBaseListTypes(params BaseTypeSyntax[] items) => AddBaseListTypesCore(items);
    }

    /// <summary>Class type declaration syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ClassDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ClassDeclarationSyntax : TypeDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeParameterListSyntax? typeParameterList;
        private BaseListSyntax? baseList;
        private SyntaxNode? constraintClauses;
        private SyntaxNode? members;

        internal ClassDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the class keyword token.</summary>
        public override SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ClassDeclarationSyntax)this.Green).keyword, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.ClassDeclarationSyntax)this.Green).identifier, GetChildPosition(3), GetChildIndex(3));

        public override TypeParameterListSyntax? TypeParameterList => GetRed(ref this.typeParameterList, 4);

        public override BaseListSyntax? BaseList => GetRed(ref this.baseList, 5);

        public override SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new SyntaxList<TypeParameterConstraintClauseSyntax>(GetRed(ref this.constraintClauses, 6));

        public override SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ClassDeclarationSyntax)this.Green).openBraceToken, GetChildPosition(7), GetChildIndex(7));

        public override SyntaxList<MemberDeclarationSyntax> Members => new SyntaxList<MemberDeclarationSyntax>(GetRed(ref this.members, 8));

        public override SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ClassDeclarationSyntax)this.Green).closeBraceToken, GetChildPosition(9), GetChildIndex(9));

        public override SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.ClassDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(10), GetChildIndex(10)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                4 => GetRed(ref this.typeParameterList, 4),
                5 => GetRed(ref this.baseList, 5),
                6 => GetRed(ref this.constraintClauses, 6)!,
                8 => GetRed(ref this.members, 8)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                4 => this.typeParameterList,
                5 => this.baseList,
                6 => this.constraintClauses,
                8 => this.members,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitClassDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitClassDeclaration(this);

        public ClassDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || keyword != this.Keyword || identifier != this.Identifier || typeParameterList != this.TypeParameterList || baseList != this.BaseList || constraintClauses != this.ConstraintClauses || openBraceToken != this.OpenBraceToken || members != this.Members || closeBraceToken != this.CloseBraceToken || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.ClassDeclaration(attributeLists, modifiers, keyword, identifier, typeParameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new ClassDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new ClassDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithKeywordCore(SyntaxToken keyword) => WithKeyword(keyword);
        public new ClassDeclarationSyntax WithKeyword(SyntaxToken keyword) => Update(this.AttributeLists, this.Modifiers, keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithIdentifierCore(SyntaxToken identifier) => WithIdentifier(identifier);
        public new ClassDeclarationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, this.Keyword, identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithTypeParameterListCore(TypeParameterListSyntax? typeParameterList) => WithTypeParameterList(typeParameterList);
        public new ClassDeclarationSyntax WithTypeParameterList(TypeParameterListSyntax? typeParameterList) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, typeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithBaseListCore(BaseListSyntax? baseList) => WithBaseList(baseList);
        public new ClassDeclarationSyntax WithBaseList(BaseListSyntax? baseList) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, baseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithConstraintClausesCore(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => WithConstraintClauses(constraintClauses);
        public new ClassDeclarationSyntax WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, constraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithOpenBraceTokenCore(SyntaxToken openBraceToken) => WithOpenBraceToken(openBraceToken);
        public new ClassDeclarationSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, openBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithMembersCore(SyntaxList<MemberDeclarationSyntax> members) => WithMembers(members);
        public new ClassDeclarationSyntax WithMembers(SyntaxList<MemberDeclarationSyntax> members) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithCloseBraceTokenCore(SyntaxToken closeBraceToken) => WithCloseBraceToken(closeBraceToken);
        public new ClassDeclarationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, closeBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken) => WithSemicolonToken(semicolonToken);
        public new ClassDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new ClassDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new ClassDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        internal override TypeDeclarationSyntax AddTypeParameterListParametersCore(params TypeParameterSyntax[] items) => AddTypeParameterListParameters(items);
        public new ClassDeclarationSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
        {
            var typeParameterList = this.TypeParameterList ?? SyntaxFactory.TypeParameterList();
            return WithTypeParameterList(typeParameterList.WithParameters(typeParameterList.Parameters.AddRange(items)));
        }
        internal override BaseTypeDeclarationSyntax AddBaseListTypesCore(params BaseTypeSyntax[] items) => AddBaseListTypes(items);
        public new ClassDeclarationSyntax AddBaseListTypes(params BaseTypeSyntax[] items)
        {
            var baseList = this.BaseList ?? SyntaxFactory.BaseList();
            return WithBaseList(baseList.WithTypes(baseList.Types.AddRange(items)));
        }
        internal override TypeDeclarationSyntax AddConstraintClausesCore(params TypeParameterConstraintClauseSyntax[] items) => AddConstraintClauses(items);
        public new ClassDeclarationSyntax AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items) => WithConstraintClauses(this.ConstraintClauses.AddRange(items));
        internal override TypeDeclarationSyntax AddMembersCore(params MemberDeclarationSyntax[] items) => AddMembers(items);
        public new ClassDeclarationSyntax AddMembers(params MemberDeclarationSyntax[] items) => WithMembers(this.Members.AddRange(items));
    }

    /// <summary>Struct type declaration syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.StructDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class StructDeclarationSyntax : TypeDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeParameterListSyntax? typeParameterList;
        private BaseListSyntax? baseList;
        private SyntaxNode? constraintClauses;
        private SyntaxNode? members;

        internal StructDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the struct keyword token.</summary>
        public override SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.StructDeclarationSyntax)this.Green).keyword, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.StructDeclarationSyntax)this.Green).identifier, GetChildPosition(3), GetChildIndex(3));

        public override TypeParameterListSyntax? TypeParameterList => GetRed(ref this.typeParameterList, 4);

        public override BaseListSyntax? BaseList => GetRed(ref this.baseList, 5);

        public override SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new SyntaxList<TypeParameterConstraintClauseSyntax>(GetRed(ref this.constraintClauses, 6));

        public override SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.StructDeclarationSyntax)this.Green).openBraceToken, GetChildPosition(7), GetChildIndex(7));

        public override SyntaxList<MemberDeclarationSyntax> Members => new SyntaxList<MemberDeclarationSyntax>(GetRed(ref this.members, 8));

        public override SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.StructDeclarationSyntax)this.Green).closeBraceToken, GetChildPosition(9), GetChildIndex(9));

        public override SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.StructDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(10), GetChildIndex(10)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                4 => GetRed(ref this.typeParameterList, 4),
                5 => GetRed(ref this.baseList, 5),
                6 => GetRed(ref this.constraintClauses, 6)!,
                8 => GetRed(ref this.members, 8)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                4 => this.typeParameterList,
                5 => this.baseList,
                6 => this.constraintClauses,
                8 => this.members,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitStructDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitStructDeclaration(this);

        public StructDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || keyword != this.Keyword || identifier != this.Identifier || typeParameterList != this.TypeParameterList || baseList != this.BaseList || constraintClauses != this.ConstraintClauses || openBraceToken != this.OpenBraceToken || members != this.Members || closeBraceToken != this.CloseBraceToken || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.StructDeclaration(attributeLists, modifiers, keyword, identifier, typeParameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new StructDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new StructDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithKeywordCore(SyntaxToken keyword) => WithKeyword(keyword);
        public new StructDeclarationSyntax WithKeyword(SyntaxToken keyword) => Update(this.AttributeLists, this.Modifiers, keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithIdentifierCore(SyntaxToken identifier) => WithIdentifier(identifier);
        public new StructDeclarationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, this.Keyword, identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithTypeParameterListCore(TypeParameterListSyntax? typeParameterList) => WithTypeParameterList(typeParameterList);
        public new StructDeclarationSyntax WithTypeParameterList(TypeParameterListSyntax? typeParameterList) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, typeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithBaseListCore(BaseListSyntax? baseList) => WithBaseList(baseList);
        public new StructDeclarationSyntax WithBaseList(BaseListSyntax? baseList) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, baseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithConstraintClausesCore(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => WithConstraintClauses(constraintClauses);
        public new StructDeclarationSyntax WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, constraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithOpenBraceTokenCore(SyntaxToken openBraceToken) => WithOpenBraceToken(openBraceToken);
        public new StructDeclarationSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, openBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithMembersCore(SyntaxList<MemberDeclarationSyntax> members) => WithMembers(members);
        public new StructDeclarationSyntax WithMembers(SyntaxList<MemberDeclarationSyntax> members) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithCloseBraceTokenCore(SyntaxToken closeBraceToken) => WithCloseBraceToken(closeBraceToken);
        public new StructDeclarationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, closeBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken) => WithSemicolonToken(semicolonToken);
        public new StructDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new StructDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new StructDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        internal override TypeDeclarationSyntax AddTypeParameterListParametersCore(params TypeParameterSyntax[] items) => AddTypeParameterListParameters(items);
        public new StructDeclarationSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
        {
            var typeParameterList = this.TypeParameterList ?? SyntaxFactory.TypeParameterList();
            return WithTypeParameterList(typeParameterList.WithParameters(typeParameterList.Parameters.AddRange(items)));
        }
        internal override BaseTypeDeclarationSyntax AddBaseListTypesCore(params BaseTypeSyntax[] items) => AddBaseListTypes(items);
        public new StructDeclarationSyntax AddBaseListTypes(params BaseTypeSyntax[] items)
        {
            var baseList = this.BaseList ?? SyntaxFactory.BaseList();
            return WithBaseList(baseList.WithTypes(baseList.Types.AddRange(items)));
        }
        internal override TypeDeclarationSyntax AddConstraintClausesCore(params TypeParameterConstraintClauseSyntax[] items) => AddConstraintClauses(items);
        public new StructDeclarationSyntax AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items) => WithConstraintClauses(this.ConstraintClauses.AddRange(items));
        internal override TypeDeclarationSyntax AddMembersCore(params MemberDeclarationSyntax[] items) => AddMembers(items);
        public new StructDeclarationSyntax AddMembers(params MemberDeclarationSyntax[] items) => WithMembers(this.Members.AddRange(items));
    }

    /// <summary>Interface type declaration syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.InterfaceDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class InterfaceDeclarationSyntax : TypeDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeParameterListSyntax? typeParameterList;
        private BaseListSyntax? baseList;
        private SyntaxNode? constraintClauses;
        private SyntaxNode? members;

        internal InterfaceDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the interface keyword token.</summary>
        public override SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.InterfaceDeclarationSyntax)this.Green).keyword, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.InterfaceDeclarationSyntax)this.Green).identifier, GetChildPosition(3), GetChildIndex(3));

        public override TypeParameterListSyntax? TypeParameterList => GetRed(ref this.typeParameterList, 4);

        public override BaseListSyntax? BaseList => GetRed(ref this.baseList, 5);

        public override SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new SyntaxList<TypeParameterConstraintClauseSyntax>(GetRed(ref this.constraintClauses, 6));

        public override SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.InterfaceDeclarationSyntax)this.Green).openBraceToken, GetChildPosition(7), GetChildIndex(7));

        public override SyntaxList<MemberDeclarationSyntax> Members => new SyntaxList<MemberDeclarationSyntax>(GetRed(ref this.members, 8));

        public override SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.InterfaceDeclarationSyntax)this.Green).closeBraceToken, GetChildPosition(9), GetChildIndex(9));

        public override SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.InterfaceDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(10), GetChildIndex(10)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                4 => GetRed(ref this.typeParameterList, 4),
                5 => GetRed(ref this.baseList, 5),
                6 => GetRed(ref this.constraintClauses, 6)!,
                8 => GetRed(ref this.members, 8)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                4 => this.typeParameterList,
                5 => this.baseList,
                6 => this.constraintClauses,
                8 => this.members,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitInterfaceDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitInterfaceDeclaration(this);

        public InterfaceDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || keyword != this.Keyword || identifier != this.Identifier || typeParameterList != this.TypeParameterList || baseList != this.BaseList || constraintClauses != this.ConstraintClauses || openBraceToken != this.OpenBraceToken || members != this.Members || closeBraceToken != this.CloseBraceToken || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.InterfaceDeclaration(attributeLists, modifiers, keyword, identifier, typeParameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new InterfaceDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new InterfaceDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithKeywordCore(SyntaxToken keyword) => WithKeyword(keyword);
        public new InterfaceDeclarationSyntax WithKeyword(SyntaxToken keyword) => Update(this.AttributeLists, this.Modifiers, keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithIdentifierCore(SyntaxToken identifier) => WithIdentifier(identifier);
        public new InterfaceDeclarationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, this.Keyword, identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithTypeParameterListCore(TypeParameterListSyntax? typeParameterList) => WithTypeParameterList(typeParameterList);
        public new InterfaceDeclarationSyntax WithTypeParameterList(TypeParameterListSyntax? typeParameterList) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, typeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithBaseListCore(BaseListSyntax? baseList) => WithBaseList(baseList);
        public new InterfaceDeclarationSyntax WithBaseList(BaseListSyntax? baseList) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, baseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithConstraintClausesCore(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => WithConstraintClauses(constraintClauses);
        public new InterfaceDeclarationSyntax WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, constraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithOpenBraceTokenCore(SyntaxToken openBraceToken) => WithOpenBraceToken(openBraceToken);
        public new InterfaceDeclarationSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, openBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithMembersCore(SyntaxList<MemberDeclarationSyntax> members) => WithMembers(members);
        public new InterfaceDeclarationSyntax WithMembers(SyntaxList<MemberDeclarationSyntax> members) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithCloseBraceTokenCore(SyntaxToken closeBraceToken) => WithCloseBraceToken(closeBraceToken);
        public new InterfaceDeclarationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, closeBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken) => WithSemicolonToken(semicolonToken);
        public new InterfaceDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Identifier, this.TypeParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new InterfaceDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new InterfaceDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        internal override TypeDeclarationSyntax AddTypeParameterListParametersCore(params TypeParameterSyntax[] items) => AddTypeParameterListParameters(items);
        public new InterfaceDeclarationSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
        {
            var typeParameterList = this.TypeParameterList ?? SyntaxFactory.TypeParameterList();
            return WithTypeParameterList(typeParameterList.WithParameters(typeParameterList.Parameters.AddRange(items)));
        }
        internal override BaseTypeDeclarationSyntax AddBaseListTypesCore(params BaseTypeSyntax[] items) => AddBaseListTypes(items);
        public new InterfaceDeclarationSyntax AddBaseListTypes(params BaseTypeSyntax[] items)
        {
            var baseList = this.BaseList ?? SyntaxFactory.BaseList();
            return WithBaseList(baseList.WithTypes(baseList.Types.AddRange(items)));
        }
        internal override TypeDeclarationSyntax AddConstraintClausesCore(params TypeParameterConstraintClauseSyntax[] items) => AddConstraintClauses(items);
        public new InterfaceDeclarationSyntax AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items) => WithConstraintClauses(this.ConstraintClauses.AddRange(items));
        internal override TypeDeclarationSyntax AddMembersCore(params MemberDeclarationSyntax[] items) => AddMembers(items);
        public new InterfaceDeclarationSyntax AddMembers(params MemberDeclarationSyntax[] items) => WithMembers(this.Members.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.RecordDeclaration"/></description></item>
    /// <item><description><see cref="SyntaxKind.RecordStructDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class RecordDeclarationSyntax : TypeDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeParameterListSyntax? typeParameterList;
        private ParameterListSyntax? parameterList;
        private BaseListSyntax? baseList;
        private SyntaxNode? constraintClauses;
        private SyntaxNode? members;

        internal RecordDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public override SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.RecordDeclarationSyntax)this.Green).keyword, GetChildPosition(2), GetChildIndex(2));

        public SyntaxToken ClassOrStructKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.RecordDeclarationSyntax)this.Green).classOrStructKeyword;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(3), GetChildIndex(3)) : default;
            }
        }

        public override SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.RecordDeclarationSyntax)this.Green).identifier, GetChildPosition(4), GetChildIndex(4));

        public override TypeParameterListSyntax? TypeParameterList => GetRed(ref this.typeParameterList, 5);

        public ParameterListSyntax? ParameterList => GetRed(ref this.parameterList, 6);

        public override BaseListSyntax? BaseList => GetRed(ref this.baseList, 7);

        public override SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new SyntaxList<TypeParameterConstraintClauseSyntax>(GetRed(ref this.constraintClauses, 8));

        public override SyntaxToken OpenBraceToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.RecordDeclarationSyntax)this.Green).openBraceToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(9), GetChildIndex(9)) : default;
            }
        }

        public override SyntaxList<MemberDeclarationSyntax> Members => new SyntaxList<MemberDeclarationSyntax>(GetRed(ref this.members, 10));

        public override SyntaxToken CloseBraceToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.RecordDeclarationSyntax)this.Green).closeBraceToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(11), GetChildIndex(11)) : default;
            }
        }

        public override SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.RecordDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(12), GetChildIndex(12)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                5 => GetRed(ref this.typeParameterList, 5),
                6 => GetRed(ref this.parameterList, 6),
                7 => GetRed(ref this.baseList, 7),
                8 => GetRed(ref this.constraintClauses, 8)!,
                10 => GetRed(ref this.members, 10)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                5 => this.typeParameterList,
                6 => this.parameterList,
                7 => this.baseList,
                8 => this.constraintClauses,
                10 => this.members,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitRecordDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRecordDeclaration(this);

        public RecordDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken classOrStructKeyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax? parameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || keyword != this.Keyword || classOrStructKeyword != this.ClassOrStructKeyword || identifier != this.Identifier || typeParameterList != this.TypeParameterList || parameterList != this.ParameterList || baseList != this.BaseList || constraintClauses != this.ConstraintClauses || openBraceToken != this.OpenBraceToken || members != this.Members || closeBraceToken != this.CloseBraceToken || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.RecordDeclaration(this.Kind(), attributeLists, modifiers, keyword, classOrStructKeyword, identifier, typeParameterList, parameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new RecordDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Keyword, this.ClassOrStructKeyword, this.Identifier, this.TypeParameterList, this.ParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new RecordDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Keyword, this.ClassOrStructKeyword, this.Identifier, this.TypeParameterList, this.ParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithKeywordCore(SyntaxToken keyword) => WithKeyword(keyword);
        public new RecordDeclarationSyntax WithKeyword(SyntaxToken keyword) => Update(this.AttributeLists, this.Modifiers, keyword, this.ClassOrStructKeyword, this.Identifier, this.TypeParameterList, this.ParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        public RecordDeclarationSyntax WithClassOrStructKeyword(SyntaxToken classOrStructKeyword) => Update(this.AttributeLists, this.Modifiers, this.Keyword, classOrStructKeyword, this.Identifier, this.TypeParameterList, this.ParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithIdentifierCore(SyntaxToken identifier) => WithIdentifier(identifier);
        public new RecordDeclarationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.ClassOrStructKeyword, identifier, this.TypeParameterList, this.ParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithTypeParameterListCore(TypeParameterListSyntax? typeParameterList) => WithTypeParameterList(typeParameterList);
        public new RecordDeclarationSyntax WithTypeParameterList(TypeParameterListSyntax? typeParameterList) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.ClassOrStructKeyword, this.Identifier, typeParameterList, this.ParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        public RecordDeclarationSyntax WithParameterList(ParameterListSyntax? parameterList) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.ClassOrStructKeyword, this.Identifier, this.TypeParameterList, parameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithBaseListCore(BaseListSyntax? baseList) => WithBaseList(baseList);
        public new RecordDeclarationSyntax WithBaseList(BaseListSyntax? baseList) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.ClassOrStructKeyword, this.Identifier, this.TypeParameterList, this.ParameterList, baseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithConstraintClausesCore(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => WithConstraintClauses(constraintClauses);
        public new RecordDeclarationSyntax WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.ClassOrStructKeyword, this.Identifier, this.TypeParameterList, this.ParameterList, this.BaseList, constraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithOpenBraceTokenCore(SyntaxToken openBraceToken) => WithOpenBraceToken(openBraceToken);
        public new RecordDeclarationSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.ClassOrStructKeyword, this.Identifier, this.TypeParameterList, this.ParameterList, this.BaseList, this.ConstraintClauses, openBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override TypeDeclarationSyntax WithMembersCore(SyntaxList<MemberDeclarationSyntax> members) => WithMembers(members);
        public new RecordDeclarationSyntax WithMembers(SyntaxList<MemberDeclarationSyntax> members) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.ClassOrStructKeyword, this.Identifier, this.TypeParameterList, this.ParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithCloseBraceTokenCore(SyntaxToken closeBraceToken) => WithCloseBraceToken(closeBraceToken);
        public new RecordDeclarationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.ClassOrStructKeyword, this.Identifier, this.TypeParameterList, this.ParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, closeBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken) => WithSemicolonToken(semicolonToken);
        public new RecordDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.ClassOrStructKeyword, this.Identifier, this.TypeParameterList, this.ParameterList, this.BaseList, this.ConstraintClauses, this.OpenBraceToken, this.Members, this.CloseBraceToken, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new RecordDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new RecordDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        internal override TypeDeclarationSyntax AddTypeParameterListParametersCore(params TypeParameterSyntax[] items) => AddTypeParameterListParameters(items);
        public new RecordDeclarationSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
        {
            var typeParameterList = this.TypeParameterList ?? SyntaxFactory.TypeParameterList();
            return WithTypeParameterList(typeParameterList.WithParameters(typeParameterList.Parameters.AddRange(items)));
        }
        public RecordDeclarationSyntax AddParameterListParameters(params ParameterSyntax[] items)
        {
            var parameterList = this.ParameterList ?? SyntaxFactory.ParameterList();
            return WithParameterList(parameterList.WithParameters(parameterList.Parameters.AddRange(items)));
        }
        internal override BaseTypeDeclarationSyntax AddBaseListTypesCore(params BaseTypeSyntax[] items) => AddBaseListTypes(items);
        public new RecordDeclarationSyntax AddBaseListTypes(params BaseTypeSyntax[] items)
        {
            var baseList = this.BaseList ?? SyntaxFactory.BaseList();
            return WithBaseList(baseList.WithTypes(baseList.Types.AddRange(items)));
        }
        internal override TypeDeclarationSyntax AddConstraintClausesCore(params TypeParameterConstraintClauseSyntax[] items) => AddConstraintClauses(items);
        public new RecordDeclarationSyntax AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items) => WithConstraintClauses(this.ConstraintClauses.AddRange(items));
        internal override TypeDeclarationSyntax AddMembersCore(params MemberDeclarationSyntax[] items) => AddMembers(items);
        public new RecordDeclarationSyntax AddMembers(params MemberDeclarationSyntax[] items) => WithMembers(this.Members.AddRange(items));
    }

    /// <summary>Enum type declaration syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.EnumDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class EnumDeclarationSyntax : BaseTypeDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private BaseListSyntax? baseList;
        private SyntaxNode? members;

        internal EnumDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the enum keyword token.</summary>
        public SyntaxToken EnumKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.EnumDeclarationSyntax)this.Green).enumKeyword, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.EnumDeclarationSyntax)this.Green).identifier, GetChildPosition(3), GetChildIndex(3));

        public override BaseListSyntax? BaseList => GetRed(ref this.baseList, 4);

        public override SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.EnumDeclarationSyntax)this.Green).openBraceToken, GetChildPosition(5), GetChildIndex(5));

        /// <summary>Gets the members declaration list.</summary>
        public SeparatedSyntaxList<EnumMemberDeclarationSyntax> Members
        {
            get
            {
                var red = GetRed(ref this.members, 6);
                return red != null ? new SeparatedSyntaxList<EnumMemberDeclarationSyntax>(red, GetChildIndex(6)) : default;
            }
        }

        public override SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.EnumDeclarationSyntax)this.Green).closeBraceToken, GetChildPosition(7), GetChildIndex(7));

        /// <summary>Gets the optional semicolon token.</summary>
        public override SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.EnumDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(8), GetChildIndex(8)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                4 => GetRed(ref this.baseList, 4),
                6 => GetRed(ref this.members, 6)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                4 => this.baseList,
                6 => this.members,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitEnumDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEnumDeclaration(this);

        public EnumDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken enumKeyword, SyntaxToken identifier, BaseListSyntax? baseList, SyntaxToken openBraceToken, SeparatedSyntaxList<EnumMemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || enumKeyword != this.EnumKeyword || identifier != this.Identifier || baseList != this.BaseList || openBraceToken != this.OpenBraceToken || members != this.Members || closeBraceToken != this.CloseBraceToken || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.EnumDeclaration(attributeLists, modifiers, enumKeyword, identifier, baseList, openBraceToken, members, closeBraceToken, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new EnumDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.EnumKeyword, this.Identifier, this.BaseList, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new EnumDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.EnumKeyword, this.Identifier, this.BaseList, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        public EnumDeclarationSyntax WithEnumKeyword(SyntaxToken enumKeyword) => Update(this.AttributeLists, this.Modifiers, enumKeyword, this.Identifier, this.BaseList, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithIdentifierCore(SyntaxToken identifier) => WithIdentifier(identifier);
        public new EnumDeclarationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, this.EnumKeyword, identifier, this.BaseList, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithBaseListCore(BaseListSyntax? baseList) => WithBaseList(baseList);
        public new EnumDeclarationSyntax WithBaseList(BaseListSyntax? baseList) => Update(this.AttributeLists, this.Modifiers, this.EnumKeyword, this.Identifier, baseList, this.OpenBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithOpenBraceTokenCore(SyntaxToken openBraceToken) => WithOpenBraceToken(openBraceToken);
        public new EnumDeclarationSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(this.AttributeLists, this.Modifiers, this.EnumKeyword, this.Identifier, this.BaseList, openBraceToken, this.Members, this.CloseBraceToken, this.SemicolonToken);
        public EnumDeclarationSyntax WithMembers(SeparatedSyntaxList<EnumMemberDeclarationSyntax> members) => Update(this.AttributeLists, this.Modifiers, this.EnumKeyword, this.Identifier, this.BaseList, this.OpenBraceToken, members, this.CloseBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithCloseBraceTokenCore(SyntaxToken closeBraceToken) => WithCloseBraceToken(closeBraceToken);
        public new EnumDeclarationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.AttributeLists, this.Modifiers, this.EnumKeyword, this.Identifier, this.BaseList, this.OpenBraceToken, this.Members, closeBraceToken, this.SemicolonToken);
        internal override BaseTypeDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken) => WithSemicolonToken(semicolonToken);
        public new EnumDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.EnumKeyword, this.Identifier, this.BaseList, this.OpenBraceToken, this.Members, this.CloseBraceToken, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new EnumDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new EnumDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        internal override BaseTypeDeclarationSyntax AddBaseListTypesCore(params BaseTypeSyntax[] items) => AddBaseListTypes(items);
        public new EnumDeclarationSyntax AddBaseListTypes(params BaseTypeSyntax[] items)
        {
            var baseList = this.BaseList ?? SyntaxFactory.BaseList();
            return WithBaseList(baseList.WithTypes(baseList.Types.AddRange(items)));
        }
        public EnumDeclarationSyntax AddMembers(params EnumMemberDeclarationSyntax[] items) => WithMembers(this.Members.AddRange(items));
    }

    /// <summary>Delegate declaration syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.DelegateDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class DelegateDeclarationSyntax : MemberDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeSyntax? returnType;
        private TypeParameterListSyntax? typeParameterList;
        private ParameterListSyntax? parameterList;
        private SyntaxNode? constraintClauses;

        internal DelegateDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the "delegate" keyword.</summary>
        public SyntaxToken DelegateKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.DelegateDeclarationSyntax)this.Green).delegateKeyword, GetChildPosition(2), GetChildIndex(2));

        /// <summary>Gets the return type.</summary>
        public TypeSyntax ReturnType => GetRed(ref this.returnType, 3)!;

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.DelegateDeclarationSyntax)this.Green).identifier, GetChildPosition(4), GetChildIndex(4));

        public TypeParameterListSyntax? TypeParameterList => GetRed(ref this.typeParameterList, 5);

        /// <summary>Gets the parameter list.</summary>
        public ParameterListSyntax ParameterList => GetRed(ref this.parameterList, 6)!;

        /// <summary>Gets the constraint clause list.</summary>
        public SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new SyntaxList<TypeParameterConstraintClauseSyntax>(GetRed(ref this.constraintClauses, 7));

        /// <summary>Gets the semicolon token.</summary>
        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.DelegateDeclarationSyntax)this.Green).semicolonToken, GetChildPosition(8), GetChildIndex(8));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.returnType, 3)!,
                5 => GetRed(ref this.typeParameterList, 5),
                6 => GetRed(ref this.parameterList, 6)!,
                7 => GetRed(ref this.constraintClauses, 7)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.returnType,
                5 => this.typeParameterList,
                6 => this.parameterList,
                7 => this.constraintClauses,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitDelegateDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDelegateDeclaration(this);

        public DelegateDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken delegateKeyword, TypeSyntax returnType, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || delegateKeyword != this.DelegateKeyword || returnType != this.ReturnType || identifier != this.Identifier || typeParameterList != this.TypeParameterList || parameterList != this.ParameterList || constraintClauses != this.ConstraintClauses || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.DelegateDeclaration(attributeLists, modifiers, delegateKeyword, returnType, identifier, typeParameterList, parameterList, constraintClauses, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new DelegateDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.DelegateKeyword, this.ReturnType, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new DelegateDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.DelegateKeyword, this.ReturnType, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.SemicolonToken);
        public DelegateDeclarationSyntax WithDelegateKeyword(SyntaxToken delegateKeyword) => Update(this.AttributeLists, this.Modifiers, delegateKeyword, this.ReturnType, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.SemicolonToken);
        public DelegateDeclarationSyntax WithReturnType(TypeSyntax returnType) => Update(this.AttributeLists, this.Modifiers, this.DelegateKeyword, returnType, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.SemicolonToken);
        public DelegateDeclarationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, this.DelegateKeyword, this.ReturnType, identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.SemicolonToken);
        public DelegateDeclarationSyntax WithTypeParameterList(TypeParameterListSyntax? typeParameterList) => Update(this.AttributeLists, this.Modifiers, this.DelegateKeyword, this.ReturnType, this.Identifier, typeParameterList, this.ParameterList, this.ConstraintClauses, this.SemicolonToken);
        public DelegateDeclarationSyntax WithParameterList(ParameterListSyntax parameterList) => Update(this.AttributeLists, this.Modifiers, this.DelegateKeyword, this.ReturnType, this.Identifier, this.TypeParameterList, parameterList, this.ConstraintClauses, this.SemicolonToken);
        public DelegateDeclarationSyntax WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => Update(this.AttributeLists, this.Modifiers, this.DelegateKeyword, this.ReturnType, this.Identifier, this.TypeParameterList, this.ParameterList, constraintClauses, this.SemicolonToken);
        public DelegateDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.DelegateKeyword, this.ReturnType, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new DelegateDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new DelegateDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        public DelegateDeclarationSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
        {
            var typeParameterList = this.TypeParameterList ?? SyntaxFactory.TypeParameterList();
            return WithTypeParameterList(typeParameterList.WithParameters(typeParameterList.Parameters.AddRange(items)));
        }
        public DelegateDeclarationSyntax AddParameterListParameters(params ParameterSyntax[] items) => WithParameterList(this.ParameterList.WithParameters(this.ParameterList.Parameters.AddRange(items)));
        public DelegateDeclarationSyntax AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items) => WithConstraintClauses(this.ConstraintClauses.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.EnumMemberDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class EnumMemberDeclarationSyntax : MemberDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private EqualsValueClauseSyntax? equalsValue;

        internal EnumMemberDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.EnumMemberDeclarationSyntax)this.Green).identifier, GetChildPosition(2), GetChildIndex(2));

        public EqualsValueClauseSyntax? EqualsValue => GetRed(ref this.equalsValue, 3);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.equalsValue, 3),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.equalsValue,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitEnumMemberDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEnumMemberDeclaration(this);

        public EnumMemberDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, EqualsValueClauseSyntax? equalsValue)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || identifier != this.Identifier || equalsValue != this.EqualsValue)
            {
                var newNode = SyntaxFactory.EnumMemberDeclaration(attributeLists, modifiers, identifier, equalsValue);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new EnumMemberDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Identifier, this.EqualsValue);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new EnumMemberDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Identifier, this.EqualsValue);
        public EnumMemberDeclarationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, identifier, this.EqualsValue);
        public EnumMemberDeclarationSyntax WithEqualsValue(EqualsValueClauseSyntax? equalsValue) => Update(this.AttributeLists, this.Modifiers, this.Identifier, equalsValue);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new EnumMemberDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new EnumMemberDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
    }

    /// <summary>Base list syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.BaseList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class BaseListSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? types;

        internal BaseListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the colon token.</summary>
        public SyntaxToken ColonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.BaseListSyntax)this.Green).colonToken, Position, 0);

        /// <summary>Gets the base type references.</summary>
        public SeparatedSyntaxList<BaseTypeSyntax> Types
        {
            get
            {
                var red = GetRed(ref this.types, 1);
                return red != null ? new SeparatedSyntaxList<BaseTypeSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.types, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.types : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitBaseList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBaseList(this);

        public BaseListSyntax Update(SyntaxToken colonToken, SeparatedSyntaxList<BaseTypeSyntax> types)
        {
            if (colonToken != this.ColonToken || types != this.Types)
            {
                var newNode = SyntaxFactory.BaseList(colonToken, types);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public BaseListSyntax WithColonToken(SyntaxToken colonToken) => Update(colonToken, this.Types);
        public BaseListSyntax WithTypes(SeparatedSyntaxList<BaseTypeSyntax> types) => Update(this.ColonToken, types);

        public BaseListSyntax AddTypes(params BaseTypeSyntax[] items) => WithTypes(this.Types.AddRange(items));
    }

    /// <summary>Provides the base class from which the classes that represent base type syntax nodes are derived. This is an abstract class.</summary>
    public abstract partial class BaseTypeSyntax : CSharpSyntaxNode
    {
        internal BaseTypeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public abstract TypeSyntax Type { get; }
        public BaseTypeSyntax WithType(TypeSyntax type) => WithTypeCore(type);
        internal abstract BaseTypeSyntax WithTypeCore(TypeSyntax type);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.SimpleBaseType"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class SimpleBaseTypeSyntax : BaseTypeSyntax
    {
        private TypeSyntax? type;

        internal SimpleBaseTypeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override TypeSyntax Type => GetRedAtZero(ref this.type)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.type)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.type : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitSimpleBaseType(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSimpleBaseType(this);

        public SimpleBaseTypeSyntax Update(TypeSyntax type)
        {
            if (type != this.Type)
            {
                var newNode = SyntaxFactory.SimpleBaseType(type);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override BaseTypeSyntax WithTypeCore(TypeSyntax type) => WithType(type);
        public new SimpleBaseTypeSyntax WithType(TypeSyntax type) => Update(type);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.PrimaryConstructorBaseType"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class PrimaryConstructorBaseTypeSyntax : BaseTypeSyntax
    {
        private TypeSyntax? type;
        private ArgumentListSyntax? argumentList;

        internal PrimaryConstructorBaseTypeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override TypeSyntax Type => GetRedAtZero(ref this.type)!;

        public ArgumentListSyntax ArgumentList => GetRed(ref this.argumentList, 1)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.type)!,
                1 => GetRed(ref this.argumentList, 1)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.type,
                1 => this.argumentList,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitPrimaryConstructorBaseType(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPrimaryConstructorBaseType(this);

        public PrimaryConstructorBaseTypeSyntax Update(TypeSyntax type, ArgumentListSyntax argumentList)
        {
            if (type != this.Type || argumentList != this.ArgumentList)
            {
                var newNode = SyntaxFactory.PrimaryConstructorBaseType(type, argumentList);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override BaseTypeSyntax WithTypeCore(TypeSyntax type) => WithType(type);
        public new PrimaryConstructorBaseTypeSyntax WithType(TypeSyntax type) => Update(type, this.ArgumentList);
        public PrimaryConstructorBaseTypeSyntax WithArgumentList(ArgumentListSyntax argumentList) => Update(this.Type, argumentList);

        public PrimaryConstructorBaseTypeSyntax AddArgumentListArguments(params ArgumentSyntax[] items) => WithArgumentList(this.ArgumentList.WithArguments(this.ArgumentList.Arguments.AddRange(items)));
    }

    /// <summary>Type parameter constraint clause.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.TypeParameterConstraintClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class TypeParameterConstraintClauseSyntax : CSharpSyntaxNode
    {
        private IdentifierNameSyntax? name;
        private SyntaxNode? constraints;

        internal TypeParameterConstraintClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken WhereKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax)this.Green).whereKeyword, Position, 0);

        /// <summary>Gets the identifier.</summary>
        public IdentifierNameSyntax Name => GetRed(ref this.name, 1)!;

        /// <summary>Gets the colon token.</summary>
        public SyntaxToken ColonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax)this.Green).colonToken, GetChildPosition(2), GetChildIndex(2));

        /// <summary>Gets the constraints list.</summary>
        public SeparatedSyntaxList<TypeParameterConstraintSyntax> Constraints
        {
            get
            {
                var red = GetRed(ref this.constraints, 3);
                return red != null ? new SeparatedSyntaxList<TypeParameterConstraintSyntax>(red, GetChildIndex(3)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.name, 1)!,
                3 => GetRed(ref this.constraints, 3)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.name,
                3 => this.constraints,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitTypeParameterConstraintClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeParameterConstraintClause(this);

        public TypeParameterConstraintClauseSyntax Update(SyntaxToken whereKeyword, IdentifierNameSyntax name, SyntaxToken colonToken, SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints)
        {
            if (whereKeyword != this.WhereKeyword || name != this.Name || colonToken != this.ColonToken || constraints != this.Constraints)
            {
                var newNode = SyntaxFactory.TypeParameterConstraintClause(whereKeyword, name, colonToken, constraints);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public TypeParameterConstraintClauseSyntax WithWhereKeyword(SyntaxToken whereKeyword) => Update(whereKeyword, this.Name, this.ColonToken, this.Constraints);
        public TypeParameterConstraintClauseSyntax WithName(IdentifierNameSyntax name) => Update(this.WhereKeyword, name, this.ColonToken, this.Constraints);
        public TypeParameterConstraintClauseSyntax WithColonToken(SyntaxToken colonToken) => Update(this.WhereKeyword, this.Name, colonToken, this.Constraints);
        public TypeParameterConstraintClauseSyntax WithConstraints(SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints) => Update(this.WhereKeyword, this.Name, this.ColonToken, constraints);

        public TypeParameterConstraintClauseSyntax AddConstraints(params TypeParameterConstraintSyntax[] items) => WithConstraints(this.Constraints.AddRange(items));
    }

    /// <summary>Base type for type parameter constraint syntax.</summary>
    public abstract partial class TypeParameterConstraintSyntax : CSharpSyntaxNode
    {
        internal TypeParameterConstraintSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    /// <summary>Constructor constraint syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ConstructorConstraint"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ConstructorConstraintSyntax : TypeParameterConstraintSyntax
    {

        internal ConstructorConstraintSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the "new" keyword.</summary>
        public SyntaxToken NewKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ConstructorConstraintSyntax)this.Green).newKeyword, Position, 0);

        /// <summary>Gets the open paren keyword.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ConstructorConstraintSyntax)this.Green).openParenToken, GetChildPosition(1), GetChildIndex(1));

        /// <summary>Gets the close paren keyword.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ConstructorConstraintSyntax)this.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitConstructorConstraint(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitConstructorConstraint(this);

        public ConstructorConstraintSyntax Update(SyntaxToken newKeyword, SyntaxToken openParenToken, SyntaxToken closeParenToken)
        {
            if (newKeyword != this.NewKeyword || openParenToken != this.OpenParenToken || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.ConstructorConstraint(newKeyword, openParenToken, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ConstructorConstraintSyntax WithNewKeyword(SyntaxToken newKeyword) => Update(newKeyword, this.OpenParenToken, this.CloseParenToken);
        public ConstructorConstraintSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(this.NewKeyword, openParenToken, this.CloseParenToken);
        public ConstructorConstraintSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.NewKeyword, this.OpenParenToken, closeParenToken);
    }

    /// <summary>Class or struct constraint syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ClassConstraint"/></description></item>
    /// <item><description><see cref="SyntaxKind.StructConstraint"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ClassOrStructConstraintSyntax : TypeParameterConstraintSyntax
    {

        internal ClassOrStructConstraintSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the constraint keyword ("class" or "struct").</summary>
        public SyntaxToken ClassOrStructKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ClassOrStructConstraintSyntax)this.Green).classOrStructKeyword, Position, 0);

        /// <summary>SyntaxToken representing the question mark.</summary>
        public SyntaxToken QuestionToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.ClassOrStructConstraintSyntax)this.Green).questionToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitClassOrStructConstraint(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitClassOrStructConstraint(this);

        public ClassOrStructConstraintSyntax Update(SyntaxToken classOrStructKeyword, SyntaxToken questionToken)
        {
            if (classOrStructKeyword != this.ClassOrStructKeyword || questionToken != this.QuestionToken)
            {
                var newNode = SyntaxFactory.ClassOrStructConstraint(this.Kind(), classOrStructKeyword, questionToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ClassOrStructConstraintSyntax WithClassOrStructKeyword(SyntaxToken classOrStructKeyword) => Update(classOrStructKeyword, this.QuestionToken);
        public ClassOrStructConstraintSyntax WithQuestionToken(SyntaxToken questionToken) => Update(this.ClassOrStructKeyword, questionToken);
    }

    /// <summary>Type constraint syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.TypeConstraint"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class TypeConstraintSyntax : TypeParameterConstraintSyntax
    {
        private TypeSyntax? type;

        internal TypeConstraintSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the type syntax.</summary>
        public TypeSyntax Type => GetRedAtZero(ref this.type)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.type)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.type : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitTypeConstraint(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeConstraint(this);

        public TypeConstraintSyntax Update(TypeSyntax type)
        {
            if (type != this.Type)
            {
                var newNode = SyntaxFactory.TypeConstraint(type);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public TypeConstraintSyntax WithType(TypeSyntax type) => Update(type);
    }

    /// <summary>Default constraint syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.DefaultConstraint"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class DefaultConstraintSyntax : TypeParameterConstraintSyntax
    {

        internal DefaultConstraintSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the "default" keyword.</summary>
        public SyntaxToken DefaultKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.DefaultConstraintSyntax)this.Green).defaultKeyword, Position, 0);

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitDefaultConstraint(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDefaultConstraint(this);

        public DefaultConstraintSyntax Update(SyntaxToken defaultKeyword)
        {
            if (defaultKeyword != this.DefaultKeyword)
            {
                var newNode = SyntaxFactory.DefaultConstraint(defaultKeyword);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public DefaultConstraintSyntax WithDefaultKeyword(SyntaxToken defaultKeyword) => Update(defaultKeyword);
    }

    public abstract partial class BaseFieldDeclarationSyntax : MemberDeclarationSyntax
    {
        internal BaseFieldDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public abstract VariableDeclarationSyntax Declaration { get; }
        public BaseFieldDeclarationSyntax WithDeclaration(VariableDeclarationSyntax declaration) => WithDeclarationCore(declaration);
        internal abstract BaseFieldDeclarationSyntax WithDeclarationCore(VariableDeclarationSyntax declaration);

        public BaseFieldDeclarationSyntax AddDeclarationVariables(params VariableDeclaratorSyntax[] items) => AddDeclarationVariablesCore(items);
        internal abstract BaseFieldDeclarationSyntax AddDeclarationVariablesCore(params VariableDeclaratorSyntax[] items);

        public abstract SyntaxToken SemicolonToken { get; }
        public BaseFieldDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => WithSemicolonTokenCore(semicolonToken);
        internal abstract BaseFieldDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken);

        public new BaseFieldDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => (BaseFieldDeclarationSyntax)WithAttributeListsCore(attributeLists);
        public new BaseFieldDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => (BaseFieldDeclarationSyntax)WithModifiersCore(modifiers);

        public new BaseFieldDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => (BaseFieldDeclarationSyntax)AddAttributeListsCore(items);

        public new BaseFieldDeclarationSyntax AddModifiers(params SyntaxToken[] items) => (BaseFieldDeclarationSyntax)AddModifiersCore(items);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.FieldDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class FieldDeclarationSyntax : BaseFieldDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private VariableDeclarationSyntax? declaration;

        internal FieldDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public override VariableDeclarationSyntax Declaration => GetRed(ref this.declaration, 2)!;

        public override SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.FieldDeclarationSyntax)this.Green).semicolonToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.declaration, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.declaration,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitFieldDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFieldDeclaration(this);

        public FieldDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || declaration != this.Declaration || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.FieldDeclaration(attributeLists, modifiers, declaration, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new FieldDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Declaration, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new FieldDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Declaration, this.SemicolonToken);
        internal override BaseFieldDeclarationSyntax WithDeclarationCore(VariableDeclarationSyntax declaration) => WithDeclaration(declaration);
        public new FieldDeclarationSyntax WithDeclaration(VariableDeclarationSyntax declaration) => Update(this.AttributeLists, this.Modifiers, declaration, this.SemicolonToken);
        internal override BaseFieldDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken) => WithSemicolonToken(semicolonToken);
        public new FieldDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.Declaration, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new FieldDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new FieldDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        internal override BaseFieldDeclarationSyntax AddDeclarationVariablesCore(params VariableDeclaratorSyntax[] items) => AddDeclarationVariables(items);
        public new FieldDeclarationSyntax AddDeclarationVariables(params VariableDeclaratorSyntax[] items) => WithDeclaration(this.Declaration.WithVariables(this.Declaration.Variables.AddRange(items)));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.EventFieldDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class EventFieldDeclarationSyntax : BaseFieldDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private VariableDeclarationSyntax? declaration;

        internal EventFieldDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public SyntaxToken EventKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.EventFieldDeclarationSyntax)this.Green).eventKeyword, GetChildPosition(2), GetChildIndex(2));

        public override VariableDeclarationSyntax Declaration => GetRed(ref this.declaration, 3)!;

        public override SyntaxToken SemicolonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.EventFieldDeclarationSyntax)this.Green).semicolonToken, GetChildPosition(4), GetChildIndex(4));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.declaration, 3)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.declaration,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitEventFieldDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEventFieldDeclaration(this);

        public EventFieldDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken eventKeyword, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || eventKeyword != this.EventKeyword || declaration != this.Declaration || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.EventFieldDeclaration(attributeLists, modifiers, eventKeyword, declaration, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new EventFieldDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.EventKeyword, this.Declaration, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new EventFieldDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.EventKeyword, this.Declaration, this.SemicolonToken);
        public EventFieldDeclarationSyntax WithEventKeyword(SyntaxToken eventKeyword) => Update(this.AttributeLists, this.Modifiers, eventKeyword, this.Declaration, this.SemicolonToken);
        internal override BaseFieldDeclarationSyntax WithDeclarationCore(VariableDeclarationSyntax declaration) => WithDeclaration(declaration);
        public new EventFieldDeclarationSyntax WithDeclaration(VariableDeclarationSyntax declaration) => Update(this.AttributeLists, this.Modifiers, this.EventKeyword, declaration, this.SemicolonToken);
        internal override BaseFieldDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken) => WithSemicolonToken(semicolonToken);
        public new EventFieldDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.EventKeyword, this.Declaration, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new EventFieldDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new EventFieldDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        internal override BaseFieldDeclarationSyntax AddDeclarationVariablesCore(params VariableDeclaratorSyntax[] items) => AddDeclarationVariables(items);
        public new EventFieldDeclarationSyntax AddDeclarationVariables(params VariableDeclaratorSyntax[] items) => WithDeclaration(this.Declaration.WithVariables(this.Declaration.Variables.AddRange(items)));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ExplicitInterfaceSpecifier"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ExplicitInterfaceSpecifierSyntax : CSharpSyntaxNode
    {
        private NameSyntax? name;

        internal ExplicitInterfaceSpecifierSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public NameSyntax Name => GetRedAtZero(ref this.name)!;

        public SyntaxToken DotToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ExplicitInterfaceSpecifierSyntax)this.Green).dotToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.name)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.name : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitExplicitInterfaceSpecifier(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitExplicitInterfaceSpecifier(this);

        public ExplicitInterfaceSpecifierSyntax Update(NameSyntax name, SyntaxToken dotToken)
        {
            if (name != this.Name || dotToken != this.DotToken)
            {
                var newNode = SyntaxFactory.ExplicitInterfaceSpecifier(name, dotToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ExplicitInterfaceSpecifierSyntax WithName(NameSyntax name) => Update(name, this.DotToken);
        public ExplicitInterfaceSpecifierSyntax WithDotToken(SyntaxToken dotToken) => Update(this.Name, dotToken);
    }

    /// <summary>Base type for method declaration syntax.</summary>
    public abstract partial class BaseMethodDeclarationSyntax : MemberDeclarationSyntax
    {
        internal BaseMethodDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the parameter list.</summary>
        public abstract ParameterListSyntax ParameterList { get; }
        public BaseMethodDeclarationSyntax WithParameterList(ParameterListSyntax parameterList) => WithParameterListCore(parameterList);
        internal abstract BaseMethodDeclarationSyntax WithParameterListCore(ParameterListSyntax parameterList);

        public BaseMethodDeclarationSyntax AddParameterListParameters(params ParameterSyntax[] items) => AddParameterListParametersCore(items);
        internal abstract BaseMethodDeclarationSyntax AddParameterListParametersCore(params ParameterSyntax[] items);

        public abstract BlockSyntax? Body { get; }
        public BaseMethodDeclarationSyntax WithBody(BlockSyntax? body) => WithBodyCore(body);
        internal abstract BaseMethodDeclarationSyntax WithBodyCore(BlockSyntax? body);

        public BaseMethodDeclarationSyntax AddBodyAttributeLists(params AttributeListSyntax[] items) => AddBodyAttributeListsCore(items);
        internal abstract BaseMethodDeclarationSyntax AddBodyAttributeListsCore(params AttributeListSyntax[] items);

        public BaseMethodDeclarationSyntax AddBodyStatements(params StatementSyntax[] items) => AddBodyStatementsCore(items);
        internal abstract BaseMethodDeclarationSyntax AddBodyStatementsCore(params StatementSyntax[] items);

        public abstract ArrowExpressionClauseSyntax? ExpressionBody { get; }
        public BaseMethodDeclarationSyntax WithExpressionBody(ArrowExpressionClauseSyntax? expressionBody) => WithExpressionBodyCore(expressionBody);
        internal abstract BaseMethodDeclarationSyntax WithExpressionBodyCore(ArrowExpressionClauseSyntax? expressionBody);

        /// <summary>Gets the optional semicolon token.</summary>
        public abstract SyntaxToken SemicolonToken { get; }
        public BaseMethodDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => WithSemicolonTokenCore(semicolonToken);
        internal abstract BaseMethodDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken);

        public new BaseMethodDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => (BaseMethodDeclarationSyntax)WithAttributeListsCore(attributeLists);
        public new BaseMethodDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => (BaseMethodDeclarationSyntax)WithModifiersCore(modifiers);

        public new BaseMethodDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => (BaseMethodDeclarationSyntax)AddAttributeListsCore(items);

        public new BaseMethodDeclarationSyntax AddModifiers(params SyntaxToken[] items) => (BaseMethodDeclarationSyntax)AddModifiersCore(items);
    }

    /// <summary>Method declaration syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.MethodDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class MethodDeclarationSyntax : BaseMethodDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeSyntax? returnType;
        private ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier;
        private TypeParameterListSyntax? typeParameterList;
        private ParameterListSyntax? parameterList;
        private SyntaxNode? constraintClauses;
        private BlockSyntax? body;
        private ArrowExpressionClauseSyntax? expressionBody;

        internal MethodDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the return type syntax.</summary>
        public TypeSyntax ReturnType => GetRed(ref this.returnType, 2)!;

        public ExplicitInterfaceSpecifierSyntax? ExplicitInterfaceSpecifier => GetRed(ref this.explicitInterfaceSpecifier, 3);

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.MethodDeclarationSyntax)this.Green).identifier, GetChildPosition(4), GetChildIndex(4));

        public TypeParameterListSyntax? TypeParameterList => GetRed(ref this.typeParameterList, 5);

        public override ParameterListSyntax ParameterList => GetRed(ref this.parameterList, 6)!;

        /// <summary>Gets the constraint clause list.</summary>
        public SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new SyntaxList<TypeParameterConstraintClauseSyntax>(GetRed(ref this.constraintClauses, 7));

        public override BlockSyntax? Body => GetRed(ref this.body, 8);

        public override ArrowExpressionClauseSyntax? ExpressionBody => GetRed(ref this.expressionBody, 9);

        /// <summary>Gets the optional semicolon token.</summary>
        public override SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.MethodDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(10), GetChildIndex(10)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.returnType, 2)!,
                3 => GetRed(ref this.explicitInterfaceSpecifier, 3),
                5 => GetRed(ref this.typeParameterList, 5),
                6 => GetRed(ref this.parameterList, 6)!,
                7 => GetRed(ref this.constraintClauses, 7)!,
                8 => GetRed(ref this.body, 8),
                9 => GetRed(ref this.expressionBody, 9),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.returnType,
                3 => this.explicitInterfaceSpecifier,
                5 => this.typeParameterList,
                6 => this.parameterList,
                7 => this.constraintClauses,
                8 => this.body,
                9 => this.expressionBody,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitMethodDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitMethodDeclaration(this);

        public MethodDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax returnType, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || returnType != this.ReturnType || explicitInterfaceSpecifier != this.ExplicitInterfaceSpecifier || identifier != this.Identifier || typeParameterList != this.TypeParameterList || parameterList != this.ParameterList || constraintClauses != this.ConstraintClauses || body != this.Body || expressionBody != this.ExpressionBody || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.MethodDeclaration(attributeLists, modifiers, returnType, explicitInterfaceSpecifier, identifier, typeParameterList, parameterList, constraintClauses, body, expressionBody, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new MethodDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.ReturnType, this.ExplicitInterfaceSpecifier, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new MethodDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.ReturnType, this.ExplicitInterfaceSpecifier, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        public MethodDeclarationSyntax WithReturnType(TypeSyntax returnType) => Update(this.AttributeLists, this.Modifiers, returnType, this.ExplicitInterfaceSpecifier, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        public MethodDeclarationSyntax WithExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, explicitInterfaceSpecifier, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        public MethodDeclarationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.ExplicitInterfaceSpecifier, identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        public MethodDeclarationSyntax WithTypeParameterList(TypeParameterListSyntax? typeParameterList) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.ExplicitInterfaceSpecifier, this.Identifier, typeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithParameterListCore(ParameterListSyntax parameterList) => WithParameterList(parameterList);
        public new MethodDeclarationSyntax WithParameterList(ParameterListSyntax parameterList) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.ExplicitInterfaceSpecifier, this.Identifier, this.TypeParameterList, parameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        public MethodDeclarationSyntax WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.ExplicitInterfaceSpecifier, this.Identifier, this.TypeParameterList, this.ParameterList, constraintClauses, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithBodyCore(BlockSyntax? body) => WithBody(body);
        public new MethodDeclarationSyntax WithBody(BlockSyntax? body) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.ExplicitInterfaceSpecifier, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithExpressionBodyCore(ArrowExpressionClauseSyntax? expressionBody) => WithExpressionBody(expressionBody);
        public new MethodDeclarationSyntax WithExpressionBody(ArrowExpressionClauseSyntax? expressionBody) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.ExplicitInterfaceSpecifier, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, expressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken) => WithSemicolonToken(semicolonToken);
        public new MethodDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.ExplicitInterfaceSpecifier, this.Identifier, this.TypeParameterList, this.ParameterList, this.ConstraintClauses, this.Body, this.ExpressionBody, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new MethodDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new MethodDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        public MethodDeclarationSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
        {
            var typeParameterList = this.TypeParameterList ?? SyntaxFactory.TypeParameterList();
            return WithTypeParameterList(typeParameterList.WithParameters(typeParameterList.Parameters.AddRange(items)));
        }
        internal override BaseMethodDeclarationSyntax AddParameterListParametersCore(params ParameterSyntax[] items) => AddParameterListParameters(items);
        public new MethodDeclarationSyntax AddParameterListParameters(params ParameterSyntax[] items) => WithParameterList(this.ParameterList.WithParameters(this.ParameterList.Parameters.AddRange(items)));
        public MethodDeclarationSyntax AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items) => WithConstraintClauses(this.ConstraintClauses.AddRange(items));
        internal override BaseMethodDeclarationSyntax AddBodyAttributeListsCore(params AttributeListSyntax[] items) => AddBodyAttributeLists(items);
        public new MethodDeclarationSyntax AddBodyAttributeLists(params AttributeListSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithAttributeLists(body.AttributeLists.AddRange(items)));
        }
        internal override BaseMethodDeclarationSyntax AddBodyStatementsCore(params StatementSyntax[] items) => AddBodyStatements(items);
        public new MethodDeclarationSyntax AddBodyStatements(params StatementSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithStatements(body.Statements.AddRange(items)));
        }
    }

    /// <summary>Operator declaration syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.OperatorDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class OperatorDeclarationSyntax : BaseMethodDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeSyntax? returnType;
        private ParameterListSyntax? parameterList;
        private BlockSyntax? body;
        private ArrowExpressionClauseSyntax? expressionBody;

        internal OperatorDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the return type.</summary>
        public TypeSyntax ReturnType => GetRed(ref this.returnType, 2)!;

        /// <summary>Gets the "operator" keyword.</summary>
        public SyntaxToken OperatorKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.OperatorDeclarationSyntax)this.Green).operatorKeyword, GetChildPosition(3), GetChildIndex(3));

        /// <summary>Gets the operator token.</summary>
        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Syntax.InternalSyntax.OperatorDeclarationSyntax)this.Green).operatorToken, GetChildPosition(4), GetChildIndex(4));

        public override ParameterListSyntax ParameterList => GetRed(ref this.parameterList, 5)!;

        public override BlockSyntax? Body => GetRed(ref this.body, 6);

        public override ArrowExpressionClauseSyntax? ExpressionBody => GetRed(ref this.expressionBody, 7);

        /// <summary>Gets the optional semicolon token.</summary>
        public override SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.OperatorDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(8), GetChildIndex(8)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.returnType, 2)!,
                5 => GetRed(ref this.parameterList, 5)!,
                6 => GetRed(ref this.body, 6),
                7 => GetRed(ref this.expressionBody, 7),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.returnType,
                5 => this.parameterList,
                6 => this.body,
                7 => this.expressionBody,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitOperatorDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitOperatorDeclaration(this);

        public OperatorDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax returnType, SyntaxToken operatorKeyword, SyntaxToken operatorToken, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || returnType != this.ReturnType || operatorKeyword != this.OperatorKeyword || operatorToken != this.OperatorToken || parameterList != this.ParameterList || body != this.Body || expressionBody != this.ExpressionBody || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.OperatorDeclaration(attributeLists, modifiers, returnType, operatorKeyword, operatorToken, parameterList, body, expressionBody, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new OperatorDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.ReturnType, this.OperatorKeyword, this.OperatorToken, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new OperatorDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.ReturnType, this.OperatorKeyword, this.OperatorToken, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        public OperatorDeclarationSyntax WithReturnType(TypeSyntax returnType) => Update(this.AttributeLists, this.Modifiers, returnType, this.OperatorKeyword, this.OperatorToken, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        public OperatorDeclarationSyntax WithOperatorKeyword(SyntaxToken operatorKeyword) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, operatorKeyword, this.OperatorToken, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        public OperatorDeclarationSyntax WithOperatorToken(SyntaxToken operatorToken) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.OperatorKeyword, operatorToken, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithParameterListCore(ParameterListSyntax parameterList) => WithParameterList(parameterList);
        public new OperatorDeclarationSyntax WithParameterList(ParameterListSyntax parameterList) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.OperatorKeyword, this.OperatorToken, parameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithBodyCore(BlockSyntax? body) => WithBody(body);
        public new OperatorDeclarationSyntax WithBody(BlockSyntax? body) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.OperatorKeyword, this.OperatorToken, this.ParameterList, body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithExpressionBodyCore(ArrowExpressionClauseSyntax? expressionBody) => WithExpressionBody(expressionBody);
        public new OperatorDeclarationSyntax WithExpressionBody(ArrowExpressionClauseSyntax? expressionBody) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.OperatorKeyword, this.OperatorToken, this.ParameterList, this.Body, expressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken) => WithSemicolonToken(semicolonToken);
        public new OperatorDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.ReturnType, this.OperatorKeyword, this.OperatorToken, this.ParameterList, this.Body, this.ExpressionBody, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new OperatorDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new OperatorDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        internal override BaseMethodDeclarationSyntax AddParameterListParametersCore(params ParameterSyntax[] items) => AddParameterListParameters(items);
        public new OperatorDeclarationSyntax AddParameterListParameters(params ParameterSyntax[] items) => WithParameterList(this.ParameterList.WithParameters(this.ParameterList.Parameters.AddRange(items)));
        internal override BaseMethodDeclarationSyntax AddBodyAttributeListsCore(params AttributeListSyntax[] items) => AddBodyAttributeLists(items);
        public new OperatorDeclarationSyntax AddBodyAttributeLists(params AttributeListSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithAttributeLists(body.AttributeLists.AddRange(items)));
        }
        internal override BaseMethodDeclarationSyntax AddBodyStatementsCore(params StatementSyntax[] items) => AddBodyStatements(items);
        public new OperatorDeclarationSyntax AddBodyStatements(params StatementSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithStatements(body.Statements.AddRange(items)));
        }
    }

    /// <summary>Conversion operator declaration syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ConversionOperatorDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ConversionOperatorDeclarationSyntax : BaseMethodDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeSyntax? type;
        private ParameterListSyntax? parameterList;
        private BlockSyntax? body;
        private ArrowExpressionClauseSyntax? expressionBody;

        internal ConversionOperatorDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the "implicit" or "explicit" token.</summary>
        public SyntaxToken ImplicitOrExplicitKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ConversionOperatorDeclarationSyntax)this.Green).implicitOrExplicitKeyword, GetChildPosition(2), GetChildIndex(2));

        /// <summary>Gets the "operator" token.</summary>
        public SyntaxToken OperatorKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ConversionOperatorDeclarationSyntax)this.Green).operatorKeyword, GetChildPosition(3), GetChildIndex(3));

        /// <summary>Gets the type.</summary>
        public TypeSyntax Type => GetRed(ref this.type, 4)!;

        public override ParameterListSyntax ParameterList => GetRed(ref this.parameterList, 5)!;

        public override BlockSyntax? Body => GetRed(ref this.body, 6);

        public override ArrowExpressionClauseSyntax? ExpressionBody => GetRed(ref this.expressionBody, 7);

        /// <summary>Gets the optional semicolon token.</summary>
        public override SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.ConversionOperatorDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(8), GetChildIndex(8)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                4 => GetRed(ref this.type, 4)!,
                5 => GetRed(ref this.parameterList, 5)!,
                6 => GetRed(ref this.body, 6),
                7 => GetRed(ref this.expressionBody, 7),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                4 => this.type,
                5 => this.parameterList,
                6 => this.body,
                7 => this.expressionBody,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitConversionOperatorDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitConversionOperatorDeclaration(this);

        public ConversionOperatorDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, TypeSyntax type, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || implicitOrExplicitKeyword != this.ImplicitOrExplicitKeyword || operatorKeyword != this.OperatorKeyword || type != this.Type || parameterList != this.ParameterList || body != this.Body || expressionBody != this.ExpressionBody || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.ConversionOperatorDeclaration(attributeLists, modifiers, implicitOrExplicitKeyword, operatorKeyword, type, parameterList, body, expressionBody, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new ConversionOperatorDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.ImplicitOrExplicitKeyword, this.OperatorKeyword, this.Type, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new ConversionOperatorDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.ImplicitOrExplicitKeyword, this.OperatorKeyword, this.Type, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        public ConversionOperatorDeclarationSyntax WithImplicitOrExplicitKeyword(SyntaxToken implicitOrExplicitKeyword) => Update(this.AttributeLists, this.Modifiers, implicitOrExplicitKeyword, this.OperatorKeyword, this.Type, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        public ConversionOperatorDeclarationSyntax WithOperatorKeyword(SyntaxToken operatorKeyword) => Update(this.AttributeLists, this.Modifiers, this.ImplicitOrExplicitKeyword, operatorKeyword, this.Type, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        public ConversionOperatorDeclarationSyntax WithType(TypeSyntax type) => Update(this.AttributeLists, this.Modifiers, this.ImplicitOrExplicitKeyword, this.OperatorKeyword, type, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithParameterListCore(ParameterListSyntax parameterList) => WithParameterList(parameterList);
        public new ConversionOperatorDeclarationSyntax WithParameterList(ParameterListSyntax parameterList) => Update(this.AttributeLists, this.Modifiers, this.ImplicitOrExplicitKeyword, this.OperatorKeyword, this.Type, parameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithBodyCore(BlockSyntax? body) => WithBody(body);
        public new ConversionOperatorDeclarationSyntax WithBody(BlockSyntax? body) => Update(this.AttributeLists, this.Modifiers, this.ImplicitOrExplicitKeyword, this.OperatorKeyword, this.Type, this.ParameterList, body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithExpressionBodyCore(ArrowExpressionClauseSyntax? expressionBody) => WithExpressionBody(expressionBody);
        public new ConversionOperatorDeclarationSyntax WithExpressionBody(ArrowExpressionClauseSyntax? expressionBody) => Update(this.AttributeLists, this.Modifiers, this.ImplicitOrExplicitKeyword, this.OperatorKeyword, this.Type, this.ParameterList, this.Body, expressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken) => WithSemicolonToken(semicolonToken);
        public new ConversionOperatorDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.ImplicitOrExplicitKeyword, this.OperatorKeyword, this.Type, this.ParameterList, this.Body, this.ExpressionBody, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new ConversionOperatorDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new ConversionOperatorDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        internal override BaseMethodDeclarationSyntax AddParameterListParametersCore(params ParameterSyntax[] items) => AddParameterListParameters(items);
        public new ConversionOperatorDeclarationSyntax AddParameterListParameters(params ParameterSyntax[] items) => WithParameterList(this.ParameterList.WithParameters(this.ParameterList.Parameters.AddRange(items)));
        internal override BaseMethodDeclarationSyntax AddBodyAttributeListsCore(params AttributeListSyntax[] items) => AddBodyAttributeLists(items);
        public new ConversionOperatorDeclarationSyntax AddBodyAttributeLists(params AttributeListSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithAttributeLists(body.AttributeLists.AddRange(items)));
        }
        internal override BaseMethodDeclarationSyntax AddBodyStatementsCore(params StatementSyntax[] items) => AddBodyStatements(items);
        public new ConversionOperatorDeclarationSyntax AddBodyStatements(params StatementSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithStatements(body.Statements.AddRange(items)));
        }
    }

    /// <summary>Constructor declaration syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ConstructorDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ConstructorDeclarationSyntax : BaseMethodDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private ParameterListSyntax? parameterList;
        private ConstructorInitializerSyntax? initializer;
        private BlockSyntax? body;
        private ArrowExpressionClauseSyntax? expressionBody;

        internal ConstructorDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.ConstructorDeclarationSyntax)this.Green).identifier, GetChildPosition(2), GetChildIndex(2));

        public override ParameterListSyntax ParameterList => GetRed(ref this.parameterList, 3)!;

        public ConstructorInitializerSyntax? Initializer => GetRed(ref this.initializer, 4);

        public override BlockSyntax? Body => GetRed(ref this.body, 5);

        public override ArrowExpressionClauseSyntax? ExpressionBody => GetRed(ref this.expressionBody, 6);

        /// <summary>Gets the optional semicolon token.</summary>
        public override SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.ConstructorDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(7), GetChildIndex(7)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.parameterList, 3)!,
                4 => GetRed(ref this.initializer, 4),
                5 => GetRed(ref this.body, 5),
                6 => GetRed(ref this.expressionBody, 6),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.parameterList,
                4 => this.initializer,
                5 => this.body,
                6 => this.expressionBody,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitConstructorDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitConstructorDeclaration(this);

        public ConstructorDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, ParameterListSyntax parameterList, ConstructorInitializerSyntax? initializer, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || identifier != this.Identifier || parameterList != this.ParameterList || initializer != this.Initializer || body != this.Body || expressionBody != this.ExpressionBody || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.ConstructorDeclaration(attributeLists, modifiers, identifier, parameterList, initializer, body, expressionBody, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new ConstructorDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Identifier, this.ParameterList, this.Initializer, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new ConstructorDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Identifier, this.ParameterList, this.Initializer, this.Body, this.ExpressionBody, this.SemicolonToken);
        public ConstructorDeclarationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, identifier, this.ParameterList, this.Initializer, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithParameterListCore(ParameterListSyntax parameterList) => WithParameterList(parameterList);
        public new ConstructorDeclarationSyntax WithParameterList(ParameterListSyntax parameterList) => Update(this.AttributeLists, this.Modifiers, this.Identifier, parameterList, this.Initializer, this.Body, this.ExpressionBody, this.SemicolonToken);
        public ConstructorDeclarationSyntax WithInitializer(ConstructorInitializerSyntax? initializer) => Update(this.AttributeLists, this.Modifiers, this.Identifier, this.ParameterList, initializer, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithBodyCore(BlockSyntax? body) => WithBody(body);
        public new ConstructorDeclarationSyntax WithBody(BlockSyntax? body) => Update(this.AttributeLists, this.Modifiers, this.Identifier, this.ParameterList, this.Initializer, body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithExpressionBodyCore(ArrowExpressionClauseSyntax? expressionBody) => WithExpressionBody(expressionBody);
        public new ConstructorDeclarationSyntax WithExpressionBody(ArrowExpressionClauseSyntax? expressionBody) => Update(this.AttributeLists, this.Modifiers, this.Identifier, this.ParameterList, this.Initializer, this.Body, expressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken) => WithSemicolonToken(semicolonToken);
        public new ConstructorDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.Identifier, this.ParameterList, this.Initializer, this.Body, this.ExpressionBody, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new ConstructorDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new ConstructorDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        internal override BaseMethodDeclarationSyntax AddParameterListParametersCore(params ParameterSyntax[] items) => AddParameterListParameters(items);
        public new ConstructorDeclarationSyntax AddParameterListParameters(params ParameterSyntax[] items) => WithParameterList(this.ParameterList.WithParameters(this.ParameterList.Parameters.AddRange(items)));
        internal override BaseMethodDeclarationSyntax AddBodyAttributeListsCore(params AttributeListSyntax[] items) => AddBodyAttributeLists(items);
        public new ConstructorDeclarationSyntax AddBodyAttributeLists(params AttributeListSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithAttributeLists(body.AttributeLists.AddRange(items)));
        }
        internal override BaseMethodDeclarationSyntax AddBodyStatementsCore(params StatementSyntax[] items) => AddBodyStatements(items);
        public new ConstructorDeclarationSyntax AddBodyStatements(params StatementSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithStatements(body.Statements.AddRange(items)));
        }
    }

    /// <summary>Constructor initializer syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.BaseConstructorInitializer"/></description></item>
    /// <item><description><see cref="SyntaxKind.ThisConstructorInitializer"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ConstructorInitializerSyntax : CSharpSyntaxNode
    {
        private ArgumentListSyntax? argumentList;

        internal ConstructorInitializerSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the colon token.</summary>
        public SyntaxToken ColonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ConstructorInitializerSyntax)this.Green).colonToken, Position, 0);

        /// <summary>Gets the "this" or "base" keyword.</summary>
        public SyntaxToken ThisOrBaseKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ConstructorInitializerSyntax)this.Green).thisOrBaseKeyword, GetChildPosition(1), GetChildIndex(1));

        public ArgumentListSyntax ArgumentList => GetRed(ref this.argumentList, 2)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.argumentList, 2)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.argumentList : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitConstructorInitializer(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitConstructorInitializer(this);

        public ConstructorInitializerSyntax Update(SyntaxToken colonToken, SyntaxToken thisOrBaseKeyword, ArgumentListSyntax argumentList)
        {
            if (colonToken != this.ColonToken || thisOrBaseKeyword != this.ThisOrBaseKeyword || argumentList != this.ArgumentList)
            {
                var newNode = SyntaxFactory.ConstructorInitializer(this.Kind(), colonToken, thisOrBaseKeyword, argumentList);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ConstructorInitializerSyntax WithColonToken(SyntaxToken colonToken) => Update(colonToken, this.ThisOrBaseKeyword, this.ArgumentList);
        public ConstructorInitializerSyntax WithThisOrBaseKeyword(SyntaxToken thisOrBaseKeyword) => Update(this.ColonToken, thisOrBaseKeyword, this.ArgumentList);
        public ConstructorInitializerSyntax WithArgumentList(ArgumentListSyntax argumentList) => Update(this.ColonToken, this.ThisOrBaseKeyword, argumentList);

        public ConstructorInitializerSyntax AddArgumentListArguments(params ArgumentSyntax[] items) => WithArgumentList(this.ArgumentList.WithArguments(this.ArgumentList.Arguments.AddRange(items)));
    }

    /// <summary>Destructor declaration syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.DestructorDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class DestructorDeclarationSyntax : BaseMethodDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private ParameterListSyntax? parameterList;
        private BlockSyntax? body;
        private ArrowExpressionClauseSyntax? expressionBody;

        internal DestructorDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the tilde token.</summary>
        public SyntaxToken TildeToken => new SyntaxToken(this, ((Syntax.InternalSyntax.DestructorDeclarationSyntax)this.Green).tildeToken, GetChildPosition(2), GetChildIndex(2));

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.DestructorDeclarationSyntax)this.Green).identifier, GetChildPosition(3), GetChildIndex(3));

        public override ParameterListSyntax ParameterList => GetRed(ref this.parameterList, 4)!;

        public override BlockSyntax? Body => GetRed(ref this.body, 5);

        public override ArrowExpressionClauseSyntax? ExpressionBody => GetRed(ref this.expressionBody, 6);

        /// <summary>Gets the optional semicolon token.</summary>
        public override SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.DestructorDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(7), GetChildIndex(7)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                4 => GetRed(ref this.parameterList, 4)!,
                5 => GetRed(ref this.body, 5),
                6 => GetRed(ref this.expressionBody, 6),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                4 => this.parameterList,
                5 => this.body,
                6 => this.expressionBody,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitDestructorDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDestructorDeclaration(this);

        public DestructorDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken tildeToken, SyntaxToken identifier, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || tildeToken != this.TildeToken || identifier != this.Identifier || parameterList != this.ParameterList || body != this.Body || expressionBody != this.ExpressionBody || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.DestructorDeclaration(attributeLists, modifiers, tildeToken, identifier, parameterList, body, expressionBody, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new DestructorDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.TildeToken, this.Identifier, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new DestructorDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.TildeToken, this.Identifier, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        public DestructorDeclarationSyntax WithTildeToken(SyntaxToken tildeToken) => Update(this.AttributeLists, this.Modifiers, tildeToken, this.Identifier, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        public DestructorDeclarationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, this.TildeToken, identifier, this.ParameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithParameterListCore(ParameterListSyntax parameterList) => WithParameterList(parameterList);
        public new DestructorDeclarationSyntax WithParameterList(ParameterListSyntax parameterList) => Update(this.AttributeLists, this.Modifiers, this.TildeToken, this.Identifier, parameterList, this.Body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithBodyCore(BlockSyntax? body) => WithBody(body);
        public new DestructorDeclarationSyntax WithBody(BlockSyntax? body) => Update(this.AttributeLists, this.Modifiers, this.TildeToken, this.Identifier, this.ParameterList, body, this.ExpressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithExpressionBodyCore(ArrowExpressionClauseSyntax? expressionBody) => WithExpressionBody(expressionBody);
        public new DestructorDeclarationSyntax WithExpressionBody(ArrowExpressionClauseSyntax? expressionBody) => Update(this.AttributeLists, this.Modifiers, this.TildeToken, this.Identifier, this.ParameterList, this.Body, expressionBody, this.SemicolonToken);
        internal override BaseMethodDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken) => WithSemicolonToken(semicolonToken);
        public new DestructorDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.TildeToken, this.Identifier, this.ParameterList, this.Body, this.ExpressionBody, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new DestructorDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new DestructorDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        internal override BaseMethodDeclarationSyntax AddParameterListParametersCore(params ParameterSyntax[] items) => AddParameterListParameters(items);
        public new DestructorDeclarationSyntax AddParameterListParameters(params ParameterSyntax[] items) => WithParameterList(this.ParameterList.WithParameters(this.ParameterList.Parameters.AddRange(items)));
        internal override BaseMethodDeclarationSyntax AddBodyAttributeListsCore(params AttributeListSyntax[] items) => AddBodyAttributeLists(items);
        public new DestructorDeclarationSyntax AddBodyAttributeLists(params AttributeListSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithAttributeLists(body.AttributeLists.AddRange(items)));
        }
        internal override BaseMethodDeclarationSyntax AddBodyStatementsCore(params StatementSyntax[] items) => AddBodyStatements(items);
        public new DestructorDeclarationSyntax AddBodyStatements(params StatementSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithStatements(body.Statements.AddRange(items)));
        }
    }

    /// <summary>Base type for property declaration syntax.</summary>
    public abstract partial class BasePropertyDeclarationSyntax : MemberDeclarationSyntax
    {
        internal BasePropertyDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the type syntax.</summary>
        public abstract TypeSyntax Type { get; }
        public BasePropertyDeclarationSyntax WithType(TypeSyntax type) => WithTypeCore(type);
        internal abstract BasePropertyDeclarationSyntax WithTypeCore(TypeSyntax type);

        /// <summary>Gets the optional explicit interface specifier.</summary>
        public abstract ExplicitInterfaceSpecifierSyntax? ExplicitInterfaceSpecifier { get; }
        public BasePropertyDeclarationSyntax WithExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier) => WithExplicitInterfaceSpecifierCore(explicitInterfaceSpecifier);
        internal abstract BasePropertyDeclarationSyntax WithExplicitInterfaceSpecifierCore(ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier);

        public abstract AccessorListSyntax? AccessorList { get; }
        public BasePropertyDeclarationSyntax WithAccessorList(AccessorListSyntax? accessorList) => WithAccessorListCore(accessorList);
        internal abstract BasePropertyDeclarationSyntax WithAccessorListCore(AccessorListSyntax? accessorList);

        public BasePropertyDeclarationSyntax AddAccessorListAccessors(params AccessorDeclarationSyntax[] items) => AddAccessorListAccessorsCore(items);
        internal abstract BasePropertyDeclarationSyntax AddAccessorListAccessorsCore(params AccessorDeclarationSyntax[] items);

        public new BasePropertyDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => (BasePropertyDeclarationSyntax)WithAttributeListsCore(attributeLists);
        public new BasePropertyDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => (BasePropertyDeclarationSyntax)WithModifiersCore(modifiers);

        public new BasePropertyDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => (BasePropertyDeclarationSyntax)AddAttributeListsCore(items);

        public new BasePropertyDeclarationSyntax AddModifiers(params SyntaxToken[] items) => (BasePropertyDeclarationSyntax)AddModifiersCore(items);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.PropertyDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class PropertyDeclarationSyntax : BasePropertyDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeSyntax? type;
        private ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier;
        private AccessorListSyntax? accessorList;
        private ArrowExpressionClauseSyntax? expressionBody;
        private EqualsValueClauseSyntax? initializer;

        internal PropertyDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public override TypeSyntax Type => GetRed(ref this.type, 2)!;

        public override ExplicitInterfaceSpecifierSyntax? ExplicitInterfaceSpecifier => GetRed(ref this.explicitInterfaceSpecifier, 3);

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.PropertyDeclarationSyntax)this.Green).identifier, GetChildPosition(4), GetChildIndex(4));

        public override AccessorListSyntax? AccessorList => GetRed(ref this.accessorList, 5);

        public ArrowExpressionClauseSyntax? ExpressionBody => GetRed(ref this.expressionBody, 6);

        public EqualsValueClauseSyntax? Initializer => GetRed(ref this.initializer, 7);

        public SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.PropertyDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(8), GetChildIndex(8)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.type, 2)!,
                3 => GetRed(ref this.explicitInterfaceSpecifier, 3),
                5 => GetRed(ref this.accessorList, 5),
                6 => GetRed(ref this.expressionBody, 6),
                7 => GetRed(ref this.initializer, 7),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.type,
                3 => this.explicitInterfaceSpecifier,
                5 => this.accessorList,
                6 => this.expressionBody,
                7 => this.initializer,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitPropertyDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPropertyDeclaration(this);

        public PropertyDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax type, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, AccessorListSyntax? accessorList, ArrowExpressionClauseSyntax? expressionBody, EqualsValueClauseSyntax? initializer, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || type != this.Type || explicitInterfaceSpecifier != this.ExplicitInterfaceSpecifier || identifier != this.Identifier || accessorList != this.AccessorList || expressionBody != this.ExpressionBody || initializer != this.Initializer || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.PropertyDeclaration(attributeLists, modifiers, type, explicitInterfaceSpecifier, identifier, accessorList, expressionBody, initializer, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new PropertyDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Type, this.ExplicitInterfaceSpecifier, this.Identifier, this.AccessorList, this.ExpressionBody, this.Initializer, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new PropertyDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Type, this.ExplicitInterfaceSpecifier, this.Identifier, this.AccessorList, this.ExpressionBody, this.Initializer, this.SemicolonToken);
        internal override BasePropertyDeclarationSyntax WithTypeCore(TypeSyntax type) => WithType(type);
        public new PropertyDeclarationSyntax WithType(TypeSyntax type) => Update(this.AttributeLists, this.Modifiers, type, this.ExplicitInterfaceSpecifier, this.Identifier, this.AccessorList, this.ExpressionBody, this.Initializer, this.SemicolonToken);
        internal override BasePropertyDeclarationSyntax WithExplicitInterfaceSpecifierCore(ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier) => WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);
        public new PropertyDeclarationSyntax WithExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier) => Update(this.AttributeLists, this.Modifiers, this.Type, explicitInterfaceSpecifier, this.Identifier, this.AccessorList, this.ExpressionBody, this.Initializer, this.SemicolonToken);
        public PropertyDeclarationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, this.Type, this.ExplicitInterfaceSpecifier, identifier, this.AccessorList, this.ExpressionBody, this.Initializer, this.SemicolonToken);
        internal override BasePropertyDeclarationSyntax WithAccessorListCore(AccessorListSyntax? accessorList) => WithAccessorList(accessorList);
        public new PropertyDeclarationSyntax WithAccessorList(AccessorListSyntax? accessorList) => Update(this.AttributeLists, this.Modifiers, this.Type, this.ExplicitInterfaceSpecifier, this.Identifier, accessorList, this.ExpressionBody, this.Initializer, this.SemicolonToken);
        public PropertyDeclarationSyntax WithExpressionBody(ArrowExpressionClauseSyntax? expressionBody) => Update(this.AttributeLists, this.Modifiers, this.Type, this.ExplicitInterfaceSpecifier, this.Identifier, this.AccessorList, expressionBody, this.Initializer, this.SemicolonToken);
        public PropertyDeclarationSyntax WithInitializer(EqualsValueClauseSyntax? initializer) => Update(this.AttributeLists, this.Modifiers, this.Type, this.ExplicitInterfaceSpecifier, this.Identifier, this.AccessorList, this.ExpressionBody, initializer, this.SemicolonToken);
        public PropertyDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.Type, this.ExplicitInterfaceSpecifier, this.Identifier, this.AccessorList, this.ExpressionBody, this.Initializer, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new PropertyDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new PropertyDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        internal override BasePropertyDeclarationSyntax AddAccessorListAccessorsCore(params AccessorDeclarationSyntax[] items) => AddAccessorListAccessors(items);
        public new PropertyDeclarationSyntax AddAccessorListAccessors(params AccessorDeclarationSyntax[] items)
        {
            var accessorList = this.AccessorList ?? SyntaxFactory.AccessorList();
            return WithAccessorList(accessorList.WithAccessors(accessorList.Accessors.AddRange(items)));
        }
    }

    /// <summary>The syntax for the expression body of an expression-bodied member.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ArrowExpressionClause"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ArrowExpressionClauseSyntax : CSharpSyntaxNode
    {
        private ExpressionSyntax? expression;

        internal ArrowExpressionClauseSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken ArrowToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ArrowExpressionClauseSyntax)this.Green).arrowToken, Position, 0);

        public ExpressionSyntax Expression => GetRed(ref this.expression, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.expression, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.expression : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitArrowExpressionClause(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitArrowExpressionClause(this);

        public ArrowExpressionClauseSyntax Update(SyntaxToken arrowToken, ExpressionSyntax expression)
        {
            if (arrowToken != this.ArrowToken || expression != this.Expression)
            {
                var newNode = SyntaxFactory.ArrowExpressionClause(arrowToken, expression);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ArrowExpressionClauseSyntax WithArrowToken(SyntaxToken arrowToken) => Update(arrowToken, this.Expression);
        public ArrowExpressionClauseSyntax WithExpression(ExpressionSyntax expression) => Update(this.ArrowToken, expression);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.EventDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class EventDeclarationSyntax : BasePropertyDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeSyntax? type;
        private ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier;
        private AccessorListSyntax? accessorList;

        internal EventDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public SyntaxToken EventKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.EventDeclarationSyntax)this.Green).eventKeyword, GetChildPosition(2), GetChildIndex(2));

        public override TypeSyntax Type => GetRed(ref this.type, 3)!;

        public override ExplicitInterfaceSpecifierSyntax? ExplicitInterfaceSpecifier => GetRed(ref this.explicitInterfaceSpecifier, 4);

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.EventDeclarationSyntax)this.Green).identifier, GetChildPosition(5), GetChildIndex(5));

        public override AccessorListSyntax? AccessorList => GetRed(ref this.accessorList, 6);

        public SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.EventDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(7), GetChildIndex(7)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.type, 3)!,
                4 => GetRed(ref this.explicitInterfaceSpecifier, 4),
                6 => GetRed(ref this.accessorList, 6),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.type,
                4 => this.explicitInterfaceSpecifier,
                6 => this.accessorList,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitEventDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEventDeclaration(this);

        public EventDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken eventKeyword, TypeSyntax type, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, AccessorListSyntax? accessorList, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || eventKeyword != this.EventKeyword || type != this.Type || explicitInterfaceSpecifier != this.ExplicitInterfaceSpecifier || identifier != this.Identifier || accessorList != this.AccessorList || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.EventDeclaration(attributeLists, modifiers, eventKeyword, type, explicitInterfaceSpecifier, identifier, accessorList, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new EventDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.EventKeyword, this.Type, this.ExplicitInterfaceSpecifier, this.Identifier, this.AccessorList, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new EventDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.EventKeyword, this.Type, this.ExplicitInterfaceSpecifier, this.Identifier, this.AccessorList, this.SemicolonToken);
        public EventDeclarationSyntax WithEventKeyword(SyntaxToken eventKeyword) => Update(this.AttributeLists, this.Modifiers, eventKeyword, this.Type, this.ExplicitInterfaceSpecifier, this.Identifier, this.AccessorList, this.SemicolonToken);
        internal override BasePropertyDeclarationSyntax WithTypeCore(TypeSyntax type) => WithType(type);
        public new EventDeclarationSyntax WithType(TypeSyntax type) => Update(this.AttributeLists, this.Modifiers, this.EventKeyword, type, this.ExplicitInterfaceSpecifier, this.Identifier, this.AccessorList, this.SemicolonToken);
        internal override BasePropertyDeclarationSyntax WithExplicitInterfaceSpecifierCore(ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier) => WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);
        public new EventDeclarationSyntax WithExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier) => Update(this.AttributeLists, this.Modifiers, this.EventKeyword, this.Type, explicitInterfaceSpecifier, this.Identifier, this.AccessorList, this.SemicolonToken);
        public EventDeclarationSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, this.EventKeyword, this.Type, this.ExplicitInterfaceSpecifier, identifier, this.AccessorList, this.SemicolonToken);
        internal override BasePropertyDeclarationSyntax WithAccessorListCore(AccessorListSyntax? accessorList) => WithAccessorList(accessorList);
        public new EventDeclarationSyntax WithAccessorList(AccessorListSyntax? accessorList) => Update(this.AttributeLists, this.Modifiers, this.EventKeyword, this.Type, this.ExplicitInterfaceSpecifier, this.Identifier, accessorList, this.SemicolonToken);
        public EventDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.EventKeyword, this.Type, this.ExplicitInterfaceSpecifier, this.Identifier, this.AccessorList, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new EventDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new EventDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        internal override BasePropertyDeclarationSyntax AddAccessorListAccessorsCore(params AccessorDeclarationSyntax[] items) => AddAccessorListAccessors(items);
        public new EventDeclarationSyntax AddAccessorListAccessors(params AccessorDeclarationSyntax[] items)
        {
            var accessorList = this.AccessorList ?? SyntaxFactory.AccessorList();
            return WithAccessorList(accessorList.WithAccessors(accessorList.Accessors.AddRange(items)));
        }
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.IndexerDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class IndexerDeclarationSyntax : BasePropertyDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeSyntax? type;
        private ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier;
        private BracketedParameterListSyntax? parameterList;
        private AccessorListSyntax? accessorList;
        private ArrowExpressionClauseSyntax? expressionBody;

        internal IndexerDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public override TypeSyntax Type => GetRed(ref this.type, 2)!;

        public override ExplicitInterfaceSpecifierSyntax? ExplicitInterfaceSpecifier => GetRed(ref this.explicitInterfaceSpecifier, 3);

        public SyntaxToken ThisKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.IndexerDeclarationSyntax)this.Green).thisKeyword, GetChildPosition(4), GetChildIndex(4));

        /// <summary>Gets the parameter list.</summary>
        public BracketedParameterListSyntax ParameterList => GetRed(ref this.parameterList, 5)!;

        public override AccessorListSyntax? AccessorList => GetRed(ref this.accessorList, 6);

        public ArrowExpressionClauseSyntax? ExpressionBody => GetRed(ref this.expressionBody, 7);

        public SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.IndexerDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(8), GetChildIndex(8)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.type, 2)!,
                3 => GetRed(ref this.explicitInterfaceSpecifier, 3),
                5 => GetRed(ref this.parameterList, 5)!,
                6 => GetRed(ref this.accessorList, 6),
                7 => GetRed(ref this.expressionBody, 7),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.type,
                3 => this.explicitInterfaceSpecifier,
                5 => this.parameterList,
                6 => this.accessorList,
                7 => this.expressionBody,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitIndexerDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitIndexerDeclaration(this);

        public IndexerDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax type, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken thisKeyword, BracketedParameterListSyntax parameterList, AccessorListSyntax? accessorList, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || type != this.Type || explicitInterfaceSpecifier != this.ExplicitInterfaceSpecifier || thisKeyword != this.ThisKeyword || parameterList != this.ParameterList || accessorList != this.AccessorList || expressionBody != this.ExpressionBody || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.IndexerDeclaration(attributeLists, modifiers, type, explicitInterfaceSpecifier, thisKeyword, parameterList, accessorList, expressionBody, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new IndexerDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Type, this.ExplicitInterfaceSpecifier, this.ThisKeyword, this.ParameterList, this.AccessorList, this.ExpressionBody, this.SemicolonToken);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new IndexerDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Type, this.ExplicitInterfaceSpecifier, this.ThisKeyword, this.ParameterList, this.AccessorList, this.ExpressionBody, this.SemicolonToken);
        internal override BasePropertyDeclarationSyntax WithTypeCore(TypeSyntax type) => WithType(type);
        public new IndexerDeclarationSyntax WithType(TypeSyntax type) => Update(this.AttributeLists, this.Modifiers, type, this.ExplicitInterfaceSpecifier, this.ThisKeyword, this.ParameterList, this.AccessorList, this.ExpressionBody, this.SemicolonToken);
        internal override BasePropertyDeclarationSyntax WithExplicitInterfaceSpecifierCore(ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier) => WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);
        public new IndexerDeclarationSyntax WithExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier) => Update(this.AttributeLists, this.Modifiers, this.Type, explicitInterfaceSpecifier, this.ThisKeyword, this.ParameterList, this.AccessorList, this.ExpressionBody, this.SemicolonToken);
        public IndexerDeclarationSyntax WithThisKeyword(SyntaxToken thisKeyword) => Update(this.AttributeLists, this.Modifiers, this.Type, this.ExplicitInterfaceSpecifier, thisKeyword, this.ParameterList, this.AccessorList, this.ExpressionBody, this.SemicolonToken);
        public IndexerDeclarationSyntax WithParameterList(BracketedParameterListSyntax parameterList) => Update(this.AttributeLists, this.Modifiers, this.Type, this.ExplicitInterfaceSpecifier, this.ThisKeyword, parameterList, this.AccessorList, this.ExpressionBody, this.SemicolonToken);
        internal override BasePropertyDeclarationSyntax WithAccessorListCore(AccessorListSyntax? accessorList) => WithAccessorList(accessorList);
        public new IndexerDeclarationSyntax WithAccessorList(AccessorListSyntax? accessorList) => Update(this.AttributeLists, this.Modifiers, this.Type, this.ExplicitInterfaceSpecifier, this.ThisKeyword, this.ParameterList, accessorList, this.ExpressionBody, this.SemicolonToken);
        public IndexerDeclarationSyntax WithExpressionBody(ArrowExpressionClauseSyntax? expressionBody) => Update(this.AttributeLists, this.Modifiers, this.Type, this.ExplicitInterfaceSpecifier, this.ThisKeyword, this.ParameterList, this.AccessorList, expressionBody, this.SemicolonToken);
        public IndexerDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.Type, this.ExplicitInterfaceSpecifier, this.ThisKeyword, this.ParameterList, this.AccessorList, this.ExpressionBody, semicolonToken);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new IndexerDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new IndexerDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        public IndexerDeclarationSyntax AddParameterListParameters(params ParameterSyntax[] items) => WithParameterList(this.ParameterList.WithParameters(this.ParameterList.Parameters.AddRange(items)));
        internal override BasePropertyDeclarationSyntax AddAccessorListAccessorsCore(params AccessorDeclarationSyntax[] items) => AddAccessorListAccessors(items);
        public new IndexerDeclarationSyntax AddAccessorListAccessors(params AccessorDeclarationSyntax[] items)
        {
            var accessorList = this.AccessorList ?? SyntaxFactory.AccessorList();
            return WithAccessorList(accessorList.WithAccessors(accessorList.Accessors.AddRange(items)));
        }
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.AccessorList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class AccessorListSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? accessors;

        internal AccessorListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.AccessorListSyntax)this.Green).openBraceToken, Position, 0);

        public SyntaxList<AccessorDeclarationSyntax> Accessors => new SyntaxList<AccessorDeclarationSyntax>(GetRed(ref this.accessors, 1));

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Syntax.InternalSyntax.AccessorListSyntax)this.Green).closeBraceToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.accessors, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.accessors : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitAccessorList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAccessorList(this);

        public AccessorListSyntax Update(SyntaxToken openBraceToken, SyntaxList<AccessorDeclarationSyntax> accessors, SyntaxToken closeBraceToken)
        {
            if (openBraceToken != this.OpenBraceToken || accessors != this.Accessors || closeBraceToken != this.CloseBraceToken)
            {
                var newNode = SyntaxFactory.AccessorList(openBraceToken, accessors, closeBraceToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public AccessorListSyntax WithOpenBraceToken(SyntaxToken openBraceToken) => Update(openBraceToken, this.Accessors, this.CloseBraceToken);
        public AccessorListSyntax WithAccessors(SyntaxList<AccessorDeclarationSyntax> accessors) => Update(this.OpenBraceToken, accessors, this.CloseBraceToken);
        public AccessorListSyntax WithCloseBraceToken(SyntaxToken closeBraceToken) => Update(this.OpenBraceToken, this.Accessors, closeBraceToken);

        public AccessorListSyntax AddAccessors(params AccessorDeclarationSyntax[] items) => WithAccessors(this.Accessors.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.GetAccessorDeclaration"/></description></item>
    /// <item><description><see cref="SyntaxKind.SetAccessorDeclaration"/></description></item>
    /// <item><description><see cref="SyntaxKind.InitAccessorDeclaration"/></description></item>
    /// <item><description><see cref="SyntaxKind.AddAccessorDeclaration"/></description></item>
    /// <item><description><see cref="SyntaxKind.RemoveAccessorDeclaration"/></description></item>
    /// <item><description><see cref="SyntaxKind.UnknownAccessorDeclaration"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class AccessorDeclarationSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? attributeLists;
        private BlockSyntax? body;
        private ArrowExpressionClauseSyntax? expressionBody;

        internal AccessorDeclarationSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the attribute declaration list.</summary>
        public SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        /// <summary>Gets the modifier list.</summary>
        public SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the keyword token, or identifier if an erroneous accessor declaration.</summary>
        public SyntaxToken Keyword => new SyntaxToken(this, ((Syntax.InternalSyntax.AccessorDeclarationSyntax)this.Green).keyword, GetChildPosition(2), GetChildIndex(2));

        /// <summary>Gets the optional body block which may be empty, but it is null if there are no braces.</summary>
        public BlockSyntax? Body => GetRed(ref this.body, 3);

        /// <summary>Gets the optional expression body.</summary>
        public ArrowExpressionClauseSyntax? ExpressionBody => GetRed(ref this.expressionBody, 4);

        /// <summary>Gets the optional semicolon token.</summary>
        public SyntaxToken SemicolonToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.AccessorDeclarationSyntax)this.Green).semicolonToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(5), GetChildIndex(5)) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                3 => GetRed(ref this.body, 3),
                4 => GetRed(ref this.expressionBody, 4),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                3 => this.body,
                4 => this.expressionBody,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitAccessorDeclaration(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAccessorDeclaration(this);

        public AccessorDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken semicolonToken)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || keyword != this.Keyword || body != this.Body || expressionBody != this.ExpressionBody || semicolonToken != this.SemicolonToken)
            {
                var newNode = SyntaxFactory.AccessorDeclaration(this.Kind(), attributeLists, modifiers, keyword, body, expressionBody, semicolonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public AccessorDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Keyword, this.Body, this.ExpressionBody, this.SemicolonToken);
        public AccessorDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Keyword, this.Body, this.ExpressionBody, this.SemicolonToken);
        public AccessorDeclarationSyntax WithKeyword(SyntaxToken keyword) => Update(this.AttributeLists, this.Modifiers, keyword, this.Body, this.ExpressionBody, this.SemicolonToken);
        public AccessorDeclarationSyntax WithBody(BlockSyntax? body) => Update(this.AttributeLists, this.Modifiers, this.Keyword, body, this.ExpressionBody, this.SemicolonToken);
        public AccessorDeclarationSyntax WithExpressionBody(ArrowExpressionClauseSyntax? expressionBody) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Body, expressionBody, this.SemicolonToken);
        public AccessorDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => Update(this.AttributeLists, this.Modifiers, this.Keyword, this.Body, this.ExpressionBody, semicolonToken);

        public AccessorDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        public AccessorDeclarationSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
        public AccessorDeclarationSyntax AddBodyAttributeLists(params AttributeListSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithAttributeLists(body.AttributeLists.AddRange(items)));
        }
        public AccessorDeclarationSyntax AddBodyStatements(params StatementSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return WithBody(body.WithStatements(body.Statements.AddRange(items)));
        }
    }

    /// <summary>Base type for parameter list syntax.</summary>
    public abstract partial class BaseParameterListSyntax : CSharpSyntaxNode
    {
        internal BaseParameterListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the parameter list.</summary>
        public abstract SeparatedSyntaxList<ParameterSyntax> Parameters { get; }
        public BaseParameterListSyntax WithParameters(SeparatedSyntaxList<ParameterSyntax> parameters) => WithParametersCore(parameters);
        internal abstract BaseParameterListSyntax WithParametersCore(SeparatedSyntaxList<ParameterSyntax> parameters);

        public BaseParameterListSyntax AddParameters(params ParameterSyntax[] items) => AddParametersCore(items);
        internal abstract BaseParameterListSyntax AddParametersCore(params ParameterSyntax[] items);
    }

    /// <summary>Parameter list syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ParameterList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ParameterListSyntax : BaseParameterListSyntax
    {
        private SyntaxNode? parameters;

        internal ParameterListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the open paren token.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ParameterListSyntax)this.Green).openParenToken, Position, 0);

        public override SeparatedSyntaxList<ParameterSyntax> Parameters
        {
            get
            {
                var red = GetRed(ref this.parameters, 1);
                return red != null ? new SeparatedSyntaxList<ParameterSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the close paren token.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ParameterListSyntax)this.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.parameters, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.parameters : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitParameterList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitParameterList(this);

        public ParameterListSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeParenToken)
        {
            if (openParenToken != this.OpenParenToken || parameters != this.Parameters || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.ParameterList(openParenToken, parameters, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ParameterListSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(openParenToken, this.Parameters, this.CloseParenToken);
        internal override BaseParameterListSyntax WithParametersCore(SeparatedSyntaxList<ParameterSyntax> parameters) => WithParameters(parameters);
        public new ParameterListSyntax WithParameters(SeparatedSyntaxList<ParameterSyntax> parameters) => Update(this.OpenParenToken, parameters, this.CloseParenToken);
        public ParameterListSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.OpenParenToken, this.Parameters, closeParenToken);

        internal override BaseParameterListSyntax AddParametersCore(params ParameterSyntax[] items) => AddParameters(items);
        public new ParameterListSyntax AddParameters(params ParameterSyntax[] items) => WithParameters(this.Parameters.AddRange(items));
    }

    /// <summary>Parameter list syntax with surrounding brackets.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.BracketedParameterList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class BracketedParameterListSyntax : BaseParameterListSyntax
    {
        private SyntaxNode? parameters;

        internal BracketedParameterListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the open bracket token.</summary>
        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.BracketedParameterListSyntax)this.Green).openBracketToken, Position, 0);

        public override SeparatedSyntaxList<ParameterSyntax> Parameters
        {
            get
            {
                var red = GetRed(ref this.parameters, 1);
                return red != null ? new SeparatedSyntaxList<ParameterSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the close bracket token.</summary>
        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.BracketedParameterListSyntax)this.Green).closeBracketToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.parameters, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.parameters : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitBracketedParameterList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBracketedParameterList(this);

        public BracketedParameterListSyntax Update(SyntaxToken openBracketToken, SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeBracketToken)
        {
            if (openBracketToken != this.OpenBracketToken || parameters != this.Parameters || closeBracketToken != this.CloseBracketToken)
            {
                var newNode = SyntaxFactory.BracketedParameterList(openBracketToken, parameters, closeBracketToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public BracketedParameterListSyntax WithOpenBracketToken(SyntaxToken openBracketToken) => Update(openBracketToken, this.Parameters, this.CloseBracketToken);
        internal override BaseParameterListSyntax WithParametersCore(SeparatedSyntaxList<ParameterSyntax> parameters) => WithParameters(parameters);
        public new BracketedParameterListSyntax WithParameters(SeparatedSyntaxList<ParameterSyntax> parameters) => Update(this.OpenBracketToken, parameters, this.CloseBracketToken);
        public BracketedParameterListSyntax WithCloseBracketToken(SyntaxToken closeBracketToken) => Update(this.OpenBracketToken, this.Parameters, closeBracketToken);

        internal override BaseParameterListSyntax AddParametersCore(params ParameterSyntax[] items) => AddParameters(items);
        public new BracketedParameterListSyntax AddParameters(params ParameterSyntax[] items) => WithParameters(this.Parameters.AddRange(items));
    }

    /// <summary>Base parameter syntax.</summary>
    public abstract partial class BaseParameterSyntax : CSharpSyntaxNode
    {
        internal BaseParameterSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the attribute declaration list.</summary>
        public abstract SyntaxList<AttributeListSyntax> AttributeLists { get; }
        public BaseParameterSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeListsCore(attributeLists);
        internal abstract BaseParameterSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists);

        public BaseParameterSyntax AddAttributeLists(params AttributeListSyntax[] items) => AddAttributeListsCore(items);
        internal abstract BaseParameterSyntax AddAttributeListsCore(params AttributeListSyntax[] items);

        /// <summary>Gets the modifier list.</summary>
        public abstract SyntaxTokenList Modifiers { get; }
        public BaseParameterSyntax WithModifiers(SyntaxTokenList modifiers) => WithModifiersCore(modifiers);
        internal abstract BaseParameterSyntax WithModifiersCore(SyntaxTokenList modifiers);

        public BaseParameterSyntax AddModifiers(params SyntaxToken[] items) => AddModifiersCore(items);
        internal abstract BaseParameterSyntax AddModifiersCore(params SyntaxToken[] items);

        public abstract TypeSyntax? Type { get; }
        public BaseParameterSyntax WithType(TypeSyntax? type) => WithTypeCore(type);
        internal abstract BaseParameterSyntax WithTypeCore(TypeSyntax? type);
    }

    /// <summary>Parameter syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.Parameter"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ParameterSyntax : BaseParameterSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeSyntax? type;
        private EqualsValueClauseSyntax? @default;

        internal ParameterSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the attribute declaration list.</summary>
        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        /// <summary>Gets the modifier list.</summary>
        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public override TypeSyntax? Type => GetRed(ref this.type, 2);

        /// <summary>Gets the identifier.</summary>
        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.ParameterSyntax)this.Green).identifier, GetChildPosition(3), GetChildIndex(3));

        public EqualsValueClauseSyntax? Default => GetRed(ref this.@default, 4);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.type, 2),
                4 => GetRed(ref this.@default, 4),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.type,
                4 => this.@default,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitParameter(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitParameter(this);

        public ParameterSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax? type, SyntaxToken identifier, EqualsValueClauseSyntax? @default)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || type != this.Type || identifier != this.Identifier || @default != this.Default)
            {
                var newNode = SyntaxFactory.Parameter(attributeLists, modifiers, type, identifier, @default);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override BaseParameterSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new ParameterSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Type, this.Identifier, this.Default);
        internal override BaseParameterSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new ParameterSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Type, this.Identifier, this.Default);
        internal override BaseParameterSyntax WithTypeCore(TypeSyntax? type) => WithType(type);
        public new ParameterSyntax WithType(TypeSyntax? type) => Update(this.AttributeLists, this.Modifiers, type, this.Identifier, this.Default);
        public ParameterSyntax WithIdentifier(SyntaxToken identifier) => Update(this.AttributeLists, this.Modifiers, this.Type, identifier, this.Default);
        public ParameterSyntax WithDefault(EqualsValueClauseSyntax? @default) => Update(this.AttributeLists, this.Modifiers, this.Type, this.Identifier, @default);

        internal override BaseParameterSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new ParameterSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override BaseParameterSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new ParameterSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
    }

    /// <summary>Parameter syntax.</summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.FunctionPointerParameter"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class FunctionPointerParameterSyntax : BaseParameterSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeSyntax? type;

        internal FunctionPointerParameterSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the attribute declaration list.</summary>
        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        /// <summary>Gets the modifier list.</summary>
        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public override TypeSyntax Type => GetRed(ref this.type, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.type, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.type,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitFunctionPointerParameter(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionPointerParameter(this);

        public FunctionPointerParameterSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax type)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || type != this.Type)
            {
                var newNode = SyntaxFactory.FunctionPointerParameter(attributeLists, modifiers, type);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override BaseParameterSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new FunctionPointerParameterSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Type);
        internal override BaseParameterSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new FunctionPointerParameterSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Type);
        internal override BaseParameterSyntax WithTypeCore(TypeSyntax? type) => WithType(type ?? throw new ArgumentNullException(nameof(type)));
        public new FunctionPointerParameterSyntax WithType(TypeSyntax type) => Update(this.AttributeLists, this.Modifiers, type);

        internal override BaseParameterSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new FunctionPointerParameterSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override BaseParameterSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new FunctionPointerParameterSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.IncompleteMember"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class IncompleteMemberSyntax : MemberDeclarationSyntax
    {
        private SyntaxNode? attributeLists;
        private TypeSyntax? type;

        internal IncompleteMemberSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref this.attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public TypeSyntax? Type => GetRed(ref this.type, 2);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.attributeLists)!,
                2 => GetRed(ref this.type, 2),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.attributeLists,
                2 => this.type,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitIncompleteMember(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitIncompleteMember(this);

        public IncompleteMemberSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax? type)
        {
            if (attributeLists != this.AttributeLists || modifiers != this.Modifiers || type != this.Type)
            {
                var newNode = SyntaxFactory.IncompleteMember(attributeLists, modifiers, type);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists) => WithAttributeLists(attributeLists);
        public new IncompleteMemberSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists) => Update(attributeLists, this.Modifiers, this.Type);
        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers) => WithModifiers(modifiers);
        public new IncompleteMemberSyntax WithModifiers(SyntaxTokenList modifiers) => Update(this.AttributeLists, modifiers, this.Type);
        public IncompleteMemberSyntax WithType(TypeSyntax? type) => Update(this.AttributeLists, this.Modifiers, type);

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items) => AddAttributeLists(items);
        public new IncompleteMemberSyntax AddAttributeLists(params AttributeListSyntax[] items) => WithAttributeLists(this.AttributeLists.AddRange(items));
        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items) => AddModifiers(items);
        public new IncompleteMemberSyntax AddModifiers(params SyntaxToken[] items) => WithModifiers(this.Modifiers.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.SkippedTokensTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class SkippedTokensTriviaSyntax : StructuredTriviaSyntax
    {

        internal SkippedTokensTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxTokenList Tokens
        {
            get
            {
                var slot = this.Green.GetSlot(0);
                return slot != null ? new SyntaxTokenList(this, slot, Position, 0) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitSkippedTokensTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSkippedTokensTrivia(this);

        public SkippedTokensTriviaSyntax Update(SyntaxTokenList tokens)
        {
            if (tokens != this.Tokens)
            {
                var newNode = SyntaxFactory.SkippedTokensTrivia(tokens);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public SkippedTokensTriviaSyntax WithTokens(SyntaxTokenList tokens) => Update(tokens);

        public SkippedTokensTriviaSyntax AddTokens(params SyntaxToken[] items) => WithTokens(this.Tokens.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.SingleLineDocumentationCommentTrivia"/></description></item>
    /// <item><description><see cref="SyntaxKind.MultiLineDocumentationCommentTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class DocumentationCommentTriviaSyntax : StructuredTriviaSyntax
    {
        private SyntaxNode? content;

        internal DocumentationCommentTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxList<XmlNodeSyntax> Content => new SyntaxList<XmlNodeSyntax>(GetRed(ref this.content, 0));

        public SyntaxToken EndOfComment => new SyntaxToken(this, ((Syntax.InternalSyntax.DocumentationCommentTriviaSyntax)this.Green).endOfComment, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.content)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.content : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitDocumentationCommentTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDocumentationCommentTrivia(this);

        public DocumentationCommentTriviaSyntax Update(SyntaxList<XmlNodeSyntax> content, SyntaxToken endOfComment)
        {
            if (content != this.Content || endOfComment != this.EndOfComment)
            {
                var newNode = SyntaxFactory.DocumentationCommentTrivia(this.Kind(), content, endOfComment);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public DocumentationCommentTriviaSyntax WithContent(SyntaxList<XmlNodeSyntax> content) => Update(content, this.EndOfComment);
        public DocumentationCommentTriviaSyntax WithEndOfComment(SyntaxToken endOfComment) => Update(this.Content, endOfComment);

        public DocumentationCommentTriviaSyntax AddContent(params XmlNodeSyntax[] items) => WithContent(this.Content.AddRange(items));
    }

    /// <summary>
    /// A symbol referenced by a cref attribute (e.g. in a &lt;see&gt; or &lt;seealso&gt; documentation comment tag).
    /// For example, the M in &lt;see cref="M" /&gt;.
    /// </summary>
    public abstract partial class CrefSyntax : CSharpSyntaxNode
    {
        internal CrefSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    /// <summary>
    /// A symbol reference that definitely refers to a type.
    /// For example, "int", "A::B", "A.B", "A&lt;T&gt;", but not "M()" (has parameter list) or "this" (indexer).
    /// NOTE: TypeCrefSyntax, QualifiedCrefSyntax, and MemberCrefSyntax overlap.  The syntax in a TypeCrefSyntax
    /// will always be bound as type, so it's safer to use QualifiedCrefSyntax or MemberCrefSyntax if the symbol
    /// might be a non-type member.
    /// </summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.TypeCref"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class TypeCrefSyntax : CrefSyntax
    {
        private TypeSyntax? type;

        internal TypeCrefSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public TypeSyntax Type => GetRedAtZero(ref this.type)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.type)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.type : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitTypeCref(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeCref(this);

        public TypeCrefSyntax Update(TypeSyntax type)
        {
            if (type != this.Type)
            {
                var newNode = SyntaxFactory.TypeCref(type);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public TypeCrefSyntax WithType(TypeSyntax type) => Update(type);
    }

    /// <summary>
    /// A symbol reference to a type or non-type member that is qualified by an enclosing type or namespace.
    /// For example, cref="System.String.ToString()".
    /// NOTE: TypeCrefSyntax, QualifiedCrefSyntax, and MemberCrefSyntax overlap.  The syntax in a TypeCrefSyntax
    /// will always be bound as type, so it's safer to use QualifiedCrefSyntax or MemberCrefSyntax if the symbol
    /// might be a non-type member.
    /// </summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.QualifiedCref"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class QualifiedCrefSyntax : CrefSyntax
    {
        private TypeSyntax? container;
        private MemberCrefSyntax? member;

        internal QualifiedCrefSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public TypeSyntax Container => GetRedAtZero(ref this.container)!;

        public SyntaxToken DotToken => new SyntaxToken(this, ((Syntax.InternalSyntax.QualifiedCrefSyntax)this.Green).dotToken, GetChildPosition(1), GetChildIndex(1));

        public MemberCrefSyntax Member => GetRed(ref this.member, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.container)!,
                2 => GetRed(ref this.member, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.container,
                2 => this.member,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitQualifiedCref(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitQualifiedCref(this);

        public QualifiedCrefSyntax Update(TypeSyntax container, SyntaxToken dotToken, MemberCrefSyntax member)
        {
            if (container != this.Container || dotToken != this.DotToken || member != this.Member)
            {
                var newNode = SyntaxFactory.QualifiedCref(container, dotToken, member);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public QualifiedCrefSyntax WithContainer(TypeSyntax container) => Update(container, this.DotToken, this.Member);
        public QualifiedCrefSyntax WithDotToken(SyntaxToken dotToken) => Update(this.Container, dotToken, this.Member);
        public QualifiedCrefSyntax WithMember(MemberCrefSyntax member) => Update(this.Container, this.DotToken, member);
    }

    /// <summary>
    /// The unqualified part of a CrefSyntax.
    /// For example, "ToString()" in "object.ToString()".
    /// NOTE: TypeCrefSyntax, QualifiedCrefSyntax, and MemberCrefSyntax overlap.  The syntax in a TypeCrefSyntax
    /// will always be bound as type, so it's safer to use QualifiedCrefSyntax or MemberCrefSyntax if the symbol
    /// might be a non-type member.
    /// </summary>
    public abstract partial class MemberCrefSyntax : CrefSyntax
    {
        internal MemberCrefSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    /// <summary>
    /// A MemberCrefSyntax specified by a name (an identifier, predefined type keyword, or an alias-qualified name,
    /// with an optional type parameter list) and an optional parameter list.
    /// For example, "M", "M&lt;T&gt;" or "M(int)".
    /// Also, "A::B()" or "string()".
    /// </summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.NameMemberCref"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class NameMemberCrefSyntax : MemberCrefSyntax
    {
        private TypeSyntax? name;
        private CrefParameterListSyntax? parameters;

        internal NameMemberCrefSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public TypeSyntax Name => GetRedAtZero(ref this.name)!;

        public CrefParameterListSyntax? Parameters => GetRed(ref this.parameters, 1);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.name)!,
                1 => GetRed(ref this.parameters, 1),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.name,
                1 => this.parameters,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitNameMemberCref(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitNameMemberCref(this);

        public NameMemberCrefSyntax Update(TypeSyntax name, CrefParameterListSyntax? parameters)
        {
            if (name != this.Name || parameters != this.Parameters)
            {
                var newNode = SyntaxFactory.NameMemberCref(name, parameters);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public NameMemberCrefSyntax WithName(TypeSyntax name) => Update(name, this.Parameters);
        public NameMemberCrefSyntax WithParameters(CrefParameterListSyntax? parameters) => Update(this.Name, parameters);

        public NameMemberCrefSyntax AddParametersParameters(params CrefParameterSyntax[] items)
        {
            var parameters = this.Parameters ?? SyntaxFactory.CrefParameterList();
            return WithParameters(parameters.WithParameters(parameters.Parameters.AddRange(items)));
        }
    }

    /// <summary>
    /// A MemberCrefSyntax specified by a this keyword and an optional parameter list.
    /// For example, "this" or "this[int]".
    /// </summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.IndexerMemberCref"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class IndexerMemberCrefSyntax : MemberCrefSyntax
    {
        private CrefBracketedParameterListSyntax? parameters;

        internal IndexerMemberCrefSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken ThisKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.IndexerMemberCrefSyntax)this.Green).thisKeyword, Position, 0);

        public CrefBracketedParameterListSyntax? Parameters => GetRed(ref this.parameters, 1);

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.parameters, 1) : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.parameters : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitIndexerMemberCref(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitIndexerMemberCref(this);

        public IndexerMemberCrefSyntax Update(SyntaxToken thisKeyword, CrefBracketedParameterListSyntax? parameters)
        {
            if (thisKeyword != this.ThisKeyword || parameters != this.Parameters)
            {
                var newNode = SyntaxFactory.IndexerMemberCref(thisKeyword, parameters);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public IndexerMemberCrefSyntax WithThisKeyword(SyntaxToken thisKeyword) => Update(thisKeyword, this.Parameters);
        public IndexerMemberCrefSyntax WithParameters(CrefBracketedParameterListSyntax? parameters) => Update(this.ThisKeyword, parameters);

        public IndexerMemberCrefSyntax AddParametersParameters(params CrefParameterSyntax[] items)
        {
            var parameters = this.Parameters ?? SyntaxFactory.CrefBracketedParameterList();
            return WithParameters(parameters.WithParameters(parameters.Parameters.AddRange(items)));
        }
    }

    /// <summary>
    /// A MemberCrefSyntax specified by an operator keyword, an operator symbol and an optional parameter list.
    /// For example, "operator +" or "operator -[int]".
    /// NOTE: the operator must be overloadable.
    /// </summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.OperatorMemberCref"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class OperatorMemberCrefSyntax : MemberCrefSyntax
    {
        private CrefParameterListSyntax? parameters;

        internal OperatorMemberCrefSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken OperatorKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.OperatorMemberCrefSyntax)this.Green).operatorKeyword, Position, 0);

        /// <summary>Gets the operator token.</summary>
        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Syntax.InternalSyntax.OperatorMemberCrefSyntax)this.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        public CrefParameterListSyntax? Parameters => GetRed(ref this.parameters, 2);

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.parameters, 2) : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.parameters : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitOperatorMemberCref(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitOperatorMemberCref(this);

        public OperatorMemberCrefSyntax Update(SyntaxToken operatorKeyword, SyntaxToken operatorToken, CrefParameterListSyntax? parameters)
        {
            if (operatorKeyword != this.OperatorKeyword || operatorToken != this.OperatorToken || parameters != this.Parameters)
            {
                var newNode = SyntaxFactory.OperatorMemberCref(operatorKeyword, operatorToken, parameters);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public OperatorMemberCrefSyntax WithOperatorKeyword(SyntaxToken operatorKeyword) => Update(operatorKeyword, this.OperatorToken, this.Parameters);
        public OperatorMemberCrefSyntax WithOperatorToken(SyntaxToken operatorToken) => Update(this.OperatorKeyword, operatorToken, this.Parameters);
        public OperatorMemberCrefSyntax WithParameters(CrefParameterListSyntax? parameters) => Update(this.OperatorKeyword, this.OperatorToken, parameters);

        public OperatorMemberCrefSyntax AddParametersParameters(params CrefParameterSyntax[] items)
        {
            var parameters = this.Parameters ?? SyntaxFactory.CrefParameterList();
            return WithParameters(parameters.WithParameters(parameters.Parameters.AddRange(items)));
        }
    }

    /// <summary>
    /// A MemberCrefSyntax specified by an implicit or explicit keyword, an operator keyword, a destination type, and an optional parameter list.
    /// For example, "implicit operator int" or "explicit operator MyType(int)".
    /// </summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ConversionOperatorMemberCref"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ConversionOperatorMemberCrefSyntax : MemberCrefSyntax
    {
        private TypeSyntax? type;
        private CrefParameterListSyntax? parameters;

        internal ConversionOperatorMemberCrefSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken ImplicitOrExplicitKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ConversionOperatorMemberCrefSyntax)this.Green).implicitOrExplicitKeyword, Position, 0);

        public SyntaxToken OperatorKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ConversionOperatorMemberCrefSyntax)this.Green).operatorKeyword, GetChildPosition(1), GetChildIndex(1));

        public TypeSyntax Type => GetRed(ref this.type, 2)!;

        public CrefParameterListSyntax? Parameters => GetRed(ref this.parameters, 3);

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                2 => GetRed(ref this.type, 2)!,
                3 => GetRed(ref this.parameters, 3),
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                2 => this.type,
                3 => this.parameters,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitConversionOperatorMemberCref(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitConversionOperatorMemberCref(this);

        public ConversionOperatorMemberCrefSyntax Update(SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, TypeSyntax type, CrefParameterListSyntax? parameters)
        {
            if (implicitOrExplicitKeyword != this.ImplicitOrExplicitKeyword || operatorKeyword != this.OperatorKeyword || type != this.Type || parameters != this.Parameters)
            {
                var newNode = SyntaxFactory.ConversionOperatorMemberCref(implicitOrExplicitKeyword, operatorKeyword, type, parameters);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public ConversionOperatorMemberCrefSyntax WithImplicitOrExplicitKeyword(SyntaxToken implicitOrExplicitKeyword) => Update(implicitOrExplicitKeyword, this.OperatorKeyword, this.Type, this.Parameters);
        public ConversionOperatorMemberCrefSyntax WithOperatorKeyword(SyntaxToken operatorKeyword) => Update(this.ImplicitOrExplicitKeyword, operatorKeyword, this.Type, this.Parameters);
        public ConversionOperatorMemberCrefSyntax WithType(TypeSyntax type) => Update(this.ImplicitOrExplicitKeyword, this.OperatorKeyword, type, this.Parameters);
        public ConversionOperatorMemberCrefSyntax WithParameters(CrefParameterListSyntax? parameters) => Update(this.ImplicitOrExplicitKeyword, this.OperatorKeyword, this.Type, parameters);

        public ConversionOperatorMemberCrefSyntax AddParametersParameters(params CrefParameterSyntax[] items)
        {
            var parameters = this.Parameters ?? SyntaxFactory.CrefParameterList();
            return WithParameters(parameters.WithParameters(parameters.Parameters.AddRange(items)));
        }
    }

    /// <summary>
    /// A list of cref parameters with surrounding punctuation.
    /// Unlike regular parameters, cref parameters do not have names.
    /// </summary>
    public abstract partial class BaseCrefParameterListSyntax : CSharpSyntaxNode
    {
        internal BaseCrefParameterListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the parameter list.</summary>
        public abstract SeparatedSyntaxList<CrefParameterSyntax> Parameters { get; }
        public BaseCrefParameterListSyntax WithParameters(SeparatedSyntaxList<CrefParameterSyntax> parameters) => WithParametersCore(parameters);
        internal abstract BaseCrefParameterListSyntax WithParametersCore(SeparatedSyntaxList<CrefParameterSyntax> parameters);

        public BaseCrefParameterListSyntax AddParameters(params CrefParameterSyntax[] items) => AddParametersCore(items);
        internal abstract BaseCrefParameterListSyntax AddParametersCore(params CrefParameterSyntax[] items);
    }

    /// <summary>
    /// A parenthesized list of cref parameters.
    /// </summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.CrefParameterList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class CrefParameterListSyntax : BaseCrefParameterListSyntax
    {
        private SyntaxNode? parameters;

        internal CrefParameterListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the open paren token.</summary>
        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CrefParameterListSyntax)this.Green).openParenToken, Position, 0);

        public override SeparatedSyntaxList<CrefParameterSyntax> Parameters
        {
            get
            {
                var red = GetRed(ref this.parameters, 1);
                return red != null ? new SeparatedSyntaxList<CrefParameterSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the close paren token.</summary>
        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CrefParameterListSyntax)this.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.parameters, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.parameters : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitCrefParameterList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCrefParameterList(this);

        public CrefParameterListSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<CrefParameterSyntax> parameters, SyntaxToken closeParenToken)
        {
            if (openParenToken != this.OpenParenToken || parameters != this.Parameters || closeParenToken != this.CloseParenToken)
            {
                var newNode = SyntaxFactory.CrefParameterList(openParenToken, parameters, closeParenToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public CrefParameterListSyntax WithOpenParenToken(SyntaxToken openParenToken) => Update(openParenToken, this.Parameters, this.CloseParenToken);
        internal override BaseCrefParameterListSyntax WithParametersCore(SeparatedSyntaxList<CrefParameterSyntax> parameters) => WithParameters(parameters);
        public new CrefParameterListSyntax WithParameters(SeparatedSyntaxList<CrefParameterSyntax> parameters) => Update(this.OpenParenToken, parameters, this.CloseParenToken);
        public CrefParameterListSyntax WithCloseParenToken(SyntaxToken closeParenToken) => Update(this.OpenParenToken, this.Parameters, closeParenToken);

        internal override BaseCrefParameterListSyntax AddParametersCore(params CrefParameterSyntax[] items) => AddParameters(items);
        public new CrefParameterListSyntax AddParameters(params CrefParameterSyntax[] items) => WithParameters(this.Parameters.AddRange(items));
    }

    /// <summary>
    /// A bracketed list of cref parameters.
    /// </summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.CrefBracketedParameterList"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class CrefBracketedParameterListSyntax : BaseCrefParameterListSyntax
    {
        private SyntaxNode? parameters;

        internal CrefBracketedParameterListSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        /// <summary>Gets the open bracket token.</summary>
        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CrefBracketedParameterListSyntax)this.Green).openBracketToken, Position, 0);

        public override SeparatedSyntaxList<CrefParameterSyntax> Parameters
        {
            get
            {
                var red = GetRed(ref this.parameters, 1);
                return red != null ? new SeparatedSyntaxList<CrefParameterSyntax>(red, GetChildIndex(1)) : default;
            }
        }

        /// <summary>Gets the close bracket token.</summary>
        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Syntax.InternalSyntax.CrefBracketedParameterListSyntax)this.Green).closeBracketToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.parameters, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.parameters : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitCrefBracketedParameterList(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCrefBracketedParameterList(this);

        public CrefBracketedParameterListSyntax Update(SyntaxToken openBracketToken, SeparatedSyntaxList<CrefParameterSyntax> parameters, SyntaxToken closeBracketToken)
        {
            if (openBracketToken != this.OpenBracketToken || parameters != this.Parameters || closeBracketToken != this.CloseBracketToken)
            {
                var newNode = SyntaxFactory.CrefBracketedParameterList(openBracketToken, parameters, closeBracketToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public CrefBracketedParameterListSyntax WithOpenBracketToken(SyntaxToken openBracketToken) => Update(openBracketToken, this.Parameters, this.CloseBracketToken);
        internal override BaseCrefParameterListSyntax WithParametersCore(SeparatedSyntaxList<CrefParameterSyntax> parameters) => WithParameters(parameters);
        public new CrefBracketedParameterListSyntax WithParameters(SeparatedSyntaxList<CrefParameterSyntax> parameters) => Update(this.OpenBracketToken, parameters, this.CloseBracketToken);
        public CrefBracketedParameterListSyntax WithCloseBracketToken(SyntaxToken closeBracketToken) => Update(this.OpenBracketToken, this.Parameters, closeBracketToken);

        internal override BaseCrefParameterListSyntax AddParametersCore(params CrefParameterSyntax[] items) => AddParameters(items);
        public new CrefBracketedParameterListSyntax AddParameters(params CrefParameterSyntax[] items) => WithParameters(this.Parameters.AddRange(items));
    }

    /// <summary>
    /// An element of a BaseCrefParameterListSyntax.
    /// Unlike a regular parameter, a cref parameter has only an optional ref or out keyword and a type -
    /// there is no name and there are no attributes or other modifiers.
    /// </summary>
    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.CrefParameter"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class CrefParameterSyntax : CSharpSyntaxNode
    {
        private TypeSyntax? type;

        internal CrefParameterSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken RefKindKeyword
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.CrefParameterSyntax)this.Green).refKindKeyword;
                return slot != null ? new SyntaxToken(this, slot, Position, 0) : default;
            }
        }

        public TypeSyntax Type => GetRed(ref this.type, 1)!;

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.type, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.type : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitCrefParameter(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCrefParameter(this);

        public CrefParameterSyntax Update(SyntaxToken refKindKeyword, TypeSyntax type)
        {
            if (refKindKeyword != this.RefKindKeyword || type != this.Type)
            {
                var newNode = SyntaxFactory.CrefParameter(refKindKeyword, type);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public CrefParameterSyntax WithRefKindKeyword(SyntaxToken refKindKeyword) => Update(refKindKeyword, this.Type);
        public CrefParameterSyntax WithType(TypeSyntax type) => Update(this.RefKindKeyword, type);
    }

    public abstract partial class XmlNodeSyntax : CSharpSyntaxNode
    {
        internal XmlNodeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.XmlElement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class XmlElementSyntax : XmlNodeSyntax
    {
        private XmlElementStartTagSyntax? startTag;
        private SyntaxNode? content;
        private XmlElementEndTagSyntax? endTag;

        internal XmlElementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public XmlElementStartTagSyntax StartTag => GetRedAtZero(ref this.startTag)!;

        public SyntaxList<XmlNodeSyntax> Content => new SyntaxList<XmlNodeSyntax>(GetRed(ref this.content, 1));

        public XmlElementEndTagSyntax EndTag => GetRed(ref this.endTag, 2)!;

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.startTag)!,
                1 => GetRed(ref this.content, 1)!,
                2 => GetRed(ref this.endTag, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.startTag,
                1 => this.content,
                2 => this.endTag,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitXmlElement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlElement(this);

        public XmlElementSyntax Update(XmlElementStartTagSyntax startTag, SyntaxList<XmlNodeSyntax> content, XmlElementEndTagSyntax endTag)
        {
            if (startTag != this.StartTag || content != this.Content || endTag != this.EndTag)
            {
                var newNode = SyntaxFactory.XmlElement(startTag, content, endTag);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public XmlElementSyntax WithStartTag(XmlElementStartTagSyntax startTag) => Update(startTag, this.Content, this.EndTag);
        public XmlElementSyntax WithContent(SyntaxList<XmlNodeSyntax> content) => Update(this.StartTag, content, this.EndTag);
        public XmlElementSyntax WithEndTag(XmlElementEndTagSyntax endTag) => Update(this.StartTag, this.Content, endTag);

        public XmlElementSyntax AddStartTagAttributes(params XmlAttributeSyntax[] items) => WithStartTag(this.StartTag.WithAttributes(this.StartTag.Attributes.AddRange(items)));
        public XmlElementSyntax AddContent(params XmlNodeSyntax[] items) => WithContent(this.Content.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.XmlElementStartTag"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class XmlElementStartTagSyntax : CSharpSyntaxNode
    {
        private XmlNameSyntax? name;
        private SyntaxNode? attributes;

        internal XmlElementStartTagSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken LessThanToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlElementStartTagSyntax)this.Green).lessThanToken, Position, 0);

        public XmlNameSyntax Name => GetRed(ref this.name, 1)!;

        public SyntaxList<XmlAttributeSyntax> Attributes => new SyntaxList<XmlAttributeSyntax>(GetRed(ref this.attributes, 2));

        public SyntaxToken GreaterThanToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlElementStartTagSyntax)this.Green).greaterThanToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.name, 1)!,
                2 => GetRed(ref this.attributes, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.name,
                2 => this.attributes,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitXmlElementStartTag(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlElementStartTag(this);

        public XmlElementStartTagSyntax Update(SyntaxToken lessThanToken, XmlNameSyntax name, SyntaxList<XmlAttributeSyntax> attributes, SyntaxToken greaterThanToken)
        {
            if (lessThanToken != this.LessThanToken || name != this.Name || attributes != this.Attributes || greaterThanToken != this.GreaterThanToken)
            {
                var newNode = SyntaxFactory.XmlElementStartTag(lessThanToken, name, attributes, greaterThanToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public XmlElementStartTagSyntax WithLessThanToken(SyntaxToken lessThanToken) => Update(lessThanToken, this.Name, this.Attributes, this.GreaterThanToken);
        public XmlElementStartTagSyntax WithName(XmlNameSyntax name) => Update(this.LessThanToken, name, this.Attributes, this.GreaterThanToken);
        public XmlElementStartTagSyntax WithAttributes(SyntaxList<XmlAttributeSyntax> attributes) => Update(this.LessThanToken, this.Name, attributes, this.GreaterThanToken);
        public XmlElementStartTagSyntax WithGreaterThanToken(SyntaxToken greaterThanToken) => Update(this.LessThanToken, this.Name, this.Attributes, greaterThanToken);

        public XmlElementStartTagSyntax AddAttributes(params XmlAttributeSyntax[] items) => WithAttributes(this.Attributes.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.XmlElementEndTag"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class XmlElementEndTagSyntax : CSharpSyntaxNode
    {
        private XmlNameSyntax? name;

        internal XmlElementEndTagSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken LessThanSlashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlElementEndTagSyntax)this.Green).lessThanSlashToken, Position, 0);

        public XmlNameSyntax Name => GetRed(ref this.name, 1)!;

        public SyntaxToken GreaterThanToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlElementEndTagSyntax)this.Green).greaterThanToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.name, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.name : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitXmlElementEndTag(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlElementEndTag(this);

        public XmlElementEndTagSyntax Update(SyntaxToken lessThanSlashToken, XmlNameSyntax name, SyntaxToken greaterThanToken)
        {
            if (lessThanSlashToken != this.LessThanSlashToken || name != this.Name || greaterThanToken != this.GreaterThanToken)
            {
                var newNode = SyntaxFactory.XmlElementEndTag(lessThanSlashToken, name, greaterThanToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public XmlElementEndTagSyntax WithLessThanSlashToken(SyntaxToken lessThanSlashToken) => Update(lessThanSlashToken, this.Name, this.GreaterThanToken);
        public XmlElementEndTagSyntax WithName(XmlNameSyntax name) => Update(this.LessThanSlashToken, name, this.GreaterThanToken);
        public XmlElementEndTagSyntax WithGreaterThanToken(SyntaxToken greaterThanToken) => Update(this.LessThanSlashToken, this.Name, greaterThanToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.XmlEmptyElement"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class XmlEmptyElementSyntax : XmlNodeSyntax
    {
        private XmlNameSyntax? name;
        private SyntaxNode? attributes;

        internal XmlEmptyElementSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken LessThanToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlEmptyElementSyntax)this.Green).lessThanToken, Position, 0);

        public XmlNameSyntax Name => GetRed(ref this.name, 1)!;

        public SyntaxList<XmlAttributeSyntax> Attributes => new SyntaxList<XmlAttributeSyntax>(GetRed(ref this.attributes, 2));

        public SyntaxToken SlashGreaterThanToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlEmptyElementSyntax)this.Green).slashGreaterThanToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                1 => GetRed(ref this.name, 1)!,
                2 => GetRed(ref this.attributes, 2)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                1 => this.name,
                2 => this.attributes,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitXmlEmptyElement(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlEmptyElement(this);

        public XmlEmptyElementSyntax Update(SyntaxToken lessThanToken, XmlNameSyntax name, SyntaxList<XmlAttributeSyntax> attributes, SyntaxToken slashGreaterThanToken)
        {
            if (lessThanToken != this.LessThanToken || name != this.Name || attributes != this.Attributes || slashGreaterThanToken != this.SlashGreaterThanToken)
            {
                var newNode = SyntaxFactory.XmlEmptyElement(lessThanToken, name, attributes, slashGreaterThanToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public XmlEmptyElementSyntax WithLessThanToken(SyntaxToken lessThanToken) => Update(lessThanToken, this.Name, this.Attributes, this.SlashGreaterThanToken);
        public XmlEmptyElementSyntax WithName(XmlNameSyntax name) => Update(this.LessThanToken, name, this.Attributes, this.SlashGreaterThanToken);
        public XmlEmptyElementSyntax WithAttributes(SyntaxList<XmlAttributeSyntax> attributes) => Update(this.LessThanToken, this.Name, attributes, this.SlashGreaterThanToken);
        public XmlEmptyElementSyntax WithSlashGreaterThanToken(SyntaxToken slashGreaterThanToken) => Update(this.LessThanToken, this.Name, this.Attributes, slashGreaterThanToken);

        public XmlEmptyElementSyntax AddAttributes(params XmlAttributeSyntax[] items) => WithAttributes(this.Attributes.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.XmlName"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class XmlNameSyntax : CSharpSyntaxNode
    {
        private XmlPrefixSyntax? prefix;

        internal XmlNameSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public XmlPrefixSyntax? Prefix => GetRedAtZero(ref this.prefix);

        public SyntaxToken LocalName => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlNameSyntax)this.Green).localName, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.prefix) : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.prefix : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitXmlName(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlName(this);

        public XmlNameSyntax Update(XmlPrefixSyntax? prefix, SyntaxToken localName)
        {
            if (prefix != this.Prefix || localName != this.LocalName)
            {
                var newNode = SyntaxFactory.XmlName(prefix, localName);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public XmlNameSyntax WithPrefix(XmlPrefixSyntax? prefix) => Update(prefix, this.LocalName);
        public XmlNameSyntax WithLocalName(SyntaxToken localName) => Update(this.Prefix, localName);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.XmlPrefix"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class XmlPrefixSyntax : CSharpSyntaxNode
    {

        internal XmlPrefixSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken Prefix => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlPrefixSyntax)this.Green).prefix, Position, 0);

        public SyntaxToken ColonToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlPrefixSyntax)this.Green).colonToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitXmlPrefix(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlPrefix(this);

        public XmlPrefixSyntax Update(SyntaxToken prefix, SyntaxToken colonToken)
        {
            if (prefix != this.Prefix || colonToken != this.ColonToken)
            {
                var newNode = SyntaxFactory.XmlPrefix(prefix, colonToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public XmlPrefixSyntax WithPrefix(SyntaxToken prefix) => Update(prefix, this.ColonToken);
        public XmlPrefixSyntax WithColonToken(SyntaxToken colonToken) => Update(this.Prefix, colonToken);
    }

    public abstract partial class XmlAttributeSyntax : CSharpSyntaxNode
    {
        internal XmlAttributeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public abstract XmlNameSyntax Name { get; }
        public XmlAttributeSyntax WithName(XmlNameSyntax name) => WithNameCore(name);
        internal abstract XmlAttributeSyntax WithNameCore(XmlNameSyntax name);

        public abstract SyntaxToken EqualsToken { get; }
        public XmlAttributeSyntax WithEqualsToken(SyntaxToken equalsToken) => WithEqualsTokenCore(equalsToken);
        internal abstract XmlAttributeSyntax WithEqualsTokenCore(SyntaxToken equalsToken);

        public abstract SyntaxToken StartQuoteToken { get; }
        public XmlAttributeSyntax WithStartQuoteToken(SyntaxToken startQuoteToken) => WithStartQuoteTokenCore(startQuoteToken);
        internal abstract XmlAttributeSyntax WithStartQuoteTokenCore(SyntaxToken startQuoteToken);

        public abstract SyntaxToken EndQuoteToken { get; }
        public XmlAttributeSyntax WithEndQuoteToken(SyntaxToken endQuoteToken) => WithEndQuoteTokenCore(endQuoteToken);
        internal abstract XmlAttributeSyntax WithEndQuoteTokenCore(SyntaxToken endQuoteToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.XmlTextAttribute"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class XmlTextAttributeSyntax : XmlAttributeSyntax
    {
        private XmlNameSyntax? name;

        internal XmlTextAttributeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override XmlNameSyntax Name => GetRedAtZero(ref this.name)!;

        public override SyntaxToken EqualsToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlTextAttributeSyntax)this.Green).equalsToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken StartQuoteToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlTextAttributeSyntax)this.Green).startQuoteToken, GetChildPosition(2), GetChildIndex(2));

        public SyntaxTokenList TextTokens
        {
            get
            {
                var slot = this.Green.GetSlot(3);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(3), GetChildIndex(3)) : default;
            }
        }

        public override SyntaxToken EndQuoteToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlTextAttributeSyntax)this.Green).endQuoteToken, GetChildPosition(4), GetChildIndex(4));

        public override SyntaxNode? GetNodeSlot(int index) => index == 0 ? GetRedAtZero(ref this.name)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 0 ? this.name : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitXmlTextAttribute(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlTextAttribute(this);

        public XmlTextAttributeSyntax Update(XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, SyntaxTokenList textTokens, SyntaxToken endQuoteToken)
        {
            if (name != this.Name || equalsToken != this.EqualsToken || startQuoteToken != this.StartQuoteToken || textTokens != this.TextTokens || endQuoteToken != this.EndQuoteToken)
            {
                var newNode = SyntaxFactory.XmlTextAttribute(name, equalsToken, startQuoteToken, textTokens, endQuoteToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override XmlAttributeSyntax WithNameCore(XmlNameSyntax name) => WithName(name);
        public new XmlTextAttributeSyntax WithName(XmlNameSyntax name) => Update(name, this.EqualsToken, this.StartQuoteToken, this.TextTokens, this.EndQuoteToken);
        internal override XmlAttributeSyntax WithEqualsTokenCore(SyntaxToken equalsToken) => WithEqualsToken(equalsToken);
        public new XmlTextAttributeSyntax WithEqualsToken(SyntaxToken equalsToken) => Update(this.Name, equalsToken, this.StartQuoteToken, this.TextTokens, this.EndQuoteToken);
        internal override XmlAttributeSyntax WithStartQuoteTokenCore(SyntaxToken startQuoteToken) => WithStartQuoteToken(startQuoteToken);
        public new XmlTextAttributeSyntax WithStartQuoteToken(SyntaxToken startQuoteToken) => Update(this.Name, this.EqualsToken, startQuoteToken, this.TextTokens, this.EndQuoteToken);
        public XmlTextAttributeSyntax WithTextTokens(SyntaxTokenList textTokens) => Update(this.Name, this.EqualsToken, this.StartQuoteToken, textTokens, this.EndQuoteToken);
        internal override XmlAttributeSyntax WithEndQuoteTokenCore(SyntaxToken endQuoteToken) => WithEndQuoteToken(endQuoteToken);
        public new XmlTextAttributeSyntax WithEndQuoteToken(SyntaxToken endQuoteToken) => Update(this.Name, this.EqualsToken, this.StartQuoteToken, this.TextTokens, endQuoteToken);

        public XmlTextAttributeSyntax AddTextTokens(params SyntaxToken[] items) => WithTextTokens(this.TextTokens.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.XmlCrefAttribute"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class XmlCrefAttributeSyntax : XmlAttributeSyntax
    {
        private XmlNameSyntax? name;
        private CrefSyntax? cref;

        internal XmlCrefAttributeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override XmlNameSyntax Name => GetRedAtZero(ref this.name)!;

        public override SyntaxToken EqualsToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlCrefAttributeSyntax)this.Green).equalsToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken StartQuoteToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlCrefAttributeSyntax)this.Green).startQuoteToken, GetChildPosition(2), GetChildIndex(2));

        public CrefSyntax Cref => GetRed(ref this.cref, 3)!;

        public override SyntaxToken EndQuoteToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlCrefAttributeSyntax)this.Green).endQuoteToken, GetChildPosition(4), GetChildIndex(4));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.name)!,
                3 => GetRed(ref this.cref, 3)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.name,
                3 => this.cref,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitXmlCrefAttribute(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlCrefAttribute(this);

        public XmlCrefAttributeSyntax Update(XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, CrefSyntax cref, SyntaxToken endQuoteToken)
        {
            if (name != this.Name || equalsToken != this.EqualsToken || startQuoteToken != this.StartQuoteToken || cref != this.Cref || endQuoteToken != this.EndQuoteToken)
            {
                var newNode = SyntaxFactory.XmlCrefAttribute(name, equalsToken, startQuoteToken, cref, endQuoteToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override XmlAttributeSyntax WithNameCore(XmlNameSyntax name) => WithName(name);
        public new XmlCrefAttributeSyntax WithName(XmlNameSyntax name) => Update(name, this.EqualsToken, this.StartQuoteToken, this.Cref, this.EndQuoteToken);
        internal override XmlAttributeSyntax WithEqualsTokenCore(SyntaxToken equalsToken) => WithEqualsToken(equalsToken);
        public new XmlCrefAttributeSyntax WithEqualsToken(SyntaxToken equalsToken) => Update(this.Name, equalsToken, this.StartQuoteToken, this.Cref, this.EndQuoteToken);
        internal override XmlAttributeSyntax WithStartQuoteTokenCore(SyntaxToken startQuoteToken) => WithStartQuoteToken(startQuoteToken);
        public new XmlCrefAttributeSyntax WithStartQuoteToken(SyntaxToken startQuoteToken) => Update(this.Name, this.EqualsToken, startQuoteToken, this.Cref, this.EndQuoteToken);
        public XmlCrefAttributeSyntax WithCref(CrefSyntax cref) => Update(this.Name, this.EqualsToken, this.StartQuoteToken, cref, this.EndQuoteToken);
        internal override XmlAttributeSyntax WithEndQuoteTokenCore(SyntaxToken endQuoteToken) => WithEndQuoteToken(endQuoteToken);
        public new XmlCrefAttributeSyntax WithEndQuoteToken(SyntaxToken endQuoteToken) => Update(this.Name, this.EqualsToken, this.StartQuoteToken, this.Cref, endQuoteToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.XmlNameAttribute"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class XmlNameAttributeSyntax : XmlAttributeSyntax
    {
        private XmlNameSyntax? name;
        private IdentifierNameSyntax? identifier;

        internal XmlNameAttributeSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override XmlNameSyntax Name => GetRedAtZero(ref this.name)!;

        public override SyntaxToken EqualsToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlNameAttributeSyntax)this.Green).equalsToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken StartQuoteToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlNameAttributeSyntax)this.Green).startQuoteToken, GetChildPosition(2), GetChildIndex(2));

        public IdentifierNameSyntax Identifier => GetRed(ref this.identifier, 3)!;

        public override SyntaxToken EndQuoteToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlNameAttributeSyntax)this.Green).endQuoteToken, GetChildPosition(4), GetChildIndex(4));

        public override SyntaxNode? GetNodeSlot(int index)
            => index switch
            {
                0 => GetRedAtZero(ref this.name)!,
                3 => GetRed(ref this.identifier, 3)!,
                _ => null,
            };

        public override SyntaxNode? GetCachedSlot(int index)
            => index switch
            {
                0 => this.name,
                3 => this.identifier,
                _ => null,
            };

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitXmlNameAttribute(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlNameAttribute(this);

        public XmlNameAttributeSyntax Update(XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, IdentifierNameSyntax identifier, SyntaxToken endQuoteToken)
        {
            if (name != this.Name || equalsToken != this.EqualsToken || startQuoteToken != this.StartQuoteToken || identifier != this.Identifier || endQuoteToken != this.EndQuoteToken)
            {
                var newNode = SyntaxFactory.XmlNameAttribute(name, equalsToken, startQuoteToken, identifier, endQuoteToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override XmlAttributeSyntax WithNameCore(XmlNameSyntax name) => WithName(name);
        public new XmlNameAttributeSyntax WithName(XmlNameSyntax name) => Update(name, this.EqualsToken, this.StartQuoteToken, this.Identifier, this.EndQuoteToken);
        internal override XmlAttributeSyntax WithEqualsTokenCore(SyntaxToken equalsToken) => WithEqualsToken(equalsToken);
        public new XmlNameAttributeSyntax WithEqualsToken(SyntaxToken equalsToken) => Update(this.Name, equalsToken, this.StartQuoteToken, this.Identifier, this.EndQuoteToken);
        internal override XmlAttributeSyntax WithStartQuoteTokenCore(SyntaxToken startQuoteToken) => WithStartQuoteToken(startQuoteToken);
        public new XmlNameAttributeSyntax WithStartQuoteToken(SyntaxToken startQuoteToken) => Update(this.Name, this.EqualsToken, startQuoteToken, this.Identifier, this.EndQuoteToken);
        public XmlNameAttributeSyntax WithIdentifier(IdentifierNameSyntax identifier) => Update(this.Name, this.EqualsToken, this.StartQuoteToken, identifier, this.EndQuoteToken);
        internal override XmlAttributeSyntax WithEndQuoteTokenCore(SyntaxToken endQuoteToken) => WithEndQuoteToken(endQuoteToken);
        public new XmlNameAttributeSyntax WithEndQuoteToken(SyntaxToken endQuoteToken) => Update(this.Name, this.EqualsToken, this.StartQuoteToken, this.Identifier, endQuoteToken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.XmlText"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class XmlTextSyntax : XmlNodeSyntax
    {

        internal XmlTextSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxTokenList TextTokens
        {
            get
            {
                var slot = this.Green.GetSlot(0);
                return slot != null ? new SyntaxTokenList(this, slot, Position, 0) : default;
            }
        }

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitXmlText(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlText(this);

        public XmlTextSyntax Update(SyntaxTokenList textTokens)
        {
            if (textTokens != this.TextTokens)
            {
                var newNode = SyntaxFactory.XmlText(textTokens);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public XmlTextSyntax WithTextTokens(SyntaxTokenList textTokens) => Update(textTokens);

        public XmlTextSyntax AddTextTokens(params SyntaxToken[] items) => WithTextTokens(this.TextTokens.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.XmlCDataSection"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class XmlCDataSectionSyntax : XmlNodeSyntax
    {

        internal XmlCDataSectionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken StartCDataToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlCDataSectionSyntax)this.Green).startCDataToken, Position, 0);

        public SyntaxTokenList TextTokens
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public SyntaxToken EndCDataToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlCDataSectionSyntax)this.Green).endCDataToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitXmlCDataSection(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlCDataSection(this);

        public XmlCDataSectionSyntax Update(SyntaxToken startCDataToken, SyntaxTokenList textTokens, SyntaxToken endCDataToken)
        {
            if (startCDataToken != this.StartCDataToken || textTokens != this.TextTokens || endCDataToken != this.EndCDataToken)
            {
                var newNode = SyntaxFactory.XmlCDataSection(startCDataToken, textTokens, endCDataToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public XmlCDataSectionSyntax WithStartCDataToken(SyntaxToken startCDataToken) => Update(startCDataToken, this.TextTokens, this.EndCDataToken);
        public XmlCDataSectionSyntax WithTextTokens(SyntaxTokenList textTokens) => Update(this.StartCDataToken, textTokens, this.EndCDataToken);
        public XmlCDataSectionSyntax WithEndCDataToken(SyntaxToken endCDataToken) => Update(this.StartCDataToken, this.TextTokens, endCDataToken);

        public XmlCDataSectionSyntax AddTextTokens(params SyntaxToken[] items) => WithTextTokens(this.TextTokens.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.XmlProcessingInstruction"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class XmlProcessingInstructionSyntax : XmlNodeSyntax
    {
        private XmlNameSyntax? name;

        internal XmlProcessingInstructionSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken StartProcessingInstructionToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlProcessingInstructionSyntax)this.Green).startProcessingInstructionToken, Position, 0);

        public XmlNameSyntax Name => GetRed(ref this.name, 1)!;

        public SyntaxTokenList TextTokens
        {
            get
            {
                var slot = this.Green.GetSlot(2);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(2), GetChildIndex(2)) : default;
            }
        }

        public SyntaxToken EndProcessingInstructionToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlProcessingInstructionSyntax)this.Green).endProcessingInstructionToken, GetChildPosition(3), GetChildIndex(3));

        public override SyntaxNode? GetNodeSlot(int index) => index == 1 ? GetRed(ref this.name, 1)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 1 ? this.name : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitXmlProcessingInstruction(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlProcessingInstruction(this);

        public XmlProcessingInstructionSyntax Update(SyntaxToken startProcessingInstructionToken, XmlNameSyntax name, SyntaxTokenList textTokens, SyntaxToken endProcessingInstructionToken)
        {
            if (startProcessingInstructionToken != this.StartProcessingInstructionToken || name != this.Name || textTokens != this.TextTokens || endProcessingInstructionToken != this.EndProcessingInstructionToken)
            {
                var newNode = SyntaxFactory.XmlProcessingInstruction(startProcessingInstructionToken, name, textTokens, endProcessingInstructionToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public XmlProcessingInstructionSyntax WithStartProcessingInstructionToken(SyntaxToken startProcessingInstructionToken) => Update(startProcessingInstructionToken, this.Name, this.TextTokens, this.EndProcessingInstructionToken);
        public XmlProcessingInstructionSyntax WithName(XmlNameSyntax name) => Update(this.StartProcessingInstructionToken, name, this.TextTokens, this.EndProcessingInstructionToken);
        public XmlProcessingInstructionSyntax WithTextTokens(SyntaxTokenList textTokens) => Update(this.StartProcessingInstructionToken, this.Name, textTokens, this.EndProcessingInstructionToken);
        public XmlProcessingInstructionSyntax WithEndProcessingInstructionToken(SyntaxToken endProcessingInstructionToken) => Update(this.StartProcessingInstructionToken, this.Name, this.TextTokens, endProcessingInstructionToken);

        public XmlProcessingInstructionSyntax AddTextTokens(params SyntaxToken[] items) => WithTextTokens(this.TextTokens.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.XmlComment"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class XmlCommentSyntax : XmlNodeSyntax
    {

        internal XmlCommentSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public SyntaxToken LessThanExclamationMinusMinusToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlCommentSyntax)this.Green).lessThanExclamationMinusMinusToken, Position, 0);

        public SyntaxTokenList TextTokens
        {
            get
            {
                var slot = this.Green.GetSlot(1);
                return slot != null ? new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1)) : default;
            }
        }

        public SyntaxToken MinusMinusGreaterThanToken => new SyntaxToken(this, ((Syntax.InternalSyntax.XmlCommentSyntax)this.Green).minusMinusGreaterThanToken, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitXmlComment(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlComment(this);

        public XmlCommentSyntax Update(SyntaxToken lessThanExclamationMinusMinusToken, SyntaxTokenList textTokens, SyntaxToken minusMinusGreaterThanToken)
        {
            if (lessThanExclamationMinusMinusToken != this.LessThanExclamationMinusMinusToken || textTokens != this.TextTokens || minusMinusGreaterThanToken != this.MinusMinusGreaterThanToken)
            {
                var newNode = SyntaxFactory.XmlComment(lessThanExclamationMinusMinusToken, textTokens, minusMinusGreaterThanToken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        public XmlCommentSyntax WithLessThanExclamationMinusMinusToken(SyntaxToken lessThanExclamationMinusMinusToken) => Update(lessThanExclamationMinusMinusToken, this.TextTokens, this.MinusMinusGreaterThanToken);
        public XmlCommentSyntax WithTextTokens(SyntaxTokenList textTokens) => Update(this.LessThanExclamationMinusMinusToken, textTokens, this.MinusMinusGreaterThanToken);
        public XmlCommentSyntax WithMinusMinusGreaterThanToken(SyntaxToken minusMinusGreaterThanToken) => Update(this.LessThanExclamationMinusMinusToken, this.TextTokens, minusMinusGreaterThanToken);

        public XmlCommentSyntax AddTextTokens(params SyntaxToken[] items) => WithTextTokens(this.TextTokens.AddRange(items));
    }

    public abstract partial class DirectiveTriviaSyntax : StructuredTriviaSyntax
    {
        internal DirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public abstract SyntaxToken HashToken { get; }
        public DirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => WithHashTokenCore(hashToken);
        internal abstract DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken);

        public abstract SyntaxToken EndOfDirectiveToken { get; }
        public DirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveTokenCore(endOfDirectiveToken);
        internal abstract DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken);

        public abstract bool IsActive { get; }
    }

    public abstract partial class BranchingDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        internal BranchingDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public abstract bool BranchTaken { get; }

        public new BranchingDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => (BranchingDirectiveTriviaSyntax)WithHashTokenCore(hashToken);
        public new BranchingDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => (BranchingDirectiveTriviaSyntax)WithEndOfDirectiveTokenCore(endOfDirectiveToken);
    }

    public abstract partial class ConditionalDirectiveTriviaSyntax : BranchingDirectiveTriviaSyntax
    {
        internal ConditionalDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public abstract ExpressionSyntax Condition { get; }
        public ConditionalDirectiveTriviaSyntax WithCondition(ExpressionSyntax condition) => WithConditionCore(condition);
        internal abstract ConditionalDirectiveTriviaSyntax WithConditionCore(ExpressionSyntax condition);

        public abstract bool ConditionValue { get; }
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.IfDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class IfDirectiveTriviaSyntax : ConditionalDirectiveTriviaSyntax
    {
        private ExpressionSyntax? condition;

        internal IfDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.IfDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken IfKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.IfDirectiveTriviaSyntax)this.Green).ifKeyword, GetChildPosition(1), GetChildIndex(1));

        public override ExpressionSyntax Condition => GetRed(ref this.condition, 2)!;

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.IfDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(3), GetChildIndex(3));

        public override bool IsActive => ((Syntax.InternalSyntax.IfDirectiveTriviaSyntax)this.Green).IsActive;

        public override bool BranchTaken => ((Syntax.InternalSyntax.IfDirectiveTriviaSyntax)this.Green).BranchTaken;

        public override bool ConditionValue => ((Syntax.InternalSyntax.IfDirectiveTriviaSyntax)this.Green).ConditionValue;

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.condition, 2)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.condition : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitIfDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitIfDirectiveTrivia(this);

        public IfDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken ifKeyword, ExpressionSyntax condition, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken, bool conditionValue)
        {
            if (hashToken != this.HashToken || ifKeyword != this.IfKeyword || condition != this.Condition || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.IfDirectiveTrivia(hashToken, ifKeyword, condition, endOfDirectiveToken, isActive, branchTaken, conditionValue);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new IfDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.IfKeyword, this.Condition, this.EndOfDirectiveToken, this.IsActive, this.BranchTaken, this.ConditionValue);
        public IfDirectiveTriviaSyntax WithIfKeyword(SyntaxToken ifKeyword) => Update(this.HashToken, ifKeyword, this.Condition, this.EndOfDirectiveToken, this.IsActive, this.BranchTaken, this.ConditionValue);
        internal override ConditionalDirectiveTriviaSyntax WithConditionCore(ExpressionSyntax condition) => WithCondition(condition);
        public new IfDirectiveTriviaSyntax WithCondition(ExpressionSyntax condition) => Update(this.HashToken, this.IfKeyword, condition, this.EndOfDirectiveToken, this.IsActive, this.BranchTaken, this.ConditionValue);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new IfDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.IfKeyword, this.Condition, endOfDirectiveToken, this.IsActive, this.BranchTaken, this.ConditionValue);
        public IfDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.IfKeyword, this.Condition, this.EndOfDirectiveToken, isActive, this.BranchTaken, this.ConditionValue);
        public IfDirectiveTriviaSyntax WithBranchTaken(bool branchTaken) => Update(this.HashToken, this.IfKeyword, this.Condition, this.EndOfDirectiveToken, this.IsActive, branchTaken, this.ConditionValue);
        public IfDirectiveTriviaSyntax WithConditionValue(bool conditionValue) => Update(this.HashToken, this.IfKeyword, this.Condition, this.EndOfDirectiveToken, this.IsActive, this.BranchTaken, conditionValue);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ElifDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ElifDirectiveTriviaSyntax : ConditionalDirectiveTriviaSyntax
    {
        private ExpressionSyntax? condition;

        internal ElifDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ElifDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken ElifKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ElifDirectiveTriviaSyntax)this.Green).elifKeyword, GetChildPosition(1), GetChildIndex(1));

        public override ExpressionSyntax Condition => GetRed(ref this.condition, 2)!;

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ElifDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(3), GetChildIndex(3));

        public override bool IsActive => ((Syntax.InternalSyntax.ElifDirectiveTriviaSyntax)this.Green).IsActive;

        public override bool BranchTaken => ((Syntax.InternalSyntax.ElifDirectiveTriviaSyntax)this.Green).BranchTaken;

        public override bool ConditionValue => ((Syntax.InternalSyntax.ElifDirectiveTriviaSyntax)this.Green).ConditionValue;

        public override SyntaxNode? GetNodeSlot(int index) => index == 2 ? GetRed(ref this.condition, 2)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 2 ? this.condition : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitElifDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitElifDirectiveTrivia(this);

        public ElifDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken elifKeyword, ExpressionSyntax condition, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken, bool conditionValue)
        {
            if (hashToken != this.HashToken || elifKeyword != this.ElifKeyword || condition != this.Condition || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.ElifDirectiveTrivia(hashToken, elifKeyword, condition, endOfDirectiveToken, isActive, branchTaken, conditionValue);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new ElifDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.ElifKeyword, this.Condition, this.EndOfDirectiveToken, this.IsActive, this.BranchTaken, this.ConditionValue);
        public ElifDirectiveTriviaSyntax WithElifKeyword(SyntaxToken elifKeyword) => Update(this.HashToken, elifKeyword, this.Condition, this.EndOfDirectiveToken, this.IsActive, this.BranchTaken, this.ConditionValue);
        internal override ConditionalDirectiveTriviaSyntax WithConditionCore(ExpressionSyntax condition) => WithCondition(condition);
        public new ElifDirectiveTriviaSyntax WithCondition(ExpressionSyntax condition) => Update(this.HashToken, this.ElifKeyword, condition, this.EndOfDirectiveToken, this.IsActive, this.BranchTaken, this.ConditionValue);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new ElifDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.ElifKeyword, this.Condition, endOfDirectiveToken, this.IsActive, this.BranchTaken, this.ConditionValue);
        public ElifDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.ElifKeyword, this.Condition, this.EndOfDirectiveToken, isActive, this.BranchTaken, this.ConditionValue);
        public ElifDirectiveTriviaSyntax WithBranchTaken(bool branchTaken) => Update(this.HashToken, this.ElifKeyword, this.Condition, this.EndOfDirectiveToken, this.IsActive, branchTaken, this.ConditionValue);
        public ElifDirectiveTriviaSyntax WithConditionValue(bool conditionValue) => Update(this.HashToken, this.ElifKeyword, this.Condition, this.EndOfDirectiveToken, this.IsActive, this.BranchTaken, conditionValue);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ElseDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ElseDirectiveTriviaSyntax : BranchingDirectiveTriviaSyntax
    {

        internal ElseDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ElseDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken ElseKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ElseDirectiveTriviaSyntax)this.Green).elseKeyword, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ElseDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(2), GetChildIndex(2));

        public override bool IsActive => ((Syntax.InternalSyntax.ElseDirectiveTriviaSyntax)this.Green).IsActive;

        public override bool BranchTaken => ((Syntax.InternalSyntax.ElseDirectiveTriviaSyntax)this.Green).BranchTaken;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitElseDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitElseDirectiveTrivia(this);

        public ElseDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken elseKeyword, SyntaxToken endOfDirectiveToken, bool isActive, bool branchTaken)
        {
            if (hashToken != this.HashToken || elseKeyword != this.ElseKeyword || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.ElseDirectiveTrivia(hashToken, elseKeyword, endOfDirectiveToken, isActive, branchTaken);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new ElseDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.ElseKeyword, this.EndOfDirectiveToken, this.IsActive, this.BranchTaken);
        public ElseDirectiveTriviaSyntax WithElseKeyword(SyntaxToken elseKeyword) => Update(this.HashToken, elseKeyword, this.EndOfDirectiveToken, this.IsActive, this.BranchTaken);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new ElseDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.ElseKeyword, endOfDirectiveToken, this.IsActive, this.BranchTaken);
        public ElseDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.ElseKeyword, this.EndOfDirectiveToken, isActive, this.BranchTaken);
        public ElseDirectiveTriviaSyntax WithBranchTaken(bool branchTaken) => Update(this.HashToken, this.ElseKeyword, this.EndOfDirectiveToken, this.IsActive, branchTaken);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.EndIfDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class EndIfDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal EndIfDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken EndIfKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax)this.Green).endIfKeyword, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(2), GetChildIndex(2));

        public override bool IsActive => ((Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitEndIfDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEndIfDirectiveTrivia(this);

        public EndIfDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken endIfKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || endIfKeyword != this.EndIfKeyword || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.EndIfDirectiveTrivia(hashToken, endIfKeyword, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new EndIfDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.EndIfKeyword, this.EndOfDirectiveToken, this.IsActive);
        public EndIfDirectiveTriviaSyntax WithEndIfKeyword(SyntaxToken endIfKeyword) => Update(this.HashToken, endIfKeyword, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new EndIfDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.EndIfKeyword, endOfDirectiveToken, this.IsActive);
        public EndIfDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.EndIfKeyword, this.EndOfDirectiveToken, isActive);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.RegionDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class RegionDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal RegionDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken RegionKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)this.Green).regionKeyword, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(2), GetChildIndex(2));

        public override bool IsActive => ((Syntax.InternalSyntax.RegionDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitRegionDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRegionDirectiveTrivia(this);

        public RegionDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken regionKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || regionKeyword != this.RegionKeyword || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.RegionDirectiveTrivia(hashToken, regionKeyword, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new RegionDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.RegionKeyword, this.EndOfDirectiveToken, this.IsActive);
        public RegionDirectiveTriviaSyntax WithRegionKeyword(SyntaxToken regionKeyword) => Update(this.HashToken, regionKeyword, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new RegionDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.RegionKeyword, endOfDirectiveToken, this.IsActive);
        public RegionDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.RegionKeyword, this.EndOfDirectiveToken, isActive);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.EndRegionDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class EndRegionDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal EndRegionDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken EndRegionKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax)this.Green).endRegionKeyword, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(2), GetChildIndex(2));

        public override bool IsActive => ((Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitEndRegionDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEndRegionDirectiveTrivia(this);

        public EndRegionDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken endRegionKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || endRegionKeyword != this.EndRegionKeyword || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.EndRegionDirectiveTrivia(hashToken, endRegionKeyword, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new EndRegionDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.EndRegionKeyword, this.EndOfDirectiveToken, this.IsActive);
        public EndRegionDirectiveTriviaSyntax WithEndRegionKeyword(SyntaxToken endRegionKeyword) => Update(this.HashToken, endRegionKeyword, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new EndRegionDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.EndRegionKeyword, endOfDirectiveToken, this.IsActive);
        public EndRegionDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.EndRegionKeyword, this.EndOfDirectiveToken, isActive);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ErrorDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ErrorDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal ErrorDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ErrorDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken ErrorKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ErrorDirectiveTriviaSyntax)this.Green).errorKeyword, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ErrorDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(2), GetChildIndex(2));

        public override bool IsActive => ((Syntax.InternalSyntax.ErrorDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitErrorDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitErrorDirectiveTrivia(this);

        public ErrorDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken errorKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || errorKeyword != this.ErrorKeyword || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.ErrorDirectiveTrivia(hashToken, errorKeyword, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new ErrorDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.ErrorKeyword, this.EndOfDirectiveToken, this.IsActive);
        public ErrorDirectiveTriviaSyntax WithErrorKeyword(SyntaxToken errorKeyword) => Update(this.HashToken, errorKeyword, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new ErrorDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.ErrorKeyword, endOfDirectiveToken, this.IsActive);
        public ErrorDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.ErrorKeyword, this.EndOfDirectiveToken, isActive);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.WarningDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class WarningDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal WarningDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.WarningDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken WarningKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.WarningDirectiveTriviaSyntax)this.Green).warningKeyword, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.WarningDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(2), GetChildIndex(2));

        public override bool IsActive => ((Syntax.InternalSyntax.WarningDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitWarningDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitWarningDirectiveTrivia(this);

        public WarningDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken warningKeyword, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || warningKeyword != this.WarningKeyword || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.WarningDirectiveTrivia(hashToken, warningKeyword, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new WarningDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.WarningKeyword, this.EndOfDirectiveToken, this.IsActive);
        public WarningDirectiveTriviaSyntax WithWarningKeyword(SyntaxToken warningKeyword) => Update(this.HashToken, warningKeyword, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new WarningDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.WarningKeyword, endOfDirectiveToken, this.IsActive);
        public WarningDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.WarningKeyword, this.EndOfDirectiveToken, isActive);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.BadDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class BadDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal BadDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.BadDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken Identifier => new SyntaxToken(this, ((Syntax.InternalSyntax.BadDirectiveTriviaSyntax)this.Green).identifier, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.BadDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(2), GetChildIndex(2));

        public override bool IsActive => ((Syntax.InternalSyntax.BadDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitBadDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBadDirectiveTrivia(this);

        public BadDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken identifier, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || identifier != this.Identifier || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.BadDirectiveTrivia(hashToken, identifier, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new BadDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.Identifier, this.EndOfDirectiveToken, this.IsActive);
        public BadDirectiveTriviaSyntax WithIdentifier(SyntaxToken identifier) => Update(this.HashToken, identifier, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new BadDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.Identifier, endOfDirectiveToken, this.IsActive);
        public BadDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.Identifier, this.EndOfDirectiveToken, isActive);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.DefineDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class DefineDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal DefineDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.DefineDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken DefineKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.DefineDirectiveTriviaSyntax)this.Green).defineKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken Name => new SyntaxToken(this, ((Syntax.InternalSyntax.DefineDirectiveTriviaSyntax)this.Green).name, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.DefineDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(3), GetChildIndex(3));

        public override bool IsActive => ((Syntax.InternalSyntax.DefineDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitDefineDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDefineDirectiveTrivia(this);

        public DefineDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken defineKeyword, SyntaxToken name, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || defineKeyword != this.DefineKeyword || name != this.Name || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.DefineDirectiveTrivia(hashToken, defineKeyword, name, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new DefineDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.DefineKeyword, this.Name, this.EndOfDirectiveToken, this.IsActive);
        public DefineDirectiveTriviaSyntax WithDefineKeyword(SyntaxToken defineKeyword) => Update(this.HashToken, defineKeyword, this.Name, this.EndOfDirectiveToken, this.IsActive);
        public DefineDirectiveTriviaSyntax WithName(SyntaxToken name) => Update(this.HashToken, this.DefineKeyword, name, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new DefineDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.DefineKeyword, this.Name, endOfDirectiveToken, this.IsActive);
        public DefineDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.DefineKeyword, this.Name, this.EndOfDirectiveToken, isActive);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.UndefDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class UndefDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal UndefDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.UndefDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken UndefKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.UndefDirectiveTriviaSyntax)this.Green).undefKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken Name => new SyntaxToken(this, ((Syntax.InternalSyntax.UndefDirectiveTriviaSyntax)this.Green).name, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.UndefDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(3), GetChildIndex(3));

        public override bool IsActive => ((Syntax.InternalSyntax.UndefDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitUndefDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitUndefDirectiveTrivia(this);

        public UndefDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken undefKeyword, SyntaxToken name, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || undefKeyword != this.UndefKeyword || name != this.Name || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.UndefDirectiveTrivia(hashToken, undefKeyword, name, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new UndefDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.UndefKeyword, this.Name, this.EndOfDirectiveToken, this.IsActive);
        public UndefDirectiveTriviaSyntax WithUndefKeyword(SyntaxToken undefKeyword) => Update(this.HashToken, undefKeyword, this.Name, this.EndOfDirectiveToken, this.IsActive);
        public UndefDirectiveTriviaSyntax WithName(SyntaxToken name) => Update(this.HashToken, this.UndefKeyword, name, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new UndefDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.UndefKeyword, this.Name, endOfDirectiveToken, this.IsActive);
        public UndefDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.UndefKeyword, this.Name, this.EndOfDirectiveToken, isActive);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.LineDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class LineDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal LineDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.LineDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken LineKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.LineDirectiveTriviaSyntax)this.Green).lineKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken Line => new SyntaxToken(this, ((Syntax.InternalSyntax.LineDirectiveTriviaSyntax)this.Green).line, GetChildPosition(2), GetChildIndex(2));

        public SyntaxToken File
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.LineDirectiveTriviaSyntax)this.Green).file;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(3), GetChildIndex(3)) : default;
            }
        }

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.LineDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(4), GetChildIndex(4));

        public override bool IsActive => ((Syntax.InternalSyntax.LineDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitLineDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLineDirectiveTrivia(this);

        public LineDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken lineKeyword, SyntaxToken line, SyntaxToken file, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || lineKeyword != this.LineKeyword || line != this.Line || file != this.File || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.LineDirectiveTrivia(hashToken, lineKeyword, line, file, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new LineDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.LineKeyword, this.Line, this.File, this.EndOfDirectiveToken, this.IsActive);
        public LineDirectiveTriviaSyntax WithLineKeyword(SyntaxToken lineKeyword) => Update(this.HashToken, lineKeyword, this.Line, this.File, this.EndOfDirectiveToken, this.IsActive);
        public LineDirectiveTriviaSyntax WithLine(SyntaxToken line) => Update(this.HashToken, this.LineKeyword, line, this.File, this.EndOfDirectiveToken, this.IsActive);
        public LineDirectiveTriviaSyntax WithFile(SyntaxToken file) => Update(this.HashToken, this.LineKeyword, this.Line, file, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new LineDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.LineKeyword, this.Line, this.File, endOfDirectiveToken, this.IsActive);
        public LineDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.LineKeyword, this.Line, this.File, this.EndOfDirectiveToken, isActive);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.PragmaWarningDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class PragmaWarningDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        private SyntaxNode? errorCodes;

        internal PragmaWarningDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.PragmaWarningDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken PragmaKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.PragmaWarningDirectiveTriviaSyntax)this.Green).pragmaKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken WarningKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.PragmaWarningDirectiveTriviaSyntax)this.Green).warningKeyword, GetChildPosition(2), GetChildIndex(2));

        public SyntaxToken DisableOrRestoreKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.PragmaWarningDirectiveTriviaSyntax)this.Green).disableOrRestoreKeyword, GetChildPosition(3), GetChildIndex(3));

        public SeparatedSyntaxList<ExpressionSyntax> ErrorCodes
        {
            get
            {
                var red = GetRed(ref this.errorCodes, 4);
                return red != null ? new SeparatedSyntaxList<ExpressionSyntax>(red, GetChildIndex(4)) : default;
            }
        }

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.PragmaWarningDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(5), GetChildIndex(5));

        public override bool IsActive => ((Syntax.InternalSyntax.PragmaWarningDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => index == 4 ? GetRed(ref this.errorCodes, 4)! : null;

        public override SyntaxNode? GetCachedSlot(int index) => index == 4 ? this.errorCodes : null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitPragmaWarningDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPragmaWarningDirectiveTrivia(this);

        public PragmaWarningDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken warningKeyword, SyntaxToken disableOrRestoreKeyword, SeparatedSyntaxList<ExpressionSyntax> errorCodes, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || pragmaKeyword != this.PragmaKeyword || warningKeyword != this.WarningKeyword || disableOrRestoreKeyword != this.DisableOrRestoreKeyword || errorCodes != this.ErrorCodes || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.PragmaWarningDirectiveTrivia(hashToken, pragmaKeyword, warningKeyword, disableOrRestoreKeyword, errorCodes, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new PragmaWarningDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.PragmaKeyword, this.WarningKeyword, this.DisableOrRestoreKeyword, this.ErrorCodes, this.EndOfDirectiveToken, this.IsActive);
        public PragmaWarningDirectiveTriviaSyntax WithPragmaKeyword(SyntaxToken pragmaKeyword) => Update(this.HashToken, pragmaKeyword, this.WarningKeyword, this.DisableOrRestoreKeyword, this.ErrorCodes, this.EndOfDirectiveToken, this.IsActive);
        public PragmaWarningDirectiveTriviaSyntax WithWarningKeyword(SyntaxToken warningKeyword) => Update(this.HashToken, this.PragmaKeyword, warningKeyword, this.DisableOrRestoreKeyword, this.ErrorCodes, this.EndOfDirectiveToken, this.IsActive);
        public PragmaWarningDirectiveTriviaSyntax WithDisableOrRestoreKeyword(SyntaxToken disableOrRestoreKeyword) => Update(this.HashToken, this.PragmaKeyword, this.WarningKeyword, disableOrRestoreKeyword, this.ErrorCodes, this.EndOfDirectiveToken, this.IsActive);
        public PragmaWarningDirectiveTriviaSyntax WithErrorCodes(SeparatedSyntaxList<ExpressionSyntax> errorCodes) => Update(this.HashToken, this.PragmaKeyword, this.WarningKeyword, this.DisableOrRestoreKeyword, errorCodes, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new PragmaWarningDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.PragmaKeyword, this.WarningKeyword, this.DisableOrRestoreKeyword, this.ErrorCodes, endOfDirectiveToken, this.IsActive);
        public PragmaWarningDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.PragmaKeyword, this.WarningKeyword, this.DisableOrRestoreKeyword, this.ErrorCodes, this.EndOfDirectiveToken, isActive);

        public PragmaWarningDirectiveTriviaSyntax AddErrorCodes(params ExpressionSyntax[] items) => WithErrorCodes(this.ErrorCodes.AddRange(items));
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.PragmaChecksumDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class PragmaChecksumDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal PragmaChecksumDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.PragmaChecksumDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken PragmaKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.PragmaChecksumDirectiveTriviaSyntax)this.Green).pragmaKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken ChecksumKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.PragmaChecksumDirectiveTriviaSyntax)this.Green).checksumKeyword, GetChildPosition(2), GetChildIndex(2));

        public SyntaxToken File => new SyntaxToken(this, ((Syntax.InternalSyntax.PragmaChecksumDirectiveTriviaSyntax)this.Green).file, GetChildPosition(3), GetChildIndex(3));

        public SyntaxToken Guid => new SyntaxToken(this, ((Syntax.InternalSyntax.PragmaChecksumDirectiveTriviaSyntax)this.Green).guid, GetChildPosition(4), GetChildIndex(4));

        public SyntaxToken Bytes => new SyntaxToken(this, ((Syntax.InternalSyntax.PragmaChecksumDirectiveTriviaSyntax)this.Green).bytes, GetChildPosition(5), GetChildIndex(5));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.PragmaChecksumDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(6), GetChildIndex(6));

        public override bool IsActive => ((Syntax.InternalSyntax.PragmaChecksumDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitPragmaChecksumDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPragmaChecksumDirectiveTrivia(this);

        public PragmaChecksumDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken pragmaKeyword, SyntaxToken checksumKeyword, SyntaxToken file, SyntaxToken guid, SyntaxToken bytes, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || pragmaKeyword != this.PragmaKeyword || checksumKeyword != this.ChecksumKeyword || file != this.File || guid != this.Guid || bytes != this.Bytes || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.PragmaChecksumDirectiveTrivia(hashToken, pragmaKeyword, checksumKeyword, file, guid, bytes, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new PragmaChecksumDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.PragmaKeyword, this.ChecksumKeyword, this.File, this.Guid, this.Bytes, this.EndOfDirectiveToken, this.IsActive);
        public PragmaChecksumDirectiveTriviaSyntax WithPragmaKeyword(SyntaxToken pragmaKeyword) => Update(this.HashToken, pragmaKeyword, this.ChecksumKeyword, this.File, this.Guid, this.Bytes, this.EndOfDirectiveToken, this.IsActive);
        public PragmaChecksumDirectiveTriviaSyntax WithChecksumKeyword(SyntaxToken checksumKeyword) => Update(this.HashToken, this.PragmaKeyword, checksumKeyword, this.File, this.Guid, this.Bytes, this.EndOfDirectiveToken, this.IsActive);
        public PragmaChecksumDirectiveTriviaSyntax WithFile(SyntaxToken file) => Update(this.HashToken, this.PragmaKeyword, this.ChecksumKeyword, file, this.Guid, this.Bytes, this.EndOfDirectiveToken, this.IsActive);
        public PragmaChecksumDirectiveTriviaSyntax WithGuid(SyntaxToken guid) => Update(this.HashToken, this.PragmaKeyword, this.ChecksumKeyword, this.File, guid, this.Bytes, this.EndOfDirectiveToken, this.IsActive);
        public PragmaChecksumDirectiveTriviaSyntax WithBytes(SyntaxToken bytes) => Update(this.HashToken, this.PragmaKeyword, this.ChecksumKeyword, this.File, this.Guid, bytes, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new PragmaChecksumDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.PragmaKeyword, this.ChecksumKeyword, this.File, this.Guid, this.Bytes, endOfDirectiveToken, this.IsActive);
        public PragmaChecksumDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.PragmaKeyword, this.ChecksumKeyword, this.File, this.Guid, this.Bytes, this.EndOfDirectiveToken, isActive);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ReferenceDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ReferenceDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal ReferenceDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken ReferenceKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax)this.Green).referenceKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken File => new SyntaxToken(this, ((Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax)this.Green).file, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(3), GetChildIndex(3));

        public override bool IsActive => ((Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitReferenceDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitReferenceDirectiveTrivia(this);

        public ReferenceDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken referenceKeyword, SyntaxToken file, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || referenceKeyword != this.ReferenceKeyword || file != this.File || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.ReferenceDirectiveTrivia(hashToken, referenceKeyword, file, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new ReferenceDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.ReferenceKeyword, this.File, this.EndOfDirectiveToken, this.IsActive);
        public ReferenceDirectiveTriviaSyntax WithReferenceKeyword(SyntaxToken referenceKeyword) => Update(this.HashToken, referenceKeyword, this.File, this.EndOfDirectiveToken, this.IsActive);
        public ReferenceDirectiveTriviaSyntax WithFile(SyntaxToken file) => Update(this.HashToken, this.ReferenceKeyword, file, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new ReferenceDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.ReferenceKeyword, this.File, endOfDirectiveToken, this.IsActive);
        public ReferenceDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.ReferenceKeyword, this.File, this.EndOfDirectiveToken, isActive);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.LoadDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class LoadDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal LoadDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.LoadDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken LoadKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.LoadDirectiveTriviaSyntax)this.Green).loadKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken File => new SyntaxToken(this, ((Syntax.InternalSyntax.LoadDirectiveTriviaSyntax)this.Green).file, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.LoadDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(3), GetChildIndex(3));

        public override bool IsActive => ((Syntax.InternalSyntax.LoadDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitLoadDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLoadDirectiveTrivia(this);

        public LoadDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken loadKeyword, SyntaxToken file, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || loadKeyword != this.LoadKeyword || file != this.File || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.LoadDirectiveTrivia(hashToken, loadKeyword, file, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new LoadDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.LoadKeyword, this.File, this.EndOfDirectiveToken, this.IsActive);
        public LoadDirectiveTriviaSyntax WithLoadKeyword(SyntaxToken loadKeyword) => Update(this.HashToken, loadKeyword, this.File, this.EndOfDirectiveToken, this.IsActive);
        public LoadDirectiveTriviaSyntax WithFile(SyntaxToken file) => Update(this.HashToken, this.LoadKeyword, file, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new LoadDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.LoadKeyword, this.File, endOfDirectiveToken, this.IsActive);
        public LoadDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.LoadKeyword, this.File, this.EndOfDirectiveToken, isActive);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.ShebangDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class ShebangDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal ShebangDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ShebangDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken ExclamationToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ShebangDirectiveTriviaSyntax)this.Green).exclamationToken, GetChildPosition(1), GetChildIndex(1));

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.ShebangDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(2), GetChildIndex(2));

        public override bool IsActive => ((Syntax.InternalSyntax.ShebangDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitShebangDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitShebangDirectiveTrivia(this);

        public ShebangDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken exclamationToken, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || exclamationToken != this.ExclamationToken || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.ShebangDirectiveTrivia(hashToken, exclamationToken, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new ShebangDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.ExclamationToken, this.EndOfDirectiveToken, this.IsActive);
        public ShebangDirectiveTriviaSyntax WithExclamationToken(SyntaxToken exclamationToken) => Update(this.HashToken, exclamationToken, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new ShebangDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.ExclamationToken, endOfDirectiveToken, this.IsActive);
        public ShebangDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.ExclamationToken, this.EndOfDirectiveToken, isActive);
    }

    /// <remarks>
    /// <para>This node is associated with the following syntax kinds:</para>
    /// <list type="bullet">
    /// <item><description><see cref="SyntaxKind.NullableDirectiveTrivia"/></description></item>
    /// </list>
    /// </remarks>
    public sealed partial class NullableDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {

        internal NullableDirectiveTriviaSyntax(InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
          : base(green, parent, position)
        {
        }

        public override SyntaxToken HashToken => new SyntaxToken(this, ((Syntax.InternalSyntax.NullableDirectiveTriviaSyntax)this.Green).hashToken, Position, 0);

        public SyntaxToken NullableKeyword => new SyntaxToken(this, ((Syntax.InternalSyntax.NullableDirectiveTriviaSyntax)this.Green).nullableKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken SettingToken => new SyntaxToken(this, ((Syntax.InternalSyntax.NullableDirectiveTriviaSyntax)this.Green).settingToken, GetChildPosition(2), GetChildIndex(2));

        public SyntaxToken TargetToken
        {
            get
            {
                var slot = ((Syntax.InternalSyntax.NullableDirectiveTriviaSyntax)this.Green).targetToken;
                return slot != null ? new SyntaxToken(this, slot, GetChildPosition(3), GetChildIndex(3)) : default;
            }
        }

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Syntax.InternalSyntax.NullableDirectiveTriviaSyntax)this.Green).endOfDirectiveToken, GetChildPosition(4), GetChildIndex(4));

        public override bool IsActive => ((Syntax.InternalSyntax.NullableDirectiveTriviaSyntax)this.Green).IsActive;

        public override SyntaxNode? GetNodeSlot(int index) => null;

        public override SyntaxNode? GetCachedSlot(int index) => null;

        public override void Accept(CSharpSyntaxVisitor visitor) => visitor.VisitNullableDirectiveTrivia(this);
        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitNullableDirectiveTrivia(this);

        public NullableDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken nullableKeyword, SyntaxToken settingToken, SyntaxToken targetToken, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != this.HashToken || nullableKeyword != this.NullableKeyword || settingToken != this.SettingToken || targetToken != this.TargetToken || endOfDirectiveToken != this.EndOfDirectiveToken)
            {
                var newNode = SyntaxFactory.NullableDirectiveTrivia(hashToken, nullableKeyword, settingToken, targetToken, endOfDirectiveToken, isActive);
                var annotations = GetAnnotations();
                return annotations?.Length > 0 ? newNode.WithAnnotations(annotations) : newNode;
            }

            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken) => WithHashToken(hashToken);
        public new NullableDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken) => Update(hashToken, this.NullableKeyword, this.SettingToken, this.TargetToken, this.EndOfDirectiveToken, this.IsActive);
        public NullableDirectiveTriviaSyntax WithNullableKeyword(SyntaxToken nullableKeyword) => Update(this.HashToken, nullableKeyword, this.SettingToken, this.TargetToken, this.EndOfDirectiveToken, this.IsActive);
        public NullableDirectiveTriviaSyntax WithSettingToken(SyntaxToken settingToken) => Update(this.HashToken, this.NullableKeyword, settingToken, this.TargetToken, this.EndOfDirectiveToken, this.IsActive);
        public NullableDirectiveTriviaSyntax WithTargetToken(SyntaxToken targetToken) => Update(this.HashToken, this.NullableKeyword, this.SettingToken, targetToken, this.EndOfDirectiveToken, this.IsActive);
        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken) => WithEndOfDirectiveToken(endOfDirectiveToken);
        public new NullableDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken) => Update(this.HashToken, this.NullableKeyword, this.SettingToken, this.TargetToken, endOfDirectiveToken, this.IsActive);
        public NullableDirectiveTriviaSyntax WithIsActive(bool isActive) => Update(this.HashToken, this.NullableKeyword, this.SettingToken, this.TargetToken, this.EndOfDirectiveToken, isActive);
    }
}
