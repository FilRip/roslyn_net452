Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure VisualBasicTypeInfo
		Implements IEquatable(Of VisualBasicTypeInfo)
		Private ReadOnly _type As TypeSymbol

		Private ReadOnly _convertedType As TypeSymbol

		Private ReadOnly _implicitConversion As Conversion

		Friend Shared None As VisualBasicTypeInfo

		Public ReadOnly Property ConvertedType As TypeSymbol
			Get
				Return Me._convertedType
			End Get
		End Property

		Public ReadOnly Property ImplicitConversion As Conversion
			Get
				Return Me._implicitConversion
			End Get
		End Property

		Public ReadOnly Property Type As TypeSymbol
			Get
				Return Me._type
			End Get
		End Property

		Shared Sub New()
			VisualBasicTypeInfo.None = New VisualBasicTypeInfo(Nothing, Nothing, New Conversion(Conversions.Identity))
		End Sub

		Friend Sub New(ByVal type As TypeSymbol, ByVal convertedType As TypeSymbol, ByVal implicitConversion As Conversion)
			Me = New VisualBasicTypeInfo() With
			{
				._type = VisualBasicTypeInfo.GetPossibleGuessForErrorType(type),
				._convertedType = VisualBasicTypeInfo.GetPossibleGuessForErrorType(convertedType),
				._implicitConversion = implicitConversion
			}
		End Sub

		Public Function ExplicitEquals(ByVal other As VisualBasicTypeInfo) As Boolean Implements IEquatable(Of VisualBasicTypeInfo).Equals
			If (Not Me._implicitConversion.ExplicitEquals(other._implicitConversion) OrElse Not TypeSymbol.Equals(Me._type, other._type, TypeCompareKind.ConsiderEverything)) Then
				Return False
			End If
			Return TypeSymbol.Equals(Me._convertedType, other._convertedType, TypeCompareKind.ConsiderEverything)
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean Implements IEquatable(Of VisualBasicTypeInfo).Equals
			If (Not TypeOf obj Is VisualBasicTypeInfo) Then
				Return False
			End If
			Return Me.ExplicitEquals(DirectCast(obj, VisualBasicTypeInfo))
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me._convertedType
			Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me._type
			Dim conversion As Microsoft.CodeAnalysis.VisualBasic.Conversion = Me._implicitConversion
			Return Hash.Combine(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(typeSymbol, Hash.Combine(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(typeSymbol1, conversion.GetHashCode()))
		End Function

		Private Shared Function GetPossibleGuessForErrorType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim errorTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ErrorTypeSymbol = TryCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.ErrorTypeSymbol)
			If (errorTypeSymbol IsNot Nothing) Then
				Dim nonErrorGuessType As NamedTypeSymbol = errorTypeSymbol.NonErrorGuessType
				If (nonErrorGuessType IsNot Nothing) Then
					typeSymbol = nonErrorGuessType
				Else
					typeSymbol = type
				End If
			Else
				typeSymbol = type
			End If
			Return typeSymbol
		End Function

		Public Shared Widening Operator CType(ByVal info As VisualBasicTypeInfo) As TypeInfo
			Dim type As TypeSymbol = info.Type
			Dim convertedType As TypeSymbol = info.ConvertedType
			Dim nullabilityInfo As Microsoft.CodeAnalysis.NullabilityInfo = New Microsoft.CodeAnalysis.NullabilityInfo()
			Dim nullabilityInfo1 As Microsoft.CodeAnalysis.NullabilityInfo = nullabilityInfo
			nullabilityInfo = New Microsoft.CodeAnalysis.NullabilityInfo()
			Return New TypeInfo(type, convertedType, nullabilityInfo1, nullabilityInfo)
		End Operator
	End Structure
End Namespace