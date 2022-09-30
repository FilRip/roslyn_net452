using System;
using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundTupleExpression : BoundExpression
    {
        public override object Display
        {
            get
            {
                PooledStringBuilder instance = PooledStringBuilder.GetInstance();
                StringBuilder builder = instance.Builder;
                ImmutableArray<BoundExpression> arguments = Arguments;
                object[] array = new object[arguments.Length];
                builder.Append('(');
                builder.Append("{0}");
                array[0] = arguments[0].Display;
                for (int i = 1; i < arguments.Length; i++)
                {
                    builder.Append(", {" + i + "}");
                    array[i] = arguments[i].Display;
                }
                builder.Append(')');
                return instance.ToStringAndFree(); // FilRip modified for test
            }
        }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<string?> ArgumentNamesOpt { get; }

        public ImmutableArray<bool> InferredNamesOpt { get; }

        internal void VisitAllElements<T>(Action<BoundExpression, T> action, T args)
        {
            ImmutableArray<BoundExpression>.Enumerator enumerator = Arguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                if (current.Kind == BoundKind.TupleLiteral)
                {
                    ((BoundTupleExpression)current).VisitAllElements(action, args);
                }
                else
                {
                    action(current, args);
                }
            }
        }

        protected BoundTupleExpression(BoundKind kind, SyntaxNode syntax, ImmutableArray<BoundExpression> arguments, ImmutableArray<string?> argumentNamesOpt, ImmutableArray<bool> inferredNamesOpt, TypeSymbol? type, bool hasErrors = false)
            : base(kind, syntax, type, hasErrors)
        {
            Arguments = arguments;
            ArgumentNamesOpt = argumentNamesOpt;
            InferredNamesOpt = inferredNamesOpt;
        }
    }
}
