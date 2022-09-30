using System;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[Flags]
	internal enum MethodConversionKind
	{
		Identity = 0,
		OneArgumentIsVbOrBoxWidening = 1,
		OneArgumentIsClrWidening = 2,
		OneArgumentIsNarrowing = 4,
		ReturnIsWidening = 8,
		ReturnIsClrNarrowing = 0x10,
		ReturnIsIsVbOrBoxNarrowing = 0x20,
		ReturnValueIsDropped = 0x40,
		AllArgumentsIgnored = 0x80,
		ExcessOptionalArgumentsOnTarget = 0x100,
		Error_ByRefByValMismatch = 0x200,
		Error_Unspecified = 0x400,
		Error_IllegalToIgnoreAllArguments = 0x800,
		Error_RestrictedType = 0x1000,
		Error_SubToFunction = 0x2000,
		Error_ReturnTypeMismatch = 0x4000,
		Error_OverloadResolution = 0x8000,
		Error_StubNotSupported = 0x10000,
		AllErrorReasons = 0x1FE00
	}
}
