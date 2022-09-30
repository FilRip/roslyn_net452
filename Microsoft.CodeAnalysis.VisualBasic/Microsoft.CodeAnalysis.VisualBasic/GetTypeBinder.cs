using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class GetTypeBinder : Binder
	{
		private class OpenTypeVisitor : VisualBasicSyntaxVisitor
		{
			private Dictionary<GenericNameSyntax, bool> _allowedMap;

			private bool _seenConstructed;

			private bool _seenGeneric;

			public OpenTypeVisitor()
			{
				_allowedMap = null;
				_seenConstructed = false;
				_seenGeneric = false;
			}

			public static void Visit(ExpressionSyntax typeSyntax, out Dictionary<GenericNameSyntax, bool> allowedMap, [Out] bool isOpenType)
			{
				OpenTypeVisitor openTypeVisitor = new OpenTypeVisitor();
				openTypeVisitor.Visit(typeSyntax);
				allowedMap = openTypeVisitor._allowedMap;
				isOpenType = openTypeVisitor._seenGeneric && !openTypeVisitor._seenConstructed;
			}

			public override void VisitGenericName(GenericNameSyntax node)
			{
				_seenGeneric = true;
				SeparatedSyntaxList<TypeSyntax> arguments = node.TypeArgumentList.Arguments;
				if (SyntaxNodeExtensions.AllAreMissingIdentifierName(arguments))
				{
					if (_allowedMap == null)
					{
						_allowedMap = new Dictionary<GenericNameSyntax, bool>();
					}
					_allowedMap[node] = !_seenConstructed;
					return;
				}
				_seenConstructed = true;
				SeparatedSyntaxList<TypeSyntax>.Enumerator enumerator = arguments.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TypeSyntax current = enumerator.Current;
					Visit(current);
				}
			}

			public override void VisitQualifiedName(QualifiedNameSyntax node)
			{
				bool seenConstructed = _seenConstructed;
				Visit(node.Right);
				bool seenConstructed2 = _seenConstructed;
				Visit(node.Left);
				if (!seenConstructed && !seenConstructed2 && _seenConstructed)
				{
					Visit(node.Right);
				}
			}

			public override void VisitArrayType(ArrayTypeSyntax node)
			{
				_seenConstructed = true;
				Visit(node.ElementType);
			}

			public override void VisitNullableType(NullableTypeSyntax node)
			{
				_seenConstructed = true;
				Visit(node.ElementType);
			}
		}

		private readonly Dictionary<GenericNameSyntax, bool> _allowedMap;

		private readonly bool _isTypeExpressionOpen;

		internal bool IsTypeExpressionOpen => _isTypeExpressionOpen;

		internal GetTypeBinder(ExpressionSyntax typeExpression, Binder containingBinder)
			: base(containingBinder)
		{
			OpenTypeVisitor.Visit(typeExpression, out _allowedMap, _isTypeExpressionOpen);
		}

		public override bool IsUnboundTypeAllowed(GenericNameSyntax Syntax)
		{
			if (_allowedMap != null && _allowedMap.TryGetValue(Syntax, out var value))
			{
				return value;
			}
			return false;
		}
	}
}
