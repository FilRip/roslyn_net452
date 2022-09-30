using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public class DiagnosticInfo : IFormattable, IObjectWritable
    {
        private readonly CommonMessageProvider _messageProvider;

        private readonly int _errorCode;

        private readonly DiagnosticSeverity _defaultSeverity;

        private readonly DiagnosticSeverity _effectiveSeverity;

        private readonly object[] _arguments;

        private static ImmutableDictionary<int, DiagnosticDescriptor> s_errorCodeToDescriptorMap;

        private static readonly ImmutableArray<string> s_compilerErrorCustomTags;

        private static readonly ImmutableArray<string> s_compilerNonErrorCustomTags;

        bool IObjectWritable.ShouldReuseInSerialization => false;

        public int Code => _errorCode;

        public virtual DiagnosticDescriptor Descriptor => GetOrCreateDescriptor(_errorCode, _defaultSeverity, _messageProvider);

        public DiagnosticSeverity Severity => _effectiveSeverity;

        public DiagnosticSeverity DefaultSeverity => _defaultSeverity;

        public int WarningLevel
        {
            get
            {
                if (_effectiveSeverity != _defaultSeverity)
                {
                    return Diagnostic.GetDefaultWarningLevel(_effectiveSeverity);
                }
                return _messageProvider.GetWarningLevel(_errorCode);
            }
        }

        public bool IsWarningAsError
        {
            get
            {
                if (DefaultSeverity == DiagnosticSeverity.Warning)
                {
                    return Severity == DiagnosticSeverity.Error;
                }
                return false;
            }
        }

        public string Category => _messageProvider.GetCategory(_errorCode);

        internal ImmutableArray<string> CustomTags => GetCustomTags(_defaultSeverity);

        public virtual IReadOnlyList<Location> AdditionalLocations => SpecializedCollections.EmptyReadOnlyList<Location>();

        public virtual string MessageIdentifier => _messageProvider.GetIdForErrorCode(_errorCode);

        public object[] Arguments => _arguments;

        public CommonMessageProvider MessageProvider => _messageProvider;

        static DiagnosticInfo()
        {
            s_errorCodeToDescriptorMap = ImmutableDictionary<int, DiagnosticDescriptor>.Empty;
            s_compilerErrorCustomTags = ImmutableArray.Create("Compiler", "Telemetry", "NotConfigurable");
            s_compilerNonErrorCustomTags = ImmutableArray.Create("Compiler", "Telemetry");
            ObjectBinder.RegisterTypeReader(typeof(DiagnosticInfo), (ObjectReader r) => new DiagnosticInfo(r));
        }

        public DiagnosticInfo(CommonMessageProvider messageProvider, int errorCode)
        {
            _messageProvider = messageProvider;
            _errorCode = errorCode;
            _defaultSeverity = messageProvider.GetSeverity(errorCode);
            _effectiveSeverity = _defaultSeverity;
            _arguments = new object[0];
        }

        public DiagnosticInfo(CommonMessageProvider messageProvider, int errorCode, params object[] arguments)
            : this(messageProvider, errorCode)
        {
            _arguments = arguments;
        }

        protected DiagnosticInfo(DiagnosticInfo original, DiagnosticSeverity overriddenSeverity)
        {
            _messageProvider = original.MessageProvider;
            _errorCode = original._errorCode;
            _defaultSeverity = original.DefaultSeverity;
            _arguments = original._arguments;
            _effectiveSeverity = overriddenSeverity;
        }

        internal static DiagnosticDescriptor GetDescriptor(int errorCode, CommonMessageProvider messageProvider)
        {
            DiagnosticSeverity severity = messageProvider.GetSeverity(errorCode);
            return GetOrCreateDescriptor(errorCode, severity, messageProvider);
        }

        private static DiagnosticDescriptor GetOrCreateDescriptor(int errorCode, DiagnosticSeverity defaultSeverity, CommonMessageProvider messageProvider)
        {
            CommonMessageProvider messageProvider2 = messageProvider;
            return ImmutableInterlocked.GetOrAdd(ref s_errorCodeToDescriptorMap, errorCode, (int code) => CreateDescriptor(code, defaultSeverity, messageProvider2));
        }

        private static DiagnosticDescriptor CreateDescriptor(int errorCode, DiagnosticSeverity defaultSeverity, CommonMessageProvider messageProvider)
        {
            string idForErrorCode = messageProvider.GetIdForErrorCode(errorCode);
            LocalizableString title = messageProvider.GetTitle(errorCode);
            LocalizableString description = messageProvider.GetDescription(errorCode);
            LocalizableString messageFormat = messageProvider.GetMessageFormat(errorCode);
            string helpLink = messageProvider.GetHelpLink(errorCode);
            string category = messageProvider.GetCategory(errorCode);
            ImmutableArray<string> customTags = GetCustomTags(defaultSeverity);
            return new DiagnosticDescriptor(idForErrorCode, title, messageFormat, category, defaultSeverity, isEnabledByDefault: true, description, helpLink, customTags);
        }

        [Conditional("DEBUG")]
        internal static void AssertMessageSerializable(object[] args)
        {
            foreach (object obj in args)
            {
                if (!(obj is IFormattable))
                {
                    Type type = obj.GetType();
                    if (!(type == typeof(string)) && !(type == typeof(AssemblyIdentity)) && !type.GetTypeInfo().IsPrimitive)
                    {
                        throw ExceptionUtilities.UnexpectedValue(type);
                    }
                }
            }
        }

        public DiagnosticInfo(CommonMessageProvider messageProvider, bool isWarningAsError, int errorCode, params object[] arguments)
            : this(messageProvider, errorCode, arguments)
        {
            if (isWarningAsError)
            {
                _effectiveSeverity = DiagnosticSeverity.Error;
            }
        }

        public virtual DiagnosticInfo GetInstanceWithSeverity(DiagnosticSeverity severity)
        {
            return new DiagnosticInfo(this, severity);
        }

        void IObjectWritable.WriteTo(ObjectWriter writer)
        {
            WriteTo(writer);
        }

        protected virtual void WriteTo(ObjectWriter writer)
        {
            writer.WriteValue(_messageProvider);
            writer.WriteUInt32((uint)_errorCode);
            writer.WriteInt32((int)_effectiveSeverity);
            writer.WriteInt32((int)_defaultSeverity);
            int num = _arguments.Length;
            writer.WriteUInt32((uint)num);
            if (num > 0)
            {
                object[] arguments = _arguments;
                foreach (object obj in arguments)
                {
                    writer.WriteString(obj.ToString());
                }
            }
        }

        protected DiagnosticInfo(ObjectReader reader)
        {
            _messageProvider = (CommonMessageProvider)reader.ReadValue();
            _errorCode = (int)reader.ReadUInt32();
            _effectiveSeverity = (DiagnosticSeverity)reader.ReadInt32();
            _defaultSeverity = (DiagnosticSeverity)reader.ReadInt32();
            int num = (int)reader.ReadUInt32();
            if (num > 0)
            {
                object[] array = (_arguments = new string[num]);
                for (int i = 0; i < num; i++)
                {
                    _arguments[i] = reader.ReadString();
                }
            }
            else
            {
                _arguments = new object[0];
            }
        }

        private static ImmutableArray<string> GetCustomTags(DiagnosticSeverity defaultSeverity)
        {
            if (defaultSeverity != DiagnosticSeverity.Error)
            {
                return s_compilerNonErrorCustomTags;
            }
            return s_compilerErrorCustomTags;
        }

        internal bool IsNotConfigurable()
        {
            return _defaultSeverity == DiagnosticSeverity.Error;
        }

        public virtual string GetMessage(IFormatProvider? formatProvider = null)
        {
            string text = _messageProvider.LoadMessage(_errorCode, formatProvider as CultureInfo);
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            if (_arguments.Length == 0)
            {
                return text;
            }
            return string.Format(formatProvider, text, GetArgumentsToUse(formatProvider));
        }

        protected object[] GetArgumentsToUse(IFormatProvider? formatProvider)
        {
            object[] array = null;
            for (int i = 0; i < _arguments.Length; i++)
            {
                if (_arguments[i] is DiagnosticInfo diagnosticInfo)
                {
                    array = InitializeArgumentListIfNeeded(array);
                    array[i] = diagnosticInfo.GetMessage(formatProvider);
                    continue;
                }
                ISymbol symbol = (_arguments[i] as ISymbol) ?? (_arguments[i] as ISymbolInternal)?.GetISymbol();
                if (symbol != null)
                {
                    array = InitializeArgumentListIfNeeded(array);
                    array[i] = _messageProvider.GetErrorDisplayString(symbol);
                }
            }
            return array ?? _arguments;
        }

        private object[] InitializeArgumentListIfNeeded(object[]? argumentsToUse)
        {
            if (argumentsToUse != null)
            {
                return argumentsToUse;
            }
            object[] array = new object[_arguments.Length];
            Array.Copy(_arguments, array, array.Length);
            return array;
        }

        public override string? ToString()
        {
            return ToString(null);
        }

        public string ToString(IFormatProvider? formatProvider)
        {
            return ((IFormattable)this).ToString(null, formatProvider);
        }

        string IFormattable.ToString(string? format, IFormatProvider? formatProvider)
        {
            return string.Format(formatProvider, "{0}: {1}", _messageProvider.GetMessagePrefix(MessageIdentifier, Severity, IsWarningAsError, formatProvider as CultureInfo), GetMessage(formatProvider));
        }

        public sealed override int GetHashCode()
        {
            int num = _errorCode;
            for (int i = 0; i < _arguments.Length; i++)
            {
                num = Hash.Combine(_arguments[i], num);
            }
            return num;
        }

        public sealed override bool Equals(object? obj)
        {
            DiagnosticInfo diagnosticInfo = obj as DiagnosticInfo;
            bool result = false;
            if (diagnosticInfo != null && diagnosticInfo._errorCode == _errorCode && diagnosticInfo.GetType() == GetType() && _arguments.Length == diagnosticInfo._arguments.Length)
            {
                result = true;
                for (int i = 0; i < _arguments.Length; i++)
                {
                    if (!object.Equals(_arguments[i], diagnosticInfo._arguments[i]))
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        private string? GetDebuggerDisplay()
        {
            return Code switch
            {
                -1 => "Unresolved DiagnosticInfo",
                -2 => "Void DiagnosticInfo",
                _ => ToString(),
            };
        }

        public virtual DiagnosticInfo GetResolvedInfo()
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
