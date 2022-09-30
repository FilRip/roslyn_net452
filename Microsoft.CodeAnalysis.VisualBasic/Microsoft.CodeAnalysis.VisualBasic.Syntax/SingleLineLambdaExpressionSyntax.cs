using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SingleLineLambdaExpressionSyntax : LambdaExpressionSyntax
	{
		internal VisualBasicSyntaxNode _body;

		public new LambdaHeaderSyntax SubOrFunctionHeader => GetRedAtZero(ref _subOrFunctionHeader);

		public VisualBasicSyntaxNode Body => GetRed(ref _body, 1);

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use SubOrFunctionHeader instead.", true)]
		public new LambdaHeaderSyntax Begin => SubOrFunctionHeader;

		internal SyntaxList<StatementSyntax> Statements => new SyntaxList<StatementSyntax>(Body);

		internal SingleLineLambdaExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SingleLineLambdaExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, LambdaHeaderSyntax subOrFunctionHeader, VisualBasicSyntaxNode body)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax)subOrFunctionHeader.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)body.Green), null, 0)
		{
		}

		internal override LambdaHeaderSyntax GetSubOrFunctionHeaderCore()
		{
			return SubOrFunctionHeader;
		}

		internal override LambdaExpressionSyntax WithSubOrFunctionHeaderCore(LambdaHeaderSyntax subOrFunctionHeader)
		{
			return WithSubOrFunctionHeader(subOrFunctionHeader);
		}

		public new SingleLineLambdaExpressionSyntax WithSubOrFunctionHeader(LambdaHeaderSyntax subOrFunctionHeader)
		{
			return Update(Kind(), subOrFunctionHeader, Body);
		}

		public SingleLineLambdaExpressionSyntax WithBody(VisualBasicSyntaxNode body)
		{
			return Update(Kind(), SubOrFunctionHeader, body);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _subOrFunctionHeader, 
				1 => _body, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => SubOrFunctionHeader, 
				1 => Body, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSingleLineLambdaExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSingleLineLambdaExpression(this);
		}

		public SingleLineLambdaExpressionSyntax Update(SyntaxKind kind, LambdaHeaderSyntax subOrFunctionHeader, VisualBasicSyntaxNode body)
		{
			if (kind != Kind() || subOrFunctionHeader != SubOrFunctionHeader || body != Body)
			{
				SingleLineLambdaExpressionSyntax singleLineLambdaExpressionSyntax = SyntaxFactory.SingleLineLambdaExpression(kind, subOrFunctionHeader, body);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(singleLineLambdaExpressionSyntax, annotations);
				}
				return singleLineLambdaExpressionSyntax;
			}
			return this;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use WithSubOrFunctionHeader instead.", true)]
		public SingleLineLambdaExpressionSyntax WithBegin(LambdaHeaderSyntax begin)
		{
			return WithSubOrFunctionHeader(begin);
		}
	}
}
