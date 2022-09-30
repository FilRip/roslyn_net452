namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal enum BindingLocation
	{
		None,
		BaseTypes,
		MethodSignature,
		GenericConstraintsClause,
		ProjectImportsDeclaration,
		SourceFileImportsDeclaration,
		Attribute,
		EventSignature,
		FieldType,
		HandlesClause,
		PropertySignature,
		PropertyAccessorSignature,
		EventAccessorSignature
	}
}
