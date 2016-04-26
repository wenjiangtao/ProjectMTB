using System;
namespace MTB
{
	public abstract class UdpPackage : NetPackage
	{
		public UdpPackage (int id)
			:base(id)
		{
		}

		public virtual void Do()
		{
		}
	}
}

