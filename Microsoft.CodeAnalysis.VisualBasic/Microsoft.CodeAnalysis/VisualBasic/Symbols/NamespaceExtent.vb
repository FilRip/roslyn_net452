Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Structure NamespaceExtent
		Private ReadOnly _kind As NamespaceKind

		Private ReadOnly _symbolOrCompilation As Object

		Public ReadOnly Property Assembly As AssemblySymbol
			Get
				If (Me.Kind <> NamespaceKind.Assembly) Then
					Throw New InvalidOperationException()
				End If
				Return DirectCast(Me._symbolOrCompilation, AssemblySymbol)
			End Get
		End Property

		Public ReadOnly Property Compilation As VisualBasicCompilation
			Get
				If (Me.Kind <> NamespaceKind.Compilation) Then
					Throw New InvalidOperationException()
				End If
				Return DirectCast(Me._symbolOrCompilation, VisualBasicCompilation)
			End Get
		End Property

		Public ReadOnly Property Kind As NamespaceKind
			Get
				Return Me._kind
			End Get
		End Property

		Public ReadOnly Property [Module] As ModuleSymbol
			Get
				If (Me.Kind <> NamespaceKind.[Module]) Then
					Throw New InvalidOperationException()
				End If
				Return DirectCast(Me._symbolOrCompilation, ModuleSymbol)
			End Get
		End Property

		Friend Sub New(ByVal [module] As ModuleSymbol)
			Me = New NamespaceExtent() With
			{
				._kind = NamespaceKind.[Module],
				._symbolOrCompilation = [module]
			}
		End Sub

		Friend Sub New(ByVal assembly As AssemblySymbol)
			Me = New NamespaceExtent() With
			{
				._kind = NamespaceKind.Assembly,
				._symbolOrCompilation = assembly
			}
		End Sub

		Friend Sub New(ByVal compilation As VisualBasicCompilation)
			Me = New NamespaceExtent() With
			{
				._kind = NamespaceKind.Compilation,
				._symbolOrCompilation = compilation
			}
		End Sub

		Public Overrides Function ToString() As String
			Dim kind As NamespaceKind = Me.Kind
			Return [String].Format("{0}: {1}", CObj(kind.ToString()), Me._symbolOrCompilation.ToString())
		End Function
	End Structure
End Namespace