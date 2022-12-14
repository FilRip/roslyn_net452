' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.CodeAnalysis.VisualBasic.Symbols

Namespace Microsoft.CodeAnalysis.VisualBasic

    Partial Friend Class BoundUnstructuredExceptionHandlingCatchFilter

#If DEBUG Then
        Private Sub Validate()
            Debug.Assert(Me.Type.IsBooleanType())
        End Sub
#End If

    End Class

End Namespace
