using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundXmlDeclaration : BoundExpression
	{
		private readonly BoundExpression _Version;

		private readonly BoundExpression _Encoding;

		private readonly BoundExpression _Standalone;

		private readonly BoundExpression _ObjectCreation;

		public BoundExpression Version => _Version;

		public BoundExpression Encoding => _Encoding;

		public BoundExpression Standalone => _Standalone;

		public BoundExpression ObjectCreation => _ObjectCreation;

		public BoundXmlDeclaration(SyntaxNode syntax, BoundExpression version, BoundExpression encoding, BoundExpression standalone, BoundExpression objectCreation, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.XmlDeclaration, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(version) || BoundNodeExtensions.NonNullAndHasErrors(encoding) || BoundNodeExtensions.NonNullAndHasErrors(standalone) || BoundNodeExtensions.NonNullAndHasErrors(objectCreation))
		{
			_Version = version;
			_Encoding = encoding;
			_Standalone = standalone;
			_ObjectCreation = objectCreation;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitXmlDeclaration(this);
		}

		public BoundXmlDeclaration Update(BoundExpression version, BoundExpression encoding, BoundExpression standalone, BoundExpression objectCreation, TypeSymbol type)
		{
			if (version != Version || encoding != Encoding || standalone != Standalone || objectCreation != ObjectCreation || (object)type != base.Type)
			{
				BoundXmlDeclaration boundXmlDeclaration = new BoundXmlDeclaration(base.Syntax, version, encoding, standalone, objectCreation, type, base.HasErrors);
				boundXmlDeclaration.CopyAttributes(this);
				return boundXmlDeclaration;
			}
			return this;
		}
	}
}
