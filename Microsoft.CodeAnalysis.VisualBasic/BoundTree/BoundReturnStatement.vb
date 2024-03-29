' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.CodeAnalysis.VisualBasic.Symbols

Namespace Microsoft.CodeAnalysis.VisualBasic

    Partial Friend Class BoundReturnStatement

        Friend Function IsEndOfMethodReturn() As Boolean
            Return Me.ExitLabelOpt Is Nothing
        End Function

#If DEBUG Then
        Private Sub Validate()
            If (ExpressionOpt IsNot Nothing AndAlso Not HasErrors AndAlso (FunctionLocalOpt Is Nothing OrElse FunctionLocalOpt.Type IsNot LambdaSymbol.ReturnTypeIsBeingInferred)) Then
                ExpressionOpt.AssertRValue()
            End If
        End Sub
#End If

    End Class

End Namespace
