
namespace MTB
{
    public interface IAIStateManager
    {
        /**
         * 获取当前AI
         * **/
        IAIState getCurrentState();
        /**
         * 运行AI状态
         * **/
        void runAIState(AIStateType key);
    }
}
