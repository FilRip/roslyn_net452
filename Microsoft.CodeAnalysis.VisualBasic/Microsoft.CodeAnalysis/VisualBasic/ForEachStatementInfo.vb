Imports Microsoft.CodeAnalysis
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Structure ForEachStatementInfo
		Public ReadOnly Property CurrentConversion As Conversion

		Public ReadOnly Property CurrentProperty As IPropertySymbol

		Public ReadOnly Property DisposeMethod As IMethodSymbol

		Public ReadOnly Property ElementConversion As Conversion

		Public ReadOnly Property ElementType As ITypeSymbol

		Public ReadOnly Property GetEnumeratorMethod As IMethodSymbol

		Public ReadOnly Property MoveNextMethod As IMethodSymbol

		Friend Sub New(ByVal getEnumeratorMethod As IMethodSymbol, ByVal moveNextMethod As IMethodSymbol, ByVal currentProperty As IPropertySymbol, ByVal disposeMethod As IMethodSymbol, ByVal elementType As ITypeSymbol, ByVal elementConversion As Conversion, ByVal currentConversion As Conversion)
			Me = New ForEachStatementInfo()
			Me.GetEnumeratorMethod = getEnumeratorMethod
			Me.MoveNextMethod = moveNextMethod
			Me.CurrentProperty = currentProperty
			Me.DisposeMethod = disposeMethod
			Me.ElementType = elementType
			Me.ElementConversion = elementConversion
			Me.CurrentConversion = currentConversion
		End Sub
	End Structure
End Namespace