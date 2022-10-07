Imports Microsoft.CodeAnalysis
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class VisualBasicScriptCompilationInfo
		Inherits ScriptCompilationInfo
		Friend Overrides ReadOnly Property CommonPreviousScriptCompilation As Compilation
			Get
				Return Me.PreviousScriptCompilation
			End Get
		End Property

		Public Shadows ReadOnly Property PreviousScriptCompilation As VisualBasicCompilation

		Friend Sub New(ByVal previousCompilationOpt As VisualBasicCompilation, ByVal returnType As Type, ByVal globalsType As Type)
			MyBase.New(returnType, globalsType)
			Me.PreviousScriptCompilation = previousCompilationOpt
		End Sub

		Friend Overrides Function CommonWithPreviousScriptCompilation(ByVal compilation As Microsoft.CodeAnalysis.Compilation) As ScriptCompilationInfo
			Return Me.WithPreviousScriptCompilation(DirectCast(compilation, VisualBasicCompilation))
		End Function

		Public Function WithPreviousScriptCompilation(ByVal compilation As VisualBasicCompilation) As VisualBasicScriptCompilationInfo
			If (compilation = Me.PreviousScriptCompilation) Then
				Return Me
			End If
			Return New VisualBasicScriptCompilationInfo(compilation, MyBase.ReturnTypeOpt, MyBase.GlobalsType)
		End Function
	End Class
End Namespace