using System.Collections.Immutable;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class BoundNodeExtensions
	{
		public static bool NonNullAndHasErrors<T>(this ImmutableArray<T> nodeArray) where T : BoundNode
		{
			if (nodeArray.IsDefault)
			{
				return false;
			}
			int num = nodeArray.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (nodeArray[i].HasErrors)
				{
					return true;
				}
			}
			return false;
		}

		public static bool NonNullAndHasErrors(this BoundNode node)
		{
			return node?.HasErrors ?? false;
		}

		public static T MakeCompilerGenerated<T>(this T @this) where T : BoundNode
		{
			@this.SetWasCompilerGenerated();
			return @this;
		}

		public static Binder GetBinderFromLambda(this BoundNode boundNode)
		{
			return boundNode.Kind switch
			{
				BoundKind.UnboundLambda => ((UnboundLambda)boundNode).Binder, 
				BoundKind.QueryLambda => ((BoundQueryLambda)boundNode).LambdaSymbol.ContainingBinder, 
				BoundKind.GroupTypeInferenceLambda => ((GroupTypeInferenceLambda)boundNode).Binder, 
				_ => null, 
			};
		}

		public static bool IsAnyLambda(this BoundNode boundNode)
		{
			BoundKind kind = boundNode.Kind;
			if (kind != BoundKind.UnboundLambda && kind != BoundKind.Lambda && kind != BoundKind.QueryLambda)
			{
				return kind == BoundKind.GroupTypeInferenceLambda;
			}
			return true;
		}
	}
}
