using System.Collections.Generic;
namespace MTB
{
    public class BaseTaskCondition
    {
        protected Dictionary<string, string> _paras;
        public int taskId { get; set; }
        public int stepId { get; set; }
        public bool isMeet = false;


        public virtual void setParams(Dictionary<string, string> paras)
        {
            _paras = paras;
        }

        public virtual bool MeetCondition()
        {
            isMeet = true;
            return true;
        }

        public virtual void dispose()
        {
            taskId = 0;
        }
    }
}
