using System;

namespace AzureService.Core.Extension
{
	public static class GuidExtensions
	{
		public static string ToShortString(this Guid guid)
		{
			string enc = Convert.ToBase64String(guid.ToByteArray());
			enc = enc.Replace("/", "_");
			enc = enc.Replace("+", "-");
			return enc.Substring(0, 22);
		}

		public static Guid ToGuid(this string encoded)
		{
			encoded = encoded.Replace("_", "/");
			encoded = encoded.Replace("-", "+");
			byte[] buffer = Convert.FromBase64String(encoded + "==");
			return new Guid(buffer);
		}
	}
}