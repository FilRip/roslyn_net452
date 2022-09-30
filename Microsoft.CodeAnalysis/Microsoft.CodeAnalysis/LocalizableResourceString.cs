using System;
using System.Globalization;
using System.Linq;
using System.Resources;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class LocalizableResourceString : LocalizableString, IObjectWritable
    {
        private readonly string _nameOfLocalizableResource;

        private readonly ResourceManager _resourceManager;

        private readonly Type _resourceSource;

        private readonly string[] _formatArguments;

        bool IObjectWritable.ShouldReuseInSerialization => false;

        static LocalizableResourceString()
        {
            ObjectBinder.RegisterTypeReader(typeof(LocalizableResourceString), (ObjectReader reader) => new LocalizableResourceString(reader));
        }

        public LocalizableResourceString(string nameOfLocalizableResource, ResourceManager resourceManager, Type resourceSource)
            : this(nameOfLocalizableResource, resourceManager, resourceSource, new string[0])
        {
        }

        public LocalizableResourceString(string nameOfLocalizableResource, ResourceManager resourceManager, Type resourceSource, params string[] formatArguments)
        {
            if (nameOfLocalizableResource == null)
            {
                throw new ArgumentNullException("nameOfLocalizableResource");
            }
            if (resourceManager == null)
            {
                throw new ArgumentNullException("resourceManager");
            }
            if (resourceSource == null)
            {
                throw new ArgumentNullException("resourceSource");
            }
            if (formatArguments == null)
            {
                throw new ArgumentNullException("formatArguments");
            }
            _resourceManager = resourceManager;
            _nameOfLocalizableResource = nameOfLocalizableResource;
            _resourceSource = resourceSource;
            _formatArguments = formatArguments;
        }

        private LocalizableResourceString(ObjectReader reader)
        {
            _resourceSource = reader.ReadType();
            _nameOfLocalizableResource = reader.ReadString();
            _resourceManager = new ResourceManager(_resourceSource);
            int num = reader.ReadInt32();
            if (num == 0)
            {
                _formatArguments = new string[0];
                return;
            }
            ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance(num);
            for (int i = 0; i < num; i++)
            {
                instance.Add(reader.ReadString());
            }
            _formatArguments = instance.ToArrayAndFree();
        }

        void IObjectWritable.WriteTo(ObjectWriter writer)
        {
            writer.WriteType(_resourceSource);
            writer.WriteString(_nameOfLocalizableResource);
            int num = _formatArguments.Length;
            writer.WriteInt32(num);
            for (int i = 0; i < num; i++)
            {
                writer.WriteString(_formatArguments[i]);
            }
        }

        protected override string GetText(IFormatProvider? formatProvider)
        {
            CultureInfo culture = (formatProvider as CultureInfo) ?? CultureInfo.CurrentUICulture;
            string @string = _resourceManager.GetString(_nameOfLocalizableResource, culture);
            if (@string == null)
            {
                return string.Empty;
            }
            if (_formatArguments.Length == 0)
            {
                return @string;
            }
            object[] formatArguments = _formatArguments;
            return string.Format(@string, formatArguments);
        }

        protected override bool AreEqual(object? other)
        {
            if (other is LocalizableResourceString localizableResourceString && _nameOfLocalizableResource == localizableResourceString._nameOfLocalizableResource && _resourceManager == localizableResourceString._resourceManager && _resourceSource == localizableResourceString._resourceSource)
            {
                return _formatArguments.SequenceEqual(localizableResourceString._formatArguments, (string a, string b) => a == b);
            }
            return false;
        }

        protected override int GetHash()
        {
            return Hash.Combine(_nameOfLocalizableResource.GetHashCode(), Hash.Combine(_resourceManager.GetHashCode(), Hash.Combine(_resourceSource.GetHashCode(), Hash.CombineValues(_formatArguments))));
        }
    }
}
