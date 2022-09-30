using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
	internal abstract class MissingMetadataTypeSymbol : InstanceErrorTypeSymbol
	{
		[DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
		internal class TopLevel : MissingMetadataTypeSymbol
		{
			private readonly string _namespaceName;

			private readonly ModuleSymbol _containingModule;

			private NamespaceSymbol _lazyContainingNamespace;

			private int _lazyTypeId;

			public string NamespaceName => _namespaceName;

			public override ModuleSymbol ContainingModule => _containingModule;

			public override AssemblySymbol ContainingAssembly => _containingModule.ContainingAssembly;

			public override Symbol ContainingSymbol
			{
				get
				{
					if ((object)_lazyContainingNamespace == null)
					{
						NamespaceSymbol namespaceSymbol = _containingModule.GlobalNamespace;
						if (_namespaceName.Length > 0)
						{
							ImmutableArray<string> immutableArray = MetadataHelpers.SplitQualifiedName(_namespaceName);
							int num = immutableArray.Length - 1;
							int i;
							for (i = 0; i <= num; i++)
							{
								NamespaceSymbol namespaceSymbol2 = null;
								string text = immutableArray[i];
								ImmutableArray<Symbol>.Enumerator enumerator = namespaceSymbol.GetMembers(text).GetEnumerator();
								while (enumerator.MoveNext())
								{
									NamespaceOrTypeSymbol namespaceOrTypeSymbol = (NamespaceOrTypeSymbol)enumerator.Current;
									if (namespaceOrTypeSymbol.Kind == SymbolKind.Namespace && string.Equals(namespaceOrTypeSymbol.Name, text, StringComparison.Ordinal))
									{
										namespaceSymbol2 = (NamespaceSymbol)namespaceOrTypeSymbol;
										break;
									}
								}
								if ((object)namespaceSymbol2 == null)
								{
									break;
								}
								namespaceSymbol = namespaceSymbol2;
							}
							for (; i < immutableArray.Length; i++)
							{
								namespaceSymbol = new MissingNamespaceSymbol(namespaceSymbol, immutableArray[i]);
							}
						}
						Interlocked.CompareExchange(ref _lazyContainingNamespace, namespaceSymbol, null);
					}
					return _lazyContainingNamespace;
				}
			}

			public override SpecialType SpecialType
			{
				get
				{
					if (_lazyTypeId == -1)
					{
						SpecialType value = SpecialType.None;
						AssemblySymbol containingAssembly = _containingModule.ContainingAssembly;
						if ((base.Arity == 0 || MangleName) && (object)containingAssembly != null && (object)containingAssembly == containingAssembly.CorLibrary && _containingModule.Ordinal == 0)
						{
							value = SpecialTypes.GetTypeFromMetadataName(MetadataHelpers.BuildQualifiedName(_namespaceName, MetadataName));
						}
						Interlocked.CompareExchange(ref _lazyTypeId, (int)value, -1);
					}
					return (SpecialType)_lazyTypeId;
				}
			}

			public TopLevel(ModuleSymbol module, string @namespace, string name, int arity, bool mangleName)
				: base(name, arity, mangleName)
			{
				_lazyTypeId = -1;
				_namespaceName = @namespace;
				_containingModule = module;
			}

			public TopLevel(ModuleSymbol module, ref MetadataTypeName fullname, SpecialType typeId = (SpecialType)(-1))
				: this(module, ref fullname, fullname.ForcedArity == -1 || fullname.ForcedArity == fullname.InferredArity)
			{
				_lazyTypeId = (int)typeId;
			}

			private TopLevel(ModuleSymbol module, ref MetadataTypeName fullname, bool mangleName)
				: this(module, fullname.NamespaceName, mangleName ? fullname.UnmangledTypeName : fullname.TypeName, mangleName ? fullname.InferredArity : fullname.ForcedArity, mangleName)
			{
			}

			public sealed override int GetHashCode()
			{
				return Hash.Combine(_containingModule, Hash.Combine(MetadataName, Hash.Combine(_namespaceName, base.Arity)));
			}

			protected sealed override bool SpecializedEquals(InstanceErrorTypeSymbol obj)
			{
				if (obj is TopLevel topLevel && string.Equals(MetadataName, topLevel.MetadataName, StringComparison.Ordinal) && base.Arity == topLevel.Arity && string.Equals(_namespaceName, topLevel._namespaceName, StringComparison.Ordinal))
				{
					return _containingModule.Equals(topLevel._containingModule);
				}
				return false;
			}

			internal override string GetEmittedNamespaceName()
			{
				return _namespaceName;
			}

			private string GetDebuggerDisplay()
			{
				string text = MetadataHelpers.BuildQualifiedName(_namespaceName, m_Name);
				if (_arity > 0)
				{
					text = text + "(Of " + new string(',', _arity - 1) + ")";
				}
				return text + "[missing]";
			}
		}

		internal class TopLevelWithCustomErrorInfo : TopLevel
		{
			private readonly DiagnosticInfo _errorInfo;

			internal override DiagnosticInfo ErrorInfo => _errorInfo;

			public TopLevelWithCustomErrorInfo(ModuleSymbol moduleSymbol, ref MetadataTypeName emittedName, DiagnosticInfo errorInfo, SpecialType typeId = (SpecialType)(-1))
				: base(moduleSymbol, ref emittedName, typeId)
			{
				_errorInfo = errorInfo;
			}

			public TopLevelWithCustomErrorInfo(ModuleSymbol moduleSymbol, ref MetadataTypeName emittedName, Func<TopLevelWithCustomErrorInfo, DiagnosticInfo> delayedErrorInfo)
				: base(moduleSymbol, ref emittedName)
			{
				_errorInfo = delayedErrorInfo(this);
			}
		}

		internal sealed class Nested : MissingMetadataTypeSymbol
		{
			private readonly NamedTypeSymbol _containingType;

			public override Symbol ContainingSymbol => _containingType;

			public override SpecialType SpecialType => SpecialType.None;

			public Nested(NamedTypeSymbol containingType, string name, int arity, bool mangleName)
				: base(name, arity, mangleName)
			{
				_containingType = containingType;
			}

			public Nested(NamedTypeSymbol containingType, ref MetadataTypeName emittedName)
				: this(containingType, ref emittedName, emittedName.ForcedArity == -1 || emittedName.ForcedArity == emittedName.InferredArity)
			{
			}

			private Nested(NamedTypeSymbol containingType, ref MetadataTypeName emittedName, bool mangleName)
				: this(containingType, mangleName ? emittedName.UnmangledTypeName : emittedName.TypeName, mangleName ? emittedName.InferredArity : emittedName.ForcedArity, mangleName)
			{
			}

			public override int GetHashCode()
			{
				return Hash.Combine(_containingType, Hash.Combine(MetadataName, base.Arity));
			}

			protected override bool SpecializedEquals(InstanceErrorTypeSymbol obj)
			{
				if (obj is Nested nested && string.Equals(MetadataName, nested.MetadataName, StringComparison.Ordinal) && base.Arity == nested.Arity)
				{
					return _containingType.Equals(nested._containingType);
				}
				return false;
			}

			private string GetDebuggerDisplay()
			{
				string text = _containingType.ToString() + "." + Name;
				if (_arity > 0)
				{
					text = text + "(Of " + new string(',', _arity - 1) + ")";
				}
				return text + "[missing]";
			}
		}

		protected readonly string m_Name;

		protected readonly bool m_MangleName;

		public override string Name => m_Name;

		internal override bool MangleName => m_MangleName;

		internal override DiagnosticInfo ErrorInfo
		{
			get
			{
				AssemblySymbol containingAssembly = ContainingAssembly;
				if (containingAssembly.IsMissing)
				{
					object objectValue = RuntimeHelpers.GetObjectValue((SpecialType != 0) ? ((IFormattable)CustomSymbolDisplayFormatter.DefaultErrorFormat(this)) : ((IFormattable)this));
					return ErrorFactory.ErrorInfo(ERRID.ERR_UnreferencedAssembly3, containingAssembly.Identity, objectValue);
				}
				ModuleSymbol containingModule = ContainingModule;
				if (containingModule.IsMissing)
				{
					return ErrorFactory.ErrorInfo(ERRID.ERR_UnreferencedModule3, containingModule.Name, this);
				}
				return ErrorFactory.ErrorInfo(ERRID.ERR_TypeRefResolutionError3, this, containingModule.Name);
			}
		}

		private MissingMetadataTypeSymbol(string name, int arity, bool mangleName)
			: base(arity)
		{
			m_Name = name;
			m_MangleName = mangleName && arity > 0;
		}
	}
}
