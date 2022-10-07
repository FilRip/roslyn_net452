Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SourceAttributeData
		Inherits VisualBasicAttributeData
		Private ReadOnly _attributeClass As NamedTypeSymbol

		Private ReadOnly _attributeConstructor As MethodSymbol

		Private ReadOnly _constructorArguments As ImmutableArray(Of TypedConstant)

		Private ReadOnly _namedArguments As ImmutableArray(Of KeyValuePair(Of String, TypedConstant))

		Private ReadOnly _isConditionallyOmitted As Boolean

		Private ReadOnly _hasErrors As Boolean

		Private ReadOnly _applicationNode As SyntaxReference

		Public Overrides ReadOnly Property ApplicationSyntaxReference As SyntaxReference
			Get
				Return Me._applicationNode
			End Get
		End Property

		Public Overrides ReadOnly Property AttributeClass As NamedTypeSymbol
			Get
				Return Me._attributeClass
			End Get
		End Property

		Public Overrides ReadOnly Property AttributeConstructor As MethodSymbol
			Get
				Return Me._attributeConstructor
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonConstructorArguments As ImmutableArray(Of TypedConstant)
			Get
				Return Me._constructorArguments
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonNamedArguments As ImmutableArray(Of KeyValuePair(Of String, TypedConstant))
			Get
				Return Me._namedArguments
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasErrors As Boolean
			Get
				Return Me._hasErrors
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsConditionallyOmitted As Boolean
			Get
				Return Me._isConditionallyOmitted
			End Get
		End Property

		Friend Sub New(ByVal applicationNode As SyntaxReference, ByVal attrClass As NamedTypeSymbol, ByVal attrMethod As MethodSymbol, ByVal constructorArgs As ImmutableArray(Of TypedConstant), ByVal namedArgs As ImmutableArray(Of KeyValuePair(Of String, TypedConstant)), ByVal isConditionallyOmitted As Boolean, ByVal hasErrors As Boolean)
			MyBase.New()
			Me._applicationNode = applicationNode
			Me._attributeClass = attrClass
			Me._attributeConstructor = attrMethod
			Me._constructorArguments = ImmutableArrayExtensions.NullToEmpty(Of TypedConstant)(constructorArgs)
			Me._namedArguments = If(namedArgs.IsDefault, ImmutableArray.Create(Of KeyValuePair(Of String, TypedConstant))(), namedArgs)
			Me._isConditionallyOmitted = isConditionallyOmitted
			Me._hasErrors = hasErrors
		End Sub

		Friend Overridable Function GetSystemType(ByVal targetSymbol As Symbol) As TypeSymbol
			Return targetSymbol.DeclaringCompilation.GetWellKnownType(WellKnownType.System_Type)
		End Function

		Friend Overrides Function GetTargetAttributeSignatureIndex(ByVal targetSymbol As Symbol, ByVal description As AttributeDescription) As Integer
			Dim num As Integer
			Dim underlying As Byte
			Dim item As AttributeDescription.TypeHandleTargetInfo
			If (Me.IsTargetAttribute(description.[Namespace], description.Name, description.MatchIgnoringCase)) Then
				Dim systemType As TypeSymbol = Nothing
				Dim attributeConstructor As MethodSymbol = Me.AttributeConstructor
				If (attributeConstructor IsNot Nothing) Then
					Dim parameters As ImmutableArray(Of ParameterSymbol) = attributeConstructor.Parameters
					Dim length As Boolean = False
					Dim length1 As Integer = CInt(description.Signatures.Length) - 1
					Dim num1 As Integer = 0
					Do
						Dim signatures As Byte() = description.Signatures(num1)
						If (signatures(0) = 32 AndAlso signatures(1) = parameters.Length AndAlso signatures(2) = 1) Then
							length = CInt(signatures.Length) = 3
							Dim num2 As Integer = 0
							Dim length2 As Integer = CInt(signatures.Length) - 1
							Dim num3 As Integer = 3
							While True
								If (num3 <= length2 AndAlso num2 < parameters.Length) Then
									Dim type As TypeSymbol = parameters(num2).Type
									Dim specialType As Microsoft.CodeAnalysis.SpecialType = type.GetEnumUnderlyingTypeOrSelf().SpecialType
									underlying = signatures(num3)
									If (underlying = 64) Then
										num3 = num3 + 1
										If (type.Kind = SymbolKind.NamedType OrElse type.Kind = SymbolKind.ErrorType) Then
											Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
											item = AttributeDescription.TypeHandleTargets(signatures(num3))
											If ([String].Equals(namedTypeSymbol.MetadataName, item.Name, StringComparison.Ordinal) AndAlso namedTypeSymbol.HasNameQualifier(item.[Namespace], StringComparison.Ordinal)) Then
												GoTo Label4
											End If
											length = False
											GoTo Label3
										Else
											length = False
											GoTo Label3
										End If
									ElseIf (type.IsArrayType()) Then
										specialType = DirectCast(type, ArrayTypeSymbol).ElementType.SpecialType
									End If
								Label5:
									Select Case underlying
										Case 2
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_Boolean
											num2 = num2 + 1
											Exit Select
										Case 3
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_Char
											num2 = num2 + 1
											Exit Select
										Case 4
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_SByte
											num2 = num2 + 1
											Exit Select
										Case 5
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_Byte
											num2 = num2 + 1
											Exit Select
										Case 6
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_Int16
											num2 = num2 + 1
											Exit Select
										Case 7
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_UInt16
											num2 = num2 + 1
											Exit Select
										Case 8
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_Int32
											num2 = num2 + 1
											Exit Select
										Case 9
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_UInt32
											num2 = num2 + 1
											Exit Select
										Case 10
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_Int64
											num2 = num2 + 1
											Exit Select
										Case 11
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_UInt64
											num2 = num2 + 1
											Exit Select
										Case 12
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_Single
											num2 = num2 + 1
											Exit Select
										Case 13
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_Double
											num2 = num2 + 1
											Exit Select
										Case 14
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_String
											num2 = num2 + 1
											Exit Select
										Case 15
										Case 16
										Case 17
										Case 18
										Case 19
										Case 20
										Case 21
										Case 22
										Case 23
										Case 24
										Case 25
										Case 26
										Case 27
											num = -1
											Return num
										Case 28
											length = specialType = Microsoft.CodeAnalysis.SpecialType.System_Object
											num2 = num2 + 1
											Exit Select
										Case 29
											length = type.IsArrayType()
											Exit Select
										Case Else
											If (underlying = 80) Then
												If (systemType Is Nothing) Then
													systemType = Me.GetSystemType(targetSymbol)
												End If
												length = TypeSymbol.Equals(type, systemType, TypeCompareKind.ConsiderEverything)
												num2 = num2 + 1
												Exit Select
											Else
												num = -1
												Return num
											End If
									End Select
									If (Not length) Then
										GoTo Label3
									End If
									num3 = num3 + 1
									Continue While
								End If
							Label3:
								If (Not length) Then
									GoTo Label1
								End If
								num = num1
								Return num
							End While
							num = -1
							Return num
						End If
					Label1:
						num1 = num1 + 1
					Loop While num1 <= length1
					num = -1
				Else
					num = -1
				End If
			Else
				num = -1
			End If
			Return num
		Label4:
			underlying = CByte(item.Underlying)
			GoTo Label5
		End Function

		Friend Function WithOmittedCondition(ByVal isConditionallyOmitted As Boolean) As SourceAttributeData
			Dim sourceAttributeDatum As SourceAttributeData
			sourceAttributeDatum = If(Me.IsConditionallyOmitted <> isConditionallyOmitted, New SourceAttributeData(Me.ApplicationSyntaxReference, Me.AttributeClass, Me.AttributeConstructor, Me.CommonConstructorArguments, Me.CommonNamedArguments, isConditionallyOmitted, Me.HasErrors), Me)
			Return sourceAttributeDatum
		End Function
	End Class
End Namespace