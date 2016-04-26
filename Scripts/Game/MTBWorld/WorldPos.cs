using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MTB
{
	public struct WorldPos {
		public int x;
		public int y;
		public int z;
		public int hashCode{get;private set;}
		public WorldPos(int x,int y,int z)
		:this()
		{
			this.x = x;
			this.y = y;
			this.z = z;
			hashCode = GetHashCodeSelf();
		}

		public bool EqualOther(WorldPos other)
		{
			return x == other.x && y == other.y && z == other.z;
		}

		public override bool Equals (object obj)
		{
			if(obj is WorldPos)
			{
				WorldPos other = (WorldPos)obj;
				return x == other.x && y == other.y && z == other.z;
			}
			return false;
		}

		public override int GetHashCode ()
		{
			unchecked
			{
				int hash = 47;
				
				hash = hash * 227 + x.GetHashCode();
				hash = hash * 227 + y.GetHashCode();
				hash = hash * 227 + z.GetHashCode();
				hashCode = hash;
				return hash;
			}
		}

		private int GetHashCodeSelf()
		{
			int hash = 47;
			hash = hash * 227 + x.GetHashCode();
			hash = hash * 227 + y.GetHashCode();
			hash = hash * 227 + z.GetHashCode();
			return hash;
		}

		public override string ToString ()
		{
			return string.Format ("{0}:{1}:{2}",x,y,z);
		}
	}

	public class WorldPosComparer : IEqualityComparer<WorldPos>
	{
		#region IEqualityComparer implementation

		bool IEqualityComparer<WorldPos>.Equals (WorldPos p1, WorldPos p2)
		{
			return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;
		}

		int IEqualityComparer<WorldPos>.GetHashCode (WorldPos obj)
		{
			return obj.hashCode;
		}

		#endregion


	}
}
