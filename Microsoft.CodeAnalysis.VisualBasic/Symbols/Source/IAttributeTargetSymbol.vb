' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
    Friend Interface IAttributeTargetSymbol
        ''' <summary>
        ''' Attribute location corresponding to this symbol.
        ''' </summary>
        ''' <remarks>
        ''' Location of an attribute if an explicit location is not specified via attribute target specification syntax.
        ''' </remarks>
        ReadOnly Property DefaultAttributeLocation As AttributeLocation
    End Interface
End Namespace

