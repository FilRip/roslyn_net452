using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.InternalUtilities;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.RuntimeMembers;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public sealed class VisualBasicCompilation : Compilation
	{
		internal class DocumentationCommentCompiler : VisualBasicSymbolVisitor
		{
			[Flags]
			internal enum WellKnownTag
			{
				None = 0,
				C = 1,
				Code = 2,
				Example = 4,
				Exception = 8,
				Include = 0x10,
				List = 0x20,
				Para = 0x40,
				Param = 0x80,
				ParamRef = 0x100,
				Permission = 0x200,
				Remarks = 0x400,
				Returns = 0x800,
				See = 0x1000,
				SeeAlso = 0x2000,
				Summary = 0x4000,
				TypeParam = 0x8000,
				TypeParamRef = 0x10000,
				Value = 0x20000,
				AllCollectable = 0x3CF98
			}

			private struct XmlNodeWithAttributes : IComparable<XmlNodeWithAttributes>
			{
				public readonly XmlNodeSyntax Node;

				public readonly SortedDictionary<string, string> Attributes;

				public XmlNodeWithAttributes(XmlNodeSyntax node)
				{
					this = default(XmlNodeWithAttributes);
					Node = node;
					Attributes = GetElementAttributes(node);
				}

				public static int CompareAttributes(SortedDictionary<string, string> a, SortedDictionary<string, string> b)
				{
					int count = a.Count;
					int num = count.CompareTo(b.Count);
					if (num != 0)
					{
						return num;
					}
					if (count > 0)
					{
						SortedDictionary<string, string>.Enumerator enumerator = a.GetEnumerator();
						SortedDictionary<string, string>.Enumerator enumerator2 = b.GetEnumerator();
						while (enumerator.MoveNext() && enumerator2.MoveNext())
						{
							num = enumerator.Current.Key.CompareTo(enumerator2.Current.Key);
							if (num != 0)
							{
								return num;
							}
							num = enumerator.Current.Value.CompareTo(enumerator2.Current.Value);
							if (num != 0)
							{
								return num;
							}
						}
					}
					return 0;
				}

				public int CompareTo(XmlNodeWithAttributes other)
				{
					int num = CompareAttributes(Attributes, other.Attributes);
					if (num != 0)
					{
						return num;
					}
					return (Node.SpanStart > other.Node.SpanStart) ? 1 : (-1);
				}

				int IComparable<XmlNodeWithAttributes>.CompareTo(XmlNodeWithAttributes other)
				{
					//ILSpy generated this explicit interface implementation from .override directive in CompareTo
					return this.CompareTo(other);
				}
			}

			private class IncludeElementExpander
			{
				private struct WellKnownTagsSupport
				{
					public readonly bool ExceptionSupported;

					public readonly bool ReturnsSupported;

					public readonly bool ParamAndParamRefSupported;

					public readonly bool ValueSupported;

					public readonly bool TypeParamSupported;

					public readonly bool TypeParamRefSupported;

					public readonly bool IsDeclareMethod;

					public readonly bool IsWriteOnlyProperty;

					public readonly string SymbolName;

					public WellKnownTagsSupport(Symbol symbol)
					{
						this = default(WellKnownTagsSupport);
						ExceptionSupported = false;
						ReturnsSupported = false;
						ParamAndParamRefSupported = false;
						ValueSupported = false;
						TypeParamSupported = false;
						TypeParamRefSupported = false;
						IsDeclareMethod = false;
						IsWriteOnlyProperty = false;
						SymbolName = GetSymbolName(symbol);
						switch (symbol.Kind)
						{
						case SymbolKind.Field:
							TypeParamRefSupported = true;
							break;
						case SymbolKind.Event:
							ExceptionSupported = true;
							ParamAndParamRefSupported = true;
							TypeParamRefSupported = true;
							break;
						case SymbolKind.Method:
						{
							MethodSymbol methodSymbol = (MethodSymbol)symbol;
							IsDeclareMethod = methodSymbol.MethodKind == MethodKind.DeclareMethod;
							ExceptionSupported = true;
							ParamAndParamRefSupported = true;
							TypeParamSupported = !IsDeclareMethod && methodSymbol.MethodKind != MethodKind.UserDefinedOperator;
							TypeParamRefSupported = true;
							if (!methodSymbol.IsSub)
							{
								ReturnsSupported = true;
							}
							break;
						}
						case SymbolKind.NamedType:
						{
							NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)symbol;
							MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
							if (namedTypeSymbol.TypeKind == TypeKind.Delegate)
							{
								if ((object)delegateInvokeMethod != null && !delegateInvokeMethod.IsSub)
								{
									ReturnsSupported = true;
								}
								else
								{
									SymbolName = "delegate sub";
								}
							}
							ParamAndParamRefSupported = namedTypeSymbol.TypeKind == TypeKind.Delegate;
							TypeParamSupported = namedTypeSymbol.TypeKind != TypeKind.Enum && namedTypeSymbol.TypeKind != TypeKind.Module;
							TypeParamRefSupported = namedTypeSymbol.TypeKind != TypeKind.Module;
							break;
						}
						case SymbolKind.Property:
						{
							PropertySymbol propertySymbol = (PropertySymbol)symbol;
							ExceptionSupported = true;
							ParamAndParamRefSupported = true;
							TypeParamRefSupported = true;
							ValueSupported = true;
							IsWriteOnlyProperty = propertySymbol.IsWriteOnly;
							ReturnsSupported = !IsWriteOnlyProperty;
							break;
						}
						default:
							throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
						}
					}
				}

				private readonly Symbol _symbol;

				private readonly WellKnownTagsSupport _tagsSupport;

				private readonly ArrayBuilder<XmlNodeSyntax> _sourceIncludeElementNodes;

				private readonly VisualBasicCompilation _compilation;

				private readonly SyntaxTree _tree;

				private readonly SyntaxTree _onlyDiagnosticsFromTree;

				private readonly TextSpan? _filterSpanWithinTree;

				private readonly BindingDiagnosticBag _diagnostics;

				private readonly CancellationToken _cancellationToken;

				private Dictionary<DocumentationCommentBinder.BinderType, Binder> _binders;

				private int _nextSourceIncludeElementIndex;

				private HashSet<Location> _inProgressIncludeElementNodes;

				private DocumentationCommentIncludeCache _includedFileCache;

				private bool ProduceDiagnostics => SyntaxExtensions.ReportDocumentationCommentDiagnostics(_tree);

				private bool ProduceXmlDiagnostics
				{
					get
					{
						if (SyntaxExtensions.ReportDocumentationCommentDiagnostics(_tree))
						{
							return _onlyDiagnosticsFromTree == null;
						}
						return false;
					}
				}

				private SourceModuleSymbol Module => (SourceModuleSymbol)_compilation.SourceModule;

				private IncludeElementExpander(Symbol symbol, ArrayBuilder<XmlNodeSyntax> sourceIncludeElementNodes, VisualBasicCompilation compilation, DocumentationCommentIncludeCache includedFileCache, SyntaxTree onlyDiagnosticsFromTree, TextSpan? filterSpanWithinTree, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken)
				{
					_binders = null;
					_symbol = symbol;
					_tagsSupport = new WellKnownTagsSupport(symbol);
					_sourceIncludeElementNodes = sourceIncludeElementNodes;
					_compilation = compilation;
					_onlyDiagnosticsFromTree = onlyDiagnosticsFromTree;
					_filterSpanWithinTree = filterSpanWithinTree;
					_diagnostics = diagnostics;
					_cancellationToken = cancellationToken;
					_tree = ((sourceIncludeElementNodes == null || sourceIncludeElementNodes.Count == 0) ? null : sourceIncludeElementNodes[0].SyntaxTree);
					_includedFileCache = includedFileCache;
					_nextSourceIncludeElementIndex = 0;
				}

				internal static string ProcessIncludes(string unprocessed, Symbol memberSymbol, ArrayBuilder<XmlNodeSyntax> sourceIncludeElementNodes, VisualBasicCompilation compilation, SyntaxTree onlyDiagnosticsFromTree, TextSpan? filterSpanWithinTree, ref DocumentationCommentIncludeCache includedFileCache, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken)
				{
					if (sourceIncludeElementNodes == null)
					{
						return unprocessed;
					}
					XDocument node;
					try
					{
						node = XDocument.Parse(unprocessed, LoadOptions.PreserveWhitespace);
					}
					catch (XmlException ex)
					{
						ProjectData.SetProjectError(ex);
						XmlException ex2 = ex;
						string result = unprocessed;
						ProjectData.ClearProjectError();
						return result;
					}
					PooledStringBuilder instance = PooledStringBuilder.GetInstance();
					using (StringWriter stringWriter = new StringWriter(instance.Builder, CultureInfo.InvariantCulture))
					{
						cancellationToken.ThrowIfCancellationRequested();
						IncludeElementExpander includeElementExpander = new IncludeElementExpander(memberSymbol, sourceIncludeElementNodes, compilation, includedFileCache, onlyDiagnosticsFromTree, filterSpanWithinTree, diagnostics, cancellationToken);
						XNode[] array = includeElementExpander.Rewrite(node, null, null);
						foreach (XNode value in array)
						{
							cancellationToken.ThrowIfCancellationRequested();
							stringWriter.Write(value);
						}
						includedFileCache = includeElementExpander._includedFileCache;
					}
					return instance.ToStringAndFree();
				}

				private Binder GetOrCreateBinder(DocumentationCommentBinder.BinderType type)
				{
					if (_binders == null)
					{
						_binders = new Dictionary<DocumentationCommentBinder.BinderType, Binder>();
					}
					Binder value = null;
					if (!_binders.TryGetValue(type, out value))
					{
						value = CreateDocumentationCommentBinderForSymbol(Module, _symbol, _tree, type);
						_binders.Add(type, value);
					}
					return value;
				}

				private XNode[] RewriteMany(XNode[] nodes, string currentXmlFilePath, XmlNodeSyntax originatingSyntax)
				{
					ArrayBuilder<XNode> arrayBuilder = null;
					foreach (XNode node in nodes)
					{
						if (arrayBuilder == null)
						{
							arrayBuilder = ArrayBuilder<XNode>.GetInstance();
						}
						arrayBuilder.AddRange(Rewrite(node, currentXmlFilePath, originatingSyntax));
					}
					if (arrayBuilder != null)
					{
						return arrayBuilder.ToArrayAndFree();
					}
					return Array.Empty<XNode>();
				}

				private XNode[] Rewrite(XNode node, string currentXmlFilePath, XmlNodeSyntax originatingSyntax)
				{
					CancellationToken cancellationToken = _cancellationToken;
					cancellationToken.ThrowIfCancellationRequested();
					string commentMessage = null;
					if (node.NodeType == XmlNodeType.Element)
					{
						XElement xElement = (XElement)node;
						if (ElementNameIs(xElement, "include"))
						{
							XNode[] array = RewriteIncludeElement(xElement, currentXmlFilePath, originatingSyntax, out commentMessage);
							if (array != null)
							{
								return array;
							}
						}
					}
					if (!(node is XContainer xContainer))
					{
						return new XNode[1] { node.Copy(copyAttributeAnnotations: true) };
					}
					IEnumerable<XNode> enumerable = xContainer.Nodes();
					XContainer xContainer2 = xContainer.Copy(copyAttributeAnnotations: true);
					if (enumerable != null)
					{
						XNode[] content = RewriteMany(enumerable.ToArray(), currentXmlFilePath, originatingSyntax);
						xContainer2.ReplaceNodes(content);
					}
					if (xContainer2.NodeType == XmlNodeType.Element && originatingSyntax != null)
					{
						XElement xElement2 = (XElement)xContainer2;
						XName name = xElement2.Name;
						DocumentationCommentBinder.BinderType binderType = DocumentationCommentBinder.BinderType.None;
						bool flag = false;
						if (ElementNameIs(xElement2, "exception"))
						{
							if (!_tagsSupport.ExceptionSupported)
							{
								commentMessage = GenerateDiagnostic(XmlLocation.Create(xElement2, currentXmlFilePath), ERRID.WRN_XMLDocIllegalTagOnElement2, name.LocalName, _tagsSupport.SymbolName);
							}
							else
							{
								flag = true;
							}
						}
						else if (ElementNameIs(xElement2, "returns"))
						{
							if (!_tagsSupport.ReturnsSupported)
							{
								commentMessage = (_tagsSupport.IsDeclareMethod ? GenerateDiagnostic(XmlLocation.Create(xElement2, currentXmlFilePath), ERRID.WRN_XMLDocReturnsOnADeclareSub) : ((!_tagsSupport.IsWriteOnlyProperty) ? GenerateDiagnostic(XmlLocation.Create(xElement2, currentXmlFilePath), ERRID.WRN_XMLDocIllegalTagOnElement2, name.LocalName, _tagsSupport.SymbolName) : GenerateDiagnostic(XmlLocation.Create(xElement2, currentXmlFilePath), ERRID.WRN_XMLDocReturnsOnWriteOnlyProperty)));
							}
						}
						else if (ElementNameIs(xElement2, "param") || ElementNameIs(xElement2, "paramref"))
						{
							binderType = DocumentationCommentBinder.BinderType.NameInParamOrParamRef;
							if (!_tagsSupport.ParamAndParamRefSupported)
							{
								commentMessage = GenerateDiagnostic(XmlLocation.Create(xElement2, currentXmlFilePath), ERRID.WRN_XMLDocIllegalTagOnElement2, name.LocalName, _tagsSupport.SymbolName);
							}
						}
						else if (ElementNameIs(xElement2, "value"))
						{
							if (!_tagsSupport.ValueSupported)
							{
								commentMessage = GenerateDiagnostic(XmlLocation.Create(xElement2, currentXmlFilePath), ERRID.WRN_XMLDocIllegalTagOnElement2, name.LocalName, _tagsSupport.SymbolName);
							}
						}
						else if (ElementNameIs(xElement2, "typeparam"))
						{
							binderType = DocumentationCommentBinder.BinderType.NameInTypeParam;
							if (!_tagsSupport.TypeParamSupported)
							{
								commentMessage = GenerateDiagnostic(XmlLocation.Create(xElement2, currentXmlFilePath), ERRID.WRN_XMLDocIllegalTagOnElement2, name.LocalName, _tagsSupport.SymbolName);
							}
						}
						else if (ElementNameIs(xElement2, "typeparamref"))
						{
							binderType = DocumentationCommentBinder.BinderType.NameInTypeParamRef;
							if (!_tagsSupport.TypeParamRefSupported)
							{
								commentMessage = GenerateDiagnostic(XmlLocation.Create(xElement2, currentXmlFilePath), ERRID.WRN_XMLDocIllegalTagOnElement2, name.LocalName, _tagsSupport.SymbolName);
							}
						}
						if (commentMessage == null)
						{
							XAttribute xAttribute = null;
							bool flag2 = false;
							foreach (XAttribute item in xElement2.Attributes())
							{
								if (AttributeNameIs(item, "cref"))
								{
									BindAndReplaceCref(item, currentXmlFilePath);
									flag2 = true;
								}
								else if (AttributeNameIs(item, "name"))
								{
									xAttribute = item;
								}
							}
							if (flag)
							{
								if (!flag2)
								{
									commentMessage = GenerateDiagnostic(XmlLocation.Create(xElement2, currentXmlFilePath), ERRID.WRN_XMLDocExceptionTagWithoutCRef);
								}
							}
							else if (binderType != 0)
							{
								commentMessage = ((xAttribute != null) ? BindName(xAttribute, name.LocalName, binderType, (binderType == DocumentationCommentBinder.BinderType.NameInParamOrParamRef) ? ERRID.WRN_XMLDocBadParamTag2 : ERRID.WRN_XMLDocBadGenericParamTag2, currentXmlFilePath) : GenerateDiagnostic(XmlLocation.Create(xElement2, currentXmlFilePath), (binderType == DocumentationCommentBinder.BinderType.NameInParamOrParamRef) ? ERRID.WRN_XMLDocParamTagWithoutName : ERRID.WRN_XMLDocGenericParamTagWithoutName));
							}
						}
					}
					if (commentMessage == null)
					{
						return new XNode[1] { xContainer2 };
					}
					return new XNode[2]
					{
						new XComment(commentMessage),
						xContainer2
					};
				}

				private static bool ElementNameIs(XElement element, string name)
				{
					if (string.IsNullOrEmpty(element.Name.NamespaceName))
					{
						return DocumentationCommentXmlNames.ElementEquals(element.Name.LocalName, name, fromVb: true);
					}
					return false;
				}

				private static bool AttributeNameIs(XAttribute attribute, string name)
				{
					if (string.IsNullOrEmpty(attribute.Name.NamespaceName))
					{
						return DocumentationCommentXmlNames.AttributeEquals(attribute.Name.LocalName, name);
					}
					return false;
				}

				private XNode[] RewriteIncludeElement(XElement includeElement, string currentXmlFilePath, XmlNodeSyntax originatingSyntax, out string commentMessage)
				{
					Location includeElementLocation = GetIncludeElementLocation(includeElement, ref currentXmlFilePath, ref originatingSyntax);
					if (!AddIncludeElementLocation(includeElementLocation))
					{
						XAttribute xAttribute = includeElement.Attribute(XName.Get("file"));
						XAttribute xAttribute2 = includeElement.Attribute(XName.Get("path"));
						commentMessage = GenerateDiagnostic(includeElementLocation, ERRID.WRN_XMLDocInvalidXMLFragment, xAttribute.Value, xAttribute2.Value);
						return new XNode[1]
						{
							new XComment(commentMessage)
						};
					}
					try
					{
						XAttribute xAttribute3 = includeElement.Attribute(XName.Get("file"));
						XAttribute xAttribute4 = includeElement.Attribute(XName.Get("path"));
						bool flag = xAttribute3 != null;
						bool flag2 = xAttribute4 != null;
						if (!flag || !flag2)
						{
							if (!flag)
							{
								commentMessage = GenerateDiagnostic(includeElementLocation, ERRID.WRN_XMLMissingFileOrPathAttribute1, "file");
							}
							if (!flag2)
							{
								commentMessage = ((commentMessage == null) ? "" : (commentMessage + " ")) + GenerateDiagnostic(includeElementLocation, ERRID.WRN_XMLMissingFileOrPathAttribute1, "path");
							}
							return new XNode[1]
							{
								new XComment(commentMessage)
							};
						}
						string value = xAttribute4.Value;
						string value2 = xAttribute3.Value;
						XmlReferenceResolver xmlReferenceResolver = _compilation.Options.XmlReferenceResolver;
						if (xmlReferenceResolver == null)
						{
							commentMessage = GenerateDiagnostic(true, includeElementLocation, ERRID.WRN_XMLDocBadFormedXML, value2, value, new CodeAnalysisResourcesLocalizableErrorArgument("XmlReferencesNotSupported"));
							return new XNode[1]
							{
								new XComment(commentMessage)
							};
						}
						string text = xmlReferenceResolver.ResolveReference(value2, currentXmlFilePath);
						if (text == null)
						{
							commentMessage = GenerateDiagnostic(true, includeElementLocation, ERRID.WRN_XMLDocBadFormedXML, value2, value, new CodeAnalysisResourcesLocalizableErrorArgument("FileNotFound"));
							return new XNode[1]
							{
								new XComment(commentMessage)
							};
						}
						if (_includedFileCache == null)
						{
							_includedFileCache = new DocumentationCommentIncludeCache(_compilation.Options.XmlReferenceResolver);
						}
						try
						{
							XDocument orMakeDocument;
							try
							{
								orMakeDocument = _includedFileCache.GetOrMakeDocument(text);
							}
							catch (IOException ex)
							{
								ProjectData.SetProjectError(ex);
								IOException ex2 = ex;
								commentMessage = GenerateDiagnostic(true, includeElementLocation, ERRID.WRN_XMLDocBadFormedXML, value2, value, ex2.Message);
								XNode[] result = new XNode[1]
								{
									new XComment(commentMessage)
								};
								ProjectData.ClearProjectError();
								return result;
							}
							string errorMessage = null;
							bool invalidXPath = false;
							XElement[] array = XmlUtilities.TrySelectElements(orMakeDocument, value, out errorMessage, out invalidXPath);
							if (array == null)
							{
								commentMessage = GenerateDiagnostic(true, includeElementLocation, ERRID.WRN_XMLDocInvalidXMLFragment, value, value2);
								return new XNode[1]
								{
									new XComment(commentMessage)
								};
							}
							if (array != null && array.Length > 0)
							{
								XNode[] array2 = RewriteMany(array, text, originatingSyntax);
								if (array2.Length > 0)
								{
									commentMessage = null;
									return array2;
								}
							}
							commentMessage = GenerateDiagnostic(true, includeElementLocation, ERRID.WRN_XMLDocInvalidXMLFragment, value, value2);
							return new XNode[1]
							{
								new XComment(commentMessage)
							};
						}
						catch (XmlException ex3)
						{
							ProjectData.SetProjectError(ex3);
							XmlException ex4 = ex3;
							commentMessage = GenerateDiagnostic(true, includeElementLocation, ERRID.WRN_XMLDocInvalidXMLFragment, value, value2);
							XNode[] result = new XNode[1]
							{
								new XComment(commentMessage)
							};
							ProjectData.ClearProjectError();
							return result;
						}
					}
					finally
					{
						RemoveIncludeElementLocation(includeElementLocation);
					}
				}

				private bool ShouldProcessLocation(Location loc)
				{
					if (_onlyDiagnosticsFromTree != null)
					{
						if (loc.Kind == LocationKind.SourceFile && ((SourceLocation)loc).SourceTree == _onlyDiagnosticsFromTree)
						{
							if (_filterSpanWithinTree.HasValue)
							{
								return _filterSpanWithinTree.Value.Contains(loc.SourceSpan);
							}
							return true;
						}
						return false;
					}
					return true;
				}

				private string GenerateDiagnostic(bool suppressDiagnostic, Location loc, ERRID id, params object[] arguments)
				{
					DiagnosticInfo diagnosticInfo = ErrorFactory.ErrorInfo(id, arguments);
					if (!suppressDiagnostic && ProduceDiagnostics && ShouldProcessLocation(loc))
					{
						_diagnostics.Add(new VBDiagnostic(diagnosticInfo, loc));
					}
					return diagnosticInfo.ToString();
				}

				private string GenerateDiagnostic(Location loc, ERRID id, params object[] arguments)
				{
					return GenerateDiagnostic(suppressDiagnostic: false, loc, id, arguments);
				}

				private bool AddIncludeElementLocation(Location location)
				{
					if (_inProgressIncludeElementNodes == null)
					{
						_inProgressIncludeElementNodes = new HashSet<Location>();
					}
					return _inProgressIncludeElementNodes.Add(location);
				}

				private bool RemoveIncludeElementLocation(Location location)
				{
					return _inProgressIncludeElementNodes.Remove(location);
				}

				private Location GetIncludeElementLocation(XElement includeElement, ref string currentXmlFilePath, ref XmlNodeSyntax originatingSyntax)
				{
					Location location = includeElement.Annotation<Location>();
					if ((object)location != null)
					{
						return location;
					}
					if (currentXmlFilePath == null)
					{
						originatingSyntax = _sourceIncludeElementNodes[_nextSourceIncludeElementIndex];
						location = originatingSyntax.GetLocation();
						_nextSourceIncludeElementIndex++;
						includeElement.AddAnnotation(location);
						currentXmlFilePath = location.GetLineSpan().Path;
					}
					else
					{
						location = XmlLocation.Create(includeElement, currentXmlFilePath);
					}
					return location;
				}

				private void BindAndReplaceCref(XAttribute attribute, string currentXmlFilePath)
				{
					BaseXmlAttributeSyntax baseXmlAttributeSyntax = SyntaxFactory.ParseDocCommentAttributeAsStandAloneEntity(attribute.ToString(), "");
					switch (baseXmlAttributeSyntax.Kind())
					{
					case SyntaxKind.XmlCrefAttribute:
					{
						Binder orCreateBinder = GetOrCreateBinder(DocumentationCommentBinder.BinderType.Cref);
						CrefReferenceSyntax reference = ((XmlCrefAttributeSyntax)baseXmlAttributeSyntax).Reference;
						CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = orCreateBinder.GetNewCompoundUseSiteInfo(_diagnostics);
						BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(_diagnostics);
						ImmutableArray<Symbol> immutableArray = orCreateBinder.BindInsideCrefAttributeValue(reference, preserveAliases: false, instance, ref useSiteInfo);
						_diagnostics.AddDependencies(instance);
						_diagnostics.AddDependencies(useSiteInfo);
						ImmutableArray<Location> errorLocations = instance.DiagnosticBag!.ToReadOnly().SelectAsArray((Diagnostic x) => x.Location).WhereAsArray((Location x) => (object)x != null);
						instance.Free();
						if (ProduceXmlDiagnostics && !useSiteInfo.Diagnostics.IsNullOrEmpty())
						{
							ProcessErrorLocations(XmlLocation.Create(attribute, currentXmlFilePath), null, useSiteInfo, errorLocations, null);
						}
						if (immutableArray.IsDefaultOrEmpty)
						{
							if (ProduceXmlDiagnostics)
							{
								ProcessErrorLocations(XmlLocation.Create(attribute, currentXmlFilePath), reference.ToFullString().TrimEnd(new char[0]), useSiteInfo, errorLocations, ERRID.WRN_XMLDocCrefAttributeNotFound1);
							}
							attribute.Value = "?:" + attribute.Value;
							break;
						}
						string text2 = null;
						Symbol symbol = null;
						ERRID value = ERRID.WRN_XMLDocCrefAttributeNotFound1;
						ImmutableArray<Symbol>.Enumerator enumerator = immutableArray.GetEnumerator();
						while (enumerator.MoveNext())
						{
							Symbol current = enumerator.Current;
							if (current.Kind == SymbolKind.TypeParameter)
							{
								value = ERRID.WRN_XMLDocCrefToTypeParameter;
								continue;
							}
							string documentationCommentId = current.OriginalDefinition.GetDocumentationCommentId();
							if (documentationCommentId != null && (text2 == null || _compilation.CompareSourceLocations(symbol.Locations[0], current.Locations[0]) > 0))
							{
								text2 = documentationCommentId;
								symbol = current;
							}
						}
						if (text2 == null)
						{
							if (ProduceXmlDiagnostics)
							{
								ProcessErrorLocations(XmlLocation.Create(attribute, currentXmlFilePath), reference.ToString(), default(CompoundUseSiteInfo<AssemblySymbol>), errorLocations, value);
							}
							attribute.Value = "?:" + attribute.Value;
						}
						else
						{
							attribute.Value = text2;
							_diagnostics.AddAssembliesUsedByCrefTarget(symbol.OriginalDefinition);
						}
						break;
					}
					case SyntaxKind.XmlAttribute:
					{
						string text = attribute.Value.Trim();
						if (text.Length < 2 || text[0] == ':' || text[1] != ':')
						{
							if (ProduceXmlDiagnostics)
							{
								_diagnostics.Add(ERRID.WRN_XMLDocCrefAttributeNotFound1, XmlLocation.Create(attribute, currentXmlFilePath), text);
							}
							attribute.Value = "?:" + text;
						}
						break;
					}
					default:
						throw ExceptionUtilities.UnexpectedValue(baseXmlAttributeSyntax.Kind());
					}
				}

				private void ProcessErrorLocations(XmlLocation currentXmlLocation, string referenceName, CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ImmutableArray<Location> errorLocations, ERRID? errid)
				{
					if (errorLocations.Length == 0)
					{
						if (useSiteInfo.Diagnostics != null)
						{
							_diagnostics.AddDiagnostics(currentXmlLocation, useSiteInfo);
						}
						else if (errid.HasValue)
						{
							_diagnostics.Add(errid.Value, currentXmlLocation, referenceName);
						}
					}
					else if (errid.HasValue)
					{
						ImmutableArray<Location>.Enumerator enumerator = errorLocations.GetEnumerator();
						while (enumerator.MoveNext())
						{
							Location current = enumerator.Current;
							_diagnostics.Add(errid.Value, current, referenceName);
						}
					}
					else
					{
						ImmutableArray<Location>.Enumerator enumerator2 = errorLocations.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							Location current2 = enumerator2.Current;
							_diagnostics.AddDiagnostics(current2, useSiteInfo);
						}
					}
				}

				private string BindName(XAttribute attribute, string elementName, DocumentationCommentBinder.BinderType type, ERRID badNameValueError, string currentXmlFilePath)
				{
					string result = null;
					string text = attribute.ToString();
					string text2 = attribute.Value.Trim();
					BaseXmlAttributeSyntax baseXmlAttributeSyntax = SyntaxFactory.ParseDocCommentAttributeAsStandAloneEntity(text, elementName);
					switch (baseXmlAttributeSyntax.Kind())
					{
					case SyntaxKind.XmlNameAttribute:
					{
						Binder orCreateBinder = GetOrCreateBinder(type);
						IdentifierNameSyntax reference = ((XmlNameAttributeSyntax)baseXmlAttributeSyntax).Reference;
						CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = orCreateBinder.GetNewCompoundUseSiteInfo(_diagnostics);
						ImmutableArray<Symbol> immutableArray = orCreateBinder.BindXmlNameAttributeValue(reference, ref useSiteInfo);
						_diagnostics.AddDependencies(useSiteInfo);
						if (ProduceDiagnostics && !useSiteInfo.Diagnostics.IsNullOrEmpty())
						{
							Location location = XmlLocation.Create(attribute, currentXmlFilePath);
							if (ShouldProcessLocation(location))
							{
								_diagnostics.AddDiagnostics(location, useSiteInfo);
							}
						}
						if (immutableArray.IsDefaultOrEmpty)
						{
							result = GenerateDiagnostic(XmlLocation.Create(attribute, currentXmlFilePath), badNameValueError, text2, _tagsSupport.SymbolName);
						}
						break;
					}
					case SyntaxKind.XmlAttribute:
						result = GenerateDiagnostic(XmlLocation.Create(attribute, currentXmlFilePath), badNameValueError, text2, _tagsSupport.SymbolName);
						break;
					default:
						throw ExceptionUtilities.UnexpectedValue(baseXmlAttributeSyntax.Kind());
					}
					return result;
				}
			}

			private class DocumentationCommentWalker : VisualBasicSyntaxWalker
			{
				private readonly Symbol _symbol;

				private readonly SyntaxTree _syntaxTree;

				private readonly Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> _wellKnownElementNodes;

				private readonly bool _reportDiagnostics;

				private readonly TextWriter _writer;

				private readonly BindingDiagnosticBag _diagnostics;

				private VisualBasicCompilation Compilation => _symbol.DeclaringCompilation;

				private SourceModuleSymbol Module => (SourceModuleSymbol)Compilation.SourceModule;

				private DocumentationCommentWalker(Symbol symbol, SyntaxTree syntaxTree, Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes, TextWriter writer, BindingDiagnosticBag diagnostics)
					: base(SyntaxWalkerDepth.Token)
				{
					_symbol = symbol;
					_syntaxTree = syntaxTree;
					_wellKnownElementNodes = wellKnownElementNodes;
					_writer = writer;
					_diagnostics = diagnostics;
					_reportDiagnostics = SyntaxExtensions.ReportDocumentationCommentDiagnostics(syntaxTree);
				}

				private void CaptureWellKnownTagNode(XmlNodeSyntax node, XmlNodeSyntax name)
				{
					if (_wellKnownElementNodes == null || name.Kind() != SyntaxKind.XmlName)
					{
						return;
					}
					WellKnownTag wellKnownTag = GetWellKnownTag(((XmlNameSyntax)name).LocalName.ValueText);
					if ((wellKnownTag & WellKnownTag.AllCollectable) != 0)
					{
						ArrayBuilder<XmlNodeSyntax> value = null;
						if (!_wellKnownElementNodes.TryGetValue(wellKnownTag, out value))
						{
							value = ArrayBuilder<XmlNodeSyntax>.GetInstance();
							_wellKnownElementNodes.Add(wellKnownTag, value);
						}
						value.Add(node);
					}
				}

				public override void VisitXmlEmptyElement(XmlEmptyElementSyntax node)
				{
					CaptureWellKnownTagNode(node, node.Name);
					base.VisitXmlEmptyElement(node);
				}

				public override void VisitXmlElement(XmlElementSyntax node)
				{
					CaptureWellKnownTagNode(node, node.StartTag.Name);
					base.VisitXmlElement(node);
				}

				private void WriteHeaderAndVisit(Symbol symbol, DocumentationCommentTriviaSyntax trivia)
				{
					_writer.Write("<member name=\"");
					_writer.Write(symbol.GetDocumentationCommentId());
					_writer.WriteLine("\">");
					Visit(trivia);
					_writer.WriteLine("</member>");
				}

				internal static string GetSubstitutedText(Symbol symbol, DocumentationCommentTriviaSyntax trivia, Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes, BindingDiagnosticBag diagnostics)
				{
					PooledStringBuilder instance = PooledStringBuilder.GetInstance();
					using (StringWriter writer = new StringWriter(instance.Builder))
					{
						new DocumentationCommentWalker(symbol, trivia.SyntaxTree, wellKnownElementNodes, writer, diagnostics).WriteHeaderAndVisit(symbol, trivia);
					}
					return instance.ToStringAndFree();
				}

				public override void DefaultVisit(SyntaxNode node)
				{
					switch (VisualBasicExtensions.Kind(node))
					{
					case SyntaxKind.XmlCrefAttribute:
					{
						XmlCrefAttributeSyntax xmlCrefAttributeSyntax = (XmlCrefAttributeSyntax)node;
						Visit(xmlCrefAttributeSyntax.Name);
						VisitToken(xmlCrefAttributeSyntax.EqualsToken);
						VisitToken(xmlCrefAttributeSyntax.StartQuoteToken);
						CrefReferenceSyntax reference = xmlCrefAttributeSyntax.Reference;
						Binder binder = CreateDocumentationCommentBinderForSymbol(Module, _symbol, _syntaxTree, DocumentationCommentBinder.BinderType.Cref);
						CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = binder.GetNewCompoundUseSiteInfo(_diagnostics);
						BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(withDiagnostics: true, _diagnostics.AccumulatesDependencies);
						ImmutableArray<Symbol> immutableArray = binder.BindInsideCrefAttributeValue(reference, preserveAliases: false, instance, ref useSiteInfo);
						_diagnostics.AddDependencies(instance);
						_diagnostics.AddDependencies(useSiteInfo);
						ImmutableArray<Location> errorLocations = instance.DiagnosticBag!.ToReadOnly().SelectAsArray((Diagnostic x) => x.Location).WhereAsArray((Location x) => (object)x != null);
						instance.Free();
						if (!useSiteInfo.Diagnostics.IsNullOrEmpty() && _reportDiagnostics)
						{
							ProcessErrorLocations(node, errorLocations, useSiteInfo, null);
						}
						if (immutableArray.IsEmpty)
						{
							ProcessErrorLocations(xmlCrefAttributeSyntax, errorLocations, default(CompoundUseSiteInfo<AssemblySymbol>), ERRID.WRN_XMLDocCrefAttributeNotFound1);
						}
						else if (immutableArray.Length > 1 && reference.Signature != null)
						{
							ProcessErrorLocations(xmlCrefAttributeSyntax, errorLocations, default(CompoundUseSiteInfo<AssemblySymbol>), ERRID.WRN_XMLDocCrefAttributeNotFound1);
						}
						else
						{
							_ = Compilation;
							string text = null;
							Symbol symbol = null;
							ERRID value = ERRID.WRN_XMLDocCrefAttributeNotFound1;
							ImmutableArray<Symbol>.Enumerator enumerator2 = immutableArray.GetEnumerator();
							while (enumerator2.MoveNext())
							{
								Symbol current2 = enumerator2.Current;
								if (current2.Kind == SymbolKind.TypeParameter)
								{
									value = ERRID.WRN_XMLDocCrefToTypeParameter;
									continue;
								}
								string documentationCommentId = current2.OriginalDefinition.GetDocumentationCommentId();
								if (documentationCommentId != null && (text == null || string.CompareOrdinal(text, documentationCommentId) > 0))
								{
									text = documentationCommentId;
									symbol = current2;
								}
							}
							if (text == null)
							{
								ProcessErrorLocations(xmlCrefAttributeSyntax, errorLocations, default(CompoundUseSiteInfo<AssemblySymbol>), value);
							}
							else
							{
								if (_writer != null)
								{
									_writer.Write(text);
								}
								_diagnostics.AddAssembliesUsedByCrefTarget(symbol.OriginalDefinition);
							}
						}
						VisitToken(xmlCrefAttributeSyntax.EndQuoteToken);
						return;
					}
					case SyntaxKind.XmlAttribute:
					{
						XmlAttributeSyntax xmlAttributeSyntax = (XmlAttributeSyntax)node;
						if (!xmlAttributeSyntax.ContainsDiagnostics && DocumentationCommentXmlNames.AttributeEquals(((XmlNameSyntax)xmlAttributeSyntax.Name).LocalName.ValueText, "cref"))
						{
							XmlStringSyntax xmlStringSyntax = (XmlStringSyntax)xmlAttributeSyntax.Value;
							string xmlString = Binder.GetXmlString(xmlStringSyntax.TextTokens);
							bool flag = xmlString.Length < 2 || xmlString[0] == ':' || xmlString[1] != ':';
							Visit(xmlAttributeSyntax.Name);
							VisitToken(xmlAttributeSyntax.EqualsToken);
							VisitToken(xmlStringSyntax.StartQuoteToken);
							if (flag && _reportDiagnostics)
							{
								_diagnostics.Add(ERRID.WRN_XMLDocCrefAttributeNotFound1, node.GetLocation(), xmlString.Trim());
							}
							if (flag && _writer != null)
							{
								_writer.Write("!:");
							}
							SyntaxTokenList.Enumerator enumerator = xmlStringSyntax.TextTokens.GetEnumerator();
							while (enumerator.MoveNext())
							{
								SyntaxToken current = enumerator.Current;
								VisitToken(current);
							}
							VisitToken(xmlStringSyntax.EndQuoteToken);
							return;
						}
						break;
					}
					}
					base.DefaultVisit(node);
				}

				private void ProcessErrorLocations(SyntaxNode node, ImmutableArray<Location> errorLocations, CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ERRID? errid)
				{
					if (node is XmlCrefAttributeSyntax xmlCrefAttributeSyntax && errid.HasValue)
					{
						if (errorLocations.Length == 0)
						{
							ProcessBadNameInCrefAttribute(xmlCrefAttributeSyntax, xmlCrefAttributeSyntax.GetLocation(), errid.Value);
							return;
						}
						ImmutableArray<Location>.Enumerator enumerator = errorLocations.GetEnumerator();
						while (enumerator.MoveNext())
						{
							Location current = enumerator.Current;
							ProcessBadNameInCrefAttribute(xmlCrefAttributeSyntax, current, errid.Value);
						}
					}
					else if (errorLocations.Length == 0 && useSiteInfo.Diagnostics != null)
					{
						_diagnostics.AddDiagnostics(node, useSiteInfo);
					}
					else if (useSiteInfo.Diagnostics != null)
					{
						ImmutableArray<Location>.Enumerator enumerator2 = errorLocations.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							Location current2 = enumerator2.Current;
							_diagnostics.AddDiagnostics(current2, useSiteInfo);
						}
					}
				}

				private void ProcessBadNameInCrefAttribute(XmlCrefAttributeSyntax crefAttribute, Location errorLocation, ERRID errid)
				{
					if (_writer != null)
					{
						_writer.Write("!:");
					}
					VisualBasicSyntaxNode reference = crefAttribute.Reference;
					Visit(reference);
					if (_reportDiagnostics)
					{
						Location location = errorLocation ?? reference.GetLocation();
						_diagnostics.Add(errid, location, reference.ToFullString().TrimEnd(new char[0]));
					}
				}

				public override void VisitToken(SyntaxToken token)
				{
					if (_writer != null)
					{
						token.WriteTo(_writer);
					}
					base.VisitToken(token);
				}
			}

			private struct DocWriter
			{
				private struct TemporaryStringBuilder
				{
					public readonly PooledStringBuilder Pooled;

					public readonly int InitialIndentDepth;

					public TemporaryStringBuilder(int indentDepth)
					{
						this = default(TemporaryStringBuilder);
						InitialIndentDepth = indentDepth;
						Pooled = PooledStringBuilder.GetInstance();
					}
				}

				private readonly TextWriter _writer;

				private int _indentDepth;

				private Stack<TemporaryStringBuilder> _temporaryStringBuilders;

				public bool IsSpecified => _writer != null;

				public int IndentDepth => _indentDepth;

				public DocWriter(TextWriter writer)
				{
					this = default(DocWriter);
					_writer = writer;
					_indentDepth = 0;
					_temporaryStringBuilders = null;
				}

				public void Indent()
				{
				}

				public void Unindent()
				{
				}

				public void WriteLine(string message)
				{
					if (IsSpecified)
					{
						if (_temporaryStringBuilders != null && _temporaryStringBuilders.Count > 0)
						{
							StringBuilder builder = _temporaryStringBuilders.Peek().Pooled.Builder;
							builder.Append(MakeIndent(_indentDepth));
							builder.AppendLine(message);
						}
						else if (_writer != null)
						{
							_writer.Write(MakeIndent(_indentDepth));
							_writer.WriteLine(message);
						}
					}
				}

				public void Write(string message)
				{
					if (IsSpecified)
					{
						if (_temporaryStringBuilders != null && _temporaryStringBuilders.Count > 0)
						{
							StringBuilder builder = _temporaryStringBuilders.Peek().Pooled.Builder;
							builder.Append(MakeIndent(_indentDepth));
							builder.Append(message);
						}
						else if (_writer != null)
						{
							_writer.Write(MakeIndent(_indentDepth));
							_writer.Write(message);
						}
					}
				}

				public void WriteSubString(string message, int start, int length, bool appendNewLine = true)
				{
					if (_temporaryStringBuilders != null && _temporaryStringBuilders.Count > 0)
					{
						StringBuilder builder = _temporaryStringBuilders.Peek().Pooled.Builder;
						builder.Append(MakeIndent(IndentDepth));
						builder.Append(message, start, length);
						if (appendNewLine)
						{
							builder.AppendLine();
						}
					}
					else if (_writer != null)
					{
						_writer.Write(MakeIndent(IndentDepth));
						int num = length - 1;
						for (int i = 0; i <= num; i++)
						{
							_writer.Write(message[start + i]);
						}
						if (appendNewLine)
						{
							_writer.WriteLine();
						}
					}
				}

				public string GetAndEndTemporaryString()
				{
					TemporaryStringBuilder temporaryStringBuilder = _temporaryStringBuilders.Pop();
					_indentDepth = temporaryStringBuilder.InitialIndentDepth;
					return temporaryStringBuilder.Pooled.ToStringAndFree();
				}

				private static string MakeIndent(int depth)
				{
					return depth switch
					{
						0 => "", 
						1 => "    ", 
						2 => "        ", 
						3 => "            ", 
						_ => new string(' ', depth * 4), 
					};
				}

				public void BeginTemporaryString()
				{
					if (_temporaryStringBuilders == null)
					{
						_temporaryStringBuilders = new Stack<TemporaryStringBuilder>();
					}
					_temporaryStringBuilders.Push(new TemporaryStringBuilder(_indentDepth));
				}
			}

			private class MislocatedDocumentationCommentFinder : VisualBasicSyntaxWalker
			{
				private readonly DiagnosticBag _diagnostics;

				private readonly TextSpan? _filterSpanWithinTree;

				private readonly CancellationToken _cancellationToken;

				private bool _isInsideMethodOrLambda;

				private MislocatedDocumentationCommentFinder(DiagnosticBag diagnostics, TextSpan? filterSpanWithinTree, CancellationToken cancellationToken)
					: base(SyntaxWalkerDepth.Trivia)
				{
					_diagnostics = diagnostics;
					_filterSpanWithinTree = filterSpanWithinTree;
					_cancellationToken = cancellationToken;
					_isInsideMethodOrLambda = false;
				}

				public static void ReportUnprocessed(SyntaxTree tree, TextSpan? filterSpanWithinTree, DiagnosticBag diagnostics, CancellationToken cancellationToken)
				{
					if (SyntaxExtensions.ReportDocumentationCommentDiagnostics(tree))
					{
						new MislocatedDocumentationCommentFinder(diagnostics, filterSpanWithinTree, cancellationToken).Visit(tree.GetRoot(cancellationToken));
					}
				}

				private bool IsSyntacticallyFilteredOut(TextSpan fullSpan)
				{
					if (_filterSpanWithinTree.HasValue)
					{
						return !_filterSpanWithinTree.Value.Contains(fullSpan);
					}
					return false;
				}

				public override void VisitMethodBlock(MethodBlockSyntax node)
				{
					VisitMethodBlockBase(node);
				}

				public override void VisitConstructorBlock(ConstructorBlockSyntax node)
				{
					VisitMethodBlockBase(node);
				}

				public override void VisitOperatorBlock(OperatorBlockSyntax node)
				{
					VisitMethodBlockBase(node);
				}

				public override void VisitAccessorBlock(AccessorBlockSyntax node)
				{
					VisitMethodBlockBase(node);
				}

				private void VisitMethodBlockBase(MethodBlockBaseSyntax node)
				{
					CancellationToken cancellationToken = _cancellationToken;
					cancellationToken.ThrowIfCancellationRequested();
					if (!IsSyntacticallyFilteredOut(node.FullSpan))
					{
						bool isInsideMethodOrLambda = _isInsideMethodOrLambda;
						_isInsideMethodOrLambda = false;
						Visit(node.BlockStatement);
						_isInsideMethodOrLambda = true;
						DefaultVisitChildrenStartingWith(node, 1);
						_isInsideMethodOrLambda = isInsideMethodOrLambda;
					}
				}

				public override void VisitMultiLineLambdaExpression(MultiLineLambdaExpressionSyntax node)
				{
					CancellationToken cancellationToken = _cancellationToken;
					cancellationToken.ThrowIfCancellationRequested();
					if (!IsSyntacticallyFilteredOut(node.FullSpan))
					{
						bool isInsideMethodOrLambda = _isInsideMethodOrLambda;
						_isInsideMethodOrLambda = true;
						base.VisitMultiLineLambdaExpression(node);
						_isInsideMethodOrLambda = isInsideMethodOrLambda;
					}
				}

				public override void DefaultVisit(SyntaxNode node)
				{
					if (node.HasStructuredTrivia && !IsSyntacticallyFilteredOut(node.FullSpan))
					{
						base.DefaultVisit(node);
					}
				}

				private void DefaultVisitChildrenStartingWith(SyntaxNode node, int start)
				{
					ChildSyntaxList childSyntaxList = node.ChildNodesAndTokens();
					int count = childSyntaxList.Count;
					int num = start;
					while (num < count)
					{
						SyntaxNodeOrToken syntaxNodeOrToken = childSyntaxList[num];
						num++;
						SyntaxNode syntaxNode = syntaxNodeOrToken.AsNode();
						if (syntaxNode != null)
						{
							Visit(syntaxNode);
						}
						else
						{
							VisitToken(syntaxNodeOrToken.AsToken());
						}
					}
				}

				public override void VisitTrivia(SyntaxTrivia trivia)
				{
					if (IsSyntacticallyFilteredOut(trivia.FullSpan))
					{
						return;
					}
					if (VisualBasicExtensions.Kind(trivia) == SyntaxKind.DocumentationCommentTrivia)
					{
						if (_isInsideMethodOrLambda)
						{
							DiagnosticBagExtensions.Add(_diagnostics, ERRID.WRN_XMLDocInsideMethod, trivia.GetLocation());
						}
						else
						{
							VisualBasicSyntaxNode visualBasicSyntaxNode = (VisualBasicSyntaxNode)trivia.Token.Parent;
							while (true)
							{
								switch (visualBasicSyntaxNode.Kind())
								{
								case SyntaxKind.AttributeList:
									goto IL_008e;
								default:
									DiagnosticBagExtensions.Add(_diagnostics, ERRID.WRN_XMLDocWithoutLanguageElement, trivia.GetLocation());
									break;
								case SyntaxKind.ModuleStatement:
								case SyntaxKind.StructureStatement:
								case SyntaxKind.InterfaceStatement:
								case SyntaxKind.ClassStatement:
								case SyntaxKind.EnumStatement:
								case SyntaxKind.EnumMemberDeclaration:
								case SyntaxKind.SubStatement:
								case SyntaxKind.FunctionStatement:
								case SyntaxKind.SubNewStatement:
								case SyntaxKind.DeclareSubStatement:
								case SyntaxKind.DeclareFunctionStatement:
								case SyntaxKind.DelegateSubStatement:
								case SyntaxKind.DelegateFunctionStatement:
								case SyntaxKind.EventStatement:
								case SyntaxKind.OperatorStatement:
								case SyntaxKind.PropertyStatement:
								case SyntaxKind.FieldDeclaration:
									break;
								}
								break;
								IL_008e:
								visualBasicSyntaxNode = visualBasicSyntaxNode.Parent;
							}
						}
					}
					base.VisitTrivia(trivia);
				}
			}

			private readonly string _assemblyName;

			private readonly VisualBasicCompilation _compilation;

			private readonly bool _processIncludes;

			private readonly bool _isForSingleSymbol;

			private readonly BindingDiagnosticBag _diagnostics;

			private readonly CancellationToken _cancellationToken;

			private readonly SyntaxTree _filterSyntaxTree;

			private readonly TextSpan? _filterSpanWithinTree;

			private DocWriter _writer;

			private DocumentationCommentIncludeCache _includedFileCache;

			private bool IsInSemanticModelMode => _isForSingleSymbol;

			private SourceModuleSymbol Module => (SourceModuleSymbol)_compilation.SourceModule;

			private bool ShouldSkipSymbol(Symbol symbol)
			{
				if (_filterSyntaxTree != null)
				{
					return !symbol.IsDefinedInSourceTree(_filterSyntaxTree, _filterSpanWithinTree, _cancellationToken);
				}
				return false;
			}

			private static string GetElementNameOfWellKnownTag(WellKnownTag tag)
			{
				return tag switch
				{
					WellKnownTag.C => "c", 
					WellKnownTag.Code => "code", 
					WellKnownTag.Example => "example", 
					WellKnownTag.Exception => "exception", 
					WellKnownTag.Include => "include", 
					WellKnownTag.List => "list", 
					WellKnownTag.Para => "para", 
					WellKnownTag.Param => "param", 
					WellKnownTag.ParamRef => "paramref", 
					WellKnownTag.Permission => "permission", 
					WellKnownTag.Remarks => "remarks", 
					WellKnownTag.Returns => "returns", 
					WellKnownTag.See => "see", 
					WellKnownTag.SeeAlso => "seealso", 
					WellKnownTag.Summary => "summary", 
					WellKnownTag.TypeParam => "typeparam", 
					WellKnownTag.TypeParamRef => "typeparamref", 
					WellKnownTag.Value => "value", 
					_ => throw ExceptionUtilities.UnexpectedValue(tag), 
				};
			}

			private static WellKnownTag GetWellKnownTag(string elementName)
			{
				if (string.IsNullOrEmpty(elementName))
				{
					return WellKnownTag.None;
				}
				switch (_003CPrivateImplementationDetails_003E.ComputeStringHash(elementName))
				{
				case 3859557458u:
					if (EmbeddedOperators.CompareString(elementName, "c", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.C;
				case 4180765940u:
					if (EmbeddedOperators.CompareString(elementName, "code", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.Code;
				case 2347908769u:
					if (EmbeddedOperators.CompareString(elementName, "example", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.Example;
				case 626768054u:
					if (EmbeddedOperators.CompareString(elementName, "exception", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.Exception;
				case 1968798311u:
					if (EmbeddedOperators.CompareString(elementName, "include", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.Include;
				case 217798785u:
					if (EmbeddedOperators.CompareString(elementName, "list", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.List;
				case 1769478187u:
					if (EmbeddedOperators.CompareString(elementName, "para", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.Para;
				case 1309554226u:
					if (EmbeddedOperators.CompareString(elementName, "param", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.Param;
				case 592831643u:
					if (EmbeddedOperators.CompareString(elementName, "paramref", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.ParamRef;
				case 735751426u:
					if (EmbeddedOperators.CompareString(elementName, "permission", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.Permission;
				case 2076528582u:
					if (EmbeddedOperators.CompareString(elementName, "remarks", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.Remarks;
				case 2718029348u:
					if (EmbeddedOperators.CompareString(elementName, "returns", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.Returns;
				case 3039226944u:
					if (EmbeddedOperators.CompareString(elementName, "see", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.See;
				case 4270218159u:
					if (EmbeddedOperators.CompareString(elementName, "seealso", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.SeeAlso;
				case 279201555u:
					if (EmbeddedOperators.CompareString(elementName, "summary", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.Summary;
				case 2921814810u:
					if (EmbeddedOperators.CompareString(elementName, "typeparam", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.TypeParam;
				case 4064055315u:
					if (EmbeddedOperators.CompareString(elementName, "typeparamref", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.TypeParamRef;
				case 1113510858u:
					if (EmbeddedOperators.CompareString(elementName, "value", TextCompare: false) != 0)
					{
						break;
					}
					return WellKnownTag.Value;
				}
				return WellKnownTag.None;
			}

			private void ReportIllegalWellKnownTagIfAny(WellKnownTag tag, Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes, string symbolName)
			{
				ReportIllegalWellKnownTagIfAny(tag, ERRID.WRN_XMLDocIllegalTagOnElement2, wellKnownElementNodes, GetElementNameOfWellKnownTag(tag), symbolName);
			}

			private void ReportIllegalWellKnownTagIfAny(WellKnownTag tag, ERRID errorId, Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes, params object[] args)
			{
				ArrayBuilder<XmlNodeSyntax> value = null;
				if (!wellKnownElementNodes.TryGetValue(tag, out value))
				{
					return;
				}
				ArrayBuilder<XmlNodeSyntax>.Enumerator enumerator = value.GetEnumerator();
				while (enumerator.MoveNext())
				{
					XmlNodeSyntax current = enumerator.Current;
					if (SyntaxExtensions.ReportDocumentationCommentDiagnostics(current.SyntaxTree))
					{
						_diagnostics.Add(errorId, current.GetLocation(), args);
					}
				}
			}

			private void ReportWarningsForDuplicatedTags(Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes, bool isEvent = false)
			{
				ArrayBuilder<XmlNodeSyntax> value = null;
				if (wellKnownElementNodes.TryGetValue(WellKnownTag.Include, out value))
				{
					ReportWarningsForDuplicatedTags(value, "include");
				}
				if (!isEvent && wellKnownElementNodes.TryGetValue(WellKnownTag.Param, out value))
				{
					ReportWarningsForDuplicatedTags(value, "param");
				}
				if (wellKnownElementNodes.TryGetValue(WellKnownTag.Permission, out value))
				{
					ReportWarningsForDuplicatedTags(value, "permission");
				}
				if (wellKnownElementNodes.TryGetValue(WellKnownTag.Remarks, out value))
				{
					ReportWarningsForDuplicatedTags(value, "remarks");
				}
				if (wellKnownElementNodes.TryGetValue(WellKnownTag.Returns, out value))
				{
					ReportWarningsForDuplicatedTags(value, "returns");
				}
				if (wellKnownElementNodes.TryGetValue(WellKnownTag.Summary, out value))
				{
					ReportWarningsForDuplicatedTags(value, "summary");
				}
				if (wellKnownElementNodes.TryGetValue(WellKnownTag.TypeParam, out value))
				{
					ReportWarningsForDuplicatedTags(value, "typeparam");
				}
				if (wellKnownElementNodes.TryGetValue(WellKnownTag.Value, out value))
				{
					ReportWarningsForDuplicatedTags(value, "value");
				}
			}

			private void ReportWarningsForDuplicatedTags(ArrayBuilder<XmlNodeSyntax> nodes, string tagName)
			{
				if (nodes == null || nodes.Count < 2)
				{
					return;
				}
				bool flag = SyntaxExtensions.ReportDocumentationCommentDiagnostics(nodes[0].SyntaxTree);
				ArrayBuilder<XmlNodeWithAttributes> instance = ArrayBuilder<XmlNodeWithAttributes>.GetInstance();
				int num = nodes.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					instance.Add(new XmlNodeWithAttributes(nodes[i]));
				}
				instance.Sort();
				int num2 = instance.Count - 2;
				for (int j = 0; j <= num2; j++)
				{
					XmlNodeWithAttributes xmlNodeWithAttributes = instance[j];
					XmlNodeWithAttributes xmlNodeWithAttributes2 = instance[j + 1];
					if (XmlNodeWithAttributes.CompareAttributes(xmlNodeWithAttributes.Attributes, xmlNodeWithAttributes2.Attributes) == 0 && flag)
					{
						_diagnostics.Add(ERRID.WRN_XMLDocDuplicateXMLNode1, xmlNodeWithAttributes2.Node.GetLocation(), tagName);
					}
				}
				instance.Free();
			}

			private static SortedDictionary<string, string> GetElementAttributes(XmlNodeSyntax element)
			{
				SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
				SyntaxList<XmlNodeSyntax>.Enumerator enumerator = GetXmlElementAttributes(element).GetEnumerator();
				while (enumerator.MoveNext())
				{
					XmlNodeSyntax current = enumerator.Current;
					string text = null;
					string text2 = null;
					SyntaxKind syntaxKind = current.Kind();
					if (syntaxKind != SyntaxKind.XmlAttribute)
					{
						if (syntaxKind != SyntaxKind.XmlCrefAttribute)
						{
							if (syntaxKind != SyntaxKind.XmlNameAttribute)
							{
								continue;
							}
							XmlNameAttributeSyntax obj = (XmlNameAttributeSyntax)current;
							text = "name";
							text2 = obj.Reference.Identifier.ToString();
						}
						else
						{
							XmlCrefAttributeSyntax obj2 = (XmlCrefAttributeSyntax)current;
							text = "cref";
							text2 = obj2.Reference.ToFullString().Trim();
						}
					}
					else
					{
						XmlAttributeSyntax xmlAttributeSyntax = (XmlAttributeSyntax)current;
						if (xmlAttributeSyntax.Name.Kind() != SyntaxKind.XmlName || xmlAttributeSyntax.Value.Kind() != SyntaxKind.XmlString)
						{
							continue;
						}
						text = ((XmlNameSyntax)xmlAttributeSyntax.Name).LocalName.ValueText;
						text2 = Binder.GetXmlString(((XmlStringSyntax)xmlAttributeSyntax.Value).TextTokens);
					}
					if (text != null && text2 != null && !sortedDictionary.ContainsKey(text))
					{
						sortedDictionary.Add(text, text2.Trim());
					}
				}
				return sortedDictionary;
			}

			private void ReportWarningsForExceptionTags(Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes)
			{
				ArrayBuilder<XmlNodeSyntax> value = null;
				if (!wellKnownElementNodes.TryGetValue(WellKnownTag.Exception, out value))
				{
					return;
				}
				ArrayBuilder<XmlNodeSyntax>.Enumerator enumerator = value.GetEnumerator();
				while (enumerator.MoveNext())
				{
					XmlNodeSyntax current = enumerator.Current;
					bool flag = false;
					SyntaxList<XmlNodeSyntax>.Enumerator enumerator2 = GetXmlElementAttributes(current).GetEnumerator();
					while (enumerator2.MoveNext())
					{
						XmlNodeSyntax current2 = enumerator2.Current;
						switch (current2.Kind())
						{
						case SyntaxKind.XmlCrefAttribute:
							flag = true;
							break;
						case SyntaxKind.XmlAttribute:
						{
							XmlNodeSyntax name = ((XmlAttributeSyntax)current2).Name;
							if (name.Kind() != SyntaxKind.XmlName || !DocumentationCommentXmlNames.AttributeEquals(((XmlNameSyntax)name).LocalName.ValueText, "cref"))
							{
								continue;
							}
							flag = true;
							break;
						}
						default:
							continue;
						}
						break;
					}
					if (!flag && SyntaxExtensions.ReportDocumentationCommentDiagnostics(current.SyntaxTree))
					{
						_diagnostics.Add(ERRID.WRN_XMLDocExceptionTagWithoutCRef, current.GetLocation());
					}
				}
			}

			private void ReportWarningsForParamAndParamRefTags(Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes, string symbolName, ImmutableArray<ParameterSymbol> parameters)
			{
				ReportWarningsForParamOrTypeParamTags(wellKnownElementNodes, WellKnownTag.Param, WellKnownTag.ParamRef, symbolName, ERRID.WRN_XMLDocBadParamTag2, ERRID.WRN_XMLDocParamTagWithoutName, parameters);
			}

			private void ReportWarningsForTypeParamTags(Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes, string symbolName, ImmutableArray<TypeParameterSymbol> typeParameters)
			{
				ReportWarningsForParamOrTypeParamTags(wellKnownElementNodes, WellKnownTag.TypeParam, WellKnownTag.None, symbolName, ERRID.WRN_XMLDocBadGenericParamTag2, ERRID.WRN_XMLDocGenericParamTagWithoutName, typeParameters);
			}

			private void ReportWarningsForTypeParamRefTags(Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes, string symbolName, Symbol symbol, SyntaxTree tree)
			{
				ArrayBuilder<XmlNodeSyntax> value = null;
				if (!wellKnownElementNodes.TryGetValue(WellKnownTag.TypeParamRef, out value))
				{
					return;
				}
				Binder binder = CreateDocumentationCommentBinderForSymbol(Module, symbol, tree, DocumentationCommentBinder.BinderType.NameInTypeParamRef);
				ArrayBuilder<XmlNodeSyntax>.Enumerator enumerator = value.GetEnumerator();
				while (enumerator.MoveNext())
				{
					XmlNodeSyntax current = enumerator.Current;
					XmlNameAttributeSyntax firstNameAttributeValue = GetFirstNameAttributeValue(current, symbolName, ERRID.WRN_XMLDocBadGenericParamTag2, ERRID.ERR_None);
					if (firstNameAttributeValue != null)
					{
						CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = binder.GetNewCompoundUseSiteInfo(_diagnostics);
						ImmutableArray<Symbol> immutableArray = binder.BindXmlNameAttributeValue(firstNameAttributeValue.Reference, ref useSiteInfo);
						if (SyntaxExtensions.ReportDocumentationCommentDiagnostics(current.SyntaxTree))
						{
							((BindingDiagnosticBag<AssemblySymbol>)_diagnostics).Add((SyntaxNode)current, useSiteInfo);
						}
						else
						{
							_diagnostics.AddDependencies(useSiteInfo);
						}
						bool flag = true;
						if (!immutableArray.IsDefault && immutableArray.Length == 1)
						{
							flag = immutableArray[0].Kind != SymbolKind.TypeParameter;
						}
						if (flag && SyntaxExtensions.ReportDocumentationCommentDiagnostics(current.SyntaxTree))
						{
							_diagnostics.Add(ERRID.WRN_XMLDocBadGenericParamTag2, current.GetLocation(), firstNameAttributeValue.Reference.Identifier.ValueText, symbolName);
						}
					}
				}
			}

			private void ReportWarningsForParamOrTypeParamTags<TSymbol>(Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes, WellKnownTag tag, WellKnownTag tagRef, string symbolName, ERRID badNameValueError, ERRID missingNameValueError, ImmutableArray<TSymbol> allowedSymbols) where TSymbol : Symbol
			{
				ArrayBuilder<XmlNodeSyntax> value = null;
				wellKnownElementNodes.TryGetValue(tag, out value);
				ArrayBuilder<XmlNodeSyntax> value2 = null;
				if (tagRef != 0)
				{
					wellKnownElementNodes.TryGetValue(tagRef, out value2);
				}
				if (value == null && value2 == null)
				{
					return;
				}
				HashSet<string> hashSet = null;
				if (allowedSymbols.Length > 10)
				{
					hashSet = new HashSet<string>(CaseInsensitiveComparison.Comparer);
					ImmutableArray<TSymbol>.Enumerator enumerator = allowedSymbols.GetEnumerator();
					while (enumerator.MoveNext())
					{
						TSymbol current = enumerator.Current;
						hashSet.Add(current.Name);
					}
				}
				if (value != null)
				{
					ReportWarningsForParamOrTypeParamTags(value, symbolName, badNameValueError, missingNameValueError, allowedSymbols, hashSet);
				}
				if (value2 != null)
				{
					ReportWarningsForParamOrTypeParamTags(value2, symbolName, badNameValueError, ERRID.ERR_None, allowedSymbols, hashSet);
				}
			}

			private void ReportWarningsForParamOrTypeParamTags<TSymbol>(ArrayBuilder<XmlNodeSyntax> builder, string symbolName, ERRID badNameValueError, ERRID missingNameValueError, ImmutableArray<TSymbol> allowedSymbols, HashSet<string> set) where TSymbol : Symbol
			{
				ArrayBuilder<XmlNodeSyntax>.Enumerator enumerator = builder.GetEnumerator();
				while (enumerator.MoveNext())
				{
					XmlNodeSyntax current = enumerator.Current;
					XmlNameAttributeSyntax firstNameAttributeValue = GetFirstNameAttributeValue(current, symbolName, badNameValueError, missingNameValueError);
					if (firstNameAttributeValue == null)
					{
						continue;
					}
					string valueText = firstNameAttributeValue.Reference.Identifier.ValueText;
					bool flag = true;
					if (set == null)
					{
						ImmutableArray<TSymbol>.Enumerator enumerator2 = allowedSymbols.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							TSymbol current2 = enumerator2.Current;
							if (CaseInsensitiveComparison.Equals(valueText, current2.Name))
							{
								flag = false;
								break;
							}
						}
					}
					else
					{
						flag = !set.Contains(valueText);
					}
					if (flag && SyntaxExtensions.ReportDocumentationCommentDiagnostics(current.SyntaxTree))
					{
						_diagnostics.Add(badNameValueError, current.GetLocation(), valueText.Trim(), symbolName);
					}
				}
			}

			private static void FreeWellKnownElementNodes(Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes)
			{
				foreach (ArrayBuilder<XmlNodeSyntax> value in wellKnownElementNodes.Values)
				{
					value.Free();
				}
			}

			private string GetDocumentationCommentForSymbol(Symbol symbol, DocumentationCommentTriviaSyntax trivia, Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes)
			{
				if (!IsInSemanticModelMode)
				{
					if (trivia.ContainsDiagnostics)
					{
						return null;
					}
					foreach (SyntaxNode item in trivia.ChildNodes())
					{
						if (item.ContainsDiagnostics)
						{
							return null;
						}
					}
				}
				string substitutedText = DocumentationCommentWalker.GetSubstitutedText(symbol, trivia, wellKnownElementNodes, _diagnostics);
				if (substitutedText == null)
				{
					return null;
				}
				string text = FormatComment(substitutedText);
				string text2 = null;
				if (_processIncludes)
				{
					ArrayBuilder<XmlNodeSyntax> value = null;
					wellKnownElementNodes.TryGetValue(WellKnownTag.Include, out value);
					text2 = IncludeElementExpander.ProcessIncludes(text, symbol, value, _compilation, _filterSyntaxTree, _filterSpanWithinTree, ref _includedFileCache, _diagnostics, _cancellationToken);
				}
				else
				{
					text2 = text;
				}
				if (IsInSemanticModelMode)
				{
					return text2;
				}
				XmlException ex = XmlDocumentationCommentTextReader.ParseAndGetException(text2);
				if (ex != null)
				{
					if (SyntaxExtensions.ReportDocumentationCommentDiagnostics(trivia.SyntaxTree))
					{
						_diagnostics.Add(ERRID.WRN_XMLDocParseError1, trivia.GetLocation(), GetDescription(ex));
					}
					return null;
				}
				return text2;
			}

			private void WriteDocumentationCommentForSymbol(string xmlDocComment)
			{
				if (!_isForSingleSymbol)
				{
					Write(xmlDocComment);
					return;
				}
				int num = xmlDocComment.Length;
				if (num - 1 > 0 && EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(xmlDocComment[num - 1]), "\n", TextCompare: false) == 0)
				{
					num--;
					if (num - 1 > 0 && EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(xmlDocComment[num - 1]), "\r", TextCompare: false) == 0)
					{
						num--;
					}
				}
				_writer.WriteSubString(xmlDocComment, 0, num, appendNewLine: false);
			}

			private static SyntaxList<XmlNodeSyntax> GetXmlElementAttributes(XmlNodeSyntax element)
			{
				return element.Kind() switch
				{
					SyntaxKind.XmlEmptyElement => ((XmlEmptyElementSyntax)element).Attributes, 
					SyntaxKind.XmlElement => GetXmlElementAttributes(((XmlElementSyntax)element).StartTag), 
					SyntaxKind.XmlElementStartTag => ((XmlElementStartTagSyntax)element).Attributes, 
					_ => default(SyntaxList<XmlNodeSyntax>), 
				};
			}

			private XmlNameAttributeSyntax GetFirstNameAttributeValue(XmlNodeSyntax element, string symbolName, ERRID badNameValueError, ERRID missingNameValueError)
			{
				SyntaxList<XmlNodeSyntax>.Enumerator enumerator = GetXmlElementAttributes(element).GetEnumerator();
				while (enumerator.MoveNext())
				{
					XmlNodeSyntax current = enumerator.Current;
					if (current.Kind() == SyntaxKind.XmlNameAttribute)
					{
						return (XmlNameAttributeSyntax)current;
					}
					if (current.Kind() != SyntaxKind.XmlAttribute)
					{
						continue;
					}
					XmlAttributeSyntax xmlAttributeSyntax = (XmlAttributeSyntax)current;
					XmlNodeSyntax name = xmlAttributeSyntax.Name;
					if (name.Kind() == SyntaxKind.XmlName && DocumentationCommentXmlNames.AttributeEquals(((XmlNameSyntax)name).LocalName.ValueText, "name"))
					{
						if (SyntaxExtensions.ReportDocumentationCommentDiagnostics(element.SyntaxTree))
						{
							XmlNodeSyntax value = xmlAttributeSyntax.Value;
							string text = ((value.Kind() == SyntaxKind.XmlString) ? Binder.GetXmlString(((XmlStringSyntax)value).TextTokens) : value.ToString());
							_diagnostics.Add(badNameValueError, current.GetLocation(), text, symbolName);
						}
						return null;
					}
				}
				if (missingNameValueError != 0 && SyntaxExtensions.ReportDocumentationCommentDiagnostics(element.SyntaxTree))
				{
					_diagnostics.Add(missingNameValueError, element.GetLocation());
				}
				return null;
			}

			private DocumentationCommentTriviaSyntax TryGetDocCommentTriviaAndGenerateDiagnostics(SyntaxNode syntaxNode)
			{
				DocumentationCommentTriviaSyntax documentationCommentTriviaSyntax = null;
				bool flag = false;
				SyntaxTriviaList.Enumerator enumerator = syntaxNode.GetLeadingTrivia().GetEnumerator();
				while (enumerator.MoveNext())
				{
					SyntaxTrivia current = enumerator.Current;
					switch (VisualBasicExtensions.Kind(current))
					{
					case SyntaxKind.DocumentationCommentTrivia:
						if (documentationCommentTriviaSyntax != null && SyntaxExtensions.ReportDocumentationCommentDiagnostics((VisualBasicSyntaxTree)current.SyntaxTree))
						{
							_diagnostics.Add(ERRID.WRN_XMLDocMoreThanOneCommentBlock, documentationCommentTriviaSyntax.GetLocation());
						}
						documentationCommentTriviaSyntax = (DocumentationCommentTriviaSyntax)current.GetStructure();
						flag = false;
						break;
					case SyntaxKind.CommentTrivia:
						flag = true;
						break;
					}
				}
				if (documentationCommentTriviaSyntax == null)
				{
					return null;
				}
				if (flag)
				{
					if (SyntaxExtensions.ReportDocumentationCommentDiagnostics(documentationCommentTriviaSyntax.SyntaxTree))
					{
						_diagnostics.Add(ERRID.WRN_XMLDocBadXMLLine, documentationCommentTriviaSyntax.GetLocation());
					}
					return null;
				}
				return documentationCommentTriviaSyntax;
			}

			private static string GetSymbolName(Symbol symbol)
			{
				switch (symbol.Kind)
				{
				case SymbolKind.Field:
				{
					Symbol associatedSymbol = ((FieldSymbol)symbol).AssociatedSymbol;
					return ((object)associatedSymbol != null && SymbolExtensions.IsWithEventsProperty(associatedSymbol)) ? "WithEvents variable" : "variable";
				}
				case SymbolKind.Method:
				{
					MethodSymbol methodSymbol = (MethodSymbol)symbol;
					return (methodSymbol.MethodKind == MethodKind.DeclareMethod) ? "declare" : ((methodSymbol.MethodKind == MethodKind.UserDefinedOperator || methodSymbol.MethodKind == MethodKind.Conversion) ? "operator" : (((MethodSymbol)symbol).IsSub ? "sub" : "function"));
				}
				case SymbolKind.Property:
					return "property";
				case SymbolKind.Event:
					return "event";
				case SymbolKind.NamedType:
					return ((NamedTypeSymbol)symbol).TypeKind switch
					{
						TypeKind.Class => "class", 
						TypeKind.Delegate => "delegate", 
						TypeKind.Enum => "enum", 
						TypeKind.Interface => "interface", 
						TypeKind.Module => "module", 
						TypeKind.Struct => "structure", 
						_ => throw ExceptionUtilities.UnexpectedValue(((NamedTypeSymbol)symbol).TypeKind), 
					};
				default:
					throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
				}
			}

			private static Binder CreateDocumentationCommentBinderForSymbol(SourceModuleSymbol module, Symbol sym, SyntaxTree tree, DocumentationCommentBinder.BinderType binderType)
			{
				Binder containingBinder;
				switch (sym.Kind)
				{
				case SymbolKind.NamedType:
				{
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)sym;
					if (namedTypeSymbol.TypeKind != TypeKind.Delegate)
					{
						containingBinder = BinderBuilder.CreateBinderForType(module, tree, namedTypeSymbol);
						break;
					}
					NamespaceOrTypeSymbol containingNamespaceOrType = namedTypeSymbol.ContainingNamespaceOrType;
					containingBinder = (containingNamespaceOrType.IsNamespace ? BinderBuilder.CreateBinderForNamespace(module, tree, (NamespaceSymbol)containingNamespaceOrType) : BinderBuilder.CreateBinderForType(module, tree, (NamedTypeSymbol)containingNamespaceOrType));
					break;
				}
				case SymbolKind.Event:
				case SymbolKind.Field:
				case SymbolKind.Method:
				case SymbolKind.Property:
					containingBinder = BinderBuilder.CreateBinderForType(module, tree, sym.ContainingType);
					break;
				default:
					return null;
				}
				return BinderBuilder.CreateBinderForDocumentationComment(containingBinder, sym, binderType);
			}

			public override void VisitEvent(EventSymbol symbol)
			{
				CancellationToken cancellationToken = _cancellationToken;
				cancellationToken.ThrowIfCancellationRequested();
				if (!ShouldSkipSymbol(symbol) && symbol is SourceEventSymbol @event)
				{
					WriteDocumentationCommentForEvent(@event);
				}
			}

			private void WriteDocumentationCommentForEvent(SourceEventSymbol @event)
			{
				DocumentationCommentTriviaSyntax documentationCommentTriviaSyntax = TryGetDocCommentTriviaAndGenerateDiagnostics(VisualBasicExtensions.GetVisualBasicSyntax(@event.SyntaxReference, _cancellationToken));
				if (documentationCommentTriviaSyntax == null)
				{
					return;
				}
				Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes = new Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>>();
				string documentationCommentForSymbol = GetDocumentationCommentForSymbol(@event, documentationCommentTriviaSyntax, wellKnownElementNodes);
				if (documentationCommentForSymbol == null)
				{
					FreeWellKnownElementNodes(wellKnownElementNodes);
					return;
				}
				if (SyntaxExtensions.ReportDocumentationCommentDiagnostics(documentationCommentTriviaSyntax.SyntaxTree) || _writer.IsSpecified)
				{
					string symbolName = GetSymbolName(@event);
					ReportWarningsForDuplicatedTags(wellKnownElementNodes, isEvent: true);
					ReportWarningsForExceptionTags(wellKnownElementNodes);
					ReportIllegalWellKnownTagIfAny(WellKnownTag.Returns, wellKnownElementNodes, symbolName);
					ImmutableArray<ParameterSymbol> parameters = ImmutableArray<ParameterSymbol>.Empty;
					if (@event.Type is NamedTypeSymbol namedTypeSymbol)
					{
						MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
						if ((object)delegateInvokeMethod != null)
						{
							parameters = delegateInvokeMethod.Parameters;
						}
					}
					ReportWarningsForParamAndParamRefTags(wellKnownElementNodes, symbolName, parameters);
					ReportIllegalWellKnownTagIfAny(WellKnownTag.Value, wellKnownElementNodes, symbolName);
					ReportIllegalWellKnownTagIfAny(WellKnownTag.TypeParam, wellKnownElementNodes, symbolName);
					ReportWarningsForTypeParamRefTags(wellKnownElementNodes, symbolName, @event, documentationCommentTriviaSyntax.SyntaxTree);
				}
				FreeWellKnownElementNodes(wellKnownElementNodes);
				WriteDocumentationCommentForSymbol(documentationCommentForSymbol);
			}

			public override void VisitField(FieldSymbol symbol)
			{
				CancellationToken cancellationToken = _cancellationToken;
				cancellationToken.ThrowIfCancellationRequested();
				if (!ShouldSkipSymbol(symbol) && symbol is SourceFieldSymbol field)
				{
					WriteDocumentationCommentForField(field);
				}
			}

			private void WriteDocumentationCommentForField(SourceFieldSymbol field)
			{
				DocumentationCommentTriviaSyntax documentationCommentTriviaSyntax = TryGetDocCommentTriviaAndGenerateDiagnostics(field.DeclarationSyntax);
				if (documentationCommentTriviaSyntax == null)
				{
					return;
				}
				Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes = new Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>>();
				string documentationCommentForSymbol = GetDocumentationCommentForSymbol(field, documentationCommentTriviaSyntax, wellKnownElementNodes);
				if (documentationCommentForSymbol == null)
				{
					FreeWellKnownElementNodes(wellKnownElementNodes);
					return;
				}
				if (SyntaxExtensions.ReportDocumentationCommentDiagnostics(documentationCommentTriviaSyntax.SyntaxTree) || _writer.IsSpecified)
				{
					string symbolName = GetSymbolName(field);
					ReportWarningsForDuplicatedTags(wellKnownElementNodes);
					ReportIllegalWellKnownTagIfAny(WellKnownTag.Exception, wellKnownElementNodes, symbolName);
					ReportIllegalWellKnownTagIfAny(WellKnownTag.Returns, wellKnownElementNodes, symbolName);
					ReportIllegalWellKnownTagIfAny(WellKnownTag.Param, wellKnownElementNodes, symbolName);
					ReportIllegalWellKnownTagIfAny(WellKnownTag.ParamRef, wellKnownElementNodes, symbolName);
					ReportIllegalWellKnownTagIfAny(WellKnownTag.Value, wellKnownElementNodes, symbolName);
					ReportIllegalWellKnownTagIfAny(WellKnownTag.TypeParam, wellKnownElementNodes, symbolName);
					ReportWarningsForTypeParamRefTags(wellKnownElementNodes, symbolName, field, documentationCommentTriviaSyntax.SyntaxTree);
				}
				FreeWellKnownElementNodes(wellKnownElementNodes);
				WriteDocumentationCommentForSymbol(documentationCommentForSymbol);
			}

			public override void VisitMethod(MethodSymbol symbol)
			{
				CancellationToken cancellationToken = _cancellationToken;
				cancellationToken.ThrowIfCancellationRequested();
				if (!ShouldSkipSymbol(symbol))
				{
					SourceMethodSymbol sourceMethodSymbol = (SourceMethodSymbol)(((object)(symbol as SourceMemberMethodSymbol)) ?? ((object)(symbol as SourceDeclareMethodSymbol)));
					if ((object)sourceMethodSymbol != null)
					{
						WriteDocumentationCommentForMethod(sourceMethodSymbol);
					}
				}
			}

			private bool WriteDocumentationCommentForMethod(SourceMethodSymbol method)
			{
				if (method.PartialImplementationPart is SourceMethodSymbol method2 && WriteDocumentationCommentForMethod(method2))
				{
					return true;
				}
				DocumentationCommentTriviaSyntax documentationCommentTriviaSyntax = TryGetDocCommentTriviaAndGenerateDiagnostics(method.Syntax);
				if (documentationCommentTriviaSyntax == null)
				{
					return false;
				}
				Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes = new Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>>();
				string documentationCommentForSymbol = GetDocumentationCommentForSymbol(method, documentationCommentTriviaSyntax, wellKnownElementNodes);
				if (documentationCommentForSymbol == null)
				{
					FreeWellKnownElementNodes(wellKnownElementNodes);
					return false;
				}
				if (SyntaxExtensions.ReportDocumentationCommentDiagnostics(documentationCommentTriviaSyntax.SyntaxTree) || _writer.IsSpecified)
				{
					string symbolName = GetSymbolName(method);
					ReportWarningsForDuplicatedTags(wellKnownElementNodes);
					ReportWarningsForExceptionTags(wellKnownElementNodes);
					if (method.IsSub)
					{
						if (method.MethodKind == MethodKind.DeclareMethod)
						{
							ReportIllegalWellKnownTagIfAny(WellKnownTag.Returns, ERRID.WRN_XMLDocReturnsOnADeclareSub, wellKnownElementNodes);
						}
						else
						{
							ReportIllegalWellKnownTagIfAny(WellKnownTag.Returns, wellKnownElementNodes, symbolName);
						}
					}
					ReportWarningsForParamAndParamRefTags(wellKnownElementNodes, symbolName, method.Parameters);
					ReportIllegalWellKnownTagIfAny(WellKnownTag.Value, wellKnownElementNodes, symbolName);
					if (method.MethodKind == MethodKind.UserDefinedOperator || method.MethodKind == MethodKind.DeclareMethod)
					{
						ReportIllegalWellKnownTagIfAny(WellKnownTag.TypeParam, wellKnownElementNodes, symbolName);
					}
					else
					{
						ReportWarningsForTypeParamTags(wellKnownElementNodes, symbolName, method.TypeParameters);
					}
					ReportWarningsForTypeParamRefTags(wellKnownElementNodes, symbolName, method, documentationCommentTriviaSyntax.SyntaxTree);
				}
				FreeWellKnownElementNodes(wellKnownElementNodes);
				WriteDocumentationCommentForSymbol(documentationCommentForSymbol);
				return true;
			}

			public override void VisitNamedType(NamedTypeSymbol symbol)
			{
				CancellationToken cancellationToken = _cancellationToken;
				cancellationToken.ThrowIfCancellationRequested();
				if (ShouldSkipSymbol(symbol))
				{
					return;
				}
				if (symbol is SourceNamedTypeSymbol namedType)
				{
					WriteDocumentationCommentForNamedType(namedType);
				}
				if (!_isForSingleSymbol)
				{
					ImmutableArray<Symbol>.Enumerator enumerator = symbol.GetMembers().GetEnumerator();
					while (enumerator.MoveNext())
					{
						Symbol current = enumerator.Current;
						Visit(current);
					}
				}
			}

			private void WriteDocumentationCommentForNamedType(SourceNamedTypeSymbol namedType)
			{
				ArrayBuilder<DocumentationCommentTriviaSyntax> instance = ArrayBuilder<DocumentationCommentTriviaSyntax>.GetInstance();
				DocumentationMode documentationMode = DocumentationMode.None;
				ImmutableArray<SyntaxReference>.Enumerator enumerator = namedType.SyntaxReferences.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SyntaxReference current = enumerator.Current;
					DocumentationCommentTriviaSyntax documentationCommentTriviaSyntax = TryGetDocCommentTriviaAndGenerateDiagnostics(VisualBasicExtensions.GetVisualBasicSyntax(current, _cancellationToken));
					if (documentationCommentTriviaSyntax != null)
					{
						instance.Add(documentationCommentTriviaSyntax);
						DocumentationMode documentationMode2 = documentationCommentTriviaSyntax.SyntaxTree.Options.DocumentationMode;
						if (documentationMode < documentationMode2)
						{
							documentationMode = documentationMode2;
						}
					}
				}
				string symbolName = GetSymbolName(namedType);
				if (instance.Count > 1)
				{
					if (documentationMode == DocumentationMode.Diagnose)
					{
						ArrayBuilder<DocumentationCommentTriviaSyntax>.Enumerator enumerator2 = instance.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							DocumentationCommentTriviaSyntax current2 = enumerator2.Current;
							_diagnostics.Add(ERRID.WRN_XMLDocOnAPartialType, current2.GetLocation(), symbolName);
						}
					}
					instance.Free();
					return;
				}
				if (instance.Count == 0)
				{
					instance.Free();
					return;
				}
				Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes = new Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>>();
				DocumentationCommentTriviaSyntax documentationCommentTriviaSyntax2 = instance[0];
				instance.Free();
				string documentationCommentForSymbol = GetDocumentationCommentForSymbol(namedType, documentationCommentTriviaSyntax2, wellKnownElementNodes);
				if (documentationCommentForSymbol == null)
				{
					FreeWellKnownElementNodes(wellKnownElementNodes);
					return;
				}
				if (SyntaxExtensions.ReportDocumentationCommentDiagnostics(documentationCommentTriviaSyntax2.SyntaxTree) || _writer.IsSpecified)
				{
					MethodSymbol delegateInvokeMethod = namedType.DelegateInvokeMethod;
					ReportWarningsForDuplicatedTags(wellKnownElementNodes);
					ReportIllegalWellKnownTagIfAny(WellKnownTag.Exception, wellKnownElementNodes, symbolName);
					if (namedType.TypeKind == TypeKind.Delegate)
					{
						if ((object)delegateInvokeMethod != null && delegateInvokeMethod.IsSub)
						{
							ReportIllegalWellKnownTagIfAny(WellKnownTag.Returns, wellKnownElementNodes, "delegate sub");
						}
					}
					else
					{
						ReportIllegalWellKnownTagIfAny(WellKnownTag.Returns, wellKnownElementNodes, symbolName);
					}
					if (namedType.TypeKind == TypeKind.Delegate)
					{
						ReportWarningsForParamAndParamRefTags(wellKnownElementNodes, GetSymbolName(delegateInvokeMethod), delegateInvokeMethod.Parameters);
					}
					else
					{
						ReportIllegalWellKnownTagIfAny(WellKnownTag.Param, wellKnownElementNodes, symbolName);
						ReportIllegalWellKnownTagIfAny(WellKnownTag.ParamRef, wellKnownElementNodes, symbolName);
					}
					ReportIllegalWellKnownTagIfAny(WellKnownTag.Value, wellKnownElementNodes, symbolName);
					if (namedType.TypeKind == TypeKind.Enum)
					{
						ReportIllegalWellKnownTagIfAny(WellKnownTag.TypeParam, wellKnownElementNodes, symbolName);
						ReportWarningsForTypeParamRefTags(wellKnownElementNodes, symbolName, namedType, documentationCommentTriviaSyntax2.SyntaxTree);
					}
					else if (namedType.TypeKind == TypeKind.Enum || namedType.TypeKind == TypeKind.Module)
					{
						ReportIllegalWellKnownTagIfAny(WellKnownTag.TypeParam, wellKnownElementNodes, symbolName);
						ReportIllegalWellKnownTagIfAny(WellKnownTag.TypeParamRef, wellKnownElementNodes, symbolName);
					}
					else
					{
						ReportWarningsForTypeParamTags(wellKnownElementNodes, symbolName, namedType.TypeParameters);
						ReportWarningsForTypeParamRefTags(wellKnownElementNodes, symbolName, namedType, documentationCommentTriviaSyntax2.SyntaxTree);
					}
				}
				FreeWellKnownElementNodes(wellKnownElementNodes);
				WriteDocumentationCommentForSymbol(documentationCommentForSymbol);
			}

			public override void VisitNamespace(NamespaceSymbol symbol)
			{
				CancellationToken cancellationToken = _cancellationToken;
				cancellationToken.ThrowIfCancellationRequested();
				if (ShouldSkipSymbol(symbol))
				{
					return;
				}
				if (symbol.IsGlobalNamespace)
				{
					WriteLine("<?xml version=\"1.0\"?>");
					WriteLine("<doc>");
					Indent();
					if (!_compilation.Options.OutputKind.IsNetModule())
					{
						WriteLine("<assembly>");
						Indent();
						WriteLine("<name>");
						WriteLine(_assemblyName);
						WriteLine("</name>");
						Unindent();
						WriteLine("</assembly>");
					}
					WriteLine("<members>");
					Indent();
				}
				ImmutableArray<Symbol>.Enumerator enumerator = symbol.GetMembers().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					Visit(current);
				}
				if (symbol.IsGlobalNamespace)
				{
					Unindent();
					WriteLine("</members>");
					Unindent();
					WriteLine("</doc>");
				}
			}

			public override void VisitProperty(PropertySymbol symbol)
			{
				CancellationToken cancellationToken = _cancellationToken;
				cancellationToken.ThrowIfCancellationRequested();
				if (!ShouldSkipSymbol(symbol) && symbol is SourcePropertySymbol property)
				{
					WriteDocumentationCommentForProperty(property);
				}
			}

			private void WriteDocumentationCommentForProperty(SourcePropertySymbol property)
			{
				if (SymbolExtensions.IsWithEventsProperty(property))
				{
					return;
				}
				DocumentationCommentTriviaSyntax documentationCommentTriviaSyntax = TryGetDocCommentTriviaAndGenerateDiagnostics(VisualBasicExtensions.GetVisualBasicSyntax(property.BlockSyntaxReference ?? property.SyntaxReference, _cancellationToken));
				if (documentationCommentTriviaSyntax == null)
				{
					return;
				}
				Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>> wellKnownElementNodes = new Dictionary<WellKnownTag, ArrayBuilder<XmlNodeSyntax>>();
				string documentationCommentForSymbol = GetDocumentationCommentForSymbol(property, documentationCommentTriviaSyntax, wellKnownElementNodes);
				if (documentationCommentForSymbol == null)
				{
					FreeWellKnownElementNodes(wellKnownElementNodes);
					return;
				}
				if (SyntaxExtensions.ReportDocumentationCommentDiagnostics(documentationCommentTriviaSyntax.SyntaxTree) || _writer.IsSpecified)
				{
					string symbolName = GetSymbolName(property);
					ReportWarningsForDuplicatedTags(wellKnownElementNodes);
					ReportWarningsForExceptionTags(wellKnownElementNodes);
					if (property.IsWriteOnly)
					{
						ReportIllegalWellKnownTagIfAny(WellKnownTag.Returns, ERRID.WRN_XMLDocReturnsOnWriteOnlyProperty, wellKnownElementNodes);
					}
					ReportWarningsForParamAndParamRefTags(wellKnownElementNodes, symbolName, property.Parameters);
					ReportIllegalWellKnownTagIfAny(WellKnownTag.TypeParam, wellKnownElementNodes, symbolName);
					ReportWarningsForTypeParamRefTags(wellKnownElementNodes, symbolName, property, documentationCommentTriviaSyntax.SyntaxTree);
				}
				FreeWellKnownElementNodes(wellKnownElementNodes);
				WriteDocumentationCommentForSymbol(documentationCommentForSymbol);
			}

			private void Indent()
			{
				_writer.Indent();
			}

			private void Unindent()
			{
				_writer.Unindent();
			}

			private void WriteLine(string message)
			{
				_writer.WriteLine(message);
			}

			private void Write(string message)
			{
				_writer.Write(message);
			}

			private string FormatComment(string substitutedText)
			{
				_writer.BeginTemporaryString();
				WriteFormattedComment(substitutedText);
				return _writer.GetAndEndTemporaryString();
			}

			private static int GetIndexOfFirstNonWhitespaceChar(string str)
			{
				return GetIndexOfFirstNonWhitespaceChar(str, 0, str.Length);
			}

			private static int GetIndexOfFirstNonWhitespaceChar(string str, int start, int end)
			{
				while ((start < end) & char.IsWhiteSpace(str[start]))
				{
					start++;
				}
				return start;
			}

			private static bool TrimmedStringStartsWith(string str, string prefix)
			{
				int indexOfFirstNonWhitespaceChar = GetIndexOfFirstNonWhitespaceChar(str);
				if (str.Length - indexOfFirstNonWhitespaceChar < prefix.Length)
				{
					return false;
				}
				int num = 0;
				int num2 = indexOfFirstNonWhitespaceChar;
				while (num < prefix.Length)
				{
					if (prefix[num] != str[num2])
					{
						return false;
					}
					num++;
					num2++;
				}
				return true;
			}

			private static int IndexOfNewLine(string str, int start, out int newLineLength)
			{
				int length = str.Length;
				while (start < length)
				{
					switch (str[start])
					{
					case '\r':
						if (start + 1 < str.Length && str[start + 1] == '\n')
						{
							newLineLength = 2;
						}
						else
						{
							newLineLength = 1;
						}
						return start;
					case '\n':
						newLineLength = 1;
						return start;
					}
					start++;
				}
				newLineLength = 0;
				return start;
			}

			private void WriteFormattedComment(string text)
			{
				int num = 3;
				int num2 = 0;
				int length = text.Length;
				while (num2 < length)
				{
					int newLineLength = 0;
					int num3 = IndexOfNewLine(text, num2, out newLineLength);
					int num4 = GetIndexOfFirstNonWhitespaceChar(text, num2, num3);
					if (num4 < num3 && text[num4] == '\'')
					{
						num4 += num;
					}
					_writer.WriteSubString(text, num4, num3 - num4);
					num2 = num3 + newLineLength;
				}
			}

			private static string GetDescription(XmlException e)
			{
				string message = e.Message;
				try
				{
					string text = string.Format(new ResourceManager("System.Xml", typeof(XmlException).GetTypeInfo().Assembly).GetString("Xml_MessageWithErrorPosition"), "", e.LineNumber, e.LinePosition);
					int num = message.IndexOf(text, StringComparison.Ordinal);
					return (num < 0) ? message : message.Remove(num, text.Length);
				}
				catch (Exception ex)
				{
					ProjectData.SetProjectError(ex);
					Exception ex2 = ex;
					string result = message;
					ProjectData.ClearProjectError();
					return result;
				}
			}

			private DocumentationCommentCompiler(string assemblyName, VisualBasicCompilation compilation, TextWriter writer, bool processIncludes, bool isForSingleSymbol, BindingDiagnosticBag diagnostics, SyntaxTree filterTree, TextSpan? filterSpanWithinTree, CultureInfo preferredCulture, CancellationToken cancellationToken)
			{
				_assemblyName = assemblyName;
				_compilation = compilation;
				_writer = new DocWriter(writer);
				_processIncludes = processIncludes;
				_isForSingleSymbol = isForSingleSymbol;
				_diagnostics = diagnostics;
				_filterSyntaxTree = filterTree;
				_filterSpanWithinTree = filterSpanWithinTree;
				_cancellationToken = cancellationToken;
			}

			internal static void WriteDocumentationCommentXml(VisualBasicCompilation compilation, string assemblyName, Stream xmlDocStream, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken, SyntaxTree filterTree = null, TextSpan? filterSpanWithinTree = null)
			{
				StreamWriter streamWriter = null;
				if (xmlDocStream != null && xmlDocStream.CanWrite)
				{
					streamWriter = new StreamWriter(xmlDocStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true, throwOnInvalidBytes: false), 1024, leaveOpen: true);
				}
				try
				{
					using (streamWriter)
					{
						new DocumentationCommentCompiler(assemblyName ?? compilation.SourceAssembly.Name, compilation, streamWriter, processIncludes: true, isForSingleSymbol: false, diagnostics, filterTree, filterSpanWithinTree, null, cancellationToken).Visit(compilation.SourceAssembly.GlobalNamespace);
						streamWriter?.Flush();
					}
				}
				catch (Exception ex)
				{
					ProjectData.SetProjectError(ex);
					Exception ex2 = ex;
					diagnostics.Add(ERRID.ERR_DocFileGen, Location.None, ex2.Message);
					ProjectData.ClearProjectError();
				}
				if (!diagnostics.AccumulatesDiagnostics)
				{
					return;
				}
				if (filterTree != null)
				{
					MislocatedDocumentationCommentFinder.ReportUnprocessed(filterTree, filterSpanWithinTree, diagnostics.DiagnosticBag, cancellationToken);
					return;
				}
				ImmutableArray<SyntaxTree>.Enumerator enumerator = compilation.SyntaxTrees.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MislocatedDocumentationCommentFinder.ReportUnprocessed(enumerator.Current, null, diagnostics.DiagnosticBag, cancellationToken);
				}
			}

			internal static string GetDocumentationCommentXml(Symbol symbol, bool processIncludes, CultureInfo preferredCulture, CancellationToken cancellationToken)
			{
				VisualBasicCompilation declaringCompilation = symbol.DeclaringCompilation;
				PooledStringBuilder instance = PooledStringBuilder.GetInstance();
				StringWriter stringWriter = new StringWriter(instance.Builder, CultureInfo.InvariantCulture);
				new DocumentationCommentCompiler(null, declaringCompilation, stringWriter, processIncludes, isForSingleSymbol: true, BindingDiagnosticBag.Discarded, null, null, preferredCulture, cancellationToken).Visit(symbol);
				stringWriter.Dispose();
				return instance.ToStringAndFree();
			}
		}

		private struct EmbeddedTreeAndDeclaration
		{
			public readonly Lazy<SyntaxTree> Tree;

			public readonly DeclarationTableEntry DeclarationEntry;

			public EmbeddedTreeAndDeclaration(Func<SyntaxTree> treeOpt, Func<RootSingleNamespaceDeclaration> rootNamespaceOpt)
			{
				this = default(EmbeddedTreeAndDeclaration);
				Tree = new Lazy<SyntaxTree>(treeOpt);
				DeclarationEntry = new DeclarationTableEntry(new Lazy<RootSingleNamespaceDeclaration>(rootNamespaceOpt), isEmbedded: true);
			}
		}

		internal class EntryPoint
		{
			public readonly MethodSymbol MethodSymbol;

			public readonly ImmutableArray<Diagnostic> Diagnostics;

			public EntryPoint(MethodSymbol methodSymbol, ImmutableArray<Diagnostic> diagnostics)
			{
				MethodSymbol = methodSymbol;
				Diagnostics = diagnostics;
			}
		}

		private struct ImportInfo
		{
			public readonly SyntaxTree Tree;

			public readonly TextSpan StatementSpan;

			public readonly ImmutableArray<TextSpan> ClauseSpans;

			public ImportInfo(ImportsStatementSyntax syntax)
			{
				this = default(ImportInfo);
				Tree = syntax.SyntaxTree;
				StatementSpan = syntax.Span;
				ArrayBuilder<TextSpan> instance = ArrayBuilder<TextSpan>.GetInstance();
				SeparatedSyntaxList<ImportsClauseSyntax>.Enumerator enumerator = syntax.ImportsClauses.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ImportsClauseSyntax current = enumerator.Current;
					instance.Add(current.Span);
				}
				ClauseSpans = instance.ToImmutableAndFree();
			}
		}

		private abstract class AbstractSymbolSearcher
		{
			private readonly PooledDictionary<Declaration, NamespaceOrTypeSymbol> _cache;

			private readonly VisualBasicCompilation _compilation;

			private readonly bool _includeNamespace;

			private readonly bool _includeType;

			private readonly bool _includeMember;

			private readonly CancellationToken _cancellationToken;

			public AbstractSymbolSearcher(VisualBasicCompilation compilation, SymbolFilter filter, CancellationToken cancellationToken)
			{
				_cache = PooledDictionary<Declaration, NamespaceOrTypeSymbol>.GetInstance();
				_compilation = compilation;
				_includeNamespace = (filter & SymbolFilter.Namespace) == SymbolFilter.Namespace;
				_includeType = (filter & SymbolFilter.Type) == SymbolFilter.Type;
				_includeMember = (filter & SymbolFilter.Member) == SymbolFilter.Member;
				_cancellationToken = cancellationToken;
			}

			protected abstract bool Matches(string name);

			protected abstract bool ShouldCheckTypeForMembers(MergedTypeDeclaration typeDeclaration);

			public IEnumerable<ISymbol> GetSymbolsWithName()
			{
				HashSet<ISymbol> hashSet = new HashSet<ISymbol>();
				ArrayBuilder<MergedNamespaceOrTypeDeclaration> instance = ArrayBuilder<MergedNamespaceOrTypeDeclaration>.GetInstance();
				AppendSymbolsWithName(instance, _compilation.MergedRootDeclaration, hashSet);
				instance.Free();
				_cache.Free();
				return hashSet;
			}

			private void AppendSymbolsWithName(ArrayBuilder<MergedNamespaceOrTypeDeclaration> spine, MergedNamespaceOrTypeDeclaration current, HashSet<ISymbol> set)
			{
				if (current.Kind == DeclarationKind.Namespace)
				{
					if (_includeNamespace && Matches(current.Name))
					{
						NamespaceOrTypeSymbol spineSymbol = GetSpineSymbol(spine);
						NamespaceOrTypeSymbol symbol = GetSymbol(spineSymbol, current);
						if ((object)symbol != null)
						{
							set.Add(symbol);
						}
					}
				}
				else
				{
					if (_includeType && Matches(current.Name))
					{
						NamespaceOrTypeSymbol spineSymbol2 = GetSpineSymbol(spine);
						NamespaceOrTypeSymbol symbol2 = GetSymbol(spineSymbol2, current);
						if ((object)symbol2 != null)
						{
							set.Add(symbol2);
						}
					}
					if (_includeMember)
					{
						MergedTypeDeclaration mergedTypeDeclaration = (MergedTypeDeclaration)current;
						if (ShouldCheckTypeForMembers(mergedTypeDeclaration))
						{
							AppendMemberSymbolsWithName(spine, mergedTypeDeclaration, set);
						}
					}
				}
				spine.Add(current);
				ImmutableArray<Declaration>.Enumerator enumerator = current.Children.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Declaration current2 = enumerator.Current;
					if (current2 is MergedNamespaceOrTypeDeclaration current3 && (_includeMember || _includeType || current2.Kind == DeclarationKind.Namespace))
					{
						AppendSymbolsWithName(spine, current3, set);
					}
				}
				spine.RemoveAt(spine.Count - 1);
			}

			private void AppendMemberSymbolsWithName(ArrayBuilder<MergedNamespaceOrTypeDeclaration> spine, MergedTypeDeclaration mergedType, HashSet<ISymbol> set)
			{
				CancellationToken cancellationToken = _cancellationToken;
				cancellationToken.ThrowIfCancellationRequested();
				spine.Add(mergedType);
				NamespaceOrTypeSymbol namespaceOrTypeSymbol = null;
				foreach (string memberName in mergedType.MemberNames)
				{
					if (Matches(memberName))
					{
						namespaceOrTypeSymbol = namespaceOrTypeSymbol ?? GetSpineSymbol(spine);
						if ((object)namespaceOrTypeSymbol != null)
						{
							set.UnionWith(namespaceOrTypeSymbol.GetMembers(memberName));
						}
					}
				}
				spine.RemoveAt(spine.Count - 1);
			}

			private NamespaceOrTypeSymbol GetSpineSymbol(ArrayBuilder<MergedNamespaceOrTypeDeclaration> spine)
			{
				if (spine.Count == 0)
				{
					return null;
				}
				NamespaceOrTypeSymbol cachedSymbol = GetCachedSymbol(spine[spine.Count - 1]);
				if ((object)cachedSymbol != null)
				{
					return cachedSymbol;
				}
				NamespaceOrTypeSymbol namespaceOrTypeSymbol = _compilation.GlobalNamespace;
				int num = spine.Count - 1;
				for (int i = 1; i <= num; i++)
				{
					namespaceOrTypeSymbol = GetSymbol(namespaceOrTypeSymbol, spine[i]);
				}
				return namespaceOrTypeSymbol;
			}

			private NamespaceOrTypeSymbol GetCachedSymbol(MergedNamespaceOrTypeDeclaration declaration)
			{
				NamespaceOrTypeSymbol value = null;
				if (_cache.TryGetValue(declaration, out value))
				{
					return value;
				}
				return null;
			}

			private NamespaceOrTypeSymbol GetSymbol(NamespaceOrTypeSymbol container, MergedNamespaceOrTypeDeclaration declaration)
			{
				if ((object)container == null)
				{
					return _compilation.GlobalNamespace;
				}
				NamespaceOrTypeSymbol cachedSymbol = GetCachedSymbol(declaration);
				if ((object)cachedSymbol != null)
				{
					return cachedSymbol;
				}
				if (declaration.Kind == DeclarationKind.Namespace)
				{
					AddCache(container.GetMembers(declaration.Name).OfType<NamespaceOrTypeSymbol>());
				}
				else
				{
					AddCache(container.GetTypeMembers(declaration.Name));
				}
				return GetCachedSymbol(declaration);
			}

			private void AddCache(IEnumerable<NamespaceOrTypeSymbol> symbols)
			{
				foreach (NamespaceOrTypeSymbol symbol in symbols)
				{
					if (symbol is MergedNamespaceSymbol mergedNamespaceSymbol)
					{
						_cache[mergedNamespaceSymbol.ConstituentNamespaces.OfType<SourceNamespaceSymbol>().First().MergedDeclaration] = symbol;
					}
					else if (symbol is SourceNamespaceSymbol sourceNamespaceSymbol)
					{
						_cache[sourceNamespaceSymbol.MergedDeclaration] = sourceNamespaceSymbol;
					}
					else if (symbol is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol)
					{
						_cache[sourceMemberContainerTypeSymbol.TypeDeclaration] = sourceMemberContainerTypeSymbol;
					}
				}
			}
		}

		private class PredicateSymbolSearcher : AbstractSymbolSearcher
		{
			private readonly Func<string, bool> _predicate;

			public PredicateSymbolSearcher(VisualBasicCompilation compilation, SymbolFilter filter, Func<string, bool> predicate, CancellationToken cancellationToken)
				: base(compilation, filter, cancellationToken)
			{
				_predicate = predicate;
			}

			protected override bool ShouldCheckTypeForMembers(MergedTypeDeclaration current)
			{
				return true;
			}

			protected override bool Matches(string name)
			{
				return _predicate(name);
			}
		}

		private class NameSymbolSearcher : AbstractSymbolSearcher
		{
			private readonly string _name;

			public NameSymbolSearcher(VisualBasicCompilation compilation, SymbolFilter filter, string name, CancellationToken cancellationToken)
				: base(compilation, filter, cancellationToken)
			{
				_name = name;
			}

			protected override bool ShouldCheckTypeForMembers(MergedTypeDeclaration current)
			{
				ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = current.Declarations.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.MemberNames.Contains(_name))
					{
						return true;
					}
				}
				return false;
			}

			protected override bool Matches(string name)
			{
				return CaseInsensitiveComparison.Equals(_name, name);
			}
		}

		internal sealed class ReferenceManager : CommonReferenceManager<VisualBasicCompilation, AssemblySymbol>
		{
			private abstract class AssemblyDataForMetadataOrCompilation : AssemblyData
			{
				private List<AssemblySymbol> _assemblies;

				private readonly AssemblyIdentity _identity;

				private readonly ImmutableArray<AssemblyIdentity> _referencedAssemblies;

				private readonly bool _embedInteropTypes;

				public override AssemblyIdentity Identity => _identity;

				public override IEnumerable<AssemblySymbol> AvailableSymbols
				{
					get
					{
						if (_assemblies == null)
						{
							_assemblies = new List<AssemblySymbol>();
							AddAvailableSymbols(_assemblies);
						}
						return _assemblies;
					}
				}

				public override ImmutableArray<AssemblyIdentity> AssemblyReferences => _referencedAssemblies;

				public sealed override bool IsLinked => _embedInteropTypes;

				protected AssemblyDataForMetadataOrCompilation(AssemblyIdentity identity, ImmutableArray<AssemblyIdentity> referencedAssemblies, bool embedInteropTypes)
				{
					_embedInteropTypes = embedInteropTypes;
					_identity = identity;
					_referencedAssemblies = referencedAssemblies;
				}

				internal abstract AssemblySymbol CreateAssemblySymbol();

				protected abstract void AddAvailableSymbols(List<AssemblySymbol> assemblies);

				public override AssemblyReferenceBinding[] BindAssemblyReferences(ImmutableArray<AssemblyData> assemblies, AssemblyIdentityComparer assemblyIdentityComparer)
				{
					return CommonReferenceManager<VisualBasicCompilation, AssemblySymbol>.ResolveReferencedAssemblies(_referencedAssemblies, assemblies, 0, assemblyIdentityComparer);
				}
			}

			private sealed class AssemblyDataForFile : AssemblyDataForMetadataOrCompilation
			{
				public readonly PEAssembly Assembly;

				public readonly WeakList<IAssemblySymbolInternal> CachedSymbols;

				public readonly DocumentationProvider DocumentationProvider;

				private readonly MetadataImportOptions _compilationImportOptions;

				private readonly string _sourceAssemblySimpleName;

				private bool _internalsVisibleComputed;

				private bool _internalsPotentiallyVisibleToCompilation;

				internal bool InternalsMayBeVisibleToCompilation
				{
					get
					{
						if (!_internalsVisibleComputed)
						{
							_internalsPotentiallyVisibleToCompilation = CommonReferenceManager<VisualBasicCompilation, AssemblySymbol>.InternalsMayBeVisibleToAssemblyBeingCompiled(_sourceAssemblySimpleName, Assembly);
							_internalsVisibleComputed = true;
						}
						return _internalsPotentiallyVisibleToCompilation;
					}
				}

				internal MetadataImportOptions EffectiveImportOptions
				{
					get
					{
						if (InternalsMayBeVisibleToCompilation && _compilationImportOptions == MetadataImportOptions.Public)
						{
							return MetadataImportOptions.Internal;
						}
						return _compilationImportOptions;
					}
				}

				public override bool ContainsNoPiaLocalTypes => Assembly.ContainsNoPiaLocalTypes();

				public override bool DeclaresTheObjectClass => Assembly.DeclaresTheObjectClass;

				public override Compilation SourceCompilation => null;

				public AssemblyDataForFile(PEAssembly assembly, WeakList<IAssemblySymbolInternal> cachedSymbols, bool embedInteropTypes, DocumentationProvider documentationProvider, string sourceAssemblySimpleName, MetadataImportOptions compilationImportOptions)
					: base(assembly.Identity, assembly.AssemblyReferences, embedInteropTypes)
				{
					_internalsVisibleComputed = false;
					_internalsPotentiallyVisibleToCompilation = false;
					CachedSymbols = cachedSymbols;
					Assembly = assembly;
					DocumentationProvider = documentationProvider;
					_compilationImportOptions = compilationImportOptions;
					_sourceAssemblySimpleName = sourceAssemblySimpleName;
				}

				internal override AssemblySymbol CreateAssemblySymbol()
				{
					return new PEAssemblySymbol(Assembly, DocumentationProvider, base.IsLinked, EffectiveImportOptions);
				}

				protected override void AddAvailableSymbols(List<AssemblySymbol> assemblies)
				{
					_ = InternalsMayBeVisibleToCompilation;
					lock (CommonReferenceManager.SymbolCacheAndReferenceManagerStateGuard)
					{
						foreach (IAssemblySymbolInternal cachedSymbol in CachedSymbols)
						{
							PEAssemblySymbol pEAssemblySymbol = cachedSymbol as PEAssemblySymbol;
							if (IsMatchingAssembly(pEAssemblySymbol))
							{
								assemblies.Add(pEAssemblySymbol);
							}
						}
					}
				}

				public override bool IsMatchingAssembly(AssemblySymbol candidateAssembly)
				{
					return IsMatchingAssembly(candidateAssembly as PEAssemblySymbol);
				}

				private bool IsMatchingAssembly(PEAssemblySymbol peAssembly)
				{
					if ((object)peAssembly == null)
					{
						return false;
					}
					if (peAssembly.Assembly != Assembly)
					{
						return false;
					}
					if (EffectiveImportOptions != peAssembly.PrimaryModule.ImportOptions)
					{
						return false;
					}
					if (!peAssembly.DocumentationProvider.Equals(DocumentationProvider))
					{
						return false;
					}
					return true;
				}
			}

			private sealed class AssemblyDataForCompilation : AssemblyDataForMetadataOrCompilation
			{
				public readonly VisualBasicCompilation Compilation;

				public override bool ContainsNoPiaLocalTypes => Compilation.MightContainNoPiaLocalTypes();

				public override bool DeclaresTheObjectClass => Compilation.DeclaresTheObjectClass;

				public override Compilation SourceCompilation => Compilation;

				public AssemblyDataForCompilation(VisualBasicCompilation compilation, bool embedInteropTypes)
					: base(compilation.Assembly.Identity, GetReferencedAssemblies(compilation), embedInteropTypes)
				{
					Compilation = compilation;
				}

				private static ImmutableArray<AssemblyIdentity> GetReferencedAssemblies(VisualBasicCompilation compilation)
				{
					ArrayBuilder<AssemblyIdentity> instance = ArrayBuilder<AssemblyIdentity>.GetInstance();
					ImmutableArray<ModuleSymbol> modules = compilation.Assembly.Modules;
					ImmutableArray<AssemblyIdentity> referencedAssemblies = modules[0].GetReferencedAssemblies();
					ImmutableArray<AssemblySymbol> referencedAssemblySymbols = modules[0].GetReferencedAssemblySymbols();
					int num = referencedAssemblies.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						if (!referencedAssemblySymbols[i].IsLinked)
						{
							instance.Add(referencedAssemblies[i]);
						}
					}
					int num2 = modules.Length - 1;
					for (int j = 1; j <= num2; j++)
					{
						instance.AddRange(modules[j].GetReferencedAssemblies());
					}
					return instance.ToImmutableAndFree();
				}

				internal override AssemblySymbol CreateAssemblySymbol()
				{
					return new RetargetingAssemblySymbol(Compilation.SourceAssembly, base.IsLinked);
				}

				protected override void AddAvailableSymbols(List<AssemblySymbol> assemblies)
				{
					assemblies.Add(Compilation.Assembly);
					lock (CommonReferenceManager.SymbolCacheAndReferenceManagerStateGuard)
					{
						Compilation.AddRetargetingAssemblySymbolsNoLock(assemblies);
					}
				}

				public override bool IsMatchingAssembly(AssemblySymbol candidateAssembly)
				{
					AssemblySymbol assemblySymbol = ((!(candidateAssembly is RetargetingAssemblySymbol retargetingAssemblySymbol)) ? (candidateAssembly as SourceAssemblySymbol) : retargetingAssemblySymbol.UnderlyingAssembly);
					return (object)assemblySymbol == Compilation.Assembly;
				}
			}

			protected override CommonMessageProvider MessageProvider => Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance;

			public ReferenceManager(string simpleAssemblyName, AssemblyIdentityComparer identityComparer, Dictionary<MetadataReference, object> observedMetadata)
				: base(simpleAssemblyName, identityComparer, observedMetadata)
			{
			}

			protected override void GetActualBoundReferencesUsedBy(AssemblySymbol assemblySymbol, List<AssemblySymbol> referencedAssemblySymbols)
			{
				ImmutableArray<ModuleSymbol>.Enumerator enumerator = assemblySymbol.Modules.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ModuleSymbol current = enumerator.Current;
					referencedAssemblySymbols.AddRange(current.GetReferencedAssemblySymbols());
				}
				int num = referencedAssemblySymbols.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					if (referencedAssemblySymbols[i].IsMissing)
					{
						referencedAssemblySymbols[i] = null;
					}
				}
			}

			protected override ImmutableArray<AssemblySymbol> GetNoPiaResolutionAssemblies(AssemblySymbol candidateAssembly)
			{
				if (candidateAssembly is SourceAssemblySymbol)
				{
					return ImmutableArray<AssemblySymbol>.Empty;
				}
				return candidateAssembly.GetNoPiaResolutionAssemblies();
			}

			protected override bool IsLinked(AssemblySymbol candidateAssembly)
			{
				return candidateAssembly.IsLinked;
			}

			protected override AssemblySymbol GetCorLibrary(AssemblySymbol candidateAssembly)
			{
				AssemblySymbol corLibrary = candidateAssembly.CorLibrary;
				if (!corLibrary.IsMissing)
				{
					return corLibrary;
				}
				return null;
			}

			protected override AssemblyData CreateAssemblyDataForFile(PEAssembly assembly, WeakList<IAssemblySymbolInternal> cachedSymbols, DocumentationProvider documentationProvider, string sourceAssemblySimpleName, MetadataImportOptions importOptions, bool embedInteropTypes)
			{
				return new AssemblyDataForFile(assembly, cachedSymbols, embedInteropTypes, documentationProvider, sourceAssemblySimpleName, importOptions);
			}

			protected override AssemblyData CreateAssemblyDataForCompilation(CompilationReference compilationReference)
			{
				if (!(compilationReference is VisualBasicCompilationReference visualBasicCompilationReference))
				{
					throw new NotSupportedException(string.Format(VBResources.CantReferenceCompilationFromTypes, compilationReference.GetType(), "Visual Basic"));
				}
				return new AssemblyDataForCompilation(visualBasicCompilationReference.Compilation, visualBasicCompilationReference.Properties.EmbedInteropTypes);
			}

			protected override bool CheckPropertiesConsistency(MetadataReference primaryReference, MetadataReference duplicateReference, DiagnosticBag diagnostics)
			{
				return true;
			}

			protected override bool WeakIdentityPropertiesEquivalent(AssemblyIdentity identity1, AssemblyIdentity identity2)
			{
				return identity1.Version == identity2.Version;
			}

			public void CreateSourceAssemblyForCompilation(VisualBasicCompilation compilation)
			{
				if (base.IsBound || !CreateAndSetSourceAssemblyFullBind(compilation))
				{
					if (!base.HasCircularReference)
					{
						CreateAndSetSourceAssemblyReuseData(compilation);
					}
					else
					{
						new ReferenceManager(SimpleAssemblyName, IdentityComparer, ObservedMetadata).CreateAndSetSourceAssemblyFullBind(compilation);
					}
				}
			}

			internal PEAssemblySymbol CreatePEAssemblyForAssemblyMetadata(AssemblyMetadata metadata, MetadataImportOptions importOptions, out ImmutableDictionary<AssemblyIdentity, AssemblyIdentity> assemblyReferenceIdentityMap)
			{
				AssemblyIdentityMap<AssemblySymbol> assemblyIdentityMap = new AssemblyIdentityMap<AssemblySymbol>();
				ImmutableArray<AssemblySymbol>.Enumerator enumerator = base.ReferencedAssemblies.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AssemblySymbol current = enumerator.Current;
					assemblyIdentityMap.Add(current.Identity, current);
				}
				PEAssembly assembly = metadata.GetAssembly();
				ImmutableArray<AssemblySymbol> immutableArray = assembly.AssemblyReferences.SelectAsArray(MapAssemblyIdentityToResolvedSymbol, assemblyIdentityMap);
				assemblyReferenceIdentityMap = CommonReferenceManager<VisualBasicCompilation, AssemblySymbol>.GetAssemblyReferenceIdentityBaselineMap(immutableArray, assembly.AssemblyReferences);
				PEAssemblySymbol pEAssemblySymbol = new PEAssemblySymbol(assembly, DocumentationProvider.Default, isLinked: false, importOptions);
				ImmutableArray<UnifiedAssembly<AssemblySymbol>> unifiedAssemblies = base.UnifiedAssemblies.WhereAsArray((UnifiedAssembly<AssemblySymbol> unified, AssemblyIdentityMap<AssemblySymbol> refAsmByIdentity) => refAsmByIdentity.Contains(unified.OriginalReference, allowHigherVersion: false), assemblyIdentityMap);
				InitializeAssemblyReuseData(pEAssemblySymbol, immutableArray, unifiedAssemblies);
				if (assembly.ContainsNoPiaLocalTypes())
				{
					pEAssemblySymbol.SetNoPiaResolutionAssemblies(base.ReferencedAssemblies);
				}
				return pEAssemblySymbol;
			}

			private static AssemblySymbol MapAssemblyIdentityToResolvedSymbol(AssemblyIdentity identity, AssemblyIdentityMap<AssemblySymbol> map)
			{
				AssemblySymbol value = null;
				if (map.TryGetValue(identity, out value, CommonReferenceManager<VisualBasicCompilation, AssemblySymbol>.CompareVersionPartsSpecifiedInSource))
				{
					return value;
				}
				if (map.TryGetValue(identity, out value, (Version v1, Version v2, AssemblySymbol s) => true))
				{
					throw new NotSupportedException(string.Format(CodeAnalysisResources.ChangingVersionOfAssemblyReferenceIsNotAllowedDuringDebugging, identity, value.Identity.Version));
				}
				return new MissingAssemblySymbol(identity);
			}

			private void CreateAndSetSourceAssemblyReuseData(VisualBasicCompilation compilation)
			{
				string moduleName = compilation.MakeSourceModuleName();
				SourceAssemblySymbol sourceAssemblySymbol = new SourceAssemblySymbol(compilation, SimpleAssemblyName, moduleName, base.ReferencedModules);
				InitializeAssemblyReuseData(sourceAssemblySymbol, base.ReferencedAssemblies, base.UnifiedAssemblies);
				if ((object)compilation._lazyAssemblySymbol != null)
				{
					return;
				}
				lock (CommonReferenceManager.SymbolCacheAndReferenceManagerStateGuard)
				{
					if ((object)compilation._lazyAssemblySymbol == null)
					{
						compilation._lazyAssemblySymbol = sourceAssemblySymbol;
					}
				}
			}

			private void InitializeAssemblyReuseData(AssemblySymbol assemblySymbol, ImmutableArray<AssemblySymbol> referencedAssemblies, ImmutableArray<UnifiedAssembly<AssemblySymbol>> unifiedAssemblies)
			{
				assemblySymbol.SetCorLibrary(base.CorLibraryOpt ?? assemblySymbol);
				ModuleReferences<AssemblySymbol> moduleReferences = new ModuleReferences<AssemblySymbol>(referencedAssemblies.SelectAsArray((AssemblySymbol a) => a.Identity), referencedAssemblies, unifiedAssemblies);
				assemblySymbol.Modules[0].SetReferences(moduleReferences);
				ImmutableArray<ModuleSymbol> modules = assemblySymbol.Modules;
				ImmutableArray<ModuleReferences<AssemblySymbol>> referencedModulesReferences = base.ReferencedModulesReferences;
				int num = modules.Length - 1;
				for (int i = 1; i <= num; i++)
				{
					modules[i].SetReferences(referencedModulesReferences[i - 1]);
				}
			}

			internal bool CreateAndSetSourceAssemblyFullBind(VisualBasicCompilation compilation)
			{
				DiagnosticBag instance = DiagnosticBag.GetInstance();
				bool referencesSupersedeLowerVersions = compilation.Options.ReferencesSupersedeLowerVersions;
				PooledDictionary<string, List<ReferencedAssemblyIdentity>> instance2 = PooledDictionary<string, List<ReferencedAssemblyIdentity>>.GetInstance();
				try
				{
					IDictionary<(string, string), MetadataReference> boundReferenceDirectiveMap = null;
					ImmutableArray<MetadataReference> boundReferenceDirectives = default(ImmutableArray<MetadataReference>);
					ImmutableArray<AssemblyData> assemblies = default(ImmutableArray<AssemblyData>);
					ImmutableArray<PEModule> modules = default(ImmutableArray<PEModule>);
					ImmutableArray<MetadataReference> references = default(ImmutableArray<MetadataReference>);
					ImmutableArray<ResolvedReference> explicitReferenceMap = ResolveMetadataReferences(compilation, instance2, out references, out boundReferenceDirectiveMap, out boundReferenceDirectives, out assemblies, out modules, instance);
					AssemblyDataForAssemblyBeingBuilt item = new AssemblyDataForAssemblyBeingBuilt(new AssemblyIdentity(noThrow: true, SimpleAssemblyName), assemblies, modules);
					ImmutableArray<AssemblyData> explicitAssemblies = assemblies.Insert(0, item);
					ImmutableArray<MetadataReference> implicitlyResolvedReferences = default(ImmutableArray<MetadataReference>);
					ImmutableArray<ResolvedReference> implicitlyResolvedReferenceMap = default(ImmutableArray<ResolvedReference>);
					ImmutableArray<AssemblyData> allAssemblies = default(ImmutableArray<AssemblyData>);
					ImmutableDictionary<AssemblyIdentity, PortableExecutableReference> implicitReferenceResolutions = compilation.ScriptCompilationInfo?.PreviousScriptCompilation?.GetBoundReferenceManager().ImplicitReferenceResolutions ?? ImmutableDictionary<AssemblyIdentity, PortableExecutableReference>.Empty;
					bool hasCircularReference;
					int corLibraryIndex;
					BoundInputAssembly[] array = Bind(compilation, explicitAssemblies, modules, references, explicitReferenceMap, compilation.Options.MetadataReferenceResolver, compilation.Options.MetadataImportOptions, referencesSupersedeLowerVersions, instance2, out allAssemblies, out implicitlyResolvedReferences, out implicitlyResolvedReferenceMap, ref implicitReferenceResolutions, instance, out hasCircularReference, out corLibraryIndex);
					ImmutableArray<MetadataReference> references2 = references.AddRange(implicitlyResolvedReferences);
					explicitReferenceMap = explicitReferenceMap.AddRange(implicitlyResolvedReferenceMap);
					Dictionary<MetadataReference, int> referencedAssembliesMap = null;
					Dictionary<MetadataReference, int> referencedModulesMap = null;
					ImmutableArray<ImmutableArray<string>> aliasesOfReferencedAssemblies = default(ImmutableArray<ImmutableArray<string>>);
					Dictionary<MetadataReference, ImmutableArray<MetadataReference>> mergedAssemblyReferencesMapOpt = null;
					CommonReferenceManager<VisualBasicCompilation, AssemblySymbol>.BuildReferencedAssembliesAndModulesMaps(array, references2, explicitReferenceMap, modules.Length, assemblies.Length, instance2, referencesSupersedeLowerVersions, out referencedAssembliesMap, out referencedModulesMap, out aliasesOfReferencedAssemblies, out mergedAssemblyReferencesMapOpt);
					List<int> list = new List<int>();
					int num = array.Length - 1;
					for (int i = 1; i <= num; i++)
					{
						if ((object)array[i].AssemblySymbol == null)
						{
							array[i].AssemblySymbol = ((AssemblyDataForMetadataOrCompilation)allAssemblies[i]).CreateAssemblySymbol();
							list.Add(i);
						}
					}
					SourceAssemblySymbol sourceAssemblySymbol = new SourceAssemblySymbol(compilation, SimpleAssemblyName, compilation.MakeSourceModuleName(), modules);
					AssemblySymbol assemblySymbol = ((corLibraryIndex == 0) ? sourceAssemblySymbol : ((corLibraryIndex <= 0) ? MissingCorLibrarySymbol.Instance : array[corLibraryIndex].AssemblySymbol));
					sourceAssemblySymbol.SetCorLibrary(assemblySymbol);
					Dictionary<AssemblyIdentity, MissingAssemblySymbol> missingAssemblies = null;
					int totalReferencedAssemblyCount = allAssemblies.Length - 1;
					ImmutableArray<ModuleReferences<AssemblySymbol>> moduleReferences = default(ImmutableArray<ModuleReferences<AssemblySymbol>>);
					SetupReferencesForSourceAssembly(sourceAssemblySymbol, modules, totalReferencedAssemblyCount, array, ref missingAssemblies, ref moduleReferences);
					if (list.Count > 0)
					{
						if (hasCircularReference)
						{
							array[0].AssemblySymbol = sourceAssemblySymbol;
						}
						InitializeNewSymbols(list, sourceAssemblySymbol, allAssemblies, array, missingAssemblies);
					}
					if ((object)compilation._lazyAssemblySymbol == null)
					{
						lock (CommonReferenceManager.SymbolCacheAndReferenceManagerStateGuard)
						{
							if ((object)compilation._lazyAssemblySymbol == null)
							{
								if (base.IsBound)
								{
									return false;
								}
								UpdateSymbolCacheNoLock(list, allAssemblies, array);
								InitializeNoLock(referencedAssembliesMap, referencedModulesMap, boundReferenceDirectiveMap, boundReferenceDirectives, references, implicitReferenceResolutions, hasCircularReference, instance.ToReadOnly(), ((object)assemblySymbol == sourceAssemblySymbol) ? null : assemblySymbol, modules, moduleReferences, sourceAssemblySymbol.SourceModule.GetReferencedAssemblySymbols(), aliasesOfReferencedAssemblies, sourceAssemblySymbol.SourceModule.GetUnifiedAssemblies(), mergedAssemblyReferencesMapOpt);
								compilation._referenceManager = this;
								compilation._lazyAssemblySymbol = sourceAssemblySymbol;
							}
						}
					}
					return true;
				}
				finally
				{
					instance.Free();
					instance2.Free();
				}
			}

			private static void InitializeNewSymbols(List<int> newSymbols, SourceAssemblySymbol assemblySymbol, ImmutableArray<AssemblyData> assemblies, BoundInputAssembly[] bindingResult, Dictionary<AssemblyIdentity, MissingAssemblySymbol> missingAssemblies)
			{
				AssemblySymbol corLibrary = assemblySymbol.CorLibrary;
				foreach (int newSymbol in newSymbols)
				{
					if (assemblies[newSymbol] is AssemblyDataForCompilation)
					{
						SetupReferencesForRetargetingAssembly(bindingResult, newSymbol, ref missingAssemblies, assemblySymbol);
					}
					else
					{
						SetupReferencesForFileAssembly((AssemblyDataForFile)assemblies[newSymbol], bindingResult, newSymbol, ref missingAssemblies, assemblySymbol);
					}
				}
				ArrayBuilder<AssemblySymbol> instance = ArrayBuilder<AssemblySymbol>.GetInstance();
				foreach (int newSymbol2 in newSymbols)
				{
					if (assemblies[newSymbol2].ContainsNoPiaLocalTypes)
					{
						bindingResult[newSymbol2].AssemblySymbol!.SetNoPiaResolutionAssemblies(assemblySymbol.Modules[0].GetReferencedAssemblySymbols());
					}
					instance.Clear();
					if (assemblies[newSymbol2].IsLinked)
					{
						instance.Add(bindingResult[newSymbol2].AssemblySymbol);
					}
					AssemblyReferenceBinding[] referenceBinding = bindingResult[newSymbol2].ReferenceBinding;
					for (int i = 0; i < referenceBinding.Length; i = checked(i + 1))
					{
						AssemblyReferenceBinding assemblyReferenceBinding = referenceBinding[i];
						if (assemblyReferenceBinding.IsBound && assemblies[assemblyReferenceBinding.DefinitionIndex].IsLinked)
						{
							instance.Add(bindingResult[assemblyReferenceBinding.DefinitionIndex].AssemblySymbol);
						}
					}
					if (instance.Count > 0)
					{
						instance.RemoveDuplicates();
						bindingResult[newSymbol2].AssemblySymbol!.SetLinkedReferencedAssemblies(instance.ToImmutable());
					}
					bindingResult[newSymbol2].AssemblySymbol!.SetCorLibrary(corLibrary);
				}
				instance.Free();
				if (missingAssemblies == null)
				{
					return;
				}
				foreach (MissingAssemblySymbol value in missingAssemblies.Values)
				{
					value.SetCorLibrary(corLibrary);
				}
			}

			private void UpdateSymbolCacheNoLock(List<int> newSymbols, ImmutableArray<AssemblyData> assemblies, BoundInputAssembly[] bindingResult)
			{
				foreach (int newSymbol in newSymbols)
				{
					if (assemblies[newSymbol] is AssemblyDataForCompilation assemblyDataForCompilation)
					{
						assemblyDataForCompilation.Compilation.CacheRetargetingAssemblySymbolNoLock(bindingResult[newSymbol].AssemblySymbol);
					}
					else
					{
						((AssemblyDataForFile)assemblies[newSymbol]).CachedSymbols.Add(bindingResult[newSymbol].AssemblySymbol);
					}
				}
			}

			private static void SetupReferencesForRetargetingAssembly(BoundInputAssembly[] bindingResult, int bindingIndex, ref Dictionary<AssemblyIdentity, MissingAssemblySymbol> missingAssemblies, SourceAssemblySymbol sourceAssemblyDebugOnly)
			{
				RetargetingAssemblySymbol retargetingAssemblySymbol = (RetargetingAssemblySymbol)bindingResult[bindingIndex].AssemblySymbol;
				ImmutableArray<ModuleSymbol> modules = retargetingAssemblySymbol.Modules;
				int length = modules.Length;
				int num = 0;
				int num2 = length - 1;
				for (int i = 0; i <= num2; i++)
				{
					ImmutableArray<AssemblyIdentity> identities = retargetingAssemblySymbol.UnderlyingAssembly.Modules[i].GetReferencedAssemblies();
					if (i == 0)
					{
						ImmutableArray<AssemblySymbol> referencedAssemblySymbols = retargetingAssemblySymbol.UnderlyingAssembly.Modules[0].GetReferencedAssemblySymbols();
						int num3 = 0;
						ImmutableArray<AssemblySymbol>.Enumerator enumerator = referencedAssemblySymbols.GetEnumerator();
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.IsLinked)
							{
								num3++;
							}
						}
						if (num3 > 0)
						{
							AssemblyIdentity[] array = new AssemblyIdentity[identities.Length - num3 - 1 + 1];
							int num4 = 0;
							int num5 = referencedAssemblySymbols.Length - 1;
							for (int j = 0; j <= num5; j++)
							{
								if (!referencedAssemblySymbols[j].IsLinked)
								{
									array[num4] = identities[j];
									num4++;
								}
							}
							identities = array.AsImmutableOrNull();
						}
					}
					int length2 = identities.Length;
					AssemblySymbol[] array2 = new AssemblySymbol[length2 - 1 + 1];
					ArrayBuilder<UnifiedAssembly<AssemblySymbol>> unifiedAssemblies = null;
					int num6 = length2 - 1;
					for (int k = 0; k <= num6; k++)
					{
						AssemblyReferenceBinding referenceBinding = bindingResult[bindingIndex].ReferenceBinding[num + k];
						if (referenceBinding.IsBound)
						{
							array2[k] = GetAssemblyDefinitionSymbol(bindingResult, referenceBinding, ref unifiedAssemblies);
						}
						else
						{
							array2[k] = GetOrAddMissingAssemblySymbol(identities[k], ref missingAssemblies);
						}
					}
					ModuleReferences<AssemblySymbol> moduleReferences = new ModuleReferences<AssemblySymbol>(identities, array2.AsImmutableOrNull(), unifiedAssemblies.AsImmutableOrEmpty());
					modules[i].SetReferences(moduleReferences, sourceAssemblyDebugOnly);
					num += length2;
				}
			}

			private static void SetupReferencesForFileAssembly(AssemblyDataForFile fileData, BoundInputAssembly[] bindingResult, int bindingIndex, ref Dictionary<AssemblyIdentity, MissingAssemblySymbol> missingAssemblies, SourceAssemblySymbol sourceAssemblyDebugOnly)
			{
				ImmutableArray<ModuleSymbol> modules = ((PEAssemblySymbol)bindingResult[bindingIndex].AssemblySymbol).Modules;
				int length = modules.Length;
				int num = 0;
				int num2 = length - 1;
				for (int i = 0; i <= num2; i++)
				{
					int num3 = fileData.Assembly.ModuleReferenceCounts[i];
					AssemblyIdentity[] array = new AssemblyIdentity[num3 - 1 + 1];
					AssemblySymbol[] array2 = new AssemblySymbol[num3 - 1 + 1];
					fileData.AssemblyReferences.CopyTo(num, array, 0, num3);
					ArrayBuilder<UnifiedAssembly<AssemblySymbol>> unifiedAssemblies = null;
					int num4 = num3 - 1;
					for (int j = 0; j <= num4; j++)
					{
						AssemblyReferenceBinding referenceBinding = bindingResult[bindingIndex].ReferenceBinding[num + j];
						if (referenceBinding.IsBound)
						{
							array2[j] = GetAssemblyDefinitionSymbol(bindingResult, referenceBinding, ref unifiedAssemblies);
						}
						else
						{
							array2[j] = GetOrAddMissingAssemblySymbol(array[j], ref missingAssemblies);
						}
					}
					ModuleReferences<AssemblySymbol> moduleReferences = new ModuleReferences<AssemblySymbol>(array.AsImmutableOrNull(), array2.AsImmutableOrNull(), unifiedAssemblies.AsImmutableOrEmpty());
					modules[i].SetReferences(moduleReferences, sourceAssemblyDebugOnly);
					num += num3;
				}
			}

			private static void SetupReferencesForSourceAssembly(SourceAssemblySymbol sourceAssembly, ImmutableArray<PEModule> modules, int totalReferencedAssemblyCount, BoundInputAssembly[] bindingResult, ref Dictionary<AssemblyIdentity, MissingAssemblySymbol> missingAssemblies, ref ImmutableArray<ModuleReferences<AssemblySymbol>> moduleReferences)
			{
				ImmutableArray<ModuleSymbol> modules2 = sourceAssembly.Modules;
				ArrayBuilder<ModuleReferences<AssemblySymbol>> arrayBuilder = ((modules2.Length > 1) ? ArrayBuilder<ModuleReferences<AssemblySymbol>>.GetInstance() : null);
				int num = 0;
				int num2 = modules2.Length - 1;
				for (int i = 0; i <= num2; i++)
				{
					int num3 = ((i == 0) ? totalReferencedAssemblyCount : modules[i - 1].ReferencedAssemblies.Length);
					AssemblyIdentity[] array = new AssemblyIdentity[num3 - 1 + 1];
					AssemblySymbol[] array2 = new AssemblySymbol[num3 - 1 + 1];
					ArrayBuilder<UnifiedAssembly<AssemblySymbol>> unifiedAssemblies = null;
					int num4 = num3 - 1;
					for (int j = 0; j <= num4; j++)
					{
						AssemblyReferenceBinding referenceBinding = bindingResult[0].ReferenceBinding[num + j];
						if (referenceBinding.IsBound)
						{
							array2[j] = GetAssemblyDefinitionSymbol(bindingResult, referenceBinding, ref unifiedAssemblies);
						}
						else
						{
							array2[j] = GetOrAddMissingAssemblySymbol(referenceBinding.ReferenceIdentity, ref missingAssemblies);
						}
						array[j] = referenceBinding.ReferenceIdentity;
					}
					ModuleReferences<AssemblySymbol> moduleReferences2 = new ModuleReferences<AssemblySymbol>(array.AsImmutableOrNull(), array2.AsImmutableOrNull(), unifiedAssemblies.AsImmutableOrEmpty());
					if (i > 0)
					{
						arrayBuilder.Add(moduleReferences2);
					}
					modules2[i].SetReferences(moduleReferences2, sourceAssembly);
					num += num3;
				}
				moduleReferences = arrayBuilder.ToImmutableOrEmptyAndFree();
			}

			private static AssemblySymbol GetAssemblyDefinitionSymbol(BoundInputAssembly[] bindingResult, AssemblyReferenceBinding referenceBinding, ref ArrayBuilder<UnifiedAssembly<AssemblySymbol>> unifiedAssemblies)
			{
				AssemblySymbol assemblySymbol = bindingResult[referenceBinding.DefinitionIndex].AssemblySymbol;
				if (referenceBinding.VersionDifference != 0)
				{
					if (unifiedAssemblies == null)
					{
						unifiedAssemblies = new ArrayBuilder<UnifiedAssembly<AssemblySymbol>>();
					}
					unifiedAssemblies.Add(new UnifiedAssembly<AssemblySymbol>(assemblySymbol, referenceBinding.ReferenceIdentity));
				}
				return assemblySymbol;
			}

			private static MissingAssemblySymbol GetOrAddMissingAssemblySymbol(AssemblyIdentity identity, ref Dictionary<AssemblyIdentity, MissingAssemblySymbol> missingAssemblies)
			{
				MissingAssemblySymbol value = null;
				if (missingAssemblies == null)
				{
					missingAssemblies = new Dictionary<AssemblyIdentity, MissingAssemblySymbol>();
				}
				else if (missingAssemblies.TryGetValue(identity, out value))
				{
					return value;
				}
				value = new MissingAssemblySymbol(identity);
				missingAssemblies.Add(identity, value);
				return value;
			}

			internal static bool IsSourceAssemblySymbolCreated(VisualBasicCompilation compilation)
			{
				return (object)compilation._lazyAssemblySymbol != null;
			}

			internal static bool IsReferenceManagerInitialized(VisualBasicCompilation compilation)
			{
				return compilation._referenceManager.IsBound;
			}
		}

		internal class SpecialMembersSignatureComparer : SignatureComparer<MethodSymbol, FieldSymbol, PropertySymbol, TypeSymbol, ParameterSymbol>
		{
			public static readonly SpecialMembersSignatureComparer Instance = new SpecialMembersSignatureComparer();

			protected SpecialMembersSignatureComparer()
			{
			}

			protected override TypeSymbol GetMDArrayElementType(TypeSymbol type)
			{
				if (type.Kind != SymbolKind.ArrayType)
				{
					return null;
				}
				ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)type;
				if (arrayTypeSymbol.IsSZArray)
				{
					return null;
				}
				return arrayTypeSymbol.ElementType;
			}

			protected override bool MatchArrayRank(TypeSymbol type, int countOfDimensions)
			{
				if (countOfDimensions == 1)
				{
					return false;
				}
				if (type.Kind != SymbolKind.ArrayType)
				{
					return false;
				}
				return ((ArrayTypeSymbol)type).Rank == countOfDimensions;
			}

			protected override TypeSymbol GetSZArrayElementType(TypeSymbol type)
			{
				if (type.Kind != SymbolKind.ArrayType)
				{
					return null;
				}
				ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)type;
				if (!arrayTypeSymbol.IsSZArray)
				{
					return null;
				}
				return arrayTypeSymbol.ElementType;
			}

			protected override TypeSymbol GetFieldType(FieldSymbol field)
			{
				return field.Type;
			}

			protected override TypeSymbol GetPropertyType(PropertySymbol prop)
			{
				return prop.Type;
			}

			protected override TypeSymbol GetGenericTypeArgument(TypeSymbol type, int argumentIndex)
			{
				if (type.Kind != SymbolKind.NamedType)
				{
					return null;
				}
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;
				if (namedTypeSymbol.Arity <= argumentIndex)
				{
					return null;
				}
				if ((object)namedTypeSymbol.ContainingType != null)
				{
					return null;
				}
				return namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics[argumentIndex];
			}

			protected override TypeSymbol GetGenericTypeDefinition(TypeSymbol type)
			{
				if (type.Kind != SymbolKind.NamedType)
				{
					return null;
				}
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;
				if ((object)namedTypeSymbol.ContainingType != null)
				{
					return null;
				}
				if (namedTypeSymbol.Arity == 0)
				{
					return null;
				}
				return namedTypeSymbol.OriginalDefinition;
			}

			protected override ImmutableArray<ParameterSymbol> GetParameters(MethodSymbol method)
			{
				return method.Parameters;
			}

			protected override ImmutableArray<ParameterSymbol> GetParameters(PropertySymbol property)
			{
				return property.Parameters;
			}

			protected override TypeSymbol GetParamType(ParameterSymbol parameter)
			{
				return parameter.Type;
			}

			protected override TypeSymbol GetPointedToType(TypeSymbol type)
			{
				return null;
			}

			protected override TypeSymbol GetReturnType(MethodSymbol method)
			{
				return method.ReturnType;
			}

			protected override bool IsByRefParam(ParameterSymbol parameter)
			{
				return parameter.IsByRef;
			}

			protected override bool IsByRefMethod(MethodSymbol method)
			{
				return method.ReturnsByRef;
			}

			protected override bool IsByRefProperty(PropertySymbol property)
			{
				return property.ReturnsByRef;
			}

			protected override bool IsGenericMethodTypeParam(TypeSymbol type, int paramPosition)
			{
				if (type.Kind != SymbolKind.TypeParameter)
				{
					return false;
				}
				TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)type;
				if (typeParameterSymbol.ContainingSymbol.Kind != SymbolKind.Method)
				{
					return false;
				}
				return typeParameterSymbol.Ordinal == paramPosition;
			}

			protected override bool IsGenericTypeParam(TypeSymbol type, int paramPosition)
			{
				if (type.Kind != SymbolKind.TypeParameter)
				{
					return false;
				}
				TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)type;
				if (typeParameterSymbol.ContainingSymbol.Kind != SymbolKind.NamedType)
				{
					return false;
				}
				return typeParameterSymbol.Ordinal == paramPosition;
			}

			protected override bool MatchTypeToTypeId(TypeSymbol type, int typeId)
			{
				return (int)type.SpecialType == typeId;
			}
		}

		private class WellKnownMembersSignatureComparer : SpecialMembersSignatureComparer
		{
			private readonly VisualBasicCompilation _compilation;

			public WellKnownMembersSignatureComparer(VisualBasicCompilation compilation)
			{
				_compilation = compilation;
			}

			protected override bool MatchTypeToTypeId(TypeSymbol type, int typeId)
			{
				if (((WellKnownType)typeId).IsWellKnownType())
				{
					return (object)type == _compilation.GetWellKnownType((WellKnownType)typeId);
				}
				return base.MatchTypeToTypeId(type, typeId);
			}
		}

		internal class TupleNamesEncoder
		{
			public static ImmutableArray<string> Encode(TypeSymbol type)
			{
				ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance();
				if (!TryGetNames(type, instance))
				{
					instance.Free();
					return default(ImmutableArray<string>);
				}
				return instance.ToImmutableAndFree();
			}

			public static ImmutableArray<TypedConstant> Encode(TypeSymbol type, TypeSymbol stringType)
			{
				ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance();
				if (!TryGetNames(type, instance))
				{
					instance.Free();
					return default(ImmutableArray<TypedConstant>);
				}
				ImmutableArray<TypedConstant> result = instance.SelectAsArray((string name, TypeSymbol constantType) => new TypedConstant(constantType, TypedConstantKind.Primitive, name), stringType);
				instance.Free();
				return result;
			}

			internal static bool TryGetNames(TypeSymbol type, ArrayBuilder<string> namesBuilder)
			{
				TypeSymbolExtensions.VisitType(type, (TypeSymbol t, ArrayBuilder<string> builder) => AddNames(t, builder), namesBuilder);
				return namesBuilder.Any((string name) => name != null);
			}

			private static bool AddNames(TypeSymbol type, ArrayBuilder<string> namesBuilder)
			{
				if (type.IsTupleType)
				{
					if (type.TupleElementNames.IsDefaultOrEmpty)
					{
						namesBuilder.AddMany(null, type.TupleElementTypes.Length);
					}
					else
					{
						namesBuilder.AddRange(type.TupleElementNames);
					}
				}
				return false;
			}
		}

		private static readonly ConcurrentLruCache<VisualBasicParseOptions, SyntaxTree> s_myTemplateCache = new ConcurrentLruCache<VisualBasicParseOptions, SyntaxTree>(5);

		private SourceAssemblySymbol _lazyAssemblySymbol;

		private ReferenceManager _referenceManager;

		private readonly VisualBasicCompilationOptions _options;

		private NamespaceSymbol _lazyGlobalNamespace;

		private readonly ImmutableArray<SyntaxTree> _syntaxTrees;

		private readonly ImmutableDictionary<SyntaxTree, int> _syntaxTreeOrdinalMap;

		private ImmutableArray<SyntaxTree> _lazyAllSyntaxTrees;

		private readonly ImmutableDictionary<SyntaxTree, DeclarationTableEntry> _rootNamespaces;

		private ConcurrentQueue<ImportInfo> _lazyImportInfos;

		private ConcurrentDictionary<(SyntaxTree SyntaxTree, int ImportsClausePosition), ImmutableArray<AssemblySymbol>> _lazyImportClauseDependencies;

		private ImmutableArray<Diagnostic> _lazyClsComplianceDiagnostics;

		private ImmutableArray<AssemblySymbol> _lazyClsComplianceDependencies;

		private readonly ImmutableArray<EmbeddedTreeAndDeclaration> _embeddedTrees;

		private readonly DeclarationTable _declarationTable;

		private readonly AnonymousTypeManager _anonymousTypeManager;

		private EmbeddedSymbolManager _lazyEmbeddedSymbolManager;

		private SyntaxTree _lazyMyTemplate;

		private readonly Lazy<ImplicitNamedTypeSymbol> _scriptClass;

		private EntryPoint _lazyEntryPoint;

		private HashSet<SyntaxTree> _lazyCompilationUnitCompletedTrees;

		private readonly LanguageVersion _languageVersion;

		private ConcurrentSet<AssemblySymbol> _lazyUsedAssemblyReferences;

		private bool _usedAssemblyReferencesFrozen;

		private readonly WellKnownMembersSignatureComparer _wellKnownMemberSignatureComparer;

		private NamedTypeSymbol[] _lazyWellKnownTypes;

		private Symbol[] _lazyWellKnownTypeMembers;

		private Symbol _lazyExtensionAttributeConstructor;

		private object _lazyExtensionAttributeConstructorErrorInfo;

		public override string Language => "Visual Basic";

		public override bool IsCaseSensitive => false;

		internal DeclarationTable Declarations => _declarationTable;

		internal MergedNamespaceDeclaration MergedRootDeclaration => Declarations.GetMergedRoot(this);

		public new VisualBasicCompilationOptions Options => _options;

		public LanguageVersion LanguageVersion => _languageVersion;

		internal AnonymousTypeManager AnonymousTypeManager => _anonymousTypeManager;

		internal override CommonAnonymousTypeManager CommonAnonymousTypeManager => _anonymousTypeManager;

		internal SyntaxTree MyTemplate
		{
			get
			{
				if (_lazyMyTemplate == VisualBasicSyntaxTree.Dummy)
				{
					VisualBasicCompilationOptions options = Options;
					if (options.EmbedVbCoreRuntime || options.SuppressEmbeddedDeclarations)
					{
						_lazyMyTemplate = null;
					}
					else
					{
						VisualBasicParseOptions visualBasicParseOptions = options.ParseOptions ?? VisualBasicParseOptions.Default;
						SyntaxTree value = null;
						if (s_myTemplateCache.TryGetValue(visualBasicParseOptions, out value))
						{
							Interlocked.CompareExchange(ref _lazyMyTemplate, value, VisualBasicSyntaxTree.Dummy);
						}
						else
						{
							string vbMyTemplateText = EmbeddedResources.VbMyTemplateText;
							VisualBasicParseOptions options2 = visualBasicParseOptions.WithLanguageVersion(LanguageVersion.Default);
							value = VisualBasicSyntaxTree.ParseText(vbMyTemplateText, isMyTemplate: true, options2);
							if (value.GetDiagnostics().Any())
							{
								throw ExceptionUtilities.Unreachable;
							}
							if (Interlocked.CompareExchange(ref _lazyMyTemplate, value, VisualBasicSyntaxTree.Dummy) == VisualBasicSyntaxTree.Dummy)
							{
								s_myTemplateCache[visualBasicParseOptions] = value;
							}
						}
					}
				}
				return _lazyMyTemplate;
			}
			set
			{
				if (value != null && value.GetDiagnostics().Any())
				{
					throw ExceptionUtilities.Unreachable;
				}
				_lazyMyTemplate = value;
			}
		}

		internal EmbeddedSymbolManager EmbeddedSymbolManager
		{
			get
			{
				if (_lazyEmbeddedSymbolManager == null)
				{
					EmbeddedSymbolKind embeddedSymbolKind = (Options.EmbedVbCoreRuntime ? EmbeddedSymbolKind.VbCore : EmbeddedSymbolKind.None) | (IncludeInternalXmlHelper() ? EmbeddedSymbolKind.XmlHelper : EmbeddedSymbolKind.None);
					if (embeddedSymbolKind != 0)
					{
						embeddedSymbolKind |= EmbeddedSymbolKind.EmbeddedAttribute;
					}
					Interlocked.CompareExchange(ref _lazyEmbeddedSymbolManager, new EmbeddedSymbolManager(embeddedSymbolKind), null);
				}
				return _lazyEmbeddedSymbolManager;
			}
		}

		internal new VisualBasicScriptCompilationInfo ScriptCompilationInfo { get; }

		internal override ScriptCompilationInfo CommonScriptCompilationInfo => ScriptCompilationInfo;

		internal VisualBasicCompilation PreviousSubmission => ScriptCompilationInfo?.PreviousScriptCompilation;

		protected override ITypeSymbol CommonScriptGlobalsType => null;

		public new ImmutableArray<SyntaxTree> SyntaxTrees => _syntaxTrees;

		internal ImmutableArray<SyntaxTree> AllSyntaxTrees
		{
			get
			{
				if (_lazyAllSyntaxTrees.IsDefault)
				{
					ArrayBuilder<SyntaxTree> instance = ArrayBuilder<SyntaxTree>.GetInstance();
					instance.AddRange(_syntaxTrees);
					ImmutableArray<EmbeddedTreeAndDeclaration>.Enumerator enumerator = _embeddedTrees.GetEnumerator();
					while (enumerator.MoveNext())
					{
						SyntaxTree value = enumerator.Current.Tree.Value;
						if (value != null)
						{
							instance.Add(value);
						}
					}
					ImmutableInterlocked.InterlockedInitialize(ref _lazyAllSyntaxTrees, instance.ToImmutableAndFree());
				}
				return _lazyAllSyntaxTrees;
			}
		}

		public override ImmutableArray<MetadataReference> DirectiveReferences => GetBoundReferenceManager().DirectiveReferences;

		internal override IDictionary<(string path, string content), MetadataReference> ReferenceDirectiveMap => GetBoundReferenceManager().ReferenceDirectiveMap;

		public override IEnumerable<AssemblyIdentity> ReferencedAssemblyNames => Assembly.Modules.SelectMany((ModuleSymbol m) => m.GetReferencedAssemblies());

		internal override IEnumerable<ReferenceDirective> ReferenceDirectives => _declarationTable.ReferenceDirectives;

		internal bool EnableEnumArrayBlockInitialization
		{
			get
			{
				Symbol wellKnownTypeMember = GetWellKnownTypeMember(WellKnownMember.System_Runtime_GCLatencyMode__SustainedLowLatency);
				if ((object)wellKnownTypeMember != null)
				{
					return wellKnownTypeMember.ContainingAssembly == Assembly.CorLibrary;
				}
				return false;
			}
		}

		internal SourceAssemblySymbol SourceAssembly
		{
			get
			{
				GetBoundReferenceManager();
				return _lazyAssemblySymbol;
			}
		}

		internal new AssemblySymbol Assembly => SourceAssembly;

		internal new ModuleSymbol SourceModule => Assembly.Modules[0];

		internal new NamespaceSymbol GlobalNamespace
		{
			get
			{
				if ((object)_lazyGlobalNamespace == null)
				{
					Interlocked.CompareExchange(ref _lazyGlobalNamespace, MergedNamespaceSymbol.CreateGlobalNamespace(this), null);
				}
				return _lazyGlobalNamespace;
			}
		}

		internal NamespaceSymbol RootNamespace => ((SourceModuleSymbol)SourceModule).RootNamespace;

		internal ImmutableArray<NamespaceOrTypeSymbol> MemberImports => ((SourceModuleSymbol)SourceModule).MemberImports.SelectAsArray((NamespaceOrTypeAndImportsClausePosition m) => m.NamespaceOrType);

		internal ImmutableArray<AliasSymbol> AliasImports => ((SourceModuleSymbol)SourceModule).AliasImports.SelectAsArray((AliasAndImportsClausePosition a) => a.Alias);

		internal bool DeclaresTheObjectClass => SourceAssembly.DeclaresTheObjectClass;

		internal new NamedTypeSymbol ScriptClass => SourceScriptClass;

		internal ImplicitNamedTypeSymbol SourceScriptClass => _scriptClass.Value;

		internal new NamedTypeSymbol ObjectType => Assembly.ObjectType;

		internal bool HasTupleNamesAttributes
		{
			get
			{
				if (GetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_TupleElementNamesAttribute__ctorTransformNames) is MethodSymbol memberSymbol)
				{
					return Binder.GetUseSiteInfoForWellKnownTypeMember(memberSymbol, WellKnownMember.System_Runtime_CompilerServices_TupleElementNamesAttribute__ctorTransformNames, embedVBRuntimeUsed: false).DiagnosticInfo == null;
				}
				return false;
			}
		}

		internal bool FeatureStrictEnabled => Feature("strict") != null;

		internal override CommonMessageProvider MessageProvider => Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance;

		internal override byte LinkerMajorVersion => 80;

		internal override bool IsDelaySigned => SourceAssembly.IsDelaySigned;

		internal override StrongNameKeys StrongNameKeys => SourceAssembly.StrongNameKeys;

		internal override Guid DebugSourceDocumentLanguageId => DebugSourceDocument.CorSymLanguageTypeBasic;

		protected override IAssemblySymbol CommonAssembly => Assembly;

		protected override INamespaceSymbol CommonGlobalNamespace => GlobalNamespace;

		protected override CompilationOptions CommonOptions => Options;

		protected override ImmutableArray<SyntaxTree> CommonSyntaxTrees => SyntaxTrees;

		protected override IModuleSymbol CommonSourceModule => SourceModule;

		protected override INamedTypeSymbol CommonScriptClass => ScriptClass;

		protected override ITypeSymbol CommonDynamicType
		{
			get
			{
				throw new NotSupportedException(VBResources.ThereIsNoDynamicTypeInVB);
			}
		}

		protected override INamedTypeSymbol CommonObjectType => ObjectType;

		public static VisualBasicCompilation Create(string assemblyName, IEnumerable<SyntaxTree> syntaxTrees = null, IEnumerable<MetadataReference> references = null, VisualBasicCompilationOptions options = null)
		{
			return Create(assemblyName, options, syntaxTrees?.Cast<SyntaxTree>(), references, null, null, null, isSubmission: false);
		}

		internal static VisualBasicCompilation CreateScriptCompilation(string assemblyName, SyntaxTree syntaxTree = null, IEnumerable<MetadataReference> references = null, VisualBasicCompilationOptions options = null, VisualBasicCompilation previousScriptCompilation = null, Type returnType = null, Type globalsType = null)
		{
			Compilation.CheckSubmissionOptions(options);
			Compilation.ValidateScriptCompilationParameters(previousScriptCompilation, returnType, ref globalsType);
			return Create(assemblyName, (options ?? new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary)).WithReferencesSupersedeLowerVersions(value: true), (syntaxTree == null) ? SpecializedCollections.EmptyEnumerable<SyntaxTree>() : new SyntaxTree[1] { syntaxTree }, references, previousScriptCompilation, returnType, globalsType, isSubmission: true);
		}

		private static VisualBasicCompilation Create(string assemblyName, VisualBasicCompilationOptions options, IEnumerable<SyntaxTree> syntaxTrees, IEnumerable<MetadataReference> references, VisualBasicCompilation previousSubmission, Type returnType, Type hostObjectType, bool isSubmission)
		{
			if ((object)options == null)
			{
				options = new VisualBasicCompilationOptions(OutputKind.ConsoleApplication);
			}
			ImmutableArray<MetadataReference> references2 = Compilation.ValidateReferences<VisualBasicCompilationReference>(references);
			VisualBasicCompilation visualBasicCompilation = null;
			ImmutableArray<EmbeddedTreeAndDeclaration> embeddedTrees = CreateEmbeddedTrees(new Lazy<VisualBasicCompilation>(() => visualBasicCompilation));
			ImmutableDictionary<SyntaxTree, DeclarationTableEntry> rootNamespaces = ImmutableDictionary.Create<SyntaxTree, DeclarationTableEntry>();
			DeclarationTable declarationTable = AddEmbeddedTrees(DeclarationTable.Empty, embeddedTrees);
			visualBasicCompilation = new VisualBasicCompilation(assemblyName, options, references2, ImmutableArray<SyntaxTree>.Empty, ImmutableDictionary.Create<SyntaxTree, int>(), rootNamespaces, embeddedTrees, declarationTable, previousSubmission, returnType, hostObjectType, isSubmission, null, reuseReferenceManager: false, null);
			if (syntaxTrees != null)
			{
				visualBasicCompilation = visualBasicCompilation.AddSyntaxTrees(syntaxTrees);
			}
			return visualBasicCompilation;
		}

		private VisualBasicCompilation(string assemblyName, VisualBasicCompilationOptions options, ImmutableArray<MetadataReference> references, ImmutableArray<SyntaxTree> syntaxTrees, ImmutableDictionary<SyntaxTree, int> syntaxTreeOrdinalMap, ImmutableDictionary<SyntaxTree, DeclarationTableEntry> rootNamespaces, ImmutableArray<EmbeddedTreeAndDeclaration> embeddedTrees, DeclarationTable declarationTable, VisualBasicCompilation previousSubmission, Type submissionReturnType, Type hostObjectType, bool isSubmission, ReferenceManager referenceManager, bool reuseReferenceManager, SemanticModelProvider semanticModelProvider, AsyncQueue<CompilationEvent> eventQueue = null)
			: base(assemblyName, references, Compilation.SyntaxTreeCommonFeatures(syntaxTrees), isSubmission, semanticModelProvider, eventQueue)
		{
			_lazyMyTemplate = VisualBasicSyntaxTree.Dummy;
			_wellKnownMemberSignatureComparer = new WellKnownMembersSignatureComparer(this);
			_lazyExtensionAttributeConstructor = ErrorTypeSymbol.UnknownResultType;
			_options = options;
			_syntaxTrees = syntaxTrees;
			_syntaxTreeOrdinalMap = syntaxTreeOrdinalMap;
			_rootNamespaces = rootNamespaces;
			_embeddedTrees = embeddedTrees;
			_declarationTable = declarationTable;
			_anonymousTypeManager = new AnonymousTypeManager(this);
			_languageVersion = CommonLanguageVersion(syntaxTrees);
			_scriptClass = new Lazy<ImplicitNamedTypeSymbol>(BindScriptClass);
			if (isSubmission)
			{
				ScriptCompilationInfo = new VisualBasicScriptCompilationInfo(previousSubmission, submissionReturnType, hostObjectType);
			}
			if (reuseReferenceManager)
			{
				_referenceManager = referenceManager;
			}
			else
			{
				_referenceManager = new ReferenceManager(MakeSourceAssemblySimpleName(), options.AssemblyIdentityComparer, referenceManager?.ObservedMetadata);
			}
			if (base.EventQueue != null)
			{
				base.EventQueue!.TryEnqueue(new CompilationStartedEvent(this));
			}
		}

		internal override void ValidateDebugEntryPoint(IMethodSymbol debugEntryPoint, DiagnosticBag diagnostics)
		{
			MethodSymbol methodSymbol = debugEntryPoint as MethodSymbol;
			if (methodSymbol?.DeclaringCompilation != this || !methodSymbol.IsDefinition)
			{
				DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_DebugEntryPointNotSourceMethodDefinition, Location.None);
			}
		}

		private LanguageVersion CommonLanguageVersion(ImmutableArray<SyntaxTree> syntaxTrees)
		{
			LanguageVersion? languageVersion = null;
			ImmutableArray<SyntaxTree>.Enumerator enumerator = syntaxTrees.GetEnumerator();
			while (enumerator.MoveNext())
			{
				LanguageVersion languageVersion2 = ((VisualBasicParseOptions)enumerator.Current.Options).LanguageVersion;
				if (!languageVersion.HasValue)
				{
					languageVersion = languageVersion2;
					continue;
				}
				int? num = (int?)languageVersion;
				int num2 = (int)languageVersion2;
				if (!(num.HasValue ? new bool?(num.GetValueOrDefault() != num2) : null).GetValueOrDefault())
				{
					continue;
				}
				throw new ArgumentException(CodeAnalysisResources.InconsistentLanguageVersions, "syntaxTrees");
			}
			return languageVersion ?? LanguageVersionFacts.MapSpecifiedToEffectiveVersion(LanguageVersion.Default);
		}

		public new VisualBasicCompilation Clone()
		{
			return new VisualBasicCompilation(base.AssemblyName, _options, base.ExternalReferences, _syntaxTrees, _syntaxTreeOrdinalMap, _rootNamespaces, _embeddedTrees, _declarationTable, PreviousSubmission, base.SubmissionReturnType, base.HostObjectType, base.IsSubmission, _referenceManager, reuseReferenceManager: true, base.SemanticModelProvider);
		}

		private VisualBasicCompilation UpdateSyntaxTrees(ImmutableArray<SyntaxTree> syntaxTrees, ImmutableDictionary<SyntaxTree, int> syntaxTreeOrdinalMap, ImmutableDictionary<SyntaxTree, DeclarationTableEntry> rootNamespaces, DeclarationTable declarationTable, bool referenceDirectivesChanged)
		{
			return new VisualBasicCompilation(base.AssemblyName, _options, base.ExternalReferences, syntaxTrees, syntaxTreeOrdinalMap, rootNamespaces, _embeddedTrees, declarationTable, PreviousSubmission, base.SubmissionReturnType, base.HostObjectType, base.IsSubmission, _referenceManager, !referenceDirectivesChanged, base.SemanticModelProvider);
		}

		public new VisualBasicCompilation WithAssemblyName(string assemblyName)
		{
			return new VisualBasicCompilation(assemblyName, Options, base.ExternalReferences, _syntaxTrees, _syntaxTreeOrdinalMap, _rootNamespaces, _embeddedTrees, _declarationTable, PreviousSubmission, base.SubmissionReturnType, base.HostObjectType, base.IsSubmission, _referenceManager, string.Equals(assemblyName, base.AssemblyName, StringComparison.Ordinal), base.SemanticModelProvider);
		}

		public new VisualBasicCompilation WithReferences(params MetadataReference[] newReferences)
		{
			return WithReferences((IEnumerable<MetadataReference>)newReferences);
		}

		public new VisualBasicCompilation WithReferences(IEnumerable<MetadataReference> newReferences)
		{
			DeclarationTable declTable = RemoveEmbeddedTrees(_declarationTable, _embeddedTrees);
			VisualBasicCompilation result = null;
			ImmutableArray<EmbeddedTreeAndDeclaration> embeddedTrees = CreateEmbeddedTrees(new Lazy<VisualBasicCompilation>(() => result));
			result = new VisualBasicCompilation(declarationTable: AddEmbeddedTrees(declTable, embeddedTrees), assemblyName: base.AssemblyName, options: Options, references: Compilation.ValidateReferences<VisualBasicCompilationReference>(newReferences), syntaxTrees: _syntaxTrees, syntaxTreeOrdinalMap: _syntaxTreeOrdinalMap, rootNamespaces: _rootNamespaces, embeddedTrees: embeddedTrees, previousSubmission: PreviousSubmission, submissionReturnType: base.SubmissionReturnType, hostObjectType: base.HostObjectType, isSubmission: base.IsSubmission, referenceManager: null, reuseReferenceManager: false, semanticModelProvider: base.SemanticModelProvider);
			return result;
		}

		public VisualBasicCompilation WithOptions(VisualBasicCompilationOptions newOptions)
		{
			_Closure_0024__57_002D0 arg = default(_Closure_0024__57_002D0);
			_Closure_0024__57_002D0 CS_0024_003C_003E8__locals0 = new _Closure_0024__57_002D0(arg);
			if ((object)newOptions == null)
			{
				throw new ArgumentNullException("newOptions");
			}
			CS_0024_003C_003E8__locals0._0024VB_0024Local_c = null;
			ImmutableArray<EmbeddedTreeAndDeclaration> embeddedTrees = _embeddedTrees;
			DeclarationTable declTable = _declarationTable;
			ImmutableDictionary<SyntaxTree, DeclarationTableEntry> declMap = _rootNamespaces;
			if (!string.Equals(Options.RootNamespace, newOptions.RootNamespace, StringComparison.Ordinal))
			{
				declMap = ImmutableDictionary.Create<SyntaxTree, DeclarationTableEntry>();
				declTable = DeclarationTable.Empty;
				embeddedTrees = CreateEmbeddedTrees(new Lazy<VisualBasicCompilation>(() => CS_0024_003C_003E8__locals0._0024VB_0024Local_c));
				declTable = AddEmbeddedTrees(declTable, embeddedTrees);
				bool referenceDirectivesChanged = false;
				ImmutableArray<SyntaxTree>.Enumerator enumerator = _syntaxTrees.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AddSyntaxTreeToDeclarationMapAndTable(enumerator.Current, newOptions, base.IsSubmission, ref declMap, ref declTable, ref referenceDirectivesChanged);
				}
			}
			else if (Options.EmbedVbCoreRuntime != newOptions.EmbedVbCoreRuntime || Options.ParseOptions != newOptions.ParseOptions)
			{
				declTable = RemoveEmbeddedTrees(declTable, _embeddedTrees);
				embeddedTrees = CreateEmbeddedTrees(new Lazy<VisualBasicCompilation>(() => CS_0024_003C_003E8__locals0._0024VB_0024Local_c));
				declTable = AddEmbeddedTrees(declTable, embeddedTrees);
			}
			CS_0024_003C_003E8__locals0._0024VB_0024Local_c = new VisualBasicCompilation(base.AssemblyName, newOptions, base.ExternalReferences, _syntaxTrees, _syntaxTreeOrdinalMap, declMap, embeddedTrees, declTable, PreviousSubmission, base.SubmissionReturnType, base.HostObjectType, base.IsSubmission, _referenceManager, _options.CanReuseCompilationReferenceManager(newOptions), base.SemanticModelProvider);
			return CS_0024_003C_003E8__locals0._0024VB_0024Local_c;
		}

		internal VisualBasicCompilation WithScriptCompilationInfo(VisualBasicScriptCompilationInfo info)
		{
			if (info == ScriptCompilationInfo)
			{
				return this;
			}
			bool reuseReferenceManager = ScriptCompilationInfo?.PreviousScriptCompilation == info?.PreviousScriptCompilation;
			return new VisualBasicCompilation(base.AssemblyName, Options, base.ExternalReferences, _syntaxTrees, _syntaxTreeOrdinalMap, _rootNamespaces, _embeddedTrees, _declarationTable, info?.PreviousScriptCompilation, info?.ReturnTypeOpt, info?.GlobalsType, info != null, _referenceManager, reuseReferenceManager, base.SemanticModelProvider);
		}

		internal override Compilation WithSemanticModelProvider(SemanticModelProvider semanticModelProvider)
		{
			if (base.SemanticModelProvider == semanticModelProvider)
			{
				return this;
			}
			return new VisualBasicCompilation(base.AssemblyName, Options, base.ExternalReferences, _syntaxTrees, _syntaxTreeOrdinalMap, _rootNamespaces, _embeddedTrees, _declarationTable, PreviousSubmission, base.SubmissionReturnType, base.HostObjectType, base.IsSubmission, _referenceManager, reuseReferenceManager: true, semanticModelProvider);
		}

		internal override Compilation WithEventQueue(AsyncQueue<CompilationEvent> eventQueue)
		{
			return new VisualBasicCompilation(base.AssemblyName, Options, base.ExternalReferences, _syntaxTrees, _syntaxTreeOrdinalMap, _rootNamespaces, _embeddedTrees, _declarationTable, PreviousSubmission, base.SubmissionReturnType, base.HostObjectType, base.IsSubmission, _referenceManager, reuseReferenceManager: true, base.SemanticModelProvider, eventQueue);
		}

		internal override void SerializePdbEmbeddedCompilationOptions(BlobBuilder builder)
		{
			WriteValue(builder, "language-version", LanguageVersionFacts.ToDisplayString(LanguageVersion));
			WriteValue(builder, "checked", Options.CheckOverflow.ToString());
			WriteValue(builder, "option-strict", Options.OptionStrict.ToString());
			WriteValue(builder, "option-infer", Options.OptionInfer.ToString());
			WriteValue(builder, "option-compare-text", Options.OptionCompareText.ToString());
			WriteValue(builder, "option-explicit", Options.OptionExplicit.ToString());
			WriteValue(builder, "embed-runtime", Options.EmbedVbCoreRuntime.ToString());
			if (Options.GlobalImports.Length > 0)
			{
				WriteValue(builder, "global-namespaces", string.Join(";", Options.GlobalImports.Select((GlobalImport x) => x.Name)));
			}
			if (!string.IsNullOrEmpty(Options.RootNamespace))
			{
				WriteValue(builder, "root-namespace", Options.RootNamespace);
			}
			if ((object)Options.ParseOptions == null)
			{
				return;
			}
			IEnumerable<string> values = Options.ParseOptions.PreprocessorSymbols.Select(delegate(KeyValuePair<string, object> p)
			{
				if (p.Value is string)
				{
					return p.Key + "=\"" + p.Value.ToString() + "\"";
				}
				return (p.Value == null) ? p.Key : (p.Key + "=" + p.Value.ToString());
			});
			WriteValue(builder, "define", string.Join(",", values));
		}

		private void WriteValue(BlobBuilder builder, string key, string value)
		{
			builder.WriteUTF8(key);
			builder.WriteByte(0);
			builder.WriteUTF8(value);
			builder.WriteByte(0);
		}

		internal override bool HasSubmissionResult()
		{
			SyntaxTree syntaxTree = SyntaxTrees.SingleOrDefault();
			if (syntaxTree == null)
			{
				return false;
			}
			CompilationUnitSyntax compilationUnitRoot = VisualBasicExtensions.GetCompilationUnitRoot(syntaxTree);
			if (compilationUnitRoot.HasErrors)
			{
				return false;
			}
			StatementSyntax statementSyntax = compilationUnitRoot.Members.LastOrDefault();
			if (statementSyntax == null)
			{
				return false;
			}
			SemanticModel semanticModel = GetSemanticModel(syntaxTree);
			switch (statementSyntax.Kind())
			{
			case SyntaxKind.PrintStatement:
			{
				ExpressionSyntax expression2 = ((PrintStatementSyntax)statementSyntax).Expression;
				semanticModel.GetTypeInfo(expression2);
				return true;
			}
			case SyntaxKind.ExpressionStatement:
			{
				ExpressionSyntax expression = ((ExpressionStatementSyntax)statementSyntax).Expression;
				return semanticModel.GetTypeInfo(expression).Type!.SpecialType != SpecialType.System_Void;
			}
			case SyntaxKind.CallStatement:
			{
				ExpressionSyntax invocation = ((CallStatementSyntax)statementSyntax).Invocation;
				return semanticModel.GetTypeInfo(invocation).Type!.SpecialType != SpecialType.System_Void;
			}
			default:
				return false;
			}
		}

		internal SynthesizedInteractiveInitializerMethod GetSubmissionInitializer()
		{
			if (!base.IsSubmission || (object)ScriptClass == null)
			{
				return null;
			}
			return ScriptClass.GetScriptInitializer();
		}

		public new bool ContainsSyntaxTree(SyntaxTree syntaxTree)
		{
			if (syntaxTree != null)
			{
				return _rootNamespaces.ContainsKey(syntaxTree);
			}
			return false;
		}

		public new VisualBasicCompilation AddSyntaxTrees(params SyntaxTree[] trees)
		{
			return AddSyntaxTrees((IEnumerable<SyntaxTree>)trees);
		}

		public new VisualBasicCompilation AddSyntaxTrees(IEnumerable<SyntaxTree> trees)
		{
			if (trees == null)
			{
				throw new ArgumentNullException("trees");
			}
			if (!trees.Any())
			{
				return this;
			}
			ArrayBuilder<SyntaxTree> instance = ArrayBuilder<SyntaxTree>.GetInstance();
			try
			{
				instance.AddRange(_syntaxTrees);
				bool referenceDirectivesChanged = false;
				int length = _syntaxTrees.Length;
				ImmutableDictionary<SyntaxTree, int> immutableDictionary = _syntaxTreeOrdinalMap;
				ImmutableDictionary<SyntaxTree, DeclarationTableEntry> declMap = _rootNamespaces;
				DeclarationTable declTable = _declarationTable;
				int num = 0;
				foreach (SyntaxTree tree in trees)
				{
					if (tree == null)
					{
						throw new ArgumentNullException(string.Format(VBResources.Trees0, num));
					}
					if (!tree.HasCompilationUnitRoot)
					{
						throw new ArgumentException(string.Format(VBResources.TreesMustHaveRootNode, num));
					}
					if (EmbeddedSymbolExtensions.IsEmbeddedOrMyTemplateTree(tree))
					{
						throw new ArgumentException(VBResources.CannotAddCompilerSpecialTree);
					}
					if (declMap.ContainsKey(tree))
					{
						throw new ArgumentException(VBResources.SyntaxTreeAlreadyPresent, string.Format(VBResources.Trees0, num));
					}
					AddSyntaxTreeToDeclarationMapAndTable(tree, _options, base.IsSubmission, ref declMap, ref declTable, ref referenceDirectivesChanged);
					instance.Add(tree);
					immutableDictionary = immutableDictionary.Add(tree, length + num);
					num++;
				}
				if (base.IsSubmission && declMap.Count > 1)
				{
					throw new ArgumentException(VBResources.SubmissionCanHaveAtMostOneSyntaxTree, "trees");
				}
				return UpdateSyntaxTrees(instance.ToImmutable(), immutableDictionary, declMap, declTable, referenceDirectivesChanged);
			}
			finally
			{
				instance.Free();
			}
		}

		private static void AddSyntaxTreeToDeclarationMapAndTable(SyntaxTree tree, VisualBasicCompilationOptions compilationOptions, bool isSubmission, ref ImmutableDictionary<SyntaxTree, DeclarationTableEntry> declMap, ref DeclarationTable declTable, ref bool referenceDirectivesChanged)
		{
			DeclarationTableEntry declarationTableEntry = new DeclarationTableEntry(new Lazy<RootSingleNamespaceDeclaration>(() => ForTree(tree, compilationOptions, isSubmission)), isEmbedded: false);
			declMap = declMap.Add(tree, declarationTableEntry);
			declTable = declTable.AddRootDeclaration(declarationTableEntry);
			referenceDirectivesChanged = referenceDirectivesChanged || VisualBasicExtensions.HasReferenceDirectives(tree);
		}

		private static RootSingleNamespaceDeclaration ForTree(SyntaxTree tree, VisualBasicCompilationOptions options, bool isSubmission)
		{
			return DeclarationTreeBuilder.ForTree(tree, options.GetRootNamespaceParts(), options.ScriptClassName ?? "", isSubmission);
		}

		public new VisualBasicCompilation RemoveSyntaxTrees(params SyntaxTree[] trees)
		{
			return RemoveSyntaxTrees((IEnumerable<SyntaxTree>)trees);
		}

		public new VisualBasicCompilation RemoveSyntaxTrees(IEnumerable<SyntaxTree> trees)
		{
			if (trees == null)
			{
				throw new ArgumentNullException("trees");
			}
			if (!trees.Any())
			{
				return this;
			}
			bool referenceDirectivesChanged = false;
			HashSet<SyntaxTree> hashSet = new HashSet<SyntaxTree>();
			ImmutableDictionary<SyntaxTree, DeclarationTableEntry> declMap = _rootNamespaces;
			DeclarationTable declTable = _declarationTable;
			foreach (SyntaxTree tree in trees)
			{
				if (EmbeddedSymbolExtensions.IsEmbeddedOrMyTemplateTree(tree))
				{
					throw new ArgumentException(VBResources.CannotRemoveCompilerSpecialTree);
				}
				RemoveSyntaxTreeFromDeclarationMapAndTable(tree, ref declMap, ref declTable, ref referenceDirectivesChanged);
				hashSet.Add(tree);
			}
			ImmutableDictionary<SyntaxTree, int> immutableDictionary = ImmutableDictionary.Create<SyntaxTree, int>();
			ArrayBuilder<SyntaxTree> instance = ArrayBuilder<SyntaxTree>.GetInstance();
			int num = 0;
			ImmutableArray<SyntaxTree>.Enumerator enumerator2 = _syntaxTrees.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				SyntaxTree current2 = enumerator2.Current;
				if (!hashSet.Contains(current2))
				{
					instance.Add(current2);
					immutableDictionary = immutableDictionary.Add(current2, num);
					num++;
				}
			}
			return UpdateSyntaxTrees(instance.ToImmutableAndFree(), immutableDictionary, declMap, declTable, referenceDirectivesChanged);
		}

		private static void RemoveSyntaxTreeFromDeclarationMapAndTable(SyntaxTree tree, ref ImmutableDictionary<SyntaxTree, DeclarationTableEntry> declMap, ref DeclarationTable declTable, ref bool referenceDirectivesChanged)
		{
			DeclarationTableEntry value = null;
			if (!declMap.TryGetValue(tree, out value))
			{
				throw new ArgumentException(string.Format(VBResources.SyntaxTreeNotFoundToRemove, tree));
			}
			declTable = declTable.RemoveRootDeclaration(value);
			declMap = declMap.Remove(tree);
			referenceDirectivesChanged = referenceDirectivesChanged || VisualBasicExtensions.HasReferenceDirectives(tree);
		}

		public new VisualBasicCompilation RemoveAllSyntaxTrees()
		{
			return UpdateSyntaxTrees(ImmutableArray<SyntaxTree>.Empty, ImmutableDictionary.Create<SyntaxTree, int>(), ImmutableDictionary.Create<SyntaxTree, DeclarationTableEntry>(), AddEmbeddedTrees(DeclarationTable.Empty, _embeddedTrees), _declarationTable.ReferenceDirectives.Any());
		}

		public new VisualBasicCompilation ReplaceSyntaxTree(SyntaxTree oldTree, SyntaxTree newTree)
		{
			if (oldTree == null)
			{
				throw new ArgumentNullException("oldTree");
			}
			if (newTree == null)
			{
				return RemoveSyntaxTrees(oldTree);
			}
			if (newTree == oldTree)
			{
				return this;
			}
			if (!newTree.HasCompilationUnitRoot)
			{
				throw new ArgumentException(VBResources.TreeMustHaveARootNodeWithCompilationUnit, "newTree");
			}
			if (EmbeddedSymbolExtensions.IsEmbeddedOrMyTemplateTree(oldTree))
			{
				throw new ArgumentException(VBResources.CannotRemoveCompilerSpecialTree);
			}
			if (EmbeddedSymbolExtensions.IsEmbeddedOrMyTemplateTree(newTree))
			{
				throw new ArgumentException(VBResources.CannotAddCompilerSpecialTree);
			}
			ImmutableDictionary<SyntaxTree, DeclarationTableEntry> declMap = _rootNamespaces;
			if (declMap.ContainsKey(newTree))
			{
				throw new ArgumentException(VBResources.SyntaxTreeAlreadyPresent, "newTree");
			}
			DeclarationTable declTable = _declarationTable;
			bool referenceDirectivesChanged = false;
			RemoveSyntaxTreeFromDeclarationMapAndTable(oldTree, ref declMap, ref declTable, ref referenceDirectivesChanged);
			AddSyntaxTreeToDeclarationMapAndTable(newTree, _options, base.IsSubmission, ref declMap, ref declTable, ref referenceDirectivesChanged);
			ImmutableDictionary<SyntaxTree, int> syntaxTreeOrdinalMap = _syntaxTreeOrdinalMap;
			int num = syntaxTreeOrdinalMap[oldTree];
			SyntaxTree[] array = _syntaxTrees.ToArray();
			array[num] = newTree;
			syntaxTreeOrdinalMap = syntaxTreeOrdinalMap.Remove(oldTree);
			syntaxTreeOrdinalMap = syntaxTreeOrdinalMap.Add(newTree, num);
			return UpdateSyntaxTrees(array.AsImmutableOrNull(), syntaxTreeOrdinalMap, declMap, declTable, referenceDirectivesChanged);
		}

		private static ImmutableArray<EmbeddedTreeAndDeclaration> CreateEmbeddedTrees(Lazy<VisualBasicCompilation> compReference)
		{
			return ImmutableArray.Create(new EmbeddedTreeAndDeclaration(delegate
			{
				VisualBasicCompilation value5 = compReference.Value;
				return (!(value5.Options.EmbedVbCoreRuntime | value5.IncludeInternalXmlHelper())) ? null : EmbeddedSymbolManager.EmbeddedSyntax;
			}, delegate
			{
				VisualBasicCompilation value4 = compReference.Value;
				return (!(value4.Options.EmbedVbCoreRuntime | value4.IncludeInternalXmlHelper())) ? null : ForTree(EmbeddedSymbolManager.EmbeddedSyntax, value4.Options, isSubmission: false);
			}), new EmbeddedTreeAndDeclaration(() => (!compReference.Value.Options.EmbedVbCoreRuntime) ? null : EmbeddedSymbolManager.VbCoreSyntaxTree, delegate
			{
				VisualBasicCompilation value3 = compReference.Value;
				return (!value3.Options.EmbedVbCoreRuntime) ? null : ForTree(EmbeddedSymbolManager.VbCoreSyntaxTree, value3.Options, isSubmission: false);
			}), new EmbeddedTreeAndDeclaration(() => (!compReference.Value.IncludeInternalXmlHelper()) ? null : EmbeddedSymbolManager.InternalXmlHelperSyntax, delegate
			{
				VisualBasicCompilation value2 = compReference.Value;
				return (!value2.IncludeInternalXmlHelper()) ? null : ForTree(EmbeddedSymbolManager.InternalXmlHelperSyntax, value2.Options, isSubmission: false);
			}), new EmbeddedTreeAndDeclaration(() => compReference.Value.MyTemplate, delegate
			{
				VisualBasicCompilation value = compReference.Value;
				return (value.MyTemplate == null) ? null : ForTree(value.MyTemplate, value.Options, isSubmission: false);
			}));
		}

		private static DeclarationTable AddEmbeddedTrees(DeclarationTable declTable, ImmutableArray<EmbeddedTreeAndDeclaration> embeddedTrees)
		{
			ImmutableArray<EmbeddedTreeAndDeclaration>.Enumerator enumerator = embeddedTrees.GetEnumerator();
			while (enumerator.MoveNext())
			{
				declTable = declTable.AddRootDeclaration(enumerator.Current.DeclarationEntry);
			}
			return declTable;
		}

		private static DeclarationTable RemoveEmbeddedTrees(DeclarationTable declTable, ImmutableArray<EmbeddedTreeAndDeclaration> embeddedTrees)
		{
			ImmutableArray<EmbeddedTreeAndDeclaration>.Enumerator enumerator = embeddedTrees.GetEnumerator();
			while (enumerator.MoveNext())
			{
				declTable = declTable.RemoveRootDeclaration(enumerator.Current.DeclarationEntry);
			}
			return declTable;
		}

		private bool IncludeInternalXmlHelper()
		{
			if (!Options.SuppressEmbeddedDeclarations && InternalXmlHelperDependencyIsSatisfied(WellKnownType.System_Linq_Enumerable) && InternalXmlHelperDependencyIsSatisfied(WellKnownType.System_Xml_Linq_XElement) && InternalXmlHelperDependencyIsSatisfied(WellKnownType.System_Xml_Linq_XName) && InternalXmlHelperDependencyIsSatisfied(WellKnownType.System_Xml_Linq_XAttribute))
			{
				return InternalXmlHelperDependencyIsSatisfied(WellKnownType.System_Xml_Linq_XNamespace);
			}
			return false;
		}

		private bool InternalXmlHelperDependencyIsSatisfied(WellKnownType type)
		{
			MetadataTypeName emittedName = MetadataTypeName.FromFullName(type.GetMetadataName(), useCLSCompliantNameArityEncoding: true);
			SourceAssemblySymbol sourceAssembly = SourceAssembly;
			ImmutableArray<AssemblySymbol>.Enumerator enumerator = sourceAssembly.SourceModule.GetReferencedAssemblySymbols().GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamedTypeSymbol namedTypeSymbol = enumerator.Current.LookupTopLevelMetadataType(ref emittedName, digThroughForwardedTypes: false);
				if (sourceAssembly.IsValidWellKnownType(namedTypeSymbol) && AssemblySymbol.IsAcceptableMatchForGetTypeByNameAndArity(namedTypeSymbol))
				{
					return true;
				}
			}
			return false;
		}

		internal override int CompareSourceLocations(Location first, Location second)
		{
			return LexicalSortKey.Compare(first, second, this);
		}

		internal override int CompareSourceLocations(SyntaxReference first, SyntaxReference second)
		{
			return LexicalSortKey.Compare(first, second, this);
		}

		internal override int GetSyntaxTreeOrdinal(SyntaxTree tree)
		{
			return _syntaxTreeOrdinalMap[tree];
		}

		internal override CommonReferenceManager CommonGetBoundReferenceManager()
		{
			return GetBoundReferenceManager();
		}

		internal new ReferenceManager GetBoundReferenceManager()
		{
			if ((object)_lazyAssemblySymbol == null)
			{
				_referenceManager.CreateSourceAssemblyForCompilation(this);
			}
			return _referenceManager;
		}

		internal bool ReferenceManagerEquals(VisualBasicCompilation other)
		{
			return _referenceManager == other._referenceManager;
		}

		internal new Symbol GetAssemblyOrModuleSymbol(MetadataReference reference)
		{
			if (reference == null)
			{
				throw new ArgumentNullException("reference");
			}
			if (reference.Properties.Kind == MetadataImageKind.Assembly)
			{
				return GetBoundReferenceManager().GetReferencedAssemblySymbol(reference);
			}
			int referencedModuleIndex = GetBoundReferenceManager().GetReferencedModuleIndex(reference);
			return (referencedModuleIndex < 0) ? null : Assembly.Modules[referencedModuleIndex];
		}

		internal MetadataReference GetMetadataReference(AssemblySymbol assemblySymbol)
		{
			return GetBoundReferenceManager().GetMetadataReference(assemblySymbol);
		}

		private protected override MetadataReference CommonGetMetadataReference(IAssemblySymbol assemblySymbol)
		{
			if (assemblySymbol is AssemblySymbol assemblySymbol2)
			{
				return GetMetadataReference(assemblySymbol2);
			}
			return null;
		}

		public override CompilationReference ToMetadataReference(ImmutableArray<string> aliases = default(ImmutableArray<string>), bool embedInteropTypes = false)
		{
			return new VisualBasicCompilationReference(this, aliases, embedInteropTypes);
		}

		public new VisualBasicCompilation AddReferences(params MetadataReference[] references)
		{
			return (VisualBasicCompilation)base.AddReferences(references);
		}

		public new VisualBasicCompilation AddReferences(IEnumerable<MetadataReference> references)
		{
			return (VisualBasicCompilation)base.AddReferences(references);
		}

		public new VisualBasicCompilation RemoveReferences(params MetadataReference[] references)
		{
			return (VisualBasicCompilation)base.RemoveReferences(references);
		}

		public new VisualBasicCompilation RemoveReferences(IEnumerable<MetadataReference> references)
		{
			return (VisualBasicCompilation)base.RemoveReferences(references);
		}

		public new VisualBasicCompilation RemoveAllReferences()
		{
			return (VisualBasicCompilation)base.RemoveAllReferences();
		}

		public new VisualBasicCompilation ReplaceReference(MetadataReference oldReference, MetadataReference newReference)
		{
			return (VisualBasicCompilation)base.ReplaceReference(oldReference, newReference);
		}

		internal new NamespaceSymbol GetCompilationNamespace(INamespaceSymbol namespaceSymbol)
		{
			if (namespaceSymbol == null)
			{
				throw new ArgumentNullException("namespaceSymbol");
			}
			if (namespaceSymbol is NamespaceSymbol namespaceSymbol2 && namespaceSymbol2.Extent.Kind == NamespaceKind.Compilation && namespaceSymbol2.Extent.Compilation == this)
			{
				return namespaceSymbol2;
			}
			if (namespaceSymbol.ContainingNamespace == null)
			{
				return GlobalNamespace;
			}
			return GetCompilationNamespace(namespaceSymbol.ContainingNamespace)?.GetMembers(namespaceSymbol.Name).OfType<NamespaceSymbol>().FirstOrDefault();
		}

		internal new MethodSymbol GetEntryPoint(CancellationToken cancellationToken)
		{
			return GetEntryPointAndDiagnostics(cancellationToken)?.MethodSymbol;
		}

		internal EntryPoint GetEntryPointAndDiagnostics(CancellationToken cancellationToken)
		{
			if (!Options.OutputKind.IsApplication() && (object)ScriptClass == null)
			{
				return null;
			}
			if (Options.MainTypeName != null && !Options.MainTypeName.IsValidClrTypeName())
			{
				return new EntryPoint(null, ImmutableArray<Diagnostic>.Empty);
			}
			if (_lazyEntryPoint == null)
			{
				ImmutableArray<Diagnostic> sealedDiagnostics = default(ImmutableArray<Diagnostic>);
				MethodSymbol methodSymbol = FindEntryPoint(cancellationToken, ref sealedDiagnostics);
				Interlocked.CompareExchange(ref _lazyEntryPoint, new EntryPoint(methodSymbol, sealedDiagnostics), null);
			}
			return _lazyEntryPoint;
		}

		private MethodSymbol FindEntryPoint(CancellationToken cancellationToken, ref ImmutableArray<Diagnostic> sealedDiagnostics)
		{
			DiagnosticBag instance = DiagnosticBag.GetInstance();
			ArrayBuilder<MethodSymbol> instance2 = ArrayBuilder<MethodSymbol>.GetInstance();
			try
			{
				string mainTypeName = Options.MainTypeName;
				NamespaceSymbol globalNamespace = SourceModule.GlobalNamespace;
				object obj;
				if (mainTypeName != null)
				{
					if ((object)ScriptClass != null)
					{
						DiagnosticBagExtensions.Add(instance, ERRID.WRN_MainIgnored, NoLocation.Singleton, mainTypeName);
						return ScriptClass.GetScriptEntryPoint();
					}
					NamespaceOrTypeSymbol namespaceOrTypeSymbol = SymbolExtensions.OfMinimalArity(globalNamespace.GetNamespaceOrTypeByQualifiedName(mainTypeName.Split(new char[1] { '.' })).OfType<NamedTypeSymbol>());
					if ((object)namespaceOrTypeSymbol == null)
					{
						DiagnosticBagExtensions.Add(instance, ERRID.ERR_StartupCodeNotFound1, NoLocation.Singleton, mainTypeName);
						return null;
					}
					SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol = namespaceOrTypeSymbol as SourceMemberContainerTypeSymbol;
					if ((object)sourceMemberContainerTypeSymbol == null || (sourceMemberContainerTypeSymbol.TypeKind != TypeKind.Class && sourceMemberContainerTypeSymbol.TypeKind != TypeKind.Struct && sourceMemberContainerTypeSymbol.TypeKind != TypeKind.Module))
					{
						DiagnosticBagExtensions.Add(instance, ERRID.ERR_StartupCodeNotFound1, NoLocation.Singleton, sourceMemberContainerTypeSymbol);
						return null;
					}
					if (sourceMemberContainerTypeSymbol.IsGenericType)
					{
						DiagnosticBagExtensions.Add(instance, ERRID.ERR_GenericSubMainsFound1, NoLocation.Singleton, sourceMemberContainerTypeSymbol);
						return null;
					}
					obj = sourceMemberContainerTypeSymbol;
					Binder binder = BinderBuilder.CreateBinderForType(sourceMemberContainerTypeSymbol.ContainingSourceModule, sourceMemberContainerTypeSymbol.SyntaxReferences[0].SyntaxTree, sourceMemberContainerTypeSymbol);
					LookupResult instance3 = LookupResult.GetInstance();
					LookupOptions options = LookupOptions.AllMethodsOfAnyArity | LookupOptions.IgnoreExtensionMethods;
					SourceMemberContainerTypeSymbol container = sourceMemberContainerTypeSymbol;
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
					binder.LookupMember(instance3, container, "Main", 0, options, ref useSiteInfo);
					if (!instance3.IsGoodOrAmbiguous || instance3.Symbols[0].Kind != SymbolKind.Method)
					{
						DiagnosticBagExtensions.Add(instance, ERRID.ERR_StartupCodeNotFound1, NoLocation.Singleton, sourceMemberContainerTypeSymbol);
						instance3.Free();
						return null;
					}
					ArrayBuilder<Symbol>.Enumerator enumerator = instance3.Symbols.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Symbol current = enumerator.Current;
						if (current.ContainingAssembly == Assembly)
						{
							instance2.Add((MethodSymbol)current);
						}
					}
					instance3.Free();
				}
				else
				{
					SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol = null;
					obj = base.AssemblyName;
					foreach (ISymbol item in GetSymbolsWithName("Main", SymbolFilter.Member, cancellationToken))
					{
						if (item is MethodSymbol methodSymbol && methodSymbol.IsEntryPointCandidate)
						{
							instance2.Add(methodSymbol);
						}
					}
					if ((object)ScriptClass != null)
					{
						ArrayBuilder<MethodSymbol>.Enumerator enumerator3 = instance2.GetEnumerator();
						while (enumerator3.MoveNext())
						{
							MethodSymbol current2 = enumerator3.Current;
							DiagnosticBagExtensions.Add(instance, ERRID.WRN_MainIgnored, current2.Locations.First(), current2);
						}
						return ScriptClass.GetScriptEntryPoint();
					}
				}
				if (instance2.Count == 0)
				{
					DiagnosticBagExtensions.Add(instance, ERRID.ERR_StartupCodeNotFound1, NoLocation.Singleton, obj);
					return null;
				}
				bool flag = false;
				ArrayBuilder<MethodSymbol> instance4 = ArrayBuilder<MethodSymbol>.GetInstance();
				ArrayBuilder<MethodSymbol>.Enumerator enumerator4 = instance2.GetEnumerator();
				while (enumerator4.MoveNext())
				{
					MethodSymbol current3 = enumerator4.Current;
					if (current3.IsViableMainMethod)
					{
						if (current3.IsGenericMethod || current3.ContainingType.IsGenericType)
						{
							flag = true;
						}
						else
						{
							instance4.Add(current3);
						}
					}
				}
				MethodSymbol methodSymbol2 = null;
				if (instance4.Count == 0)
				{
					if (flag)
					{
						DiagnosticBagExtensions.Add(instance, ERRID.ERR_GenericSubMainsFound1, NoLocation.Singleton, obj);
					}
					else
					{
						DiagnosticBagExtensions.Add(instance, ERRID.ERR_InValidSubMainsFound1, NoLocation.Singleton, obj);
					}
				}
				else if (instance4.Count > 1)
				{
					instance4.Sort(LexicalOrderSymbolComparer.Instance);
					DiagnosticBagExtensions.Add(instance, ERRID.ERR_MoreThanOneValidMainWasFound2, NoLocation.Singleton, base.AssemblyName, new FormattedSymbolList(instance4.ToArray(), CustomSymbolDisplayFormatter.ErrorMessageFormatNoModifiersNoReturnType));
				}
				else
				{
					methodSymbol2 = instance4[0];
					if (methodSymbol2.IsAsync && methodSymbol2 is SourceMemberMethodSymbol sourceMemberMethodSymbol)
					{
						Location nonMergedLocation = sourceMemberMethodSymbol.NonMergedLocation;
						if ((object)nonMergedLocation != null)
						{
							Binder.ReportDiagnostic(instance, nonMergedLocation, ERRID.ERR_AsyncSubMain);
						}
					}
				}
				instance4.Free();
				return methodSymbol2;
			}
			finally
			{
				instance2.Free();
				sealedDiagnostics = instance.ToReadOnlyAndFree();
			}
		}

		internal override void ReportUnusedImports(DiagnosticBag diagnostics, CancellationToken cancellationToken)
		{
			ReportUnusedImports(null, new BindingDiagnosticBag(diagnostics), cancellationToken);
		}

		private void ReportUnusedImports(SyntaxTree filterTree, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken)
		{
			if (_lazyImportInfos != null && (filterTree == null || Compilation.ReportUnusedImportsInTree(filterTree)))
			{
				ArrayBuilder<TextSpan> arrayBuilder = null;
				foreach (ImportInfo lazyImportInfo in _lazyImportInfos)
				{
					cancellationToken.ThrowIfCancellationRequested();
					SyntaxTree tree = lazyImportInfo.Tree;
					if ((filterTree != null && filterTree != tree) || !Compilation.ReportUnusedImportsInTree(tree))
					{
						continue;
					}
					ImmutableArray<TextSpan> clauseSpans = lazyImportInfo.ClauseSpans;
					int length = clauseSpans.Length;
					if (length == 1)
					{
						if (!IsImportDirectiveUsed(tree, clauseSpans[0].Start))
						{
							diagnostics.Add(ERRID.HDN_UnusedImportStatement, tree.GetLocation(lazyImportInfo.StatementSpan));
						}
						else
						{
							AddImportsDependencies(diagnostics, tree, clauseSpans[0]);
						}
						continue;
					}
					arrayBuilder?.Clear();
					ImmutableArray<TextSpan>.Enumerator enumerator2 = lazyImportInfo.ClauseSpans.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						TextSpan current2 = enumerator2.Current;
						if (!IsImportDirectiveUsed(tree, current2.Start))
						{
							if (arrayBuilder == null)
							{
								arrayBuilder = ArrayBuilder<TextSpan>.GetInstance();
							}
							arrayBuilder.Add(current2);
						}
						else
						{
							AddImportsDependencies(diagnostics, tree, current2);
						}
					}
					if (arrayBuilder == null || arrayBuilder.Count <= 0)
					{
						continue;
					}
					if (arrayBuilder.Count == length)
					{
						diagnostics.Add(ERRID.HDN_UnusedImportStatement, tree.GetLocation(lazyImportInfo.StatementSpan));
						continue;
					}
					ArrayBuilder<TextSpan>.Enumerator enumerator3 = arrayBuilder.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						TextSpan current3 = enumerator3.Current;
						diagnostics.Add(ERRID.HDN_UnusedImportClause, tree.GetLocation(current3));
					}
				}
				arrayBuilder?.Free();
			}
			CompleteTrees(filterTree);
		}

		private void AddImportsDependencies(BindingDiagnosticBag diagnostics, SyntaxTree infoTree, TextSpan clauseSpan)
		{
			ImmutableArray<AssemblySymbol> value = default(ImmutableArray<AssemblySymbol>);
			if (diagnostics.AccumulatesDependencies && _lazyImportClauseDependencies != null && _lazyImportClauseDependencies.TryGetValue((infoTree, clauseSpan.Start), out value))
			{
				diagnostics.AddDependencies(value);
			}
		}

		internal override void CompleteTrees(SyntaxTree filterTree)
		{
			if (base.EventQueue == null)
			{
				return;
			}
			if (filterTree != null)
			{
				CompleteTree(filterTree);
				return;
			}
			ImmutableArray<SyntaxTree>.Enumerator enumerator = SyntaxTrees.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxTree current = enumerator.Current;
				CompleteTree(current);
			}
		}

		private void CompleteTree(SyntaxTree tree)
		{
			if (EmbeddedSymbolExtensions.IsEmbeddedOrMyTemplateTree(tree))
			{
				return;
			}
			if (_lazyCompilationUnitCompletedTrees == null)
			{
				Interlocked.CompareExchange(ref _lazyCompilationUnitCompletedTrees, new HashSet<SyntaxTree>(), null);
			}
			lock (_lazyCompilationUnitCompletedTrees)
			{
				if (_lazyCompilationUnitCompletedTrees.Add(tree))
				{
					base.EventQueue!.TryEnqueue(new CompilationUnitCompletedEvent(this, tree));
					if (_lazyCompilationUnitCompletedTrees.Count == SyntaxTrees.Length)
					{
						CompleteCompilationEventQueue_NoLock();
					}
				}
			}
		}

		internal bool ShouldAddEvent(Symbol symbol)
		{
			if (base.EventQueue != null)
			{
				return symbol.IsInSource();
			}
			return false;
		}

		internal void SymbolDeclaredEvent(Symbol symbol)
		{
			if (ShouldAddEvent(symbol))
			{
				base.EventQueue!.TryEnqueue(new SymbolDeclaredCompilationEvent(this, symbol));
			}
		}

		internal void RecordImportsClauseDependencies(SyntaxTree syntaxTree, int importsClausePosition, ImmutableArray<AssemblySymbol> dependencies)
		{
			if (!dependencies.IsDefaultOrEmpty)
			{
				LazyInitializer.EnsureInitialized(ref _lazyImportClauseDependencies).TryAdd((syntaxTree, importsClausePosition), dependencies);
			}
		}

		internal void RecordImports(ImportsStatementSyntax syntax)
		{
			LazyInitializer.EnsureInitialized(ref _lazyImportInfos).Enqueue(new ImportInfo(syntax));
		}

		internal bool MightContainNoPiaLocalTypes()
		{
			return SourceAssembly.MightContainNoPiaLocalTypes();
		}

		public Conversion ClassifyConversion(ITypeSymbol source, ITypeSymbol destination)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			TypeSymbol typeSymbol = SymbolExtensions.EnsureVbSymbolOrNothing<ITypeSymbol, TypeSymbol>(source, "source");
			TypeSymbol typeSymbol2 = SymbolExtensions.EnsureVbSymbolOrNothing<ITypeSymbol, TypeSymbol>(destination, "destination");
			Conversion result;
			if (TypeSymbolExtensions.IsErrorType(typeSymbol) || TypeSymbolExtensions.IsErrorType(typeSymbol2))
			{
				result = new Conversion(default(KeyValuePair<ConversionKind, MethodSymbol>));
			}
			else
			{
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				result = new Conversion(Conversions.ClassifyConversion(typeSymbol, typeSymbol2, ref useSiteInfo));
			}
			return result;
		}

		public override CommonConversion ClassifyCommonConversion(ITypeSymbol source, ITypeSymbol destination)
		{
			return ClassifyConversion(source, destination).ToCommonConversion();
		}

		internal override IConvertibleConversion ClassifyConvertibleConversion(IOperation source, ITypeSymbol destination, ref ConstantValue constantValue)
		{
			constantValue = null;
			if (destination == null)
			{
				return new Conversion(default(KeyValuePair<ConversionKind, MethodSymbol>));
			}
			ITypeSymbol type = source.Type;
			ConstantValue constantValue2 = source.GetConstantValue();
			if (type == null)
			{
				if ((object)constantValue2 != null && constantValue2.IsNothing && destination.IsReferenceType)
				{
					constantValue = constantValue2;
					return new Conversion(new KeyValuePair<ConversionKind, MethodSymbol>(ConversionKind.WideningNothingLiteral, null));
				}
				return new Conversion(default(KeyValuePair<ConversionKind, MethodSymbol>));
			}
			Conversion conversion = ClassifyConversion(type, destination);
			if (conversion.IsReference && (object)constantValue2 != null && constantValue2.IsNothing)
			{
				constantValue = constantValue2;
			}
			return conversion;
		}

		private ImplicitNamedTypeSymbol BindScriptClass()
		{
			return (ImplicitNamedTypeSymbol)CommonBindScriptClass();
		}

		internal new NamedTypeSymbol GetSpecialType(SpecialType typeId)
		{
			return Assembly.GetSpecialType(typeId);
		}

		internal Symbol GetSpecialTypeMember(SpecialMember memberId)
		{
			return Assembly.GetSpecialTypeMember(memberId);
		}

		internal override ISymbolInternal CommonGetSpecialTypeMember(SpecialMember specialMember)
		{
			return GetSpecialTypeMember(specialMember);
		}

		internal TypeSymbol GetTypeByReflectionType(Type type)
		{
			return GetSpecialType(SpecialType.System_Object);
		}

		internal new NamedTypeSymbol GetTypeByMetadataName(string fullyQualifiedMetadataName)
		{
			AssemblySymbol assembly = Assembly;
			(AssemblySymbol, AssemblySymbol) conflicts = default((AssemblySymbol, AssemblySymbol));
			return assembly.GetTypeByMetadataName(fullyQualifiedMetadataName, includeReferences: true, isWellKnownType: false, out conflicts);
		}

		internal ArrayTypeSymbol CreateArrayTypeSymbol(TypeSymbol elementType, int rank = 1)
		{
			if ((object)elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (rank < 1)
			{
				throw new ArgumentException("rank");
			}
			return ArrayTypeSymbol.CreateVBArray(elementType, default(ImmutableArray<CustomModifier>), rank, this);
		}

		private protected override bool IsSymbolAccessibleWithinCore(ISymbol symbol, ISymbol within, ITypeSymbol throughType)
		{
			Symbol symbol2 = SymbolExtensions.EnsureVbSymbolOrNothing<ISymbol, Symbol>(symbol, "symbol");
			Symbol symbol3 = SymbolExtensions.EnsureVbSymbolOrNothing<ISymbol, Symbol>(within, "within");
			TypeSymbol throughTypeOpt = SymbolExtensions.EnsureVbSymbolOrNothing<ITypeSymbol, TypeSymbol>(throughType, "throughType");
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo;
			if (symbol3.Kind != SymbolKind.Assembly)
			{
				NamedTypeSymbol within2 = (NamedTypeSymbol)symbol3;
				useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				return AccessCheck.IsSymbolAccessible(symbol2, within2, throughTypeOpt, ref useSiteInfo);
			}
			AssemblySymbol within3 = (AssemblySymbol)symbol3;
			useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
			return AccessCheck.IsSymbolAccessible(symbol2, within3, ref useSiteInfo);
		}

		[Obsolete("Compilation.IsSymbolAccessibleWithin is not designed for use within the compilers", true)]
		internal new bool IsSymbolAccessibleWithin(ISymbol symbol, ISymbol within, ITypeSymbol throughType = null)
		{
			throw new NotImplementedException();
		}

		public new SemanticModel GetSemanticModel(SyntaxTree syntaxTree, bool ignoreAccessibility = false)
		{
			SemanticModel semanticModel = null;
			if (base.SemanticModelProvider != null)
			{
				semanticModel = base.SemanticModelProvider!.GetSemanticModel(syntaxTree, this, ignoreAccessibility);
			}
			return semanticModel ?? CreateSemanticModel(syntaxTree, ignoreAccessibility);
		}

		internal override SemanticModel CreateSemanticModel(SyntaxTree syntaxTree, bool ignoreAccessibility)
		{
			return new SyntaxTreeSemanticModel(this, (SourceModuleSymbol)SourceModule, syntaxTree, ignoreAccessibility);
		}

		public override ImmutableArray<Diagnostic> GetDiagnostics(CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetDiagnostics(CompilationStage.Compile, includeEarlierStages: true, cancellationToken);
		}

		public override ImmutableArray<Diagnostic> GetParseDiagnostics(CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetDiagnostics(CompilationStage.Parse, includeEarlierStages: false, cancellationToken);
		}

		public override ImmutableArray<Diagnostic> GetDeclarationDiagnostics(CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetDiagnostics(CompilationStage.Declare, includeEarlierStages: false, cancellationToken);
		}

		public override ImmutableArray<Diagnostic> GetMethodBodyDiagnostics(CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetDiagnostics(CompilationStage.Compile, includeEarlierStages: false, cancellationToken);
		}

		internal ImmutableArray<Diagnostic> GetDiagnostics(CompilationStage stage, bool includeEarlierStages = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			DiagnosticBag instance = DiagnosticBag.GetInstance();
			GetDiagnostics(stage, includeEarlierStages, instance, cancellationToken);
			return instance.ToReadOnlyAndFree();
		}

		internal override void GetDiagnostics(CompilationStage stage, bool includeEarlierStages, DiagnosticBag diagnostics, CancellationToken cancellationToken = default(CancellationToken))
		{
			DiagnosticBag incoming = DiagnosticBag.GetInstance();
			GetDiagnosticsWithoutFiltering(stage, includeEarlierStages, new BindingDiagnosticBag(incoming), cancellationToken);
			FilterAndAppendAndFreeDiagnostics(diagnostics, ref incoming, cancellationToken);
		}

		private void GetDiagnosticsWithoutFiltering(CompilationStage stage, bool includeEarlierStages, BindingDiagnosticBag builder, CancellationToken cancellationToken = default(CancellationToken))
		{
			_Closure_0024__183_002D0 arg = default(_Closure_0024__183_002D0);
			_Closure_0024__183_002D0 CS_0024_003C_003E8__locals0 = new _Closure_0024__183_002D0(arg);
			CS_0024_003C_003E8__locals0._0024VB_0024Me = this;
			CS_0024_003C_003E8__locals0._0024VB_0024Local_builder = builder;
			CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken = cancellationToken;
			if (stage == CompilationStage.Parse || (stage > CompilationStage.Parse && includeEarlierStages))
			{
				if (Options.ConcurrentBuild)
				{
					RoslynParallel.For(0, SyntaxTrees.Length, UICultureUtilities.WithCurrentUICulture(delegate(int i)
					{
						CS_0024_003C_003E8__locals0._0024VB_0024Local_builder.AddRange(CS_0024_003C_003E8__locals0._0024VB_0024Me.SyntaxTrees[i].GetDiagnostics(CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken));
					}), CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken);
				}
				else
				{
					ImmutableArray<SyntaxTree>.Enumerator enumerator = SyntaxTrees.GetEnumerator();
					while (enumerator.MoveNext())
					{
						SyntaxTree current = enumerator.Current;
						CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken.ThrowIfCancellationRequested();
						CS_0024_003C_003E8__locals0._0024VB_0024Local_builder.AddRange(current.GetDiagnostics(CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken));
					}
				}
				HashSet<ParseOptions> hashSet = new HashSet<ParseOptions>();
				if ((object)Options.ParseOptions != null)
				{
					hashSet.Add(Options.ParseOptions);
				}
				ImmutableArray<SyntaxTree>.Enumerator enumerator2 = SyntaxTrees.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					SyntaxTree current2 = enumerator2.Current;
					CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken.ThrowIfCancellationRequested();
					if (!current2.Options.Errors.IsDefaultOrEmpty && hashSet.Add(current2.Options))
					{
						Location location = current2.GetLocation(TextSpan.FromBounds(0, 0));
						ImmutableArray<Diagnostic>.Enumerator enumerator3 = current2.Options.Errors.GetEnumerator();
						while (enumerator3.MoveNext())
						{
							Diagnostic current3 = enumerator3.Current;
							CS_0024_003C_003E8__locals0._0024VB_0024Local_builder.Add(current3.WithLocation(location));
						}
					}
				}
			}
			if (stage == CompilationStage.Declare || (stage > CompilationStage.Declare && includeEarlierStages))
			{
				CheckAssemblyName(CS_0024_003C_003E8__locals0._0024VB_0024Local_builder.DiagnosticBag);
				CS_0024_003C_003E8__locals0._0024VB_0024Local_builder.AddRange(Options.Errors);
				CS_0024_003C_003E8__locals0._0024VB_0024Local_builder.AddRange(GetBoundReferenceManager().Diagnostics);
				SourceAssembly.GetAllDeclarationErrors(CS_0024_003C_003E8__locals0._0024VB_0024Local_builder, CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken);
				AddClsComplianceDiagnostics(CS_0024_003C_003E8__locals0._0024VB_0024Local_builder, CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken);
				if (base.EventQueue != null && SyntaxTrees.Length == 0)
				{
					EnsureCompilationEventQueueCompleted();
				}
			}
			if (stage == CompilationStage.Compile || (stage > CompilationStage.Compile && includeEarlierStages))
			{
				BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag(DiagnosticBag.GetInstance(), CS_0024_003C_003E8__locals0._0024VB_0024Local_builder.AccumulatesDependencies ? new ConcurrentSet<AssemblySymbol>() : null);
				GetDiagnosticsForAllMethodBodies(CS_0024_003C_003E8__locals0._0024VB_0024Local_builder.HasAnyErrors(), bindingDiagnosticBag, doLowering: false, CS_0024_003C_003E8__locals0._0024VB_0024Local_cancellationToken);
				CS_0024_003C_003E8__locals0._0024VB_0024Local_builder.AddRange(bindingDiagnosticBag);
				bindingDiagnosticBag.DiagnosticBag!.Free();
			}
		}

		private void AddClsComplianceDiagnostics(BindingDiagnosticBag diagnostics, CancellationToken cancellationToken, SyntaxTree filterTree = null, TextSpan? filterSpanWithinTree = null)
		{
			if (filterTree != null)
			{
				ClsComplianceChecker.CheckCompliance(this, diagnostics, cancellationToken, filterTree, filterSpanWithinTree);
				return;
			}
			if (_lazyClsComplianceDiagnostics.IsDefault || _lazyClsComplianceDependencies.IsDefault)
			{
				BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
				ClsComplianceChecker.CheckCompliance(this, instance, cancellationToken);
				ImmutableBindingDiagnostic<AssemblySymbol> immutableBindingDiagnostic = instance.ToReadOnlyAndFree();
				ImmutableInterlocked.InterlockedInitialize(ref _lazyClsComplianceDependencies, immutableBindingDiagnostic.Dependencies);
				ImmutableInterlocked.InterlockedInitialize(ref _lazyClsComplianceDiagnostics, immutableBindingDiagnostic.Diagnostics);
			}
			diagnostics.AddRange(new ImmutableBindingDiagnostic<AssemblySymbol>(_lazyClsComplianceDiagnostics, _lazyClsComplianceDependencies), allowMismatchInDependencyAccumulation: true);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_185_FilterDiagnosticsByLocation))]
		private static IEnumerable<Diagnostic> FilterDiagnosticsByLocation(IEnumerable<Diagnostic> diagnostics, SyntaxTree tree, TextSpan? filterSpanWithinTree)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_185_FilterDiagnosticsByLocation(-2)
			{
				_0024P_diagnostics = diagnostics,
				_0024P_tree = tree,
				_0024P_filterSpanWithinTree = filterSpanWithinTree
			};
		}

		internal ImmutableArray<Diagnostic> GetDiagnosticsForSyntaxTree(CompilationStage stage, SyntaxTree tree, TextSpan? filterSpanWithinTree, bool includeEarlierStages, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!SyntaxTrees.Contains(tree))
			{
				throw new ArgumentException("Cannot GetDiagnosticsForSyntax for a tree that is not part of the compilation", "tree");
			}
			BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag(DiagnosticBag.GetInstance());
			if (stage == CompilationStage.Parse || (stage > CompilationStage.Parse && includeEarlierStages))
			{
				cancellationToken.ThrowIfCancellationRequested();
				IEnumerable<Diagnostic> diagnostics = tree.GetDiagnostics(cancellationToken);
				diagnostics = FilterDiagnosticsByLocation(diagnostics, tree, filterSpanWithinTree);
				bindingDiagnosticBag.AddRange(diagnostics);
			}
			if (stage == CompilationStage.Declare || (stage > CompilationStage.Declare && includeEarlierStages))
			{
				IEnumerable<Diagnostic> diagnostics2 = FilterDiagnosticsByLocation(((SourceModuleSymbol)SourceModule).GetDeclarationErrorsInTree(tree, filterSpanWithinTree, FilterDiagnosticsByLocation, cancellationToken), tree, filterSpanWithinTree);
				bindingDiagnosticBag.AddRange(diagnostics2);
				AddClsComplianceDiagnostics(bindingDiagnosticBag, cancellationToken, tree, filterSpanWithinTree);
			}
			if (stage == CompilationStage.Compile || (stage > CompilationStage.Compile && includeEarlierStages))
			{
				BindingDiagnosticBag bindingDiagnosticBag2 = new BindingDiagnosticBag(DiagnosticBag.GetInstance());
				GetDiagnosticsForMethodBodiesInTree(tree, filterSpanWithinTree, bindingDiagnosticBag.HasAnyErrors(), bindingDiagnosticBag2, cancellationToken);
				if (!bindingDiagnosticBag2.DiagnosticBag!.IsEmptyWithoutResolution)
				{
					IEnumerable<Diagnostic> enumerable = FilterDiagnosticsByLocation(bindingDiagnosticBag2.DiagnosticBag!.AsEnumerableWithoutResolution(), tree, filterSpanWithinTree);
					foreach (Diagnostic item in enumerable)
					{
						bindingDiagnosticBag.Add(item);
					}
				}
			}
			DiagnosticBag instance = DiagnosticBag.GetInstance();
			DiagnosticBag incoming = bindingDiagnosticBag.DiagnosticBag;
			FilterAndAppendAndFreeDiagnostics(instance, ref incoming, cancellationToken);
			return instance.ToReadOnlyAndFree<Diagnostic>();
		}

		private void GetDiagnosticsForAllMethodBodies(bool hasDeclarationErrors, BindingDiagnosticBag diagnostics, bool doLowering, CancellationToken cancellationToken)
		{
			MethodCompiler.GetCompileDiagnostics(this, SourceModule.GlobalNamespace, null, null, hasDeclarationErrors, diagnostics, doLowering, cancellationToken);
			DocumentationCommentCompiler.WriteDocumentationCommentXml(this, null, null, diagnostics, cancellationToken);
			ReportUnusedImports(null, diagnostics, cancellationToken);
		}

		private void GetDiagnosticsForMethodBodiesInTree(SyntaxTree tree, TextSpan? filterSpanWithinTree, bool hasDeclarationErrors, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken)
		{
			_ = (SourceModuleSymbol)SourceModule;
			MethodCompiler.GetCompileDiagnostics(this, SourceModule.GlobalNamespace, tree, filterSpanWithinTree, hasDeclarationErrors, diagnostics, doLoweringPhase: false, cancellationToken);
			DocumentationCommentCompiler.WriteDocumentationCommentXml(this, null, null, diagnostics, cancellationToken, tree, filterSpanWithinTree);
			if (!filterSpanWithinTree.HasValue || filterSpanWithinTree.Value == tree.GetRoot(cancellationToken).FullSpan)
			{
				ReportUnusedImports(tree, diagnostics, cancellationToken);
			}
		}

		internal override AnalyzerDriver CreateAnalyzerDriver(ImmutableArray<DiagnosticAnalyzer> analyzers, AnalyzerManager analyzerManager, SeverityFilter severityFilter)
		{
			Func<SyntaxNode, SyntaxKind> getKind = (SyntaxNode node) => VisualBasicExtensions.Kind(node);
			Func<SyntaxTrivia, bool> isComment = (SyntaxTrivia trivia) => VisualBasicExtensions.Kind(trivia) == SyntaxKind.CommentTrivia;
			return new AnalyzerDriver<SyntaxKind>(analyzers, getKind, analyzerManager, severityFilter, isComment);
		}

		protected override void AppendDefaultVersionResource(Stream resourceStream)
		{
			string text = SourceAssembly.FileVersion ?? SourceAssembly.Identity.Version.ToString();
			Win32ResourceConversions.AppendVersionToResourceStream(resourceStream, !Options.OutputKind.IsApplication(), text, SourceModule.Name, SourceModule.Name, SourceAssembly.InformationalVersion ?? text, SourceAssembly.Identity.Version, SourceAssembly.Title ?? " ", SourceAssembly.Copyright ?? " ", SourceAssembly.Trademark, SourceAssembly.Product, SourceAssembly.Description, SourceAssembly.Company);
		}

		internal override CommonPEModuleBuilder CreateModuleBuilder(EmitOptions emitOptions, IMethodSymbol debugEntryPoint, Stream sourceLinkStream, IEnumerable<EmbeddedText> embeddedTexts, IEnumerable<ResourceDescription> manifestResources, CompilationTestData testData, DiagnosticBag diagnostics, CancellationToken cancellationToken)
		{
			return CreateModuleBuilder(emitOptions, debugEntryPoint, sourceLinkStream, embeddedTexts, manifestResources, testData, diagnostics, ImmutableArray<NamedTypeSymbol>.Empty, cancellationToken);
		}

		internal CommonPEModuleBuilder CreateModuleBuilder(EmitOptions emitOptions, IMethodSymbol debugEntryPoint, Stream sourceLinkStream, IEnumerable<EmbeddedText> embeddedTexts, IEnumerable<ResourceDescription> manifestResources, CompilationTestData testData, DiagnosticBag diagnostics, ImmutableArray<NamedTypeSymbol> additionalTypes, CancellationToken cancellationToken)
		{
			string runtimeMetadataVersion = GetRuntimeMetadataVersion();
			ModulePropertiesForSerialization serializationProperties = ConstructModuleSerializationProperties(emitOptions, runtimeMetadataVersion);
			if (manifestResources == null)
			{
				manifestResources = SpecializedCollections.EmptyEnumerable<ResourceDescription>();
			}
			PEModuleBuilder pEModuleBuilder;
			if (Options.OutputKind.IsNetModule())
			{
				pEModuleBuilder = new PENetModuleBuilder((SourceModuleSymbol)SourceModule, emitOptions, serializationProperties, manifestResources);
			}
			else
			{
				OutputKind outputKind = (Options.OutputKind.IsValid() ? Options.OutputKind : OutputKind.DynamicallyLinkedLibrary);
				pEModuleBuilder = new PEAssemblyBuilder(SourceAssembly, emitOptions, outputKind, serializationProperties, manifestResources, additionalTypes);
			}
			if (debugEntryPoint != null)
			{
				pEModuleBuilder.SetDebugEntryPoint((MethodSymbol)debugEntryPoint, diagnostics);
			}
			pEModuleBuilder.SourceLinkStreamOpt = sourceLinkStream;
			if (embeddedTexts != null)
			{
				pEModuleBuilder.EmbeddedTexts = embeddedTexts;
			}
			if (testData != null)
			{
				pEModuleBuilder.SetMethodTestData(testData.Methods);
				testData.Module = pEModuleBuilder;
			}
			return pEModuleBuilder;
		}

		internal override bool CompileMethods(CommonPEModuleBuilder moduleBuilder, bool emittingPdb, bool emitMetadataOnly, bool emitTestCoverageData, DiagnosticBag diagnostics, Predicate<ISymbolInternal> filterOpt, CancellationToken cancellationToken)
		{
			bool flag = !FilterAndAppendDiagnostics(diagnostics, GetDiagnostics(CompilationStage.Declare, includeEarlierStages: true, cancellationToken), null, cancellationToken);
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)moduleBuilder;
			EmbeddedSymbolManager.MarkAllDeferredSymbolsAsReferenced(this);
			if (!flag)
			{
				pEModuleBuilder.TranslateImports(diagnostics);
			}
			if (emitMetadataOnly)
			{
				if (flag)
				{
					return false;
				}
				if (pEModuleBuilder.SourceModule.HasBadAttributes)
				{
					DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_ModuleEmitFailure, NoLocation.Singleton, pEModuleBuilder.SourceModule.Name, new LocalizableResourceString("ModuleHasInvalidAttributes", CodeAnalysisResources.ResourceManager, typeof(CodeAnalysisResources)));
					return false;
				}
				SynthesizedMetadataCompiler.ProcessSynthesizedMembers(this, pEModuleBuilder, cancellationToken);
			}
			else
			{
				if ((emittingPdb || emitTestCoverageData) && !CreateDebugDocuments(pEModuleBuilder.DebugDocumentsBuilder, pEModuleBuilder.EmbeddedTexts, diagnostics))
				{
					return false;
				}
				DiagnosticBag incoming = DiagnosticBag.GetInstance();
				MethodCompiler.CompileMethodBodies(this, pEModuleBuilder, emittingPdb, emitTestCoverageData, flag, filterOpt, new BindingDiagnosticBag(incoming), cancellationToken);
				bool flag2 = !FilterAndAppendAndFreeDiagnostics(diagnostics, ref incoming, cancellationToken);
				if (flag || flag2)
				{
					return false;
				}
			}
			cancellationToken.ThrowIfCancellationRequested();
			return true;
		}

		internal override bool GenerateResourcesAndDocumentationComments(CommonPEModuleBuilder moduleBuilder, Stream xmlDocStream, Stream win32Resources, bool useRawWin32Resources, string outputNameOverride, DiagnosticBag diagnostics, CancellationToken cancellationToken)
		{
			DiagnosticBag incoming = DiagnosticBag.GetInstance();
			SetupWin32Resources(moduleBuilder, win32Resources, useRawWin32Resources, incoming);
			ReportManifestResourceDuplicates(moduleBuilder.ManifestResources, from x in SourceAssembly.Modules.Skip(1)
				select x.Name, AddedModulesResourceNames(incoming), incoming);
			if (!FilterAndAppendAndFreeDiagnostics(diagnostics, ref incoming, cancellationToken))
			{
				return false;
			}
			cancellationToken.ThrowIfCancellationRequested();
			DiagnosticBag incoming2 = DiagnosticBag.GetInstance();
			string assemblyName = FileNameUtilities.ChangeExtension(outputNameOverride, null);
			DocumentationCommentCompiler.WriteDocumentationCommentXml(this, assemblyName, xmlDocStream, new BindingDiagnosticBag(incoming2), cancellationToken);
			return FilterAndAppendAndFreeDiagnostics(diagnostics, ref incoming2, cancellationToken);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_201_AddedModulesResourceNames))]
		private IEnumerable<string> AddedModulesResourceNames(DiagnosticBag diagnostics)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_201_AddedModulesResourceNames(-2)
			{
				_0024VB_0024Me = this,
				_0024P_diagnostics = diagnostics
			};
		}

		internal override EmitDifferenceResult EmitDifference(EmitBaseline baseline, IEnumerable<SemanticEdit> edits, Func<ISymbol, bool> isAddedSymbol, Stream metadataStream, Stream ilStream, Stream pdbStream, ICollection<MethodDefinitionHandle> updatedMethods, CompilationTestData testData, CancellationToken cancellationToken)
		{
			return EmitHelpers.EmitDifference(this, baseline, edits, isAddedSymbol, metadataStream, ilStream, pdbStream, updatedMethods, testData, cancellationToken);
		}

		internal string GetRuntimeMetadataVersion()
		{
			if (Assembly.CorLibrary is PEAssemblySymbol pEAssemblySymbol)
			{
				return pEAssemblySymbol.Assembly.ManifestModule.MetadataVersion;
			}
			return string.Empty;
		}

		internal override void AddDebugSourceDocumentsForChecksumDirectives(DebugDocumentsBuilder documentsBuilder, SyntaxTree tree, DiagnosticBag diagnosticBag)
		{
			IList<DirectiveTriviaSyntax> directives = VisualBasicExtensions.GetDirectives(tree.GetRoot(), (DirectiveTriviaSyntax d) => d.Kind() == SyntaxKind.ExternalChecksumDirectiveTrivia && !d.ContainsDiagnostics);
			foreach (ExternalChecksumDirectiveTriviaSyntax item in directives)
			{
				string valueText = item.ExternalSource.ValueText;
				string valueText2 = item.Checksum.ValueText;
				string text = documentsBuilder.NormalizeDebugDocumentPath(valueText, tree.FilePath);
				DebugSourceDocument debugSourceDocument = documentsBuilder.TryGetDebugDocumentForNormalizedPath(text);
				if (debugSourceDocument != null)
				{
					if (!debugSourceDocument.IsComputedChecksum)
					{
						DebugSourceInfo sourceInfo = debugSourceDocument.GetSourceInfo();
						if (!CheckSumMatches(valueText2, sourceInfo.Checksum) || !(Guid.Parse(item.Guid.ValueText) == sourceInfo.ChecksumAlgorithmId))
						{
							DiagnosticBagExtensions.Add(diagnosticBag, ERRID.WRN_MultipleDeclFileExtChecksum, new SourceLocation(item), valueText);
						}
					}
				}
				else
				{
					DebugSourceDocument document = new DebugSourceDocument(text, DebugSourceDocument.CorSymLanguageTypeBasic, MakeCheckSumBytes(item.Checksum.ValueText), Guid.Parse(item.Guid.ValueText));
					documentsBuilder.AddDebugDocument(document);
				}
			}
		}

		private static bool CheckSumMatches(string bytesText, ImmutableArray<byte> bytes)
		{
			if (bytesText.Length != bytes.Length * 2)
			{
				return false;
			}
			int num = bytesText.Length / 2 - 1;
			for (int i = 0; i <= num; i++)
			{
				if (SyntaxFacts.IntegralLiteralCharacterValue(bytesText[i * 2]) * 16 + SyntaxFacts.IntegralLiteralCharacterValue(bytesText[i * 2 + 1]) != bytes[i])
				{
					return false;
				}
			}
			return true;
		}

		private static ImmutableArray<byte> MakeCheckSumBytes(string bytesText)
		{
			ArrayBuilder<byte> instance = ArrayBuilder<byte>.GetInstance();
			int num = bytesText.Length / 2 - 1;
			for (int i = 0; i <= num; i++)
			{
				byte item = (byte)(SyntaxFacts.IntegralLiteralCharacterValue(bytesText[i * 2]) * 16 + SyntaxFacts.IntegralLiteralCharacterValue(bytesText[i * 2 + 1]));
				instance.Add(item);
			}
			return instance.ToImmutableAndFree();
		}

		internal override bool HasCodeToEmit()
		{
			ImmutableArray<SyntaxTree>.Enumerator enumerator = SyntaxTrees.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (VisualBasicExtensions.GetCompilationUnitRoot(enumerator.Current).Members.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		protected override Compilation CommonWithReferences(IEnumerable<MetadataReference> newReferences)
		{
			return WithReferences(newReferences);
		}

		protected override Compilation CommonWithAssemblyName(string assemblyName)
		{
			return WithAssemblyName(assemblyName);
		}

		protected override Compilation CommonWithScriptCompilationInfo(ScriptCompilationInfo info)
		{
			return WithScriptCompilationInfo((VisualBasicScriptCompilationInfo)info);
		}

		protected override SemanticModel CommonGetSemanticModel(SyntaxTree syntaxTree, bool ignoreAccessibility)
		{
			return GetSemanticModel(syntaxTree, ignoreAccessibility);
		}

		protected override Compilation CommonAddSyntaxTrees(IEnumerable<SyntaxTree> trees)
		{
			if (trees is SyntaxTree[] trees2)
			{
				return AddSyntaxTrees(trees2);
			}
			if (trees == null)
			{
				throw new ArgumentNullException("trees");
			}
			return AddSyntaxTrees(trees.Cast<SyntaxTree>());
		}

		protected override Compilation CommonRemoveSyntaxTrees(IEnumerable<SyntaxTree> trees)
		{
			if (trees is SyntaxTree[] trees2)
			{
				return RemoveSyntaxTrees(trees2);
			}
			if (trees == null)
			{
				throw new ArgumentNullException("trees");
			}
			return RemoveSyntaxTrees(trees.Cast<SyntaxTree>());
		}

		protected override Compilation CommonRemoveAllSyntaxTrees()
		{
			return RemoveAllSyntaxTrees();
		}

		protected override Compilation CommonReplaceSyntaxTree(SyntaxTree oldTree, SyntaxTree newTree)
		{
			return ReplaceSyntaxTree(oldTree, newTree);
		}

		protected override Compilation CommonWithOptions(CompilationOptions options)
		{
			return WithOptions((VisualBasicCompilationOptions)options);
		}

		protected override bool CommonContainsSyntaxTree(SyntaxTree syntaxTree)
		{
			return ContainsSyntaxTree(syntaxTree);
		}

		protected override ISymbol CommonGetAssemblyOrModuleSymbol(MetadataReference reference)
		{
			return GetAssemblyOrModuleSymbol(reference);
		}

		protected override Compilation CommonClone()
		{
			return Clone();
		}

		private protected override INamedTypeSymbolInternal CommonGetSpecialType(SpecialType specialType)
		{
			return GetSpecialType(specialType);
		}

		protected override INamespaceSymbol CommonGetCompilationNamespace(INamespaceSymbol namespaceSymbol)
		{
			return GetCompilationNamespace(namespaceSymbol);
		}

		protected override INamedTypeSymbol CommonGetTypeByMetadataName(string metadataName)
		{
			return GetTypeByMetadataName(metadataName);
		}

		protected override INamedTypeSymbol CommonCreateErrorTypeSymbol(INamespaceOrTypeSymbol container, string name, int arity)
		{
			return new ExtendedErrorTypeSymbol(SymbolExtensions.EnsureVbSymbolOrNothing<INamespaceOrTypeSymbol, NamespaceOrTypeSymbol>(container, "container"), name, arity);
		}

		protected override INamespaceSymbol CommonCreateErrorNamespaceSymbol(INamespaceSymbol container, string name)
		{
			return new MissingNamespaceSymbol(SymbolExtensions.EnsureVbSymbolOrNothing<INamespaceSymbol, NamespaceSymbol>(container, "container"), name);
		}

		protected override IArrayTypeSymbol CommonCreateArrayTypeSymbol(ITypeSymbol elementType, int rank, NullableAnnotation elementNullableAnnotation)
		{
			return CreateArrayTypeSymbol(SymbolExtensions.EnsureVbSymbolOrNothing<ITypeSymbol, TypeSymbol>(elementType, "elementType"), rank);
		}

		protected override INamedTypeSymbol CommonCreateTupleTypeSymbol(ImmutableArray<ITypeSymbol> elementTypes, ImmutableArray<string> elementNames, ImmutableArray<Location> elementLocations, ImmutableArray<NullableAnnotation> elementNullableAnnotations)
		{
			ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance(elementTypes.Length);
			int num = elementTypes.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				instance.Add(SymbolExtensions.EnsureVbSymbolOrNothing<ITypeSymbol, TypeSymbol>(elementTypes[i], string.Format("{0}[{1}]", "elementTypes", i)));
			}
			return TupleTypeSymbol.Create(null, instance.ToImmutableAndFree(), elementLocations, elementNames, this, shouldCheckConstraints: false, default(ImmutableArray<bool>));
		}

		protected override INamedTypeSymbol CommonCreateTupleTypeSymbol(INamedTypeSymbol underlyingType, ImmutableArray<string> elementNames, ImmutableArray<Location> elementLocations, ImmutableArray<NullableAnnotation> elementNullableAnnotations)
		{
			if (!SymbolExtensions.EnsureVbSymbolOrNothing<INamedTypeSymbol, NamedTypeSymbol>(underlyingType, "underlyingType").IsTupleCompatible(out var tupleCardinality))
			{
				throw new ArgumentException(CodeAnalysisResources.TupleUnderlyingTypeMustBeTupleCompatible, "underlyingType");
			}
			elementNames = Compilation.CheckTupleElementNames(tupleCardinality, elementNames);
			Compilation.CheckTupleElementLocations(tupleCardinality, elementLocations);
			Compilation.CheckTupleElementNullableAnnotations(tupleCardinality, elementNullableAnnotations);
			return TupleTypeSymbol.Create(null, SymbolExtensions.EnsureVbSymbolOrNothing<INamedTypeSymbol, NamedTypeSymbol>(underlyingType, "underlyingType"), elementLocations, elementNames, default(ImmutableArray<bool>));
		}

		protected override IPointerTypeSymbol CommonCreatePointerTypeSymbol(ITypeSymbol elementType)
		{
			throw new NotSupportedException(VBResources.ThereAreNoPointerTypesInVB);
		}

		protected override IFunctionPointerTypeSymbol CommonCreateFunctionPointerTypeSymbol(ITypeSymbol returnType, RefKind refKind, ImmutableArray<ITypeSymbol> parameterTypes, ImmutableArray<RefKind> parameterRefKinds, SignatureCallingConvention callingConvention, ImmutableArray<INamedTypeSymbol> callingConventionTypes)
		{
			throw new NotSupportedException(VBResources.ThereAreNoFunctionPointerTypesInVB);
		}

		protected override INamedTypeSymbol CommonCreateNativeIntegerTypeSymbol(bool signed)
		{
			throw new NotSupportedException(VBResources.ThereAreNoNativeIntegerTypesInVB);
		}

		protected override INamedTypeSymbol CommonCreateAnonymousTypeSymbol(ImmutableArray<ITypeSymbol> memberTypes, ImmutableArray<string> memberNames, ImmutableArray<Location> memberLocations, ImmutableArray<bool> memberIsReadOnly, ImmutableArray<NullableAnnotation> memberNullableAnnotations)
		{
			int num = 0;
			ImmutableArray<ITypeSymbol>.Enumerator enumerator = memberTypes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SymbolExtensions.EnsureVbSymbolOrNothing<ITypeSymbol, TypeSymbol>(enumerator.Current, string.Format("{0}({1})", "memberTypes", num));
				num++;
			}
			ArrayBuilder<AnonymousTypeField> instance = ArrayBuilder<AnonymousTypeField>.GetInstance();
			int num2 = memberTypes.Length - 1;
			for (num = 0; num <= num2; num++)
			{
				ITypeSymbol typeSymbol = memberTypes[num];
				string name = memberNames[num];
				Location location = (memberLocations.IsDefault ? Location.None : memberLocations[num]);
				bool isKeyOrByRef = memberIsReadOnly.IsDefault || memberIsReadOnly[num];
				instance.Add(new AnonymousTypeField(name, (TypeSymbol)typeSymbol, location, isKeyOrByRef));
			}
			AnonymousTypeDescriptor typeDescr = new AnonymousTypeDescriptor(instance.ToImmutableAndFree(), Location.None, isImplicitlyDeclared: false);
			return AnonymousTypeManager.ConstructAnonymousTypeSymbol(typeDescr);
		}

		protected override IMethodSymbol CommonGetEntryPoint(CancellationToken cancellationToken)
		{
			return GetEntryPoint(cancellationToken);
		}

		public override bool ContainsSymbolsWithName(Func<string, bool> predicate, SymbolFilter filter = SymbolFilter.TypeAndMember, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			if (filter == SymbolFilter.None)
			{
				throw new ArgumentException(VBResources.NoNoneSearchCriteria, "filter");
			}
			return DeclarationTable.ContainsName(MergedRootDeclaration, predicate, filter, cancellationToken);
		}

		public override IEnumerable<ISymbol> GetSymbolsWithName(Func<string, bool> predicate, SymbolFilter filter = SymbolFilter.TypeAndMember, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			if (filter == SymbolFilter.None)
			{
				throw new ArgumentException(VBResources.NoNoneSearchCriteria, "filter");
			}
			return new PredicateSymbolSearcher(this, filter, predicate, cancellationToken).GetSymbolsWithName();
		}

		public override bool ContainsSymbolsWithName(string name, SymbolFilter filter = SymbolFilter.TypeAndMember, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (filter == SymbolFilter.None)
			{
				throw new ArgumentException(VBResources.NoNoneSearchCriteria, "filter");
			}
			return DeclarationTable.ContainsName(MergedRootDeclaration, name, filter, cancellationToken);
		}

		public override IEnumerable<ISymbol> GetSymbolsWithName(string name, SymbolFilter filter = SymbolFilter.TypeAndMember, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (filter == SymbolFilter.None)
			{
				throw new ArgumentException(VBResources.NoNoneSearchCriteria, "filter");
			}
			return new NameSymbolSearcher(this, filter, name, cancellationToken).GetSymbolsWithName();
		}

		internal override bool IsUnreferencedAssemblyIdentityDiagnosticCode(int code)
		{
			if (code == 30005 || code == 30652)
			{
				return true;
			}
			return false;
		}

		public override ImmutableArray<MetadataReference> GetUsedAssemblyReferences(CancellationToken cancellationToken = default(CancellationToken))
		{
			ConcurrentSet<AssemblySymbol> completeSetOfUsedAssemblies = GetCompleteSetOfUsedAssemblies(cancellationToken);
			if (completeSetOfUsedAssemblies == null)
			{
				return ImmutableArray<MetadataReference>.Empty;
			}
			ArrayBuilder<MetadataReference> instance = ArrayBuilder<MetadataReference>.GetInstance(completeSetOfUsedAssemblies.Count);
			foreach (MetadataReference reference in base.References)
			{
				if (reference.Properties.Kind == MetadataImageKind.Assembly)
				{
					Symbol referencedAssemblySymbol = GetBoundReferenceManager().GetReferencedAssemblySymbol(reference);
					if ((object)referencedAssemblySymbol != null && completeSetOfUsedAssemblies.Contains((AssemblySymbol)referencedAssemblySymbol))
					{
						instance.Add(reference);
					}
				}
			}
			return instance.ToImmutableAndFree();
		}

		private ConcurrentSet<AssemblySymbol> GetCompleteSetOfUsedAssemblies(CancellationToken cancellationToken)
		{
			if (!_usedAssemblyReferencesFrozen && !Volatile.Read(ref _usedAssemblyReferencesFrozen))
			{
				BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag(DiagnosticBag.GetInstance(), new ConcurrentSet<AssemblySymbol>());
				GetDiagnosticsWithoutFiltering(CompilationStage.Declare, includeEarlierStages: true, bindingDiagnosticBag, cancellationToken);
				bool flag = bindingDiagnosticBag.HasAnyErrors();
				if (!flag)
				{
					bindingDiagnosticBag.DiagnosticBag!.Clear();
					GetDiagnosticsForAllMethodBodies(hasDeclarationErrors: false, bindingDiagnosticBag, doLowering: true, cancellationToken);
					flag = bindingDiagnosticBag.HasAnyErrors();
					if (!flag)
					{
						AddUsedAssemblies(bindingDiagnosticBag.DependenciesBag);
					}
				}
				CompleteTheSetOfUsedAssemblies(flag, cancellationToken);
				bindingDiagnosticBag.DiagnosticBag!.Free();
			}
			return _lazyUsedAssemblyReferences;
		}

		private void AddUsedAssembly(AssemblySymbol dependency, ArrayBuilder<AssemblySymbol> stack)
		{
			if (AddUsedAssembly(dependency))
			{
				stack.Push(dependency);
			}
		}

		private void AddReferencedAssemblies(AssemblySymbol assembly, bool includeMainModule, ArrayBuilder<AssemblySymbol> stack)
		{
			int num = ((!includeMainModule) ? 1 : 0);
			int num2 = assembly.Modules.Length - 1;
			for (int i = num; i <= num2; i++)
			{
				ImmutableArray<AssemblySymbol>.Enumerator enumerator = assembly.Modules[i].ReferencedAssemblySymbols.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AssemblySymbol current = enumerator.Current;
					AddUsedAssembly(current, stack);
				}
			}
		}

		private void CompleteTheSetOfUsedAssemblies(bool seenErrors, CancellationToken cancellationToken)
		{
			if (_usedAssemblyReferencesFrozen || Volatile.Read(ref _usedAssemblyReferencesFrozen))
			{
				return;
			}
			if (seenErrors)
			{
				ImmutableArray<AssemblySymbol>.Enumerator enumerator = SourceModule.ReferencedAssemblySymbols.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AssemblySymbol current = enumerator.Current;
					AddUsedAssembly(current);
				}
			}
			else
			{
				int num = SourceAssembly.Modules.Length - 1;
				for (int i = 1; i <= num; i++)
				{
					ImmutableArray<AssemblySymbol>.Enumerator enumerator2 = SourceAssembly.Modules[i].ReferencedAssemblySymbols.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						AssemblySymbol current2 = enumerator2.Current;
						AddUsedAssembly(current2);
					}
				}
				if (_usedAssemblyReferencesFrozen || Volatile.Read(ref _usedAssemblyReferencesFrozen))
				{
					return;
				}
				if (_lazyUsedAssemblyReferences != null)
				{
					lock (_lazyUsedAssemblyReferences)
					{
						if (_usedAssemblyReferencesFrozen || Volatile.Read(ref _usedAssemblyReferencesFrozen))
						{
							return;
						}
						ArrayBuilder<AssemblySymbol> instance = ArrayBuilder<AssemblySymbol>.GetInstance(_lazyUsedAssemblyReferences.Count);
						instance.AddRange(_lazyUsedAssemblyReferences);
						while (instance.Count != 0)
						{
							AssemblySymbol assemblySymbol = instance.Pop();
							if (assemblySymbol is SourceAssemblySymbol sourceAssemblySymbol)
							{
								ConcurrentSet<AssemblySymbol> completeSetOfUsedAssemblies = sourceAssemblySymbol.DeclaringCompilation.GetCompleteSetOfUsedAssemblies(cancellationToken);
								if (completeSetOfUsedAssemblies != null)
								{
									ConcurrentSet<AssemblySymbol>.KeyEnumerator enumerator3 = completeSetOfUsedAssemblies.GetEnumerator();
									while (enumerator3.MoveNext())
									{
										AssemblySymbol current3 = enumerator3.Current;
										AddUsedAssembly(current3, instance);
									}
								}
							}
							else if (assemblySymbol is RetargetingAssemblySymbol retargetingAssemblySymbol)
							{
								ConcurrentSet<AssemblySymbol> completeSetOfUsedAssemblies = retargetingAssemblySymbol.UnderlyingAssembly.DeclaringCompilation.GetCompleteSetOfUsedAssemblies(cancellationToken);
								if (completeSetOfUsedAssemblies != null)
								{
									ImmutableArray<AssemblySymbol>.Enumerator enumerator4 = retargetingAssemblySymbol.UnderlyingAssembly.SourceModule.ReferencedAssemblySymbols.GetEnumerator();
									while (enumerator4.MoveNext())
									{
										AssemblySymbol current4 = enumerator4.Current;
										if (!current4.IsLinked && completeSetOfUsedAssemblies.Contains(current4))
										{
											AssemblySymbol to = null;
											if (!((RetargetingModuleSymbol)retargetingAssemblySymbol.Modules[0]).RetargetingDefinitions(current4, out to))
											{
												to = current4;
											}
											AddUsedAssembly(to, instance);
										}
									}
								}
								AddReferencedAssemblies(retargetingAssemblySymbol, includeMainModule: false, instance);
							}
							else
							{
								AddReferencedAssemblies(assemblySymbol, includeMainModule: true, instance);
							}
						}
						instance.Free();
					}
				}
				if ((object)SourceAssembly.CorLibrary != null)
				{
					AddUsedAssembly(SourceAssembly.CorLibrary);
				}
			}
			_usedAssemblyReferencesFrozen = true;
		}

		internal void AddUsedAssemblies(ICollection<AssemblySymbol> assemblies)
		{
			if (assemblies.IsNullOrEmpty())
			{
				return;
			}
			foreach (AssemblySymbol assembly in assemblies)
			{
				AddUsedAssembly(assembly);
			}
		}

		internal bool AddUsedAssembly(AssemblySymbol assembly)
		{
			if ((object)assembly == null || (object)assembly == SourceAssembly || assembly.IsMissing)
			{
				return false;
			}
			if (_lazyUsedAssemblyReferences == null)
			{
				Interlocked.CompareExchange(ref _lazyUsedAssemblyReferences, new ConcurrentSet<AssemblySymbol>(), null);
			}
			return _lazyUsedAssemblyReferences.Add(assembly);
		}

		internal MethodSymbol GetExtensionAttributeConstructor(out UseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			MethodSymbol methodSymbol = null;
			DiagnosticInfo lazyExtensionAttributeConstructorErrorInfo = null;
			if ((object)_lazyExtensionAttributeConstructor == ErrorTypeSymbol.UnknownResultType)
			{
				NamespaceSymbol namespaceSymbol = GlobalNamespace.LookupNestedNamespace(ImmutableArray.Create("System", "Runtime", "CompilerServices"));
				NamedTypeSymbol namedTypeSymbol = null;
				SourceModuleSymbol sourceModuleSymbol = (SourceModuleSymbol)SourceModule;
				Binder binder = BinderBuilder.CreateSourceModuleBinder(sourceModuleSymbol);
				if ((object)namespaceSymbol != null)
				{
					ImmutableArray<NamedTypeSymbol> typeMembers = namespaceSymbol.GetTypeMembers(AttributeDescription.CaseInsensitiveExtensionAttribute.Name, 0);
					bool flag = false;
					ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = typeMembers.GetEnumerator();
					while (enumerator.MoveNext())
					{
						NamedTypeSymbol current = enumerator.Current;
						CompoundUseSiteInfo<AssemblySymbol> useSiteInfo2 = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
						if (!binder.IsAccessible(current, ref useSiteInfo2))
						{
							continue;
						}
						if ((object)current.ContainingModule == sourceModuleSymbol)
						{
							namedTypeSymbol = current;
							flag = false;
							break;
						}
						if ((object)namedTypeSymbol == null)
						{
							namedTypeSymbol = current;
						}
						else if ((object)current.ContainingAssembly == Assembly)
						{
							if ((object)namedTypeSymbol.ContainingAssembly == Assembly)
							{
								flag = true;
								continue;
							}
							namedTypeSymbol = current;
							flag = false;
						}
						else if ((object)namedTypeSymbol.ContainingAssembly != Assembly)
						{
							flag = true;
						}
					}
					if (flag)
					{
						namedTypeSymbol = null;
					}
				}
				if ((object)namedTypeSymbol != null && !TypeSymbolExtensions.IsStructureType(namedTypeSymbol) && !namedTypeSymbol.IsMustInherit)
				{
					NamedTypeSymbol wellKnownType = GetWellKnownType(WellKnownType.System_Attribute);
					NamedTypeSymbol subType = namedTypeSymbol;
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo2 = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
					if (TypeSymbolExtensions.IsBaseTypeOf(wellKnownType, subType, ref useSiteInfo2))
					{
						NamedTypeSymbol sym = namedTypeSymbol;
						useSiteInfo2 = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
						if (binder.IsAccessible(sym, ref useSiteInfo2))
						{
							ImmutableArray<MethodSymbol>.Enumerator enumerator2 = namedTypeSymbol.InstanceConstructors.GetEnumerator();
							while (enumerator2.MoveNext())
							{
								MethodSymbol current2 = enumerator2.Current;
								if (current2.ParameterCount == 0)
								{
									useSiteInfo2 = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
									if (binder.IsAccessible(current2, ref useSiteInfo2))
									{
										methodSymbol = current2;
									}
									break;
								}
							}
						}
					}
				}
				if ((object)methodSymbol == null)
				{
					lazyExtensionAttributeConstructorErrorInfo = ErrorFactory.ErrorInfo(ERRID.ERR_MissingRuntimeHelper, AttributeDescription.CaseInsensitiveExtensionAttribute.FullName + "..ctor");
				}
				else if ((methodSymbol.ContainingType.GetAttributeUsageInfo().ValidTargets & (AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)) != (AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method))
				{
					lazyExtensionAttributeConstructorErrorInfo = ErrorFactory.ErrorInfo(ERRID.ERR_ExtensionAttributeInvalid);
				}
				_lazyExtensionAttributeConstructorErrorInfo = lazyExtensionAttributeConstructorErrorInfo;
				Interlocked.CompareExchange(ref _lazyExtensionAttributeConstructor, methodSymbol, ErrorTypeSymbol.UnknownResultType);
			}
			methodSymbol = (MethodSymbol)_lazyExtensionAttributeConstructor;
			lazyExtensionAttributeConstructorErrorInfo = (DiagnosticInfo)Volatile.Read(ref _lazyExtensionAttributeConstructorErrorInfo);
			if (lazyExtensionAttributeConstructorErrorInfo != null)
			{
				useSiteInfo = new UseSiteInfo<AssemblySymbol>(lazyExtensionAttributeConstructorErrorInfo);
			}
			else
			{
				useSiteInfo = Binder.GetUseSiteInfoForMemberAndContainingType(methodSymbol);
			}
			return methodSymbol;
		}

		internal SynthesizedAttributeData TrySynthesizeAttribute(WellKnownMember constructor, ImmutableArray<TypedConstant> arguments = default(ImmutableArray<TypedConstant>), ImmutableArray<KeyValuePair<WellKnownMember, TypedConstant>> namedArguments = default(ImmutableArray<KeyValuePair<WellKnownMember, TypedConstant>>), bool isOptionalUse = false)
		{
			if (!(GetWellKnownTypeMember(constructor) is MethodSymbol methodSymbol) || Binder.GetUseSiteInfoForWellKnownTypeMember(methodSymbol, constructor, embedVBRuntimeUsed: false).DiagnosticInfo != null)
			{
				return ReturnNothingOrThrowIfAttributeNonOptional(constructor, isOptionalUse);
			}
			if (arguments.IsDefault)
			{
				arguments = ImmutableArray<TypedConstant>.Empty;
			}
			ImmutableArray<KeyValuePair<string, TypedConstant>> namedArgs;
			if (namedArguments.IsDefault)
			{
				namedArgs = ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty;
			}
			else
			{
				ArrayBuilder<KeyValuePair<string, TypedConstant>> arrayBuilder = new ArrayBuilder<KeyValuePair<string, TypedConstant>>(namedArguments.Length);
				ImmutableArray<KeyValuePair<WellKnownMember, TypedConstant>>.Enumerator enumerator = namedArguments.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<WellKnownMember, TypedConstant> current = enumerator.Current;
					Symbol wellKnownTypeMember = GetWellKnownTypeMember(current.Key);
					if ((object)wellKnownTypeMember == null || wellKnownTypeMember is ErrorTypeSymbol || Binder.GetUseSiteInfoForWellKnownTypeMember(wellKnownTypeMember, current.Key, embedVBRuntimeUsed: false).DiagnosticInfo != null)
					{
						return ReturnNothingOrThrowIfAttributeNonOptional(constructor);
					}
					arrayBuilder.Add(new KeyValuePair<string, TypedConstant>(wellKnownTypeMember.Name, current.Value));
				}
				namedArgs = arrayBuilder.ToImmutableAndFree();
			}
			return new SynthesizedAttributeData(methodSymbol, arguments, namedArgs);
		}

		private static SynthesizedAttributeData ReturnNothingOrThrowIfAttributeNonOptional(WellKnownMember constructor, bool isOptionalUse = false)
		{
			if (isOptionalUse || WellKnownMembers.IsSynthesizedAttributeOptional(constructor))
			{
				return null;
			}
			throw ExceptionUtilities.Unreachable;
		}

		internal SynthesizedAttributeData SynthesizeExtensionAttribute()
		{
			UseSiteInfo<AssemblySymbol> useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
			return SynthesizedAttributeData.Create(GetExtensionAttributeConstructor(out useSiteInfo), WellKnownMember.System_Runtime_CompilerServices_ExtensionAttribute__ctor);
		}

		internal SynthesizedAttributeData SynthesizeStateMachineAttribute(MethodSymbol method, ModuleCompilationState compilationState)
		{
			NamedTypeSymbol stateMachineType = null;
			if (compilationState.TryGetStateMachineType(method, out stateMachineType))
			{
				WellKnownMember constructor = (method.IsAsync ? WellKnownMember.System_Runtime_CompilerServices_AsyncStateMachineAttribute__ctor : WellKnownMember.System_Runtime_CompilerServices_IteratorStateMachineAttribute__ctor);
				TypedConstant item = new TypedConstant(GetWellKnownType(WellKnownType.System_Type), TypedConstantKind.Type, stateMachineType.IsGenericType ? stateMachineType.ConstructUnboundGenericType() : stateMachineType);
				return TrySynthesizeAttribute(constructor, ImmutableArray.Create(item));
			}
			return null;
		}

		internal SynthesizedAttributeData SynthesizeDecimalConstantAttribute(decimal value)
		{
			value.GetBits(out var isNegative, out var scale, out var low, out var mid, out var high);
			NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Byte);
			NamedTypeSymbol specialType2 = GetSpecialType(SpecialType.System_UInt32);
			return TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_DecimalConstantAttribute__ctor, ImmutableArray.Create<TypedConstant>(new TypedConstant(specialType, TypedConstantKind.Primitive, scale), new TypedConstant(specialType, TypedConstantKind.Primitive, (byte)(isNegative ? 128u : 0u)), new TypedConstant(specialType2, TypedConstantKind.Primitive, high), new TypedConstant(specialType2, TypedConstantKind.Primitive, mid), new TypedConstant(specialType2, TypedConstantKind.Primitive, low)));
		}

		internal SynthesizedAttributeData SynthesizeDebuggerBrowsableNeverAttribute()
		{
			if (Options.OptimizationLevel != 0)
			{
				return null;
			}
			return TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerBrowsableAttribute__ctor, ImmutableArray.Create(new TypedConstant(GetWellKnownType(WellKnownType.System_Diagnostics_DebuggerBrowsableState), TypedConstantKind.Enum, DebuggerBrowsableState.Never)));
		}

		internal SynthesizedAttributeData SynthesizeDebuggerHiddenAttribute()
		{
			if (Options.OptimizationLevel != 0)
			{
				return null;
			}
			return TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerHiddenAttribute__ctor);
		}

		internal SynthesizedAttributeData SynthesizeEditorBrowsableNeverAttribute()
		{
			return TrySynthesizeAttribute(WellKnownMember.System_ComponentModel_EditorBrowsableAttribute__ctor, ImmutableArray.Create(new TypedConstant(GetWellKnownType(WellKnownType.System_ComponentModel_EditorBrowsableState), TypedConstantKind.Enum, EditorBrowsableState.Never)));
		}

		internal SynthesizedAttributeData SynthesizeDebuggerNonUserCodeAttribute()
		{
			if (Options.OptimizationLevel != 0)
			{
				return null;
			}
			return TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerNonUserCodeAttribute__ctor);
		}

		internal SynthesizedAttributeData SynthesizeOptionalDebuggerStepThroughAttribute()
		{
			if (Options.OptimizationLevel != 0)
			{
				return null;
			}
			return TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerStepThroughAttribute__ctor);
		}

		internal Symbol GetWellKnownTypeMember(WellKnownMember member)
		{
			if (IsMemberMissing(member))
			{
				return null;
			}
			if (_lazyWellKnownTypeMembers == null || (object)_lazyWellKnownTypeMembers[(int)member] == ErrorTypeSymbol.UnknownResultType)
			{
				if (_lazyWellKnownTypeMembers == null)
				{
					Symbol[] array = new Symbol[418];
					int num = array.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						array[i] = ErrorTypeSymbol.UnknownResultType;
					}
					Interlocked.CompareExchange(ref _lazyWellKnownTypeMembers, array, null);
				}
				Microsoft.CodeAnalysis.RuntimeMembers.MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(member);
				NamedTypeSymbol namedTypeSymbol = ((descriptor.DeclaringTypeId <= 45) ? GetSpecialType((SpecialType)descriptor.DeclaringTypeId) : GetWellKnownType((WellKnownType)descriptor.DeclaringTypeId));
				Symbol value = null;
				if (!TypeSymbolExtensions.IsErrorType(namedTypeSymbol))
				{
					value = GetRuntimeMember(namedTypeSymbol, ref descriptor, _wellKnownMemberSignatureComparer, Assembly);
				}
				Interlocked.CompareExchange(ref _lazyWellKnownTypeMembers[(int)member], value, ErrorTypeSymbol.UnknownResultType);
			}
			return _lazyWellKnownTypeMembers[(int)member];
		}

		internal override bool IsSystemTypeReference(ITypeSymbolInternal type)
		{
			return TypeSymbol.Equals((TypeSymbol)type, GetWellKnownType(WellKnownType.System_Type), TypeCompareKind.ConsiderEverything);
		}

		internal override ISymbolInternal CommonGetWellKnownTypeMember(WellKnownMember member)
		{
			return GetWellKnownTypeMember(member);
		}

		internal override ITypeSymbolInternal CommonGetWellKnownType(WellKnownType wellknownType)
		{
			return GetWellKnownType(wellknownType);
		}

		internal override bool IsAttributeType(ITypeSymbol type)
		{
			if (type.Kind != SymbolKind.NamedType)
			{
				return false;
			}
			NamedTypeSymbol derivedType = (NamedTypeSymbol)type;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
			return TypeSymbolExtensions.IsOrDerivedFromWellKnownClass(derivedType, WellKnownType.System_Attribute, this, ref useSiteInfo);
		}

		internal NamedTypeSymbol GetWellKnownType(WellKnownType type)
		{
			int num = (int)(type - 46);
			if (_lazyWellKnownTypes == null || (object)_lazyWellKnownTypes[num] == null)
			{
				if (_lazyWellKnownTypes == null)
				{
					Interlocked.CompareExchange(ref _lazyWellKnownTypes, new NamedTypeSymbol[257], null);
				}
				string metadataName = type.GetMetadataName();
				(AssemblySymbol, AssemblySymbol) conflicts = default((AssemblySymbol, AssemblySymbol));
				NamedTypeSymbol namedTypeSymbol = ((!IsTypeMissing(type)) ? Assembly.GetTypeByMetadataName(metadataName, includeReferences: true, isWellKnownType: true, out conflicts, useCLSCompliantNameArityEncoding: true, Options.IgnoreCorLibraryDuplicatedTypes) : null);
				if ((object)namedTypeSymbol == null)
				{
					MetadataTypeName emittedName = MetadataTypeName.FromFullName(metadataName, useCLSCompliantNameArityEncoding: true);
					if (type.IsValueTupleType())
					{
						Func<MissingMetadataTypeSymbol.TopLevelWithCustomErrorInfo, DiagnosticInfo> delayedErrorInfo;
						if ((object)conflicts.Item1 == null)
						{
							delayedErrorInfo = (MissingMetadataTypeSymbol.TopLevelWithCustomErrorInfo t) => ErrorFactory.ErrorInfo(ERRID.ERR_ValueTupleTypeRefResolutionError1, t);
						}
						else
						{
							(AssemblySymbol, AssemblySymbol) tuple = conflicts;
							delayedErrorInfo = (MissingMetadataTypeSymbol.TopLevelWithCustomErrorInfo t) => ErrorFactory.ErrorInfo(ERRID.ERR_ValueTupleResolutionAmbiguous3, t, tuple.Item1, tuple.Item2);
						}
						namedTypeSymbol = new MissingMetadataTypeSymbol.TopLevelWithCustomErrorInfo(Assembly.Modules[0], ref emittedName, delayedErrorInfo);
					}
					else
					{
						namedTypeSymbol = new MissingMetadataTypeSymbol.TopLevel(Assembly.Modules[0], ref emittedName);
					}
				}
				Interlocked.CompareExchange(ref _lazyWellKnownTypes[num], namedTypeSymbol, null);
			}
			return _lazyWellKnownTypes[num];
		}

		internal static Symbol GetRuntimeMember(NamedTypeSymbol declaringType, ref Microsoft.CodeAnalysis.RuntimeMembers.MemberDescriptor descriptor, SignatureComparer<MethodSymbol, FieldSymbol, PropertySymbol, TypeSymbol, ParameterSymbol> comparer, AssemblySymbol accessWithinOpt)
		{
			Symbol symbol = null;
			MethodKind methodKind = MethodKind.Ordinary;
			bool flag = (descriptor.Flags & MemberFlags.Static) != 0;
			SymbolKind symbolKind;
			switch (descriptor.Flags & MemberFlags.KindMask)
			{
			case MemberFlags.Constructor:
				symbolKind = SymbolKind.Method;
				methodKind = MethodKind.Constructor;
				break;
			case MemberFlags.Method:
				symbolKind = SymbolKind.Method;
				break;
			case MemberFlags.PropertyGet:
				symbolKind = SymbolKind.Method;
				methodKind = MethodKind.PropertyGet;
				break;
			case MemberFlags.Field:
				symbolKind = SymbolKind.Field;
				break;
			case MemberFlags.Property:
				symbolKind = SymbolKind.Property;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(descriptor.Flags);
			}
			ImmutableArray<Symbol>.Enumerator enumerator = declaringType.GetMembers(descriptor.Name).GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind != symbolKind || current.IsShared != flag || (current.DeclaredAccessibility != Accessibility.Public && ((object)accessWithinOpt == null || !Symbol.IsSymbolAccessible(current, accessWithinOpt))) || !string.Equals(current.Name, descriptor.Name, StringComparison.Ordinal))
				{
					continue;
				}
				switch (symbolKind)
				{
				case SymbolKind.Method:
				{
					MethodSymbol methodSymbol = (MethodSymbol)current;
					MethodKind methodKind2 = methodSymbol.MethodKind;
					if (methodKind2 == MethodKind.Conversion || methodKind2 == MethodKind.UserDefinedOperator)
					{
						methodKind2 = MethodKind.Ordinary;
					}
					if (methodSymbol.Arity != descriptor.Arity || methodKind2 != methodKind || (descriptor.Flags & MemberFlags.Virtual) != 0 != (methodSymbol.IsOverridable || methodSymbol.IsOverrides || methodSymbol.IsMustOverride) || !comparer.MatchMethodSignature(methodSymbol, descriptor.Signature))
					{
						continue;
					}
					break;
				}
				case SymbolKind.Property:
				{
					PropertySymbol propertySymbol = (PropertySymbol)current;
					if ((descriptor.Flags & MemberFlags.Virtual) != 0 != (propertySymbol.IsOverridable || propertySymbol.IsOverrides || propertySymbol.IsMustOverride) || !comparer.MatchPropertySignature(propertySymbol, descriptor.Signature))
					{
						continue;
					}
					break;
				}
				case SymbolKind.Field:
					if (!comparer.MatchFieldSignature((FieldSymbol)current, descriptor.Signature))
					{
						continue;
					}
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(symbolKind);
				}
				if ((object)symbol != null)
				{
					symbol = null;
					break;
				}
				symbol = current;
			}
			return symbol;
		}

		internal SynthesizedAttributeData SynthesizeTupleNamesAttribute(TypeSymbol type)
		{
			NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_String);
			ImmutableArray<TypedConstant> array = TupleNamesEncoder.Encode(type, specialType);
			ImmutableArray<TypedConstant> arguments = ImmutableArray.Create(new TypedConstant(ArrayTypeSymbol.CreateSZArray(specialType, ImmutableArray<CustomModifier>.Empty, specialType.ContainingAssembly), array));
			return TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_TupleElementNamesAttribute__ctorTransformNames, arguments);
		}
	}
}
