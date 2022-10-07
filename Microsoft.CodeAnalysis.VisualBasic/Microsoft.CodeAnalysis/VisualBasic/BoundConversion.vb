Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundConversion
		Inherits BoundConversionOrCast
		Private ReadOnly _Operand As BoundExpression

		Private ReadOnly _ConversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind

		Private ReadOnly _Checked As Boolean

		Private ReadOnly _ExplicitCastInCode As Boolean

		Private ReadOnly _ConstantValueOpt As ConstantValue

		Private ReadOnly _ExtendedInfoOpt As BoundExtendedConversionInfo

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

		Public Overrides ReadOnly Property ConversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Get
				Return Me._ConversionKind
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitCastInCode As Boolean
			Get
				Return Me._ExplicitCastInCode
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				' 
				' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbol Microsoft.CodeAnalysis.VisualBasic.BoundConversion::get_ExpressionSymbol()
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

		Public ReadOnly Property ExtendedInfoOpt As BoundExtendedConversionInfo
			Get
				Return Me._ExtendedInfoOpt
			End Get
		End Property

		Public Overrides ReadOnly Property Operand As BoundExpression
			Get
				Return Me._Operand
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operand As BoundExpression, ByVal conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind, ByVal checked As Boolean, ByVal explicitCastInCode As Boolean, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, operand, conversionKind, checked, explicitCastInCode, Nothing, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operand As BoundExpression, ByVal conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind, ByVal checked As Boolean, ByVal explicitCastInCode As Boolean, ByVal constantValueOpt As ConstantValue, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, operand, conversionKind, checked, explicitCastInCode, constantValueOpt, Nothing, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal operand As BoundExpression, ByVal conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind, ByVal checked As Boolean, ByVal explicitCastInCode As Boolean, ByVal constantValueOpt As ConstantValue, ByVal extendedInfoOpt As BoundExtendedConversionInfo, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.Conversion, syntax, type, If(hasErrors OrElse operand.NonNullAndHasErrors(), True, extendedInfoOpt.NonNullAndHasErrors()))
			Me._Operand = operand
			Me._ConversionKind = conversionKind
			Me._Checked = checked
			Me._ExplicitCastInCode = explicitCastInCode
			Me._ConstantValueOpt = constantValueOpt
			Me._ExtendedInfoOpt = extendedInfoOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitConversion(Me)
		End Function

		Public Function Update(ByVal operand As BoundExpression, ByVal conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind, ByVal checked As Boolean, ByVal explicitCastInCode As Boolean, ByVal constantValueOpt As ConstantValue, ByVal extendedInfoOpt As BoundExtendedConversionInfo, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundConversion
			Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion
			If (operand <> Me.Operand OrElse conversionKind <> Me.ConversionKind OrElse checked <> Me.Checked OrElse explicitCastInCode <> Me.ExplicitCastInCode OrElse CObj(constantValueOpt) <> CObj(Me.ConstantValueOpt) OrElse extendedInfoOpt <> Me.ExtendedInfoOpt OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundConversion1 As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = New Microsoft.CodeAnalysis.VisualBasic.BoundConversion(MyBase.Syntax, operand, conversionKind, checked, explicitCastInCode, constantValueOpt, extendedInfoOpt, type, MyBase.HasErrors)
				boundConversion1.CopyAttributes(Me)
				boundConversion = boundConversion1
			Else
				boundConversion = Me
			End If
			Return boundConversion
		End Function
	End Class
End Namespace