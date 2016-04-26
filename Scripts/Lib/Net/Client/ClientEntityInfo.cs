using System;
using System.Collections.Generic;
using UnityEngine;
namespace MTB
{
	public class ClientEntityInfo
	{
		public int aoId{get;set;}
		public int type{get;set;}
		public int entityId{get;set;}
		public Vector3 position{get;set;}
		public List<int> extData{get;set;}
		public int roleId{get;set;}
		public ClientEntityInfo ()
		{
		}
	}
}

