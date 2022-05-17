namespace Metroidvania.Entities
{
    /// <summary>The base class for entity behaviours</summary>
    /// <typeparam name="TEntity">The entity behaviour type</typeparam>
    /// <typeparam name="TState">The state type</typeparam>
    public abstract class EntityStateMachine<TEntity, TState> : EntityBehaviour
        where TEntity : EntityStateMachine<TEntity, EntityBehaviourState<TEntity>>
        where TState : EntityBehaviourState<TEntity>
    {
        /// <summary>The state running in the machine</summary>
        protected virtual TState currentState { get; set; }

        protected virtual void Update()
        {
            currentState?.LogicUpdate();
        }

        /// <summary>Switches the current state of this machine</summary>
        protected virtual void SwitchState(TState state)
        {
            currentState?.Exit();
            currentState = state;
            currentState.Enter();
        }
    }
}