Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SubstitutedNamedType
		Inherits NamedTypeSymbol
		Private ReadOnly _substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution

		Public NotOverridable Overrides ReadOnly Property Arity As Integer
			Get
				Return Me.OriginalDefinition.Arity
			End Get
		End Property

		Friend Overrides ReadOnly Property CoClassType As TypeSymbol
			Get
				Return Me.OriginalDefinition.CoClassType
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me.OriginalDefinition.ContainingAssembly
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me.OriginalDefinition.DeclaredAccessibility
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me.OriginalDefinition.DeclaringSyntaxReferences
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property DefaultPropertyName As String
			Get
				Return Me.OriginalDefinition.DefaultPropertyName
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property EmbeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			Get
				Return Me.OriginalDefinition.EmbeddedSymbolKind
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property EnumUnderlyingType As NamedTypeSymbol
			Get
				Return Me.OriginalDefinition.EnumUnderlyingType
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasCodeAnalysisEmbeddedAttribute As Boolean
			Get
				Return Me.OriginalDefinition.HasCodeAnalysisEmbeddedAttribute
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Return Me.OriginalDefinition.HasDeclarativeSecurity
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me.OriginalDefinition.HasSpecialName
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasVisualBasicEmbeddedAttribute As Boolean
			Get
				Return Me.OriginalDefinition.HasVisualBasicEmbeddedAttribute
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsComImport As Boolean
			Get
				Return Me.OriginalDefinition.IsComImport
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExtensibleInterfaceNoUseSiteDiagnostics As Boolean
			Get
				Return Me.OriginalDefinition.IsExtensibleInterfaceNoUseSiteDiagnostics
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me.OriginalDefinition.IsImplicitlyDeclared
			End Get
		End Property

		Friend Overrides ReadOnly Property IsInterface As Boolean
			Get
				Return Me.OriginalDefinition.IsInterface
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsMustInherit As Boolean
			Get
				Return Me.OriginalDefinition.IsMustInherit
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsNotInheritable As Boolean
			Get
				Return Me.OriginalDefinition.IsNotInheritable
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsSerializable As Boolean
			Get
				Return Me.OriginalDefinition.IsSerializable
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsWindowsRuntimeImport As Boolean
			Get
				Return Me.OriginalDefinition.IsWindowsRuntimeImport
			End Get
		End Property

		Friend Overrides ReadOnly Property Layout As TypeLayout
			Get
				Return Me.OriginalDefinition.Layout
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me.OriginalDefinition.Locations
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property MangleName As Boolean
			Get
				Return Me.OriginalDefinition.MangleName
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingCharSet As CharSet
			Get
				Return Me.OriginalDefinition.MarshallingCharSet
			End Get
		End Property

		Public Overrides ReadOnly Property MemberNames As IEnumerable(Of String)
			Get
				Return Me.OriginalDefinition.MemberNames
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property MetadataName As String
			Get
				Return Me.OriginalDefinition.MetadataName
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				Return Me.OriginalDefinition.MightContainExtensionMethods
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Name As String
			Get
				Return Me.OriginalDefinition.Name
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me.OriginalDefinition.ObsoleteAttributeData
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property OriginalDefinition As NamedTypeSymbol
			Get
				Return DirectCast(Me._substitution.TargetGenericDefinition, NamedTypeSymbol)
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
			Get
				Return Me.OriginalDefinition.ShouldAddWinRTMembers
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
			Get
				Return Me.OriginalDefinition.TypeKind
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Get
				Return Me._substitution
			End Get
		End Property

		Private Sub New(ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution)
			MyBase.New()
			Me._substitution = substitution
		End Sub

		Protected Overridable Function CreateSubstitutedEventSymbol(ByVal memberEvent As EventSymbol, ByVal addMethod As SubstitutedMethodSymbol, ByVal removeMethod As SubstitutedMethodSymbol, ByVal raiseMethod As SubstitutedMethodSymbol, ByVal associatedField As SubstitutedFieldSymbol) As SubstitutedEventSymbol
			Return New SubstitutedEventSymbol(Me, memberEvent, addMethod, removeMethod, raiseMethod, associatedField)
		End Function

		Public NotOverridable Overrides Function Equals(ByVal other As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType::Equals(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.TypeCompareKind)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean Equals(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.TypeCompareKind)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			Throw ExceptionUtilities.Unreachable
		End Sub

		Friend NotOverridable Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return Me.OriginalDefinition.GetAppliedConditionalSymbols()
		End Function

		Public NotOverridable Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.OriginalDefinition.GetAttributes()
		End Function

		Friend NotOverridable Overrides Function GetAttributeUsageInfo() As AttributeUsageInfo
			Return Me.OriginalDefinition.GetAttributeUsageInfo()
		End Function

		Friend Overrides Function GetDirectBaseTypeNoUseSiteDiagnostics(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim directBaseTypeNoUseSiteDiagnostics As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.OriginalDefinition.GetDirectBaseTypeNoUseSiteDiagnostics(basesBeingResolved)
			If (directBaseTypeNoUseSiteDiagnostics Is Nothing) Then
				namedTypeSymbol = Nothing
			Else
				Dim typeWithModifier As TypeWithModifiers = directBaseTypeNoUseSiteDiagnostics.InternalSubstituteTypeParameters(Me._substitution)
				namedTypeSymbol = DirectCast(typeWithModifier.AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			End If
			Return namedTypeSymbol
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me.OriginalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Friend NotOverridable Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
			Throw ExceptionUtilities.Unreachable
		End Function

		Public NotOverridable Overrides Function GetHashCode() As Integer
			Dim num As Integer
			Dim hashCode As Integer = Me.OriginalDefinition.GetHashCode()
			If (Not Me._substitution.WasConstructedForModifiers()) Then
				hashCode = Hash.Combine(Of NamedTypeSymbol)(Me.ContainingType, hashCode)
				If (Me <> Me.ConstructedFrom) Then
					Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = Me.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator()
					While enumerator.MoveNext()
						hashCode = Hash.Combine(Of TypeSymbol)(enumerator.Current, hashCode)
					End While
				End If
				num = hashCode
			Else
				num = hashCode
			End If
			Return num
		End Function

		Friend Function GetMemberForDefinition(ByVal member As Symbol) As Symbol
			Return Me.SubstituteTypeParametersInMember(member)
		End Function

		Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			Return Me.GetMembers_Worker(Me.OriginalDefinition.GetMembers())
		End Function

		Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Dim func As Func(Of Symbol, SubstitutedNamedType, Symbol)
			Dim members As ImmutableArray(Of Symbol) = Me.OriginalDefinition.GetMembers(name)
			If (SubstitutedNamedType._Closure$__.$I86-0 Is Nothing) Then
				func = Function(member As Symbol, self As SubstitutedNamedType) self.SubstituteTypeParametersInMember(member)
				SubstitutedNamedType._Closure$__.$I86-0 = func
			Else
				func = SubstitutedNamedType._Closure$__.$I86-0
			End If
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of Symbol, SubstitutedNamedType, Symbol)(members, func, Me)
		End Function

		Private Function GetMembers_Worker(ByVal members As ImmutableArray(Of Symbol)) As ImmutableArray(Of Symbol)
			Dim current As Symbol
			Dim func As Func(Of MethodSymbol, MethodSymbol)
			Dim substitutedFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedFieldSymbol
			Dim substitutedFieldSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedFieldSymbol
			Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
			Dim methodSymbols As IEnumerable(Of MethodSymbol) = members.OfType(Of MethodSymbol)()
			If (SubstitutedNamedType._Closure$__.$I83-0 Is Nothing) Then
				func = Function(m As MethodSymbol) m
				SubstitutedNamedType._Closure$__.$I83-0 = func
			Else
				func = SubstitutedNamedType._Closure$__.$I83-0
			End If
			Dim dictionary As Dictionary(Of MethodSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol) = methodSymbols.ToDictionary(Of MethodSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol)(func, Function(m As MethodSymbol) Me.SubstituteTypeParametersForMemberMethod(m))
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = members.GetEnumerator()
			While True
				If (Not enumerator.MoveNext()) Then
					Return instance.ToImmutableAndFree()
				End If
				current = enumerator.Current
				Dim kind As SymbolKind = current.Kind
				Select Case kind
					Case SymbolKind.[Event]
						Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol)
						Dim methodSubstitute As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol = SubstitutedNamedType.GetMethodSubstitute(dictionary, eventSymbol.AddMethod)
						Dim substitutedMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol = SubstitutedNamedType.GetMethodSubstitute(dictionary, eventSymbol.RemoveMethod)
						Dim methodSubstitute1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol = SubstitutedNamedType.GetMethodSubstitute(dictionary, eventSymbol.RaiseMethod)
						If (eventSymbol.AssociatedField Is Nothing) Then
							substitutedFieldSymbol = Nothing
						Else
							substitutedFieldSymbol = Me.SubstituteTypeParametersForMemberField(eventSymbol.AssociatedField)
						End If
						Dim substitutedFieldSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedFieldSymbol = substitutedFieldSymbol
						instance.Add(Me.CreateSubstitutedEventSymbol(eventSymbol, methodSubstitute, substitutedMethodSymbol, methodSubstitute1, substitutedFieldSymbol2))
						Continue While
					Case SymbolKind.Field
						instance.Add(Me.SubstituteTypeParametersForMemberField(DirectCast(current, FieldSymbol)))
						Continue While
					Case SymbolKind.Label
					Case SymbolKind.Local
					Case SymbolKind.NetModule
						Exit Select
					Case SymbolKind.Method
						instance.Add(dictionary(DirectCast(current, MethodSymbol)))
						Continue While
					Case SymbolKind.NamedType
						instance.Add(Me.SubstituteTypeParametersForMemberType(DirectCast(current, NamedTypeSymbol)))
						Continue While
					Case Else
						If (kind = SymbolKind.[Property]) Then
							Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
							Dim substitutedMethodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol = SubstitutedNamedType.GetMethodSubstitute(dictionary, propertySymbol.GetMethod)
							Dim methodSubstitute2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol = SubstitutedNamedType.GetMethodSubstitute(dictionary, propertySymbol.SetMethod)
							If (propertySymbol.AssociatedField Is Nothing) Then
								substitutedFieldSymbol1 = Nothing
							Else
								substitutedFieldSymbol1 = Me.SubstituteTypeParametersForMemberField(propertySymbol.AssociatedField)
							End If
							instance.Add(New SubstitutedPropertySymbol(Me, propertySymbol, substitutedMethodSymbol1, methodSubstitute2, substitutedFieldSymbol1))
							Continue While
						End If

				End Select
			End While
			Throw ExceptionUtilities.UnexpectedValue(current.Kind)
		End Function

		Friend Overrides Function GetMembersUnordered() As ImmutableArray(Of Symbol)
			Return Me.GetMembers_Worker(Me.OriginalDefinition.GetMembersUnordered())
		End Function

		Private Shared Function GetMethodSubstitute(ByVal methodSubstitutions As Dictionary(Of MethodSymbol, SubstitutedMethodSymbol), ByVal method As MethodSymbol) As SubstitutedMethodSymbol
			If (method Is Nothing) Then
				Return Nothing
			End If
			Return methodSubstitutions(method)
		End Function

		Friend NotOverridable Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Return Me.OriginalDefinition.GetSecurityInformation()
		End Function

		Friend NotOverridable Overrides Function GetSynthesizedWithEventsOverrides() As IEnumerable(Of PropertySymbol)
			Return New SubstitutedNamedType.VB$StateMachine_98_GetSynthesizedWithEventsOverrides(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Dim func As Func(Of NamedTypeSymbol, SubstitutedNamedType, NamedTypeSymbol)
			Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me.OriginalDefinition.GetTypeMembers()
			If (SubstitutedNamedType._Closure$__.$I88-0 Is Nothing) Then
				func = Function(nestedType As NamedTypeSymbol, self As SubstitutedNamedType) self.SubstituteTypeParametersForMemberType(nestedType)
				SubstitutedNamedType._Closure$__.$I88-0 = func
			Else
				func = SubstitutedNamedType._Closure$__.$I88-0
			End If
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of NamedTypeSymbol, SubstitutedNamedType, NamedTypeSymbol)(typeMembers, func, Me)
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Dim func As Func(Of NamedTypeSymbol, SubstitutedNamedType, NamedTypeSymbol)
			Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me.OriginalDefinition.GetTypeMembers(name)
			If (SubstitutedNamedType._Closure$__.$I89-0 Is Nothing) Then
				func = Function(nestedType As NamedTypeSymbol, self As SubstitutedNamedType) self.SubstituteTypeParametersForMemberType(nestedType)
				SubstitutedNamedType._Closure$__.$I89-0 = func
			Else
				func = SubstitutedNamedType._Closure$__.$I89-0
			End If
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of NamedTypeSymbol, SubstitutedNamedType, NamedTypeSymbol)(typeMembers, func, Me)
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
			Dim func As Func(Of NamedTypeSymbol, SubstitutedNamedType, NamedTypeSymbol)
			Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me.OriginalDefinition.GetTypeMembers(name, arity)
			If (SubstitutedNamedType._Closure$__.$I90-0 Is Nothing) Then
				func = Function(nestedType As NamedTypeSymbol, self As SubstitutedNamedType) self.SubstituteTypeParametersForMemberType(nestedType)
				SubstitutedNamedType._Closure$__.$I90-0 = func
			Else
				func = SubstitutedNamedType._Closure$__.$I90-0
			End If
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of NamedTypeSymbol, SubstitutedNamedType, NamedTypeSymbol)(typeMembers, func, Me)
		End Function

		Friend Overrides Function GetTypeMembersUnordered() As ImmutableArray(Of NamedTypeSymbol)
			Dim func As Func(Of NamedTypeSymbol, SubstitutedNamedType, NamedTypeSymbol)
			Dim typeMembersUnordered As ImmutableArray(Of NamedTypeSymbol) = Me.OriginalDefinition.GetTypeMembersUnordered()
			If (SubstitutedNamedType._Closure$__.$I87-0 Is Nothing) Then
				func = Function(nestedType As NamedTypeSymbol, self As SubstitutedNamedType) self.SubstituteTypeParametersForMemberType(nestedType)
				SubstitutedNamedType._Closure$__.$I87-0 = func
			Else
				func = SubstitutedNamedType._Closure$__.$I87-0
			End If
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of NamedTypeSymbol, SubstitutedNamedType, NamedTypeSymbol)(typeMembersUnordered, func, Me)
		End Function

		Friend NotOverridable Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim baseTypeNoUseSiteDiagnostics As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.OriginalDefinition.BaseTypeNoUseSiteDiagnostics
			If (baseTypeNoUseSiteDiagnostics Is Nothing) Then
				namedTypeSymbol = Nothing
			Else
				Dim typeWithModifier As TypeWithModifiers = baseTypeNoUseSiteDiagnostics.InternalSubstituteTypeParameters(Me._substitution)
				namedTypeSymbol = DirectCast(typeWithModifier.AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			End If
			Return namedTypeSymbol
		End Function

		Friend NotOverridable Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Dim empty As ImmutableArray(Of NamedTypeSymbol)
			Dim interfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = Me.OriginalDefinition.InterfacesNoUseSiteDiagnostics
			If (interfacesNoUseSiteDiagnostics.Length <> 0) Then
				Dim namedTypeSymbolArray(interfacesNoUseSiteDiagnostics.Length - 1 + 1 - 1) As NamedTypeSymbol
				Dim length As Integer = interfacesNoUseSiteDiagnostics.Length - 1
				Dim num As Integer = 0
				Do
					Dim typeWithModifier As TypeWithModifiers = interfacesNoUseSiteDiagnostics(num).InternalSubstituteTypeParameters(Me._substitution)
					namedTypeSymbolArray(num) = DirectCast(typeWithModifier.AsTypeSymbolOnly(), NamedTypeSymbol)
					num = num + 1
				Loop While num <= length
				empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of NamedTypeSymbol)(namedTypeSymbolArray)
			Else
				empty = ImmutableArray(Of NamedTypeSymbol).Empty
			End If
			Return empty
		End Function

		Friend NotOverridable Overrides Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Dim typeWithModifier As TypeWithModifiers = Me.OriginalDefinition.GetDeclaredBase(basesBeingResolved).InternalSubstituteTypeParameters(Me._substitution)
			Return DirectCast(typeWithModifier.AsTypeSymbolOnly(), NamedTypeSymbol)
		End Function

		Friend NotOverridable Overrides Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Dim empty As ImmutableArray(Of NamedTypeSymbol)
			Dim declaredInterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = Me.OriginalDefinition.GetDeclaredInterfacesNoUseSiteDiagnostics(basesBeingResolved)
			If (declaredInterfacesNoUseSiteDiagnostics.Length <> 0) Then
				Dim namedTypeSymbolArray(declaredInterfacesNoUseSiteDiagnostics.Length - 1 + 1 - 1) As NamedTypeSymbol
				Dim length As Integer = declaredInterfacesNoUseSiteDiagnostics.Length - 1
				Dim num As Integer = 0
				Do
					Dim typeWithModifier As TypeWithModifiers = declaredInterfacesNoUseSiteDiagnostics(num).InternalSubstituteTypeParameters(Me._substitution)
					namedTypeSymbolArray(num) = DirectCast(typeWithModifier.AsTypeSymbolOnly(), NamedTypeSymbol)
					num = num + 1
				Loop While num <= length
				empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of NamedTypeSymbol)(namedTypeSymbolArray)
			Else
				empty = ImmutableArray(Of NamedTypeSymbol).Empty
			End If
			Return empty
		End Function

		Private Function SubstituteTypeParametersForMemberEvent(ByVal memberEvent As EventSymbol) As SubstitutedEventSymbol
			Dim substitutedMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol
			Dim substitutedMethodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol
			Dim substitutedMethodSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol
			Dim substitutedFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedFieldSymbol
			If (memberEvent.AddMethod Is Nothing) Then
				substitutedMethodSymbol = Nothing
			Else
				substitutedMethodSymbol = Me.SubstituteTypeParametersForMemberMethod(memberEvent.AddMethod)
			End If
			Dim substitutedMethodSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol = substitutedMethodSymbol
			If (memberEvent.RemoveMethod Is Nothing) Then
				substitutedMethodSymbol1 = Nothing
			Else
				substitutedMethodSymbol1 = Me.SubstituteTypeParametersForMemberMethod(memberEvent.RemoveMethod)
			End If
			Dim substitutedMethodSymbol4 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol = substitutedMethodSymbol1
			If (memberEvent.RaiseMethod Is Nothing) Then
				substitutedMethodSymbol2 = Nothing
			Else
				substitutedMethodSymbol2 = Me.SubstituteTypeParametersForMemberMethod(memberEvent.RaiseMethod)
			End If
			Dim substitutedMethodSymbol5 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol = substitutedMethodSymbol2
			If (memberEvent.AssociatedField Is Nothing) Then
				substitutedFieldSymbol = Nothing
			Else
				substitutedFieldSymbol = Me.SubstituteTypeParametersForMemberField(memberEvent.AssociatedField)
			End If
			Return Me.CreateSubstitutedEventSymbol(memberEvent, substitutedMethodSymbol3, substitutedMethodSymbol4, substitutedMethodSymbol5, substitutedFieldSymbol)
		End Function

		Protected Overridable Function SubstituteTypeParametersForMemberField(ByVal memberField As FieldSymbol) As SubstitutedFieldSymbol
			Return New SubstitutedFieldSymbol(Me, memberField)
		End Function

		Protected Overridable Function SubstituteTypeParametersForMemberMethod(ByVal memberMethod As MethodSymbol) As SubstitutedMethodSymbol
			Dim specializedNonGenericMethod As SubstitutedMethodSymbol
			If (memberMethod.Arity <= 0) Then
				specializedNonGenericMethod = New SubstitutedMethodSymbol.SpecializedNonGenericMethod(Me, memberMethod)
			Else
				specializedNonGenericMethod = SubstitutedMethodSymbol.SpecializedGenericMethod.Create(Me, memberMethod)
			End If
			Return specializedNonGenericMethod
		End Function

		Private Function SubstituteTypeParametersForMemberProperty(ByVal memberProperty As PropertySymbol) As SubstitutedPropertySymbol
			Dim substitutedMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol
			Dim substitutedMethodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol
			Dim substitutedFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedFieldSymbol
			If (memberProperty.GetMethod Is Nothing) Then
				substitutedMethodSymbol = Nothing
			Else
				substitutedMethodSymbol = Me.SubstituteTypeParametersForMemberMethod(memberProperty.GetMethod)
			End If
			Dim substitutedMethodSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol = substitutedMethodSymbol
			If (memberProperty.SetMethod Is Nothing) Then
				substitutedMethodSymbol1 = Nothing
			Else
				substitutedMethodSymbol1 = Me.SubstituteTypeParametersForMemberMethod(memberProperty.SetMethod)
			End If
			Dim substitutedMethodSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedMethodSymbol = substitutedMethodSymbol1
			If (memberProperty.AssociatedField Is Nothing) Then
				substitutedFieldSymbol = Nothing
			Else
				substitutedFieldSymbol = Me.SubstituteTypeParametersForMemberField(memberProperty.AssociatedField)
			End If
			Return New SubstitutedPropertySymbol(Me, memberProperty, substitutedMethodSymbol2, substitutedMethodSymbol3, substitutedFieldSymbol)
		End Function

		Private Function SubstituteTypeParametersForMemberType(ByVal memberType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (memberType.Arity <> 0) Then
				namedTypeSymbol = SubstitutedNamedType.SpecializedGenericType.Create(Me, memberType)
			Else
				namedTypeSymbol = SubstitutedNamedType.SpecializedNonGenericType.Create(Me, memberType, Me._substitution)
			End If
			Return namedTypeSymbol
		End Function

		Private Function SubstituteTypeParametersInMember(ByVal member As Symbol) As Symbol
			Dim addMethod As Symbol
			Dim kind As SymbolKind = member.Kind
			Select Case kind
				Case SymbolKind.[Event]
					addMethod = Me.SubstituteTypeParametersForMemberEvent(DirectCast(member, EventSymbol))
					Exit Select
				Case SymbolKind.Field
					addMethod = Me.SubstituteTypeParametersForMemberField(DirectCast(member, FieldSymbol))
					Exit Select
				Case SymbolKind.Label
				Case SymbolKind.Local
				Case SymbolKind.NetModule
					Throw ExceptionUtilities.UnexpectedValue(member.Kind)
				Case SymbolKind.Method
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(member, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					Select Case methodSymbol.MethodKind
						Case MethodKind.EventAdd
							addMethod = Me.SubstituteTypeParametersForMemberEvent(DirectCast(methodSymbol.AssociatedSymbol, EventSymbol)).AddMethod

						Case MethodKind.EventRaise
							addMethod = Me.SubstituteTypeParametersForMemberEvent(DirectCast(methodSymbol.AssociatedSymbol, EventSymbol)).RaiseMethod

						Case MethodKind.EventRemove
							addMethod = Me.SubstituteTypeParametersForMemberEvent(DirectCast(methodSymbol.AssociatedSymbol, EventSymbol)).RemoveMethod

						Case MethodKind.ExplicitInterfaceImplementation
						Case MethodKind.UserDefinedOperator
						Case MethodKind.Ordinary
						Label0:
							addMethod = Me.SubstituteTypeParametersForMemberMethod(methodSymbol)

						Case MethodKind.PropertyGet
						Case MethodKind.PropertySet
							Dim substitutedPropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedPropertySymbol = Me.SubstituteTypeParametersForMemberProperty(DirectCast(methodSymbol.AssociatedSymbol, PropertySymbol))
							addMethod = If(methodSymbol.MethodKind = MethodKind.PropertyGet, substitutedPropertySymbol.GetMethod, substitutedPropertySymbol.SetMethod)

						Case Else
							GoTo Label0
					End Select

				Case SymbolKind.NamedType
					addMethod = Me.SubstituteTypeParametersForMemberType(DirectCast(member, NamedTypeSymbol))
					Exit Select
				Case Else
					If (kind = SymbolKind.[Property]) Then
						addMethod = Me.SubstituteTypeParametersForMemberProperty(DirectCast(member, PropertySymbol))
						Exit Select
					Else
						Throw ExceptionUtilities.UnexpectedValue(member.Kind)
					End If
			End Select
			Return addMethod
		End Function

		Friend Class ConstructedInstanceType
			Inherits SubstitutedNamedType.ConstructedType
			Public Overrides ReadOnly Property ConstructedFrom As NamedTypeSymbol
				Get
					Return MyBase.OriginalDefinition
				End Get
			End Property

			Public Sub New(ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution)
				MyBase.New(substitution)
			End Sub

			Friend Overrides Function InternalSubstituteTypeParameters(ByVal additionalSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As TypeWithModifiers
				Return New TypeWithModifiers(Me.InternalSubstituteTypeParametersInConstructedInstanceType(additionalSubstitution))
			End Function

			Private Function InternalSubstituteTypeParametersInConstructedInstanceType(ByVal additionalSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
				If (additionalSubstitution IsNot Nothing) Then
					Dim originalDefinition1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = MyBase.OriginalDefinition
					Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = originalDefinition1.ContainingType
					If (containingType Is Nothing) Then
						namedTypeSymbol = Nothing
					Else
						namedTypeSymbol = DirectCast(containingType.InternalSubstituteTypeParameters(additionalSubstitution).AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					End If
					If (CObj(namedTypeSymbol) = CObj(containingType)) Then
						typeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.AdjustForConstruct(Nothing, Me._substitution, additionalSubstitution)
						If (typeSubstitution IsNot Nothing) Then
							originalDefinition = If(typeSubstitution = Me._substitution, Me, New SubstitutedNamedType.ConstructedInstanceType(typeSubstitution))
						Else
							originalDefinition = MyBase.OriginalDefinition
						End If
					Else
						Dim specializedGenericType As SubstitutedNamedType.SpecializedGenericType = SubstitutedNamedType.SpecializedGenericType.Create(namedTypeSymbol, originalDefinition1)
						typeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.AdjustForConstruct(namedTypeSymbol.TypeSubstitution, Me._substitution, additionalSubstitution)
						originalDefinition = New SubstitutedNamedType.ConstructedSpecializedGenericType(specializedGenericType, typeSubstitution)
					End If
				Else
					originalDefinition = Me
				End If
				Return originalDefinition
			End Function
		End Class

		Friend Class ConstructedSpecializedGenericType
			Inherits SubstitutedNamedType.ConstructedType
			Private ReadOnly _constructedFrom As SubstitutedNamedType.SpecializedGenericType

			Public Overrides ReadOnly Property ConstructedFrom As NamedTypeSymbol
				Get
					Return Me._constructedFrom
				End Get
			End Property

			Public Sub New(ByVal constructedFrom As SubstitutedNamedType.SpecializedGenericType, ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution)
				MyBase.New(substitution)
				Me._constructedFrom = constructedFrom
			End Sub

			Friend Overrides Function InternalSubstituteTypeParameters(ByVal additionalSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As TypeWithModifiers
				Return New TypeWithModifiers(Me.InternalSubstituteTypeParametersInConstructedSpecializedGenericType(additionalSubstitution))
			End Function

			Private Function InternalSubstituteTypeParametersInConstructedSpecializedGenericType(ByVal additionalSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim constructedInstanceType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				If (additionalSubstitution IsNot Nothing) Then
					Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._constructedFrom.OriginalDefinition
					Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._constructedFrom.ContainingType
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(containingType.InternalSubstituteTypeParameters(additionalSubstitution).AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.AdjustForConstruct(namedTypeSymbol.TypeSubstitution, Me._substitution, additionalSubstitution)
					If (typeSubstitution Is Nothing) Then
						constructedInstanceType = originalDefinition
					ElseIf (Not namedTypeSymbol.IsDefinition) Then
						Dim specializedGenericType As SubstitutedNamedType.SpecializedGenericType = Me._constructedFrom
						If (CObj(namedTypeSymbol) <> CObj(containingType)) Then
							specializedGenericType = SubstitutedNamedType.SpecializedGenericType.Create(namedTypeSymbol, originalDefinition)
						End If
						constructedInstanceType = If(CObj(specializedGenericType) <> CObj(Me._constructedFrom) OrElse typeSubstitution <> Me._substitution, New SubstitutedNamedType.ConstructedSpecializedGenericType(specializedGenericType, typeSubstitution), Me)
					Else
						constructedInstanceType = New SubstitutedNamedType.ConstructedInstanceType(typeSubstitution)
					End If
				Else
					constructedInstanceType = Me
				End If
				Return constructedInstanceType
			End Function
		End Class

		Friend MustInherit Class ConstructedType
			Inherits SubstitutedNamedType
			Private ReadOnly _typeArguments As ImmutableArray(Of TypeSymbol)

			Private ReadOnly _hasTypeArgumentsCustomModifiers As Boolean

			Friend Overrides ReadOnly Property CanConstruct As Boolean
				Get
					Return False
				End Get
			End Property

			Public NotOverridable Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me.ConstructedFrom.ContainingSymbol
				End Get
			End Property

			Friend NotOverridable Overrides ReadOnly Property HasTypeArgumentsCustomModifiers As Boolean
				Get
					Return Me._hasTypeArgumentsCustomModifiers
				End Get
			End Property

			Public Overrides ReadOnly Property IsAnonymousType As Boolean
				Get
					Return Me.ConstructedFrom.IsAnonymousType
				End Get
			End Property

			Friend NotOverridable Overrides ReadOnly Property TypeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
				Get
					Return Me._typeArguments
				End Get
			End Property

			Public NotOverridable Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return Me.ConstructedFrom.TypeParameters
				End Get
			End Property

			Protected Sub New(ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution)
				MyBase.New(substitution)
				Me._typeArguments = substitution.GetTypeArgumentsFor(MyBase.OriginalDefinition, Me._hasTypeArgumentsCustomModifiers)
			End Sub

			Public Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As NamedTypeSymbol
				Throw New InvalidOperationException()
			End Function

			Public NotOverridable Overrides Function GetTypeArgumentCustomModifiers(ByVal ordinal As Integer) As ImmutableArray(Of CustomModifier)
				Dim emptyTypeArgumentCustomModifiers As ImmutableArray(Of CustomModifier)
				If (Not Me._hasTypeArgumentsCustomModifiers) Then
					emptyTypeArgumentCustomModifiers = MyBase.GetEmptyTypeArgumentCustomModifiers(ordinal)
				Else
					Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Me._substitution
					Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = MyBase.OriginalDefinition.TypeParameters
					emptyTypeArgumentCustomModifiers = typeSubstitution.GetTypeArgumentsCustomModifiersFor(typeParameters(ordinal))
				End If
				Return emptyTypeArgumentCustomModifiers
			End Function

			Friend NotOverridable Overrides Function GetUnificationUseSiteDiagnosticRecursive(ByVal owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As DiagnosticInfo
				Dim unificationUseSiteDiagnosticRecursive As DiagnosticInfo = If(Me.ConstructedFrom.GetUnificationUseSiteDiagnosticRecursive(owner, checkedTypes), Symbol.GetUnificationUseSiteDiagnosticRecursive(Of TypeSymbol)(Me._typeArguments, owner, checkedTypes))
				If (unificationUseSiteDiagnosticRecursive Is Nothing AndAlso Me._hasTypeArgumentsCustomModifiers) Then
					Dim arity As Integer = MyBase.Arity - 1
					For i As Integer = 0 To arity
						unificationUseSiteDiagnosticRecursive = Symbol.GetUnificationUseSiteDiagnosticRecursive(Me.GetTypeArgumentCustomModifiers(i), owner, checkedTypes)
						If (unificationUseSiteDiagnosticRecursive IsNot Nothing) Then
							Exit For
						End If
					Next

				End If
				Return unificationUseSiteDiagnosticRecursive
			End Function
		End Class

		Friend Class SpecializedGenericType
			Inherits SubstitutedNamedType.SpecializedType
			Private ReadOnly _typeParameters As ImmutableArray(Of TypeParameterSymbol)

			Friend Overrides ReadOnly Property CanConstruct As Boolean
				Get
					Dim flag As Boolean
					Dim containingType As NamedTypeSymbol = Me._container
					While True
						If (containingType.Arity <= 0) Then
							containingType = containingType.ContainingType
							If (containingType Is Nothing OrElse containingType.IsDefinition) Then
								flag = True
								Exit While
							End If
						ElseIf (CObj(containingType.ConstructedFrom) <> CObj(containingType)) Then
							flag = True
							Exit While
						Else
							flag = False
							Exit While
						End If
					End While
					Return flag
				End Get
			End Property

			Friend NotOverridable Overrides ReadOnly Property HasTypeArgumentsCustomModifiers As Boolean
				Get
					Return False
				End Get
			End Property

			Friend Overrides ReadOnly Property TypeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
				Get
					Return StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me.TypeParameters)
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return Me._typeParameters
				End Get
			End Property

			Private Sub New(ByVal container As NamedTypeSymbol, ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal typeParameters As ImmutableArray(Of SubstitutedTypeParameterSymbol))
				MyBase.New(container, substitution)
				Me._typeParameters = StaticCast(Of TypeParameterSymbol).From(Of SubstitutedTypeParameterSymbol)(typeParameters)
				Dim enumerator As ImmutableArray(Of SubstitutedTypeParameterSymbol).Enumerator = typeParameters.GetEnumerator()
				While enumerator.MoveNext()
					enumerator.Current.SetContainingSymbol(Me)
				End While
			End Sub

			Public Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As NamedTypeSymbol
				Dim constructedSpecializedGenericType As NamedTypeSymbol
				MyBase.CheckCanConstructAndTypeArguments(typeArguments)
				typeArguments = typeArguments.TransformToCanonicalFormFor(Me)
				If (Not typeArguments.IsDefault) Then
					Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(Me._substitution.Parent, MyBase.OriginalDefinition, typeArguments, True)
					constructedSpecializedGenericType = New SubstitutedNamedType.ConstructedSpecializedGenericType(Me, typeSubstitution)
				Else
					constructedSpecializedGenericType = Me
				End If
				Return constructedSpecializedGenericType
			End Function

			Public Shared Function Create(ByVal container As NamedTypeSymbol, ByVal fullInstanceType As NamedTypeSymbol) As SubstitutedNamedType.SpecializedGenericType
				Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = fullInstanceType.TypeParameters
				Dim substitutedTypeParameterSymbol(typeParameters.Length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol
				Dim length As Integer = typeParameters.Length - 1
				Dim num As Integer = 0
				Do
					substitutedTypeParameterSymbol(num) = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol(typeParameters(num))
					num = num + 1
				Loop While num <= length
				Dim substitutedTypeParameterSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol)(substitutedTypeParameterSymbol)
				Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.CreateForAlphaRename(container.TypeSubstitution, substitutedTypeParameterSymbols)
				Return New SubstitutedNamedType.SpecializedGenericType(container, typeSubstitution, substitutedTypeParameterSymbols)
			End Function

			Public NotOverridable Overrides Function GetTypeArgumentCustomModifiers(ByVal ordinal As Integer) As ImmutableArray(Of CustomModifier)
				Return MyBase.GetEmptyTypeArgumentCustomModifiers(ordinal)
			End Function

			Friend Overrides Function InternalSubstituteTypeParameters(ByVal additionalSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As TypeWithModifiers
				Throw ExceptionUtilities.Unreachable
			End Function
		End Class

		Friend Class SpecializedNonGenericType
			Inherits SubstitutedNamedType.SpecializedType
			Friend Overrides ReadOnly Property CanConstruct As Boolean
				Get
					Return False
				End Get
			End Property

			Friend NotOverridable Overrides ReadOnly Property HasTypeArgumentsCustomModifiers As Boolean
				Get
					Return False
				End Get
			End Property

			Friend Overrides ReadOnly Property TypeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
				Get
					Return ImmutableArray(Of TypeSymbol).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return ImmutableArray(Of TypeParameterSymbol).Empty
				End Get
			End Property

			Private Sub New(ByVal container As NamedTypeSymbol, ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution)
				MyBase.New(container, substitution)
			End Sub

			Public Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As NamedTypeSymbol
				Throw New InvalidOperationException()
			End Function

			Public Shared Function Create(ByVal container As NamedTypeSymbol, ByVal fullInstanceType As NamedTypeSymbol, ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As SubstitutedNamedType.SpecializedType
				Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = container.TypeSubstitution
				If (substitution.TargetGenericDefinition <> fullInstanceType) Then
					substitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(fullInstanceType, typeSubstitution, Nothing)
				ElseIf (substitution.Parent <> typeSubstitution) Then
					substitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(fullInstanceType, typeSubstitution, Nothing)
				End If
				Return New SubstitutedNamedType.SpecializedNonGenericType(container, substitution)
			End Function

			Public NotOverridable Overrides Function GetTypeArgumentCustomModifiers(ByVal ordinal As Integer) As ImmutableArray(Of CustomModifier)
				Return MyBase.GetEmptyTypeArgumentCustomModifiers(ordinal)
			End Function

			Friend Overrides Function InternalSubstituteTypeParameters(ByVal additionalSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As TypeWithModifiers
				Return New TypeWithModifiers(Me.InternalSubstituteTypeParametersInSpecializedNonGenericType(additionalSubstitution))
			End Function

			Private Function InternalSubstituteTypeParametersInSpecializedNonGenericType(ByVal additionalSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				If (additionalSubstitution IsNot Nothing) Then
					Dim typeWithModifier As TypeWithModifiers = Me._container.InternalSubstituteTypeParameters(additionalSubstitution)
					Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(typeWithModifier.AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					If (CObj(namedTypeSymbol1) = CObj(Me._container)) Then
						namedTypeSymbol = Me
					Else
						Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = MyBase.OriginalDefinition
						If (Not namedTypeSymbol1.IsDefinition) Then
							namedTypeSymbol = SubstitutedNamedType.SpecializedNonGenericType.Create(namedTypeSymbol1, originalDefinition, namedTypeSymbol1.TypeSubstitution)
						Else
							namedTypeSymbol = originalDefinition
						End If
					End If
				Else
					namedTypeSymbol = Me
				End If
				Return namedTypeSymbol
			End Function
		End Class

		Friend MustInherit Class SpecializedType
			Inherits SubstitutedNamedType
			Protected ReadOnly _container As NamedTypeSymbol

			Public Overrides ReadOnly Property ConstructedFrom As NamedTypeSymbol
				Get
					Return Me
				End Get
			End Property

			Public NotOverridable Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me._container
				End Get
			End Property

			Public Shadows ReadOnly Property ContainingType As NamedTypeSymbol
				Get
					Return Me._container
				End Get
			End Property

			Protected Sub New(ByVal container As NamedTypeSymbol, ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution)
				MyBase.New(substitution)
				Me._container = container
			End Sub

			Friend NotOverridable Overrides Function GetUnificationUseSiteDiagnosticRecursive(ByVal owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As DiagnosticInfo
				Return Nothing
			End Function
		End Class
	End Class
End Namespace