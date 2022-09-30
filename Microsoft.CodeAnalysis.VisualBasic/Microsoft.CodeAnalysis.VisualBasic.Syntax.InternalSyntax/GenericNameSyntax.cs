using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class GenericNameSyntax : SimpleNameSyntax
	{
		internal readonly TypeArgumentListSyntax _typeArgumentList;

		internal static Func<ObjectReader, object> CreateInstance;

		internal TypeArgumentListSyntax TypeArgumentList => _typeArgumentList;

		internal GenericNameSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier, TypeArgumentListSyntax typeArgumentList)
			: base(kind, identifier)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(typeArgumentList);
			_typeArgumentList = typeArgumentList;
		}

		internal GenericNameSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier, TypeArgumentListSyntax typeArgumentList, ISyntaxFactoryContext context)
			: base(kind, identifier)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(typeArgumentList);
			_typeArgumentList = typeArgumentList;
		}

		internal GenericNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax identifier, TypeArgumentListSyntax typeArgumentList)
			: base(kind, errors, annotations, identifier)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(typeArgumentList);
			_typeArgumentList = typeArgumentList;
		}

		internal GenericNameSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			TypeArgumentListSyntax typeArgumentListSyntax = (TypeArgumentListSyntax)reader.ReadValue();
			if (typeArgumentListSyntax != null)
			{
				AdjustFlagsAndWidth(typeArgumentListSyntax);
				_typeArgumentList = typeArgumentListSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_typeArgumentList);
		}

		static GenericNameSyntax()
		{
			CreateInstance = (ObjectReader o) => new GenericNameSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(GenericNameSyntax), (ObjectReader r) => new GenericNameSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _identifier, 
				1 => _typeArgumentList, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new GenericNameSyntax(base.Kind, newErrors, GetAnnotations(), _identifier, _typeArgumentList);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new GenericNameSyntax(base.Kind, GetDiagnostics(), annotations, _identifier, _typeArgumentList);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitGenericName(this);
		}
	}
}
