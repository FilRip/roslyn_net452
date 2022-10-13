' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Namespace Microsoft.CodeAnalysis.VisualBasic

    ''' <summary>
    ''' Provides context for binding statements in speculative code.
    ''' </summary>
    Friend NotInheritable Class SpeculativeStatementBinder
        Inherits ExecutableCodeBinder

        ''' <summary>
        ''' Create binder for binding statements in speculative code. 
        ''' </summary>
        Public Sub New(root As VisualBasicSyntaxNode, containingBinder As Binder)
            MyBase.New(root, containingBinder)
        End Sub

        Public Overrides ReadOnly Property IsSemanticModelBinder As Boolean
            Get
                Return True
            End Get
        End Property
    End Class

End Namespace
