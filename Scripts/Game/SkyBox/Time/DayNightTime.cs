using UnityEngine;
using System.Collections;

namespace MTB
{
	public class DayNightTime : Singleton<DayNightTime> {
		public int normalTimeCycles = 10;
		public TimeSlotConfig normalTimeConfig = new TimeSlotConfig();
		public TimeSlotConfig specialTimeConfig = new TimeSlotConfig();
		public bool usePersistanceTime = true;

		public float _time;
		public float TotalTime{get{return _time;}}
		public DayTimeSlot _timeSlot;
		public DayTimeSlot TimeSlot{get{return _timeSlot;}}
		public float _slotElapsedTime;
		public float SlotElapsedTime{get{return _slotElapsedTime;}}
		public float _slotTime;
		public float SlotTime{get{return _slotTime; }}
		public bool _isSpecialTime;
		public bool IsSpecialTime{get{return _isSpecialTime;}}
		public int _days;
		public int Days{get{return _days;}}

		private float _totalCycleTime;
		private float _totalNormalCycleTime;
		private bool _isNetTime;

		void Awake()
		{
			DayNightTime.Instance = this;
			normalTimeConfig.Init();
			specialTimeConfig.Init();
			_totalNormalCycleTime = normalTimeConfig.TotalSeconds * normalTimeCycles;
			_totalCycleTime = _totalNormalCycleTime + specialTimeConfig.TotalSeconds;
		}

		public void SetNetTime(bool isNetTime)
		{
			this._isNetTime = isNetTime;
		}

		public void UpdateTime(float time)
		{
			if(_isNetTime)
			{
				_time = time;
				Refresh();
			}
		}

		void Start () {
			if(!_isNetTime)
			{
				if(usePersistanceTime)
					_time = PlayerPrefs.GetFloat("MTB_Time") ;
				else
					_time = 0;
				Refresh();
			}
		}
		
		void FixedUpdate()
		{
			_time += Time.fixedDeltaTime;
			Refresh();
		}
		void OnDestroy()
		{
			PlayerPrefs.SetFloat("MTB_Time", _time);
		}


		private void Refresh()
		{
			float oneCycleElapsedTime = _time % _totalCycleTime;
			int cycles = (int)(_time / _totalCycleTime);
			int daysPerCycle;
			TimeSlotConfig config;
			float oneDayElapsedTime;
			if(oneCycleElapsedTime < _totalNormalCycleTime)
			{
				_isSpecialTime = false;
				config = normalTimeConfig;
				oneDayElapsedTime = oneCycleElapsedTime % config.TotalSeconds;
				daysPerCycle = (int)(oneCycleElapsedTime / config.TotalSeconds) + 1;
			}
			else
			{
				_isSpecialTime = true;
				config = specialTimeConfig;
				oneDayElapsedTime = oneCycleElapsedTime - _totalNormalCycleTime;
				daysPerCycle = normalTimeCycles + 1;
			}
			_days = cycles * ( normalTimeCycles + 1) + daysPerCycle;
			DayTimeSlot curDayTimeSlot = config.GetSlot(oneDayElapsedTime,out _slotElapsedTime,out _slotTime);
			ChangeSlot(curDayTimeSlot);

		}

		private void ChangeSlot(DayTimeSlot slot)
		{
			_timeSlot = slot;
		}
	}

	[System.Serializable]
	public class TimeSlotConfig
	{
		public float daySeconds = 480f;
		public float nightSeconds = 480f;
		public float morningSeconds = 120f;
		public float eveningSeconds = 120f;


		public void Init()
		{
			_totalSeconds = daySeconds + nightSeconds + morningSeconds + eveningSeconds;
			_increaseTimeArray = new float[]{morningSeconds,morningSeconds + daySeconds,_totalSeconds - nightSeconds,_totalSeconds};
			_slotArray = new DayTimeSlot[]{DayTimeSlot.Morning,DayTimeSlot.Day,DayTimeSlot.Evening,DayTimeSlot.Night};
			_timeArray = new float[]{morningSeconds,daySeconds,eveningSeconds,nightSeconds};
		}

		private float _totalSeconds;
		public float TotalSeconds
		{
			get{
				return _totalSeconds;
			}
		}

		private DayTimeSlot[] _slotArray;
		private float[] _increaseTimeArray;
		private float[] _timeArray;
		public DayTimeSlot GetSlot(float time,out float elapsedTime,out float slotTime)
		{
			for (int i = 0; i < _increaseTimeArray.Length - 1; i++) {
				if(time < _increaseTimeArray[i])
				{
					elapsedTime = i > 0 ? time - _increaseTimeArray[i - 1] : time;
					slotTime = _timeArray[i];
					return _slotArray[i];
				}
			}
			elapsedTime = time - _increaseTimeArray[_increaseTimeArray.Length - 2];
			slotTime = _timeArray[_increaseTimeArray.Length - 1];
			return _slotArray[_increaseTimeArray.Length - 1];
		}
	}
}
