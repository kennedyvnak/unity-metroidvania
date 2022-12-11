namespace Metroidvania.Events {
    public delegate void EventChannelAction();
    public delegate void EventChannelAction<T0>(T0 value0);
    public delegate void EventChannelAction<T0, T1>(T0 value0, T1 value1);
    public delegate void EventChannelAction<T0, T1, T2>(T0 value0, T1 value1, T2 value2);
}