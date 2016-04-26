using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TaskManager : Singleton<TaskManager>
{
	private List<Task> _taskList;

	void Awake()
	{
		_taskList = new List<Task>();
		StartCoroutine(RunTask());
	}

	public void AddTask(Task task)
	{
		_taskList.Add(task);
	}

	public IEnumerator RunTask()
	{
		while(true)
		{
			for (int i = _taskList.Count - 1; i >= 0; i--) {
				if(!_taskList[i].Run())
				{
					_taskList[i].Dispose();
					_taskList.RemoveAt(i);
				}
				else
				{
					if(_taskList[i].Current() is TaskWaitForSeconds)
					{
						_taskList[i].StartWaitForSeconds((TaskWaitForSeconds)_taskList[i].Current());
					}
				}
			}
			yield return null;
		}
		yield return null;
	}

}

public class TaskWaitForSeconds
{
	public readonly float seconds;
	private float startSeconds;
	public TaskWaitForSeconds(float seconds)
	{
		this.seconds = seconds;
		startSeconds = Time.time;
	}

	public bool Finish()
	{
		return (Time.time - startSeconds) > seconds;
	}
}

