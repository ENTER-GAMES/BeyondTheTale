using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CompletionPoint : Point<ICompletable>
{
    [SerializeField]
    private UnityEvent onComplete = new UnityEvent();

    protected override void Hit(ICompletable target)
    {
        target.Complete();
        onComplete.Invoke();
    }
}

public interface ICompletable : IAbleForPoint
{
    public void Complete();
}