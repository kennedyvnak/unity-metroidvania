namespace Metroidvania.Entities
{
    public abstract class EntityBehaviourState<TEntity>
        where TEntity : EntityBehaviour<TEntity, EntityBehaviourState<TEntity>>
    {
        public readonly TEntity target;

        protected EntityBehaviourState(TEntity target)
        {
            this.target = target;
        }

        public virtual void LogicUpdate()
        {
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }
    }
}