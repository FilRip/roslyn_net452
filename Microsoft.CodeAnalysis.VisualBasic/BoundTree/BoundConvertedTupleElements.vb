' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Namespace Microsoft.CodeAnalysis.VisualBasic

    Partial Friend Class BoundConvertedTupleElements

#If DEBUG Then
        Private Sub Validate()
            Debug.Assert(ElementPlaceholders.Length = ConvertedElements.Length)
            Debug.Assert(Not ElementPlaceholders.Contains(Nothing))
            Debug.Assert(Not ConvertedElements.Contains(Nothing))
        End Sub
#End If
    End Class
End Namespace
