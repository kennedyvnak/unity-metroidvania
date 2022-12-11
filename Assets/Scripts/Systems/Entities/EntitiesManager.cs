using Metroidvania.Events;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Metroidvania.Entities {
    // Why don't I make the main character the only target? To not create dependencies and allow the creation of 
    // a multiplayer system or others systems that include multiples targets in the future
    /// <summary>Class for make entity management. see also <see cref="Addressables"/></summary>
    public class EntitiesManager : ScriptableSingleton<EntitiesManager> {
        public delegate void OnEntityDelegate(EntityBehaviour entity);

        /// <summary>All available targets in scene</summary>
        public List<EntityTarget> targets { get; private set; } = new List<EntityTarget>();

        /// <summary>All instantiated entities in scene</summary>
        public List<EntityBehaviour> entities { get; private set; } = new List<EntityBehaviour>();

        /// <summary>Called when a target is added to the manager</summary>
        public ObjectEventChannel targetValidated;

        /// <summary>Called when a target is removed from the manager</summary>
        public ObjectEventChannel targetReleased;

        /// <summary>Called when a entity is instantiated by the manager. see also <see cref="InstantiateEntity"/></summary>
        public ObjectEventChannel entityInstantiated;

        /// <summary>Called when a entity is destroyed by the manager. see also <see cref="DestroyEntity"/></summary>
        public ObjectEventChannel entityDestroyed;

        /// <summary>Instantiate a new entity to the given position</summary>
        /// <param name="entityObject">Entity to be instantiated</param>
        /// <param name="position">Position where the entity will be instantiated in world position units</param>
        /// <param name="entityInstantiated">Event called when the entity is instantiated</param>
        /// <returns>The instantiation operation</returns>
        public AsyncOperationHandle<GameObject> InstantiateEntity(EntityObject entityObject, Vector2 position) {
            AsyncOperationHandle<GameObject> op = entityObject.prefab.InstantiateAsync(position, quaternion.identity);
            op.Completed += opHandle => {
                EntityBehaviour instantiatedEntity = opHandle.Result.GetComponent<EntityBehaviour>();
                entities.Add(instantiatedEntity);
                entityInstantiated?.Raise(instantiatedEntity);
            };
            return op;
        }

        /// <summary>Destroy the given entity</summary>
        /// <param name="entity">The entity that will be destroyed</param>
        public void DestroyEntity(EntityBehaviour entity) {
            if (!entities.Contains(entity))
                return;

            entities.Remove(entity);
            entityDestroyed?.Raise(entity);
            Addressables.ReleaseInstance(entity.gameObject);
        }

        /// <summary>Add a target to the manager</summary>
        /// <param name="target">The target to be added</param>
        public void AddTarget(EntityTarget target) {
            if (targets.Contains(target))
                return;
            targets.Add(target);
            targetValidated?.Raise(target);
        }

        /// <summary>Remove a target from the manager</summary>
        /// <param name="target">The target to be removed</param>
        public void RemoveTarget(EntityTarget target) {
            if (!targets.Contains(target))
                return;
            targets.Remove(target);
            targetReleased?.Raise(target);
        }

        /// <summary>Get the closest available target in the scene</summary>
        /// <param name="position">The position to compare distances</param>
        /// <returns>The closest target in scene</returns>
        public EntityTarget GetClosestTarget(Vector2 position) {
            return targets.Count switch {
                // If has only one target, return it
                1 => targets[0],
                // If there are no targets, returns null
                <= 0 => null,
                // If have more than one target, return the closest one
                > 1 => GetClosest()
            };

            EntityTarget GetClosest() {
                EntityTarget closest = null;
                float curDistance = Mathf.Infinity;

                foreach (EntityTarget target in targets) {
                    float distance = (position - (Vector2)target.t.position).sqrMagnitude;
                    if (distance >= curDistance)
                        continue;
                    curDistance = distance;
                    closest = target;
                }

                return closest;
            }
        }
    }
}