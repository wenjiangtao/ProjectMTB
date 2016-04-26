using System;
using System.Threading;
namespace MTB
{
	public abstract class ProcessWorker
	{
		protected bool _exit = true;
		protected bool _realExit = true;
		protected Thread workerThread;
		protected DataProcessorManager _manager;
		public ProcessWorker (DataProcessorManager manager)
		{
			_manager = manager;
		}

		public void Start()
		{
			_exit = false;
			_realExit = false;
			workerThread = new Thread(new ThreadStart(DoWork));
			workerThread.IsBackground = true;
			workerThread.Start();
		}

		public void Stop()
		{
			_exit = true;
			while(!_realExit);
		}

		protected void DoWork()
		{
			while(!_exit)
			{
				try{
				Process();
				}catch(Exception e)
				{
					UnityEngine.Debug.LogError(e.Message + "\n" + e.StackTrace);
				}
				Thread.Sleep(1);
			}
			_realExit = true;
		}

		protected abstract void Process();
	}
}

