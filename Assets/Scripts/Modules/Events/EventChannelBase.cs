using UnityEngine;

namespace Metroidvania.Events
{
    public abstract class EventChannelBase : ScriptableObject
    {
        [TextArea()] public string eventDescription;
    }

    public abstract class EventChannelBase<T0> : EventChannelBase
    {
        public EventChannelAction<T0> OnEventRaise;

        public void Raise(T0 arg0)
        {
            OnEventRaise?.Invoke(arg0);
        }
    }

    public abstract class EventChannelBase<T0, T1> : EventChannelBase
    {
        public EventChannelAction<T0, T1> OnEventRaise;

        public void Raise(T0 arg0, T1 arg1)
        {
            OnEventRaise?.Invoke(arg0, arg1);
        }
    }
}
