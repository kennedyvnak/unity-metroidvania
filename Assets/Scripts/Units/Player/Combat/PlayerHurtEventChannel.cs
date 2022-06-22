using Metroidvania.Entities;
using Metroidvania.Events;
using UnityEngine;

namespace Metroidvania.Player
{
    [CreateAssetMenu(menuName = "Scriptables/Player/Hurt Event")]
    public class PlayerHurtEventChannel : EventChannelBase<PlayerController, EntityHitData>
    {
    }
}