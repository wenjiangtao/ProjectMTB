namespace MTB
{
    public interface IMonsterAIState : IAIState
    {
        AIStateType getType();
        IAIContext getAIContext();
        BaseMonsterAIComponent getMonsterAIComponent();
    }
}
