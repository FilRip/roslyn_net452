Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class CConst
		Protected ReadOnly _errid As ERRID

		Protected ReadOnly _diagnosticArguments As Object()

		Public ReadOnly Property ErrorArgs As Object()
			Get
				Return If(Me._diagnosticArguments, Array.Empty(Of Object)())
			End Get
		End Property

		Public ReadOnly Property ErrorId As ERRID
			Get
				Return Me._errid
			End Get
		End Property

		Public ReadOnly Property IsBad As Boolean
			Get
				Return Me.SpecialType = Microsoft.CodeAnalysis.SpecialType.None
			End Get
		End Property

		Public ReadOnly Property IsBooleanTrue As Boolean
			Get
				Dim flag As Boolean
				If (Not Me.IsBad) Then
					Dim cConst As CConst(Of Boolean) = TryCast(Me, CConst(Of Boolean))
					flag = If(cConst Is Nothing, False, cConst.Value)
				Else
					flag = False
				End If
				Return flag
			End Get
		End Property

		Public MustOverride ReadOnly Property SpecialType As Microsoft.CodeAnalysis.SpecialType

		Public MustOverride ReadOnly Property ValueAsObject As Object

		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal id As ERRID, ByVal ParamArray diagnosticArguments As Object())
			MyBase.New()
			Me._errid = id
			Me._diagnosticArguments = diagnosticArguments
		End Sub

		Friend Shared Function Create(ByVal value As Boolean) As CConst(Of Boolean)
			Return New CConst(Of Boolean)(value, Microsoft.CodeAnalysis.SpecialType.System_Boolean)
		End Function

		Friend Shared Function Create(ByVal value As Byte) As CConst(Of Byte)
			Return New CConst(Of Byte)(value, Microsoft.CodeAnalysis.SpecialType.System_Byte)
		End Function

		Friend Shared Function Create(ByVal value As SByte) As CConst(Of SByte)
			Return New CConst(Of SByte)(value, Microsoft.CodeAnalysis.SpecialType.System_SByte)
		End Function

		Friend Shared Function Create(ByVal value As Char) As CConst(Of Char)
			Return New CConst(Of Char)(value, Microsoft.CodeAnalysis.SpecialType.System_Char)
		End Function

		Friend Shared Function Create(ByVal value As Short) As CConst(Of Short)
			Return New CConst(Of Short)(value, Microsoft.CodeAnalysis.SpecialType.System_Int16)
		End Function

		Friend Shared Function Create(ByVal value As UShort) As CConst(Of UShort)
			Return New CConst(Of UShort)(value, Microsoft.CodeAnalysis.SpecialType.System_UInt16)
		End Function

		Friend Shared Function Create(ByVal value As Integer) As CConst(Of Integer)
			Return New CConst(Of Integer)(value, Microsoft.CodeAnalysis.SpecialType.System_Int32)
		End Function

		Friend Shared Function Create(ByVal value As UInteger) As CConst(Of UInteger)
			Return New CConst(Of UInteger)(value, Microsoft.CodeAnalysis.SpecialType.System_UInt32)
		End Function

		Friend Shared Function Create(ByVal value As Long) As CConst(Of Long)
			Return New CConst(Of Long)(value, Microsoft.CodeAnalysis.SpecialType.System_Int64)
		End Function

		Friend Shared Function Create(ByVal value As ULong) As CConst(Of ULong)
			Return New CConst(Of ULong)(value, Microsoft.CodeAnalysis.SpecialType.System_UInt64)
		End Function

		Friend Shared Function Create(ByVal value As [Decimal]) As CConst(Of [Decimal])
			Return New CConst(Of [Decimal])(value, Microsoft.CodeAnalysis.SpecialType.System_Decimal)
		End Function

		Friend Shared Function Create(ByVal value As String) As CConst(Of String)
			Return New CConst(Of String)(value, Microsoft.CodeAnalysis.SpecialType.System_String)
		End Function

		Friend Shared Function Create(ByVal value As Single) As CConst(Of Single)
			Return New CConst(Of Single)(value, Microsoft.CodeAnalysis.SpecialType.System_Single)
		End Function

		Friend Shared Function Create(ByVal value As Double) As CConst(Of Double)
			Return New CConst(Of Double)(value, Microsoft.CodeAnalysis.SpecialType.System_Double)
		End Function

		Friend Shared Function Create(ByVal value As DateTime) As CConst(Of DateTime)
			Return New CConst(Of DateTime)(value, Microsoft.CodeAnalysis.SpecialType.System_DateTime)
		End Function

		Friend Shared Function CreateChecked(ByVal value As Object) As CConst
			Return CConst.TryCreate(RuntimeHelpers.GetObjectValue(value))
		End Function

		Friend Shared Function CreateNothing() As CConst(Of Object)
			Return New CConst(Of Object)(Nothing, Microsoft.CodeAnalysis.SpecialType.System_Object)
		End Function

		Friend Shared Function TryCreate(ByVal value As Object) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			Dim cConst As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst
			If (value IsNot Nothing) Then
				Select Case SpecialTypeExtensions.FromRuntimeTypeOfLiteralValue(RuntimeHelpers.GetObjectValue(value))
					Case Microsoft.CodeAnalysis.SpecialType.System_Boolean
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToBoolean(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_Char
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToChar(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_SByte
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToSByte(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_Byte
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToByte(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_Int16
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToInt16(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToUInt16(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_Int32
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToInt32(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToUInt32(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_Int64
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToInt64(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToUInt64(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToDecimal(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_Single
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToSingle(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_Double
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToDouble(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_String
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToString(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_IntPtr
					Case Microsoft.CodeAnalysis.SpecialType.System_UIntPtr
					Case Microsoft.CodeAnalysis.SpecialType.System_Array
					Case Microsoft.CodeAnalysis.SpecialType.System_Collections_IEnumerable
					Case Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IEnumerable_T
					Case Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IList_T
					Case Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_ICollection_T
					Case Microsoft.CodeAnalysis.SpecialType.System_Collections_IEnumerator
					Case Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IEnumerator_T
					Case Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IReadOnlyList_T
					Case Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IReadOnlyCollection_T
					Case Microsoft.CodeAnalysis.SpecialType.System_Nullable_T
					Label0:
						cConst = Nothing
						Exit Select
					Case Microsoft.CodeAnalysis.SpecialType.System_DateTime
						cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.Create(Convert.ToDateTime(RuntimeHelpers.GetObjectValue(value)))
						Exit Select
					Case Else
						GoTo Label0
				End Select
			Else
				cConst = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CConst.CreateNothing()
			End If
			Return cConst
		End Function

		Public MustOverride Function WithError(ByVal id As ERRID) As CConst
	End Class
End Namespace