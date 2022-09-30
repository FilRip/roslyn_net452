using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class SyntaxToken : VisualBasicSyntaxNode
	{
		internal class TriviaInfo : IObjectWritable
		{
			private const int s_maximumCachedTriviaWidth = 40;

			private const int s_triviaInfoCacheSize = 64;

			private static readonly Func<GreenNode, int> s_triviaKeyHasher;

			private static readonly Func<GreenNode, TriviaInfo, bool> s_triviaKeyEquality;

			private static readonly CachingFactory<GreenNode, TriviaInfo> s_triviaInfoCache;

			public GreenNode _leadingTrivia;

			public GreenNode _trailingTrivia;

			private bool IObjectWritable_ShouldReuseInSerialization => ShouldCacheTriviaInfo(_leadingTrivia, _trailingTrivia);

			static TriviaInfo()
			{
				s_triviaKeyHasher = (GreenNode key) => Hash.Combine(key.ToFullString(), (short)key.RawKind);
				s_triviaKeyEquality = (GreenNode key, TriviaInfo value) => key == value._leadingTrivia || (key.RawKind == value._leadingTrivia.RawKind && key.FullWidth == value._leadingTrivia.FullWidth && EmbeddedOperators.CompareString(key.ToFullString(), value._leadingTrivia.ToFullString(), TextCompare: false) == 0);
				s_triviaInfoCache = new CachingFactory<GreenNode, TriviaInfo>(64, null, s_triviaKeyHasher, s_triviaKeyEquality);
				ObjectBinder.RegisterTypeReader(typeof(TriviaInfo), (ObjectReader r) => new TriviaInfo(r));
			}

			private TriviaInfo(GreenNode leadingTrivia, GreenNode trailingTrivia)
			{
				_leadingTrivia = leadingTrivia;
				_trailingTrivia = trailingTrivia;
			}

			private static bool ShouldCacheTriviaInfo(GreenNode leadingTrivia, GreenNode trailingTrivia)
			{
				if (trailingTrivia == null)
				{
					return leadingTrivia.RawKind == 734 && leadingTrivia.Flags == NodeFlags.IsNotMissing && leadingTrivia.FullWidth <= 40;
				}
				return leadingTrivia.RawKind == 729 && leadingTrivia.Flags == NodeFlags.IsNotMissing && trailingTrivia.RawKind == 729 && trailingTrivia.Flags == NodeFlags.IsNotMissing && trailingTrivia.FullWidth == 1 && EmbeddedOperators.CompareString(trailingTrivia.ToFullString(), " ", TextCompare: false) == 0 && leadingTrivia.FullWidth <= 40;
			}

			public static TriviaInfo Create(GreenNode leadingTrivia, GreenNode trailingTrivia)
			{
				if (ShouldCacheTriviaInfo(leadingTrivia, trailingTrivia))
				{
					TriviaInfo value = null;
					lock (s_triviaInfoCache)
					{
						if (!s_triviaInfoCache.TryGetValue(leadingTrivia, out value))
						{
							value = new TriviaInfo(leadingTrivia, trailingTrivia);
							s_triviaInfoCache.Add(leadingTrivia, value);
						}
					}
					return value;
				}
				return new TriviaInfo(leadingTrivia, trailingTrivia);
			}

			public TriviaInfo(ObjectReader reader)
			{
				_leadingTrivia = (GreenNode)reader.ReadValue();
				_trailingTrivia = (GreenNode)reader.ReadValue();
			}

			public void WriteTo(ObjectWriter writer)
			{
				writer.WriteValue(_leadingTrivia);
				writer.WriteValue(_trailingTrivia);
			}

			void IObjectWritable.WriteTo(ObjectWriter writer)
			{
				//ILSpy generated this explicit interface implementation from .override directive in WriteTo
				this.WriteTo(writer);
			}
		}

		private readonly string _text;

		private readonly object _trailingTriviaOrTriviaInfo;

		internal override bool ShouldReuseInSerialization
		{
			get
			{
				if (base.ShouldReuseInSerialization)
				{
					return base.FullWidth < 42;
				}
				return false;
			}
		}

		internal string Text => _text;

		private int _leadingTriviaWidth
		{
			get
			{
				if (_trailingTriviaOrTriviaInfo is TriviaInfo triviaInfo)
				{
					return triviaInfo._leadingTrivia.FullWidth;
				}
				return 0;
			}
		}

		public sealed override bool IsToken => true;

		internal virtual bool IsKeyword => false;

		internal virtual object ObjectValue => ValueText;

		internal virtual string ValueText => Text;

		internal bool IsEndOfLine
		{
			get
			{
				if (base.Kind != SyntaxKind.StatementTerminatorToken)
				{
					return base.Kind == SyntaxKind.EndOfFileToken;
				}
				return true;
			}
		}

		internal bool IsEndOfParse => base.Kind == SyntaxKind.EndOfFileToken;

		protected SyntaxToken(SyntaxKind kind, string text, GreenNode precedingTrivia, GreenNode followingTrivia)
			: base(kind, text.Length)
		{
			SetFlags(NodeFlags.IsNotMissing);
			_text = text;
			if (followingTrivia != null)
			{
				AdjustFlagsAndWidth(followingTrivia);
				_trailingTriviaOrTriviaInfo = followingTrivia;
			}
			if (precedingTrivia != null)
			{
				AdjustFlagsAndWidth(precedingTrivia);
				_trailingTriviaOrTriviaInfo = TriviaInfo.Create(precedingTrivia, (GreenNode)_trailingTriviaOrTriviaInfo);
			}
			ClearFlagIfMissing();
		}

		protected SyntaxToken(SyntaxKind kind, DiagnosticInfo[] errors, string text, GreenNode precedingTrivia, GreenNode followingTrivia)
			: base(kind, errors, text.Length)
		{
			SetFlags(NodeFlags.IsNotMissing);
			_text = text;
			if (followingTrivia != null)
			{
				AdjustFlagsAndWidth(followingTrivia);
				_trailingTriviaOrTriviaInfo = followingTrivia;
			}
			if (precedingTrivia != null)
			{
				AdjustFlagsAndWidth(precedingTrivia);
				_trailingTriviaOrTriviaInfo = TriviaInfo.Create(precedingTrivia, (GreenNode)_trailingTriviaOrTriviaInfo);
			}
			ClearFlagIfMissing();
		}

		protected SyntaxToken(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode precedingTrivia, GreenNode followingTrivia)
			: base(kind, errors, annotations, text.Length)
		{
			SetFlags(NodeFlags.IsNotMissing);
			_text = text;
			if (followingTrivia != null)
			{
				AdjustFlagsAndWidth(followingTrivia);
				_trailingTriviaOrTriviaInfo = followingTrivia;
			}
			if (precedingTrivia != null)
			{
				AdjustFlagsAndWidth(precedingTrivia);
				_trailingTriviaOrTriviaInfo = TriviaInfo.Create(precedingTrivia, (GreenNode)_trailingTriviaOrTriviaInfo);
			}
			ClearFlagIfMissing();
		}

		internal SyntaxToken(ObjectReader reader)
			: base(reader)
		{
			SetFlags(NodeFlags.IsNotMissing);
			_text = reader.ReadString();
			base.FullWidth = _text.Length;
			_trailingTriviaOrTriviaInfo = RuntimeHelpers.GetObjectValue(reader.ReadValue());
			TriviaInfo triviaInfo = _trailingTriviaOrTriviaInfo as TriviaInfo;
			GreenNode greenNode = ((triviaInfo != null) ? triviaInfo._trailingTrivia : (_trailingTriviaOrTriviaInfo as GreenNode));
			GreenNode greenNode2 = triviaInfo?._leadingTrivia;
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
			}
			if (greenNode2 != null)
			{
				AdjustFlagsAndWidth(greenNode2);
			}
			ClearFlagIfMissing();
		}

		private void ClearFlagIfMissing()
		{
			if (Text.Length == 0 && base.Kind != SyntaxKind.EndOfFileToken && base.Kind != SyntaxKind.EmptyToken)
			{
				ClearFlags(NodeFlags.IsNotMissing);
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteString(_text);
			writer.WriteValue(RuntimeHelpers.GetObjectValue(_trailingTriviaOrTriviaInfo));
		}

		internal sealed override GreenNode GetSlot(int index)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal sealed override GreenNode GetLeadingTrivia()
		{
			if (_trailingTriviaOrTriviaInfo is TriviaInfo triviaInfo)
			{
				return triviaInfo._leadingTrivia;
			}
			return null;
		}

		public sealed override int GetLeadingTriviaWidth()
		{
			return _leadingTriviaWidth;
		}

		internal sealed override GreenNode GetTrailingTrivia()
		{
			if (_trailingTriviaOrTriviaInfo is GreenNode result)
			{
				return result;
			}
			if (_trailingTriviaOrTriviaInfo is TriviaInfo triviaInfo)
			{
				return triviaInfo._trailingTrivia;
			}
			return null;
		}

		public sealed override int GetTrailingTriviaWidth()
		{
			return base.FullWidth - _text.Length - _leadingTriviaWidth;
		}

		internal sealed override void AddSyntaxErrors(List<DiagnosticInfo> accumulatedErrors)
		{
			if (GetDiagnostics() != null)
			{
				accumulatedErrors.AddRange(GetDiagnostics());
			}
			GreenNode leadingTrivia = GetLeadingTrivia();
			if (leadingTrivia != null)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(leadingTrivia);
				int num = syntaxList.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					((VisualBasicSyntaxNode)syntaxList.ItemUntyped(i)).AddSyntaxErrors(accumulatedErrors);
				}
			}
			GreenNode trailingTrivia = GetTrailingTrivia();
			if (trailingTrivia != null)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList2 = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(trailingTrivia);
				int num2 = syntaxList2.Count - 1;
				for (int j = 0; j <= num2; j++)
				{
					((VisualBasicSyntaxNode)syntaxList2.ItemUntyped(j)).AddSyntaxErrors(accumulatedErrors);
				}
			}
		}

		protected override void WriteTokenTo(TextWriter writer, bool leading, bool trailing)
		{
			if (leading)
			{
				GetLeadingTrivia()?.WriteTo(writer, leading: true, trailing: true);
			}
			writer.Write(Text);
			if (trailing)
			{
				GetTrailingTrivia()?.WriteTo(writer, leading: true, trailing: true);
			}
		}

		public sealed override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSyntaxToken(this);
		}

		public override string ToString()
		{
			return _text;
		}

		public override object GetValue()
		{
			return ObjectValue;
		}

		public override string GetValueText()
		{
			return ValueText;
		}

		public bool IsBinaryOperator()
		{
			switch (base.Kind)
			{
			case SyntaxKind.AndKeyword:
			case SyntaxKind.AndAlsoKeyword:
			case SyntaxKind.IsKeyword:
			case SyntaxKind.IsNotKeyword:
			case SyntaxKind.LikeKeyword:
			case SyntaxKind.ModKeyword:
			case SyntaxKind.OrKeyword:
			case SyntaxKind.OrElseKeyword:
			case SyntaxKind.XorKeyword:
			case SyntaxKind.AmpersandToken:
			case SyntaxKind.AsteriskToken:
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
			case SyntaxKind.SlashToken:
			case SyntaxKind.LessThanToken:
			case SyntaxKind.LessThanEqualsToken:
			case SyntaxKind.LessThanGreaterThanToken:
			case SyntaxKind.EqualsToken:
			case SyntaxKind.GreaterThanToken:
			case SyntaxKind.GreaterThanEqualsToken:
			case SyntaxKind.BackslashToken:
			case SyntaxKind.CaretToken:
			case SyntaxKind.LessThanLessThanToken:
			case SyntaxKind.GreaterThanGreaterThanToken:
				return true;
			default:
				return false;
			}
		}

		public static T AddLeadingTrivia<T>(T token, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> newTrivia) where T : SyntaxToken
		{
			if (newTrivia.Node == null)
			{
				return token;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> nodes = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(token.GetLeadingTrivia());
			GreenNode node;
			if (nodes.Node == null)
			{
				node = newTrivia.Node;
			}
			else
			{
				SyntaxListBuilder<VisualBasicSyntaxNode> syntaxListBuilder = SyntaxListBuilder<VisualBasicSyntaxNode>.Create();
				syntaxListBuilder.AddRange(newTrivia);
				syntaxListBuilder.AddRange(nodes);
				node = syntaxListBuilder.ToList().Node;
			}
			return (T)token.WithLeadingTrivia(node);
		}

		public static T AddTrailingTrivia<T>(T token, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> newTrivia) where T : SyntaxToken
		{
			if (newTrivia.Node == null)
			{
				return token;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> nodes = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(token.GetTrailingTrivia());
			GreenNode node;
			if (nodes.Node == null)
			{
				node = newTrivia.Node;
			}
			else
			{
				SyntaxListBuilder<VisualBasicSyntaxNode> syntaxListBuilder = SyntaxListBuilder<VisualBasicSyntaxNode>.Create();
				syntaxListBuilder.AddRange(nodes);
				syntaxListBuilder.AddRange(newTrivia);
				node = syntaxListBuilder.ToList().Node;
			}
			return (T)token.WithTrailingTrivia(node);
		}

		internal static SyntaxToken Create(SyntaxKind kind, GreenNode leading = null, GreenNode trailing = null, string text = null)
		{
			string text2 = ((text == null) ? SyntaxFacts.GetText(kind) : text);
			if ((int)kind >= 413)
			{
				if ((int)kind <= 633 || kind == SyntaxKind.NameOfKeyword)
				{
					return new KeywordSyntax(kind, text2, leading, trailing);
				}
				if ((int)kind <= 692 || kind == SyntaxKind.EndOfInterpolatedStringToken || kind == SyntaxKind.DollarSignDoubleQuoteToken)
				{
					return new PunctuationSyntax(kind, text2, leading, trailing);
				}
			}
			throw ExceptionUtilities.UnexpectedValue(kind);
		}

		public static explicit operator Microsoft.CodeAnalysis.SyntaxToken(SyntaxToken token)
		{
			return new Microsoft.CodeAnalysis.SyntaxToken(null, token, 0, 0);
		}

		public override bool IsEquivalentTo(GreenNode other)
		{
			if (!base.IsEquivalentTo(other))
			{
				return false;
			}
			SyntaxToken syntaxToken = (SyntaxToken)other;
			if (!string.Equals(Text, syntaxToken.Text, StringComparison.Ordinal))
			{
				return false;
			}
			if (base.HasLeadingTrivia != syntaxToken.HasLeadingTrivia || base.HasTrailingTrivia != syntaxToken.HasTrailingTrivia)
			{
				return false;
			}
			if (base.HasLeadingTrivia && !GetLeadingTrivia().IsEquivalentTo(syntaxToken.GetLeadingTrivia()))
			{
				return false;
			}
			if (base.HasTrailingTrivia && !GetTrailingTrivia().IsEquivalentTo(syntaxToken.GetTrailingTrivia()))
			{
				return false;
			}
			return true;
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int position)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
