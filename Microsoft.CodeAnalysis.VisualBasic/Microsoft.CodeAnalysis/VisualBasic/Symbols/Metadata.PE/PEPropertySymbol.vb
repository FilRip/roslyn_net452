Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Linq
Imports System.Reflection
Imports System.Reflection.Metadata
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend Class PEPropertySymbol
		Inherits PropertySymbol
		Private ReadOnly _name As String

		Private ReadOnly _flags As PropertyAttributes

		Private ReadOnly _containingType As PENamedTypeSymbol

		Private ReadOnly _signatureHeader As SignatureHeader

		Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

		Private ReadOnly _returnsByRef As Boolean

		Private ReadOnly _propertyType As TypeSymbol

		Private ReadOnly _getMethod As PEMethodSymbol

		Private ReadOnly _setMethod As PEMethodSymbol

		Private ReadOnly _handle As PropertyDefinitionHandle

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private _lazyDocComment As Tuple(Of CultureInfo, String)

		Private _lazyCachedUseSiteInfo As CachedUseSiteInfo(Of AssemblySymbol)

		Private _isWithEvents As Integer

		Private Const s_unsetAccessibility As Integer = -1

		Private _lazyDeclaredAccessibility As Integer

		Private _lazyObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData

		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return DirectCast(Me._signatureHeader.RawValue, Microsoft.Cci.CallingConvention)
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				If (Me._lazyDeclaredAccessibility = -1) Then
					Interlocked.CompareExchange(Me._lazyDeclaredAccessibility, CInt(PEPropertySymbol.GetDeclaredAccessibility(Me)), -1)
				End If
				Return DirectCast(Me._lazyDeclaredAccessibility, Accessibility)
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of PropertySymbol)
			Get
				Dim empty As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
				Dim enumerator As IEnumerator(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol) = Nothing
				Dim enumerator1 As IEnumerator(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol) = Nothing
				If ((Me._getMethod Is Nothing OrElse Me._getMethod.ExplicitInterfaceImplementations.Length = 0) AndAlso (Me._setMethod Is Nothing OrElse Me._setMethod.ExplicitInterfaceImplementations.Length = 0)) Then
					empty = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol).Empty
				Else
					Dim propertiesForExplicitlyImplementedAccessor As ISet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol) = PEPropertyOrEventHelpers.GetPropertiesForExplicitlyImplementedAccessor(Me._getMethod)
					Dim propertySymbols As ISet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol) = PEPropertyOrEventHelpers.GetPropertiesForExplicitlyImplementedAccessor(Me._setMethod)
					Using instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol).GetInstance()
						enumerator = propertiesForExplicitlyImplementedAccessor.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = enumerator.Current
							Dim setMethod As MethodSymbol = current.SetMethod
							If (setMethod IsNot Nothing AndAlso setMethod.RequiresImplementation() AndAlso Not propertySymbols.Contains(current)) Then
								Continue While
							End If
							instance.Add(current)
						End While
					End Using
					Using enumerator1
						enumerator1 = propertySymbols.GetEnumerator()
						While enumerator1.MoveNext()
							Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = enumerator1.Current
							Dim getMethod As MethodSymbol = propertySymbol.GetMethod
							If (getMethod IsNot Nothing AndAlso getMethod.RequiresImplementation()) Then
								Continue While
							End If
							instance.Add(propertySymbol)
						End While
					End Using
					empty = instance.ToImmutableAndFree()
				End If
				Return empty
			End Get
		End Property

		Public Overrides ReadOnly Property GetMethod As MethodSymbol
			Get
				Return Me._getMethod
			End Get
		End Property

		Friend ReadOnly Property Handle As PropertyDefinitionHandle
			Get
				Return Me._handle
			End Get
		End Property

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return (Me._flags And PropertyAttributes.RTSpecialName) <> PropertyAttributes.None
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return (Me._flags And PropertyAttributes.SpecialName) <> PropertyAttributes.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsDefault As Boolean
			Get
				Dim defaultPropertyName As String = Me._containingType.DefaultPropertyName
				If ([String].IsNullOrEmpty(defaultPropertyName)) Then
					Return False
				End If
				Return CaseInsensitiveComparison.Equals(defaultPropertyName, Me._name)
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				If (Me._getMethod IsNot Nothing AndAlso Me._getMethod.IsMustOverride) Then
					Return True
				End If
				If (Me._setMethod Is Nothing) Then
					Return False
				End If
				Return Me._setMethod.IsMustOverride
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMyGroupCollectionProperty As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				If (Me._getMethod IsNot Nothing AndAlso Not Me._getMethod.IsNotOverridable) Then
					Return False
				End If
				If (Me._setMethod Is Nothing) Then
					Return True
				End If
				Return Me._setMethod.IsNotOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				If (Me._getMethod IsNot Nothing AndAlso Me._getMethod.IsOverloads) Then
					Return True
				End If
				If (Me._setMethod Is Nothing) Then
					Return False
				End If
				Return Me._setMethod.IsOverloads
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				If (Me.IsMustOverride OrElse Me.IsOverrides) Then
					Return False
				End If
				If (Me._getMethod IsNot Nothing AndAlso Me._getMethod.IsOverridable) Then
					Return True
				End If
				If (Me._setMethod Is Nothing) Then
					Return False
				End If
				Return Me._setMethod.IsOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				If (Me._getMethod IsNot Nothing AndAlso Me._getMethod.IsOverrides) Then
					Return True
				End If
				If (Me._setMethod Is Nothing) Then
					Return False
				End If
				Return Me._setMethod.IsOverrides
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				If (Me._getMethod IsNot Nothing AndAlso Not Me._getMethod.IsShared) Then
					Return False
				End If
				If (Me._setMethod Is Nothing) Then
					Return True
				End If
				Return Me._setMethod.IsShared
			End Get
		End Property

		Public Overrides ReadOnly Property IsWithEvents As Boolean
			Get
				If (Me._isWithEvents = 0) Then
					Me.SetIsWithEvents(MyBase.IsWithEvents)
				End If
				Return Me._isWithEvents = 2
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._containingType.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				ObsoleteAttributeHelpers.InitializeObsoleteDataFromMetadata(Me._lazyObsoleteAttributeData, Me._handle, DirectCast(Me.ContainingModule, PEModuleSymbol))
				Return Me._lazyObsoleteAttributeData
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._parameters
			End Get
		End Property

		Friend ReadOnly Property PropertyFlags As PropertyAttributes
			Get
				Return Me._flags
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return Me._returnsByRef
			End Get
		End Property

		Public Overrides ReadOnly Property SetMethod As MethodSymbol
			Get
				Return Me._setMethod
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._propertyType
			End Get
		End Property

		Public Overrides ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Private Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal containingType As PENamedTypeSymbol, ByVal handle As PropertyDefinitionHandle, ByVal getMethod As PEMethodSymbol, ByVal setMethod As PEMethodSymbol, ByVal metadataDecoder As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.MetadataDecoder, ByVal signatureHeader As System.Reflection.Metadata.SignatureHeader, ByVal propertyParams As ParamInfo(Of TypeSymbol)())
			MyBase.New()
			Dim paramInfo As ParamInfo(Of TypeSymbol)
			Dim signatureForMethod As ParamInfo(Of TypeSymbol)()
			Dim paramInfoArray As ParamInfo(Of TypeSymbol)()
			Dim func As Func(Of ParamInfo(Of TypeSymbol), Boolean)
			Dim flag As Boolean
			Dim flag1 As Boolean
			Me._lazyCachedUseSiteInfo = CachedUseSiteInfo(Of AssemblySymbol).Uninitialized
			Me._isWithEvents = 0
			Me._lazyDeclaredAccessibility = -1
			Me._lazyObsoleteAttributeData = Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized
			Me._signatureHeader = signatureHeader
			Me._containingType = containingType
			Me._handle = handle
			Dim [module] As PEModule = moduleSymbol.[Module]
			Dim badImageFormatException As System.BadImageFormatException = Nothing
			Try
				[module].GetPropertyDefPropsOrThrow(handle, Me._name, Me._flags)
			Catch badImageFormatException1 As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException1)
				badImageFormatException = badImageFormatException1
				If (Me._name Is Nothing) Then
					Me._name = [String].Empty
				End If
				ProjectData.ClearProjectError()
			End Try
			Me._getMethod = getMethod
			Me._setMethod = setMethod
			Dim signatureHeader1 As System.Reflection.Metadata.SignatureHeader = New System.Reflection.Metadata.SignatureHeader()
			Dim badImageFormatException2 As System.BadImageFormatException = Nothing
			If (Me._getMethod Is Nothing) Then
				signatureForMethod = Nothing
			Else
				signatureForMethod = metadataDecoder.GetSignatureForMethod(Me._getMethod.Handle, signatureHeader1, badImageFormatException2, True)
			End If
			Dim paramInfoArray1 As ParamInfo(Of TypeSymbol)() = signatureForMethod
			Dim badImageFormatException3 As System.BadImageFormatException = Nothing
			If (Me._setMethod Is Nothing) Then
				paramInfoArray = Nothing
			Else
				paramInfoArray = metadataDecoder.GetSignatureForMethod(Me._setMethod.Handle, signatureHeader1, badImageFormatException3, True)
			End If
			Dim paramInfoArray2 As ParamInfo(Of TypeSymbol)() = paramInfoArray
			Dim flag2 As Boolean = PEPropertySymbol.DoSignaturesMatch(metadataDecoder, propertyParams, Me._getMethod, paramInfoArray1, Me._setMethod, paramInfoArray2)
			Dim flag3 As Boolean = True
			Me._parameters = PEPropertySymbol.GetParameters(Me, Me._getMethod, Me._setMethod, propertyParams, flag3)
			If (flag2 AndAlso flag3 AndAlso badImageFormatException2 Is Nothing AndAlso badImageFormatException3 Is Nothing AndAlso badImageFormatException Is Nothing) Then
				Dim paramInfoArray3 As ParamInfo(Of TypeSymbol)() = propertyParams
				If (PEPropertySymbol._Closure$__.$I18-0 Is Nothing) Then
					func = Function(p As ParamInfo(Of TypeSymbol))
						If (p.RefCustomModifiers.AnyRequired()) Then
							Return True
						End If
						Return p.CustomModifiers.AnyRequired()
					End Function
					PEPropertySymbol._Closure$__.$I18-0 = func
				Else
					func = PEPropertySymbol._Closure$__.$I18-0
				End If
				If (Not DirectCast(paramInfoArray3, IEnumerable(Of ParamInfo(Of TypeSymbol))).Any(func)) Then
					If (Me._getMethod IsNot Nothing) Then
						flag = Me._getMethod.SetAssociatedProperty(Me, MethodKind.PropertyGet)
					End If
					If (Me._setMethod IsNot Nothing) Then
						flag1 = Me._setMethod.SetAssociatedProperty(Me, MethodKind.PropertySet)
					End If
					paramInfo = propertyParams(0)
					Me._returnsByRef = paramInfo.IsByRef
					Me._propertyType = paramInfo.Type
					Me._propertyType = TupleTypeDecoder.DecodeTupleTypesIfApplicable(Me._propertyType, handle, moduleSymbol)
					Return
				End If
			End If
			Dim objArray() As [Object] = { CustomSymbolDisplayFormatter.QualifiedName(Me) }
			Me._lazyCachedUseSiteInfo.Initialize(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedProperty1, objArray))
			If (Me._getMethod IsNot Nothing) Then
				flag = Me._getMethod.SetAssociatedProperty(Me, MethodKind.PropertyGet)
			End If
			If (Me._setMethod IsNot Nothing) Then
				flag1 = Me._setMethod.SetAssociatedProperty(Me, MethodKind.PropertySet)
			End If
			paramInfo = propertyParams(0)
			Me._returnsByRef = paramInfo.IsByRef
			Me._propertyType = paramInfo.Type
			Me._propertyType = TupleTypeDecoder.DecodeTupleTypesIfApplicable(Me._propertyType, handle, moduleSymbol)
		End Sub

		Friend Shared Function Create(ByVal moduleSymbol As PEModuleSymbol, ByVal containingType As PENamedTypeSymbol, ByVal handle As PropertyDefinitionHandle, ByVal getMethod As PEMethodSymbol, ByVal setMethod As PEMethodSymbol) As PEPropertySymbol
			Dim signatureHeader As System.Reflection.Metadata.SignatureHeader
			Dim pEPropertySymbolWithCustomModifier As PEPropertySymbol
			Dim metadataDecoder As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.MetadataDecoder = New Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.MetadataDecoder(moduleSymbol, containingType)
			Dim badImageFormatException As System.BadImageFormatException = Nothing
			Dim signatureForProperty As ParamInfo(Of TypeSymbol)() = metadataDecoder.GetSignatureForProperty(handle, signatureHeader, badImageFormatException)
			Dim paramInfo As ParamInfo(Of TypeSymbol) = signatureForProperty(0)
			If (Not paramInfo.CustomModifiers.IsDefaultOrEmpty OrElse Not paramInfo.RefCustomModifiers.IsDefaultOrEmpty) Then
				pEPropertySymbolWithCustomModifier = New PEPropertySymbol.PEPropertySymbolWithCustomModifiers(moduleSymbol, containingType, handle, getMethod, setMethod, metadataDecoder, signatureHeader, signatureForProperty)
			Else
				pEPropertySymbolWithCustomModifier = New PEPropertySymbol(moduleSymbol, containingType, handle, getMethod, setMethod, metadataDecoder, signatureHeader, signatureForProperty)
			End If
			If (badImageFormatException IsNot Nothing) Then
				Dim objArray() As [Object] = { CustomSymbolDisplayFormatter.QualifiedName(pEPropertySymbolWithCustomModifier) }
				pEPropertySymbolWithCustomModifier._lazyCachedUseSiteInfo.Initialize(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedProperty1, objArray))
			End If
			Return pEPropertySymbolWithCustomModifier
		End Function

		Private Shared Function DoSignaturesMatch(ByVal metadataDecoder As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.MetadataDecoder, ByVal propertyParams As ParamInfo(Of TypeSymbol)(), ByVal getMethodOpt As PEMethodSymbol, ByVal getMethodParamsOpt As ParamInfo(Of TypeSymbol)(), ByVal setMethodOpt As PEMethodSymbol, ByVal setMethodParamsOpt As ParamInfo(Of TypeSymbol)()) As Boolean
			Dim flag As Boolean
			If (getMethodOpt Is Nothing) Then
				If (metadataDecoder.DoPropertySignaturesMatch(propertyParams, setMethodParamsOpt, True, False, False)) Then
					If (getMethodOpt Is Nothing OrElse setMethodOpt Is Nothing) Then
						flag = True
					ElseIf (metadataDecoder.DoPropertySignaturesMatch(getMethodParamsOpt, setMethodParamsOpt, True, True, False)) Then
						flag = If(getMethodOpt.IsMustOverride <> setMethodOpt.IsMustOverride OrElse getMethodOpt.IsNotOverridable <> setMethodOpt.IsNotOverridable OrElse getMethodOpt.IsOverrides <> setMethodOpt.IsOverrides OrElse getMethodOpt.IsShared <> setMethodOpt.IsShared, False, True)
					Else
						flag = False
					End If
					Return flag
				End If
				flag = False
				Return flag
			Else
				If (metadataDecoder.DoPropertySignaturesMatch(propertyParams, getMethodParamsOpt, False, False, False)) Then
					If (getMethodOpt Is Nothing OrElse setMethodOpt Is Nothing) Then
						flag = True
					ElseIf (metadataDecoder.DoPropertySignaturesMatch(getMethodParamsOpt, setMethodParamsOpt, True, True, False)) Then
						flag = If(getMethodOpt.IsMustOverride <> setMethodOpt.IsMustOverride OrElse getMethodOpt.IsNotOverridable <> setMethodOpt.IsNotOverridable OrElse getMethodOpt.IsOverrides <> setMethodOpt.IsOverrides OrElse getMethodOpt.IsShared <> setMethodOpt.IsShared, False, True)
					Else
						flag = False
					End If
					Return flag
				End If
				flag = False
				Return flag
			End If
			If (getMethodOpt Is Nothing OrElse setMethodOpt Is Nothing) Then
				flag = True
			ElseIf (metadataDecoder.DoPropertySignaturesMatch(getMethodParamsOpt, setMethodParamsOpt, True, True, False)) Then
				flag = If(getMethodOpt.IsMustOverride <> setMethodOpt.IsMustOverride OrElse getMethodOpt.IsNotOverridable <> setMethodOpt.IsNotOverridable OrElse getMethodOpt.IsOverrides <> setMethodOpt.IsOverrides OrElse getMethodOpt.IsShared <> setMethodOpt.IsShared, False, True)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function GetAccessorParameter(ByVal accessor As PEMethodSymbol, ByVal index As Integer) As PEParameterSymbol
			Dim item As PEParameterSymbol
			If (accessor IsNot Nothing) Then
				Dim parameters As ImmutableArray(Of ParameterSymbol) = accessor.Parameters
				If (index >= parameters.Length) Then
					item = Nothing
					Return item
				End If
				item = DirectCast(parameters(index), PEParameterSymbol)
				Return item
			End If
			item = Nothing
			Return item
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			If (Me._lazyCustomAttributes.IsDefault) Then
				DirectCast(Me.ContainingModule, PEModuleSymbol).LoadCustomAttributes(Me._handle, Me._lazyCustomAttributes)
			End If
			Return Me._lazyCustomAttributes
		End Function

		Friend Overrides Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Return DirectCast(Me.GetAttributes(), IEnumerable(Of VisualBasicAttributeData))
		End Function

		Private Shared Function GetDeclaredAccessibility(ByVal [property] As PropertySymbol) As Microsoft.CodeAnalysis.Accessibility
			Dim declaredAccessibility As Microsoft.CodeAnalysis.Accessibility
			Dim num As Integer
			Dim getMethod As MethodSymbol = [property].GetMethod
			Dim setMethod As MethodSymbol = [property].SetMethod
			If (getMethod Is Nothing) Then
				declaredAccessibility = If(setMethod Is Nothing, Microsoft.CodeAnalysis.Accessibility.NotApplicable, setMethod.DeclaredAccessibility)
			ElseIf (setMethod IsNot Nothing) Then
				Dim accessibility As Microsoft.CodeAnalysis.Accessibility = getMethod.DeclaredAccessibility
				Dim declaredAccessibility1 As Microsoft.CodeAnalysis.Accessibility = setMethod.DeclaredAccessibility
				num = CInt(If(accessibility > declaredAccessibility1, declaredAccessibility1, accessibility))
				Dim accessibility1 As Microsoft.CodeAnalysis.Accessibility = If(accessibility > declaredAccessibility1, accessibility, declaredAccessibility1)
				declaredAccessibility = If(num <> 3 OrElse accessibility1 <> Microsoft.CodeAnalysis.Accessibility.Internal, accessibility1, Microsoft.CodeAnalysis.Accessibility.ProtectedOrInternal)
			Else
				declaredAccessibility = getMethod.DeclaredAccessibility
			End If
			Return declaredAccessibility
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return PEDocumentationCommentUtils.GetDocumentationComment(Me, Me._containingType.ContainingPEModule, preferredCulture, cancellationToken, Me._lazyDocComment)
		End Function

		Private Shared Function GetParameters(ByVal [property] As PEPropertySymbol, ByVal getMethod As PEMethodSymbol, ByVal setMethod As PEMethodSymbol, ByVal propertyParams As ParamInfo(Of TypeSymbol)(), ByRef parametersMatch As Boolean) As ImmutableArray(Of ParameterSymbol)
			Dim empty As ImmutableArray(Of ParameterSymbol)
			parametersMatch = True
			If (CInt(propertyParams.Length) >= 2) Then
				Dim parameterSymbolArray(CInt(propertyParams.Length) - 2 + 1 - 1) As ParameterSymbol
				Dim length As Integer = CInt(propertyParams.Length) - 2
				Dim num As Integer = 0
				Do
					Dim paramInfo As ParamInfo(Of TypeSymbol) = propertyParams(num + 1)
					Dim accessorParameter As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEParameterSymbol = PEPropertySymbol.GetAccessorParameter(getMethod, num)
					Dim pEParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEParameterSymbol = PEPropertySymbol.GetAccessorParameter(setMethod, num)
					If (accessorParameter Is Nothing) Then
						accessorParameter = pEParameterSymbol
					End If
					Dim pEParameterSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEParameterSymbol = accessorParameter
					Dim name As String = Nothing
					Dim isByRef As Boolean = False
					Dim explicitDefaultConstantValue As ConstantValue = Nothing
					Dim paramFlags As ParameterAttributes = ParameterAttributes.None
					Dim handle As ParameterHandle = paramInfo.Handle
					Dim isParamArray As Boolean = False
					Dim hasOptionCompare As Boolean = False
					If (pEParameterSymbol1 IsNot Nothing) Then
						name = pEParameterSymbol1.Name
						isByRef = pEParameterSymbol1.IsByRef
						explicitDefaultConstantValue = pEParameterSymbol1.ExplicitDefaultConstantValue
						paramFlags = pEParameterSymbol1.ParamFlags
						handle = pEParameterSymbol1.Handle
						isParamArray = pEParameterSymbol1.IsParamArray
						hasOptionCompare = pEParameterSymbol1.HasOptionCompare
					End If
					If (accessorParameter IsNot Nothing AndAlso pEParameterSymbol IsNot Nothing) Then
						If ((paramFlags And ParameterAttributes.[Optional]) <> (pEParameterSymbol.ParamFlags And ParameterAttributes.[Optional])) Then
							paramFlags = paramFlags And (ParameterAttributes.[In] Or ParameterAttributes.Out Or ParameterAttributes.Lcid Or ParameterAttributes.Retval Or ParameterAttributes.ReservedMask Or ParameterAttributes.HasDefault Or ParameterAttributes.HasFieldMarshal Or ParameterAttributes.Reserved3 Or ParameterAttributes.Reserved4)
							explicitDefaultConstantValue = Nothing
						ElseIf (explicitDefaultConstantValue <> pEParameterSymbol.ExplicitDefaultConstantValue) Then
							explicitDefaultConstantValue = Nothing
							paramFlags = paramFlags And (ParameterAttributes.[In] Or ParameterAttributes.Out Or ParameterAttributes.Lcid Or ParameterAttributes.Retval Or ParameterAttributes.ReservedMask Or ParameterAttributes.HasDefault Or ParameterAttributes.HasFieldMarshal Or ParameterAttributes.Reserved3 Or ParameterAttributes.Reserved4)
						End If
						If (Not CaseInsensitiveComparison.Equals(name, pEParameterSymbol.Name)) Then
							name = Nothing
						End If
						If (pEParameterSymbol.IsByRef) Then
							isByRef = True
						End If
						If (Not pEParameterSymbol.IsParamArray) Then
							isParamArray = False
						End If
						If (hasOptionCompare <> pEParameterSymbol.HasOptionCompare) Then
							hasOptionCompare = False
							parametersMatch = False
						End If
					End If
					parameterSymbolArray(num) = Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEParameterSymbol.Create([property], name, isByRef, VisualBasicCustomModifier.Convert(paramInfo.RefCustomModifiers), paramInfo.Type, handle, paramFlags, isParamArray, hasOptionCompare, num, explicitDefaultConstantValue, VisualBasicCustomModifier.Convert(paramInfo.CustomModifiers))
					num = num + 1
				Loop While num <= length
				empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of ParameterSymbol)(parameterSymbolArray)
			Else
				empty = ImmutableArray(Of ParameterSymbol).Empty
			End If
			Return empty
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim primaryDependency As AssemblySymbol = MyBase.PrimaryDependency
			If (Not Me._lazyCachedUseSiteInfo.IsInitialized) Then
				Me._lazyCachedUseSiteInfo.Initialize(primaryDependency, MyBase.CalculateUseSiteInfo())
			End If
			Return Me._lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency)
		End Function

		Friend Sub SetIsWithEvents(ByVal value As Boolean)
			Interlocked.CompareExchange(Me._isWithEvents, CInt(If(value, ThreeState.[True], ThreeState.[False])), 0)
		End Sub

		Private NotInheritable Class PEPropertySymbolWithCustomModifiers
			Inherits PEPropertySymbol
			Private ReadOnly _typeCustomModifiers As ImmutableArray(Of CustomModifier)

			Private ReadOnly _refCustomModifiers As ImmutableArray(Of CustomModifier)

			Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return Me._refCustomModifiers
				End Get
			End Property

			Public Overrides ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return Me._typeCustomModifiers
				End Get
			End Property

			Public Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal containingType As PENamedTypeSymbol, ByVal handle As PropertyDefinitionHandle, ByVal getMethod As PEMethodSymbol, ByVal setMethod As PEMethodSymbol, ByVal metadataDecoder As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.MetadataDecoder, ByVal signatureHeader As System.Reflection.Metadata.SignatureHeader, ByVal propertyParams As ParamInfo(Of TypeSymbol)())
				MyBase.New(moduleSymbol, containingType, handle, getMethod, setMethod, metadataDecoder, signatureHeader, propertyParams)
				Dim paramInfo As ParamInfo(Of TypeSymbol) = propertyParams(0)
				Me._typeCustomModifiers = VisualBasicCustomModifier.Convert(paramInfo.CustomModifiers)
				Me._refCustomModifiers = VisualBasicCustomModifier.Convert(paramInfo.RefCustomModifiers)
			End Sub
		End Class
	End Class
End Namespace