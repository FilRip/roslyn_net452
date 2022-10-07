Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class VisualBasicCustomModifier
		Inherits CustomModifier
		Implements ICustomModifier
		Protected ReadOnly m_Modifier As NamedTypeSymbol

		ReadOnly Property CciIsOptional As Boolean Implements ICustomModifier.IsOptional
			Get
				Return Me.IsOptional
			End Get
		End Property

		Public Overrides ReadOnly Property Modifier As INamedTypeSymbol
			Get
				Return Me.m_Modifier
			End Get
		End Property

		Public ReadOnly Property ModifierSymbol As NamedTypeSymbol
			Get
				Return Me.m_Modifier
			End Get
		End Property

		Private Sub New(ByVal modifier As NamedTypeSymbol)
			MyBase.New()
			Me.m_Modifier = modifier
		End Sub

		Private Function CciGetModifier(ByVal context As EmitContext) As ITypeReference Implements ICustomModifier.GetModifier
			Return DirectCast(context.[Module], PEModuleBuilder).Translate(Me.ModifierSymbol, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, False, False)
		End Function

		Friend Shared Function Convert(ByVal customModifiers As ImmutableArray(Of ModifierInfo(Of TypeSymbol))) As ImmutableArray(Of CustomModifier)
			Dim empty As ImmutableArray(Of CustomModifier)
			If (Not customModifiers.IsDefault) Then
				empty = ImmutableArrayExtensions.SelectAsArray(Of ModifierInfo(Of TypeSymbol), CustomModifier)(customModifiers, New Func(Of ModifierInfo(Of TypeSymbol), CustomModifier)(AddressOf VisualBasicCustomModifier.Convert))
			Else
				empty = ImmutableArray(Of CustomModifier).Empty
			End If
			Return empty
		End Function

		Private Shared Function Convert(ByVal customModifier As ModifierInfo(Of TypeSymbol)) As Microsoft.CodeAnalysis.CustomModifier
			Dim modifier As NamedTypeSymbol = DirectCast(customModifier.Modifier, NamedTypeSymbol)
			If (Not customModifier.IsOptional) Then
				Return VisualBasicCustomModifier.CreateRequired(modifier)
			End If
			Return VisualBasicCustomModifier.CreateOptional(modifier)
		End Function

		Friend Shared Function CreateOptional(ByVal modifier As NamedTypeSymbol) As CustomModifier
			Return New VisualBasicCustomModifier.OptionalCustomModifier(modifier)
		End Function

		Friend Shared Function CreateRequired(ByVal modifier As NamedTypeSymbol) As CustomModifier
			Return New VisualBasicCustomModifier.RequiredCustomModifier(modifier)
		End Function

		Public Overrides MustOverride Function Equals(ByVal obj As Object) As Boolean

		Public Overrides MustOverride Function GetHashCode() As Integer

		Private Class OptionalCustomModifier
			Inherits VisualBasicCustomModifier
			Public Overrides ReadOnly Property IsOptional As Boolean
				Get
					Return True
				End Get
			End Property

			Public Sub New(ByVal modifier As NamedTypeSymbol)
				MyBase.New(modifier)
			End Sub

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				Dim flag As Boolean
				If (obj <> Me) Then
					Dim optionalCustomModifier As VisualBasicCustomModifier.OptionalCustomModifier = TryCast(obj, VisualBasicCustomModifier.OptionalCustomModifier)
					flag = If(optionalCustomModifier Is Nothing, False, optionalCustomModifier.m_Modifier.Equals(Me.m_Modifier))
				Else
					flag = True
				End If
				Return flag
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return Me.m_Modifier.GetHashCode()
			End Function
		End Class

		Private Class RequiredCustomModifier
			Inherits VisualBasicCustomModifier
			Public Overrides ReadOnly Property IsOptional As Boolean
				Get
					Return False
				End Get
			End Property

			Public Sub New(ByVal modifier As NamedTypeSymbol)
				MyBase.New(modifier)
			End Sub

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				Dim flag As Boolean
				If (obj <> Me) Then
					Dim requiredCustomModifier As VisualBasicCustomModifier.RequiredCustomModifier = TryCast(obj, VisualBasicCustomModifier.RequiredCustomModifier)
					flag = If(requiredCustomModifier Is Nothing, False, requiredCustomModifier.m_Modifier.Equals(Me.m_Modifier))
				Else
					flag = True
				End If
				Return flag
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return Me.m_Modifier.GetHashCode()
			End Function
		End Class
	End Class
End Namespace