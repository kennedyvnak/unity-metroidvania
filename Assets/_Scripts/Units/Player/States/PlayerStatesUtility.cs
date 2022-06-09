namespace Metroidvania.Player.States
{
    /// <summary>Utility class for the player states</summary>
    public static class PlayerStatesUtility
    {
        /// <summary>Enter this state on the machine</summary>
        /// <param name="state">The state to enter</param>
        public static T SetActive<T>(this T state) where T : PlayerStateBase
        {
            state.machine.SwitchState(state);
            return state;
        }

        public static bool IsCrouchState(PlayerStateBase previousState)
        {
            return previousState.metadatas.TryGetMetadata<PlayerCrouchMetadata>(out var md);
        }
    }
}