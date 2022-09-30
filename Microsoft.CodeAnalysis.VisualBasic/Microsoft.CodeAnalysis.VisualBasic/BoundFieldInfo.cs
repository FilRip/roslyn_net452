using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundFieldInfo : BoundExpression
	{
		private readonly FieldSymbol _Field;

		public FieldSymbol Field => _Field;

		public BoundFieldInfo(SyntaxNode syntax, FieldSymbol field, TypeSymbol type, bool hasErrors)
			: base(BoundKind.FieldInfo, syntax, type, hasErrors)
		{
			_Field = field;
		}

		public BoundFieldInfo(SyntaxNode syntax, FieldSymbol field, TypeSymbol type)
			: base(BoundKind.FieldInfo, syntax, type)
		{
			_Field = field;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitFieldInfo(this);
		}

		public BoundFieldInfo Update(FieldSymbol field, TypeSymbol type)
		{
			if ((object)field != Field || (object)type != base.Type)
			{
				BoundFieldInfo boundFieldInfo = new BoundFieldInfo(base.Syntax, field, type, base.HasErrors);
				boundFieldInfo.CopyAttributes(this);
				return boundFieldInfo;
			}
			return this;
		}
	}
}
