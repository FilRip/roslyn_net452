using System;
using System.Collections.Generic;
using System.IO;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SyntaxTrivia : VisualBasicSyntaxNode
	{
		private readonly string _text;

		internal override bool ShouldReuseInSerialization
		{
			get
			{
				SyntaxKind kind = base.Kind;
				if (kind - 729 <= SyntaxKind.EmptyStatement || kind - 733 <= SyntaxKind.List)
				{
					return true;
				}
				return false;
			}
		}

		internal string Text => _text;

		public override bool IsTrivia => true;

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SyntaxTrivia(base.Kind, newErrors, GetAnnotations(), Text);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SyntaxTrivia(base.Kind, GetDiagnostics(), annotations, Text);
		}

		internal SyntaxTrivia(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text)
			: base(kind, errors, annotations, text.Length)
		{
			_text = text;
			if (text.Length > 0)
			{
				SetFlags(NodeFlags.IsNotMissing);
			}
		}

		internal SyntaxTrivia(SyntaxKind kind, string text, ISyntaxFactoryContext context)
			: this(kind, text)
		{
			SetFactoryContext(context);
		}

		internal SyntaxTrivia(SyntaxKind kind, string text)
			: base(kind, text.Length)
		{
			_text = text;
			if (text.Length > 0)
			{
				SetFlags(NodeFlags.IsNotMissing);
			}
		}

		internal SyntaxTrivia(ObjectReader reader)
			: base(reader)
		{
			_text = reader.ReadString();
			base.FullWidth = _text.Length;
			if (Text.Length > 0)
			{
				SetFlags(NodeFlags.IsNotMissing);
			}
		}

		static SyntaxTrivia()
		{
			ObjectBinder.RegisterTypeReader(typeof(SyntaxTrivia), (ObjectReader r) => new SyntaxTrivia(r));
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteString(_text);
		}

		internal sealed override GreenNode GetSlot(int index)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal sealed override GreenNode GetTrailingTrivia()
		{
			return null;
		}

		public sealed override int GetTrailingTriviaWidth()
		{
			return 0;
		}

		internal sealed override GreenNode GetLeadingTrivia()
		{
			return null;
		}

		public sealed override int GetLeadingTriviaWidth()
		{
			return 0;
		}

		protected override void WriteTriviaTo(TextWriter writer)
		{
			writer.Write(Text);
		}

		public sealed override string ToFullString()
		{
			return _text;
		}

		public override string ToString()
		{
			return _text;
		}

		internal sealed override void AddSyntaxErrors(List<DiagnosticInfo> accumulatedErrors)
		{
			if (GetDiagnostics() != null)
			{
				accumulatedErrors.AddRange(GetDiagnostics());
			}
		}

		public sealed override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSyntaxTrivia(this);
		}

		public static explicit operator Microsoft.CodeAnalysis.SyntaxTrivia(SyntaxTrivia trivia)
		{
			Microsoft.CodeAnalysis.SyntaxToken token = default(Microsoft.CodeAnalysis.SyntaxToken);
			return new Microsoft.CodeAnalysis.SyntaxTrivia(in token, trivia, 0, 0);
		}

		public override bool IsEquivalentTo(GreenNode other)
		{
			if (!base.IsEquivalentTo(other))
			{
				return false;
			}
			SyntaxTrivia syntaxTrivia = (SyntaxTrivia)other;
			return string.Equals(Text, syntaxTrivia.Text, StringComparison.Ordinal);
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int position)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
