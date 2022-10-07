Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Enum GeneratedNameKind
		None
		HoistedMeField
		HoistedSynthesizedLocalField
		HoistedUserVariableField
		IteratorCurrentField
		IteratorInitialThreadIdField
		IteratorParameterProxyField
		StateMachineAwaiterField
		StateMachineStateField
		StateMachineHoistedUserVariableField
		StaticLocalField
		TransparentIdentifier
		AnonymousTransparentIdentifier
		AnonymousType
		LambdaCacheField
		LambdaDisplayClass
	End Enum
End Namespace