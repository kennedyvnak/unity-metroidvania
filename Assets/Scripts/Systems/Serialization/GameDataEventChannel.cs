using Metroidvania.Events;
using UnityEngine;

namespace Metroidvania.Serialization {
    [CreateAssetMenu(menuName = "Scriptables/Serialization/Game Data Event")]
    public class GameDataEventChannel : EventChannelBase<GameData> {
    }
}