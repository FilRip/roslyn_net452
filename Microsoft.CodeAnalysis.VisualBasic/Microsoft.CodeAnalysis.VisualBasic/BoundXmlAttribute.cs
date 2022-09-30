using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundXmlAttribute : BoundExpression
	{
		private readonly BoundExpression _Name;

		private readonly BoundExpression _Value;

		private readonly bool _MatchesImport;

		private readonly BoundExpression _ObjectCreation;

		public BoundExpression Name => _Name;

		public BoundExpression Value => _Value;

		public bool MatchesImport => _MatchesImport;

		public BoundExpression ObjectCreation => _ObjectCreation;

		public BoundXmlAttribute(SyntaxNode syntax, BoundExpression name, BoundExpression value, bool matchesImport, BoundExpression objectCreation, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.XmlAttribute, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(name) || BoundNodeExtensions.NonNullAndHasErrors(value) || BoundNodeExtensions.NonNullAndHasErrors(objectCreation))
		{
			_Name = name;
			_Value = value;
			_MatchesImport = matchesImport;
			_ObjectCreation = objectCreation;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitXmlAttribute(this);
		}

		public BoundXmlAttribute Update(BoundExpression name, BoundExpression value, bool matchesImport, BoundExpression objectCreation, TypeSymbol type)
		{
			if (name != Name || value != Value || matchesImport != MatchesImport || objectCreation != ObjectCreation || (object)type != base.Type)
			{
				BoundXmlAttribute boundXmlAttribute = new BoundXmlAttribute(base.Syntax, name, value, matchesImport, objectCreation, type, base.HasErrors);
				boundXmlAttribute.CopyAttributes(this);
				return boundXmlAttribute;
			}
			return this;
		}
	}
}
