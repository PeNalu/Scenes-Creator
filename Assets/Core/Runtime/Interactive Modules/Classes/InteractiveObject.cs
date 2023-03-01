using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onInteract;

    [SerializeField]
    private bool isEnable = false;
    
    public void Interact()
    {
        if (isEnable)
        {
            OnInteract();
            OninteractCallback?.Invoke();
        }
    }

    protected virtual void OnInteract()
    {
        onInteract.Invoke();
    }

    public bool IsEnable()
    {
        return isEnable;
    }

    public void IsEnable(bool value)
    {
        isEnable = value;
    }

    #region [Event Callbacks]
    public event Action OninteractCallback;
    #endregion
}
