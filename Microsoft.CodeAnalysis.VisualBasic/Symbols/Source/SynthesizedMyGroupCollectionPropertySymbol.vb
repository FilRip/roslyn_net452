' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Collections.Immutable

Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols

    ''' <summary>
    ''' Represents a compiler "MyGroupCollection" property.
    ''' </summary>
    Friend Class SynthesizedMyGroupCollectionPropertySymbol
        Inherits SynthesizedPropertyBase

        Private ReadOnly _name As String
        Private ReadOnly _field As SynthesizedMyGroupCollectionPropertyBackingFieldSymbol
        Private ReadOnly _getMethod As SynthesizedMyGroupCollectionPropertyGetAccessorSymbol
        Private ReadOnly _setMethodOpt As SynthesizedMyGroupCollectionPropertySetAccessorSymbol
        Public ReadOnly AttributeSyntax As SyntaxReference
        Public ReadOnly DefaultInstanceAlias As String

        Public Sub New(
            container As SourceNamedTypeSymbol,
            attributeSyntax As AttributeSyntax,
            propertyName As String,
            fieldName As String,
            type As NamedTypeSymbol,
            createMethod As String,
            disposeMethod As String,
            defaultInstanceAlias As String
        )
            Me.AttributeSyntax = attributeSyntax.SyntaxTree.GetReference(attributeSyntax)
            Me.DefaultInstanceAlias = defaultInstanceAlias

            _name = propertyName
            _field = New SynthesizedMyGroupCollectionPropertyBackingFieldSymbol(container, Me, type, fieldName)
            _getMethod = New SynthesizedMyGroupCollectionPropertyGetAccessorSymbol(container, Me, createMethod)

            If disposeMethod.Length > 0 Then
                _setMethodOpt = New SynthesizedMyGroupCollectionPropertySetAccessorSymbol(container, Me, disposeMethod)
            End If
        End Sub

        Public Overrides ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        Public Overrides ReadOnly Property Type As TypeSymbol
            Get
                Return _field.Type
            End Get
        End Property

        Public Overrides ReadOnly Property GetMethod As MethodSymbol
            Get
                Return _getMethod
            End Get
        End Property

        Public Overrides ReadOnly Property SetMethod As MethodSymbol
            Get
                Return _setMethodOpt
            End Get
        End Property

        Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
            Return LexicalSortKey.NotInSource
        End Function

        Public Overrides ReadOnly Property ContainingSymbol As Symbol
            Get
                Return _field.ContainingSymbol
            End Get
        End Property

        Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
            Get
                Return _field.ContainingType
            End Get
        End Property

        Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
            Get
                Return _field
            End Get
        End Property

        Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
            Get
                Return ImmutableArray(Of SyntaxReference).Empty
            End Get
        End Property

        Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
            Get
                Return ImmutableArray(Of Location).Empty
            End Get
        End Property

        Friend Overrides ReadOnly Property IsMyGroupCollectionProperty As Boolean
            Get
                Return True
            End Get
        End Property

        Public Sub RelocateDiagnostics(source As DiagnosticBag, destination As DiagnosticBag)
            If source.IsEmptyWithoutResolution Then
                Return
            End If

            Dim diagnosticLocation As Location = AttributeSyntax.GetLocation()

            For Each diag As VBDiagnostic In source.AsEnumerable
                destination.Add(diag.WithLocation(diagnosticLocation))
            Next
        End Sub
    End Class

End Namespace
