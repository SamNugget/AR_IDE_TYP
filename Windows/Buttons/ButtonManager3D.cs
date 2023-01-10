using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

public abstract class ButtonManager3D : MonoBehaviour
{
    [SerializeField] private GameObject buttonFab;
    // https://learn.microsoft.com/en-us/dotnet/api/microsoft.mixedreality.toolkit.utilities.gridobjectcollection?view=mixed-reality-toolkit-unity-2020-dotnet-2.8.0
    private GridObjectCollection gridObjectCollection;



    public abstract void createButtons();

    protected void spawnButton(string buttonLabel, char action, object data, string iconName = null)
    {
        GameObject spawned = Instantiate(buttonFab, gridObjectCollection.transform);

        ActionButton aB = spawned.GetComponent<ActionButton>();
        if (aB == null) Debug.LogError("Err no action button component");
        aB.setLabel(buttonLabel);
        aB.setAction(action);
        aB.setData(data);
        if (iconName != null) aB.setIcon(iconName);
    }

    protected void deleteButton(string buttonLabel)
    {
        foreach (Transform child in gridObjectCollection.transform)
        {
            if (child.GetComponent<ActionButton>().getLabel() == buttonLabel)
            {
                Destroy(child.gameObject);
                return;
            }
        }
        Debug.Log("No button by the name of " + buttonLabel + " was found");
    }

    public void Start()
    {
        gridObjectCollection = GetComponentInChildren<GridObjectCollection>();
        if (gridObjectCollection == null) Debug.LogError("Err no grid object collection component");
        
        createButtons();
        gridObjectCollection.UpdateCollection();
    }
}
