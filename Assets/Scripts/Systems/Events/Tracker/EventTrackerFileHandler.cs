using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Metroidvania.Events.Tracker.Handles {
    public class EventTrackerFileHandler : IEventTrackerHandler, IDisposable {
        public const string LogFolder = "EventTracks";
        public const string FileName = "EventTrack";
        public const string Extension = "log";

        private string currentFilePath { get; set; }

        private StringBuilder trackTextBuilder { get; set; }
        private StringBuilder trackArgsTextBuilder { get; set; }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeHandler() {
            if (!EventsTracker.instance.trackingEnabled
                || !EventsTracker.instance.enabledHandlers.HasFlag(EventsTracker.TracksHandler.File))
                return;

            EventsTracker.instance.AddHandler(new EventTrackerFileHandler());
        }

        public EventTrackerFileHandler() {
            currentFilePath = GetUniquePath();

            string rootDirectory = Path.GetDirectoryName(currentFilePath);
            if (!Directory.Exists(rootDirectory))
                Directory.CreateDirectory(rootDirectory);

            trackTextBuilder = new StringBuilder();
            trackArgsTextBuilder = new StringBuilder();

            WriteText($"[{System.DateTime.Now.TimeOfDay}] Initializing events tracking\n\n");
            Application.quitting += Dispose;
        }

        public void Dispose() {
            WriteText($"\n[{System.DateTime.Now.TimeOfDay}] Disposing events tracking");
        }

        public void BeginTrack(EventsTracker.EventTrack track) {
        }

        public void EndTrack(EventsTracker.EventTrack track) {
            LogTrackToFile(track);
        }

        private void LogTrackToFile(EventsTracker.EventTrack track) {
            trackTextBuilder.Clear();
            trackArgsTextBuilder.Clear();

            for (int i = 0; i < track.invokeParams.Length; i++) {
                object invokeParam = track.invokeParams[i];
                trackArgsTextBuilder.AppendFormat("\n  Arg[{0}]: {1} ({2})", i, invokeParam, invokeParam.GetType());
            }

            trackTextBuilder.AppendFormat("[{0} ({1})] The invocation lasted from {2} to {3} ({4:0}ms) {5}\n",
                track.channel.name, track.channel.GetType().Name, track.invokeTime, track.releaseTime, (track.releaseTime - track.invokeTime) * 1e+8, trackArgsTextBuilder);

            WriteText(trackTextBuilder.ToString());
        }

        private void WriteText(string text) {
            StreamWriter writer = File.AppendText(currentFilePath);
            writer.Write(text);
            writer.Close();
        }

        private string GetUniquePath() {
            int id = 0;
            string path = GetFilePathById(id);

            while (File.Exists(path))
                path = GetFilePathById(id++);

            return path;
        }

        private static string GetFilePathById(int id) => $"{GetLogsFolder()}/{FileName}-{id}.{Extension}";

        private static string GetLogsFolder() => Path.Combine(Application.persistentDataPath, LogFolder);
    }
}