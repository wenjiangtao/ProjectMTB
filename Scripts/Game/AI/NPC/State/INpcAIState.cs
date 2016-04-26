namespace MTB
{
    public interface INpcAIState : IAIState
    {
        AIStateType getType();
        BaseNPCAIComponent getNPCAIComponent();
    }
}
