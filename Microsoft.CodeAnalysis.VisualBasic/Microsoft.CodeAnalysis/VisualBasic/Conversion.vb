Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Operations
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Structure Conversion
		Implements IEquatable(Of Conversion), IConvertibleConversion
		Private ReadOnly _convKind As ConversionKind

		Private ReadOnly _method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol

		Public ReadOnly Property Exists As Boolean
			Get
				Return Not Conversions.NoConversion(Me._convKind)
			End Get
		End Property

		Public ReadOnly Property IsAnonymousDelegate As Boolean
			Get
				Return (Me._convKind And ConversionKind.AnonymousDelegate) <> ConversionKind.DelegateRelaxationLevelNone
			End Get
		End Property

		Public ReadOnly Property IsArray As Boolean
			Get
				Return (Me._convKind And ConversionKind.Array) <> ConversionKind.DelegateRelaxationLevelNone
			End Get
		End Property

		Public ReadOnly Property IsBoolean As Boolean
			Get
				Return (Me._convKind And ConversionKind.[Boolean]) <> ConversionKind.DelegateRelaxationLevelNone
			End Get
		End Property

		Public ReadOnly Property IsDefault As Boolean
			Get
				Return (Me._convKind And ConversionKind.WideningNothingLiteral) = ConversionKind.WideningNothingLiteral
			End Get
		End Property

		Public ReadOnly Property IsIdentity As Boolean
			Get
				Return Conversions.IsIdentityConversion(Me._convKind)
			End Get
		End Property

		Public ReadOnly Property IsLambda As Boolean
			Get
				Return (Me._convKind And ConversionKind.Lambda) <> ConversionKind.DelegateRelaxationLevelNone
			End Get
		End Property

		Public ReadOnly Property IsNarrowing As Boolean
			Get
				Return Conversions.IsNarrowingConversion(Me._convKind)
			End Get
		End Property

		Public ReadOnly Property IsNullableValueType As Boolean
			Get
				Return (Me._convKind And ConversionKind.Nullable) <> ConversionKind.DelegateRelaxationLevelNone
			End Get
		End Property

		Public ReadOnly Property IsNumeric As Boolean
			Get
				Return (Me._convKind And ConversionKind.Numeric) <> ConversionKind.DelegateRelaxationLevelNone
			End Get
		End Property

		Public ReadOnly Property IsReference As Boolean
			Get
				Return (Me._convKind And ConversionKind.Reference) <> ConversionKind.DelegateRelaxationLevelNone
			End Get
		End Property

		Public ReadOnly Property IsString As Boolean
			Get
				Return (Me._convKind And ConversionKind.[String]) <> ConversionKind.DelegateRelaxationLevelNone
			End Get
		End Property

		Public ReadOnly Property IsTypeParameter As Boolean
			Get
				Return (Me._convKind And ConversionKind.TypeParameter) <> ConversionKind.DelegateRelaxationLevelNone
			End Get
		End Property

		Public ReadOnly Property IsUserDefined As Boolean
			Get
				Return (Me._convKind And ConversionKind.UserDefined) <> ConversionKind.DelegateRelaxationLevelNone
			End Get
		End Property

		Public ReadOnly Property IsValueType As Boolean
			Get
				Return (Me._convKind And ConversionKind.Value) <> ConversionKind.DelegateRelaxationLevelNone
			End Get
		End Property

		Public ReadOnly Property IsWidening As Boolean
			Get
				Return Conversions.IsWideningConversion(Me._convKind)
			End Get
		End Property

		Friend ReadOnly Property Kind As ConversionKind
			Get
				Return Me._convKind
			End Get
		End Property

		Friend ReadOnly Property Method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Get
				Return Me._method
			End Get
		End Property

		Public ReadOnly Property MethodSymbol As IMethodSymbol
			Get
				Return Me._method
			End Get
		End Property

		Friend Sub New(ByVal conv As KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol))
			Me = New Conversion() With
			{
				._convKind = conv.Key,
				._method = conv.Value
			}
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean Implements IEquatable(Of Conversion).Equals
			If (Not TypeOf obj Is Conversion) Then
				Return False
			End If
			Return Me = DirectCast(obj, Conversion)
		End Function

		Public Function ExplicitEquals(ByVal other As Conversion) As Boolean Implements IEquatable(Of Conversion).Equals
			If (Me._convKind <> other._convKind) Then
				Return False
			End If
			Return Me.Method = other.Method
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(Me.Method, CInt(Me._convKind))
		End Function

		Public Shared Operator =(ByVal left As Conversion, ByVal right As Conversion) As Boolean
			Return left.ExplicitEquals(right)
		End Operator

		Public Shared Operator <>(ByVal left As Conversion, ByVal right As Conversion) As Boolean
			Return Not (left = right)
		End Operator

		Public Function ToCommonConversion() As CommonConversion Implements IConvertibleConversion.ToCommonConversion
			Return New CommonConversion(Me.Exists, Me.IsIdentity, Me.IsNumeric, Me.IsReference, Me.IsWidening, Me.IsNullableValueType, Me.MethodSymbol)
		End Function

		Public Overrides Function ToString() As String
			Return Me._convKind.ToString()
		End Function
	End Structure
End Namespace