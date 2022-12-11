namespace Metroidvania.Entities {
    /// <summary>The base class for entity behaviours</summary>
    /// <typeparam name="TEntity">The entity behaviour type for handle the TState</typeparam>
    public abstract class EntityStateMachine<TEntity> : EntityBehaviour
        where TEntity : EntityStateMachine<TEntity> {
        /// <summary>The state running in the machine</summary>
        protected virtual EntityBehaviourState<TEntity> currentState { get; set; }

        protected virtual void Update() {
            currentState?.LogicUpdate();
        }

        /// <summary>Switches the current state of this machine</summary>
        protected virtual void SwitchState(EntityBehaviourState<TEntity> state) {
            currentState?.Exit();
            currentState = state;
            currentState.Enter();
        }
    }
}