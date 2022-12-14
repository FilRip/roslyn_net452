' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols

    <Flags()>
    Friend Enum TypeParameterConstraintKind
        None = 0
        ReferenceType = 1
        ValueType = 2
        Constructor = 4
    End Enum

End Namespace
