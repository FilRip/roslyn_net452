using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class AnonymousObjectCreationExpressionSyntax : NewExpressionSyntax
	{
		internal readonly ObjectMemberInitializerSyntax _initializer;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ObjectMemberInitializerSyntax Initializer => _initializer;

		internal AnonymousObjectCreationExpressionSyntax(SyntaxKind kind, KeywordSyntax newKeyword, GreenNode attributeLists, ObjectMemberInitializerSyntax initializer)
			: base(kind, newKeyword, attributeLists)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(initializer);
			_initializer = initializer;
		}

		internal AnonymousObjectCreationExpressionSyntax(SyntaxKind kind, KeywordSyntax newKeyword, GreenNode attributeLists, ObjectMemberInitializerSyntax initializer, ISyntaxFactoryContext context)
			: base(kind, newKeyword, attributeLists)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(initializer);
			_initializer = initializer;
		}

		internal AnonymousObjectCreationExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax newKeyword, GreenNode attributeLists, ObjectMemberInitializerSyntax initializer)
			: base(kind, errors, annotations, newKeyword, attributeLists)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(initializer);
			_initializer = initializer;
		}

		internal AnonymousObjectCreationExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			ObjectMemberInitializerSyntax objectMemberInitializerSyntax = (ObjectMemberInitializerSyntax)reader.ReadValue();
			if (objectMemberInitializerSyntax != null)
			{
				AdjustFlagsAndWidth(objectMemberInitializerSyntax);
				_initializer = objectMemberInitializerSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_initializer);
		}

		static AnonymousObjectCreationExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new AnonymousObjectCreationExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(AnonymousObjectCreationExpressionSyntax), (ObjectReader r) => new AnonymousObjectCreationExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AnonymousObjectCreationExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _newKeyword, 
				1 => _attributeLists, 
				2 => _initializer, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new AnonymousObjectCreationExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _newKeyword, _attributeLists, _initializer);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new AnonymousObjectCreationExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _newKeyword, _attributeLists, _initializer);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitAnonymousObjectCreationExpression(this);
		}
	}
}
