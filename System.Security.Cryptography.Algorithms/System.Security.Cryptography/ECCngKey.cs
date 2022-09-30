using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography
{
	internal sealed class ECCngKey
	{
		private SafeNCryptKeyHandle _keyHandle;

		private int _lastKeySize;

		private string _lastAlgorithm;

		private readonly string _algorithmGroup;

		internal int KeySize { get; private set; }

		internal ECCngKey(string algorithmGroup)
		{
			_algorithmGroup = algorithmGroup;
		}

		internal string GetCurveName(int callerKeySizeProperty)
		{
			using SafeNCryptKeyHandle ncryptHandle = GetDuplicatedKeyHandle(callerKeySizeProperty);
			string lastAlgorithm = _lastAlgorithm;
			if (ECCng.IsECNamedCurve(lastAlgorithm))
			{
				return CngKeyLite.GetCurveName(ncryptHandle);
			}
			return ECCng.SpecialNistAlgorithmToCurveName(lastAlgorithm);
		}

		internal SafeNCryptKeyHandle GetDuplicatedKeyHandle(int callerKeySizeProperty)
		{
			if (ECCng.IsECNamedCurve(_lastAlgorithm))
			{
				return new DuplicateSafeNCryptKeyHandle(_keyHandle);
			}
			if (_lastKeySize != callerKeySizeProperty)
			{
				bool flag = _algorithmGroup == "ECDSA";
				string text = callerKeySizeProperty switch
				{
					256 => flag ? "ECDSA_P256" : "ECDH_P256", 
					384 => flag ? "ECDSA_P384" : "ECDH_P384", 
					521 => flag ? "ECDSA_P521" : "ECDH_P521", 
					_ => throw new ArgumentException(SR.Cryptography_InvalidKeySize), 
				};
				if (_keyHandle != null)
				{
					DisposeKey();
				}
				_keyHandle = CngKeyLite.GenerateNewExportableKey(text, callerKeySizeProperty);
				_lastKeySize = callerKeySizeProperty;
				_lastAlgorithm = text;
				KeySize = callerKeySizeProperty;
			}
			return new DuplicateSafeNCryptKeyHandle(_keyHandle);
		}

		internal void GenerateKey(ECCurve curve)
		{
			curve.Validate();
			if (_keyHandle != null)
			{
				DisposeKey();
			}
			string text = null;
			int num = 0;
			if (curve.IsNamed)
			{
				if (string.IsNullOrEmpty(curve.Oid.FriendlyName))
				{
					throw new PlatformNotSupportedException(string.Format(SR.Cryptography_InvalidCurveOid, curve.Oid.Value));
				}
				text = ((!(_algorithmGroup == "ECDSA")) ? ECCng.EcdhCurveNameToAlgorithm(curve.Oid.FriendlyName) : ECCng.EcdsaCurveNameToAlgorithm(curve.Oid.FriendlyName));
				if (ECCng.IsECNamedCurve(text))
				{
					try
					{
						_keyHandle = CngKeyLite.GenerateNewExportableKey(text, curve.Oid.FriendlyName);
						num = CngKeyLite.GetKeyLength(_keyHandle);
					}
					catch (CryptographicException ex)
					{
						Interop.NCrypt.ErrorCode hResult = (Interop.NCrypt.ErrorCode)ex.HResult;
						if ((curve.IsNamed && hResult == Interop.NCrypt.ErrorCode.NTE_INVALID_PARAMETER) || hResult == Interop.NCrypt.ErrorCode.NTE_NOT_SUPPORTED)
						{
							throw new PlatformNotSupportedException(string.Format(SR.Cryptography_CurveNotSupported, curve.Oid.FriendlyName), ex);
						}
						throw;
					}
				}
				else
				{
					switch (text)
					{
					case "ECDSA_P256":
					case "ECDH_P256":
						num = 256;
						break;
					case "ECDSA_P384":
					case "ECDH_P384":
						num = 384;
						break;
					case "ECDSA_P521":
					case "ECDH_P521":
						num = 521;
						break;
					default:
						throw new ArgumentException(SR.Cryptography_InvalidKeySize);
					}
					_keyHandle = CngKeyLite.GenerateNewExportableKey(text, num);
				}
			}
			else
			{
				if (!curve.IsExplicit)
				{
					throw new PlatformNotSupportedException(string.Format(SR.Cryptography_CurveNotSupported, curve.CurveType.ToString()));
				}
				text = _algorithmGroup;
				_keyHandle = CngKeyLite.GenerateNewExportableKey(text, ref curve);
				num = CngKeyLite.GetKeyLength(_keyHandle);
			}
			_lastAlgorithm = text;
			_lastKeySize = num;
			KeySize = num;
		}

		internal void DisposeKey()
		{
			if (_keyHandle != null)
			{
				_keyHandle.Dispose();
				_keyHandle = null;
			}
			_lastAlgorithm = null;
			_lastKeySize = 0;
		}

		internal void SetHandle(SafeNCryptKeyHandle keyHandle, string algorithmName)
		{
			_keyHandle?.Dispose();
			_keyHandle = keyHandle;
			_lastAlgorithm = algorithmName;
			KeySize = CngKeyLite.GetKeyLength(keyHandle);
			_lastKeySize = KeySize;
		}
	}
}
