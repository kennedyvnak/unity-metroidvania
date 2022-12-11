namespace Metroidvania.Events.Tracker.Handles {
    public interface IEventTrackerHandler {
        void BeginTrack(EventsTracker.EventTrack track);
        void EndTrack(EventsTracker.EventTrack track);
    }
}
