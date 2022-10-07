Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SourceFieldSymbol
		Inherits FieldSymbol
		Implements IAttributeTargetSymbol
		Protected ReadOnly m_memberFlags As SourceMemberFlags

		Private ReadOnly _containingType As SourceMemberContainerTypeSymbol

		Private ReadOnly _name As String

		Private ReadOnly _syntaxRef As SyntaxReference

		Private _lazyDocComment As String

		Private _lazyExpandedDocComment As String

		Private _lazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData)

		Private _eventProduced As Integer

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Public ReadOnly Property ContainingSourceType As SourceMemberContainerTypeSymbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Friend MustOverride ReadOnly Property DeclarationSyntax As VisualBasicSyntaxNode

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return DirectCast((Me.m_memberFlags And SourceMemberFlags.AccessibilityMask), Accessibility)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Symbol.GetDeclaringSyntaxReferenceHelper(Me._syntaxRef)
			End Get
		End Property

		Public ReadOnly Property DefaultAttributeLocation As AttributeLocation Implements IAttributeTargetSymbol.DefaultAttributeLocation
			Get
				Return AttributeLocation.Field
			End Get
		End Property

		Friend Overridable ReadOnly Property EqualsValueOrAsNewInitOpt As VisualBasicSyntaxNode
			Get
				Return Nothing
			End Get
		End Property

		Friend MustOverride ReadOnly Property GetAttributeDeclarations As OneOrMany(Of SyntaxList(Of AttributeListSyntax))

		Friend Overrides ReadOnly Property HasDeclaredType As Boolean
			Get
				Return (Me.m_memberFlags And SourceMemberFlags.InferredFieldType) = SourceMemberFlags.None
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return EmbeddedOperators.CompareString(Me.Name, "value__", False) = 0
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Dim flag As Boolean
				If (Not Me.HasRuntimeSpecialName) Then
					Dim decodedWellKnownAttributeData As CommonFieldWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
					flag = If(decodedWellKnownAttributeData Is Nothing, False, decodedWellKnownAttributeData.HasSpecialNameAttribute)
				Else
					flag = True
				End If
				Return flag
			End Get
		End Property

		Public Overrides ReadOnly Property IsConst As Boolean
			Get
				Return (Me.m_memberFlags And SourceMemberFlags.[Const]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._containingType.AreMembersImplicitlyDeclared
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsNotSerialized As Boolean
			Get
				Dim decodedWellKnownAttributeData As CommonFieldWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasNonSerializedAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property IsReadOnly As Boolean
			Get
				Return (Me.m_memberFlags And SourceMemberFlags.[ReadOnly]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return (Me.m_memberFlags And SourceMemberFlags.[Shared]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray.Create(Of Location)(SourceFieldSymbol.GetSymbolLocation(Me._syntaxRef))
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Dim decodedWellKnownAttributeData As CommonFieldWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return Nothing
				End If
				Return decodedWellKnownAttributeData.MarshallingInformation
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Dim uninitialized As Microsoft.CodeAnalysis.ObsoleteAttributeData
				Dim obsoleteAttributeDatum As Microsoft.CodeAnalysis.ObsoleteAttributeData
				If (Me._containingType.AnyMemberHasAttributes) Then
					Dim customAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me._lazyCustomAttributesBag
					If (customAttributesBag Is Nothing OrElse Not customAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed) Then
						uninitialized = Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized
					Else
						Dim earlyDecodedWellKnownAttributeData As CommonFieldEarlyWellKnownAttributeData = DirectCast(Me._lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData, CommonFieldEarlyWellKnownAttributeData)
						If (earlyDecodedWellKnownAttributeData IsNot Nothing) Then
							obsoleteAttributeDatum = earlyDecodedWellKnownAttributeData.ObsoleteAttributeData
						Else
							obsoleteAttributeDatum = Nothing
						End If
						uninitialized = obsoleteAttributeDatum
					End If
				Else
					uninitialized = Nothing
				End If
				Return uninitialized
			End Get
		End Property

		Friend Overrides ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return (Me.m_memberFlags And SourceMemberFlags.[Shadows]) <> SourceMemberFlags.None
			End Get
		End Property

		Friend ReadOnly Property Syntax As VisualBasicSyntaxNode
			Get
				Return Me._syntaxRef.GetVisualBasicSyntax(New CancellationToken())
			End Get
		End Property

		Friend ReadOnly Property SyntaxTree As Microsoft.CodeAnalysis.SyntaxTree
			Get
				Return Me._syntaxRef.SyntaxTree
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property TypeLayoutOffset As Nullable(Of Integer)
			Get
				Dim decodedWellKnownAttributeData As CommonFieldWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData IsNot Nothing) Then
					Return decodedWellKnownAttributeData.Offset
				End If
				Return Nothing
			End Get
		End Property

		Protected Sub New(ByVal container As SourceMemberContainerTypeSymbol, ByVal syntaxRef As SyntaxReference, ByVal name As String, ByVal memberFlags As SourceMemberFlags)
			MyBase.New()
			Me._name = name
			Me._containingType = container
			Me._syntaxRef = syntaxRef
			Me.m_memberFlags = memberFlags
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			If (Me.IsConst AndAlso Me.GetConstantValue(ConstantFieldsInProgress.Empty) IsNot Nothing) Then
				Dim decodedWellKnownAttributeData As CommonFieldWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing OrElse decodedWellKnownAttributeData.ConstValue = Microsoft.CodeAnalysis.ConstantValue.Unset) Then
					If (Me.Type.SpecialType = Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
						Dim constantValue As DateTime = DirectCast(Me.ConstantValue, DateTime)
						Dim specialType As NamedTypeSymbol = Me.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int64)
						Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
						Dim typedConstants As ImmutableArray(Of TypedConstant) = ImmutableArray.Create(Of TypedConstant)(New TypedConstant(specialType, TypedConstantKind.Primitive, constantValue.Ticks))
						Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
						Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_DateTimeConstantAttribute__ctor, typedConstants, keyValuePairs, False))
					ElseIf (Me.Type.SpecialType = Microsoft.CodeAnalysis.SpecialType.System_Decimal) Then
						Dim num As [Decimal] = CDec(Me.ConstantValue)
						Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeDecimalConstantAttribute(num))
					End If
				End If
			End If
			If (Me.Type.ContainsTupleNames()) Then
				Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeTupleNamesAttribute(Me.Type))
			End If
		End Sub

		Private Sub BindConstantTupleIfNecessary(ByVal startsCycle As Boolean)
			If (Me.GetLazyConstantTuple() Is Nothing) Then
				Dim instance As PooledHashSet(Of SourceFieldSymbol) = PooledHashSet(Of SourceFieldSymbol).GetInstance()
				Dim dependency As ConstantFieldsInProgress.Dependencies = New ConstantFieldsInProgress.Dependencies(instance)
				Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
				Dim evaluatedConstant As Microsoft.CodeAnalysis.VisualBasic.Symbols.EvaluatedConstant = Me.MakeConstantTuple(dependency, bindingDiagnosticBag)
				If (startsCycle) Then
					bindingDiagnosticBag.Clear()
					bindingDiagnosticBag.Add(ERRID.ERR_CircularEvaluation1, Me.Locations(0), New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(Me) })
				End If
				Me.SetLazyConstantTuple(evaluatedConstant, bindingDiagnosticBag)
				bindingDiagnosticBag.Free()
				instance.Free()
			End If
		End Sub

		<Conditional("DEBUG")>
		Private Shared Sub CheckGraph(ByVal graph As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo))
			Dim enumerator As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo).Enumerator = New Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo).Enumerator()
			Dim enumerator1 As ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator = New ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator()
			Dim enumerator2 As ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator = New ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator()
			Dim num As Integer = 10
			Try
				enumerator = graph.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo) = enumerator.Current
					Dim key As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = current.Key
					Dim value As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo = current.Value
					Try
						enumerator1 = value.Dependencies.GetEnumerator()
						While enumerator1.MoveNext()
							Dim sourceFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = enumerator1.Current
							Dim dependencyInfo As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo()
							graph.TryGetValue(sourceFieldSymbol, dependencyInfo)
						End While
					Finally
						DirectCast(enumerator1, IDisposable).Dispose()
					End Try
					Try
						enumerator2 = value.DependedOnBy.GetEnumerator()
						While enumerator2.MoveNext()
							Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = enumerator2.Current
							Dim dependencyInfo1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo()
							graph.TryGetValue(current1, dependencyInfo1)
						End While
					Finally
						DirectCast(enumerator2, IDisposable).Dispose()
					End Try
					num = num - 1
					If (num <> 0) Then
						Continue While
					End If
					Return
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
		End Sub

		Private Sub CreateGraph(ByVal graph As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo))
			Dim enumerator As ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator = New ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator()
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).GetInstance()
			instance.Push(Me)
			While instance.Count > 0
				Dim sourceFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = instance.Pop()
				Dim dependencyInfo As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo()
				If (Not graph.TryGetValue(sourceFieldSymbol, dependencyInfo)) Then
					dependencyInfo = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo() With
					{
						.DependedOnBy = ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Empty
					}
				ElseIf (dependencyInfo.Dependencies IsNot Nothing) Then
					Continue While
				End If
				Dim constantValueDependencies As ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) = sourceFieldSymbol.GetConstantValueDependencies()
				dependencyInfo.Dependencies = constantValueDependencies
				graph(sourceFieldSymbol) = dependencyInfo
				Try
					enumerator = constantValueDependencies.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = enumerator.Current
						instance.Push(current)
						If (Not graph.TryGetValue(current, dependencyInfo)) Then
							dependencyInfo = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo() With
							{
								.DependedOnBy = ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Empty
							}
						End If
						dependencyInfo.DependedOnBy = dependencyInfo.DependedOnBy.Add(sourceFieldSymbol)
						graph(current) = dependencyInfo
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			End While
			instance.Free()
		End Sub

		Friend NotOverridable Overrides Sub DecodeWellKnownAttribute(ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation))
			Dim attribute As VisualBasicAttributeData = arguments.Attribute
			Dim diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (attribute.IsTargetAttribute(Me, AttributeDescription.TupleElementNamesAttribute)) Then
				diagnostics.Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt.Location)
			End If
			If (attribute.IsTargetAttribute(Me, AttributeDescription.SpecialNameAttribute)) Then
				arguments.GetOrCreateData(Of CommonFieldWellKnownAttributeData)().HasSpecialNameAttribute = True
				Return
			End If
			If (attribute.IsTargetAttribute(Me, AttributeDescription.NonSerializedAttribute)) Then
				If (Me.ContainingType.IsSerializable) Then
					arguments.GetOrCreateData(Of CommonFieldWellKnownAttributeData)().HasNonSerializedAttribute = True
					Return
				End If
				diagnostics.Add(ERRID.ERR_InvalidNonSerializedUsage, arguments.AttributeSyntaxOpt.GetLocation())
				Return
			End If
			If (Not attribute.IsTargetAttribute(Me, AttributeDescription.FieldOffsetAttribute)) Then
				If (attribute.IsTargetAttribute(Me, AttributeDescription.MarshalAsAttribute)) Then
					MarshalAsAttributeDecoder(Of CommonFieldWellKnownAttributeData, AttributeSyntax, VisualBasicAttributeData, AttributeLocation).Decode(arguments, AttributeTargets.Field, MessageProvider.Instance)
					Return
				End If
				If (attribute.IsTargetAttribute(Me, AttributeDescription.DateTimeConstantAttribute)) Then
					Me.VerifyConstantValueMatches(attribute.DecodeDateTimeConstantValue(), arguments)
					Return
				End If
				If (Not attribute.IsTargetAttribute(Me, AttributeDescription.DecimalConstantAttribute)) Then
					MyBase.DecodeWellKnownAttribute(arguments)
					Return
				End If
				Me.VerifyConstantValueMatches(attribute.DecodeDecimalConstantValue(), arguments)
				Return
			End If
			Dim num As Integer = attribute.CommonConstructorArguments(0).DecodeValue(Of Integer)(SpecialType.System_Int32)
			If (num < 0) Then
				Dim argumentSyntaxes As SeparatedSyntaxList(Of ArgumentSyntax) = arguments.AttributeSyntaxOpt.ArgumentList.Arguments
				diagnostics.Add(ERRID.ERR_BadAttribute1, argumentSyntaxes(0).GetLocation(), New [Object]() { attribute.AttributeClass })
				num = 0
			End If
			arguments.GetOrCreateData(Of CommonFieldWellKnownAttributeData)().SetFieldOffset(num)
		End Sub

		Friend NotOverridable Overrides Function EarlyDecodeWellKnownAttribute(ByRef arguments As EarlyDecodeWellKnownAttributeArguments(Of EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation)) As VisualBasicAttributeData
			Dim visualBasicAttributeDatum As VisualBasicAttributeData
			Dim visualBasicAttributeDatum1 As VisualBasicAttributeData = Nothing
			Dim obsoleteAttributeDatum As Microsoft.CodeAnalysis.ObsoleteAttributeData = Nothing
			If (Not MyBase.EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(arguments, visualBasicAttributeDatum1, obsoleteAttributeDatum)) Then
				visualBasicAttributeDatum = MyBase.EarlyDecodeWellKnownAttribute(arguments)
			Else
				If (obsoleteAttributeDatum IsNot Nothing) Then
					arguments.GetOrCreateData(Of CommonFieldEarlyWellKnownAttributeData)().ObsoleteAttributeData = obsoleteAttributeDatum
				End If
				visualBasicAttributeDatum = visualBasicAttributeDatum1
			End If
			Return visualBasicAttributeDatum
		End Function

		Friend Shared Function FindFieldOrWithEventsSymbolFromSyntax(ByVal variableName As SyntaxToken, ByVal tree As Microsoft.CodeAnalysis.SyntaxTree, ByVal container As NamedTypeSymbol) As Symbol
			Dim valueText As String = variableName.ValueText
			Dim fieldLocationFromSyntax As TextSpan = SourceFieldSymbol.GetFieldLocationFromSyntax(variableName)
			Return container.FindFieldOrProperty(valueText, fieldLocationFromSyntax, tree)
		End Function

		Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			MyBase.GenerateDeclarationErrors(cancellationToken)
			Dim type As TypeSymbol = Me.Type
			Me.GetConstantValue(ConstantFieldsInProgress.Empty)
			Dim containingModule As SourceModuleSymbol = DirectCast(Me.ContainingModule, SourceModuleSymbol)
			If (Interlocked.CompareExchange(Me._eventProduced, 1, 0) = 0 AndAlso Not Me.IsImplicitlyDeclared) Then
				containingModule.DeclaringCompilation.SymbolDeclaredEvent(Me)
			End If
		End Sub

		Public NotOverridable Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.GetAttributesBag().Attributes
		End Function

		Private Function GetAttributesBag() As CustomAttributesBag(Of VisualBasicAttributeData)
			If (Me._lazyCustomAttributesBag Is Nothing OrElse Not Me._lazyCustomAttributesBag.IsSealed) Then
				MyBase.LoadAndValidateAttributes(Me.GetAttributeDeclarations, Me._lazyCustomAttributesBag, AttributeLocation.None)
			End If
			Return Me._lazyCustomAttributesBag
		End Function

		Friend Overrides Function GetConstantValue(ByVal inProgress As ConstantFieldsInProgress) As Microsoft.CodeAnalysis.ConstantValue
			Return Nothing
		End Function

		Private Function GetConstantValueDependencies() As ImmutableHashSet(Of SourceFieldSymbol)
			Dim empty As ImmutableHashSet(Of SourceFieldSymbol)
			Dim sourceFieldSymbols As ImmutableHashSet(Of SourceFieldSymbol)
			Dim lazyConstantTuple As EvaluatedConstant = Me.GetLazyConstantTuple()
			If (lazyConstantTuple Is Nothing) Then
				Dim instance As PooledHashSet(Of SourceFieldSymbol) = PooledHashSet(Of SourceFieldSymbol).GetInstance()
				Dim dependency As ConstantFieldsInProgress.Dependencies = New ConstantFieldsInProgress.Dependencies(instance)
				Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
				lazyConstantTuple = Me.MakeConstantTuple(dependency, bindingDiagnosticBag)
				If (instance.Count <> 0 OrElse lazyConstantTuple.Value.IsBad OrElse bindingDiagnosticBag.HasAnyResolvedErrors()) Then
					sourceFieldSymbols = ImmutableHashSet(Of SourceFieldSymbol).Empty.Union(instance)
				Else
					Me.SetLazyConstantTuple(lazyConstantTuple, bindingDiagnosticBag)
					sourceFieldSymbols = ImmutableHashSet(Of SourceFieldSymbol).Empty
				End If
				bindingDiagnosticBag.Free()
				instance.Free()
				empty = sourceFieldSymbols
			Else
				empty = ImmutableHashSet(Of SourceFieldSymbol).Empty
			End If
			Return empty
		End Function

		Protected Function GetConstantValueImpl(ByVal inProgress As ConstantFieldsInProgress) As Microsoft.CodeAnalysis.ConstantValue
			Dim value As Microsoft.CodeAnalysis.ConstantValue
			Dim lazyConstantTuple As EvaluatedConstant = Me.GetLazyConstantTuple()
			If (lazyConstantTuple IsNot Nothing) Then
				value = lazyConstantTuple.Value
			ElseIf (inProgress.IsEmpty) Then
				Dim instance As ArrayBuilder(Of ConstantValueUtils.FieldInfo) = ArrayBuilder(Of ConstantValueUtils.FieldInfo).GetInstance()
				Me.OrderAllDependencies(instance)
				Dim enumerator As ArrayBuilder(Of ConstantValueUtils.FieldInfo).Enumerator = instance.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ConstantValueUtils.FieldInfo = enumerator.Current
					current.Field.BindConstantTupleIfNecessary(current.StartsCycle)
				End While
				instance.Free()
				value = Me.GetLazyConstantTuple().Value
			Else
				inProgress.AddDependency(Me)
				value = Microsoft.CodeAnalysis.ConstantValue.Bad
			End If
			Return value
		End Function

		Private Function GetDecodedWellKnownAttributeData() As CommonFieldWellKnownAttributeData
			Dim attributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me._lazyCustomAttributesBag
			If (attributesBag Is Nothing OrElse Not attributesBag.IsDecodedWellKnownAttributeDataComputed) Then
				attributesBag = Me.GetAttributesBag()
			End If
			Return DirectCast(attributesBag.DecodedWellKnownAttributeData, CommonFieldWellKnownAttributeData)
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Dim str As String
			str = If(Not expandIncludes, SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(Me, preferredCulture, expandIncludes, Me._lazyDocComment, cancellationToken), SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(Me, preferredCulture, expandIncludes, Me._lazyExpandedDocComment, cancellationToken))
			Return str
		End Function

		Private Shared Function GetFieldLocationFromSyntax(ByVal node As SyntaxToken) As TextSpan
			Return node.Span
		End Function

		Protected Overridable Function GetLazyConstantTuple() As EvaluatedConstant
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return New LexicalSortKey(Me._syntaxRef, Me.DeclaringCompilation)
		End Function

		Private Shared Function GetStartOfFirstCycle(ByVal graph As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo), ByRef fieldsInvolvedInCycles As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol
			Dim sourceFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol
			Dim declaringCompilation As Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol, VisualBasicCompilation)
			Dim func As Func(Of IGrouping(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol), IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol))
			If (fieldsInvolvedInCycles Is Nothing) Then
				fieldsInvolvedInCycles = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).GetInstance(graph.Count)
				Dim sourceFieldSymbols As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) = fieldsInvolvedInCycles
				Dim keys As Dictionary(Of !0, !1).KeyCollection = graph.Keys
				If (Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol._Closure$__.$I48-0 Is Nothing) Then
					declaringCompilation = Function(f As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) f.DeclaringCompilation
					Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol._Closure$__.$I48-0 = declaringCompilation
				Else
					declaringCompilation = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol._Closure$__.$I48-0
				End If
				Dim groupings As IEnumerable(Of IGrouping(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol)) = keys.GroupBy(Of VisualBasicCompilation)(declaringCompilation)
				If (Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol._Closure$__.$I48-1 Is Nothing) Then
					func = Function(g As IGrouping(Of VisualBasicCompilation, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol))
						Dim variable As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol._Closure$__48-0 = Nothing
						variable = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol._Closure$__48-0(variable) With
						{
							.$VB$Local_g = g
						}
						Return variable.$VB$Local_g.OrderByDescending(New Comparison(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol)(AddressOf variable._Lambda$__2))
					End Function
					Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol._Closure$__.$I48-1 = func
				Else
					func = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol._Closure$__.$I48-1
				End If
				sourceFieldSymbols.AddRange(groupings.SelectMany(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol)(func))
			End If
			Do
				sourceFieldSymbol = fieldsInvolvedInCycles.Pop()
			Loop While Not graph.ContainsKey(sourceFieldSymbol) OrElse Not Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.IsPartOfCycle(graph, sourceFieldSymbol)
			Return sourceFieldSymbol
		End Function

		Private Shared Function GetSymbolLocation(ByVal syntaxRef As SyntaxReference) As Location
			Dim syntax As SyntaxNode = syntaxRef.GetSyntax(New CancellationToken())
			Return syntaxRef.SyntaxTree.GetLocation(SourceFieldSymbol.GetFieldLocationFromSyntax(DirectCast(syntax, ModifiedIdentifierSyntax).Identifier))
		End Function

		Private Shared Function IsPartOfCycle(ByVal graph As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo), ByVal field As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) As Boolean
			Dim enumerator As ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator = New ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator()
			Dim instance As PooledHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) = PooledHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).GetInstance()
			Dim sourceFieldSymbols As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).GetInstance()
			Dim sourceFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = field
			Dim flag As Boolean = False
			sourceFieldSymbols.Push(field)
			While sourceFieldSymbols.Count > 0
				field = sourceFieldSymbols.Pop()
				Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo = graph(field)
				If (Not item.Dependencies.Contains(sourceFieldSymbol)) Then
					Try
						enumerator = item.Dependencies.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = enumerator.Current
							If (Not instance.Add(current)) Then
								Continue While
							End If
							sourceFieldSymbols.Push(current)
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
				Else
					flag = True
					Exit While
				End If
			End While
			sourceFieldSymbols.Free()
			instance.Free()
			Return flag
		End Function

		Protected Overridable Function MakeConstantTuple(ByVal dependencies As ConstantFieldsInProgress.Dependencies, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As EvaluatedConstant
			Throw ExceptionUtilities.Unreachable
		End Function

		Private Sub OrderAllDependencies(ByVal order As ArrayBuilder(Of ConstantValueUtils.FieldInfo))
			Dim instance As PooledDictionary(Of SourceFieldSymbol, SourceFieldSymbol.DependencyInfo) = PooledDictionary(Of SourceFieldSymbol, SourceFieldSymbol.DependencyInfo).GetInstance()
			Me.CreateGraph(instance)
			SourceFieldSymbol.OrderGraph(instance, order)
			instance.Free()
		End Sub

		Private Shared Sub OrderGraph(ByVal graph As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo), ByVal order As ArrayBuilder(Of ConstantValueUtils.FieldInfo))
			Dim enumerator As IEnumerator(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) = Nothing
			Dim enumerator1 As ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator = New ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator()
			Dim enumerator2 As ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator = New ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator()
			Dim enumerator3 As ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator = New ImmutableHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator()
			Dim pooledHashSet As PooledHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) = Nothing
			Dim sourceFieldSymbols As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) = Nothing
			While graph.Count > 0
				Dim keys As Object = pooledHashSet
				If (keys Is Nothing) Then
					keys = graph.Keys
				End If
				Dim sourceFieldSymbols1 As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) = keys
				Using instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).GetInstance()
					enumerator = sourceFieldSymbols1.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = enumerator.Current
						Dim dependencyInfo As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo()
						If (Not graph.TryGetValue(current, dependencyInfo) OrElse dependencyInfo.Dependencies.Count <> 0) Then
							Continue While
						End If
						instance.Add(current)
					End While
				End Using
				If (pooledHashSet IsNot Nothing) Then
					pooledHashSet.Free()
				End If
				pooledHashSet = Nothing
				If (instance.Count <= 0) Then
					Dim startOfFirstCycle As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.GetStartOfFirstCycle(graph, sourceFieldSymbols)
					Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo = graph(startOfFirstCycle)
					Try
						enumerator2 = item.Dependencies.GetEnumerator()
						While enumerator2.MoveNext()
							Dim sourceFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = enumerator2.Current
							Dim item1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo = graph(sourceFieldSymbol)
							item1.DependedOnBy = item1.DependedOnBy.Remove(startOfFirstCycle)
							graph(sourceFieldSymbol) = item1
						End While
					Finally
						DirectCast(enumerator2, IDisposable).Dispose()
					End Try
					item = graph(startOfFirstCycle)
					Dim instance1 As PooledHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) = PooledHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).GetInstance()
					Try
						enumerator3 = item.DependedOnBy.GetEnumerator()
						While enumerator3.MoveNext()
							Dim current1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = enumerator3.Current
							Dim dependencyInfo1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo = graph(current1)
							dependencyInfo1.Dependencies = dependencyInfo1.Dependencies.Remove(startOfFirstCycle)
							graph(current1) = dependencyInfo1
							instance1.Add(current1)
						End While
					Finally
						DirectCast(enumerator3, IDisposable).Dispose()
					End Try
					graph.Remove(startOfFirstCycle)
					order.Add(New ConstantValueUtils.FieldInfo(startOfFirstCycle, True))
					pooledHashSet = instance1
				Else
					Dim pooledHashSet1 As PooledHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol) = PooledHashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).GetInstance()
					Dim enumerator4 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator = instance.GetEnumerator()
					While enumerator4.MoveNext()
						Dim sourceFieldSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = enumerator4.Current
						Dim item2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo = graph(sourceFieldSymbol1)
						Try
							enumerator1 = item2.DependedOnBy.GetEnumerator()
							While enumerator1.MoveNext()
								Dim current2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol = enumerator1.Current
								Dim dependencyInfo2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol.DependencyInfo = graph(current2)
								dependencyInfo2.Dependencies = dependencyInfo2.Dependencies.Remove(sourceFieldSymbol1)
								graph(current2) = dependencyInfo2
								pooledHashSet1.Add(current2)
							End While
						Finally
							DirectCast(enumerator1, IDisposable).Dispose()
						End Try
						graph.Remove(sourceFieldSymbol1)
					End While
					Dim enumerator5 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFieldSymbol).Enumerator = instance.GetEnumerator()
					While enumerator5.MoveNext()
						order.Add(New ConstantValueUtils.FieldInfo(enumerator5.Current, False))
					End While
					pooledHashSet = pooledHashSet1
				End If
				instance.Free()
			End While
			If (pooledHashSet IsNot Nothing) Then
				pooledHashSet.Free()
			End If
			If (sourceFieldSymbols IsNot Nothing) Then
				sourceFieldSymbols.Free()
			End If
		End Sub

		Friend Sub SetCustomAttributeData(ByVal attributeData As CustomAttributesBag(Of VisualBasicAttributeData))
			Me._lazyCustomAttributesBag = attributeData
		End Sub

		Protected Overridable Sub SetLazyConstantTuple(ByVal constantTuple As EvaluatedConstant, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Throw ExceptionUtilities.Unreachable
		End Sub

		Private Sub VerifyConstantValueMatches(ByVal attrValue As Microsoft.CodeAnalysis.ConstantValue, ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation))
			Dim constValue As Microsoft.CodeAnalysis.ConstantValue
			Dim orCreateData As CommonFieldWellKnownAttributeData = arguments.GetOrCreateData(Of CommonFieldWellKnownAttributeData)()
			Dim diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (Not Me.IsConst) Then
				constValue = orCreateData.ConstValue
				If (constValue = Microsoft.CodeAnalysis.ConstantValue.Unset) Then
					orCreateData.ConstValue = attrValue
				ElseIf (constValue <> attrValue) Then
					diagnostics.Add(ERRID.ERR_FieldHasMultipleDistinctConstantValues, arguments.AttributeSyntaxOpt.GetLocation())
					Return
				End If
			Else
				If (Me.Type.IsDecimalType() OrElse Me.Type.IsDateTimeType()) Then
					constValue = Me.GetConstantValue(ConstantFieldsInProgress.Empty)
					If (constValue IsNot Nothing AndAlso Not constValue.IsBad AndAlso constValue <> attrValue) Then
						diagnostics.Add(ERRID.ERR_FieldHasMultipleDistinctConstantValues, arguments.AttributeSyntaxOpt.GetLocation())
					End If
				Else
					diagnostics.Add(ERRID.ERR_FieldHasMultipleDistinctConstantValues, arguments.AttributeSyntaxOpt.GetLocation())
				End If
				If (orCreateData.ConstValue = Microsoft.CodeAnalysis.ConstantValue.Unset) Then
					orCreateData.ConstValue = attrValue
					Return
				End If
			End If
		End Sub

		Private Structure DependencyInfo
			Public Dependencies As ImmutableHashSet(Of SourceFieldSymbol)

			Public DependedOnBy As ImmutableHashSet(Of SourceFieldSymbol)
		End Structure
	End Class
End Namespace