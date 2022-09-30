using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	[StandardModule]
	internal sealed class FeatureExtensions
	{
		internal static string GetFeatureFlag(this Feature feature)
		{
			return null;
		}

		internal static LanguageVersion GetLanguageVersion(this Feature feature)
		{
			switch (feature)
			{
			case Feature.AutoProperties:
			case Feature.LineContinuation:
			case Feature.StatementLambdas:
			case Feature.CoContraVariance:
			case Feature.CollectionInitializers:
			case Feature.SubLambdas:
			case Feature.ArrayLiterals:
				return LanguageVersion.VisualBasic10;
			case Feature.AsyncExpressions:
			case Feature.Iterators:
			case Feature.GlobalNamespace:
				return LanguageVersion.VisualBasic11;
			case Feature.NullPropagatingOperator:
			case Feature.NameOfExpressions:
			case Feature.InterpolatedStrings:
			case Feature.ReadonlyAutoProperties:
			case Feature.RegionsEverywhere:
			case Feature.MultilineStringLiterals:
			case Feature.CObjInAttributeArguments:
			case Feature.LineContinuationComments:
			case Feature.TypeOfIsNot:
			case Feature.YearFirstDateLiterals:
			case Feature.WarningDirectives:
			case Feature.PartialModules:
			case Feature.PartialInterfaces:
			case Feature.ImplementingReadonlyOrWriteonlyPropertyWithReadwrite:
				return LanguageVersion.VisualBasic14;
			case Feature.DigitSeparators:
			case Feature.BinaryLiterals:
			case Feature.Tuples:
				return LanguageVersion.VisualBasic15;
			case Feature.InferredTupleNames:
				return LanguageVersion.VisualBasic15_3;
			case Feature.LeadingDigitSeparator:
			case Feature.NonTrailingNamedArguments:
			case Feature.PrivateProtected:
				return LanguageVersion.VisualBasic15_5;
			case Feature.UnconstrainedTypeParameterInConditional:
			case Feature.CommentsAfterLineContinuation:
				return LanguageVersion.VisualBasic16;
			case Feature.InitOnlySettersUsage:
				return LanguageVersion.VisualBasic16_9;
			default:
				throw ExceptionUtilities.UnexpectedValue(feature);
			}
		}

		internal static ERRID GetResourceId(this Feature feature)
		{
			return feature switch
			{
				Feature.AutoProperties => ERRID.FEATURE_AutoProperties, 
				Feature.ReadonlyAutoProperties => ERRID.FEATURE_ReadonlyAutoProperties, 
				Feature.LineContinuation => ERRID.FEATURE_LineContinuation, 
				Feature.StatementLambdas => ERRID.FEATURE_StatementLambdas, 
				Feature.CoContraVariance => ERRID.FEATURE_CoContraVariance, 
				Feature.CollectionInitializers => ERRID.FEATURE_CollectionInitializers, 
				Feature.SubLambdas => ERRID.FEATURE_SubLambdas, 
				Feature.ArrayLiterals => ERRID.FEATURE_ArrayLiterals, 
				Feature.AsyncExpressions => ERRID.FEATURE_AsyncExpressions, 
				Feature.Iterators => ERRID.FEATURE_Iterators, 
				Feature.GlobalNamespace => ERRID.FEATURE_GlobalNamespace, 
				Feature.NullPropagatingOperator => ERRID.FEATURE_NullPropagatingOperator, 
				Feature.NameOfExpressions => ERRID.FEATURE_NameOfExpressions, 
				Feature.RegionsEverywhere => ERRID.FEATURE_RegionsEverywhere, 
				Feature.MultilineStringLiterals => ERRID.FEATURE_MultilineStringLiterals, 
				Feature.CObjInAttributeArguments => ERRID.FEATURE_CObjInAttributeArguments, 
				Feature.LineContinuationComments => ERRID.FEATURE_LineContinuationComments, 
				Feature.TypeOfIsNot => ERRID.FEATURE_TypeOfIsNot, 
				Feature.YearFirstDateLiterals => ERRID.FEATURE_YearFirstDateLiterals, 
				Feature.WarningDirectives => ERRID.FEATURE_WarningDirectives, 
				Feature.PartialModules => ERRID.FEATURE_PartialModules, 
				Feature.PartialInterfaces => ERRID.FEATURE_PartialInterfaces, 
				Feature.ImplementingReadonlyOrWriteonlyPropertyWithReadwrite => ERRID.FEATURE_ImplementingReadonlyOrWriteonlyPropertyWithReadwrite, 
				Feature.DigitSeparators => ERRID.FEATURE_DigitSeparators, 
				Feature.BinaryLiterals => ERRID.FEATURE_BinaryLiterals, 
				Feature.Tuples => ERRID.FEATURE_Tuples, 
				Feature.LeadingDigitSeparator => ERRID.FEATURE_LeadingDigitSeparator, 
				Feature.PrivateProtected => ERRID.FEATURE_PrivateProtected, 
				Feature.InterpolatedStrings => ERRID.FEATURE_InterpolatedStrings, 
				Feature.UnconstrainedTypeParameterInConditional => ERRID.FEATURE_UnconstrainedTypeParameterInConditional, 
				Feature.CommentsAfterLineContinuation => ERRID.FEATURE_CommentsAfterLineContinuation, 
				Feature.InitOnlySettersUsage => ERRID.FEATURE_InitOnlySettersUsage, 
				_ => throw ExceptionUtilities.UnexpectedValue(feature), 
			};
		}
	}
}
