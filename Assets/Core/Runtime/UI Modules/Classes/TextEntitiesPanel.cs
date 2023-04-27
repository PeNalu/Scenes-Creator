using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TextEntitiesPanel : MonoBehaviour
{
    [SerializeField]
    private GeneratorSettings settings;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private TextEntitySlot slotTemplate;

    [SerializeField]
    private Transform container;

    //Stored required properties.
    private Vector3 pPositions;
    private List<TextEntity> textEntities;
    private List<TextEntitySlot> textEntitySlots;

    private void Awake()
    {
        textEntitySlots = new List<TextEntitySlot>();
    }

    public void Initialize(List<TextEntity> textEntities)
    {
        this.textEntities = textEntities;

        foreach (TextEntity entity in textEntities)
        {
            if (!settings.stopEntities.Contains(entity.name) && !settings.animationStates.Contains(entity.name))
            {
                TextEntitySlot textEntitySlot = Instantiate(slotTemplate, container);
                textEntitySlot.Initialize(entity, textEntities);
                textEntitySlots.Add(textEntitySlot);
            }
        }

        Show();
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;

        for (int i = 0; i < textEntitySlots.Count; i++)
        {
            Destroy(textEntitySlots[i].gameObject);
        }
        textEntitySlots.Clear();
    }

    public bool IsActive()
    {
        return canvasGroup.interactable;
    }
}
