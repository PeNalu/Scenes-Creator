using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeneratorPanel : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private RoomDataSlot roomTemplate;

    [SerializeField]
    private Transform roomContainer;

    [SerializeField]
    private TextEntitiesPanel textEntitiesPanel;

    [SerializeField]
    private ApartmentGenerator apartmentGenerator;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private UnityEvent onEnable;

    //Stored required properties.
    private Vector3 pPositions;
    private List<RoomDataSlot> roomDataSlots;

    private void Awake()
    {
        pPositions = player.position;
        roomDataSlots = new List<RoomDataSlot>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (canvasGroup.interactable || textEntitiesPanel.IsActive())
            {
                Hide();
                textEntitiesPanel.Hide();
            }
            else if(!canvasGroup.interactable)
            {
                Show();
            }
        }
    }

    public void CreateUI()
    {
        foreach (RoomData item in apartmentGenerator.GetRoomDatas())
        {
            RoomDataSlot roomDataSlot = Instantiate(roomTemplate, roomContainer);
            roomDataSlot.Initialize(item);
            roomDataSlot.OnClick += OnClick;
            roomDataSlots.Add(roomDataSlot);
        }
    }

    private void OnClick(RoomData roomData)
    {
        canvasGroup.alpha = 0;
        textEntitiesPanel.Initialize(roomData.textEntities);
        Hide();
    }

    public void Show()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        onEnable?.Invoke();
        player.position = pPositions;
        CreateUI();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;

        for (int i = 0; i < roomDataSlots.Count; i++)
        {
            Destroy(roomDataSlots[i].gameObject);
        }
        roomDataSlots.Clear();
    }
}
