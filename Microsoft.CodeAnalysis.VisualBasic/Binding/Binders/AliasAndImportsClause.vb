' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Collections.Immutable

Imports Microsoft.CodeAnalysis.VisualBasic.Symbols

Namespace Microsoft.CodeAnalysis.VisualBasic
    Friend Structure AliasAndImportsClausePosition
        Public ReadOnly [Alias] As AliasSymbol
        Public ReadOnly ImportsClausePosition As Integer
        Public ReadOnly Dependencies As ImmutableArray(Of AssemblySymbol)

        Public Sub New([alias] As AliasSymbol, importsClausePosition As Integer, dependencies As ImmutableArray(Of AssemblySymbol))
            Me.Alias = [alias]
            Me.ImportsClausePosition = importsClausePosition
            Me.Dependencies = dependencies
        End Sub
    End Structure
End Namespace
