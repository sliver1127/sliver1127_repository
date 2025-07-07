public interface IState
{
    
    void OnEnter();
    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    void OnExit();
    /// <summary>
    /// Called every frame while in this state.
    /// </summary>
    void OnUpdate();
}