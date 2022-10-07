Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Structure AnonymousTypeDescriptor
		Implements IEquatable(Of AnonymousTypeDescriptor)
		Public Const SubReturnParameterName As String = "Sub"

		Public Const FunctionReturnParameterName As String = "Function"

		Public ReadOnly Location As Microsoft.CodeAnalysis.Location

		Public ReadOnly Fields As ImmutableArray(Of AnonymousTypeField)

		Public ReadOnly Key As String

		Public ReadOnly IsImplicitlyDeclared As Boolean

		Public ReadOnly Property Parameters As ImmutableArray(Of AnonymousTypeField)
			Get
				Return Me.Fields
			End Get
		End Property

		Public Sub New(ByVal fields As ImmutableArray(Of AnonymousTypeField), ByVal location As Microsoft.CodeAnalysis.Location, ByVal isImplicitlyDeclared As Boolean)
			Dim name As Func(Of AnonymousTypeField, String)
			Dim isKey As Func(Of AnonymousTypeField, Boolean)
			Me = New AnonymousTypeDescriptor() With
			{
				.Fields = fields,
				.Location = location,
				.IsImplicitlyDeclared = isImplicitlyDeclared
			}
			Dim anonymousTypeFields As ImmutableArray(Of AnonymousTypeField) = fields
			If (AnonymousTypeDescriptor._Closure$__.$I10-0 Is Nothing) Then
				name = Function(f As AnonymousTypeField) f.Name
				AnonymousTypeDescriptor._Closure$__.$I10-0 = name
			Else
				name = AnonymousTypeDescriptor._Closure$__.$I10-0
			End If
			If (AnonymousTypeDescriptor._Closure$__.$I10-1 Is Nothing) Then
				isKey = Function(f As AnonymousTypeField) f.IsKey
				AnonymousTypeDescriptor._Closure$__.$I10-1 = isKey
			Else
				isKey = AnonymousTypeDescriptor._Closure$__.$I10-1
			End If
			Me.Key = AnonymousTypeDescriptor.ComputeKey(Of AnonymousTypeField)(anonymousTypeFields, name, isKey)
		End Sub

		<Conditional("DEBUG")>
		Friend Sub AssertGood()
			Dim enumerator As ImmutableArray(Of AnonymousTypeField).Enumerator = Me.Fields.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As AnonymousTypeField = enumerator.Current
			End While
		End Sub

		Friend Shared Function ComputeKey(Of T)(ByVal fields As ImmutableArray(Of T), ByVal getName As Func(Of T, String), ByVal getIsKey As Func(Of T, Boolean)) As String
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			Dim builder As StringBuilder = instance.Builder
			Dim enumerator As ImmutableArray(Of T).Enumerator = fields.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As T = enumerator.Current
				builder.Append("|"C)
				builder.Append(getName(current))
				builder.Append(If(getIsKey(current), "+"C, "-"C))
			End While
			CaseInsensitiveComparison.ToLower(builder)
			Return instance.ToStringAndFree()
		End Function

		Public Function ExplicitEquals(ByVal other As AnonymousTypeDescriptor) As Boolean Implements IEquatable(Of AnonymousTypeDescriptor).Equals
			Return Me.Equals(other, TypeCompareKind.ConsiderEverything)
		End Function

		Public Function Equals(ByVal other As AnonymousTypeDescriptor, ByVal compareKind As TypeCompareKind) As Boolean Implements IEquatable(Of AnonymousTypeDescriptor).Equals
			Dim flag As Boolean
			If (Me.Key.Equals(other.Key)) Then
				Dim fields As ImmutableArray(Of AnonymousTypeField) = Me.Fields
				Dim length As Integer = fields.Length
				Dim anonymousTypeFields As ImmutableArray(Of AnonymousTypeField) = other.Fields
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				While num1 <= num
					If (fields(num1).Type.Equals(anonymousTypeFields(num1).Type, compareKind)) Then
						num1 = num1 + 1
					Else
						flag = False
						Return flag
					End If
				End While
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean Implements IEquatable(Of AnonymousTypeDescriptor).Equals
			If (Not TypeOf obj Is AnonymousTypeDescriptor) Then
				Return False
			End If
			Return Me.ExplicitEquals(DirectCast(obj, AnonymousTypeDescriptor))
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me.Key.GetHashCode()
		End Function

		Friend Shared Function GetReturnParameterName(ByVal isFunction As Boolean) As String
			If (Not isFunction) Then
				Return "Sub"
			End If
			Return "Function"
		End Function

		Public Function SubstituteTypeParametersIfNeeded(ByVal substitution As TypeSubstitution, <Out> ByRef newDescriptor As AnonymousTypeDescriptor) As Boolean
			Dim length As Integer = Me.Fields.Length
			Dim anonymousTypeField(length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeField
			Dim type As Boolean = False
			Dim num As Integer = length - 1
			Dim num1 As Integer = 0
			Do
				Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeField = Me.Fields(num1)
				anonymousTypeField(num1) = New Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeField(item.Name, item.Type.InternalSubstituteTypeParameters(substitution).Type, item.Location, item.IsKey)
				If (Not type) Then
					type = CObj(item.Type) <> CObj(anonymousTypeField(num1).Type)
				End If
				num1 = num1 + 1
			Loop While num1 <= num
			If (Not type) Then
				newDescriptor = New AnonymousTypeDescriptor()
			Else
				newDescriptor = New AnonymousTypeDescriptor(ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeField)(anonymousTypeField), Me.Location, Me.IsImplicitlyDeclared)
			End If
			Return type
		End Function
	End Structure
End Namespace