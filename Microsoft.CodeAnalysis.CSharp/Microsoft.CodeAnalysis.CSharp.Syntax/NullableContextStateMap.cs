using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    internal readonly struct NullableContextStateMap
    {
        private sealed class PositionComparer : IComparer<NullableContextState>
        {
            internal static readonly PositionComparer Instance = new PositionComparer();

            public int Compare(NullableContextState x, NullableContextState y)
            {
                return x.Position.CompareTo(y.Position);
            }
        }

        private readonly ImmutableArray<NullableContextState> _contexts;

        internal static NullableContextStateMap Create(SyntaxTree tree)
        {
            return new NullableContextStateMap(GetContexts(tree));
        }

        private NullableContextStateMap(ImmutableArray<NullableContextState> contexts)
        {
            _contexts = contexts;
        }

        private static NullableContextState GetContextForFileStart()
        {
            return new NullableContextState(0, NullableContextState.State.Unknown, NullableContextState.State.Unknown);
        }

        private int GetContextStateIndex(int position)
        {
            int num = ImmutableArray.BinarySearch(value: new NullableContextState(position, NullableContextState.State.Unknown, NullableContextState.State.Unknown), array: _contexts, comparer: PositionComparer.Instance);
            if (num < 0)
            {
                num = ~num - 1;
            }
            return num;
        }

        internal NullableContextState GetContextState(int position)
        {
            int contextStateIndex = GetContextStateIndex(position);
            if (contextStateIndex >= 0)
            {
                return _contexts[contextStateIndex];
            }
            return GetContextForFileStart();
        }

        internal bool? IsNullableAnalysisEnabled(TextSpan span)
        {
            bool flag = false;
            int num = GetContextStateIndex(span.Start);
            NullableContextState nullableContextState = ((num < 0) ? GetContextForFileStart() : _contexts[num]);
            do
            {
                switch (nullableContextState.WarningsState)
                {
                    case NullableContextState.State.Enabled:
                        return true;
                    case NullableContextState.State.Unknown:
                    case NullableContextState.State.ExplicitlyRestored:
                        flag = true;
                        break;
                }
                num++;
                if (num >= _contexts.Length)
                {
                    break;
                }
                nullableContextState = _contexts[num];
            }
            while (nullableContextState.Position < span.End);
            if (!flag)
            {
                return false;
            }
            return null;
        }

        private static ImmutableArray<NullableContextState> GetContexts(SyntaxTree tree)
        {
            NullableContextState nullableContextState = GetContextForFileStart();
            ArrayBuilder<NullableContextState> instance = ArrayBuilder<NullableContextState>.GetInstance();
            foreach (DirectiveTriviaSyntax directive in tree.GetRoot().GetDirectives())
            {
                if (directive.Kind() == SyntaxKind.NullableDirectiveTrivia)
                {
                    NullableDirectiveTriviaSyntax nullableDirectiveTriviaSyntax = (NullableDirectiveTriviaSyntax)directive;
                    if (!nullableDirectiveTriviaSyntax.SettingToken.IsMissing && nullableDirectiveTriviaSyntax.IsActive)
                    {
                        int endPosition = nullableDirectiveTriviaSyntax.EndPosition;
                        SyntaxKind syntaxKind = nullableDirectiveTriviaSyntax.SettingToken.Kind();
                        NullableContextState.State state = syntaxKind switch
                        {
                            SyntaxKind.EnableKeyword => NullableContextState.State.Enabled,
                            SyntaxKind.DisableKeyword => NullableContextState.State.Disabled,
                            SyntaxKind.RestoreKeyword => NullableContextState.State.ExplicitlyRestored,
                            _ => throw ExceptionUtilities.UnexpectedValue(syntaxKind),
                        };
                        SyntaxKind syntaxKind2 = nullableDirectiveTriviaSyntax.TargetToken.Kind();
                        NullableContextState nullableContextState2 = syntaxKind2 switch
                        {
                            SyntaxKind.None => new NullableContextState(endPosition, state, state),
                            SyntaxKind.WarningsKeyword => new NullableContextState(endPosition, state, nullableContextState.AnnotationsState),
                            SyntaxKind.AnnotationsKeyword => new NullableContextState(endPosition, nullableContextState.WarningsState, state),
                            _ => throw ExceptionUtilities.UnexpectedValue(syntaxKind2),
                        };
                        instance.Add(nullableContextState2);
                        nullableContextState = nullableContextState2;
                    }
                }
            }
            return instance.ToImmutableAndFree();
        }
    }
}
