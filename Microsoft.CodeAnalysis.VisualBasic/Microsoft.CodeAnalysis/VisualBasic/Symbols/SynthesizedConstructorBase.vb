Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SynthesizedConstructorBase
		Inherits SynthesizedMethodBase
		Protected ReadOnly m_isShared As Boolean

		Protected ReadOnly m_syntaxReference As SyntaxReference

		Protected ReadOnly m_voidType As TypeSymbol

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Dim accessibility As Microsoft.CodeAnalysis.Accessibility
				If (Not Me.IsShared) Then
					accessibility = If(Not Me.m_containingType.IsMustInherit, Microsoft.CodeAnalysis.Accessibility.[Public], Microsoft.CodeAnalysis.Accessibility.[Protected])
				Else
					accessibility = Microsoft.CodeAnalysis.Accessibility.[Private]
				End If
				Return accessibility
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsExternalMethod As Boolean
			Get
				Dim containingType As NamedTypeSymbol = MyBase.ContainingType
				If (containingType Is Nothing) Then
					Return False
				End If
				Return containingType.IsComImport
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me.m_isShared
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsSub As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me.m_containingType.Locations
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				If (Not Me.m_isShared) Then
					Return Microsoft.CodeAnalysis.MethodKind.Constructor
				End If
				Return Microsoft.CodeAnalysis.MethodKind.StaticConstructor
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Name As String
			Get
				If (Not Me.m_isShared) Then
					Return ".ctor"
				End If
				Return ".cctor"
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me.m_voidType
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				If (Me.m_syntaxReference Is Nothing) Then
					Return Nothing
				End If
				Return DirectCast(Me.m_syntaxReference.GetSyntax(New CancellationToken()), VisualBasicSyntaxNode)
			End Get
		End Property

		Protected Sub New(ByVal syntaxReference As Microsoft.CodeAnalysis.SyntaxReference, ByVal container As NamedTypeSymbol, ByVal isShared As Boolean, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyBase.New(container)
			Me.m_syntaxReference = syntaxReference
			Me.m_isShared = isShared
			If (binder Is Nothing) Then
				Me.m_voidType = Me.ContainingAssembly.GetSpecialType(SpecialType.System_Void)
				Return
			End If
			Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
			Me.m_voidType = binder.GetSpecialType(SpecialType.System_Void, syntaxReference.GetSyntax(cancellationToken), diagnostics)
		End Sub

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return Me.m_containingType.GetLexicalSortKey()
		End Function
	End Class
End Namespace