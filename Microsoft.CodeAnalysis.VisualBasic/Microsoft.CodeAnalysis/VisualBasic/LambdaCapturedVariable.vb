Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class LambdaCapturedVariable
		Inherits SynthesizedFieldSymbol
		Private ReadOnly _isMe As Boolean

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCapturedFrame As Boolean
			Get
				Return Me._isMe
			End Get
		End Property

		Public Overrides ReadOnly Property IsConst As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me.IsConst
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray(Of Location).Empty
			End Get
		End Property

		Friend Sub New(ByVal frame As LambdaFrame, ByVal captured As Symbol, ByVal type As TypeSymbol, ByVal fieldName As String, ByVal isMeParameter As Boolean)
			MyBase.New(frame, captured, type, fieldName, Accessibility.[Public], False, False, False)
			Me._isMe = isMeParameter
		End Sub

		Public Shared Function Create(ByVal frame As LambdaFrame, ByVal captured As Symbol, ByRef uniqueId As Integer) As LambdaCapturedVariable
			Dim capturedVariableFieldName As String = LambdaCapturedVariable.GetCapturedVariableFieldName(captured, uniqueId)
			Dim capturedVariableFieldType As TypeSymbol = LambdaCapturedVariable.GetCapturedVariableFieldType(frame, captured)
			Return New LambdaCapturedVariable(frame, captured, capturedVariableFieldType, capturedVariableFieldName, LambdaCapturedVariable.IsMe(captured))
		End Function

		Public Shared Function GetCapturedVariableFieldName(ByVal captured As Symbol, ByRef uniqueId As Integer) As String
			Dim str As String
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = TryCast(captured, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
			If (localSymbol Is Nothing OrElse Not localSymbol.IsCompilerGenerated) Then
				Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = TryCast(captured, Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
				str = If(parameterSymbol Is Nothing OrElse Not parameterSymbol.IsMe, [String].Concat("$VB$Local_", captured.Name), "$VB$Me")
			Else
				Dim synthesizedKind As SynthesizedLocalKind = localSymbol.SynthesizedKind
				If (synthesizedKind = SynthesizedLocalKind.[With]) Then
					uniqueId = uniqueId + 1
					str = [String].Concat("$W", Microsoft.VisualBasic.CompilerServices.Conversions.ToString(uniqueId))
				ElseIf (synthesizedKind <> SynthesizedLocalKind.LambdaDisplayClass) Then
					uniqueId = uniqueId + 1
					str = [String].Concat("$VB$NonLocal_", Microsoft.VisualBasic.CompilerServices.Conversions.ToString(uniqueId))
				Else
					uniqueId = uniqueId + 1
					str = [String].Concat("$VB$NonLocal_$VB$Closure_", Microsoft.VisualBasic.CompilerServices.Conversions.ToString(uniqueId))
				End If
			End If
			Return str
		End Function

		Public Shared Function GetCapturedVariableFieldType(ByVal frame As LambdaFrame, ByVal captured As Symbol) As TypeSymbol
			Dim type As TypeSymbol
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = TryCast(captured, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
			If (localSymbol Is Nothing) Then
				type = DirectCast(captured, ParameterSymbol).Type.InternalSubstituteTypeParameters(frame.TypeMap).Type
			Else
				Dim originalDefinition As LambdaFrame = TryCast(localSymbol.Type.OriginalDefinition, LambdaFrame)
				If (originalDefinition Is Nothing) Then
					type = localSymbol.Type.InternalSubstituteTypeParameters(frame.TypeMap).Type
				Else
					type = LambdaRewriter.ConstructFrameType(Of TypeSymbol)(originalDefinition, frame.TypeArgumentsNoUseSiteDiagnostics)
				End If
			End If
			Return type
		End Function

		Public Shared Function IsMe(ByVal captured As Symbol) As Boolean
			Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = TryCast(captured, Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
			If (parameterSymbol Is Nothing) Then
				Return False
			End If
			Return parameterSymbol.IsMe
		End Function
	End Class
End Namespace