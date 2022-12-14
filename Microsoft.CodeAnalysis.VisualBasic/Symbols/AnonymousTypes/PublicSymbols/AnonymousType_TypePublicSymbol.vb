' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Collections.Immutable

Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols

    Partial Friend NotInheritable Class AnonymousTypeManager

        Friend NotInheritable Class AnonymousTypePublicSymbol
            Inherits AnonymousTypeOrDelegatePublicSymbol

            Private ReadOnly _properties As ImmutableArray(Of AnonymousTypePropertyPublicSymbol)
            Private ReadOnly _members As ImmutableArray(Of Symbol)
            Private ReadOnly _interfaces As ImmutableArray(Of NamedTypeSymbol)

            Public Sub New(manager As AnonymousTypeManager, typeDescr As AnonymousTypeDescriptor)
                MyBase.New(manager, typeDescr)

                Dim fieldsCount As Integer = typeDescr.Fields.Length

                Dim methodMembersBuilder = ArrayBuilder(Of Symbol).GetInstance()
                Dim otherMembersBuilder = ArrayBuilder(Of Symbol).GetInstance()

                ' The array storing property symbols to be used in 
                ' generation of constructor and other methods
                Dim propertiesArray = New AnonymousTypePropertyPublicSymbol(fieldsCount - 1) {}

                ' Anonymous types with at least one Key field are being generated slightly different
                Dim hasAtLeastOneKeyField As Boolean = False

                '  Process fields
                For fieldIndex = 0 To fieldsCount - 1

                    Dim field As AnonymousTypeField = typeDescr.Fields(fieldIndex)
                    If field.IsKey Then
                        hasAtLeastOneKeyField = True
                    End If

                    ' Add a property
                    Dim [property] As New AnonymousTypePropertyPublicSymbol(Me, fieldIndex)
                    propertiesArray(fieldIndex) = [property]

                    ' Property related symbols
                    otherMembersBuilder.Add([property])
                    methodMembersBuilder.Add([property].GetMethod)
                    If [property].SetMethod IsNot Nothing Then
                        methodMembersBuilder.Add([property].SetMethod)
                    End If
                Next

                _properties = propertiesArray.AsImmutableOrNull()

                ' Add a constructor
                methodMembersBuilder.Add(CreateConstructorSymbol())
                ' Add 'ToString'
                methodMembersBuilder.Add(CreateToStringMethod())

                ' Add optional members
                If hasAtLeastOneKeyField AndAlso Me.Manager.System_IEquatable_T_Equals IsNot Nothing Then

                    ' Add 'GetHashCode'
                    methodMembersBuilder.Add(CreateGetHashCodeMethod())

                    ' Add optional 'Inherits IEquatable'
                    Dim equatableInterface As NamedTypeSymbol = Me.Manager.System_IEquatable_T.Construct(ImmutableArray.Create(Of TypeSymbol)(Me))
                    Me._interfaces = ImmutableArray.Create(Of NamedTypeSymbol)(equatableInterface)

                    ' Add 'IEquatable.Equals'
                    Dim method As Symbol = DirectCast(equatableInterface, SubstitutedNamedType).GetMemberForDefinition(Me.Manager.System_IEquatable_T_Equals)
                    methodMembersBuilder.Add(CreateIEquatableEqualsMethod(DirectCast(method, MethodSymbol)))

                    ' Add 'Equals'
                    methodMembersBuilder.Add(CreateEqualsMethod())

                Else
                    _interfaces = ImmutableArray(Of NamedTypeSymbol).Empty
                End If

                methodMembersBuilder.AddRange(otherMembersBuilder)
                otherMembersBuilder.Free()
                _members = methodMembersBuilder.ToImmutableAndFree()
            End Sub

            Private Function CreateConstructorSymbol() As MethodSymbol
                Dim constructor As New SynthesizedSimpleConstructorSymbol(Me)

                Dim fieldsCount As Integer = Me._properties.Length
                Dim paramsArr = New ParameterSymbol(fieldsCount - 1) {}
                For index = 0 To fieldsCount - 1
                    Dim [property] As PropertySymbol = Me._properties(index)
                    paramsArr(index) = New SynthesizedParameterSimpleSymbol(constructor, [property].Type, index, [property].Name)
                Next
                constructor.SetParameters(paramsArr.AsImmutableOrNull())

                Return constructor
            End Function

            Private Function CreateEqualsMethod() As MethodSymbol
                Dim method As New SynthesizedSimpleMethodSymbol(Me, WellKnownMemberNames.ObjectEquals, Me.Manager.System_Boolean,
                                                                overriddenMethod:=Me.Manager.System_Object__Equals,
                                                                isOverloads:=True)

                method.SetParameters(ImmutableArray.Create(Of ParameterSymbol)(
                                        New SynthesizedParameterSimpleSymbol(method, Me.Manager.System_Object, 0, "obj")
                                    ))

                Return method
            End Function

            Private Function CreateIEquatableEqualsMethod(iEquatableEquals As MethodSymbol) As MethodSymbol
                Dim method As New SynthesizedSimpleMethodSymbol(Me, WellKnownMemberNames.ObjectEquals, Me.Manager.System_Boolean,
                                                                interfaceMethod:=iEquatableEquals,
                                                                isOverloads:=True)

                method.SetParameters(ImmutableArray.Create(Of ParameterSymbol)(
                                        New SynthesizedParameterSimpleSymbol(method, Me, 0, "val")
                                    ))

                Return method
            End Function

            Private Function CreateGetHashCodeMethod() As MethodSymbol
                Dim method As New SynthesizedSimpleMethodSymbol(Me, WellKnownMemberNames.ObjectGetHashCode, Me.Manager.System_Int32,
                                                                overriddenMethod:=Me.Manager.System_Object__GetHashCode)

                method.SetParameters(ImmutableArray(Of ParameterSymbol).Empty)
                Return method
            End Function

            Private Function CreateToStringMethod() As MethodSymbol
                Dim method As New SynthesizedSimpleMethodSymbol(Me, WellKnownMemberNames.ObjectToString, Me.Manager.System_String,
                                                                overriddenMethod:=Me.Manager.System_Object__ToString)

                method.SetParameters(ImmutableArray(Of ParameterSymbol).Empty)
                Return method
            End Function

            Public Overrides ReadOnly Property TypeKind As TypeKind
                Get
                    Return TypeKind.Class
                End Get
            End Property

            Friend Overrides ReadOnly Property IsInterface As Boolean
                Get
                    Return False
                End Get
            End Property

            Public ReadOnly Property Properties As ImmutableArray(Of AnonymousTypePropertyPublicSymbol)
                Get
                    Return Me._properties
                End Get
            End Property

            Friend Overrides Function InternalSubstituteTypeParameters(substitution As TypeSubstitution) As TypeWithModifiers
                Dim newDescriptor As New AnonymousTypeDescriptor
                If Not Me.TypeDescriptor.SubstituteTypeParametersIfNeeded(substitution, newDescriptor) Then
                    Return New TypeWithModifiers(Me)
                End If

                Return New TypeWithModifiers(Me.Manager.ConstructAnonymousTypeSymbol(newDescriptor))
            End Function

            Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
                Return _members
            End Function

            Friend Overrides Function MakeAcyclicBaseType(diagnostics As BindingDiagnosticBag) As NamedTypeSymbol
                Return Me.Manager.System_Object
            End Function

            Friend Overrides Function MakeAcyclicInterfaces(diagnostics As BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
                Return _interfaces
            End Function

            Public Overrides Function MapToImplementationSymbol() As NamedTypeSymbol
                Return Me.Manager.ConstructAnonymousTypeImplementationSymbol(Me)
            End Function

            Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
                Get
                    Return GetDeclaringSyntaxReferenceHelper(Of AnonymousObjectCreationExpressionSyntax)(Me.Locations)
                End Get
            End Property
        End Class

    End Class

End Namespace
