using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomDataSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Text textField;

    //Stored required properties.
    private RoomData roomData;

    public void Initialize(RoomData roomData)
    {
        this.roomData = roomData;
        textField.text = roomData.roomName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(roomData);
        print(roomData.roomName);
    }

    #region [Event Callback Functions]
    public event Action<RoomData> OnClick;
    #endregion
}
