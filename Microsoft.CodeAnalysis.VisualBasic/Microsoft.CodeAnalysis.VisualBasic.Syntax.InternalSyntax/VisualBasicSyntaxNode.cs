using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	[DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
	internal abstract class VisualBasicSyntaxNode : GreenNode
	{
		private static readonly ConditionalWeakTable<SyntaxNode, Dictionary<Microsoft.CodeAnalysis.SyntaxTrivia, SyntaxNode>> s_structuresTable = new ConditionalWeakTable<SyntaxNode, Dictionary<Microsoft.CodeAnalysis.SyntaxTrivia, SyntaxNode>>();

		internal SyntaxKind Kind => (SyntaxKind)base.RawKind;

		internal SyntaxKind ContextualKind => (SyntaxKind)RawContextualKind;

		public override string KindText => Kind.ToString();

		public override string Language => "Visual Basic";

		public override bool IsStructuredTrivia => this is StructuredTriviaSyntax;

		public override bool IsDirective => this is DirectiveTriviaSyntax;

		public override bool IsSkippedTokensTrivia => Kind == SyntaxKind.SkippedTokensTrivia;

		public override bool IsDocumentationCommentTrivia => Kind == SyntaxKind.DocumentationCommentTrivia;

		protected int _slotCount
		{
			get
			{
				return base.SlotCount;
			}
			set
			{
				base.SlotCount = value;
			}
		}

		public virtual VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitVisualBasicSyntaxNode(this);
		}

		protected void SetFactoryContext(ISyntaxFactoryContext context)
		{
			if (context.IsWithinAsyncMethodOrLambda)
			{
				SetFlags(NodeFlags.FactoryContextIsInAsync);
			}
			if (context.IsWithinIteratorContext)
			{
				SetFlags(NodeFlags.FactoryContextIsInQuery);
			}
		}

		internal bool MatchesFactoryContext(ISyntaxFactoryContext context)
		{
			if (context.IsWithinAsyncMethodOrLambda == base.ParsedInAsync)
			{
				return context.IsWithinIteratorContext == base.ParsedInIterator;
			}
			return false;
		}

		internal VisualBasicSyntaxNode(ObjectReader reader)
			: base(reader)
		{
		}

		protected override int GetSlotCount()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal SyntaxToken GetFirstToken()
		{
			return (SyntaxToken)GetFirstTerminal();
		}

		internal SyntaxToken GetLastToken()
		{
			return (SyntaxToken)GetLastTerminal();
		}

		internal virtual GreenNode GetLeadingTrivia()
		{
			return GetFirstToken()?.GetLeadingTrivia();
		}

		public override GreenNode GetLeadingTriviaCore()
		{
			return GetLeadingTrivia();
		}

		internal virtual GreenNode GetTrailingTrivia()
		{
			return GetLastToken()?.GetTrailingTrivia();
		}

		public override GreenNode GetTrailingTriviaCore()
		{
			return GetTrailingTrivia();
		}

		protected VisualBasicSyntaxNode(SyntaxKind kind)
			: base((ushort)kind)
		{
			GreenStats.NoteGreen(this);
		}

		protected VisualBasicSyntaxNode(SyntaxKind kind, int width)
			: base((ushort)kind, width)
		{
			GreenStats.NoteGreen(this);
		}

		protected VisualBasicSyntaxNode(SyntaxKind kind, DiagnosticInfo[] errors)
			: base((ushort)kind, errors)
		{
			GreenStats.NoteGreen(this);
		}

		protected VisualBasicSyntaxNode(SyntaxKind kind, DiagnosticInfo[] errors, int width)
			: base((ushort)kind, errors, width)
		{
			GreenStats.NoteGreen(this);
		}

		internal VisualBasicSyntaxNode(SyntaxKind kind, DiagnosticInfo[] diagnostics, SyntaxAnnotation[] annotations)
			: base((ushort)kind, diagnostics, annotations)
		{
			GreenStats.NoteGreen(this);
		}

		internal VisualBasicSyntaxNode(SyntaxKind kind, DiagnosticInfo[] diagnostics, SyntaxAnnotation[] annotations, int fullWidth)
			: base((ushort)kind, diagnostics, annotations, fullWidth)
		{
			GreenStats.NoteGreen(this);
		}

		internal virtual IList<DiagnosticInfo> GetSyntaxErrors()
		{
			if (!base.ContainsDiagnostics)
			{
				return null;
			}
			List<DiagnosticInfo> list = new List<DiagnosticInfo>();
			AddSyntaxErrors(list);
			return list;
		}

		internal virtual void AddSyntaxErrors(List<DiagnosticInfo> accumulatedErrors)
		{
			if (GetDiagnostics() != null)
			{
				accumulatedErrors.AddRange(GetDiagnostics());
			}
			int slotCount = base.SlotCount;
			if (slotCount == 0)
			{
				return;
			}
			int num = slotCount - 1;
			for (int i = 0; i <= num; i++)
			{
				GreenNode slot = GetSlot(i);
				if (slot != null && slot.ContainsDiagnostics)
				{
					((VisualBasicSyntaxNode)slot).AddSyntaxErrors(accumulatedErrors);
				}
			}
		}

		private string GetDebuggerDisplay()
		{
			string text = ToFullString();
			if (text.Length > 400)
			{
				text = text.Substring(0, 400);
			}
			return Kind.ToString() + ":" + text;
		}

		internal static bool IsEquivalentTo(VisualBasicSyntaxNode left, VisualBasicSyntaxNode right)
		{
			if (left == right)
			{
				return true;
			}
			if (left == null || right == null)
			{
				return false;
			}
			return left.IsEquivalentTo(right);
		}

		public override SyntaxNode GetStructure(Microsoft.CodeAnalysis.SyntaxTrivia trivia)
		{
			if (!trivia.HasStructure)
			{
				return null;
			}
			SyntaxNode parent = trivia.Token.Parent;
			if (parent == null)
			{
				return Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax.Create(trivia);
			}
			SyntaxNode value = null;
			Dictionary<Microsoft.CodeAnalysis.SyntaxTrivia, SyntaxNode> orCreateValue = s_structuresTable.GetOrCreateValue(parent);
			lock (orCreateValue)
			{
				if (!orCreateValue.TryGetValue(trivia, out value))
				{
					value = Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax.Create(trivia);
					orCreateValue.Add(trivia, value);
				}
			}
			return value;
		}

		public override Microsoft.CodeAnalysis.SyntaxToken CreateSeparator<TNode>(SyntaxNode element)
		{
			SyntaxKind kind = SyntaxKind.CommaToken;
			if (VisualBasicExtensions.Kind(element) == SyntaxKind.JoinCondition)
			{
				kind = SyntaxKind.AndKeyword;
			}
			return Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.Token(kind);
		}

		public override bool IsTriviaWithEndOfLine()
		{
			if (Kind != SyntaxKind.EndOfLineTrivia)
			{
				return Kind == SyntaxKind.CommentTrivia;
			}
			return true;
		}
	}
}
