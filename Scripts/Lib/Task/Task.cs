using System;
using System.Collections;
using UnityEngine;
public class Task
{
	public delegate void FinishHandler();
	public event FinishHandler OnFinished;
	private IEnumerator _c;
	private TaskWaitForSeconds _t;

	public Task (IEnumerator c)
	{
		_c = c;
	}

	public void StartWaitForSeconds(TaskWaitForSeconds t)
	{
		_t = t;
	}

	public bool Run()
	{
		if(_t!=null)
		{
			if(!_t.Finish())return true;
			else _t = null;
		}
		if(_c.MoveNext())
		{
			return true;
		}
		else
		{
			if(OnFinished != null)
			{
				OnFinished();
			}
		}
		return false;
	}

	public object Current()
	{
		return _c.Current;
	}

	public void Dispose()
	{
		_c = null;
	}
}

