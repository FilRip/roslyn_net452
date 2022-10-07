Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Linq
Imports System.Reflection.Metadata
Imports System.Reflection.PortableExecutable
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
	Friend NotInheritable Class RetargetingModuleSymbol
		Inherits NonMissingModuleSymbol
		Private ReadOnly _retargetingAssembly As RetargetingAssemblySymbol

		Private ReadOnly _underlyingModule As SourceModuleSymbol

		Private ReadOnly _retargetingAssemblyMap As Dictionary(Of AssemblySymbol, RetargetingModuleSymbol.DestinationData)

		Friend ReadOnly RetargetingTranslator As RetargetingModuleSymbol.RetargetingSymbolTranslator

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private ReadOnly _symbolMap As ConcurrentDictionary(Of Symbol, Symbol)

		Private ReadOnly _createRetargetingMethod As Func(Of Symbol, RetargetingMethodSymbol)

		Private ReadOnly _createRetargetingNamespace As Func(Of Symbol, RetargetingNamespaceSymbol)

		Private ReadOnly _createRetargetingTypeParameter As Func(Of Symbol, RetargetingTypeParameterSymbol)

		Private ReadOnly _createRetargetingNamedType As Func(Of Symbol, RetargetingNamedTypeSymbol)

		Private ReadOnly _createRetargetingField As Func(Of Symbol, RetargetingFieldSymbol)

		Private ReadOnly _createRetargetingProperty As Func(Of Symbol, RetargetingPropertySymbol)

		Private ReadOnly _createRetargetingEvent As Func(Of Symbol, RetargetingEventSymbol)

		Friend Overrides ReadOnly Property Bit32Required As Boolean
			Get
				Return Me._underlyingModule.Bit32Required
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me._retargetingAssembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._retargetingAssembly
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property DefaultMarshallingCharSet As Nullable(Of CharSet)
			Get
				Return Me._underlyingModule.DefaultMarshallingCharSet
			End Get
		End Property

		Public Overrides ReadOnly Property GlobalNamespace As NamespaceSymbol
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingModule.GlobalNamespace)
			End Get
		End Property

		Friend Overrides ReadOnly Property HasAssemblyCompilationRelaxationsAttribute As Boolean
			Get
				Return Me._underlyingModule.HasAssemblyCompilationRelaxationsAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property HasAssemblyRuntimeCompatibilityAttribute As Boolean
			Get
				Return Me._underlyingModule.HasAssemblyRuntimeCompatibilityAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._underlyingModule.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property Machine As System.Reflection.PortableExecutable.Machine
			Get
				Return Me._underlyingModule.Machine
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._underlyingModule.MetadataName
			End Get
		End Property

		Friend Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				Return Me._underlyingModule.MightContainExtensionMethods
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._underlyingModule.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property NamespaceNames As ICollection(Of String)
			Get
				Return Me._underlyingModule.NamespaceNames
			End Get
		End Property

		Friend Overrides ReadOnly Property Ordinal As Integer
			Get
				Return 0
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeNames As ICollection(Of String)
			Get
				Return Me._underlyingModule.TypeNames
			End Get
		End Property

		Public ReadOnly Property UnderlyingModule As SourceModuleSymbol
			Get
				Return Me._underlyingModule
			End Get
		End Property

		Public Sub New(ByVal retargetingAssembly As RetargetingAssemblySymbol, ByVal underlyingModule As SourceModuleSymbol)
			MyBase.New()
			Me._retargetingAssemblyMap = New Dictionary(Of AssemblySymbol, RetargetingModuleSymbol.DestinationData)()
			Me._symbolMap = New ConcurrentDictionary(Of Symbol, Symbol)()
			Me._retargetingAssembly = retargetingAssembly
			Me._underlyingModule = underlyingModule
			Me.RetargetingTranslator = New RetargetingModuleSymbol.RetargetingSymbolTranslator(Me)
			Me._createRetargetingMethod = New Func(Of Symbol, RetargetingMethodSymbol)(AddressOf Me.CreateRetargetingMethod)
			Me._createRetargetingNamespace = New Func(Of Symbol, RetargetingNamespaceSymbol)(AddressOf Me.CreateRetargetingNamespace)
			Me._createRetargetingNamedType = New Func(Of Symbol, RetargetingNamedTypeSymbol)(AddressOf Me.CreateRetargetingNamedType)
			Me._createRetargetingField = New Func(Of Symbol, RetargetingFieldSymbol)(AddressOf Me.CreateRetargetingField)
			Me._createRetargetingTypeParameter = New Func(Of Symbol, RetargetingTypeParameterSymbol)(AddressOf Me.CreateRetargetingTypeParameter)
			Me._createRetargetingProperty = New Func(Of Symbol, RetargetingPropertySymbol)(AddressOf Me.CreateRetargetingProperty)
			Me._createRetargetingEvent = New Func(Of Symbol, RetargetingEventSymbol)(AddressOf Me.CreateRetargetingEvent)
		End Sub

		Private Function CreateRetargetingEvent(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As RetargetingEventSymbol
			Return New RetargetingEventSymbol(Me, DirectCast(symbol, EventSymbol))
		End Function

		Private Function CreateRetargetingField(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As RetargetingFieldSymbol
			Return New RetargetingFieldSymbol(Me, DirectCast(symbol, FieldSymbol))
		End Function

		Private Function CreateRetargetingMethod(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As RetargetingMethodSymbol
			Return New RetargetingMethodSymbol(Me, DirectCast(symbol, MethodSymbol))
		End Function

		Private Function CreateRetargetingNamedType(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As RetargetingNamedTypeSymbol
			Return New RetargetingNamedTypeSymbol(Me, DirectCast(symbol, NamedTypeSymbol))
		End Function

		Private Function CreateRetargetingNamespace(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As RetargetingNamespaceSymbol
			Return New RetargetingNamespaceSymbol(Me, DirectCast(symbol, NamespaceSymbol))
		End Function

		Private Function CreateRetargetingProperty(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As RetargetingPropertySymbol
			Return New RetargetingPropertySymbol(Me, DirectCast(symbol, PropertySymbol))
		End Function

		Private Function CreateRetargetingTypeParameter(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As RetargetingTypeParameterSymbol
			Dim typeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol)
			Dim containingSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = typeParameterSymbol.ContainingSymbol
			If (containingSymbol.Kind = SymbolKind.Method) Then
				Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = containingSymbol.ContainingType
			Else
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			End If
			Return New RetargetingTypeParameterSymbol(Me, typeParameterSymbol)
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.GetRetargetedAttributes(Me._underlyingModule, Me._lazyCustomAttributes, False)
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingModule.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Public Overrides Function GetMetadata() As ModuleMetadata
			Return Me._underlyingModule.GetMetadata()
		End Function

		Friend Function RetargetingDefinitions(ByVal from As AssemblySymbol, <Out> ByRef [to] As AssemblySymbol) As Boolean
			Dim flag As Boolean
			Dim destinationDatum As RetargetingModuleSymbol.DestinationData = New RetargetingModuleSymbol.DestinationData()
			If (Me._retargetingAssemblyMap.TryGetValue(from, destinationDatum)) Then
				[to] = destinationDatum.[To]
				flag = True
			Else
				[to] = Nothing
				flag = False
			End If
			Return flag
		End Function

		Friend Overrides Sub SetReferences(ByVal moduleReferences As ModuleReferences(Of AssemblySymbol), Optional ByVal originatingSourceAssemblyDebugOnly As SourceAssemblySymbol = Nothing)
			MyBase.SetReferences(moduleReferences, originatingSourceAssemblyDebugOnly)
			Me._retargetingAssemblyMap.Clear()
			Dim referencedAssemblySymbols As ImmutableArray(Of AssemblySymbol) = Me._underlyingModule.GetReferencedAssemblySymbols()
			Dim symbols As ImmutableArray(Of AssemblySymbol) = moduleReferences.Symbols
			Dim identities As ImmutableArray(Of AssemblyIdentity) = moduleReferences.Identities
			Dim num As Integer = -1
			Dim num1 As Integer = -1
			While True
				num = num + 1
				num1 = num1 + 1
				If (num >= symbols.Length) Then
					Exit While
				End If
				While referencedAssemblySymbols(num1).IsLinked
					num1 = num1 + 1
				End While
				If (CObj(symbols(num)) <> CObj(referencedAssemblySymbols(num1))) Then
					Dim destinationDatum As RetargetingModuleSymbol.DestinationData = New RetargetingModuleSymbol.DestinationData()
					If (Not Me._retargetingAssemblyMap.TryGetValue(referencedAssemblySymbols(num1), destinationDatum)) Then
						Dim namedTypeSymbols As ConcurrentDictionary(Of NamedTypeSymbol, NamedTypeSymbol) = New ConcurrentDictionary(Of NamedTypeSymbol, NamedTypeSymbol)()
						Dim assemblySymbols As Dictionary(Of AssemblySymbol, RetargetingModuleSymbol.DestinationData) = Me._retargetingAssemblyMap
						Dim item As AssemblySymbol = referencedAssemblySymbols(num1)
						Dim destinationDatum1 As RetargetingModuleSymbol.DestinationData = New RetargetingModuleSymbol.DestinationData() With
						{
							.[To] = symbols(num),
							.SymbolMap = namedTypeSymbols
						}
						assemblySymbols.Add(item, destinationDatum1)
					End If
				End If
			End While
		End Sub

		Private Structure DestinationData
			Public [To] As AssemblySymbol

			Public SymbolMap As ConcurrentDictionary(Of NamedTypeSymbol, NamedTypeSymbol)
		End Structure

		Friend Class RetargetingSymbolTranslator
			Inherits VisualBasicSymbolVisitor(Of RetargetOptions, Symbol)
			Private ReadOnly _retargetingModule As RetargetingModuleSymbol

			Private ReadOnly Property RetargetingAssembly As RetargetingAssemblySymbol
				Get
					Return Me._retargetingModule._retargetingAssembly
				End Get
			End Property

			Private ReadOnly Property RetargetingAssemblyMap As Dictionary(Of AssemblySymbol, RetargetingModuleSymbol.DestinationData)
				Get
					Return Me._retargetingModule._retargetingAssemblyMap
				End Get
			End Property

			Private ReadOnly Property SymbolMap As ConcurrentDictionary(Of Symbol, Symbol)
				Get
					Return Me._retargetingModule._symbolMap
				End Get
			End Property

			Private ReadOnly Property UnderlyingModule As SourceModuleSymbol
				Get
					Return Me._retargetingModule._underlyingModule
				End Get
			End Property

			Public Sub New(ByVal retargetingModule As RetargetingModuleSymbol)
				MyBase.New()
				Me._retargetingModule = retargetingModule
			End Sub

			Private Function FindEventInRetargetedType(ByVal [event] As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol, ByVal retargetedType As NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol
				Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.Retarget([event].Type, RetargetOptions.RetargetPrimitiveTypesByName)
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = retargetedType.GetMembers([event].Name).GetEnumerator()
				While True
					If (enumerator.MoveNext()) Then
						Dim current As Symbol = enumerator.Current
						If (current.Kind = SymbolKind.[Event]) Then
							Dim eventSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol)
							If (Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(eventSymbol1.Type, typeSymbol, TypeCompareKind.ConsiderEverything)) Then
								eventSymbol = eventSymbol1
								Exit While
							End If
						End If
					Else
						eventSymbol = Nothing
						Exit While
					End If
				End While
				Return eventSymbol
			End Function

			Private Function FindMethodInRetargetedType(ByVal method As MethodSymbol, ByVal retargetedType As NamedTypeSymbol, ByVal retargetedMethodComparer As IEqualityComparer(Of MethodSymbol)) As MethodSymbol
				Return RetargetingModuleSymbol.RetargetingSymbolTranslator.RetargetedTypeMethodFinder.Find(Me, method, retargetedType, retargetedMethodComparer)
			End Function

			Private Function FindPropertyInRetargetedType(ByVal [property] As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol, ByVal retargetedType As NamedTypeSymbol, ByVal retargetedPropertyComparer As IEqualityComparer(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol
				Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol
				Dim flag As Boolean = False
				Dim explicitDefaultConstantValue As ConstantValue
				Dim instance As ArrayBuilder(Of ParameterSymbol) = ArrayBuilder(Of ParameterSymbol).GetInstance()
				Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = [property].Parameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ParameterSymbol = enumerator.Current
					Dim parameterSymbols As ArrayBuilder(Of ParameterSymbol) = instance
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.Retarget(current.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode)
					Dim customModifiers As ImmutableArray(Of CustomModifier) = Me.RetargetModifiers(current.CustomModifiers, flag)
					Dim customModifiers1 As ImmutableArray(Of CustomModifier) = Me.RetargetModifiers(current.RefCustomModifiers, flag)
					If (current.HasExplicitDefaultValue) Then
						explicitDefaultConstantValue = current.ExplicitDefaultConstantValue
					Else
						explicitDefaultConstantValue = Nothing
					End If
					parameterSymbols.Add(New SignatureOnlyParameterSymbol(typeSymbol, customModifiers, customModifiers1, explicitDefaultConstantValue, current.IsParamArray, current.IsByRef, current.IsOut, current.IsOptional))
				End While
				Dim signatureOnlyPropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SignatureOnlyPropertySymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SignatureOnlyPropertySymbol([property].Name, retargetedType, [property].IsReadOnly, [property].IsWriteOnly, instance.ToImmutableAndFree(), [property].ReturnsByRef, Me.Retarget([property].Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode), Me.RetargetModifiers([property].TypeCustomModifiers, flag), Me.RetargetModifiers([property].RefCustomModifiers, flag), False, False)
				Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = retargetedType.GetMembers([property].Name).GetEnumerator()
				While True
					If (enumerator1.MoveNext()) Then
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator1.Current
						If (symbol.Kind = SymbolKind.[Property]) Then
							Dim propertySymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
							If (retargetedPropertyComparer.Equals(propertySymbol1, signatureOnlyPropertySymbol)) Then
								propertySymbol = propertySymbol1
								Exit While
							End If
						End If
					Else
						propertySymbol = Nothing
						Exit While
					End If
				End While
				Return propertySymbol
			End Function

			Friend Function GetRetargetedAttributes(ByVal underlyingSymbol As Symbol, ByRef lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData), Optional ByVal getReturnTypeAttributes As Boolean = False) As ImmutableArray(Of VisualBasicAttributeData)
				Dim returnTypeAttributes As ImmutableArray(Of VisualBasicAttributeData)
				If (lazyCustomAttributes.IsDefault) Then
					If (getReturnTypeAttributes) Then
						returnTypeAttributes = DirectCast(underlyingSymbol, MethodSymbol).GetReturnTypeAttributes()
					Else
						returnTypeAttributes = underlyingSymbol.GetAttributes()
						If (underlyingSymbol.Kind = SymbolKind.Method) Then
							DirectCast(underlyingSymbol, MethodSymbol).GetReturnTypeAttributes()
						End If
					End If
					Dim visualBasicAttributeDatas As ImmutableArray(Of VisualBasicAttributeData) = Me.RetargetAttributes(returnTypeAttributes)
					ImmutableInterlocked.InterlockedCompareExchange(Of VisualBasicAttributeData)(lazyCustomAttributes, visualBasicAttributeDatas, New ImmutableArray(Of VisualBasicAttributeData)())
				End If
				Return lazyCustomAttributes
			End Function

			Private Function IsNoPiaIllegalGenericInstantiation(ByVal oldArguments As ArrayBuilder(Of TypeWithModifiers), ByVal newArguments As ArrayBuilder(Of TypeWithModifiers), ByVal startOfNonInterfaceArguments As Integer) As Boolean
				Dim flag As Boolean
				If (Me.UnderlyingModule.ContainsExplicitDefinitionOfNoPiaLocalTypes) Then
					Dim count As Integer = oldArguments.Count - 1
					Dim num As Integer = startOfNonInterfaceArguments
					While num <= count
						If (Not Me.IsOrClosedOverAnExplicitLocalType(oldArguments(num).Type)) Then
							num = num + 1
						Else
							flag = True
							Return flag
						End If
					End While
				End If
				Dim assembliesToEmbedTypesFrom As ImmutableArray(Of AssemblySymbol) = Me.UnderlyingModule.GetAssembliesToEmbedTypesFrom()
				If (assembliesToEmbedTypesFrom.Length > 0) Then
					Dim count1 As Integer = oldArguments.Count - 1
					Dim num1 As Integer = startOfNonInterfaceArguments
					While num1 <= count1
						If (Not MetadataDecoder.IsOrClosedOverATypeFromAssemblies(oldArguments(num1).Type, assembliesToEmbedTypesFrom)) Then
							num1 = num1 + 1
						Else
							flag = True
							Return flag
						End If
					End While
				End If
				Dim linkedReferencedAssemblies As ImmutableArray(Of AssemblySymbol) = Me.RetargetingAssembly.GetLinkedReferencedAssemblies()
				If (Not linkedReferencedAssemblies.IsDefaultOrEmpty) Then
					Dim count2 As Integer = newArguments.Count - 1
					Dim num2 As Integer = startOfNonInterfaceArguments
					While num2 <= count2
						If (Not MetadataDecoder.IsOrClosedOverATypeFromAssemblies(newArguments(num2).Type, linkedReferencedAssemblies)) Then
							num2 = num2 + 1
						Else
							flag = True
							Return flag
						End If
					End While
				End If
				flag = False
				Return flag
			End Function

			Private Function IsOrClosedOverAnExplicitLocalType(ByVal symbol As TypeSymbol) As Boolean
				Dim flag As Boolean
				Dim kind As SymbolKind = symbol.Kind
				If (kind > SymbolKind.ErrorType) Then
					If (kind = SymbolKind.NamedType) Then
						GoTo Label0
					End If
					If (kind <> SymbolKind.TypeParameter) Then
						Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
					End If
					flag = False
					Return flag
				Else
					If (kind <> SymbolKind.ArrayType) Then
						GoTo Label3
					End If
					flag = Me.IsOrClosedOverAnExplicitLocalType(DirectCast(symbol, ArrayTypeSymbol).ElementType)
					Return flag
				End If
				Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
			Label0:
				Dim tupleUnderlyingType As NamedTypeSymbol = DirectCast(symbol, NamedTypeSymbol)
				If (tupleUnderlyingType.IsTupleType) Then
					tupleUnderlyingType = tupleUnderlyingType.TupleUnderlyingType
				End If
				If (symbol.OriginalDefinition.ContainingModule <> Me._retargetingModule.UnderlyingModule OrElse Not tupleUnderlyingType.IsExplicitDefinitionOfNoPiaLocalType) Then
					Do
						Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = tupleUnderlyingType.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator()
						While enumerator.MoveNext()
							If (Not Me.IsOrClosedOverAnExplicitLocalType(enumerator.Current)) Then
								Continue While
							End If
							flag = True
							Return flag
						End While
						tupleUnderlyingType = tupleUnderlyingType.ContainingType
					Loop While tupleUnderlyingType IsNot Nothing
					flag = False
				Else
					flag = True
				End If
				Return flag
			Label3:
				If (kind = SymbolKind.ErrorType) Then
					GoTo Label0
				End If
				Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
			End Function

			Private Shared Function PerformTypeRetargeting(ByRef destination As RetargetingModuleSymbol.DestinationData, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim metadataTypeName As Microsoft.CodeAnalysis.MetadataTypeName
				Dim orAdd As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
				If (Not destination.SymbolMap.TryGetValue(type, orAdd)) Then
					Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = type.ContainingType
					If (containingType Is Nothing) Then
						metadataTypeName = Microsoft.CodeAnalysis.MetadataTypeName.FromNamespaceAndTypeName(If(type.GetEmittedNamespaceName(), type.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat)), type.MetadataName, False, type.Arity)
						namedTypeSymbol = destination.[To].LookupTopLevelMetadataType(metadataTypeName, True)
					Else
						Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = RetargetingModuleSymbol.RetargetingSymbolTranslator.PerformTypeRetargeting(destination, containingType)
						metadataTypeName = Microsoft.CodeAnalysis.MetadataTypeName.FromTypeName(type.MetadataName, False, type.Arity)
						namedTypeSymbol = namedTypeSymbol1.LookupMetadataType(metadataTypeName)
					End If
					orAdd = destination.SymbolMap.GetOrAdd(type, namedTypeSymbol)
				End If
				Return orAdd
			End Function

			Public Function Retarget(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return symbol.Accept(Of RetargetOptions, Microsoft.CodeAnalysis.VisualBasic.Symbol)(Me, RetargetOptions.RetargetPrimitiveTypesByName)
			End Function

			Public Function Retarget(ByVal marshallingInfo As MarshalPseudoCustomAttributeData) As MarshalPseudoCustomAttributeData
				Dim marshalPseudoCustomAttributeDatum As MarshalPseudoCustomAttributeData
				Dim func As Func(Of TypeSymbol, RetargetingModuleSymbol.RetargetingSymbolTranslator, TypeSymbol)
				If (marshallingInfo IsNot Nothing) Then
					Dim marshalPseudoCustomAttributeDatum1 As MarshalPseudoCustomAttributeData = marshallingInfo
					If (RetargetingModuleSymbol.RetargetingSymbolTranslator._Closure$__.$I11-0 Is Nothing) Then
						func = Function(type As TypeSymbol, translator As RetargetingModuleSymbol.RetargetingSymbolTranslator) translator.Retarget(type, RetargetOptions.RetargetPrimitiveTypesByTypeCode)
						RetargetingModuleSymbol.RetargetingSymbolTranslator._Closure$__.$I11-0 = func
					Else
						func = RetargetingModuleSymbol.RetargetingSymbolTranslator._Closure$__.$I11-0
					End If
					marshalPseudoCustomAttributeDatum = marshalPseudoCustomAttributeDatum1.WithTranslatedTypes(Of TypeSymbol, RetargetingModuleSymbol.RetargetingSymbolTranslator)(func, Me)
				Else
					marshalPseudoCustomAttributeDatum = Nothing
				End If
				Return marshalPseudoCustomAttributeDatum
			End Function

			Public Function Retarget(ByVal symbol As TypeSymbol, ByVal options As RetargetOptions) As TypeSymbol
				Return DirectCast(symbol.Accept(Of RetargetOptions, Microsoft.CodeAnalysis.VisualBasic.Symbol)(Me, options), TypeSymbol)
			End Function

			Public Function Retarget(ByVal ns As NamespaceSymbol) As NamespaceSymbol
				Return DirectCast(Me.SymbolMap.GetOrAdd(ns, Me._retargetingModule._createRetargetingNamespace), NamespaceSymbol)
			End Function

			Public Function Retarget(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal options As RetargetOptions) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim noPiaIllegalGenericInstantiationSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = type.OriginalDefinition
				Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.RetargetNamedTypeDefinition(originalDefinition, options)
				If (CObj(type) = CObj(originalDefinition)) Then
					namedTypeSymbol = namedTypeSymbol1
				ElseIf (namedTypeSymbol1.Kind = SymbolKind.ErrorType AndAlso Not namedTypeSymbol1.IsGenericType) Then
					namedTypeSymbol = namedTypeSymbol1
				ElseIf (Not type.IsUnboundGenericType) Then
					Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = type
					Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers).GetInstance()
					Dim count As Integer = 2147483647
					While containingType IsNot Nothing
						If (count = 2147483647 AndAlso Not containingType.IsInterface) Then
							count = instance.Count
						End If
						Dim arity As Integer = containingType.Arity
						If (arity > 0) Then
							Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = containingType.TypeArgumentsNoUseSiteDiagnostics
							If (Not containingType.HasTypeArgumentsCustomModifiers) Then
								Dim num As Integer = arity - 1
								For i As Integer = 0 To num
									instance.Add(New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers(typeArgumentsNoUseSiteDiagnostics(i)))
								Next

							Else
								Dim num1 As Integer = arity - 1
								For j As Integer = 0 To num1
									instance.Add(New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers(typeArgumentsNoUseSiteDiagnostics(j), containingType.GetTypeArgumentCustomModifiers(j)))
								Next

							End If
						End If
						containingType = containingType.ContainingType
					End While
					Dim flag As Boolean = Not originalDefinition.Equals(namedTypeSymbol1)
					Dim typeWithModifiers As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers).GetInstance(instance.Count)
					Dim enumerator As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers).Enumerator = instance.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers = enumerator.Current
						Dim flag1 As Boolean = False
						Dim typeWithModifier As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers = New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers(DirectCast(current.Type.Accept(Of RetargetOptions, Symbol)(Me, RetargetOptions.RetargetPrimitiveTypesByTypeCode), TypeSymbol), Me.RetargetModifiers(current.CustomModifiers, flag1))
						If (Not flag AndAlso (flag1 OrElse Not TypeSymbol.Equals(typeWithModifier.Type, current.Type, TypeCompareKind.ConsiderEverything))) Then
							flag = True
						End If
						typeWithModifiers.Add(typeWithModifier)
					End While
					Dim flag2 As Boolean = Me.IsNoPiaIllegalGenericInstantiation(instance, typeWithModifiers, count)
					instance.Free()
					If (flag) Then
						containingType = namedTypeSymbol1
						Dim typeParameterSymbols As ArrayBuilder(Of TypeParameterSymbol) = ArrayBuilder(Of TypeParameterSymbol).GetInstance(typeWithModifiers.Count)
						While containingType IsNot Nothing
							If (containingType.Arity > 0) Then
								typeParameterSymbols.AddRange(containingType.TypeParameters)
							End If
							containingType = containingType.ContainingType
						End While
						typeParameterSymbols.ReverseContents()
						typeWithModifiers.ReverseContents()
						Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(namedTypeSymbol1, typeParameterSymbols.ToImmutableAndFree(), typeWithModifiers.ToImmutable(), False)
						noPiaIllegalGenericInstantiationSymbol = namedTypeSymbol1.Construct(typeSubstitution)
					Else
						noPiaIllegalGenericInstantiationSymbol = type
					End If
					typeWithModifiers.Free()
					If (flag2) Then
						noPiaIllegalGenericInstantiationSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.NoPiaIllegalGenericInstantiationSymbol(noPiaIllegalGenericInstantiationSymbol)
					End If
					namedTypeSymbol = noPiaIllegalGenericInstantiationSymbol
				Else
					namedTypeSymbol = If(CObj(namedTypeSymbol1) <> CObj(originalDefinition), namedTypeSymbol1.AsUnboundGenericType(), type)
				End If
				Return namedTypeSymbol
			End Function

			Public Overridable Function Retarget(ByVal typeParameter As TypeParameterSymbol) As TypeParameterSymbol
				Return DirectCast(Me.SymbolMap.GetOrAdd(typeParameter, Me._retargetingModule._createRetargetingTypeParameter), TypeParameterSymbol)
			End Function

			Public Function Retarget(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol
				Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol
				Dim elementType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type.ElementType
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.Retarget(elementType, RetargetOptions.RetargetPrimitiveTypesByTypeCode)
				Dim flag As Boolean = False
				Dim customModifiers As ImmutableArray(Of CustomModifier) = Me.RetargetModifiers(type.CustomModifiers, flag)
				If (flag OrElse Not elementType.Equals(typeSymbol)) Then
					arrayTypeSymbol = If(Not type.IsSZArray, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol.CreateMDArray(typeSymbol, customModifiers, type.Rank, type.Sizes, type.LowerBounds, Me.RetargetingAssembly), Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol.CreateSZArray(typeSymbol, customModifiers, Me.RetargetingAssembly))
				Else
					arrayTypeSymbol = type
				End If
				Return arrayTypeSymbol
			End Function

			Public Function Retarget(ByVal type As ErrorTypeSymbol) As ErrorTypeSymbol
				Dim extendedErrorTypeSymbol As ErrorTypeSymbol
				Dim displayName As Object
				If (type.GetUseSiteInfo().DiagnosticInfo Is Nothing) Then
					Dim errorInfo As DiagnosticInfo = type.ErrorInfo
					If (errorInfo Is Nothing) Then
						Dim objArray(0) As [Object]
						Dim containingAssembly As AssemblySymbol = type.ContainingAssembly
						If (containingAssembly IsNot Nothing) Then
							displayName = containingAssembly.Identity.GetDisplayName(False)
						Else
							displayName = Nothing
						End If
						If (displayName Is Nothing) Then
							displayName = ""
						End If
						objArray(0) = displayName
						errorInfo = ErrorFactory.ErrorInfo(ERRID.ERR_InReferencedAssembly, objArray)
					End If
					extendedErrorTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.ExtendedErrorTypeSymbol(errorInfo, type.Name, type.Arity, type.CandidateSymbols, type.ResultKind, True)
				Else
					extendedErrorTypeSymbol = type
				End If
				Return extendedErrorTypeSymbol
			End Function

			Public Function Retarget(ByVal sequence As IEnumerable(Of NamedTypeSymbol)) As IEnumerable(Of NamedTypeSymbol)
				Return sequence.[Select](Of NamedTypeSymbol)(Function(s As NamedTypeSymbol) Me.Retarget(s, RetargetOptions.RetargetPrimitiveTypesByName))
			End Function

			Public Function Retarget(ByVal arr As ImmutableArray(Of Symbol)) As ImmutableArray(Of Symbol)
				Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance(arr.Length)
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = arr.GetEnumerator()
				While enumerator.MoveNext()
					instance.Add(Me.Retarget(enumerator.Current))
				End While
				Return instance.ToImmutableAndFree()
			End Function

			Public Function Retarget(ByVal sequence As ImmutableArray(Of NamedTypeSymbol)) As ImmutableArray(Of NamedTypeSymbol)
				Dim instance As ArrayBuilder(Of NamedTypeSymbol) = ArrayBuilder(Of NamedTypeSymbol).GetInstance(sequence.Length)
				Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = sequence.GetEnumerator()
				While enumerator.MoveNext()
					instance.Add(Me.Retarget(enumerator.Current, RetargetOptions.RetargetPrimitiveTypesByName))
				End While
				Return instance.ToImmutableAndFree()
			End Function

			Public Function Retarget(ByVal sequence As ImmutableArray(Of TypeSymbol)) As ImmutableArray(Of TypeSymbol)
				Dim instance As ArrayBuilder(Of TypeSymbol) = ArrayBuilder(Of TypeSymbol).GetInstance(sequence.Length)
				Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = sequence.GetEnumerator()
				While enumerator.MoveNext()
					instance.Add(Me.Retarget(enumerator.Current, RetargetOptions.RetargetPrimitiveTypesByName))
				End While
				Return instance.ToImmutableAndFree()
			End Function

			Public Function Retarget(ByVal list As ImmutableArray(Of TypeParameterSymbol)) As ImmutableArray(Of TypeParameterSymbol)
				Dim instance As ArrayBuilder(Of TypeParameterSymbol) = ArrayBuilder(Of TypeParameterSymbol).GetInstance(list.Length)
				Dim enumerator As ImmutableArray(Of TypeParameterSymbol).Enumerator = list.GetEnumerator()
				While enumerator.MoveNext()
					instance.Add(Me.Retarget(enumerator.Current))
				End While
				Return instance.ToImmutableAndFree()
			End Function

			Public Function Retarget(ByVal method As MethodSymbol) As RetargetingMethodSymbol
				Return DirectCast(Me.SymbolMap.GetOrAdd(method, Me._retargetingModule._createRetargetingMethod), RetargetingMethodSymbol)
			End Function

			Public Function Retarget(ByVal method As MethodSymbol, ByVal retargetedMethodComparer As IEqualityComparer(Of MethodSymbol)) As MethodSymbol
				Dim orAdd As MethodSymbol
				If (method.ContainingModule <> Me.UnderlyingModule OrElse Not method.IsDefinition) Then
					Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = method.ContainingType
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.Retarget(containingType, RetargetOptions.RetargetPrimitiveTypesByName)
					orAdd = If(CObj(namedTypeSymbol) = CObj(containingType), method, Me.FindMethodInRetargetedType(method, namedTypeSymbol, retargetedMethodComparer))
				Else
					orAdd = DirectCast(Me.SymbolMap.GetOrAdd(method, Me._retargetingModule._createRetargetingMethod), RetargetingMethodSymbol)
				End If
				Return orAdd
			End Function

			Public Function Retarget(ByVal field As FieldSymbol) As RetargetingFieldSymbol
				Return DirectCast(Me.SymbolMap.GetOrAdd(field, Me._retargetingModule._createRetargetingField), RetargetingFieldSymbol)
			End Function

			Public Function Retarget(ByVal [property] As PropertySymbol) As RetargetingPropertySymbol
				Return DirectCast(Me.SymbolMap.GetOrAdd([property], Me._retargetingModule._createRetargetingProperty), RetargetingPropertySymbol)
			End Function

			Public Function Retarget(ByVal [event] As EventSymbol) As RetargetingEventSymbol
				Return DirectCast(Me.SymbolMap.GetOrAdd([event], Me._retargetingModule._createRetargetingEvent), RetargetingEventSymbol)
			End Function

			Public Function Retarget(ByVal [property] As PropertySymbol, ByVal retargetedPropertyComparer As IEqualityComparer(Of PropertySymbol)) As PropertySymbol
				Dim orAdd As PropertySymbol
				If ([property].ContainingModule <> Me.UnderlyingModule OrElse Not [property].IsDefinition) Then
					Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = [property].ContainingType
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.Retarget(containingType, RetargetOptions.RetargetPrimitiveTypesByName)
					orAdd = If(CObj(namedTypeSymbol) = CObj(containingType), [property], Me.FindPropertyInRetargetedType([property], namedTypeSymbol, retargetedPropertyComparer))
				Else
					orAdd = DirectCast(Me.SymbolMap.GetOrAdd([property], Me._retargetingModule._createRetargetingProperty), RetargetingPropertySymbol)
				End If
				Return orAdd
			End Function

			Private Function RetargetAttributeConstructorArguments(ByVal constructorArguments As ImmutableArray(Of TypedConstant)) As ImmutableArray(Of TypedConstant)
				Dim immutable As ImmutableArray(Of TypedConstant) = constructorArguments
				Dim flag As Boolean = False
				If (Not constructorArguments.IsDefault AndAlso System.Linq.ImmutableArrayExtensions.Any(Of TypedConstant)(constructorArguments)) Then
					Dim instance As ArrayBuilder(Of TypedConstant) = ArrayBuilder(Of TypedConstant).GetInstance(constructorArguments.Length)
					Dim enumerator As ImmutableArray(Of TypedConstant).Enumerator = constructorArguments.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As TypedConstant = enumerator.Current
						instance.Add(Me.RetargetTypedConstant(current, flag))
					End While
					If (flag) Then
						immutable = instance.ToImmutable()
					End If
					instance.Free()
				End If
				Return immutable
			End Function

			Private Function RetargetAttributeData(ByVal oldAttribute As VisualBasicAttributeData) As VisualBasicAttributeData
				Dim containingType As NamedTypeSymbol
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
				Dim attributeConstructor As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = oldAttribute.AttributeConstructor
				If (attributeConstructor Is Nothing) Then
					methodSymbol = Nothing
				Else
					methodSymbol = Me.Retarget(attributeConstructor, MethodSignatureComparer.RetargetedExplicitMethodImplementationComparer)
				End If
				Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = methodSymbol
				Dim attributeClass As NamedTypeSymbol = oldAttribute.AttributeClass
				If (methodSymbol1 IsNot Nothing) Then
					containingType = methodSymbol1.ContainingType
				ElseIf (attributeClass Is Nothing) Then
					containingType = Nothing
				Else
					containingType = Me.Retarget(attributeClass, RetargetOptions.RetargetPrimitiveTypesByTypeCode)
				End If
				Dim typedConstants As ImmutableArray(Of TypedConstant) = Me.RetargetAttributeConstructorArguments(oldAttribute.CommonConstructorArguments)
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of String, TypedConstant)) = Me.RetargetAttributeNamedArguments(oldAttribute.CommonNamedArguments)
				Return New RetargetingAttributeData(oldAttribute.ApplicationSyntaxReference, containingType, methodSymbol1, typedConstants, keyValuePairs, oldAttribute.IsConditionallyOmitted, oldAttribute.HasErrors)
			End Function

			Private Function RetargetAttributeNamedArguments(ByVal namedArguments As ImmutableArray(Of KeyValuePair(Of String, Microsoft.CodeAnalysis.TypedConstant))) As ImmutableArray(Of KeyValuePair(Of String, Microsoft.CodeAnalysis.TypedConstant))
				Dim immutable As ImmutableArray(Of KeyValuePair(Of String, Microsoft.CodeAnalysis.TypedConstant)) = namedArguments
				Dim flag As Boolean = False
				If (System.Linq.ImmutableArrayExtensions.Any(Of KeyValuePair(Of String, Microsoft.CodeAnalysis.TypedConstant))(namedArguments)) Then
					Dim instance As ArrayBuilder(Of KeyValuePair(Of String, Microsoft.CodeAnalysis.TypedConstant)) = ArrayBuilder(Of KeyValuePair(Of String, Microsoft.CodeAnalysis.TypedConstant)).GetInstance(namedArguments.Length)
					Dim enumerator As ImmutableArray(Of KeyValuePair(Of String, Microsoft.CodeAnalysis.TypedConstant)).Enumerator = namedArguments.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of String, Microsoft.CodeAnalysis.TypedConstant) = enumerator.Current
						Dim value As Microsoft.CodeAnalysis.TypedConstant = current.Value
						Dim flag1 As Boolean = False
						Dim typedConstant As Microsoft.CodeAnalysis.TypedConstant = Me.RetargetTypedConstant(value, flag1)
						If (Not flag1) Then
							instance.Add(current)
						Else
							instance.Add(New KeyValuePair(Of String, Microsoft.CodeAnalysis.TypedConstant)(current.Key, typedConstant))
							flag = True
						End If
					End While
					If (flag) Then
						immutable = instance.ToImmutable()
					End If
					instance.Free()
				End If
				Return immutable
			End Function

			Private Function RetargetAttributes(ByVal oldAttributes As ImmutableArray(Of VisualBasicAttributeData)) As ImmutableArray(Of VisualBasicAttributeData)
				Dim func As Func(Of VisualBasicAttributeData, RetargetingModuleSymbol.RetargetingSymbolTranslator, VisualBasicAttributeData)
				Dim visualBasicAttributeDatas As ImmutableArray(Of VisualBasicAttributeData) = oldAttributes
				If (RetargetingModuleSymbol.RetargetingSymbolTranslator._Closure$__.$I27-0 Is Nothing) Then
					func = Function(a As VisualBasicAttributeData, t As RetargetingModuleSymbol.RetargetingSymbolTranslator) t.RetargetAttributeData(a)
					RetargetingModuleSymbol.RetargetingSymbolTranslator._Closure$__.$I27-0 = func
				Else
					func = RetargetingModuleSymbol.RetargetingSymbolTranslator._Closure$__.$I27-0
				End If
				Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of VisualBasicAttributeData, RetargetingModuleSymbol.RetargetingSymbolTranslator, VisualBasicAttributeData)(visualBasicAttributeDatas, func, Me)
			End Function

			Friend Function RetargetAttributes(ByVal attributes As IEnumerable(Of VisualBasicAttributeData)) As IEnumerable(Of VisualBasicAttributeData)
				Return New RetargetingModuleSymbol.RetargetingSymbolTranslator.VB$StateMachine_28_RetargetAttributes(-2) With
				{
					.$VB$Me = Me,
					.$P_attributes = attributes
				}
			End Function

			Public Function RetargetImplementedEvent(ByVal [event] As EventSymbol) As EventSymbol
				Dim orAdd As EventSymbol
				If ([event].ContainingModule <> Me.UnderlyingModule OrElse Not [event].IsDefinition) Then
					Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = [event].ContainingType
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.Retarget(containingType, RetargetOptions.RetargetPrimitiveTypesByName)
					orAdd = If(CObj(namedTypeSymbol) = CObj(containingType), [event], Me.FindEventInRetargetedType([event], namedTypeSymbol))
				Else
					orAdd = DirectCast(Me.SymbolMap.GetOrAdd([event], Me._retargetingModule._createRetargetingEvent), RetargetingEventSymbol)
				End If
				Return orAdd
			End Function

			Friend Function RetargetModifiers(ByVal oldModifiers As ImmutableArray(Of CustomModifier), ByRef modifiersHaveChanged As Boolean) As ImmutableArray(Of CustomModifier)
				Dim instance As ArrayBuilder(Of CustomModifier) = Nothing
				Dim length As Integer = oldModifiers.Length - 1
				Dim num As Integer = 0
				Do
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.Retarget(DirectCast(oldModifiers(num).Modifier, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol), RetargetOptions.RetargetPrimitiveTypesByName)
					If (Not namedTypeSymbol.Equals(oldModifiers(num).Modifier)) Then
						If (instance Is Nothing) Then
							instance = ArrayBuilder(Of CustomModifier).GetInstance(oldModifiers.Length)
							instance.AddRange(oldModifiers, num)
						End If
						instance.Add(If(oldModifiers(num).IsOptional, VisualBasicCustomModifier.CreateOptional(namedTypeSymbol), VisualBasicCustomModifier.CreateRequired(namedTypeSymbol)))
					ElseIf (instance IsNot Nothing) Then
						instance.Add(oldModifiers(num))
					End If
					num = num + 1
				Loop While num <= length
				modifiersHaveChanged = instance IsNot Nothing
				If (Not modifiersHaveChanged) Then
					Return oldModifiers
				End If
				Return instance.ToImmutableAndFree()
			End Function

			Friend Function RetargetModifiers(ByVal oldModifiers As ImmutableArray(Of CustomModifier), ByRef lazyCustomModifiers As ImmutableArray(Of CustomModifier)) As ImmutableArray(Of CustomModifier)
				Dim flag As Boolean = False
				If (lazyCustomModifiers.IsDefault) Then
					Dim customModifiers As ImmutableArray(Of CustomModifier) = Me.RetargetModifiers(oldModifiers, flag)
					ImmutableInterlocked.InterlockedCompareExchange(Of CustomModifier)(lazyCustomModifiers, customModifiers, New ImmutableArray(Of CustomModifier)())
				End If
				Return lazyCustomModifiers
			End Function

			Friend Function RetargetModifiers(ByVal oldTypeModifiers As ImmutableArray(Of CustomModifier), ByVal oldRefModifiers As ImmutableArray(Of CustomModifier), ByRef lazyCustomModifiers As CustomModifiersTuple) As CustomModifiersTuple
				Dim flag As Boolean = False
				If (lazyCustomModifiers Is Nothing) Then
					Dim customModifiers As ImmutableArray(Of CustomModifier) = Me.RetargetModifiers(oldTypeModifiers, flag)
					Dim customModifiers1 As ImmutableArray(Of CustomModifier) = Me.RetargetModifiers(oldRefModifiers, flag)
					Interlocked.CompareExchange(Of CustomModifiersTuple)(lazyCustomModifiers, CustomModifiersTuple.Create(customModifiers, customModifiers1), Nothing)
				End If
				Return lazyCustomModifiers
			End Function

			Private Function RetargetNamedTypeDefinition(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal options As RetargetOptions) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim primitiveType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim flag As Boolean
				If (Not type.IsTupleType) Then
					If (options = RetargetOptions.RetargetPrimitiveTypesByTypeCode) Then
						Dim primitiveTypeCode As Microsoft.Cci.PrimitiveTypeCode = type.PrimitiveTypeCode
						If (primitiveTypeCode = Microsoft.Cci.PrimitiveTypeCode.NotPrimitive) Then
							GoTo Label1
						End If
						primitiveType = Me.RetargetingAssembly.GetPrimitiveType(primitiveTypeCode)
						Return primitiveType
					End If
				Label1:
					If (type.Kind <> SymbolKind.ErrorType) Then
						Dim containingAssembly As AssemblySymbol = type.ContainingAssembly
						flag = If(containingAssembly <> Me.RetargetingAssembly.UnderlyingAssembly, containingAssembly.IsLinked, type.IsExplicitDefinitionOfNoPiaLocalType)
						If (flag) Then
							primitiveType = Me.RetargetNoPiaLocalType(type)
						ElseIf (containingAssembly <> Me.RetargetingAssembly.UnderlyingAssembly) Then
							Dim destinationDatum As RetargetingModuleSymbol.DestinationData = New RetargetingModuleSymbol.DestinationData()
							primitiveType = If(Me.RetargetingAssemblyMap.TryGetValue(containingAssembly, destinationDatum), RetargetingModuleSymbol.RetargetingSymbolTranslator.PerformTypeRetargeting(destinationDatum, type), type)
						Else
							primitiveType = Me.RetargetNamedTypeDefinitionFromUnderlyingAssembly(type)
						End If
					Else
						primitiveType = Me.Retarget(DirectCast(type, ErrorTypeSymbol))
					End If
				Else
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.Retarget(type.TupleUnderlyingType, options)
					If (Not namedTypeSymbol.IsTupleOrCompatibleWithTupleOfCardinality(type.TupleElementTypes.Length)) Then
						primitiveType = namedTypeSymbol
					Else
						primitiveType = DirectCast(type, TupleTypeSymbol).WithUnderlyingType(namedTypeSymbol)
					End If
				End If
				Return primitiveType
			End Function

			Private Shared Function RetargetNamedTypeDefinition(ByVal type As PENamedTypeSymbol, ByVal addedModule As PEModuleSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim metadataTypeName As Microsoft.CodeAnalysis.MetadataTypeName
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
				If (Not addedModule.TypeHandleToTypeMap.TryGetValue(type.Handle, typeSymbol)) Then
					Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = type.ContainingType
					If (containingType Is Nothing) Then
						metadataTypeName = Microsoft.CodeAnalysis.MetadataTypeName.FromNamespaceAndTypeName(If(type.GetEmittedNamespaceName(), type.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat)), type.MetadataName, False, type.Arity)
						namedTypeSymbol1 = addedModule.LookupTopLevelMetadataType(metadataTypeName)
					Else
						Dim namedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = RetargetingModuleSymbol.RetargetingSymbolTranslator.RetargetNamedTypeDefinition(DirectCast(containingType, PENamedTypeSymbol), addedModule)
						metadataTypeName = Microsoft.CodeAnalysis.MetadataTypeName.FromTypeName(type.MetadataName, False, type.Arity)
						namedTypeSymbol1 = namedTypeSymbol2.LookupMetadataType(metadataTypeName)
					End If
					namedTypeSymbol = namedTypeSymbol1
				Else
					namedTypeSymbol = DirectCast(typeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				End If
				Return namedTypeSymbol
			End Function

			Private Function RetargetNamedTypeDefinitionFromUnderlyingAssembly(ByVal type As NamedTypeSymbol) As NamedTypeSymbol
				Dim orAdd As NamedTypeSymbol
				Dim containingModule As ModuleSymbol = type.ContainingModule
				If (containingModule <> Me.UnderlyingModule) Then
					Dim item As PEModuleSymbol = DirectCast(Me.RetargetingAssembly.Modules(containingModule.Ordinal), PEModuleSymbol)
					orAdd = RetargetingModuleSymbol.RetargetingSymbolTranslator.RetargetNamedTypeDefinition(DirectCast(type, PENamedTypeSymbol), item)
				Else
					Dim containingType As NamedTypeSymbol = type.ContainingType
					While containingType IsNot Nothing
						If (Not containingType.IsExplicitDefinitionOfNoPiaLocalType) Then
							containingType = containingType.ContainingType
						Else
							orAdd = DirectCast(Me.SymbolMap.GetOrAdd(type, New UnsupportedMetadataTypeSymbol(DirectCast(Nothing, BadImageFormatException))), NamedTypeSymbol)
							Return orAdd
						End If
					End While
					orAdd = DirectCast(Me.SymbolMap.GetOrAdd(type, Me._retargetingModule._createRetargetingNamedType), NamedTypeSymbol)
				End If
				Return orAdd
			End Function

			Private Function RetargetNoPiaLocalType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim unsupportedMetadataTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
				Dim orAdd As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
				If (Not Me.RetargetingAssembly.m_NoPiaUnificationMap.TryGetValue(type, orAdd)) Then
					If (type.ContainingSymbol.Kind = SymbolKind.NamedType OrElse type.Arity <> 0) Then
						unsupportedMetadataTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(DirectCast(Nothing, BadImageFormatException))
					Else
						Dim isInterface As Boolean = type.IsInterface
						Dim guidString As Boolean = False
						Dim str As String = Nothing
						Dim valueInternal As String = Nothing
						If (isInterface) Then
							guidString = type.GetGuidString(str)
						End If
						Dim metadataTypeName As Microsoft.CodeAnalysis.MetadataTypeName = Microsoft.CodeAnalysis.MetadataTypeName.FromFullName(type.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), False, type.Arity)
						Dim fullName As String = Nothing
						If (type.ContainingModule = Me._retargetingModule.UnderlyingModule) Then
							Dim enumerator As ImmutableArray(Of VisualBasicAttributeData).Enumerator = type.GetAttributes().GetEnumerator()
							While enumerator.MoveNext()
								Dim current As VisualBasicAttributeData = enumerator.Current
								Dim targetAttributeSignatureIndex As Integer = current.GetTargetAttributeSignatureIndex(type, AttributeDescription.TypeIdentifierAttribute)
								If (targetAttributeSignatureIndex = -1) Then
									Continue While
								End If
								If (targetAttributeSignatureIndex <> 1 OrElse current.CommonConstructorArguments.Length <> 2) Then
									GoTo Label0
								End If
								Dim item As TypedConstant = current.CommonConstructorArguments(0)
								valueInternal = TryCast(item.ValueInternal, [String])
								item = current.CommonConstructorArguments(1)
								fullName = TryCast(item.ValueInternal, [String])
								GoTo Label0
							End While
						ElseIf (Not guidString AndAlso Not isInterface) Then
							type.ContainingAssembly.GetGuidString(valueInternal)
							fullName = metadataTypeName.FullName
						End If
					Label0:
						unsupportedMetadataTypeSymbol = MetadataDecoder.SubstituteNoPiaLocalType(metadataTypeName, isInterface, type.BaseTypeNoUseSiteDiagnostics, str, valueInternal, fullName, Me.RetargetingAssembly)
					End If
					orAdd = Me.RetargetingAssembly.m_NoPiaUnificationMap.GetOrAdd(type, unsupportedMetadataTypeSymbol)
					namedTypeSymbol = orAdd
				Else
					namedTypeSymbol = orAdd
				End If
				Return namedTypeSymbol
			End Function

			Private Function RetargetTypedConstant(ByVal oldConstant As Microsoft.CodeAnalysis.TypedConstant, ByRef typedConstantChanged As Boolean) As Microsoft.CodeAnalysis.TypedConstant
				Dim typedConstant As Microsoft.CodeAnalysis.TypedConstant
				Dim objectValue As Object
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
				Dim typeInternal As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = DirectCast(oldConstant.TypeInternal, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
				If (typeInternal Is Nothing) Then
					typeSymbol = Nothing
				Else
					typeSymbol = Me.Retarget(typeInternal, RetargetOptions.RetargetPrimitiveTypesByTypeCode)
				End If
				Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeSymbol
				If (oldConstant.Kind <> TypedConstantKind.Array) Then
					Dim obj As Object = RuntimeHelpers.GetObjectValue(oldConstant.ValueInternal)
					If (oldConstant.Kind <> TypedConstantKind.Type OrElse obj Is Nothing) Then
						objectValue = RuntimeHelpers.GetObjectValue(obj)
					Else
						objectValue = Me.Retarget(DirectCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol), RetargetOptions.RetargetPrimitiveTypesByTypeCode)
					End If
					If (CObj(typeSymbol1) <> CObj(typeInternal) OrElse CObj(objectValue) <> CObj(obj)) Then
						typedConstantChanged = True
						typedConstant = New Microsoft.CodeAnalysis.TypedConstant(typeSymbol1, oldConstant.Kind, RuntimeHelpers.GetObjectValue(objectValue))
					Else
						typedConstant = oldConstant
					End If
				Else
					Dim typedConstants As ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant) = Me.RetargetAttributeConstructorArguments(oldConstant.Values)
					If (CObj(typeSymbol1) <> CObj(typeInternal) OrElse typedConstants <> oldConstant.Values) Then
						typedConstantChanged = True
						typedConstant = New Microsoft.CodeAnalysis.TypedConstant(typeSymbol1, typedConstants)
					Else
						typedConstant = oldConstant
					End If
				End If
				Return typedConstant
			End Function

			Public Overrides Function VisitArrayType(ByVal symbol As ArrayTypeSymbol, ByVal arg As RetargetOptions) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me.Retarget(symbol)
			End Function

			Public Overrides Function VisitErrorType(ByVal symbol As ErrorTypeSymbol, ByVal options As RetargetOptions) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me.Retarget(symbol)
			End Function

			Public Overrides Function VisitEvent(ByVal symbol As EventSymbol, ByVal arg As RetargetOptions) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me.Retarget(symbol)
			End Function

			Public Overrides Function VisitField(ByVal symbol As FieldSymbol, ByVal options As RetargetOptions) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me.Retarget(symbol)
			End Function

			Public Overrides Function VisitMethod(ByVal symbol As MethodSymbol, ByVal options As RetargetOptions) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me.Retarget(symbol)
			End Function

			Public Overrides Function VisitModule(ByVal symbol As ModuleSymbol, ByVal options As RetargetOptions) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me._retargetingModule
			End Function

			Public Overrides Function VisitNamedType(ByVal symbol As NamedTypeSymbol, ByVal options As RetargetOptions) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me.Retarget(symbol, options)
			End Function

			Public Overrides Function VisitNamespace(ByVal symbol As NamespaceSymbol, ByVal options As RetargetOptions) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me.Retarget(symbol)
			End Function

			Public Overrides Function VisitProperty(ByVal symbol As PropertySymbol, ByVal arg As RetargetOptions) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me.Retarget(symbol)
			End Function

			Public Overrides Function VisitTypeParameter(ByVal symbol As TypeParameterSymbol, ByVal options As RetargetOptions) As Microsoft.CodeAnalysis.VisualBasic.Symbol
				Return Me.Retarget(symbol)
			End Function

			Private Class RetargetedTypeMethodFinder
				Inherits RetargetingModuleSymbol.RetargetingSymbolTranslator
				Private Sub New(ByVal retargetingModule As RetargetingModuleSymbol)
					MyBase.New(retargetingModule)
				End Sub

				Public Shared Function Find(ByVal translator As RetargetingModuleSymbol.RetargetingSymbolTranslator, ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal retargetedType As NamedTypeSymbol, ByVal retargetedMethodComparer As IEqualityComparer(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
					If (Not retargetedType.IsErrorType()) Then
						methodSymbol = If(method.IsGenericMethod, RetargetingModuleSymbol.RetargetingSymbolTranslator.RetargetedTypeMethodFinder.FindWorker(New RetargetingModuleSymbol.RetargetingSymbolTranslator.RetargetedTypeMethodFinder(translator._retargetingModule), method, retargetedType, retargetedMethodComparer), RetargetingModuleSymbol.RetargetingSymbolTranslator.RetargetedTypeMethodFinder.FindWorker(translator, method, retargetedType, retargetedMethodComparer))
					Else
						methodSymbol = Nothing
					End If
					Return methodSymbol
				End Function

				Private Shared Function FindWorker(ByVal translator As RetargetingModuleSymbol.RetargetingSymbolTranslator, ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal retargetedType As NamedTypeSymbol, ByVal retargetedMethodComparer As IEqualityComparer(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
					Dim flag As Boolean = False
					Dim instance As ArrayBuilder(Of ParameterSymbol) = ArrayBuilder(Of ParameterSymbol).GetInstance(method.Parameters.Length)
					Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = method.Parameters.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As ParameterSymbol = enumerator.Current
						instance.Add(New SignatureOnlyParameterSymbol(translator.Retarget(current.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode), translator.RetargetModifiers(current.CustomModifiers, flag), translator.RetargetModifiers(current.RefCustomModifiers, flag), current.ExplicitDefaultConstantValue, current.IsParamArray, current.IsByRef, current.IsOut, current.IsOptional))
					End While
					Dim signatureOnlyMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SignatureOnlyMethodSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SignatureOnlyMethodSymbol(method.Name, retargetedType, method.MethodKind, method.CallingConvention, IndexedTypeParameterSymbol.Take(method.Arity), instance.ToImmutableAndFree(), method.ReturnsByRef, translator.Retarget(method.ReturnType, RetargetOptions.RetargetPrimitiveTypesByTypeCode), translator.RetargetModifiers(method.ReturnTypeCustomModifiers, flag), translator.RetargetModifiers(method.RefCustomModifiers, flag), ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).Empty, False)
					Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = retargetedType.GetMembers(method.Name).GetEnumerator()
					While True
						If (enumerator1.MoveNext()) Then
							Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator1.Current
							If (symbol.Kind = SymbolKind.Method) Then
								Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
								If (retargetedMethodComparer.Equals(methodSymbol1, signatureOnlyMethodSymbol)) Then
									methodSymbol = methodSymbol1
									Exit While
								End If
							End If
						Else
							methodSymbol = Nothing
							Exit While
						End If
					End While
					Return methodSymbol
				End Function

				Public Overrides Function Retarget(ByVal typeParameter As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol
					Dim typeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol
					typeParameterSymbol = If(typeParameter.ContainingModule <> MyBase.UnderlyingModule, IndexedTypeParameterSymbol.GetTypeParameter(typeParameter.Ordinal), MyBase.Retarget(typeParameter))
					Return typeParameterSymbol
				End Function
			End Class
		End Class
	End Class
End Namespace