using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ModifiedIdentifierSyntax : VisualBasicSyntaxNode
	{
		internal readonly IdentifierTokenSyntax _identifier;

		internal readonly PunctuationSyntax _nullable;

		internal readonly ArgumentListSyntax _arrayBounds;

		internal readonly GreenNode _arrayRankSpecifiers;

		internal static Func<ObjectReader, object> CreateInstance;

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal PunctuationSyntax Nullable => _nullable;

		internal ArgumentListSyntax ArrayBounds => _arrayBounds;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax> ArrayRankSpecifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArrayRankSpecifierSyntax>(_arrayRankSpecifiers);

		internal ModifiedIdentifierSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier, PunctuationSyntax nullable, ArgumentListSyntax arrayBounds, GreenNode arrayRankSpecifiers)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (nullable != null)
			{
				AdjustFlagsAndWidth(nullable);
				_nullable = nullable;
			}
			if (arrayBounds != null)
			{
				AdjustFlagsAndWidth(arrayBounds);
				_arrayBounds = arrayBounds;
			}
			if (arrayRankSpecifiers != null)
			{
				AdjustFlagsAndWidth(arrayRankSpecifiers);
				_arrayRankSpecifiers = arrayRankSpecifiers;
			}
		}

		internal ModifiedIdentifierSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier, PunctuationSyntax nullable, ArgumentListSyntax arrayBounds, GreenNode arrayRankSpecifiers, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (nullable != null)
			{
				AdjustFlagsAndWidth(nullable);
				_nullable = nullable;
			}
			if (arrayBounds != null)
			{
				AdjustFlagsAndWidth(arrayBounds);
				_arrayBounds = arrayBounds;
			}
			if (arrayRankSpecifiers != null)
			{
				AdjustFlagsAndWidth(arrayRankSpecifiers);
				_arrayRankSpecifiers = arrayRankSpecifiers;
			}
		}

		internal ModifiedIdentifierSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax identifier, PunctuationSyntax nullable, ArgumentListSyntax arrayBounds, GreenNode arrayRankSpecifiers)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (nullable != null)
			{
				AdjustFlagsAndWidth(nullable);
				_nullable = nullable;
			}
			if (arrayBounds != null)
			{
				AdjustFlagsAndWidth(arrayBounds);
				_arrayBounds = arrayBounds;
			}
			if (arrayRankSpecifiers != null)
			{
				AdjustFlagsAndWidth(arrayRankSpecifiers);
				_arrayRankSpecifiers = arrayRankSpecifiers;
			}
		}

		internal ModifiedIdentifierSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_identifier = identifierTokenSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_nullable = punctuationSyntax;
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
				_arrayRankSpecifiers = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_identifier);
			writer.WriteValue(_nullable);
			writer.WriteValue(_arrayBounds);
			writer.WriteValue(_arrayRankSpecifiers);
		}

		static ModifiedIdentifierSyntax()
		{
			CreateInstance = (ObjectReader o) => new ModifiedIdentifierSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ModifiedIdentifierSyntax), (ObjectReader r) => new ModifiedIdentifierSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _identifier, 
				1 => _nullable, 
				2 => _arrayBounds, 
				3 => _arrayRankSpecifiers, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ModifiedIdentifierSyntax(base.Kind, newErrors, GetAnnotations(), _identifier, _nullable, _arrayBounds, _arrayRankSpecifiers);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ModifiedIdentifierSyntax(base.Kind, GetDiagnostics(), annotations, _identifier, _nullable, _arrayBounds, _arrayRankSpecifiers);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitModifiedIdentifier(this);
		}
	}
}
