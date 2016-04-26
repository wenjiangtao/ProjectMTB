using UnityEngine;
using System.Collections;

public class DelegateDef{
	public delegate void ParamsDelegate(params object[] paras);
	public delegate void VoidDelegate();
	public delegate void Vector2Delegate(Vector2 v);
}
