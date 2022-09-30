using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ObjectCreationExpressionSyntax : NewExpressionSyntax
	{
		internal readonly TypeSyntax _type;

		internal readonly ArgumentListSyntax _argumentList;

		internal readonly ObjectCreationInitializerSyntax _initializer;

		internal static Func<ObjectReader, object> CreateInstance;

		internal TypeSyntax Type => _type;

		internal ArgumentListSyntax ArgumentList => _argumentList;

		internal ObjectCreationInitializerSyntax Initializer => _initializer;

		internal ObjectCreationExpressionSyntax(SyntaxKind kind, KeywordSyntax newKeyword, GreenNode attributeLists, TypeSyntax type, ArgumentListSyntax argumentList, ObjectCreationInitializerSyntax initializer)
			: base(kind, newKeyword, attributeLists)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(type);
			_type = type;
			if (argumentList != null)
			{
				AdjustFlagsAndWidth(argumentList);
				_argumentList = argumentList;
			}
			if (initializer != null)
			{
				AdjustFlagsAndWidth(initializer);
				_initializer = initializer;
			}
		}

		internal ObjectCreationExpressionSyntax(SyntaxKind kind, KeywordSyntax newKeyword, GreenNode attributeLists, TypeSyntax type, ArgumentListSyntax argumentList, ObjectCreationInitializerSyntax initializer, ISyntaxFactoryContext context)
			: base(kind, newKeyword, attributeLists)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(type);
			_type = type;
			if (argumentList != null)
			{
				AdjustFlagsAndWidth(argumentList);
				_argumentList = argumentList;
			}
			if (initializer != null)
			{
				AdjustFlagsAndWidth(initializer);
				_initializer = initializer;
			}
		}

		internal ObjectCreationExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax newKeyword, GreenNode attributeLists, TypeSyntax type, ArgumentListSyntax argumentList, ObjectCreationInitializerSyntax initializer)
			: base(kind, errors, annotations, newKeyword, attributeLists)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(type);
			_type = type;
			if (argumentList != null)
			{
				AdjustFlagsAndWidth(argumentList);
				_argumentList = argumentList;
			}
			if (initializer != null)
			{
				AdjustFlagsAndWidth(initializer);
				_initializer = initializer;
			}
		}

		internal ObjectCreationExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
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
				_argumentList = argumentListSyntax;
			}
			ObjectCreationInitializerSyntax objectCreationInitializerSyntax = (ObjectCreationInitializerSyntax)reader.ReadValue();
			if (objectCreationInitializerSyntax != null)
			{
				AdjustFlagsAndWidth(objectCreationInitializerSyntax);
				_initializer = objectCreationInitializerSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_type);
			writer.WriteValue(_argumentList);
			writer.WriteValue(_initializer);
		}

		static ObjectCreationExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new ObjectCreationExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ObjectCreationExpressionSyntax), (ObjectReader r) => new ObjectCreationExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _newKeyword, 
				1 => _attributeLists, 
				2 => _type, 
				3 => _argumentList, 
				4 => _initializer, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ObjectCreationExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _newKeyword, _attributeLists, _type, _argumentList, _initializer);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ObjectCreationExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _newKeyword, _attributeLists, _type, _argumentList, _initializer);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitObjectCreationExpression(this);
		}
	}
}
