using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DayNightSignalReceiver : MonoBehaviour, INotificationReceiver
{
    [SerializeField] private SignalAsset nightBeginSignal;
    [SerializeField] private SignalAsset dayBeginSignal;

    public void OnNotify(Playable _origin, INotification _notification, object _context)
    {
        if (_notification is SignalEmitter emitter)
        {
            if (emitter.asset == nightBeginSignal)
                GameEvents.RaiseNightBegin();
            else if (emitter.asset == dayBeginSignal)
                GameEvents.RaiseDayBegin();
        }
    }
}
