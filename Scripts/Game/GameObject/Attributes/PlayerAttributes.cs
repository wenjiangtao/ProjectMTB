using System;
namespace MTB
{
	public class PlayerAttributes : BaseAttributes
	{
		public int handMaterialId{get;set;}

		public int playerId
		{
			get
			{
				return objectId;
			}
			set
			{
				objectId = value;
			}
		}

		protected override void Awake ()
		{
			base.Awake ();
			handMaterialId = 0;
		}
	}
}

