Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class LambdaBodyBinder
		Inherits SubOrFunctionBodyBinder
		Private ReadOnly _functionValue As LocalSymbol

		Public Sub New(ByVal lambdaSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol, ByVal containingBinder As Binder)
			MyBase.New(lambdaSymbol, lambdaSymbol.Syntax, containingBinder)
			Me._functionValue = LambdaBodyBinder.CreateFunctionValueLocal(lambdaSymbol)
		End Sub

		Private Shared Function CreateFunctionValueLocal(ByVal lambdaSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol) As LocalSymbol
			Dim synthesizedLocal As LocalSymbol
			If (lambdaSymbol.IsImplicitlyDeclared OrElse lambdaSymbol.IsSub) Then
				synthesizedLocal = Nothing
			Else
				Dim subOrFunctionHeader As LambdaHeaderSyntax = DirectCast(lambdaSymbol.Syntax, LambdaExpressionSyntax).SubOrFunctionHeader
				synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(lambdaSymbol, lambdaSymbol.ReturnType, SynthesizedLocalKind.FunctionReturnValue, subOrFunctionHeader, False)
			End If
			Return synthesizedLocal
		End Function

		Public Overrides Function GetContinueLabel(ByVal continueSyntaxKind As SyntaxKind) As LabelSymbol
			Return Nothing
		End Function

		Public Overrides Function GetExitLabel(ByVal exitSyntaxKind As SyntaxKind) As LabelSymbol
			Return Nothing
		End Function

		Public Overrides Function GetLocalForFunctionValue() As LocalSymbol
			Return Me._functionValue
		End Function

		Public Overrides Function GetReturnLabel() As LabelSymbol
			Return Nothing
		End Function

		Friend Overrides Sub LookupInSingleBinder(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal originalBinder As Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			MyBase.LookupInSingleBinder(lookupResult, name, arity, options, originalBinder, useSiteInfo)
			If ((options And LookupOptions.LabelsOnly) = LookupOptions.LabelsOnly AndAlso lookupResult.Kind = LookupResultKind.Empty) Then
				lookupResult.SetFrom(SingleLookupResult.EmptyAndStopLookup)
			End If
		End Sub
	End Class
End Namespace