Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure VisualBasicPreprocessingSymbolInfo
		Implements IEquatable(Of VisualBasicPreprocessingSymbolInfo)
		Private ReadOnly _symbol As PreprocessingSymbol

		Private ReadOnly _constantValue As Object

		Private ReadOnly _isDefined As Boolean

		Friend Shared None As VisualBasicPreprocessingSymbolInfo

		Public ReadOnly Property ConstantValue As Object
			Get
				Return Me._constantValue
			End Get
		End Property

		Public ReadOnly Property IsDefined As Boolean
			Get
				Return Me._isDefined
			End Get
		End Property

		Public ReadOnly Property Symbol As PreprocessingSymbol
			Get
				Return Me._symbol
			End Get
		End Property

		Shared Sub New()
			VisualBasicPreprocessingSymbolInfo.None = New VisualBasicPreprocessingSymbolInfo(Nothing, Nothing, False)
		End Sub

		Friend Sub New(ByVal symbol As PreprocessingSymbol, ByVal constantValueOpt As Object, ByVal isDefined As Boolean)
			Me = New VisualBasicPreprocessingSymbolInfo() With
			{
				._symbol = symbol,
				._constantValue = RuntimeHelpers.GetObjectValue(constantValueOpt),
				._isDefined = isDefined
			}
		End Sub

		Public Function ExplicitEquals(ByVal other As VisualBasicPreprocessingSymbolInfo) As Boolean Implements IEquatable(Of VisualBasicPreprocessingSymbolInfo).Equals
			If (Me._isDefined <> other._isDefined OrElse Not (Me._symbol = other._symbol)) Then
				Return False
			End If
			Return [Object].Equals(RuntimeHelpers.GetObjectValue(Me._constantValue), RuntimeHelpers.GetObjectValue(other._constantValue))
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean Implements IEquatable(Of VisualBasicPreprocessingSymbolInfo).Equals
			If (Not TypeOf obj Is VisualBasicTypeInfo) Then
				Return False
			End If
			Return Me.Equals(DirectCast(obj, VisualBasicTypeInfo))
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Of PreprocessingSymbol)(Me._symbol, Hash.Combine(Of Object)(RuntimeHelpers.GetObjectValue(Me._constantValue), -Me._isDefined))
		End Function

		Public Shared Widening Operator CType(ByVal info As VisualBasicPreprocessingSymbolInfo) As PreprocessingSymbolInfo
			Return New PreprocessingSymbolInfo(info.Symbol, info.IsDefined)
		End Operator
	End Structure
End Namespace