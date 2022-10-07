Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Enum SymbolComparisonResults
		NameMismatch = 1
		ReturnTypeMismatch = 2
		ArityMismatch = 4
		ConstraintMismatch = 8
		CallingConventionMismatch = 16
		CustomModifierMismatch = 32
		RequiredExtraParameterMismatch = 64
		OptionalParameterMismatch = 128
		RequiredParameterTypeMismatch = 256
		OptionalParameterTypeMismatch = 512
		OptionalParameterValueMismatch = 1024
		ParameterByrefMismatch = 2048
		ParamArrayMismatch = 4096
		PropertyAccessorMismatch = 8192
		PropertyInitOnlyMismatch = 16384
		VarargMismatch = 32768
		TotalParameterCountMismatch = 65536
		TupleNamesMismatch = 131072
		MismatchesForExplicitInterfaceImplementations = 139305
		MismatchesForConflictingMethods = 146618
		AllParameterMismatches = 204768
		AllMismatches = 262143
	End Enum
End Namespace