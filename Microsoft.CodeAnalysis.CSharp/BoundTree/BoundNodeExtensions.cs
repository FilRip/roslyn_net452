// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static partial class BoundNodeExtensions
    {
        // Return if any node in an array of nodes has errors.
        public static bool HasErrors<T>(this ImmutableArray<T> nodeArray)
            where T : BoundNode
        {
            if (nodeArray.IsDefault)
                return false;

            for (int i = 0, n = nodeArray.Length; i < n; ++i)
            {
                if (nodeArray[i].HasErrors)
                    return true;
            }

            return false;
        }

        // Like HasErrors property, but also returns false for a null node. 
        public static bool HasErrors([NotNullWhen(true)] this BoundNode? node)
        {
            return node != null && node.HasErrors;
        }

        public static bool IsConstructorInitializer(this BoundStatement statement)
        {
            if (statement!.Kind == BoundKind.ExpressionStatement)
            {
                BoundExpression expression = ((BoundExpressionStatement)statement).Expression;
                if (expression.Kind == BoundKind.Sequence && ((BoundSequence)expression).SideEffects.IsDefaultOrEmpty)
                {
                    // in case there is a pattern variable declared in a ctor-initializer, it gets wrapped in a bound sequence.
                    expression = ((BoundSequence)expression).Value;
                }

                return expression.Kind == BoundKind.Call && ((BoundCall)expression).IsConstructorInitializer();
            }

            return false;
        }

        public static bool IsConstructorInitializer(this BoundCall call)
        {
            MethodSymbol method = call!.Method;
            BoundExpression? receiverOpt = call!.ReceiverOpt;
            return method.MethodKind == MethodKind.Constructor &&
                receiverOpt != null &&
                (receiverOpt.Kind == BoundKind.ThisReference || receiverOpt.Kind == BoundKind.BaseReference);
        }

        public static T MakeCompilerGenerated<T>(this T node) where T : BoundNode
        {
            node.WasCompilerGenerated = true;
            return node;
        }
    }
}
