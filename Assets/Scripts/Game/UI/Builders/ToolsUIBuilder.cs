using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ToolsUIBuilder : MonoBehaviour
{
    private const string PREFAB_PATH = "UI/ToolCell";

    public static Dictionary<EntityType, Button> Build(EntityType[] entities, Transform parent)
    {
        if (parent == null || entities.Length == 0)
        {
            Debug.LogError("Invalid parent or entities");
            return null;
        }


        var toolPrefab = Resources.Load<GameObject>(PREFAB_PATH);

        if(toolPrefab == null)
        {
            Debug.LogError("Tool prefab is not found: " + PREFAB_PATH);
            return null;
        }

        var result = new Dictionary<EntityType, Button>();

        for (int i = 0; i < entities.Length; i++)
        {
            Button tool = Object.Instantiate(toolPrefab, parent).GetComponent<Button>();
            tool.transform.name = entities[i].ToString();
            tool.transform.GetChild(0).GetComponent<Image>().sprite = EntitiesCollection.GetEntityUISprite(entities[i]);
            result.Add(entities[i], tool);
        }

        return result;
    }

}
