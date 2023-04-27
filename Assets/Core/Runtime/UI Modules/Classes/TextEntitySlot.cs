using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TextEntitySlot : MonoBehaviour
{
    [SerializeField]
    private GeneratorSettings settings;

    [SerializeField]
    private Text entityNameField;

    [SerializeField]
    private Dropdown posDropdown;

    [SerializeField]
    private Dropdown parentDropdown;

    [SerializeField]
    private Toggle interactToggle;

    [SerializeField]
    private Toggle grabbableToggle;

    //Stored required properties.
    private TextEntity textEntity;

    public void Initialize(TextEntity textEntity, List<TextEntity> textEntities)
    {
        this.textEntity = textEntity;
        entityNameField.text = textEntity.name;

        posDropdown.ClearOptions();
        posDropdown.AddOptions(settings.positions);
        posDropdown.value = settings.positions.IndexOf(textEntity.position);
        posDropdown.onValueChanged.AddListener((index) =>
        {
            textEntity.position = settings.positions[index];
        });

        List<string> parents = textEntities.Select(x => x.name).Where(x => x != textEntity.name).ToList();
        parentDropdown.ClearOptions();
        parentDropdown.AddOptions(parents);
        parentDropdown.value = parents.IndexOf(textEntity.parentObj);
        parentDropdown.onValueChanged.AddListener((index) =>
        {
            textEntity.parentObj = parents[index];
        });

        interactToggle.isOn = textEntity.interactable;
        interactToggle.onValueChanged.AddListener((b) =>
        {
            textEntity.interactable = b;
        });
        
        grabbableToggle.isOn = textEntity.grabbable;
        grabbableToggle.onValueChanged.AddListener((b) =>
        {
            textEntity.grabbable = b;
        });
    }
}
