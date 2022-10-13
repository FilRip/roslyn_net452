' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Namespace Microsoft.CodeAnalysis.VisualBasic

    Partial Friend Class BoundRangeVariable
        Public Overrides ReadOnly Property ExpressionSymbol As Symbol
            Get
                Return Me.RangeVariable
            End Get
        End Property
    End Class

End Namespace
