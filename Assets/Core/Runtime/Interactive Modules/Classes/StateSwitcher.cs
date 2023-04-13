using UnityEngine;
using UnityEngine.Events;

public class StateSwitcher : MonoBehaviour
{
    [SerializeField]
    private UnityEvent fState;

    [SerializeField]
    private UnityEvent sState;

    //Stored required properties.
    private bool flag = true;

    private void Start()
    {
        fState.Invoke();
        flag = false;
    }

    public void SwitchState()
    {
        if (!flag)
        {
            sState.Invoke();
            flag = true;
        }
        else
        {
            fState.Invoke();
            flag = false;
        }
    }
}
