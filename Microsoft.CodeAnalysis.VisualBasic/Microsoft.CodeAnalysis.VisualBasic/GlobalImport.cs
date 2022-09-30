using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
	public sealed class GlobalImport : IEquatable<GlobalImport>
	{
		private class ImportDiagnosticInfo : DiagnosticInfo
		{
			private readonly string _importText;

			private readonly int _startIndex;

			private readonly int _length;

			private readonly DiagnosticInfo _wrappedDiagnostic;

			static ImportDiagnosticInfo()
			{
				ObjectBinder.RegisterTypeReader(typeof(ImportDiagnosticInfo), (ObjectReader r) => new ImportDiagnosticInfo(r));
			}

			private ImportDiagnosticInfo(ObjectReader reader)
				: base(reader)
			{
				_importText = reader.ReadString();
				_startIndex = reader.ReadInt32();
				_length = reader.ReadInt32();
				_wrappedDiagnostic = (DiagnosticInfo)reader.ReadValue();
			}

			protected override void WriteTo(ObjectWriter writer)
			{
				base.WriteTo(writer);
				writer.WriteString(_importText);
				writer.WriteInt32(_startIndex);
				writer.WriteInt32(_length);
				writer.WriteValue(_wrappedDiagnostic);
			}

			public override string GetMessage(IFormatProvider formatProvider = null)
			{
				string format = ErrorFactory.IdToString(ERRID.ERR_GeneralProjectImportsError3, formatProvider as CultureInfo);
				return string.Format(formatProvider, format, _importText, _importText.Substring(_startIndex, _length), _wrappedDiagnostic.GetMessage(formatProvider));
			}

			public ImportDiagnosticInfo(DiagnosticInfo wrappedDiagnostic, string importText, int startIndex, int length)
				: base(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, wrappedDiagnostic.Code)
			{
				_wrappedDiagnostic = wrappedDiagnostic;
				_importText = importText;
				_startIndex = startIndex;
				_length = length;
			}
		}

		private readonly SyntaxReference _clause;

		private readonly string _importedName;

		public ImportsClauseSyntax Clause => (ImportsClauseSyntax)_clause.GetSyntax();

		internal bool IsXmlClause => Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(Clause, SyntaxKind.XmlNamespaceImportsClause);

		public string Name => _importedName;

		internal GlobalImport(ImportsClauseSyntax clause, string importedName)
		{
			_clause = clause.SyntaxTree.GetReference(clause);
			_importedName = importedName;
		}

		public static GlobalImport Parse(string importedNames)
		{
			return Parse(new string[1] { importedNames }).ElementAtOrDefault(0);
		}

		public static GlobalImport Parse(string importedNames, out ImmutableArray<Diagnostic> diagnostics)
		{
			return Parse(new string[1] { importedNames }, out diagnostics).ElementAtOrDefault(0);
		}

		public static IEnumerable<GlobalImport> Parse(IEnumerable<string> importedNames)
		{
			DiagnosticBag instance = DiagnosticBag.GetInstance();
			GlobalImport[] result = OptionsValidator.ParseImports(importedNames, instance);
			Diagnostic diagnostic = instance.AsEnumerable().FirstOrDefault((Diagnostic diag) => diag.Severity == DiagnosticSeverity.Error);
			instance.Free();
			if (diagnostic != null)
			{
				throw new ArgumentException(diagnostic.GetMessage(CultureInfo.CurrentUICulture));
			}
			return result;
		}

		public static IEnumerable<GlobalImport> Parse(params string[] importedNames)
		{
			return Parse((IEnumerable<string>)importedNames);
		}

		public static IEnumerable<GlobalImport> Parse(IEnumerable<string> importedNames, out ImmutableArray<Diagnostic> diagnostics)
		{
			DiagnosticBag instance = DiagnosticBag.GetInstance();
			GlobalImport[] result = OptionsValidator.ParseImports(importedNames, instance);
			diagnostics = instance.ToReadOnlyAndFree<Diagnostic>();
			return result;
		}

		internal Diagnostic MapDiagnostic(Diagnostic unmappedDiag)
		{
			if (unmappedDiag.Code == 40056)
			{
				return new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.WRN_UndefinedOrEmptyProjectNamespaceOrClass1, _importedName), NoLocation.Singleton);
			}
			TextSpan sourceSpan = unmappedDiag.Location.SourceSpan;
			int num = sourceSpan.Start - _clause.Span.Start;
			int length = sourceSpan.Length;
			if (num < 0 || length <= 0 || num >= _importedName.Length)
			{
				num = 0;
				length = _importedName.Length;
			}
			length = Math.Min(_importedName.Length - num, length);
			return new VBDiagnostic(new ImportDiagnosticInfo(((DiagnosticWithInfo)unmappedDiag).Info, _importedName, num, length), NoLocation.Singleton);
		}

		private string GetDebuggerDisplay()
		{
			return Name;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as GlobalImport);
		}

		public bool Equals(GlobalImport other)
		{
			if ((object)this == other)
			{
				return true;
			}
			if ((object)other == null)
			{
				return false;
			}
			return string.Equals(Name, other.Name, StringComparison.Ordinal) && string.Equals(Clause.ToFullString(), other.Clause.ToFullString(), StringComparison.Ordinal);
		}

		bool IEquatable<GlobalImport>.Equals(GlobalImport other)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Equals
			return this.Equals(other);
		}

		public override int GetHashCode()
		{
			return Hash.Combine(Name.GetHashCode(), StringComparer.Ordinal.GetHashCode(Clause.ToFullString()));
		}

		public static bool operator ==(GlobalImport left, GlobalImport right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(GlobalImport left, GlobalImport right)
		{
			return !object.Equals(left, right);
		}
	}
}
