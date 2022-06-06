using System.Collections.Generic;
using Metroidvania.Events.Tracker.Handles;
using UnityEngine;
using System;

namespace Metroidvania.Events.Tracker
{
    // TODO: Add editor UI and add an text file in runtime to handle events tacking
    public class EventsTracker : ScriptableSingleton<EventsTracker>
    {
        [System.Flags]
        public enum TracksHandler
        {
            File = 1 << 0,
        }

        public class EventTrack : IDisposable
        {
            public readonly EventsTracker tracker;
            public readonly EventChannelBase channel;

            public System.Action<EventTrack> trackReleased;

            public double invokeTime, releaseTime;

            public object[] invokeParams;

            public EventTrack(EventsTracker tracker, EventChannelBase channel, System.Action<EventTrack> trackReleased, object[] invokeParams)
            {
                this.tracker = tracker;
                this.channel = channel;
                this.invokeParams = invokeParams;
                this.trackReleased = trackReleased;
                this.invokeTime = Time.realtimeSinceStartupAsDouble;
                this.releaseTime = -1;
            }

            public void Dispose()
            {
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

        public EventTrack BeginEventTrack(EventChannelBase channel, object[] args)
        {
            var eventTrack = new EventTrack(this, channel, EventTrackReleased, args);

            if (_handlers != null)
            {
                foreach (var handle in _handlers)
                    handle.BeginTrack(eventTrack);
            }

            OnBeginEventTrack?.Invoke(eventTrack);
            return eventTrack;
        }

        private void EventTrackReleased(EventTrack track)
        {
            if (_handlers != null)
            {
                foreach (var handle in _handlers)
                    handle.EndTrack(track);
            }

            OnEndEventTrack?.Invoke(track);
        }

        public void AddHandler(IEventTrackerHandler handler)
        {
            if (_handlers == null)
                _handlers = new List<IEventTrackerHandler>();
            _handlers.Add(handler);
        }
    }
}
