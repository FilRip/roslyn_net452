Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundUnaryOperator
		Inherits BoundExpression
		Private ReadOnly _OperatorKind As UnaryOperatorKind

		Private ReadOnly _Operand As BoundExpression

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
				' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbol Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator::get_ExpressionSymbol()
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

		Public ReadOnly Property Operand As BoundExpression
			Get
				Return Me._Operand
			End Get
		End Property

		Public ReadOnly Property OperatorKind As UnaryOperatorKind
			Get
				Return Me._OperatorKind
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operatorKind As UnaryOperatorKind, ByVal operand As BoundExpression, ByVal checked As Boolean, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, operatorKind, operand, checked, Nothing, type, If(hasErrors, True, operand.HasErrors))
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operatorKind As UnaryOperatorKind, ByVal operand As BoundExpression, ByVal checked As Boolean, ByVal constantValueOpt As ConstantValue, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.UnaryOperator, syntax, type, If(hasErrors, True, operand.NonNullAndHasErrors()))
			Me._OperatorKind = operatorKind
			Me._Operand = operand
			Me._Checked = checked
			Me._ConstantValueOpt = constantValueOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitUnaryOperator(Me)
		End Function

		Public Function Update(ByVal operatorKind As UnaryOperatorKind, ByVal operand As BoundExpression, ByVal checked As Boolean, ByVal constantValueOpt As ConstantValue, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator
			Dim boundUnaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator
			If (operatorKind <> Me.OperatorKind OrElse operand <> Me.Operand OrElse checked <> Me.Checked OrElse CObj(constantValueOpt) <> CObj(Me.ConstantValueOpt) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundUnaryOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator(MyBase.Syntax, operatorKind, operand, checked, constantValueOpt, type, MyBase.HasErrors)
				boundUnaryOperator1.CopyAttributes(Me)
				boundUnaryOperator = boundUnaryOperator1
			Else
				boundUnaryOperator = Me
			End If
			Return boundUnaryOperator
		End Function
	End Class
End Namespace