using System;

public interface IInteractionState
{
    event Action<InteractionAnimType> OnInteractionStarted;
    event Action                      OnInteractionEnded;
    event Action                      OnInteractionDenied;
}
