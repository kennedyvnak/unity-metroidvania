using Metroidvania.Events.Tracker;
using UnityEngine;

// TODO: An events tracker window
namespace Metroidvania.Events
{
    public abstract class EventChannelBase : ScriptableObject
    {
        [TextArea()] public string eventDescription;

        protected void SendTrack(params object[] args)
        {
            if (!EventsTracker.instance.trackingEnabled)
                return;

            EventsTracker.instance.TrackEvent(channel: this, args);
        }
    }

    public abstract class EventChannelBase<T0> : EventChannelBase
    {
        public EventChannelAction<T0> OnEventRaise;

        public void Raise(T0 arg0)
        {
            OnEventRaise?.Invoke(arg0);
            SendTrack(arg0);
        }
    }

    public abstract class EventChannelBase<T0, T1> : EventChannelBase
    {
        public EventChannelAction<T0, T1> OnEventRaise;

        public void Raise(T0 arg0, T1 arg1)
        {
            OnEventRaise?.Invoke(arg0, arg1);
            SendTrack(arg0, arg1);
        }
    }
}
