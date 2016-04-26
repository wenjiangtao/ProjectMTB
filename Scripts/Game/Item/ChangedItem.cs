using System;
using System.Collections.Generic;
namespace MTB
{
	public class ChangedItem
	{
		public delegate void ChangedPropertyHandler(ChangedItem item,string property,object oldValue,object newValue);
		public event ChangedPropertyHandler On_PropertyChanged;

		private Dictionary<string,object> _valueMap;
		public ChangedItem ()
		{
			_valueMap = new Dictionary<string, object>();
		}

		public void SetProperty(string property,object value)
		{
			object oldValue;
			_valueMap.TryGetValue(property,out oldValue);
			if(oldValue == null)
			{
				_valueMap.Add(property,value);
				return;
			}
			if(oldValue != value)
			{
				_valueMap[property] = value;
				if(On_PropertyChanged != null)
				{
					On_PropertyChanged(this,property,oldValue,value);
				}
			}
		}

		public object GetProperty(string property)
		{
			object value;
			_valueMap.TryGetValue(property,out value);
			return value;
		}
	}
}

