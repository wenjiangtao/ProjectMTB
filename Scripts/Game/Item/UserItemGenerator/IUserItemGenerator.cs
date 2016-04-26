using System;
namespace MTB
{
	public interface IUserItemGenerator
	{
		UserItem Generate(params object[] param);
	}
}

