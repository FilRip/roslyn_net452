Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundMethodGroup
		Inherits BoundMethodOrPropertyGroup
		Private ReadOnly _TypeArgumentsOpt As BoundTypeArguments

		Private ReadOnly _Methods As ImmutableArray(Of MethodSymbol)

		Private ReadOnly _PendingExtensionMethodsOpt As ExtensionMethodGroup

		Private ReadOnly _ResultKind As LookupResultKind

		Public ReadOnly Property Methods As ImmutableArray(Of MethodSymbol)
			Get
				Return Me._Methods
			End Get
		End Property

		Public ReadOnly Property PendingExtensionMethodsOpt As ExtensionMethodGroup
			Get
				Return Me._PendingExtensionMethodsOpt
			End Get
		End Property

		Public Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Return Me._ResultKind
			End Get
		End Property

		Public ReadOnly Property TypeArgumentsOpt As BoundTypeArguments
			Get
				Return Me._TypeArgumentsOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal typeArgumentsOpt As BoundTypeArguments, ByVal methods As ImmutableArray(Of MethodSymbol), ByVal resultKind As LookupResultKind, ByVal receiverOpt As BoundExpression, ByVal qualificationKind As Microsoft.CodeAnalysis.VisualBasic.QualificationKind, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, typeArgumentsOpt, methods, Nothing, resultKind, receiverOpt, qualificationKind, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal typeArgumentsOpt As BoundTypeArguments, ByVal methods As ImmutableArray(Of MethodSymbol), ByVal pendingExtensionMethodsOpt As ExtensionMethodGroup, ByVal resultKind As LookupResultKind, ByVal receiverOpt As BoundExpression, ByVal qualificationKind As Microsoft.CodeAnalysis.VisualBasic.QualificationKind, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.MethodGroup, syntax, receiverOpt, qualificationKind, If(hasErrors OrElse typeArgumentsOpt.NonNullAndHasErrors(), True, receiverOpt.NonNullAndHasErrors()))
			Me._TypeArgumentsOpt = typeArgumentsOpt
			Me._Methods = methods
			Me._PendingExtensionMethodsOpt = pendingExtensionMethodsOpt
			Me._ResultKind = resultKind
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitMethodGroup(Me)
		End Function

		Public Function AdditionalExtensionMethods(<InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of MethodSymbol)
			Dim empty As ImmutableArray(Of MethodSymbol)
			If (Me._PendingExtensionMethodsOpt IsNot Nothing) Then
				empty = Me._PendingExtensionMethodsOpt.LazyLookupAdditionalExtensionMethods(Me, useSiteInfo)
			Else
				empty = ImmutableArray(Of MethodSymbol).Empty
			End If
			Return empty
		End Function

		Public Function Update(ByVal typeArgumentsOpt As BoundTypeArguments, ByVal methods As ImmutableArray(Of MethodSymbol), ByVal pendingExtensionMethodsOpt As ExtensionMethodGroup, ByVal resultKind As LookupResultKind, ByVal receiverOpt As BoundExpression, ByVal qualificationKind As Microsoft.CodeAnalysis.VisualBasic.QualificationKind) As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup
			Dim boundMethodGroup As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup
			If (typeArgumentsOpt <> Me.TypeArgumentsOpt OrElse methods <> Me.Methods OrElse pendingExtensionMethodsOpt <> Me.PendingExtensionMethodsOpt OrElse resultKind <> Me.ResultKind OrElse receiverOpt <> MyBase.ReceiverOpt OrElse qualificationKind <> MyBase.QualificationKind) Then
				Dim boundMethodGroup1 As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup = New Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup(MyBase.Syntax, typeArgumentsOpt, methods, pendingExtensionMethodsOpt, resultKind, receiverOpt, qualificationKind, MyBase.HasErrors)
				boundMethodGroup1.CopyAttributes(Me)
				boundMethodGroup = boundMethodGroup1
			Else
				boundMethodGroup = Me
			End If
			Return boundMethodGroup
		End Function
	End Class
End Namespace