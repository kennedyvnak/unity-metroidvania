using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Metroidvania.Events.Tracker
{
    // TODO: Add editor UI and add an text file in runtime to handle events tacking
    public class EventsTracker : ScriptableSingleton<EventsTracker>
    {
        public struct EventTrack
        {
            public EventChannelBase channel;

            public double invokeTime;

            public object[] invokeParams;
        }

        public System.Action<EventTrack> EventRaised;
        public bool trackingEnabled;

        private void OnEnable()
        {
            EventRaised += OnEventRaise;
        }

        private void OnDisable()
        {
            EventRaised -= OnEventRaise;
        }

        private void OnEventRaise(EventTrack track)
        {
            StringBuilder argsSb = new StringBuilder();
            for (int i = 0; i < track.invokeParams.Length; i++)
            {
                object invokeParam = track.invokeParams[i];
                argsSb.AppendFormat("\n  - Arg {0}: {1} ({2})", i, invokeParam, invokeParam.GetType());
            }

            Debug.Log($"The channel {track.channel.name} ({track.channel.GetType().Name}) has raised an event at ({track.invokeTime}). {argsSb}");
        }

        private List<EventChannelBase> _trackingChannels = new List<EventChannelBase>();

        public void TrackEvent(EventChannelBase channel, object[] args)
        {
            if (!trackingEnabled) return;

            var stackTrace = new EventTrack() { channel = channel, invokeTime = Time.realtimeSinceStartupAsDouble, invokeParams = args };

            EventRaised?.Invoke(stackTrace);
        }
    }
}
