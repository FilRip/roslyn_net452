Imports Microsoft.CodeAnalysis
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Structure TypeWithModifiers
		Implements IEquatable(Of TypeWithModifiers)
		Public ReadOnly Type As TypeSymbol

		Public ReadOnly CustomModifiers As ImmutableArray(Of CustomModifier)

		Public Sub New(ByVal type As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier))
			Me = New TypeWithModifiers() With
			{
				.Type = type,
				.CustomModifiers = Microsoft.CodeAnalysis.ImmutableArrayExtensions.NullToEmpty(Of CustomModifier)(customModifiers)
			}
		End Sub

		Public Sub New(ByVal type As TypeSymbol)
			Me = New TypeWithModifiers() With
			{
				.Type = type,
				.CustomModifiers = ImmutableArray(Of CustomModifier).Empty
			}
		End Sub

		Public Function AsTypeSymbolOnly() As TypeSymbol
			Return Me.Type
		End Function

		<Obsolete("Use the strongly typed overload.", True)>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean Implements IEquatable(Of TypeWithModifiers).Equals
			If (Not TypeOf obj Is TypeWithModifiers) Then
				Return False
			End If
			Return Me.ExplicitEquals(DirectCast(obj, TypeWithModifiers))
		End Function

		Public Function ExplicitEquals(ByVal other As TypeWithModifiers) As Boolean Implements IEquatable(Of TypeWithModifiers).Equals
			Return Me.IsSameType(other, TypeCompareKind.ConsiderEverything)
		End Function

		<Obsolete("Use Is method.", True)>
		Public Function Equals(ByVal other As TypeSymbol) As Boolean Implements IEquatable(Of TypeWithModifiers).Equals
			Return Me.[Is](other)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Of TypeSymbol)(Me.Type, Hash.CombineValues(Of CustomModifier)(Me.CustomModifiers, 2147483647))
		End Function

		Public Function InternalSubstituteTypeParameters(ByVal substitution As TypeSubstitution) As TypeWithModifiers
			Dim typeWithModifier As TypeWithModifiers
			Dim customModifiers As ImmutableArray(Of CustomModifier) = If(substitution IsNot Nothing, substitution.SubstituteCustomModifiers(Me.CustomModifiers), Me.CustomModifiers)
			Dim typeWithModifier1 As TypeWithModifiers = Me.Type.InternalSubstituteTypeParameters(substitution)
			typeWithModifier = If(Not typeWithModifier1.[Is](Me.Type) OrElse customModifiers <> Me.CustomModifiers, New TypeWithModifiers(typeWithModifier1.Type, Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of CustomModifier)(customModifiers, typeWithModifier1.CustomModifiers)), Me)
			Return typeWithModifier
		End Function

		Public Function [Is](ByVal other As TypeSymbol) As Boolean
			If (Not TypeSymbol.Equals(Me.Type, other, TypeCompareKind.ConsiderEverything)) Then
				Return False
			End If
			Return Me.CustomModifiers.IsEmpty
		End Function

		Friend Function IsSameType(ByVal other As TypeWithModifiers, ByVal compareKind As TypeCompareKind) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers::IsSameType(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers,Microsoft.CodeAnalysis.TypeCompareKind)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean IsSameType(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers,Microsoft.CodeAnalysis.TypeCompareKind)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Shared Operator =(ByVal x As TypeWithModifiers, ByVal y As TypeWithModifiers) As Boolean
			Return x.ExplicitEquals(y)
		End Operator

		Public Shared Operator <>(ByVal x As TypeWithModifiers, ByVal y As TypeWithModifiers) As Boolean
			Return Not x.ExplicitEquals(y)
		End Operator
	End Structure
End Namespace