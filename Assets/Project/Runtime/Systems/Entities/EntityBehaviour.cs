namespace Metroidvania.Entities
{
    public abstract class EntityBehaviour<TEntity, TState> : EntityObject
        where TEntity : EntityBehaviour<TEntity, EntityBehaviourState<TEntity>>
        where TState : EntityBehaviourState<TEntity>
    {
        protected virtual TState currentState { get; set; }

        protected virtual void Update()
        {
            currentState?.LogicUpdate();
        }

        protected virtual void SwitchState(TState behaviour)
        {
            currentState?.Exit();
            currentState = behaviour;
            currentState.Enter();
        }
    }
}