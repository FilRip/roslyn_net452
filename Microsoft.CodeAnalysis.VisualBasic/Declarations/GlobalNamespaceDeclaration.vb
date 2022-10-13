' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols

    ''' <summary>
    ''' Represents global namespace. Namespace's name is always empty
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class GlobalNamespaceDeclaration
        Inherits SingleNamespaceDeclaration

        Public Sub New(hasImports As Boolean,
                       syntaxReference As SyntaxReference,
                       nameLocation As Location,
                       children As ImmutableArray(Of SingleNamespaceOrTypeDeclaration))
            MyBase.New(String.Empty, hasImports, syntaxReference, nameLocation, children)
        End Sub

        Public Overrides ReadOnly Property IsGlobalNamespace As Boolean
            Get
                Return True
            End Get
        End Property
    End Class
End Namespace
