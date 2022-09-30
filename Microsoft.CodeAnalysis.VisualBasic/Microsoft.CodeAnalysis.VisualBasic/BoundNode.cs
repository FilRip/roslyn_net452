using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundNode : IBoundNodeWithIOperationChildren
	{
		[Flags]
		private enum BoundNodeAttributes : byte
		{
			HasErrors = 1,
			WasCompilerGenerated = 2
		}

		private readonly BoundKind _kind;

		private BoundNodeAttributes _attributes;

		private readonly SyntaxNode _syntax;

		public bool HasErrors => (_attributes & BoundNodeAttributes.HasErrors) != 0;

		public bool WasCompilerGenerated => (_attributes & BoundNodeAttributes.WasCompilerGenerated) != 0;

		public BoundKind Kind => _kind;

		public SyntaxNode Syntax => _syntax;

		public SyntaxTree SyntaxTree => (VisualBasicSyntaxTree)_syntax.SyntaxTree;

		public ImmutableArray<BoundNode> IBoundNodeWithIOperationChildren_Children => Children;

		protected virtual ImmutableArray<BoundNode> Children => ImmutableArray<BoundNode>.Empty;

		public BoundNode(BoundKind kind, SyntaxNode syntax)
		{
			_kind = kind;
			_syntax = syntax;
		}

		public BoundNode(BoundKind kind, SyntaxNode syntax, bool hasErrors)
			: this(kind, syntax)
		{
			if (hasErrors)
			{
				_attributes = BoundNodeAttributes.HasErrors;
			}
		}

		protected void CopyAttributes(BoundNode node)
		{
			if (node.WasCompilerGenerated)
			{
				SetWasCompilerGenerated();
			}
		}

		[Conditional("DEBUG")]
		private static void ValidateLocationInformation(BoundKind kind, SyntaxNode syntax)
		{
		}

		public void SetWasCompilerGenerated()
		{
			_attributes |= BoundNodeAttributes.WasCompilerGenerated;
		}

		public virtual BoundNode Accept(BoundTreeVisitor visitor)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
