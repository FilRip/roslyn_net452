Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedAttributeData
		Inherits SourceAttributeData
		Friend Sub New(ByVal wellKnownMember As MethodSymbol, ByVal arguments As ImmutableArray(Of TypedConstant), ByVal namedArgs As ImmutableArray(Of KeyValuePair(Of String, TypedConstant)))
			MyBase.New(Nothing, wellKnownMember.ContainingType, wellKnownMember, arguments, namedArgs, False, False)
		End Sub

		Friend Shared Function Create(ByVal constructorSymbol As MethodSymbol, ByVal constructor As WellKnownMember, Optional ByVal arguments As ImmutableArray(Of TypedConstant) = Nothing, Optional ByVal namedArguments As ImmutableArray(Of KeyValuePair(Of String, TypedConstant)) = Nothing) As SynthesizedAttributeData
			Dim synthesizedAttributeDatum As SynthesizedAttributeData
			If (Binder.GetUseSiteInfoForWellKnownTypeMember(constructorSymbol, constructor, False).DiagnosticInfo Is Nothing) Then
				If (arguments.IsDefault) Then
					arguments = ImmutableArray(Of TypedConstant).Empty
				End If
				If (namedArguments.IsDefault) Then
					namedArguments = ImmutableArray(Of KeyValuePair(Of String, TypedConstant)).Empty
				End If
				synthesizedAttributeDatum = New SynthesizedAttributeData(constructorSymbol, arguments, namedArguments)
			Else
				If (Not WellKnownMembers.IsSynthesizedAttributeOptional(constructor)) Then
					Throw ExceptionUtilities.Unreachable
				End If
				synthesizedAttributeDatum = Nothing
			End If
			Return synthesizedAttributeDatum
		End Function
	End Class
End Namespace