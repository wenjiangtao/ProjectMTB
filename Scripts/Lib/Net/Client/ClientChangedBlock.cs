using System;
namespace MTB
{
	public struct ClientChangedBlock
	{
		public Int16 index{get;private set;}
		public byte blockType{get;private set;}
		public byte extendId{get;private set;}
		public ClientChangedBlock (Int16 index,byte blockType,byte extendId)
		{
			this.index = index;
			this.blockType = blockType;
			this.extendId = extendId;
		}
	}
}

