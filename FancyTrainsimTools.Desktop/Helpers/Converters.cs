using System;

namespace FancyTrainsimToolsDesktop.Helpers
	{
	public class Converters
		{
		public static UInt64 GetUuid()
			{
			var G = Guid.NewGuid();
			var Bytes = G.ToByteArray();
			return BitConverter.ToUInt64(Bytes, 0);
			}

		public static String GetUuidString()
			{
			return GetUuid().ToString().Substring(0, 10);
			}
		}
	}
