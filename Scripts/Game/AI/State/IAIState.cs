namespace MTB
{
    public interface IAIState
    {

        /**
         * 进入状态
         * **/
        void stateIn();

        /**
         * 结束状态
         * **/
        void stateOut();

        /**
         * 当前ai
         * **/
        IAIComponent getAIComponent();

        /**
         * 每帧执行，返回下一个状态 (返回当前状态时不转换)
         * **/
        AIStateType onThink();

        void dispose();

    }
}
