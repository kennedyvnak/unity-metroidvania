using Metroidvania.Events.Tracker.Handles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Metroidvania.Events.Tracker {
    public class EventsTracker : ScriptableSingleton<EventsTracker> {
        [System.Flags]
        public enum TracksHandler {
            File = 1 << 0,
        }

        public class EventTrack : IDisposable {
            public readonly EventsTracker tracker;
            public readonly EventChannelBase channel;

            public System.Action<EventTrack> trackReleased;

            public double invokeTime, releaseTime;

            public object[] invokeParams;

            public EventTrack(EventsTracker tracker, EventChannelBase channel, System.Action<EventTrack> trackReleased, object[] invokeParams) {
                this.tracker = tracker;
                this.channel = channel;
                this.invokeParams = invokeParams;
                this.trackReleased = trackReleased;
                invokeTime = Time.realtimeSinceStartupAsDouble;
                releaseTime = -1;
            }

            public void Dispose() {
                releaseTime = Time.realtimeSinceStartupAsDouble;
                trackReleased?.Invoke(this);
            }
        }

        public static System.Action<EventTrack> OnBeginEventTrack;
        public static System.Action<EventTrack> OnEndEventTrack;

        [SerializeField] private bool m_trackingEnabled;
        [SerializeField] private TracksHandler m_enabledHandlers;

        public bool trackingEnabled => m_trackingEnabled;
        public TracksHandler enabledHandlers => m_enabledHandlers;

        private List<IEventTrackerHandler> _handlers;

        public EventTrack BeginEventTrack(EventChannelBase channel, object[] args) {
            EventTrack eventTrack = new EventTrack(this, channel, EventTrackReleased, args);

            if (_handlers != null) {
                foreach (IEventTrackerHandler handle in _handlers)
                    handle.BeginTrack(eventTrack);
            }

            OnBeginEventTrack?.Invoke(eventTrack);
            return eventTrack;
        }

        private void EventTrackReleased(EventTrack track) {
            if (_handlers != null) {
                foreach (IEventTrackerHandler handle in _handlers)
                    handle.EndTrack(track);
            }

            OnEndEventTrack?.Invoke(track);
        }

        public void AddHandler(IEventTrackerHandler handler) {
            _handlers ??= new List<IEventTrackerHandler>();
            _handlers.Add(handler);
        }
    }
}
