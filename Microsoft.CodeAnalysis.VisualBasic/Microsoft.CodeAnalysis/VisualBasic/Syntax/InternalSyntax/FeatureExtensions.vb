Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Module FeatureExtensions
		<Extension>
		Friend Function GetFeatureFlag(ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature) As String
			Return Nothing
		End Function

		<Extension>
		Friend Function GetLanguageVersion(ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature) As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion
			Dim languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion
			Select Case feature
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.AutoProperties
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.LineContinuation
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.StatementLambdas
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.CoContraVariance
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.CollectionInitializers
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.SubLambdas
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.ArrayLiterals
					languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic10
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.AsyncExpressions
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.Iterators
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.GlobalNamespace
					languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic11
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.NullPropagatingOperator
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.NameOfExpressions
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.InterpolatedStrings
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.ReadonlyAutoProperties
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.RegionsEverywhere
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.MultilineStringLiterals
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.CObjInAttributeArguments
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.LineContinuationComments
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.TypeOfIsNot
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.YearFirstDateLiterals
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.WarningDirectives
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.PartialModules
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.PartialInterfaces
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.ImplementingReadonlyOrWriteonlyPropertyWithReadwrite
					languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic14
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.DigitSeparators
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.BinaryLiterals
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.Tuples
					languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.InferredTupleNames
					languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15_3
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.LeadingDigitSeparator
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.NonTrailingNamedArguments
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.PrivateProtected
					languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15_5
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.UnconstrainedTypeParameterInConditional
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.CommentsAfterLineContinuation
					languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.InitOnlySettersUsage
					languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16_9
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(feature)
			End Select
			Return languageVersion
		End Function

		<Extension>
		Friend Function GetResourceId(ByVal feature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature) As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Select Case feature
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.AutoProperties
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_AutoProperties
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.LineContinuation
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_LineContinuation
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.StatementLambdas
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_StatementLambdas
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.CoContraVariance
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_CoContraVariance
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.CollectionInitializers
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_CollectionInitializers
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.SubLambdas
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_SubLambdas
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.ArrayLiterals
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_ArrayLiterals
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.AsyncExpressions
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_AsyncExpressions
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.Iterators
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_Iterators
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.GlobalNamespace
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_GlobalNamespace
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.NullPropagatingOperator
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_NullPropagatingOperator
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.NameOfExpressions
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_NameOfExpressions
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.InterpolatedStrings
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_InterpolatedStrings
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.ReadonlyAutoProperties
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_ReadonlyAutoProperties
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.RegionsEverywhere
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_RegionsEverywhere
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.MultilineStringLiterals
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_MultilineStringLiterals
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.CObjInAttributeArguments
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_CObjInAttributeArguments
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.LineContinuationComments
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_LineContinuationComments
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.TypeOfIsNot
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_TypeOfIsNot
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.YearFirstDateLiterals
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_YearFirstDateLiterals
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.WarningDirectives
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_WarningDirectives
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.PartialModules
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_PartialModules
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.PartialInterfaces
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_PartialInterfaces
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.ImplementingReadonlyOrWriteonlyPropertyWithReadwrite
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_ImplementingReadonlyOrWriteonlyPropertyWithReadwrite
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.DigitSeparators
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_DigitSeparators
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.BinaryLiterals
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_BinaryLiterals
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.Tuples
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_Tuples
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.InferredTupleNames
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.NonTrailingNamedArguments
					Throw ExceptionUtilities.UnexpectedValue(feature)
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.LeadingDigitSeparator
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_LeadingDigitSeparator
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.PrivateProtected
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_PrivateProtected
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.UnconstrainedTypeParameterInConditional
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_UnconstrainedTypeParameterInConditional
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.CommentsAfterLineContinuation
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_CommentsAfterLineContinuation
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.Feature.InitOnlySettersUsage
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.FEATURE_InitOnlySettersUsage
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(feature)
			End Select
			Return eRRID
		End Function
	End Module
End Namespace