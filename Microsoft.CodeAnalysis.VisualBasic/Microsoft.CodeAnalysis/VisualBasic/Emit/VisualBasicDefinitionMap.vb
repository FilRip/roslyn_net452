Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection.Metadata
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class VisualBasicDefinitionMap
		Inherits DefinitionMap
		Private ReadOnly _metadataDecoder As MetadataDecoder

		Private ReadOnly _mapToMetadata As VisualBasicSymbolMatcher

		Private ReadOnly _mapToPrevious As VisualBasicSymbolMatcher

		Protected Overrides ReadOnly Property MapToMetadataSymbolMatcher As SymbolMatcher
			Get
				Return Me._mapToMetadata
			End Get
		End Property

		Protected Overrides ReadOnly Property MapToPreviousSymbolMatcher As SymbolMatcher
			Get
				Return Me._mapToPrevious
			End Get
		End Property

		Friend Overrides ReadOnly Property MessageProvider As CommonMessageProvider
			Get
				Return Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance
			End Get
		End Property

		Public Sub New(ByVal edits As IEnumerable(Of SemanticEdit), ByVal metadataDecoder As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.MetadataDecoder, ByVal mapToMetadata As VisualBasicSymbolMatcher, ByVal mapToPrevious As VisualBasicSymbolMatcher)
			MyBase.New(edits)
			Me._metadataDecoder = metadataDecoder
			Me._mapToMetadata = mapToMetadata
			Me._mapToPrevious = If(mapToPrevious, mapToMetadata)
		End Sub

		Private Shared Function CreateLocalSlotMap(ByVal methodEncInfo As EditAndContinueMethodDebugInformation, ByVal slotMetadata As ImmutableArray(Of LocalInfo(Of TypeSymbol))) As ImmutableArray(Of Microsoft.CodeAnalysis.Emit.EncLocalInfo)
			Dim enumerator As Dictionary(Of Microsoft.CodeAnalysis.Emit.EncLocalInfo, Integer).Enumerator = New Dictionary(Of Microsoft.CodeAnalysis.Emit.EncLocalInfo, Integer).Enumerator()
			Dim key(slotMetadata.Length - 1 + 1 - 1) As Microsoft.CodeAnalysis.Emit.EncLocalInfo
			Dim localSlots As ImmutableArray(Of LocalSlotDebugInfo) = methodEncInfo.LocalSlots
			If (Not localSlots.IsDefault) Then
				Dim num As Integer = Math.Min(localSlots.Length, slotMetadata.Length)
				Dim encLocalInfos As Dictionary(Of Microsoft.CodeAnalysis.Emit.EncLocalInfo, Integer) = New Dictionary(Of Microsoft.CodeAnalysis.Emit.EncLocalInfo, Integer)()
				Dim num1 As Integer = num - 1
				Dim num2 As Integer = 0
				Do
					Dim item As LocalSlotDebugInfo = localSlots(num2)
					If (item.SynthesizedKind.IsLongLived()) Then
						Dim localInfo As LocalInfo(Of TypeSymbol) = slotMetadata(num2)
						If (localInfo.CustomModifiers.IsDefaultOrEmpty) Then
							Dim encLocalInfo As Microsoft.CodeAnalysis.Emit.EncLocalInfo = New Microsoft.CodeAnalysis.Emit.EncLocalInfo(item, DirectCast(localInfo.Type.GetCciAdapter(), ITypeReference), localInfo.Constraints, localInfo.SignatureOpt)
							encLocalInfos.Add(encLocalInfo, num2)
						End If
					End If
					num2 = num2 + 1
				Loop While num2 <= num1
				Try
					enumerator = encLocalInfos.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of Microsoft.CodeAnalysis.Emit.EncLocalInfo, Integer) = enumerator.Current
						key(current.Value) = current.Key
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			End If
			Dim length As Integer = CInt(key.Length) - 1
			Dim num3 As Integer = 0
			Do
				If (key(num3).IsDefault) Then
					key(num3) = New Microsoft.CodeAnalysis.Emit.EncLocalInfo(slotMetadata(num3).SignatureOpt)
				End If
				num3 = num3 + 1
			Loop While num3 <= length
			Return ImmutableArray.Create(Of Microsoft.CodeAnalysis.Emit.EncLocalInfo)(key)
		End Function

		Protected Overrides Function GetISymbolInternalOrNull(ByVal symbol As ISymbol) As ISymbolInternal
			Return TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbol)
		End Function

		Protected Overrides Function GetLambdaSyntaxFacts() As LambdaSyntaxFacts
			Return VisualBasicLambdaSyntaxFacts.Instance
		End Function

		Protected Overrides Function GetLocalSlotMapFromMetadata(ByVal handle As StandaloneSignatureHandle, ByVal debugInfo As EditAndContinueMethodDebugInformation) As ImmutableArray(Of EncLocalInfo)
			Return VisualBasicDefinitionMap.CreateLocalSlotMap(debugInfo, Me._metadataDecoder.GetLocalsOrThrow(handle))
		End Function

		Protected Overrides Sub GetStateMachineFieldMapFromMetadata(ByVal stateMachineType As ITypeSymbolInternal, ByVal localSlotDebugInfo As ImmutableArray(Of Microsoft.CodeAnalysis.CodeGen.LocalSlotDebugInfo), <Out> ByRef hoistedLocalMap As IReadOnlyDictionary(Of Microsoft.CodeAnalysis.Emit.EncHoistedLocalInfo, Integer), <Out> ByRef awaiterMap As IReadOnlyDictionary(Of ITypeReference, Integer), <Out> ByRef awaiterSlotCount As Integer)
			Dim num As Integer
			Dim encHoistedLocalInfos As Dictionary(Of Microsoft.CodeAnalysis.Emit.EncHoistedLocalInfo, Integer) = New Dictionary(Of Microsoft.CodeAnalysis.Emit.EncHoistedLocalInfo, Integer)()
			Dim typeReferences As Dictionary(Of ITypeReference, Integer) = New Dictionary(Of ITypeReference, Integer)(SymbolEquivalentEqualityComparer.Instance)
			Dim num1 As Integer = -1
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = DirectCast(stateMachineType, TypeSymbol).GetMembers().GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Symbol = enumerator.Current
				If (current.Kind <> SymbolKind.Field) Then
					Continue While
				End If
				Dim name As String = current.Name
				Dim kind As GeneratedNameKind = GeneratedNames.GetKind(name)
				If (kind <> GeneratedNameKind.HoistedSynthesizedLocalField) Then
					If (kind = GeneratedNameKind.StateMachineAwaiterField) Then
						If (Not GeneratedNames.TryParseSlotIndex("$A", name, num)) Then
							Continue While
						End If
						Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
						typeReferences(DirectCast(fieldSymbol.Type.GetCciAdapter(), ITypeReference)) = num
						If (num <= num1) Then
							Continue While
						End If
						num1 = num
						Continue While
					ElseIf (kind <> GeneratedNameKind.StateMachineHoistedUserVariableField) Then
						Continue While
					End If
				End If
				Dim str As String = Nothing
				If (Not GeneratedNames.TryParseSlotIndex("$S", name, num) AndAlso Not GeneratedNames.TryParseStateMachineHoistedUserVariableName(name, str, num)) Then
					Continue While
				End If
				Dim fieldSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
				If (num >= localSlotDebugInfo.Length) Then
					Continue While
				End If
				Dim encHoistedLocalInfo As Microsoft.CodeAnalysis.Emit.EncHoistedLocalInfo = New Microsoft.CodeAnalysis.Emit.EncHoistedLocalInfo(localSlotDebugInfo(num), DirectCast(fieldSymbol1.Type.GetCciAdapter(), ITypeReference))
				encHoistedLocalInfos(encHoistedLocalInfo) = num
			End While
			hoistedLocalMap = encHoistedLocalInfos
			awaiterMap = typeReferences
			awaiterSlotCount = num1 + 1
		End Sub

		Friend Function TryGetAnonymousTypeName(ByVal template As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol, <Out> ByRef name As String, <Out> ByRef index As Integer) As Boolean
			Return Me._mapToPrevious.TryGetAnonymousTypeName(template, name, index)
		End Function

		Friend Overrides Function TryGetEventHandle(ByVal def As IEventDefinition, <Out> ByRef handle As EventDefinitionHandle) As Boolean
			Dim flag As Boolean
			Dim internalSymbol As Object
			Dim definition As IDefinition = Me._mapToMetadata.MapDefinition(def)
			If (definition IsNot Nothing) Then
				internalSymbol = definition.GetInternalSymbol()
			Else
				internalSymbol = Nothing
			End If
			Dim pEEventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEEventSymbol = TryCast(internalSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEEventSymbol)
			If (pEEventSymbol Is Nothing) Then
				handle = New EventDefinitionHandle()
				flag = False
			Else
				handle = pEEventSymbol.Handle
				flag = True
			End If
			Return flag
		End Function

		Friend Overrides Function TryGetFieldHandle(ByVal def As IFieldDefinition, <Out> ByRef handle As FieldDefinitionHandle) As Boolean
			Dim flag As Boolean
			Dim internalSymbol As Object
			Dim definition As IDefinition = Me._mapToMetadata.MapDefinition(def)
			If (definition IsNot Nothing) Then
				internalSymbol = definition.GetInternalSymbol()
			Else
				internalSymbol = Nothing
			End If
			Dim pEFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEFieldSymbol = TryCast(internalSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEFieldSymbol)
			If (pEFieldSymbol Is Nothing) Then
				handle = New FieldDefinitionHandle()
				flag = False
			Else
				handle = pEFieldSymbol.Handle
				flag = True
			End If
			Return flag
		End Function

		Friend Overrides Function TryGetMethodHandle(ByVal def As IMethodDefinition, <Out> ByRef handle As MethodDefinitionHandle) As Boolean
			Dim flag As Boolean
			Dim internalSymbol As Object
			Dim definition As IDefinition = Me._mapToMetadata.MapDefinition(def)
			If (definition IsNot Nothing) Then
				internalSymbol = definition.GetInternalSymbol()
			Else
				internalSymbol = Nothing
			End If
			Dim pEMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol = TryCast(internalSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol)
			If (pEMethodSymbol Is Nothing) Then
				handle = New MethodDefinitionHandle()
				flag = False
			Else
				handle = pEMethodSymbol.Handle
				flag = True
			End If
			Return flag
		End Function

		Friend Overrides Function TryGetPropertyHandle(ByVal def As IPropertyDefinition, <Out> ByRef handle As PropertyDefinitionHandle) As Boolean
			Dim flag As Boolean
			Dim internalSymbol As Object
			Dim definition As IDefinition = Me._mapToMetadata.MapDefinition(def)
			If (definition IsNot Nothing) Then
				internalSymbol = definition.GetInternalSymbol()
			Else
				internalSymbol = Nothing
			End If
			Dim pEPropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEPropertySymbol = TryCast(internalSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEPropertySymbol)
			If (pEPropertySymbol Is Nothing) Then
				handle = New PropertyDefinitionHandle()
				flag = False
			Else
				handle = pEPropertySymbol.Handle
				flag = True
			End If
			Return flag
		End Function

		Protected Overrides Function TryGetStateMachineType(ByVal methodHandle As EntityHandle) As ITypeSymbolInternal
			Dim typeSymbolForSerializedType As ITypeSymbolInternal
			Dim str As String = Nothing
			If (Me._metadataDecoder.[Module].HasStringValuedAttribute(methodHandle, AttributeDescription.AsyncStateMachineAttribute, str) OrElse Me._metadataDecoder.[Module].HasStringValuedAttribute(methodHandle, AttributeDescription.IteratorStateMachineAttribute, str)) Then
				typeSymbolForSerializedType = Me._metadataDecoder.GetTypeSymbolForSerializedType(str)
			Else
				typeSymbolForSerializedType = Nothing
			End If
			Return typeSymbolForSerializedType
		End Function

		Friend Overrides Function TryGetTypeHandle(ByVal def As ITypeDefinition, <Out> ByRef handle As TypeDefinitionHandle) As Boolean
			Dim flag As Boolean
			Dim internalSymbol As Object
			Dim definition As IDefinition = Me._mapToMetadata.MapDefinition(def)
			If (definition IsNot Nothing) Then
				internalSymbol = definition.GetInternalSymbol()
			Else
				internalSymbol = Nothing
			End If
			Dim pENamedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol = TryCast(internalSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PENamedTypeSymbol)
			If (pENamedTypeSymbol Is Nothing) Then
				handle = New TypeDefinitionHandle()
				flag = False
			Else
				handle = pENamedTypeSymbol.Handle
				flag = True
			End If
			Return flag
		End Function
	End Class
End Namespace