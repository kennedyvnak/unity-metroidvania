using UnityEngine;

namespace Metroidvania.Events {
    [CreateAssetMenu(fileName = "New Void Event Channel", menuName = "Scriptables/Events/Void Event")]
    public class VoidEventChannel : EventChannelBase {
        public EventChannelAction OnEventRaise;

        public void Raise() {
            using (BeginTrack())
                OnEventRaise?.Invoke();
        }
    }
}
