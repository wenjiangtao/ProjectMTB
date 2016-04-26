using System;
namespace MTB
{
	public class ValueSerializationFactory
	{
		private static IValueSerialization[] _map = new IValueSerialization[]{
			new Int32ValueSerialization(),
			new ByteValueSerialization(),
			new BlockValueSerialization()
		};
		public ValueSerializationFactory ()
		{
		}

		public static IValueSerialization GetSerialization(ValueSerializationType type)
		{
			return _map[(byte)type];
		}
	}
}

