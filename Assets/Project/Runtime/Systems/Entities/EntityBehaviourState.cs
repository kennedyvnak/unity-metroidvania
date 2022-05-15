namespace Metroidvania.Entities
{
    /// <summary>
    /// The state of all entity state machines.
    /// </summary>
    /// <typeparam name="TEntity">The entity behaviour type</typeparam>
    public abstract class EntityBehaviourState<TEntity>
        where TEntity : EntityStateMachine<TEntity, EntityBehaviourState<TEntity>>
    {
        /// <summary>The target entity behaviour</summary>
        public readonly TEntity target;

        protected EntityBehaviourState(TEntity target)
        {
            this.target = target;
        }
        
        /// <summary>This method should be called in target.Update()</summary> 
        public virtual void LogicUpdate()
        {
        }

        /// <summary>This method should be called when the target switch to this state</summary> 
        public virtual void Enter()
        {
        }

        /// <summary>This method should be called when the target is in this state and switch to another state</summary> 
        public virtual void Exit()
        {
        }
    }
}