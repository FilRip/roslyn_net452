' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Namespace Microsoft.CodeAnalysis.VisualBasic
    Partial Friend NotInheritable Class LocalRewriter
        Public Overrides Function VisitExitStatement(node As BoundExitStatement) As BoundNode
            Dim boundGoto As BoundStatement = New BoundGotoStatement(node.Syntax, node.Label, Nothing)

            If ShouldGenerateUnstructuredExceptionHandlingResumeCode(node) Then
                boundGoto = Concat(RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax), boundGoto)
            End If

            If Instrument(node, boundGoto) Then
                boundGoto = _instrumenterOpt.InstrumentExitStatement(node, boundGoto)
            End If

            Return boundGoto
        End Function
    End Class
End Namespace
