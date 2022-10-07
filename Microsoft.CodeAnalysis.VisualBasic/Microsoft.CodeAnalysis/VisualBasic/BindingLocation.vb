Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Enum BindingLocation
		None
		BaseTypes
		MethodSignature
		GenericConstraintsClause
		ProjectImportsDeclaration
		SourceFileImportsDeclaration
		Attribute
		EventSignature
		FieldType
		HandlesClause
		PropertySignature
		PropertyAccessorSignature
		EventAccessorSignature
	End Enum
End Namespace