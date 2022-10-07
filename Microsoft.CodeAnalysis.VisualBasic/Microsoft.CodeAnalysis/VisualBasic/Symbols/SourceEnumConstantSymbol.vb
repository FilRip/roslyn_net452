Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SourceEnumConstantSymbol
		Inherits SourceFieldSymbol
		Private _constantTuple As EvaluatedConstant

		Friend NotOverridable Overrides ReadOnly Property DeclarationSyntax As VisualBasicSyntaxNode
			Get
				Return MyBase.Syntax
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property GetAttributeDeclarations As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Get
				Return OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(DirectCast(MyBase.Syntax, EnumMemberDeclarationSyntax).AttributeLists)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Dim identifier As SyntaxToken = DirectCast(MyBase.Syntax, EnumMemberDeclarationSyntax).Identifier
				Return ImmutableArray.Create(Of Location)(identifier.GetLocation())
			End Get
		End Property

		Friend Overrides ReadOnly Property MeParameter As ParameterSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return MyBase.ContainingType
			End Get
		End Property

		Protected Sub New(ByVal containingEnum As SourceNamedTypeSymbol, ByVal bodyBinder As Binder, ByVal syntax As EnumMemberDeclarationSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyBase.New(containingEnum, bodyBinder.GetSyntaxReference(syntax), syntax.Identifier.ValueText, SourceMemberFlags.AccessibilityFriend Or SourceMemberFlags.AccessibilityPrivateProtected Or SourceMemberFlags.AccessibilityPublic Or SourceMemberFlags.[Shared] Or SourceMemberFlags.[Const] Or SourceMemberFlags.MethodHandlesEvents)
			If (CaseInsensitiveComparison.Equals(MyBase.Name, "value__")) Then
				diagnostics.Add(ERRID.ERR_ClashWithReservedEnumMember1, syntax.Identifier.GetLocation(), New [Object]() { MyBase.Name })
			End If
		End Sub

		Public Shared Function CreateExplicitValuedConstant(ByVal containingEnum As SourceNamedTypeSymbol, ByVal bodyBinder As Binder, ByVal syntax As EnumMemberDeclarationSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As SourceEnumConstantSymbol
			Return New SourceEnumConstantSymbol.ExplicitValuedEnumConstantSymbol(containingEnum, bodyBinder, syntax, syntax.Initializer, diagnostics)
		End Function

		Public Shared Function CreateImplicitValuedConstant(ByVal containingEnum As SourceNamedTypeSymbol, ByVal bodyBinder As Binder, ByVal syntax As EnumMemberDeclarationSyntax, ByVal otherConstant As SourceEnumConstantSymbol, ByVal otherConstantOffset As Integer, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As SourceEnumConstantSymbol
			Dim implicitValuedEnumConstantSymbol As SourceEnumConstantSymbol
			If (otherConstant IsNot Nothing) Then
				implicitValuedEnumConstantSymbol = New SourceEnumConstantSymbol.ImplicitValuedEnumConstantSymbol(containingEnum, bodyBinder, syntax, otherConstant, CUInt(otherConstantOffset), diagnostics)
			Else
				implicitValuedEnumConstantSymbol = New SourceEnumConstantSymbol.ZeroValuedEnumConstantSymbol(containingEnum, bodyBinder, syntax, diagnostics)
			End If
			Return implicitValuedEnumConstantSymbol
		End Function

		Friend NotOverridable Overrides Function GetConstantValue(ByVal inProgress As ConstantFieldsInProgress) As Microsoft.CodeAnalysis.ConstantValue
			Return MyBase.GetConstantValueImpl(inProgress)
		End Function

		Protected NotOverridable Overrides Function GetLazyConstantTuple() As EvaluatedConstant
			Return Me._constantTuple
		End Function

		Protected Overrides MustOverride Function MakeConstantTuple(ByVal dependencies As ConstantFieldsInProgress.Dependencies, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As EvaluatedConstant

		Protected NotOverridable Overrides Sub SetLazyConstantTuple(ByVal constantTuple As EvaluatedConstant, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			DirectCast(Me.ContainingModule, SourceModuleSymbol).AtomicStoreReferenceAndDiagnostics(Of EvaluatedConstant)(Me._constantTuple, constantTuple, diagnostics, Nothing)
		End Sub

		Private NotInheritable Class ExplicitValuedEnumConstantSymbol
			Inherits SourceEnumConstantSymbol
			Private ReadOnly _equalsValueNodeRef As SyntaxReference

			Public Sub New(ByVal containingEnum As SourceNamedTypeSymbol, ByVal bodyBinder As Binder, ByVal syntax As EnumMemberDeclarationSyntax, ByVal initializer As EqualsValueSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				MyBase.New(containingEnum, bodyBinder, syntax, diagnostics)
				Me._equalsValueNodeRef = bodyBinder.GetSyntaxReference(initializer)
			End Sub

			Protected Overrides Function MakeConstantTuple(ByVal dependencies As ConstantFieldsInProgress.Dependencies, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As EvaluatedConstant
				Return ConstantValueUtils.EvaluateFieldConstant(Me, Me._equalsValueNodeRef, dependencies, diagnostics)
			End Function
		End Class

		Private NotInheritable Class ImplicitValuedEnumConstantSymbol
			Inherits SourceEnumConstantSymbol
			Private ReadOnly _otherConstant As SourceEnumConstantSymbol

			Private ReadOnly _otherConstantOffset As UInteger

			Public Sub New(ByVal containingEnum As SourceNamedTypeSymbol, ByVal bodyBinder As Binder, ByVal syntax As EnumMemberDeclarationSyntax, ByVal otherConstant As SourceEnumConstantSymbol, ByVal otherConstantOffset As UInteger, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				MyBase.New(containingEnum, bodyBinder, syntax, diagnostics)
				Me._otherConstant = otherConstant
				Me._otherConstantOffset = otherConstantOffset
			End Sub

			Protected Overrides Function MakeConstantTuple(ByVal dependencies As ConstantFieldsInProgress.Dependencies, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As EvaluatedConstant
				Dim bad As Microsoft.CodeAnalysis.ConstantValue = Microsoft.CodeAnalysis.ConstantValue.Bad
				Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = Me._otherConstant.GetConstantValue(New ConstantFieldsInProgress(Me, dependencies))
				If (Not constantValue.IsBad AndAlso EnumConstantHelper.OffsetValue(constantValue, Me._otherConstantOffset, bad) = EnumOverflowKind.OverflowReport) Then
					diagnostics.Add(ERRID.ERR_ExpressionOverflow1, MyBase.Locations(0), New [Object]() { Me })
				End If
				Return New EvaluatedConstant(bad, MyBase.Type)
			End Function
		End Class

		Private NotInheritable Class ZeroValuedEnumConstantSymbol
			Inherits SourceEnumConstantSymbol
			Public Sub New(ByVal containingEnum As SourceNamedTypeSymbol, ByVal bodyBinder As Binder, ByVal syntax As EnumMemberDeclarationSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				MyBase.New(containingEnum, bodyBinder, syntax, diagnostics)
			End Sub

			Protected Overrides Function MakeConstantTuple(ByVal dependencies As ConstantFieldsInProgress.Dependencies, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As EvaluatedConstant
				Dim enumUnderlyingType As NamedTypeSymbol = MyBase.ContainingType.EnumUnderlyingType
				Return New EvaluatedConstant(Microsoft.CodeAnalysis.ConstantValue.[Default](enumUnderlyingType.SpecialType), enumUnderlyingType)
			End Function
		End Class
	End Class
End Namespace