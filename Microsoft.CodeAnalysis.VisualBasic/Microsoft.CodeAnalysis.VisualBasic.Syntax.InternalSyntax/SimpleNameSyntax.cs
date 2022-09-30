using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class SimpleNameSyntax : NameSyntax
	{
		internal readonly IdentifierTokenSyntax _identifier;

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal SimpleNameSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier)
			: base(kind)
		{
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
		}

		internal SimpleNameSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
		}

		internal SimpleNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax identifier)
			: base(kind, errors, annotations)
		{
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
		}

		internal SimpleNameSyntax(ObjectReader reader)
			: base(reader)
		{
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_identifier = identifierTokenSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_identifier);
		}
	}
}
