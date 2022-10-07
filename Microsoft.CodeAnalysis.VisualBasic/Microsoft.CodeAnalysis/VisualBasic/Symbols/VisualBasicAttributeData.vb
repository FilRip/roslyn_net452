Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class VisualBasicAttributeData
		Inherits AttributeData
		Implements ICustomAttribute
		Private _lazyIsSecurityAttribute As ThreeState

		ReadOnly Property AllowMultiple1 As Boolean Implements ICustomAttribute.AllowMultiple
			Get
				Return Me.AttributeClass.GetAttributeUsageInfo().AllowMultiple
			End Get
		End Property

		Public Shadows MustOverride ReadOnly Property ApplicationSyntaxReference As SyntaxReference

		ReadOnly Property ArgumentCount As Integer Implements ICustomAttribute.ArgumentCount
			Get
				Return Me.CommonConstructorArguments.Length
			End Get
		End Property

		Public Shadows MustOverride ReadOnly Property AttributeClass As NamedTypeSymbol

		Public Shadows MustOverride ReadOnly Property AttributeConstructor As MethodSymbol

		Protected Overrides ReadOnly Property CommonApplicationSyntaxReference As SyntaxReference
			Get
				Return Me.ApplicationSyntaxReference
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonAttributeClass As INamedTypeSymbol
			Get
				Return Me.AttributeClass
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonAttributeConstructor As IMethodSymbol
			Get
				Return Me.AttributeConstructor
			End Get
		End Property

		Public Shadows ReadOnly Property ConstructorArguments As IEnumerable(Of TypedConstant)
			Get
				Return DirectCast(Me.CommonConstructorArguments, IEnumerable(Of TypedConstant))
			End Get
		End Property

		ReadOnly Property NamedArgumentCount As UShort Implements ICustomAttribute.NamedArgumentCount
			Get
				Return CUShort(Me.CommonNamedArguments.Length)
			End Get
		End Property

		Public Shadows ReadOnly Property NamedArguments As IEnumerable(Of KeyValuePair(Of String, TypedConstant))
			Get
				Return DirectCast(Me.CommonNamedArguments, IEnumerable(Of KeyValuePair(Of String, TypedConstant)))
			End Get
		End Property

		Protected Sub New()
			MyBase.New()
			Me._lazyIsSecurityAttribute = ThreeState.Unknown
		End Sub

		Private Function Constructor1(ByVal context As EmitContext, ByVal reportDiagnostics As Boolean) As IMethodReference Implements ICustomAttribute.Constructor
			Dim methodReference As IMethodReference
			Dim location As Object
			If (Not Me.AttributeConstructor.IsDefaultValueTypeConstructor()) Then
				methodReference = DirectCast(context.[Module], PEModuleBuilder).Translate(Me.AttributeConstructor, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, False)
			Else
				If (reportDiagnostics) Then
					Dim diagnostics As DiagnosticBag = context.Diagnostics
					Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = context.SyntaxNode
					If (syntaxNode IsNot Nothing) Then
						location = syntaxNode.GetLocation()
					Else
						location = Nothing
					End If
					If (location Is Nothing) Then
						location = NoLocation.Singleton
					End If
					diagnostics.Add(ERRID.ERR_AttributeMustBeClassNotStruct1, location, New [Object]() { Me.AttributeClass })
				End If
				methodReference = Nothing
			End If
			Return methodReference
		End Function

		Private Function CreateMetadataArray(ByVal argument As TypedConstant, ByVal context As EmitContext) As Microsoft.CodeAnalysis.CodeGen.MetadataCreateArray
			Dim metadataCreateArray As Microsoft.CodeAnalysis.CodeGen.MetadataCreateArray
			Dim values As ImmutableArray(Of TypedConstant) = argument.Values
			Dim arrayTypeReference As IArrayTypeReference = DirectCast(context.[Module], PEModuleBuilder).Translate(DirectCast(argument.TypeInternal, ArrayTypeSymbol))
			If (values.Length <> 0) Then
				Dim metadataExpressionArray(values.Length - 1 + 1 - 1) As IMetadataExpression
				Dim length As Integer = values.Length - 1
				Dim num As Integer = 0
				Do
					metadataExpressionArray(num) = Me.CreateMetadataExpression(values(num), context)
					num = num + 1
				Loop While num <= length
				metadataCreateArray = New Microsoft.CodeAnalysis.CodeGen.MetadataCreateArray(arrayTypeReference, arrayTypeReference.GetElementType(context), Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of IMetadataExpression)(metadataExpressionArray))
			Else
				metadataCreateArray = New Microsoft.CodeAnalysis.CodeGen.MetadataCreateArray(arrayTypeReference, arrayTypeReference.GetElementType(context), ImmutableArray(Of IMetadataExpression).Empty)
			End If
			Return metadataCreateArray
		End Function

		Private Function CreateMetadataConstant(ByVal type As ITypeSymbolInternal, ByVal value As Object, ByVal context As EmitContext) As MetadataConstant
			Return DirectCast(context.[Module], PEModuleBuilder).CreateConstant(DirectCast(type, TypeSymbol), RuntimeHelpers.GetObjectValue(value), DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
		End Function

		Private Function CreateMetadataExpression(ByVal argument As TypedConstant, ByVal context As EmitContext) As IMetadataExpression
			Dim metadataExpression As IMetadataExpression
			If (Not argument.IsNull) Then
				Dim kind As TypedConstantKind = argument.Kind
				If (kind = TypedConstantKind.Type) Then
					metadataExpression = Me.CreateType(argument, context)
				ElseIf (kind <> TypedConstantKind.Array) Then
					metadataExpression = Me.CreateMetadataConstant(argument.TypeInternal, RuntimeHelpers.GetObjectValue(argument.ValueInternal), context)
				Else
					metadataExpression = Me.CreateMetadataArray(argument, context)
				End If
			Else
				metadataExpression = Me.CreateMetadataConstant(argument.TypeInternal, Nothing, context)
			End If
			Return metadataExpression
		End Function

		Private Function CreateMetadataNamedArgument(ByVal name As String, ByVal argument As TypedConstant, ByVal context As EmitContext) As IMetadataNamedArgument
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me.LookupName(name)
			Dim metadataExpression As IMetadataExpression = Me.CreateMetadataExpression(argument, context)
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
			typeSymbol = If(fieldSymbol Is Nothing, DirectCast(symbol, PropertySymbol).Type, fieldSymbol.Type)
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Return New MetadataNamedArgument(symbol, [module].Translate(typeSymbol, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics), metadataExpression)
		End Function

		Private Function CreateType(ByVal argument As TypedConstant, ByVal context As EmitContext) As MetadataTypeOf
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Dim syntaxNode As VisualBasicSyntaxNode = DirectCast(context.SyntaxNode, VisualBasicSyntaxNode)
			Dim diagnostics As DiagnosticBag = context.Diagnostics
			Return New MetadataTypeOf([module].Translate(DirectCast(argument.ValueInternal, TypeSymbol), syntaxNode, diagnostics), [module].Translate(DirectCast(argument.TypeInternal, TypeSymbol), syntaxNode, diagnostics))
		End Function

		Friend Sub DecodeClassInterfaceAttribute(ByVal nodeOpt As AttributeSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim classInterfaceType As System.Runtime.InteropServices.ClassInterfaceType
			Dim item As TypedConstant = Me.CommonConstructorArguments(0)
			If (item.Kind = TypedConstantKind.[Enum]) Then
				classInterfaceType = item.DecodeValue(Of System.Runtime.InteropServices.ClassInterfaceType)(SpecialType.System_Enum)
			Else
				classInterfaceType = item.DecodeValue(Of Short)(SpecialType.System_Int16)
			End If
			If (classInterfaceType > System.Runtime.InteropServices.ClassInterfaceType.AutoDual) Then
				diagnostics.Add(ERRID.ERR_BadAttribute1, If(nodeOpt IsNot Nothing, nodeOpt.ArgumentList.Arguments(0).GetLocation(), NoLocation.Singleton), New [Object]() { Me.AttributeClass })
			End If
		End Sub

		Friend Function DecodeDefaultMemberAttribute() As String
			Return MyBase.GetConstructorArgument(Of String)(0, SpecialType.System_String)
		End Function

		Friend Sub DecodeGuidAttribute(ByVal nodeOpt As AttributeSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim guid As System.Guid
			Dim location As Microsoft.CodeAnalysis.Location
			Dim constructorArgument As String = MyBase.GetConstructorArgument(Of String)(0, SpecialType.System_String)
			If (Not System.Guid.TryParseExact(constructorArgument, "D", guid)) Then
				Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = diagnostics
				location = If(nodeOpt IsNot Nothing, nodeOpt.ArgumentList.Arguments(0).GetLocation(), NoLocation.Singleton)
				Dim attributeClass() As [Object] = { Me.AttributeClass, Nothing }
				attributeClass(1) = If(constructorArgument, Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.NullLiteral)
				bindingDiagnosticBag.Add(ERRID.ERR_BadAttributeUuid2, location, attributeClass)
			End If
		End Sub

		Friend Sub DecodeInterfaceTypeAttribute(ByVal node As AttributeSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim comInterfaceType As System.Runtime.InteropServices.ComInterfaceType = System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual
			If (Not Me.DecodeInterfaceTypeAttribute(comInterfaceType)) Then
				Dim arguments As SeparatedSyntaxList(Of ArgumentSyntax) = node.ArgumentList.Arguments
				diagnostics.Add(ERRID.ERR_BadAttribute1, arguments(0).GetLocation(), New [Object]() { Me.AttributeClass })
			End If
		End Sub

		Friend Function DecodeInterfaceTypeAttribute(<Out> ByRef interfaceType As System.Runtime.InteropServices.ComInterfaceType) As Boolean
			Dim comInterfaceType As System.Runtime.InteropServices.ComInterfaceType
			Dim item As TypedConstant = Me.CommonConstructorArguments(0)
			If (item.Kind = TypedConstantKind.[Enum]) Then
				comInterfaceType = item.DecodeValue(Of System.Runtime.InteropServices.ComInterfaceType)(SpecialType.System_Enum)
			Else
				comInterfaceType = item.DecodeValue(Of Short)(SpecialType.System_Int16)
			End If
			interfaceType = comInterfaceType
			Return If(CInt(interfaceType) > 3, False, True)
		End Function

		Friend Function DecodePermissionSetAttribute(ByVal compilation As VisualBasicCompilation, ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation)) As String
			Dim str As String
			Dim str1 As String
			Dim location As Microsoft.CodeAnalysis.Location
			Dim str2 As String = Nothing
			Dim commonNamedArguments As ImmutableArray(Of KeyValuePair(Of String, TypedConstant)) = Me.CommonNamedArguments
			If (commonNamedArguments.Length = 1) Then
				Dim item As KeyValuePair(Of String, TypedConstant) = commonNamedArguments(0)
				Dim attributeClass As NamedTypeSymbol = Me.AttributeClass
				Dim str3 As String = "File"
				Dim str4 As String = "Hex"
				If (EmbeddedOperators.CompareString(item.Key, str3, False) = 0 AndAlso VisualBasicAttributeData.PermissionSetAttributeTypeHasRequiredProperty(attributeClass, str3)) Then
					Dim valueInternal As String = CStr(item.Value.ValueInternal)
					Dim xmlReferenceResolver As Microsoft.CodeAnalysis.XmlReferenceResolver = compilation.Options.XmlReferenceResolver
					If (xmlReferenceResolver IsNot Nothing) Then
						str1 = xmlReferenceResolver.ResolveReference(valueInternal, Nothing)
					Else
						str1 = Nothing
					End If
					str2 = str1
					If (str2 IsNot Nothing) Then
						If (VisualBasicAttributeData.PermissionSetAttributeTypeHasRequiredProperty(attributeClass, str4)) Then
							str = str2
							Return str
						End If
						str = Nothing
						Return str
					Else
						If (arguments.AttributeSyntaxOpt IsNot Nothing) Then
							Dim argumentSyntaxes As SeparatedSyntaxList(Of ArgumentSyntax) = arguments.AttributeSyntaxOpt.ArgumentList.Arguments
							location = argumentSyntaxes(1).GetLocation()
						Else
							location = NoLocation.Singleton
						End If
						Dim location1 As Microsoft.CodeAnalysis.Location = location
						DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag).Add(ERRID.ERR_PermissionSetAttributeInvalidFile, location1, New [Object]() { If(valueInternal, "<empty>"), str3 })
					End If
				End If
			End If
			str = str2
			Return str
		End Function

		Friend Sub DecodeSecurityAttribute(Of T As {WellKnownAttributeData, ISecurityAttributeTarget, New})(ByVal targetSymbol As Symbol, ByVal compilation As VisualBasicCompilation, ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation))
			Dim flag As Boolean = False
			Dim declarativeSecurityAction As System.Reflection.DeclarativeSecurityAction = Me.DecodeSecurityAttributeAction(targetSymbol, compilation, arguments.AttributeSyntaxOpt, flag, DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag))
			If (Not flag) Then
				Dim orCreateData As SecurityWellKnownAttributeData = arguments.GetOrCreateData(Of T)().GetOrCreateData()
				orCreateData.SetSecurityAttribute(arguments.Index, declarativeSecurityAction, arguments.AttributesCount)
				If (Me.IsTargetAttribute(targetSymbol, AttributeDescription.PermissionSetAttribute)) Then
					Dim str As String = Me.DecodePermissionSetAttribute(compilation, arguments)
					If (str IsNot Nothing) Then
						orCreateData.SetPathForPermissionSetAttributeFixup(arguments.Index, str, arguments.AttributesCount)
					End If
				End If
			End If
		End Sub

		Private Function DecodeSecurityAttributeAction(ByVal targetSymbol As Symbol, ByVal compilation As VisualBasicCompilation, ByVal nodeOpt As AttributeSyntax, ByRef hasErrors As Boolean, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As System.Reflection.DeclarativeSecurityAction
			Dim declarativeSecurityAction As System.Reflection.DeclarativeSecurityAction
			Dim attributeClass As [Object]()
			Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim location As Microsoft.CodeAnalysis.Location
			If (Me.AttributeConstructor.ParameterCount <> 0) Then
				Dim typedConstant As Microsoft.CodeAnalysis.TypedConstant = System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Microsoft.CodeAnalysis.TypedConstant)(Me.CommonConstructorArguments)
				Dim typeInternal As TypeSymbol = DirectCast(typedConstant.TypeInternal, TypeSymbol)
				Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = New CompoundUseSiteInfo(Of AssemblySymbol)(diagnostics, compilation.Assembly)
				If (typeInternal IsNot Nothing AndAlso typeInternal.IsOrDerivedFromWellKnownClass(WellKnownType.System_Security_Permissions_SecurityAction, compilation, compoundUseSiteInfo)) Then
					declarativeSecurityAction = Me.ValidateSecurityAction(typedConstant, targetSymbol, nodeOpt, diagnostics, hasErrors)
					Return declarativeSecurityAction
				End If
				diagnostics.Add(If(nodeOpt IsNot Nothing, nodeOpt.Name.GetLocation(), NoLocation.Singleton), compoundUseSiteInfo)
			Else
				If (Not Me.IsTargetAttribute(targetSymbol, AttributeDescription.HostProtectionAttribute)) Then
					bindingDiagnosticBag = diagnostics
					attributeClass = New [Object]() { Me.AttributeClass }
					diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_SecurityAttributeMissingAction, attributeClass)
					location = If(nodeOpt IsNot Nothing, nodeOpt.Name.GetLocation(), NoLocation.Singleton)
					bindingDiagnosticBag.Add(diagnosticInfo, location)
					hasErrors = True
					declarativeSecurityAction = System.Reflection.DeclarativeSecurityAction.None
					Return declarativeSecurityAction
				End If
				declarativeSecurityAction = System.Reflection.DeclarativeSecurityAction.LinkDemand
				Return declarativeSecurityAction
			End If
			bindingDiagnosticBag = diagnostics
			attributeClass = New [Object]() { Me.AttributeClass }
			diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_SecurityAttributeMissingAction, attributeClass)
			location = If(nodeOpt IsNot Nothing, nodeOpt.Name.GetLocation(), NoLocation.Singleton)
			bindingDiagnosticBag.Add(diagnosticInfo, location)
			hasErrors = True
			declarativeSecurityAction = System.Reflection.DeclarativeSecurityAction.None
			Return declarativeSecurityAction
		End Function

		Friend Function DecodeTypeLibTypeAttribute() As Microsoft.Cci.TypeLibTypeFlags
			Dim item As TypedConstant = Me.CommonConstructorArguments(0)
			If (item.Kind <> TypedConstantKind.[Enum]) Then
				Return DirectCast(item.DecodeValue(Of Short)(SpecialType.System_Int16), Microsoft.Cci.TypeLibTypeFlags)
			End If
			Return item.DecodeValue(Of Microsoft.Cci.TypeLibTypeFlags)(SpecialType.System_Enum)
		End Function

		Private Function GetArguments1(ByVal context As EmitContext) As ImmutableArray(Of IMetadataExpression) Implements ICustomAttribute.GetArguments
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of TypedConstant, IMetadataExpression)(Me.CommonConstructorArguments, Function(arg As TypedConstant) Me.CreateMetadataExpression(arg, context))
		End Function

		Private Function GetNamedArguments1(ByVal context As EmitContext) As ImmutableArray(Of IMetadataNamedArgument) Implements ICustomAttribute.GetNamedArguments
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of KeyValuePair(Of String, TypedConstant), IMetadataNamedArgument)(Me.CommonNamedArguments, Function(namedArgument As KeyValuePair(Of String, TypedConstant)) Me.CreateMetadataNamedArgument(namedArgument.Key, namedArgument.Value, context))
		End Function

		Friend MustOverride Function GetTargetAttributeSignatureIndex(ByVal targetSymbol As Symbol, ByVal description As AttributeDescription) As Integer

		Private Function GetType1(ByVal context As EmitContext) As ITypeReference Implements ICustomAttribute.[GetType]
			Return DirectCast(context.[Module], PEModuleBuilder).Translate(Me.AttributeClass, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, False, False)
		End Function

		Friend Function IsSecurityAttribute(ByVal comp As VisualBasicCompilation) As Boolean
			If (Me._lazyIsSecurityAttribute = ThreeState.Unknown) Then
				Dim attributeClass As NamedTypeSymbol = Me.AttributeClass
				Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				Me._lazyIsSecurityAttribute = attributeClass.IsOrDerivedFromWellKnownClass(WellKnownType.System_Security_Permissions_SecurityAttribute, comp, discarded).ToThreeState()
			End If
			Return Me._lazyIsSecurityAttribute.Value()
		End Function

		Protected Friend NotOverridable Overrides Function IsStringProperty(ByVal memberName As String) As Boolean
			Dim flag As Boolean
			If (Me.AttributeClass IsNot Nothing) Then
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me.AttributeClass.GetMembers(memberName).GetEnumerator()
				While enumerator.MoveNext()
					Dim current As PropertySymbol = TryCast(enumerator.Current, PropertySymbol)
					If (If(current IsNot Nothing, current.Type.SpecialType <> SpecialType.System_String, True)) Then
						Continue While
					End If
					flag = True
					Return flag
				End While
			End If
			flag = False
			Return flag
		End Function

		Friend Overridable Function IsTargetAttribute(ByVal namespaceName As String, ByVal typeName As String, Optional ByVal ignoreCase As Boolean = False) As Boolean
			Dim flag As Boolean
			If (Not Me.AttributeClass.IsErrorType() OrElse TypeOf Me.AttributeClass Is MissingMetadataTypeSymbol) Then
				Dim stringComparison As System.StringComparison = If(ignoreCase, System.StringComparison.OrdinalIgnoreCase, System.StringComparison.Ordinal)
				flag = If(Not Me.AttributeClass.HasNameQualifier(namespaceName, stringComparison), False, Me.AttributeClass.Name.Equals(typeName, stringComparison))
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Function IsTargetAttribute(ByVal targetSymbol As Symbol, ByVal description As AttributeDescription) As Boolean
			Return Me.GetTargetAttributeSignatureIndex(targetSymbol, description) <> -1
		End Function

		Friend Shared Function IsTargetEarlyAttribute(ByVal attributeType As NamedTypeSymbol, ByVal attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax, ByVal description As AttributeDescription) As Boolean
			Dim func As Func(Of ArgumentSyntax, Boolean)
			Dim num As Integer
			If (attributeSyntax.ArgumentList IsNot Nothing) Then
				Dim arguments As IEnumerable(Of ArgumentSyntax) = DirectCast(attributeSyntax.ArgumentList.Arguments, IEnumerable(Of ArgumentSyntax))
				If (VisualBasicAttributeData._Closure$__.$I31-0 Is Nothing) Then
					func = Function(arg As ArgumentSyntax)
						If (arg.Kind() <> SyntaxKind.SimpleArgument) Then
							Return False
						End If
						Return Not arg.IsNamed
					End Function
					VisualBasicAttributeData._Closure$__.$I31-0 = func
				Else
					func = VisualBasicAttributeData._Closure$__.$I31-0
				End If
				num = arguments.Where(func).Count()
			Else
				num = 0
			End If
			Return AttributeData.IsTargetEarlyAttribute(attributeType, num, description)
		End Function

		Private Function LookupName(ByVal name As String) As Symbol
			Dim unknownResultType As Symbol
			Dim attributeClass As NamedTypeSymbol = Me.AttributeClass
		Label0:
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = attributeClass.GetMembers(name).GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Symbol = enumerator.Current
					If (current.DeclaredAccessibility = Accessibility.[Public]) Then
						unknownResultType = current
						Exit While
					End If
				Else
					attributeClass = attributeClass.BaseTypeNoUseSiteDiagnostics
					If (attributeClass IsNot Nothing) Then
						GoTo Label0
					End If
					unknownResultType = ErrorTypeSymbol.UnknownResultType
					Exit While
				End If
			End While
			Return unknownResultType
		End Function

		Private Shared Function PermissionSetAttributeTypeHasRequiredProperty(ByVal permissionSetType As NamedTypeSymbol, ByVal propName As String) As Boolean
			Dim flag As Boolean
			Dim members As ImmutableArray(Of Symbol) = permissionSetType.GetMembers(propName)
			If (members.Length = 1 AndAlso members(0).Kind = SymbolKind.[Property]) Then
				Dim item As PropertySymbol = DirectCast(members(0), PropertySymbol)
				If (item.Type Is Nothing OrElse item.Type.SpecialType <> SpecialType.System_String OrElse item.DeclaredAccessibility <> Accessibility.[Public] OrElse item.GetArity() <> 0 OrElse Not item.HasSet OrElse item.SetMethod.DeclaredAccessibility <> Accessibility.[Public]) Then
					flag = False
					Return flag
				End If
				flag = True
				Return flag
			End If
			flag = False
			Return flag
		End Function

		Friend Function ShouldEmitAttribute(ByVal target As Symbol, ByVal isReturnType As Boolean, ByVal emittingAssemblyAttributesInNetModule As Boolean) As Boolean
			Dim flag As Boolean
			If (Me.HasErrors) Then
				Throw ExceptionUtilities.Unreachable
			End If
			If (Not Me.IsConditionallyOmitted) Then
				Select Case target.Kind
					Case SymbolKind.Assembly
						If ((emittingAssemblyAttributesInNetModule OrElse Not Me.IsTargetAttribute(target, AttributeDescription.AssemblyCultureAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.AssemblyVersionAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.AssemblyFlagsAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.AssemblyAlgorithmIdAttribute)) AndAlso (Not Me.IsTargetAttribute(target, AttributeDescription.CLSCompliantAttribute) OrElse target.DeclaringCompilation.Options.OutputKind <> OutputKind.NetModule) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.TypeForwardedToAttribute) AndAlso Not Me.IsSecurityAttribute(target.DeclaringCompilation)) Then
							GoTo Label0
						End If
						flag = False
						Exit Select
					Case SymbolKind.DynamicType
					Case SymbolKind.ErrorType
					Case SymbolKind.Label
					Case SymbolKind.Local
					Case SymbolKind.[Namespace]
					Case SymbolKind.PointerType
					Label0:
						flag = True
						Exit Select
					Case SymbolKind.[Event]
						If (Not Me.IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.NonSerializedAttribute)) Then
							GoTo Label0
						End If
						flag = False
						Exit Select
					Case SymbolKind.Field
						If (Not Me.IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.NonSerializedAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.FieldOffsetAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.MarshalAsAttribute)) Then
							GoTo Label0
						End If
						flag = False
						Exit Select
					Case SymbolKind.Method
						If (Not isReturnType) Then
							If (Not Me.IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.MethodImplAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.DllImportAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.PreserveSigAttribute) AndAlso Not Me.IsSecurityAttribute(target.DeclaringCompilation)) Then
								GoTo Label0
							End If
							flag = False
							Exit Select
						Else
							If (Not Me.IsTargetAttribute(target, AttributeDescription.MarshalAsAttribute)) Then
								GoTo Label0
							End If
							flag = False
							Exit Select
						End If
					Case SymbolKind.NetModule
						If (Not Me.IsTargetAttribute(target, AttributeDescription.CLSCompliantAttribute) OrElse target.DeclaringCompilation.Options.OutputKind = OutputKind.NetModule) Then
							GoTo Label0
						End If
						flag = False
						Exit Select
					Case SymbolKind.NamedType
						If (Not Me.IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.ComImportAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.SerializableAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.StructLayoutAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.WindowsRuntimeImportAttribute) AndAlso Not Me.IsSecurityAttribute(target.DeclaringCompilation)) Then
							GoTo Label0
						End If
						flag = False
						Exit Select
					Case SymbolKind.Parameter
						If (Not Me.IsTargetAttribute(target, AttributeDescription.OptionalAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.MarshalAsAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.InAttribute) AndAlso Not Me.IsTargetAttribute(target, AttributeDescription.OutAttribute)) Then
							GoTo Label0
						End If
						flag = False
						Exit Select
					Case SymbolKind.[Property]
						If (Not Me.IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute)) Then
							GoTo Label0
						End If
						flag = False
						Exit Select
					Case Else
						GoTo Label0
				End Select
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Overrides Function ToString() As String
			Dim str As String
			If (Me.AttributeClass Is Nothing) Then
				str = MyBase.ToString()
			Else
				Dim displayString As String = Me.AttributeClass.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat)
				If (Not (Not System.Linq.ImmutableArrayExtensions.Any(Of TypedConstant)(Me.CommonConstructorArguments) And Not System.Linq.ImmutableArrayExtensions.Any(Of KeyValuePair(Of String, TypedConstant))(Me.CommonNamedArguments))) Then
					Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
					Dim builder As StringBuilder = instance.Builder
					builder.Append(displayString)
					builder.Append("(")
					Dim flag As Boolean = True
					Dim enumerator As ImmutableArray(Of TypedConstant).Enumerator = Me.CommonConstructorArguments.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As TypedConstant = enumerator.Current
						If (Not flag) Then
							builder.Append(", ")
						End If
						builder.Append(current.ToVisualBasicString())
						flag = False
					End While
					Dim enumerator1 As ImmutableArray(Of KeyValuePair(Of String, TypedConstant)).Enumerator = Me.CommonNamedArguments.GetEnumerator()
					While enumerator1.MoveNext()
						Dim keyValuePair As KeyValuePair(Of String, TypedConstant) = enumerator1.Current
						If (Not flag) Then
							builder.Append(", ")
						End If
						builder.Append(keyValuePair.Key)
						builder.Append(":=")
						builder.Append(keyValuePair.Value.ToVisualBasicString())
						flag = False
					End While
					builder.Append(")")
					str = instance.ToStringAndFree()
				Else
					str = displayString
				End If
			End If
			Return str
		End Function

		Private Function ValidateSecurityAction(ByVal typedValue As TypedConstant, ByVal targetSymbol As Symbol, ByVal nodeOpt As AttributeSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <Out> ByRef hasErrors As Boolean) As System.Reflection.DeclarativeSecurityAction
			Dim declarativeSecurityAction As System.Reflection.DeclarativeSecurityAction
			Dim flag As Boolean = False
			Dim arguments As SeparatedSyntaxList(Of ArgumentSyntax)
			Dim str As [Object]
			Dim location As Microsoft.CodeAnalysis.Location
			Dim obj As [Object]
			Dim singleton As Microsoft.CodeAnalysis.Location
			Dim str1 As [Object]
			Dim location1 As Microsoft.CodeAnalysis.Location
			Dim obj1 As [Object]
			Dim singleton1 As Microsoft.CodeAnalysis.Location
			Dim [integer] As Integer = Microsoft.VisualBasic.CompilerServices.Conversions.ToInteger(typedValue.ValueInternal)
			hasErrors = False
			Select Case [integer]
				Case 1
					If (flag) Then
						If (targetSymbol.Kind = SymbolKind.NamedType OrElse targetSymbol.Kind = SymbolKind.Method) Then
							Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = diagnostics
							If (nodeOpt IsNot Nothing) Then
								arguments = nodeOpt.ArgumentList.Arguments
								singleton = arguments(0).GetLocation()
							Else
								singleton = NoLocation.Singleton
							End If
							Dim objArray(0) As [Object]
							If (nodeOpt IsNot Nothing) Then
								arguments = nodeOpt.ArgumentList.Arguments
								obj = arguments(0).ToString()
							Else
								obj = ""
							End If
							objArray(0) = obj
							bindingDiagnosticBag.Add(ERRID.ERR_SecurityAttributeInvalidActionTypeOrMethod, singleton, objArray)
							hasErrors = True
							declarativeSecurityAction = System.Reflection.DeclarativeSecurityAction.None
							Exit Select
						End If
					ElseIf (targetSymbol.Kind = SymbolKind.Assembly) Then
						Dim bindingDiagnosticBag1 As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = diagnostics
						If (nodeOpt IsNot Nothing) Then
							arguments = nodeOpt.ArgumentList.Arguments
							location = arguments(0).GetLocation()
						Else
							location = NoLocation.Singleton
						End If
						Dim objArray1(0) As [Object]
						If (nodeOpt IsNot Nothing) Then
							arguments = nodeOpt.ArgumentList.Arguments
							str = arguments(0).ToString()
						Else
							str = ""
						End If
						objArray1(0) = str
						bindingDiagnosticBag1.Add(ERRID.ERR_SecurityAttributeInvalidActionAssembly, location, objArray1)
						hasErrors = True
						declarativeSecurityAction = System.Reflection.DeclarativeSecurityAction.None
						Exit Select
					End If
					declarativeSecurityAction = CShort([integer])
					Exit Select
				Case 2
				Case 3
				Case 4
				Case 5
					flag = False
					GoTo Label0
				Case 6
				Case 7
					If (Not Me.IsTargetAttribute(targetSymbol, AttributeDescription.PrincipalPermissionAttribute)) Then
						flag = False
						GoTo Label0
					Else
						Dim bindingDiagnosticBag2 As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = diagnostics
						If (nodeOpt IsNot Nothing) Then
							arguments = nodeOpt.ArgumentList.Arguments
							location1 = arguments(0).GetLocation()
						Else
							location1 = NoLocation.Singleton
						End If
						Dim objArray2(0) As [Object]
						If (nodeOpt IsNot Nothing) Then
							arguments = nodeOpt.ArgumentList.Arguments
							str1 = arguments(0).ToString()
						Else
							str1 = ""
						End If
						objArray2(0) = str1
						bindingDiagnosticBag2.Add(ERRID.ERR_PrincipalPermissionInvalidAction, location1, objArray2)
						hasErrors = True
						declarativeSecurityAction = System.Reflection.DeclarativeSecurityAction.None
						Exit Select
					End If
				Case 8
				Case 9
				Case 10
					flag = True
					GoTo Label0
				Case Else
					Dim bindingDiagnosticBag3 As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = diagnostics
					If (nodeOpt IsNot Nothing) Then
						arguments = nodeOpt.ArgumentList.Arguments
						singleton1 = arguments(0).GetLocation()
					Else
						singleton1 = NoLocation.Singleton
					End If
					Dim objArray3() As [Object] = { If(nodeOpt IsNot Nothing, nodeOpt.Name.ToString(), ""), Nothing }
					If (nodeOpt IsNot Nothing) Then
						arguments = nodeOpt.ArgumentList.Arguments
						obj1 = arguments(0).ToString()
					Else
						obj1 = ""
					End If
					objArray3(1) = obj1
					bindingDiagnosticBag3.Add(ERRID.ERR_SecurityAttributeInvalidActionTypeOrMethod, singleton1, objArray3)
					hasErrors = True
					declarativeSecurityAction = System.Reflection.DeclarativeSecurityAction.None
					Exit Select
			End Select
			Return declarativeSecurityAction
		End Function
	End Class
End Namespace