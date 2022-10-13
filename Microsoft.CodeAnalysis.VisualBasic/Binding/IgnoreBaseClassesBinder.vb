' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Namespace Microsoft.CodeAnalysis.VisualBasic

    ''' <summary>
    ''' Causes lookups to ignore base classes. Used for binding
    ''' Imports statements.
    ''' </summary>
    Friend NotInheritable Class IgnoreBaseClassesBinder
        Inherits Binder

        Public Sub New(containingBinder As Binder)
            MyBase.New(containingBinder, ignoreBaseClassesInLookup:=True)
        End Sub
    End Class

End Namespace
