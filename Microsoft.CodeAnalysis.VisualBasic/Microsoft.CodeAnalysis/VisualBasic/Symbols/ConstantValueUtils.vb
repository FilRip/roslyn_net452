Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module ConstantValueUtils
		Private Function BindFieldOrEnumInitializer(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal fieldOrEnumSymbol As FieldSymbol, ByVal equalsValueOrAsNewSyntax As VisualBasicSyntaxNode, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <Out> ByRef constValue As ConstantValue) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim sourceEnumConstantSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEnumConstantSymbol = TryCast(fieldOrEnumSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEnumConstantSymbol)
			If (sourceEnumConstantSymbol Is Nothing) Then
				Dim sourceFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = DirectCast(fieldOrEnumSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol)
				boundExpression = binder.BindFieldAndEnumConstantInitializer(sourceFieldSymbol, equalsValueOrAsNewSyntax, False, diagnostics, constValue)
			Else
				boundExpression = binder.BindFieldAndEnumConstantInitializer(sourceEnumConstantSymbol, equalsValueOrAsNewSyntax, True, diagnostics, constValue)
			End If
			Return boundExpression
		End Function

		Public Function EvaluateFieldConstant(ByVal field As SourceFieldSymbol, ByVal equalsValueOrAsNewNodeRef As SyntaxReference, ByVal dependencies As ConstantFieldsInProgress.Dependencies, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As EvaluatedConstant
			Dim type As TypeSymbol
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(field.ContainingSourceType.ContainingSourceModule, equalsValueOrAsNewNodeRef.SyntaxTree, field.ContainingSourceType)
			Dim visualBasicSyntax As VisualBasicSyntaxNode = equalsValueOrAsNewNodeRef.GetVisualBasicSyntax(New CancellationToken())
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = Nothing
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = ConstantValueUtils.BindFieldOrEnumInitializer(New ConstantFieldsInProgressBinder(New ConstantFieldsInProgress(field, dependencies), binder, field), field, visualBasicSyntax, diagnostics, constantValue)
			If (Not boundExpression.IsNothingLiteral()) Then
				type = boundExpression.Type
			Else
				type = binder.GetSpecialType(SpecialType.System_Object, visualBasicSyntax, diagnostics)
			End If
			Return New EvaluatedConstant(If(constantValue, Microsoft.CodeAnalysis.ConstantValue.Bad), type)
		End Function

		<DebuggerDisplay("{GetDebuggerDisplay(), nq}")>
		Friend Structure FieldInfo
			Public ReadOnly Field As SourceFieldSymbol

			Public ReadOnly StartsCycle As Boolean

			Public Sub New(ByVal field As SourceFieldSymbol, ByVal startsCycle As Boolean)
				Me = New ConstantValueUtils.FieldInfo() With
				{
					.Field = field,
					.StartsCycle = startsCycle
				}
			End Sub

			Private Function GetDebuggerDisplay() As String
				Dim str As String = Me.Field.ToString()
				If (Me.StartsCycle) Then
					str = [String].Concat(str, " [cycle]")
				End If
				Return str
			End Function
		End Structure
	End Module
End Namespace