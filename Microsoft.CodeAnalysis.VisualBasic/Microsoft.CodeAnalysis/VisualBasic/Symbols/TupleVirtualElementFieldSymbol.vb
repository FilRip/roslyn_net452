Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class TupleVirtualElementFieldSymbol
		Inherits TupleElementFieldSymbol
		Private ReadOnly _name As String

		Private ReadOnly _cannotUse As Boolean

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property IsVirtualTupleField As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeLayoutOffset As Nullable(Of Integer)
			Get
				Return Nothing
			End Get
		End Property

		Public Sub New(ByVal container As TupleTypeSymbol, ByVal underlyingField As FieldSymbol, ByVal name As String, ByVal cannotUse As Boolean, ByVal tupleElementOrdinal As Integer, ByVal location As Microsoft.CodeAnalysis.Location, ByVal isImplicitlyDeclared As Boolean, ByVal correspondingDefaultFieldOpt As TupleElementFieldSymbol)
			MyBase.New(container, underlyingField, tupleElementOrdinal, location, isImplicitlyDeclared, correspondingDefaultFieldOpt)
			Me._name = name
			Me._cannotUse = cannotUse
		End Sub

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			useSiteInfo = If(Not Me._cannotUse, MyBase.GetUseSiteInfo(), New UseSiteInfo(Of AssemblySymbol)(ErrorFactory.ErrorInfo(ERRID.ERR_TupleInferredNamesNotAvailable, New [Object]() { Me._name, New VisualBasicRequiredLanguageVersion(LanguageVersion.VisualBasic15_3) })))
			Return useSiteInfo
		End Function
	End Class
End Namespace