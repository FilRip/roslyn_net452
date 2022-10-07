Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	<Flags>
	Friend Enum MethodConversionKind
		Identity = 0
		OneArgumentIsVbOrBoxWidening = 1
		OneArgumentIsClrWidening = 2
		OneArgumentIsNarrowing = 4
		ReturnIsWidening = 8
		ReturnIsClrNarrowing = 16
		ReturnIsIsVbOrBoxNarrowing = 32
		ReturnValueIsDropped = 64
		AllArgumentsIgnored = 128
		ExcessOptionalArgumentsOnTarget = 256
		Error_ByRefByValMismatch = 512
		Error_Unspecified = 1024
		Error_IllegalToIgnoreAllArguments = 2048
		Error_RestrictedType = 4096
		Error_SubToFunction = 8192
		Error_ReturnTypeMismatch = 16384
		Error_OverloadResolution = 32768
		Error_StubNotSupported = 65536
		AllErrorReasons = 130560
	End Enum
End Namespace