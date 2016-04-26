using System;
namespace MTB
{
    public struct CaveValue
    {
        public CaveType type;
        public int lowHeight;
        public int highHeight;

        public CaveValue(CaveType type, int lowheight, int highheight)
        {
            this.type = type;
            this.lowHeight = lowheight;
            this.highHeight = highheight;
        }
    }
}
