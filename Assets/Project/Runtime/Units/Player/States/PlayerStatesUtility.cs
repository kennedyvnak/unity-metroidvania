namespace Metroidvania.Player.States
{
    public static class PlayerStatesUtility
    {
        public static void SetActive(this PlayerStateBase state)
        {
            state.machine.SwitchState(state);
        }
    }
}