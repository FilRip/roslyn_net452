Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
	Friend NotInheritable Class EmbeddedEvent
		Inherits EmbeddedTypesManager(Of PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter).CommonEmbeddedEvent
		Protected Overrides ReadOnly Property ContainingType As EmbeddedType
			Get
				Return MyBase.AnAccessor.ContainingType
			End Get
		End Property

		Protected Overrides ReadOnly Property IsRuntimeSpecial As Boolean
			Get
				Return MyBase.UnderlyingEvent.AdaptedEventSymbol.HasRuntimeSpecialName
			End Get
		End Property

		Protected Overrides ReadOnly Property IsSpecialName As Boolean
			Get
				Return MyBase.UnderlyingEvent.AdaptedEventSymbol.HasSpecialName
			End Get
		End Property

		Protected Overrides ReadOnly Property Name As String
			Get
				Return MyBase.UnderlyingEvent.AdaptedEventSymbol.MetadataName
			End Get
		End Property

		Protected Overrides ReadOnly Property Visibility As TypeMemberVisibility
			Get
				Return PEModuleBuilder.MemberVisibility(MyBase.UnderlyingEvent.AdaptedEventSymbol)
			End Get
		End Property

		Public Sub New(ByVal underlyingEvent As EventSymbol, ByVal adder As EmbeddedMethod, ByVal remover As EmbeddedMethod, ByVal caller As EmbeddedMethod)
			MyBase.New(underlyingEvent, adder, remover, caller)
		End Sub

		Protected Overrides Sub EmbedCorrespondingComEventInterfaceMethodInternal(ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag, ByVal isUsedForComAwareEventBinding As Boolean)
			Dim underlyingNamedType As NamedTypeSymbol = Me.ContainingType.UnderlyingNamedType
			Dim enumerator As ImmutableArray(Of VisualBasicAttributeData).Enumerator = underlyingNamedType.AdaptedNamedTypeSymbol.GetAttributes().GetEnumerator()
			While enumerator.MoveNext()
				Dim current As VisualBasicAttributeData = enumerator.Current
				If (Not current.IsTargetAttribute(underlyingNamedType.AdaptedNamedTypeSymbol, AttributeDescription.ComEventInterfaceAttribute)) Then
					Continue While
				End If
				Dim flag As Boolean = False
				Dim valueInternal As NamedTypeSymbol = Nothing
				If (current.CommonConstructorArguments.Length = 2) Then
					valueInternal = TryCast(current.CommonConstructorArguments(0).ValueInternal, NamedTypeSymbol)
					If (valueInternal IsNot Nothing) Then
						flag = Me.EmbedMatchingInterfaceMethods(valueInternal, syntaxNodeOpt, diagnostics)
						Dim enumerator1 As ImmutableArray(Of NamedTypeSymbol).Enumerator = valueInternal.AllInterfacesNoUseSiteDiagnostics.GetEnumerator()
						While enumerator1.MoveNext()
							If (Not Me.EmbedMatchingInterfaceMethods(enumerator1.Current, syntaxNodeOpt, diagnostics)) Then
								Continue While
							End If
							flag = True
						End While
					End If
				End If
				If (flag OrElse Not isUsedForComAwareEventBinding) Then
					Exit While
				End If
				If (valueInternal Is Nothing) Then
					EmbeddedTypesManager.ReportDiagnostic(diagnostics, ERRID.ERR_SourceInterfaceMustBeInterface, syntaxNodeOpt, New [Object]() { underlyingNamedType.AdaptedNamedTypeSymbol, MyBase.UnderlyingEvent.AdaptedEventSymbol })
					Return
				End If
				Dim discardedDependencies As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).DiscardedDependencies
				valueInternal.AllInterfacesWithDefinitionUseSiteDiagnostics(discardedDependencies)
				diagnostics.Add(If(syntaxNodeOpt Is Nothing, NoLocation.Singleton, syntaxNodeOpt.GetLocation()), discardedDependencies.Diagnostics)
				EmbeddedTypesManager.ReportDiagnostic(diagnostics, ERRID.ERR_EventNoPIANoBackingMember, syntaxNodeOpt, New [Object]() { valueInternal, MyBase.UnderlyingEvent.AdaptedEventSymbol.MetadataName, MyBase.UnderlyingEvent.AdaptedEventSymbol })
				Return
			End While
		End Sub

		Private Function EmbedMatchingInterfaceMethods(ByVal sourceInterface As NamedTypeSymbol, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As Boolean
			Dim flag As Boolean = False
			Dim members As ImmutableArray(Of Symbol) = sourceInterface.GetMembers(MyBase.UnderlyingEvent.AdaptedEventSymbol.MetadataName)
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = members.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Symbol = enumerator.Current
				If (current.Kind <> SymbolKind.Method) Then
					Continue While
				End If
				Me.TypeManager.EmbedMethodIfNeedTo(DirectCast(current, MethodSymbol).GetCciAdapter(), syntaxNodeOpt, diagnostics)
				flag = True
			End While
			Return flag
		End Function

		Protected Overrides Function GetCustomAttributesToEmit(ByVal moduleBuilder As PEModuleBuilder) As IEnumerable(Of VisualBasicAttributeData)
			Return MyBase.UnderlyingEvent.AdaptedEventSymbol.GetCustomAttributesToEmit(moduleBuilder.CompilationState)
		End Function

		Protected Overrides Function [GetType](ByVal moduleBuilder As PEModuleBuilder, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As ITypeReference
			Return moduleBuilder.Translate(MyBase.UnderlyingEvent.AdaptedEventSymbol.Type, syntaxNodeOpt, diagnostics)
		End Function
	End Class
End Namespace