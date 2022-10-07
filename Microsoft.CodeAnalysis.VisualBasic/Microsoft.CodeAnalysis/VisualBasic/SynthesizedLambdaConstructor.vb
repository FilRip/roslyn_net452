Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class SynthesizedLambdaConstructor
		Inherits SynthesizedMethod
		Implements ISynthesizedMethodBodyImplementationSymbol
		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property HasMethodBodyDependency As Boolean Implements ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return True
			End Get
		End Property

		Public ReadOnly Property Method As IMethodSymbolInternal Implements ISynthesizedMethodBodyImplementationSymbol.Method
			Get
				Return DirectCast(MyBase.ContainingSymbol, ISynthesizedMethodBodyImplementationSymbol).Method
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Microsoft.CodeAnalysis.MethodKind.Constructor
			End Get
		End Property

		Friend Sub New(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal containingType As LambdaFrame)
			MyBase.New(syntaxNode, containingType, ".ctor", False)
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
		End Sub

		Friend Function AsMember(ByVal frameType As NamedTypeSymbol) As MethodSymbol
			Dim memberForDefinition As MethodSymbol
			If (CObj(frameType) <> CObj(MyBase.ContainingType)) Then
				memberForDefinition = DirectCast(DirectCast(frameType, SubstitutedNamedType).GetMemberForDefinition(Me), MethodSymbol)
			Else
				memberForDefinition = Me
			End If
			Return memberForDefinition
		End Function

		Friend NotOverridable Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
			Return False
		End Function
	End Class
End Namespace