Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class ConstantFieldsInProgress
		Private ReadOnly _fieldOpt As SourceFieldSymbol

		Private ReadOnly _dependencies As ConstantFieldsInProgress.Dependencies

		Friend ReadOnly Shared Empty As ConstantFieldsInProgress

		Public ReadOnly Property IsEmpty As Boolean
			Get
				Return Me._fieldOpt Is Nothing
			End Get
		End Property

		Shared Sub New()
			ConstantFieldsInProgress.Empty = New ConstantFieldsInProgress(Nothing, New ConstantFieldsInProgress.Dependencies())
		End Sub

		Friend Sub New(ByVal fieldOpt As SourceFieldSymbol, ByVal dependencies As ConstantFieldsInProgress.Dependencies)
			MyBase.New()
			Me._fieldOpt = fieldOpt
			Me._dependencies = dependencies
		End Sub

		Friend Sub AddDependency(ByVal field As SourceFieldSymbol)
			Me._dependencies.Add(field)
		End Sub

		Public Function AnyDependencies() As Boolean
			Return Me._dependencies.Any()
		End Function

		Friend Structure Dependencies
			Private ReadOnly _builder As HashSet(Of SourceFieldSymbol)

			Friend Sub New(ByVal builder As HashSet(Of SourceFieldSymbol))
				Me = New ConstantFieldsInProgress.Dependencies() With
				{
					._builder = builder
				}
			End Sub

			Friend Sub Add(ByVal field As SourceFieldSymbol)
				Me._builder.Add(field)
			End Sub

			Friend Function Any() As Boolean
				Return Me._builder.Count <> 0
			End Function

			<Conditional("DEBUG")>
			Friend Sub Freeze()
			End Sub
		End Structure
	End Class
End Namespace