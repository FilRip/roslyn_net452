using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class CustomObsoleteDiagnosticInfo : DiagnosticInfo
    {
        private DiagnosticDescriptor? _descriptor;

        internal ObsoleteAttributeData Data { get; }

        public override string MessageIdentifier
        {
            get
            {
                string diagnosticId = Data.DiagnosticId;
                if (!string.IsNullOrEmpty(diagnosticId))
                {
                    return diagnosticId;
                }
                return base.MessageIdentifier;
            }
        }

        public override DiagnosticDescriptor Descriptor
        {
            get
            {
                if (_descriptor == null)
                {
                    Interlocked.CompareExchange(ref _descriptor, CreateDescriptor(), null);
                }
                return _descriptor;
            }
        }

        public CustomObsoleteDiagnosticInfo(CommonMessageProvider messageProvider, int errorCode, ObsoleteAttributeData data, params object[] arguments)
            : base(messageProvider, errorCode, arguments)
        {
            Data = data;
        }

        private CustomObsoleteDiagnosticInfo(CustomObsoleteDiagnosticInfo baseInfo, DiagnosticSeverity effectiveSeverity)
            : base(baseInfo, effectiveSeverity)
        {
            Data = baseInfo.Data;
        }

        public override DiagnosticInfo GetInstanceWithSeverity(DiagnosticSeverity severity)
        {
            return new CustomObsoleteDiagnosticInfo(this, severity);
        }

        private DiagnosticDescriptor CreateDescriptor()
        {
            DiagnosticDescriptor descriptor = base.Descriptor;
            string diagnosticId = Data.DiagnosticId;
            string urlFormat = Data.UrlFormat;
            if (diagnosticId == null && urlFormat == null)
            {
                return descriptor;
            }
            string messageIdentifier = MessageIdentifier;
            string helpLinkUri = descriptor.HelpLinkUri;
            if (urlFormat != null)
            {
                try
                {
                    helpLinkUri = string.Format(urlFormat, messageIdentifier);
                }
                catch
                {
                }
            }
            ImmutableArray<string> customTags;
            if (diagnosticId == null)
            {
                customTags = descriptor.CustomTags.ToImmutableArray();
            }
            else
            {
                int num = 1;
                if (descriptor.CustomTags is ICollection<string> collection)
                {
                    int count = collection.Count;
                    num += count;
                }
                ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance(num);
                instance.AddRange(descriptor.CustomTags);
                instance.Add("CustomObsolete");
                customTags = instance.ToImmutableAndFree();
            }
            return new DiagnosticDescriptor(messageIdentifier, descriptor.Title, descriptor.MessageFormat, descriptor.Category, descriptor.DefaultSeverity, descriptor.IsEnabledByDefault, descriptor.Description, helpLinkUri, customTags);
        }
    }
}
