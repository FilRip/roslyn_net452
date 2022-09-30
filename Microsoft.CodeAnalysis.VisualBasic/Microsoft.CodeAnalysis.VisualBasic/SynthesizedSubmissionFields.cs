using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class SynthesizedSubmissionFields
	{
		private readonly NamedTypeSymbol _declaringSubmissionClass;

		private readonly VisualBasicCompilation _compilation;

		private FieldSymbol _hostObjectField;

		private Dictionary<ImplicitNamedTypeSymbol, FieldSymbol> _previousSubmissionFieldMap;

		internal int Count
		{
			get
			{
				if (_previousSubmissionFieldMap != null)
				{
					return _previousSubmissionFieldMap.Count;
				}
				return 0;
			}
		}

		internal IEnumerable<FieldSymbol> FieldSymbols
		{
			get
			{
				if (_previousSubmissionFieldMap != null)
				{
					return _previousSubmissionFieldMap.Values;
				}
				return Array.Empty<FieldSymbol>();
			}
		}

		public SynthesizedSubmissionFields(VisualBasicCompilation compilation, NamedTypeSymbol submissionClass)
		{
			_declaringSubmissionClass = submissionClass;
			_compilation = compilation;
		}

		internal FieldSymbol GetHostObjectField()
		{
			if ((object)_hostObjectField != null)
			{
				return _hostObjectField;
			}
			TypeSymbol typeSymbol = null;
			if ((object)typeSymbol != null && typeSymbol.Kind != SymbolKind.ErrorType)
			{
				_hostObjectField = new SynthesizedFieldSymbol(_declaringSubmissionClass, _declaringSubmissionClass, typeSymbol, "<host-object>", Accessibility.Private, isReadOnly: true);
				return _hostObjectField;
			}
			return null;
		}

		internal FieldSymbol GetOrMakeField(ImplicitNamedTypeSymbol previousSubmissionType)
		{
			if (_previousSubmissionFieldMap == null)
			{
				_previousSubmissionFieldMap = new Dictionary<ImplicitNamedTypeSymbol, FieldSymbol>();
			}
			FieldSymbol value = null;
			if (!_previousSubmissionFieldMap.TryGetValue(previousSubmissionType, out value))
			{
				value = new SynthesizedFieldSymbol(_declaringSubmissionClass, _declaringSubmissionClass, previousSubmissionType, "<" + previousSubmissionType.Name + ">", Accessibility.Private, isReadOnly: true);
				_previousSubmissionFieldMap.Add(previousSubmissionType, value);
			}
			return value;
		}

		internal void AddToType(NamedTypeSymbol containingType, PEModuleBuilder moduleBeingBuilt)
		{
			foreach (FieldSymbol fieldSymbol in FieldSymbols)
			{
				moduleBeingBuilt.AddSynthesizedDefinition(containingType, fieldSymbol.GetCciAdapter());
			}
			FieldSymbol hostObjectField = GetHostObjectField();
			if ((object)hostObjectField != null)
			{
				moduleBeingBuilt.AddSynthesizedDefinition(containingType, hostObjectField.GetCciAdapter());
			}
		}
	}
}
