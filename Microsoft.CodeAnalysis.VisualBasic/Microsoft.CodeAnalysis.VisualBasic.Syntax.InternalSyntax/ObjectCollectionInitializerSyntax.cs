using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ObjectCollectionInitializerSyntax : ObjectCreationInitializerSyntax
	{
		internal readonly KeywordSyntax _fromKeyword;

		internal readonly CollectionInitializerSyntax _initializer;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax FromKeyword => _fromKeyword;

		internal CollectionInitializerSyntax Initializer => _initializer;

		internal ObjectCollectionInitializerSyntax(SyntaxKind kind, KeywordSyntax fromKeyword, CollectionInitializerSyntax initializer)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(fromKeyword);
			_fromKeyword = fromKeyword;
			AdjustFlagsAndWidth(initializer);
			_initializer = initializer;
		}

		internal ObjectCollectionInitializerSyntax(SyntaxKind kind, KeywordSyntax fromKeyword, CollectionInitializerSyntax initializer, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(fromKeyword);
			_fromKeyword = fromKeyword;
			AdjustFlagsAndWidth(initializer);
			_initializer = initializer;
		}

		internal ObjectCollectionInitializerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax fromKeyword, CollectionInitializerSyntax initializer)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(fromKeyword);
			_fromKeyword = fromKeyword;
			AdjustFlagsAndWidth(initializer);
			_initializer = initializer;
		}

		internal ObjectCollectionInitializerSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_fromKeyword = keywordSyntax;
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
			writer.WriteValue(_fromKeyword);
			writer.WriteValue(_initializer);
		}

		static ObjectCollectionInitializerSyntax()
		{
			CreateInstance = (ObjectReader o) => new ObjectCollectionInitializerSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ObjectCollectionInitializerSyntax), (ObjectReader r) => new ObjectCollectionInitializerSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCollectionInitializerSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _fromKeyword, 
				1 => _initializer, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ObjectCollectionInitializerSyntax(base.Kind, newErrors, GetAnnotations(), _fromKeyword, _initializer);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ObjectCollectionInitializerSyntax(base.Kind, GetDiagnostics(), annotations, _fromKeyword, _initializer);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitObjectCollectionInitializer(this);
		}
	}
}
