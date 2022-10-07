Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SourceLambdaSymbol
		Inherits LambdaSymbol
		Private ReadOnly _unboundLambda As Microsoft.CodeAnalysis.VisualBasic.UnboundLambda

		Private _lazyAnonymousDelegateSymbol As NamedTypeSymbol

		Public Overrides ReadOnly Property AssociatedAnonymousDelegate As NamedTypeSymbol
			Get
				If (Me._lazyAnonymousDelegateSymbol = ErrorTypeSymbol.UnknownResultType) Then
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.MakeAssociatedAnonymousDelegate()
					Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(Me._lazyAnonymousDelegateSymbol, namedTypeSymbol, ErrorTypeSymbol.UnknownResultType)
				End If
				Return Me._lazyAnonymousDelegateSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property IsAsync As Boolean
			Get
				Return (Me._unboundLambda.Flags And SourceMemberFlags.Async) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsIterator As Boolean
			Get
				Return (Me._unboundLambda.Flags And SourceMemberFlags.Iterator) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property SynthesizedKind As SynthesizedLambdaKind
			Get
				Return SynthesizedLambdaKind.UserDefined
			End Get
		End Property

		Public ReadOnly Property UnboundLambda As Microsoft.CodeAnalysis.VisualBasic.UnboundLambda
			Get
				Return Me._unboundLambda
			End Get
		End Property

		Public Sub New(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal unboundLambda As Microsoft.CodeAnalysis.VisualBasic.UnboundLambda, ByVal parameters As ImmutableArray(Of BoundLambdaParameterSymbol), ByVal returnType As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder)
			MyBase.New(syntaxNode, parameters, returnType, binder)
			Me._lazyAnonymousDelegateSymbol = ErrorTypeSymbol.UnknownResultType
			Me._unboundLambda = unboundLambda
		End Sub

		Friend Function MakeAssociatedAnonymousDelegate() As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim key As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._unboundLambda.InferredAnonymousDelegate.Key
			Dim targetSignature As Microsoft.CodeAnalysis.VisualBasic.UnboundLambda.TargetSignature = New Microsoft.CodeAnalysis.VisualBasic.UnboundLambda.TargetSignature(key.DelegateInvokeMethod)
			If (Me._unboundLambda.Bind(targetSignature).LambdaSymbol = Me) Then
				namedTypeSymbol = key
			Else
				namedTypeSymbol = Nothing
			End If
			Return namedTypeSymbol
		End Function
	End Class
End Namespace