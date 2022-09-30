using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class DeclarationTreeBuilder : VisualBasicSyntaxVisitor<SingleNamespaceOrTypeDeclaration>
	{
		private struct TypeBlockInfo
		{
			public readonly TypeBlockSyntax TypeBlockSyntax;

			public readonly SingleTypeDeclaration TypeDeclaration;

			public readonly ArrayBuilder<int> NestedTypes;

			public TypeBlockInfo(TypeBlockSyntax typeBlockSyntax)
				: this(typeBlockSyntax, null, null)
			{
			}

			private TypeBlockInfo(TypeBlockSyntax typeBlockSyntax, SingleTypeDeclaration declaration, ArrayBuilder<int> nestedTypes)
			{
				this = default(TypeBlockInfo);
				TypeBlockSyntax = typeBlockSyntax;
				TypeDeclaration = declaration;
				NestedTypes = nestedTypes;
			}

			public TypeBlockInfo WithNestedTypes(ArrayBuilder<int> nested)
			{
				return new TypeBlockInfo(TypeBlockSyntax, null, nested);
			}

			public TypeBlockInfo WithDeclaration(SingleTypeDeclaration declaration)
			{
				return new TypeBlockInfo(TypeBlockSyntax, declaration, NestedTypes);
			}
		}

		private readonly ImmutableArray<string> _rootNamespace;

		private readonly string _scriptClassName;

		private readonly bool _isSubmission;

		private readonly SyntaxTree _syntaxTree;

		private static readonly ObjectPool<ImmutableHashSet<string>.Builder> s_memberNameBuilderPool = new ObjectPool<ImmutableHashSet<string>.Builder>(() => ImmutableHashSet.CreateBuilder(CaseInsensitiveComparison.Comparer));

		public static RootSingleNamespaceDeclaration ForTree(SyntaxTree tree, ImmutableArray<string> rootNamespace, string scriptClassName, bool isSubmission)
		{
			return (RootSingleNamespaceDeclaration)new DeclarationTreeBuilder(tree, rootNamespace, scriptClassName, isSubmission).ForDeclaration(tree.GetRoot());
		}

		private DeclarationTreeBuilder(SyntaxTree syntaxTree, ImmutableArray<string> rootNamespace, string scriptClassName, bool isSubmission)
		{
			_syntaxTree = syntaxTree;
			_rootNamespace = rootNamespace;
			_scriptClassName = scriptClassName;
			_isSubmission = isSubmission;
		}

		private SingleNamespaceOrTypeDeclaration ForDeclaration(SyntaxNode node)
		{
			return Visit(node);
		}

		private ImmutableArray<SingleNamespaceOrTypeDeclaration> VisitNamespaceChildren(VisualBasicSyntaxNode node, SyntaxList<StatementSyntax> members)
		{
			SingleNamespaceOrTypeDeclaration implicitClass = null;
			ArrayBuilder<SingleNamespaceOrTypeDeclaration> arrayBuilder = VisitNamespaceChildren(node, members, out implicitClass);
			if (implicitClass != null)
			{
				arrayBuilder.Add(implicitClass);
			}
			return arrayBuilder.ToImmutableAndFree();
		}

		private ArrayBuilder<SingleNamespaceOrTypeDeclaration> VisitNamespaceChildren(VisualBasicSyntaxNode node, SyntaxList<StatementSyntax> members, out SingleNamespaceOrTypeDeclaration implicitClass)
		{
			ArrayBuilder<SingleNamespaceOrTypeDeclaration> instance = ArrayBuilder<SingleNamespaceOrTypeDeclaration>.GetInstance();
			ArrayBuilder<SingleTypeDeclaration> instance2 = ArrayBuilder<SingleTypeDeclaration>.GetInstance();
			bool flag = false;
			SyntaxList<StatementSyntax>.Enumerator enumerator = members.GetEnumerator();
			while (enumerator.MoveNext())
			{
				StatementSyntax current = enumerator.Current;
				SingleNamespaceOrTypeDeclaration singleNamespaceOrTypeDeclaration = Visit(current);
				if (singleNamespaceOrTypeDeclaration != null)
				{
					if (singleNamespaceOrTypeDeclaration.Kind == DeclarationKind.EventSyntheticDelegate)
					{
						instance2.Add((SingleTypeDeclaration)singleNamespaceOrTypeDeclaration);
						flag = true;
					}
					else
					{
						instance.Add(singleNamespaceOrTypeDeclaration);
					}
				}
				else if (!flag)
				{
					flag = current.Kind() != SyntaxKind.IncompleteMember && current.Kind() != SyntaxKind.EmptyStatement;
				}
			}
			if (flag)
			{
				SingleTypeDeclaration.TypeDeclarationFlags declFlags = SingleTypeDeclaration.TypeDeclarationFlags.None;
				ImmutableHashSet<string> nonTypeMemberNames = GetNonTypeMemberNames(members, ref declFlags);
				implicitClass = CreateImplicitClass(node, nonTypeMemberNames, instance2.ToImmutable(), declFlags);
			}
			else
			{
				implicitClass = null;
			}
			instance2.Free();
			return instance;
		}

		private static ImmutableArray<ReferenceDirective> GetReferenceDirectives(CompilationUnitSyntax compilationUnit)
		{
			IList<ReferenceDirectiveTriviaSyntax> referenceDirectives = compilationUnit.GetReferenceDirectives((ReferenceDirectiveTriviaSyntax d) => !d.File.ContainsDiagnostics && !string.IsNullOrEmpty(d.File.ValueText));
			if (referenceDirectives.Count == 0)
			{
				return ImmutableArray<ReferenceDirective>.Empty;
			}
			ArrayBuilder<ReferenceDirective> instance = ArrayBuilder<ReferenceDirective>.GetInstance(referenceDirectives.Count);
			foreach (ReferenceDirectiveTriviaSyntax item in referenceDirectives)
			{
				instance.Add(new ReferenceDirective(item.File.ValueText, new SourceLocation(item)));
			}
			return instance.ToImmutableAndFree();
		}

		private SingleNamespaceOrTypeDeclaration CreateImplicitClass(VisualBasicSyntaxNode parent, ImmutableHashSet<string> memberNames, ImmutableArray<SingleTypeDeclaration> children, SingleTypeDeclaration.TypeDeclarationFlags declFlags)
		{
			SyntaxReference reference = _syntaxTree.GetReference(parent);
			return new SingleTypeDeclaration(DeclarationKind.ImplicitClass, "<invalid-global-code>", 0, DeclarationModifiers.Friend | DeclarationModifiers.Partial | DeclarationModifiers.NotInheritable, declFlags, reference, reference.GetLocation(), memberNames, children);
		}

		private SingleNamespaceOrTypeDeclaration CreateScriptClass(VisualBasicSyntaxNode parent, ImmutableArray<SingleTypeDeclaration> children, ImmutableHashSet<string> memberNames, SingleTypeDeclaration.TypeDeclarationFlags declFlags)
		{
			SyntaxReference reference = _syntaxTree.GetReference(parent);
			string[] array = _scriptClassName.Split(new char[1] { '.' });
			SingleNamespaceOrTypeDeclaration singleNamespaceOrTypeDeclaration = new SingleTypeDeclaration(_isSubmission ? DeclarationKind.Submission : DeclarationKind.Script, array.Last(), 0, DeclarationModifiers.Friend | DeclarationModifiers.Partial | DeclarationModifiers.NotInheritable, declFlags, reference, reference.GetLocation(), memberNames, children);
			for (int i = array.Length - 2; i >= 0; i += -1)
			{
				singleNamespaceOrTypeDeclaration = new SingleNamespaceDeclaration(array[i], hasImports: false, reference, reference.GetLocation(), ImmutableArray.Create(singleNamespaceOrTypeDeclaration));
			}
			return singleNamespaceOrTypeDeclaration;
		}

		public override SingleNamespaceOrTypeDeclaration VisitCompilationUnit(CompilationUnitSyntax node)
		{
			ImmutableArray<SingleNamespaceOrTypeDeclaration> globalDeclarations = default(ImmutableArray<SingleNamespaceOrTypeDeclaration>);
			ImmutableArray<SingleNamespaceOrTypeDeclaration> nonGlobal = default(ImmutableArray<SingleNamespaceOrTypeDeclaration>);
			_syntaxTree.GetReference(node);
			SingleNamespaceOrTypeDeclaration implicitClass = null;
			ImmutableArray<SingleNamespaceOrTypeDeclaration> declarations;
			ImmutableArray<ReferenceDirective> referenceDirectives;
			if (_syntaxTree.Options.Kind != 0)
			{
				ArrayBuilder<SingleNamespaceOrTypeDeclaration> instance = ArrayBuilder<SingleNamespaceOrTypeDeclaration>.GetInstance();
				ArrayBuilder<SingleTypeDeclaration> instance2 = ArrayBuilder<SingleTypeDeclaration>.GetInstance();
				SyntaxList<StatementSyntax>.Enumerator enumerator = node.Members.GetEnumerator();
				while (enumerator.MoveNext())
				{
					StatementSyntax current = enumerator.Current;
					SingleNamespaceOrTypeDeclaration singleNamespaceOrTypeDeclaration = Visit(current);
					if (singleNamespaceOrTypeDeclaration != null)
					{
						if (singleNamespaceOrTypeDeclaration.Kind == DeclarationKind.Namespace)
						{
							instance.Add(singleNamespaceOrTypeDeclaration);
						}
						else
						{
							instance2.Add((SingleTypeDeclaration)singleNamespaceOrTypeDeclaration);
						}
					}
				}
				SingleTypeDeclaration.TypeDeclarationFlags declFlags = SingleTypeDeclaration.TypeDeclarationFlags.None;
				ImmutableHashSet<string> nonTypeMemberNames = GetNonTypeMemberNames(node.Members, ref declFlags);
				implicitClass = CreateScriptClass(node, instance2.ToImmutableAndFree(), nonTypeMemberNames, declFlags);
				declarations = instance.ToImmutableAndFree();
				referenceDirectives = GetReferenceDirectives(node);
			}
			else
			{
				declarations = VisitNamespaceChildren(node, node.Members, out implicitClass).ToImmutableAndFree();
				referenceDirectives = ImmutableArray<ReferenceDirective>.Empty;
			}
			FindGlobalDeclarations(declarations, implicitClass, ref globalDeclarations, ref nonGlobal);
			if (_rootNamespace.Length == 0)
			{
				return new RootSingleNamespaceDeclaration(hasImports: true, _syntaxTree.GetReference(node), globalDeclarations.Concat(nonGlobal), referenceDirectives, node.Attributes.Any());
			}
			SingleNamespaceDeclaration item = BuildRootNamespace(node, nonGlobal);
			ImmutableArray<SingleNamespaceOrTypeDeclaration> children = globalDeclarations.Add(item).OfType<SingleNamespaceOrTypeDeclaration>().AsImmutable();
			return new RootSingleNamespaceDeclaration(hasImports: true, _syntaxTree.GetReference(node), children, referenceDirectives, node.Attributes.Any());
		}

		private void FindGlobalDeclarations(ImmutableArray<SingleNamespaceOrTypeDeclaration> declarations, SingleNamespaceOrTypeDeclaration implicitClass, ref ImmutableArray<SingleNamespaceOrTypeDeclaration> globalDeclarations, ref ImmutableArray<SingleNamespaceOrTypeDeclaration> nonGlobal)
		{
			ArrayBuilder<SingleNamespaceOrTypeDeclaration> instance = ArrayBuilder<SingleNamespaceOrTypeDeclaration>.GetInstance();
			ArrayBuilder<SingleNamespaceOrTypeDeclaration> instance2 = ArrayBuilder<SingleNamespaceOrTypeDeclaration>.GetInstance();
			if (implicitClass != null)
			{
				instance2.Add(implicitClass);
			}
			ImmutableArray<SingleNamespaceOrTypeDeclaration>.Enumerator enumerator = declarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SingleNamespaceOrTypeDeclaration current = enumerator.Current;
				if (current is SingleNamespaceDeclaration singleNamespaceDeclaration && singleNamespaceDeclaration.IsGlobalNamespace)
				{
					instance.AddRange(singleNamespaceDeclaration.Children);
				}
				else
				{
					instance2.Add(current);
				}
			}
			globalDeclarations = instance.ToImmutableAndFree();
			nonGlobal = instance2.ToImmutableAndFree();
		}

		private string UnescapeIdentifier(string identifier)
		{
			if (EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(identifier[0]), "[", TextCompare: false) == 0)
			{
				return identifier.Substring(1, identifier.Length - 2);
			}
			return identifier;
		}

		private SingleNamespaceDeclaration BuildRootNamespace(CompilationUnitSyntax node, ImmutableArray<SingleNamespaceOrTypeDeclaration> children)
		{
			SingleNamespaceDeclaration singleNamespaceDeclaration = null;
			SyntaxReference syntaxReference = _syntaxTree.GetReference(node);
			Location nameLocation = syntaxReference.GetLocation();
			for (int i = _rootNamespace.Length - 1; i >= 0; i += -1)
			{
				singleNamespaceDeclaration = new SingleNamespaceDeclaration(UnescapeIdentifier(_rootNamespace[i]), hasImports: true, syntaxReference, nameLocation, children, isPartOfRootNamespace: true);
				syntaxReference = null;
				nameLocation = null;
				children = ImmutableArray.Create((SingleNamespaceOrTypeDeclaration)singleNamespaceDeclaration);
			}
			return singleNamespaceDeclaration;
		}

		public override SingleNamespaceOrTypeDeclaration VisitNamespaceBlock(NamespaceBlockSyntax nsBlockSyntax)
		{
			NamespaceStatementSyntax namespaceStatement = nsBlockSyntax.NamespaceStatement;
			ImmutableArray<SingleNamespaceOrTypeDeclaration> children = VisitNamespaceChildren(nsBlockSyntax, nsBlockSyntax.Members);
			NameSyntax nameSyntax = namespaceStatement.Name;
			while (nameSyntax is QualifiedNameSyntax)
			{
				QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax)nameSyntax;
				SingleNamespaceDeclaration singleNamespaceDeclaration = new SingleNamespaceDeclaration(qualifiedNameSyntax.Right.Identifier.ValueText, hasImports: true, _syntaxTree.GetReference(qualifiedNameSyntax), _syntaxTree.GetLocation(qualifiedNameSyntax.Right.Span), children);
				children = new SingleNamespaceDeclaration[1] { singleNamespaceDeclaration }.OfType<SingleNamespaceOrTypeDeclaration>().AsImmutable();
				nameSyntax = qualifiedNameSyntax.Left;
			}
			if (nameSyntax.Kind() == SyntaxKind.GlobalName)
			{
				if (nsBlockSyntax.Parent.Kind() == SyntaxKind.CompilationUnit)
				{
					return new GlobalNamespaceDeclaration(hasImports: true, _syntaxTree.GetReference(nameSyntax), _syntaxTree.GetLocation(nameSyntax.Span), children);
				}
				return new SingleNamespaceDeclaration("Global", hasImports: true, _syntaxTree.GetReference(nameSyntax), _syntaxTree.GetLocation(nameSyntax.Span), children);
			}
			return new SingleNamespaceDeclaration(((IdentifierNameSyntax)nameSyntax).Identifier.ValueText, hasImports: true, _syntaxTree.GetReference(nameSyntax), _syntaxTree.GetLocation(nameSyntax.Span), children);
		}

		private SingleNamespaceOrTypeDeclaration VisitTypeBlockNew(TypeBlockSyntax topTypeBlockSyntax)
		{
			ArrayBuilder<TypeBlockInfo> instance = ArrayBuilder<TypeBlockInfo>.GetInstance();
			instance.Add(new TypeBlockInfo(topTypeBlockSyntax));
			int i;
			for (i = 0; i < instance.Count; i++)
			{
				TypeBlockInfo typeBlockInfo = instance[i];
				SyntaxList<StatementSyntax> members = typeBlockInfo.TypeBlockSyntax.Members;
				if (members.Count <= 0)
				{
					continue;
				}
				ArrayBuilder<int> arrayBuilder = null;
				SyntaxList<StatementSyntax>.Enumerator enumerator = members.GetEnumerator();
				while (enumerator.MoveNext())
				{
					StatementSyntax current = enumerator.Current;
					SyntaxKind syntaxKind = current.Kind();
					if (syntaxKind - 50 <= (SyntaxKind)3)
					{
						if (arrayBuilder == null)
						{
							arrayBuilder = ArrayBuilder<int>.GetInstance();
						}
						arrayBuilder.Add(instance.Count);
						instance.Add(new TypeBlockInfo((TypeBlockSyntax)current));
					}
				}
				if (arrayBuilder != null)
				{
					instance[i] = typeBlockInfo.WithNestedTypes(arrayBuilder);
				}
			}
			ArrayBuilder<SingleTypeDeclaration> instance2 = ArrayBuilder<SingleTypeDeclaration>.GetInstance();
			while (i > 0)
			{
				i--;
				TypeBlockInfo typeBlockInfo2 = instance[i];
				ImmutableArray<SingleTypeDeclaration> children = ImmutableArray<SingleTypeDeclaration>.Empty;
				SyntaxList<StatementSyntax> members2 = typeBlockInfo2.TypeBlockSyntax.Members;
				if (members2.Count > 0)
				{
					instance2.Clear();
					SyntaxList<StatementSyntax>.Enumerator enumerator2 = members2.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						StatementSyntax current2 = enumerator2.Current;
						SyntaxKind syntaxKind2 = current2.Kind();
						if (syntaxKind2 - 50 > (SyntaxKind)3 && Visit(current2) is SingleTypeDeclaration item)
						{
							instance2.Add(item);
						}
					}
					ArrayBuilder<int> nestedTypes = typeBlockInfo2.NestedTypes;
					if (nestedTypes != null)
					{
						int num = nestedTypes.Count - 1;
						for (int j = 0; j <= num; j++)
						{
							instance2.Add(instance[nestedTypes[j]].TypeDeclaration);
						}
						nestedTypes.Free();
					}
					children = instance2.ToImmutable();
				}
				TypeBlockSyntax typeBlockSyntax = typeBlockInfo2.TypeBlockSyntax;
				TypeStatementSyntax blockStatement = typeBlockSyntax.BlockStatement;
				int arity = 0;
				SyntaxKind syntaxKind3 = typeBlockSyntax.Kind();
				if (syntaxKind3 - 51 <= SyntaxKind.EmptyStatement)
				{
					arity = GetArity(blockStatement.TypeParameterList);
				}
				SingleTypeDeclaration.TypeDeclarationFlags declFlags = (blockStatement.AttributeLists.Any() ? SingleTypeDeclaration.TypeDeclarationFlags.HasAnyAttributes : SingleTypeDeclaration.TypeDeclarationFlags.None);
				if (typeBlockSyntax.Inherits.Any())
				{
					declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.HasBaseDeclarations;
				}
				ImmutableHashSet<string> nonTypeMemberNames = GetNonTypeMemberNames(typeBlockSyntax.Members, ref declFlags);
				instance[i] = typeBlockInfo2.WithDeclaration(new SingleTypeDeclaration(GetKind(blockStatement.Kind()), blockStatement.Identifier.ValueText, arity, GetModifiers(blockStatement.Modifiers), declFlags, _syntaxTree.GetReference(typeBlockSyntax), _syntaxTree.GetLocation(typeBlockSyntax.BlockStatement.Identifier.Span), nonTypeMemberNames, children));
			}
			instance2.Free();
			SingleTypeDeclaration typeDeclaration = instance[0].TypeDeclaration;
			instance.Free();
			return typeDeclaration;
		}

		public override SingleNamespaceOrTypeDeclaration VisitModuleBlock(ModuleBlockSyntax moduleBlockSyntax)
		{
			return VisitTypeBlockNew(moduleBlockSyntax);
		}

		public override SingleNamespaceOrTypeDeclaration VisitClassBlock(ClassBlockSyntax classBlockSyntax)
		{
			return VisitTypeBlockNew(classBlockSyntax);
		}

		public override SingleNamespaceOrTypeDeclaration VisitStructureBlock(StructureBlockSyntax structureBlockSyntax)
		{
			return VisitTypeBlockNew(structureBlockSyntax);
		}

		public override SingleNamespaceOrTypeDeclaration VisitInterfaceBlock(InterfaceBlockSyntax interfaceBlockSyntax)
		{
			return VisitTypeBlockNew(interfaceBlockSyntax);
		}

		public override SingleNamespaceOrTypeDeclaration VisitEnumBlock(EnumBlockSyntax enumBlockSyntax)
		{
			EnumStatementSyntax enumStatement = enumBlockSyntax.EnumStatement;
			SingleTypeDeclaration.TypeDeclarationFlags declFlags = (enumStatement.AttributeLists.Any() ? SingleTypeDeclaration.TypeDeclarationFlags.HasAnyAttributes : SingleTypeDeclaration.TypeDeclarationFlags.None);
			if (enumStatement.UnderlyingType != null)
			{
				declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.HasBaseDeclarations;
			}
			ImmutableHashSet<string> memberNames = GetMemberNames(enumBlockSyntax, ref declFlags);
			return new SingleTypeDeclaration(GetKind(enumStatement.Kind()), enumStatement.Identifier.ValueText, 0, GetModifiers(enumStatement.Modifiers), declFlags, _syntaxTree.GetReference(enumBlockSyntax), _syntaxTree.GetLocation(enumBlockSyntax.EnumStatement.Identifier.Span), memberNames, VisitTypeChildren(enumBlockSyntax.Members));
		}

		private ImmutableArray<SingleTypeDeclaration> VisitTypeChildren(SyntaxList<StatementSyntax> members)
		{
			if (members.Count == 0)
			{
				return ImmutableArray<SingleTypeDeclaration>.Empty;
			}
			ArrayBuilder<SingleTypeDeclaration> instance = ArrayBuilder<SingleTypeDeclaration>.GetInstance();
			SyntaxList<StatementSyntax>.Enumerator enumerator = members.GetEnumerator();
			while (enumerator.MoveNext())
			{
				StatementSyntax current = enumerator.Current;
				if (Visit(current) is SingleTypeDeclaration item)
				{
					instance.Add(item);
				}
			}
			return instance.ToImmutableAndFree();
		}

		private static ImmutableHashSet<string> ToImmutableAndFree(ImmutableHashSet<string>.Builder builder)
		{
			ImmutableHashSet<string> result = builder.ToImmutable();
			builder.Clear();
			s_memberNameBuilderPool.Free(builder);
			return result;
		}

		private ImmutableHashSet<string> GetNonTypeMemberNames(SyntaxList<StatementSyntax> members, ref SingleTypeDeclaration.TypeDeclarationFlags declFlags)
		{
			bool flag = false;
			bool flag2 = false;
			ImmutableHashSet<string>.Builder builder = s_memberNameBuilderPool.Allocate();
			SyntaxList<StatementSyntax>.Enumerator enumerator = members.GetEnumerator();
			while (enumerator.MoveNext())
			{
				StatementSyntax current = enumerator.Current;
				switch (current.Kind())
				{
				case SyntaxKind.FieldDeclaration:
				{
					flag2 = true;
					FieldDeclarationSyntax obj2 = (FieldDeclarationSyntax)current;
					if (obj2.AttributeLists.Any())
					{
						flag = true;
					}
					SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator4 = obj2.Declarators.GetEnumerator();
					while (enumerator4.MoveNext())
					{
						SeparatedSyntaxList<ModifiedIdentifierSyntax>.Enumerator enumerator5 = enumerator4.Current.Names.GetEnumerator();
						while (enumerator5.MoveNext())
						{
							ModifiedIdentifierSyntax current2 = enumerator5.Current;
							builder.Add(current2.Identifier.ValueText);
						}
					}
					break;
				}
				case SyntaxKind.SubBlock:
				case SyntaxKind.FunctionBlock:
				case SyntaxKind.ConstructorBlock:
				case SyntaxKind.OperatorBlock:
				{
					flag2 = true;
					MethodBaseSyntax blockStatement = ((MethodBlockBaseSyntax)current).BlockStatement;
					if (blockStatement.AttributeLists.Any())
					{
						flag = true;
					}
					AddMemberNames(blockStatement, builder);
					break;
				}
				case SyntaxKind.PropertyBlock:
				{
					flag2 = true;
					PropertyBlockSyntax propertyBlockSyntax = (PropertyBlockSyntax)current;
					if (propertyBlockSyntax.PropertyStatement.AttributeLists.Any())
					{
						flag = true;
					}
					else
					{
						SyntaxList<AccessorBlockSyntax>.Enumerator enumerator2 = propertyBlockSyntax.Accessors.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.BlockStatement.AttributeLists.Any())
							{
								flag = true;
							}
						}
					}
					AddMemberNames(propertyBlockSyntax.PropertyStatement, builder);
					break;
				}
				case SyntaxKind.SubStatement:
				case SyntaxKind.FunctionStatement:
				case SyntaxKind.SubNewStatement:
				case SyntaxKind.DeclareSubStatement:
				case SyntaxKind.DeclareFunctionStatement:
				case SyntaxKind.OperatorStatement:
				case SyntaxKind.PropertyStatement:
				{
					flag2 = true;
					MethodBaseSyntax methodBaseSyntax = (MethodBaseSyntax)current;
					if (methodBaseSyntax.AttributeLists.Any())
					{
						flag = true;
					}
					AddMemberNames(methodBaseSyntax, builder);
					break;
				}
				case SyntaxKind.EventBlock:
				{
					flag2 = true;
					EventBlockSyntax eventBlockSyntax = (EventBlockSyntax)current;
					if (eventBlockSyntax.EventStatement.AttributeLists.Any())
					{
						flag = true;
					}
					else
					{
						SyntaxList<AccessorBlockSyntax>.Enumerator enumerator3 = eventBlockSyntax.Accessors.GetEnumerator();
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current.BlockStatement.AttributeLists.Any())
							{
								flag = true;
							}
						}
					}
					string valueText2 = eventBlockSyntax.EventStatement.Identifier.ValueText;
					builder.Add(valueText2);
					break;
				}
				case SyntaxKind.EventStatement:
				{
					flag2 = true;
					EventStatementSyntax obj = (EventStatementSyntax)current;
					if (obj.AttributeLists.Any())
					{
						flag = true;
					}
					string valueText = obj.Identifier.ValueText;
					builder.Add(valueText);
					break;
				}
				}
			}
			if (flag)
			{
				declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.AnyMemberHasAttributes;
			}
			if (flag2)
			{
				declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.HasAnyNontypeMembers;
			}
			return ToImmutableAndFree(builder);
		}

		private ImmutableHashSet<string> GetMemberNames(EnumBlockSyntax enumBlockSyntax, ref SingleTypeDeclaration.TypeDeclarationFlags declFlags)
		{
			if (enumBlockSyntax.Members.Count != 0)
			{
				declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.HasAnyNontypeMembers;
			}
			ImmutableHashSet<string>.Builder builder = s_memberNameBuilderPool.Allocate();
			bool flag = false;
			SyntaxList<StatementSyntax>.Enumerator enumerator = enumBlockSyntax.Members.GetEnumerator();
			while (enumerator.MoveNext())
			{
				StatementSyntax current = enumerator.Current;
				if (current.Kind() == SyntaxKind.EnumMemberDeclaration)
				{
					EnumMemberDeclarationSyntax enumMemberDeclarationSyntax = (EnumMemberDeclarationSyntax)current;
					builder.Add(enumMemberDeclarationSyntax.Identifier.ValueText);
					if (!flag && enumMemberDeclarationSyntax.AttributeLists.Any())
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.AnyMemberHasAttributes;
			}
			return ToImmutableAndFree(builder);
		}

		private void AddMemberNames(MethodBaseSyntax methodDecl, ImmutableHashSet<string>.Builder results)
		{
			string memberNameFromSyntax = SourceMethodSymbol.GetMemberNameFromSyntax(methodDecl);
			results.Add(memberNameFromSyntax);
		}

		public override SingleNamespaceOrTypeDeclaration VisitDelegateStatement(DelegateStatementSyntax node)
		{
			SingleTypeDeclaration.TypeDeclarationFlags typeDeclarationFlags = (node.AttributeLists.Any() ? SingleTypeDeclaration.TypeDeclarationFlags.HasAnyAttributes : SingleTypeDeclaration.TypeDeclarationFlags.None);
			typeDeclarationFlags |= SingleTypeDeclaration.TypeDeclarationFlags.HasAnyNontypeMembers;
			return new SingleTypeDeclaration(DeclarationKind.Delegate, node.Identifier.ValueText, GetArity(node.TypeParameterList), GetModifiers(node.Modifiers), typeDeclarationFlags, _syntaxTree.GetReference(node), _syntaxTree.GetLocation(node.Identifier.Span), ImmutableHashSet<string>.Empty, ImmutableArray<SingleTypeDeclaration>.Empty);
		}

		public override SingleNamespaceOrTypeDeclaration VisitEventStatement(EventStatementSyntax node)
		{
			if (node.AsClause != null || node.ImplementsClause != null)
			{
				return null;
			}
			SingleTypeDeclaration.TypeDeclarationFlags typeDeclarationFlags = (node.AttributeLists.Any() ? SingleTypeDeclaration.TypeDeclarationFlags.HasAnyAttributes : SingleTypeDeclaration.TypeDeclarationFlags.None);
			typeDeclarationFlags |= SingleTypeDeclaration.TypeDeclarationFlags.HasAnyNontypeMembers;
			return new SingleTypeDeclaration(DeclarationKind.EventSyntheticDelegate, node.Identifier.ValueText, 0, GetModifiers(node.Modifiers), typeDeclarationFlags, _syntaxTree.GetReference(node), _syntaxTree.GetLocation(node.Identifier.Span), ImmutableHashSet<string>.Empty, ImmutableArray<SingleTypeDeclaration>.Empty);
		}

		public static DeclarationKind GetKind(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.ClassStatement:
				return DeclarationKind.Class;
			case SyntaxKind.InterfaceStatement:
				return DeclarationKind.Interface;
			case SyntaxKind.StructureStatement:
				return DeclarationKind.Structure;
			case SyntaxKind.NamespaceStatement:
				return DeclarationKind.Namespace;
			case SyntaxKind.ModuleStatement:
				return DeclarationKind.Module;
			case SyntaxKind.EnumStatement:
				return DeclarationKind.Enum;
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
				return DeclarationKind.Delegate;
			default:
				throw ExceptionUtilities.UnexpectedValue(kind);
			}
		}

		public static int GetArity(TypeParameterListSyntax typeParamsSyntax)
		{
			return typeParamsSyntax?.Parameters.Count ?? 0;
		}

		private static DeclarationModifiers GetModifiers(SyntaxTokenList modifiers)
		{
			DeclarationModifiers declarationModifiers = DeclarationModifiers.None;
			SyntaxTokenList.Enumerator enumerator = modifiers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxToken current = enumerator.Current;
				DeclarationModifiers declarationModifiers2 = DeclarationModifiers.None;
				switch (VisualBasicExtensions.Kind(current))
				{
				case SyntaxKind.MustInheritKeyword:
					declarationModifiers2 = DeclarationModifiers.MustInherit;
					break;
				case SyntaxKind.NotInheritableKeyword:
					declarationModifiers2 = DeclarationModifiers.NotInheritable;
					break;
				case SyntaxKind.PartialKeyword:
					declarationModifiers2 = DeclarationModifiers.Partial;
					break;
				case SyntaxKind.ShadowsKeyword:
					declarationModifiers2 = DeclarationModifiers.Shadows;
					break;
				case SyntaxKind.PublicKeyword:
					declarationModifiers2 = DeclarationModifiers.Public;
					break;
				case SyntaxKind.ProtectedKeyword:
					declarationModifiers2 = DeclarationModifiers.Protected;
					break;
				case SyntaxKind.FriendKeyword:
					declarationModifiers2 = DeclarationModifiers.Friend;
					break;
				case SyntaxKind.PrivateKeyword:
					declarationModifiers2 = DeclarationModifiers.Private;
					break;
				case SyntaxKind.SharedKeyword:
					declarationModifiers2 = DeclarationModifiers.Shared;
					break;
				case SyntaxKind.ReadOnlyKeyword:
					declarationModifiers2 = DeclarationModifiers.ReadOnly;
					break;
				case SyntaxKind.WriteOnlyKeyword:
					declarationModifiers2 = DeclarationModifiers.WriteOnly;
					break;
				case SyntaxKind.OverridesKeyword:
					declarationModifiers2 = DeclarationModifiers.Overrides;
					break;
				case SyntaxKind.OverridableKeyword:
					declarationModifiers2 = DeclarationModifiers.Overridable;
					break;
				case SyntaxKind.MustOverrideKeyword:
					declarationModifiers2 = DeclarationModifiers.MustOverride;
					break;
				case SyntaxKind.NotOverridableKeyword:
					declarationModifiers2 = DeclarationModifiers.NotOverridable;
					break;
				case SyntaxKind.OverloadsKeyword:
					declarationModifiers2 = DeclarationModifiers.Overloads;
					break;
				case SyntaxKind.WithEventsKeyword:
					declarationModifiers2 = DeclarationModifiers.WithEvents;
					break;
				case SyntaxKind.DimKeyword:
					declarationModifiers2 = DeclarationModifiers.Dim;
					break;
				case SyntaxKind.ConstKeyword:
					declarationModifiers2 = DeclarationModifiers.Const;
					break;
				case SyntaxKind.DefaultKeyword:
					declarationModifiers2 = DeclarationModifiers.Default;
					break;
				case SyntaxKind.StaticKeyword:
					declarationModifiers2 = DeclarationModifiers.Static;
					break;
				case SyntaxKind.WideningKeyword:
					declarationModifiers2 = DeclarationModifiers.Widening;
					break;
				case SyntaxKind.NarrowingKeyword:
					declarationModifiers2 = DeclarationModifiers.Narrowing;
					break;
				case SyntaxKind.AsyncKeyword:
					declarationModifiers2 = DeclarationModifiers.Async;
					break;
				case SyntaxKind.IteratorKeyword:
					declarationModifiers2 = DeclarationModifiers.Iterator;
					break;
				default:
					if (!current.GetDiagnostics().Any((Diagnostic d) => d.Severity == DiagnosticSeverity.Error))
					{
						throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(current));
					}
					break;
				}
				declarationModifiers |= declarationModifiers2;
			}
			return declarationModifiers;
		}
	}
}
