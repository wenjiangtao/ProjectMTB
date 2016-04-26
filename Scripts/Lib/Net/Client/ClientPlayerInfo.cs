using System;
using UnityEngine;
namespace MTB
{
	public class ClientPlayerInfo
	{
		public int id{get;set;}
		public int aoId{get;set;}
		public int playerId{get;set;}
		public Vector3 position{get;set;}
		public string configName{get;set;}
		public int seed{get;set;}
		public ClientPlayerInfo ()
		{
		}
	}
}

