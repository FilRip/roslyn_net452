using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class ImplicitNamedTypeSymbol : SourceMemberContainerTypeSymbol
	{
		public override bool IsImplicitlyDeclared
		{
			get
			{
				if (!base.IsImplicitClass)
				{
					return base.IsScriptClass;
				}
				return true;
			}
		}

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		internal override bool IsComImport => false;

		internal override bool HasSpecialName => false;

		internal override bool IsWindowsRuntimeImport => false;

		internal override bool ShouldAddWinRTMembers => false;

		public override bool IsSerializable => false;

		internal override TypeLayout Layout => default(TypeLayout);

		internal bool HasStructLayoutAttribute => false;

		internal override CharSet MarshallingCharSet => base.DefaultMarshallingCharSet;

		internal override bool HasDeclarativeSecurity => false;

		internal override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal override bool HasCodeAnalysisEmbeddedAttribute => false;

		internal override bool HasVisualBasicEmbeddedAttribute => false;

		internal override TypeSymbol CoClassType => null;

		internal override bool IsExtensibleInterfaceNoUseSiteDiagnostics => false;

		internal ImplicitNamedTypeSymbol(MergedTypeDeclaration declaration, NamespaceOrTypeSymbol containingSymbol, SourceModuleSymbol containingModule)
			: base(declaration, containingSymbol, containingModule)
		{
		}

		internal override AttributeUsageInfo GetAttributeUsageInfo()
		{
			return AttributeUsageInfo.Null;
		}

		internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
		{
			return GetDeclaredBase(default(BasesBeingResolved));
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		internal override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			NamedTypeSymbol specialType = DeclaringCompilation.GetSpecialType(SpecialType.System_Object);
			UseSiteInfo<AssemblySymbol> useSiteInfo = specialType.GetUseSiteInfo();
			diagnostics.Add(useSiteInfo, base.Locations[0]);
			if (TypeKind != TypeKind.Submission)
			{
				return specialType;
			}
			return null;
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			return null;
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return ImmutableArray<string>.Empty;
		}

		protected override Location GetInheritsOrImplementsLocation(NamedTypeSymbol @base, bool getInherits)
		{
			return NoLocation.Singleton;
		}

		protected override void AddDeclaredNonTypeMembers(MembersAndInitializersBuilder membersBuilder, BindingDiagnosticBag diagnostics)
		{
			ImmutableArray<SyntaxReference>.Enumerator enumerator = base.SyntaxReferences.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxReference current = enumerator.Current;
				VisualBasicSyntaxNode visualBasicSyntax = VisualBasicExtensions.GetVisualBasicSyntax(current);
				Binder binder = BinderBuilder.CreateBinderForType(base.ContainingSourceModule, current.SyntaxTree, this);
				ArrayBuilder<FieldOrPropertyInitializer> staticInitializers = null;
				ArrayBuilder<FieldOrPropertyInitializer> instanceInitializers = null;
				bool isImplicitClass = base.IsImplicitClass;
				SyntaxList<StatementSyntax>.Enumerator enumerator2 = ((visualBasicSyntax.Kind() == SyntaxKind.CompilationUnit) ? ((CompilationUnitSyntax)visualBasicSyntax).Members : ((NamespaceBlockSyntax)visualBasicSyntax).Members).GetEnumerator();
				while (enumerator2.MoveNext())
				{
					StatementSyntax current2 = enumerator2.Current;
					bool reportAsInvalid = isImplicitClass && !current2.HasErrors;
					AddMember(current2, binder, diagnostics, membersBuilder, ref staticInitializers, ref instanceInitializers, reportAsInvalid);
				}
				ArrayBuilder<ImmutableArray<FieldOrPropertyInitializer>> allInitializers = membersBuilder.StaticInitializers;
				SourceMemberContainerTypeSymbol.AddInitializers(ref allInitializers, staticInitializers);
				membersBuilder.StaticInitializers = allInitializers;
				allInitializers = membersBuilder.InstanceInitializers;
				SourceMemberContainerTypeSymbol.AddInitializers(ref allInitializers, instanceInitializers);
				membersBuilder.InstanceInitializers = allInitializers;
			}
		}

		internal override IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides()
		{
			return SpecializedCollections.EmptyEnumerable<PropertySymbol>();
		}
	}
}
