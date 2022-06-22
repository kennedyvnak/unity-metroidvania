using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroidvania.Player.States
{
    /// <summary>Base class for all player state metadatas</summary>
    public abstract class PlayerStateMetadataBase
    {
        private readonly PlayerStateBase _state;
        public PlayerStateBase state => _state;

        public PlayerStateMetadataBase(PlayerStateBase state)
        {
            _state = state;
            state.metadatas.AddMetadata(this);
        }
    }

    /// <summary>
    /// Modules are a little different from metadata. Metadata defines something in a state.
    /// A module simulates behaviour in the state
    /// </summary>
    public abstract class PlayerStateModuleBase : PlayerStateMetadataBase
    {
        protected PlayerStateModuleBase(PlayerStateBase state) : base(state)
        {
        }
    }

    /// <summary>Collection of player state metadatas</summary>
    public class PlayerStateMetadatas : IEnumerable<PlayerStateMetadataBase>
    {
        private readonly PlayerStateBase state;
        private List<PlayerStateMetadataBase> _metadatas = new List<PlayerStateMetadataBase>();

        public PlayerStateMetadatas(PlayerStateBase state)
        {
            this.state = state;
        }

        /// <summary>Get the first metadata of the requested type in collection</summary>
        public TMetadata GetMetadata<TMetadata>() where TMetadata : PlayerStateMetadataBase
        {
            foreach (PlayerStateMetadataBase Metadata in this)
            {
                if (Metadata is TMetadata targetMetadata)
                    return targetMetadata;
            }
            return null;
        }

        /// <summary>Get the first metadata of the requested type in collection and return true if the metadata don't is null</summary>
        public bool TryGetMetadata<TMetadata>(out TMetadata metadata) where TMetadata : PlayerStateMetadataBase
        {
            metadata = GetMetadata<TMetadata>();
            return metadata != null;
        }

        /// <summary>Add a metadata to the collection</summary>
        public void AddMetadata(PlayerStateMetadataBase metadata)
        {
            _metadatas.Add(metadata);
        }

        /// <summary>Remove a metadata from the collection</summary>
        public bool RemoveMetadata(PlayerStateMetadataBase metadata)
        {
            return _metadatas.Remove(metadata);
        }

        public IEnumerator<PlayerStateMetadataBase> GetEnumerator() => _metadatas.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>Use this metadata to define the state is an crouch state</summary>
    public class PlayerCrouchMetadata : PlayerStateMetadataBase
    {
        public PlayerCrouchMetadata(PlayerStateBase state) : base(state)
        {
        }

        /// <summary>A coroutine to perform the exit transition animation and switch the state.</summary>
        /// <param name="nextState">The state that will be active when the animation end</param>
        public IEnumerator ExitCrouchState(PlayerStateBase nextState)
        {
            state.machine.player.animator.SwitchAnimation(PlayerAnimator.CrouchTransitionAnimKey);
            yield return CoroutinesUtility.GetYieldSeconds(state.machine.player.data.crouchTransitionTime);
            nextState.SetActive();
        }
    }

    public class PlayerDurationModule : PlayerStateModuleBase
    {
        public float enterTime;

        public PlayerDurationModule(PlayerStateBase state) : base(state)
        {
        }

        public void Enter() => enterTime = Time.time;

        public bool HasElapsed(float duration) => GetElapsedTime() >= duration;

        public float GetElapsedTime() => Time.time - enterTime;
    }

    public class PlayerCooldownModule : PlayerStateModuleBase
    {
        private float _cooldownStartTime;

        public float cooldownDuration;

        public PlayerCooldownModule(PlayerStateBase state, float cooldownDuration) : base(state)
        {
            this.cooldownDuration = cooldownDuration;
        }

        public bool IsInCooldown() => Time.time - _cooldownStartTime <= cooldownDuration;

        public void StartCooldown() => _cooldownStartTime = Time.time;
    }

    public class PlayerInvincibilityModule : PlayerStateModuleBase
    {
        public PlayerInvincibilityModule(PlayerStateBase state) : base(state)
        {
        }
    }
}