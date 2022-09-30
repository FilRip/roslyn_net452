using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ArrayCreationExpressionSyntax : NewExpressionSyntax
	{
		internal readonly TypeSyntax _type;

		internal readonly ArgumentListSyntax _arrayBounds;

		internal readonly GreenNode _rankSpecifiers;

		internal readonly CollectionInitializerSyntax _initializer;

		internal static Func<ObjectReader, object> CreateInstance;

		internal TypeSyntax Type => _type;

		internal ArgumentListSyntax ArrayBounds => _arrayBounds;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> RankSpecifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax>(_rankSpecifiers);

		internal CollectionInitializerSyntax Initializer => _initializer;

		internal ArrayCreationExpressionSyntax(SyntaxKind kind, KeywordSyntax newKeyword, GreenNode attributeLists, TypeSyntax type, ArgumentListSyntax arrayBounds, GreenNode rankSpecifiers, CollectionInitializerSyntax initializer)
			: base(kind, newKeyword, attributeLists)
		{
			base._slotCount = 6;
			AdjustFlagsAndWidth(type);
			_type = type;
			if (arrayBounds != null)
			{
				AdjustFlagsAndWidth(arrayBounds);
				_arrayBounds = arrayBounds;
			}
			if (rankSpecifiers != null)
			{
				AdjustFlagsAndWidth(rankSpecifiers);
				_rankSpecifiers = rankSpecifiers;
			}
			AdjustFlagsAndWidth(initializer);
			_initializer = initializer;
		}

		internal ArrayCreationExpressionSyntax(SyntaxKind kind, KeywordSyntax newKeyword, GreenNode attributeLists, TypeSyntax type, ArgumentListSyntax arrayBounds, GreenNode rankSpecifiers, CollectionInitializerSyntax initializer, ISyntaxFactoryContext context)
			: base(kind, newKeyword, attributeLists)
		{
			base._slotCount = 6;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(type);
			_type = type;
			if (arrayBounds != null)
			{
				AdjustFlagsAndWidth(arrayBounds);
				_arrayBounds = arrayBounds;
			}
			if (rankSpecifiers != null)
			{
				AdjustFlagsAndWidth(rankSpecifiers);
				_rankSpecifiers = rankSpecifiers;
			}
			AdjustFlagsAndWidth(initializer);
			_initializer = initializer;
		}

		internal ArrayCreationExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax newKeyword, GreenNode attributeLists, TypeSyntax type, ArgumentListSyntax arrayBounds, GreenNode rankSpecifiers, CollectionInitializerSyntax initializer)
			: base(kind, errors, annotations, newKeyword, attributeLists)
		{
			base._slotCount = 6;
			AdjustFlagsAndWidth(type);
			_type = type;
			if (arrayBounds != null)
			{
				AdjustFlagsAndWidth(arrayBounds);
				_arrayBounds = arrayBounds;
			}
			if (rankSpecifiers != null)
			{
				AdjustFlagsAndWidth(rankSpecifiers);
				_rankSpecifiers = rankSpecifiers;
			}
			AdjustFlagsAndWidth(initializer);
			_initializer = initializer;
		}

		internal ArrayCreationExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 6;
			TypeSyntax typeSyntax = (TypeSyntax)reader.ReadValue();
			if (typeSyntax != null)
			{
				AdjustFlagsAndWidth(typeSyntax);
				_type = typeSyntax;
			}
			ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)reader.ReadValue();
			if (argumentListSyntax != null)
			{
				AdjustFlagsAndWidth(argumentListSyntax);
				_arrayBounds = argumentListSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_rankSpecifiers = greenNode;
			}
			CollectionInitializerSyntax collectionInitializerSyntax = (CollectionInitializerSyntax)reader.ReadValue();
			if (collectionInitializerSyntax != null)
			{
				AdjustFlagsAndWidth(collectionInitializerSyntax);
				_initializer = collectionInitializerSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_type);
			writer.WriteValue(_arrayBounds);
			writer.WriteValue(_rankSpecifiers);
			writer.WriteValue(_initializer);
		}

		static ArrayCreationExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new ArrayCreationExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ArrayCreationExpressionSyntax), (ObjectReader r) => new ArrayCreationExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _newKeyword, 
				1 => _attributeLists, 
				2 => _type, 
				3 => _arrayBounds, 
				4 => _rankSpecifiers, 
				5 => _initializer, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ArrayCreationExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _newKeyword, _attributeLists, _type, _arrayBounds, _rankSpecifiers, _initializer);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ArrayCreationExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _newKeyword, _attributeLists, _type, _arrayBounds, _rankSpecifiers, _initializer);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitArrayCreationExpression(this);
		}
	}
}
