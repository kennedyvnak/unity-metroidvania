using Metroidvania.Events.Tracker;
using UnityEngine;

namespace Metroidvania.Events {
    public abstract class EventChannelBase : ScriptableObject {
        [TextArea()] public string eventDescription;

        protected EventsTracker.EventTrack BeginTrack(params object[] args) {
            return !EventsTracker.instance.trackingEnabled
                ? default(EventsTracker.EventTrack)
                : EventsTracker.instance.BeginEventTrack(channel: this, args);
        }
    }

    public abstract class EventChannelBase<T0> : EventChannelBase {
        public EventChannelAction<T0> OnEventRaise;

        public void Raise(T0 arg0) {
            using (BeginTrack(arg0))
                OnEventRaise?.Invoke(arg0);
        }
    }

    public abstract class EventChannelBase<T0, T1> : EventChannelBase {
        public EventChannelAction<T0, T1> OnEventRaise;

        public void Raise(T0 arg0, T1 arg1) {
            using (BeginTrack(arg0, arg1))
                OnEventRaise?.Invoke(arg0, arg1);
        }
    }
}
