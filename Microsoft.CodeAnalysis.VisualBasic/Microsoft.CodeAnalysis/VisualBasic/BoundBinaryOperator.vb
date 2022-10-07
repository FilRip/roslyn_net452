Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundBinaryOperator
		Inherits BoundExpression
		Private ReadOnly _OperatorKind As BinaryOperatorKind

		Private ReadOnly _Left As BoundExpression

		Private ReadOnly _Right As BoundExpression

		Private ReadOnly _Checked As Boolean

		Private ReadOnly _ConstantValueOpt As ConstantValue

		Public ReadOnly Property Checked As Boolean
			Get
				Return Me._Checked
			End Get
		End Property

		Public Overrides ReadOnly Property ConstantValueOpt As ConstantValue
			Get
				Return Me._ConstantValueOpt
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				' 
				' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbol Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator::get_ExpressionSymbol()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbol get_ExpressionSymbol()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à ..(MethodBody , & ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:ligne 345
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Get
		End Property

		Public ReadOnly Property Left As BoundExpression
			Get
				Return Me._Left
			End Get
		End Property

		Public ReadOnly Property OperatorKind As BinaryOperatorKind
			Get
				Return Me._OperatorKind
			End Get
		End Property

		Public ReadOnly Property Right As BoundExpression
			Get
				Return Me._Right
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operatorKind As BinaryOperatorKind, ByVal left As BoundExpression, ByVal right As BoundExpression, ByVal checked As Boolean, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, operatorKind, left, right, checked, Nothing, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operatorKind As BinaryOperatorKind, ByVal left As BoundExpression, ByVal right As BoundExpression, ByVal checked As Boolean, ByVal constantValueOpt As ConstantValue, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.BinaryOperator, syntax, type, If(hasErrors OrElse left.NonNullAndHasErrors(), True, right.NonNullAndHasErrors()))
			Me._OperatorKind = operatorKind
			Me._Left = left
			Me._Right = right
			Me._Checked = checked
			Me._ConstantValueOpt = constantValueOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitBinaryOperator(Me)
		End Function

		Public Function Update(ByVal operatorKind As BinaryOperatorKind, ByVal left As BoundExpression, ByVal right As BoundExpression, ByVal checked As Boolean, ByVal constantValueOpt As ConstantValue, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator
			Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator
			If (operatorKind <> Me.OperatorKind OrElse left <> Me.Left OrElse right <> Me.Right OrElse checked <> Me.Checked OrElse CObj(constantValueOpt) <> CObj(Me.ConstantValueOpt) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundBinaryOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(MyBase.Syntax, operatorKind, left, right, checked, constantValueOpt, type, MyBase.HasErrors)
				boundBinaryOperator1.CopyAttributes(Me)
				boundBinaryOperator = boundBinaryOperator1
			Else
				boundBinaryOperator = Me
			End If
			Return boundBinaryOperator
		End Function
	End Class
End Namespace