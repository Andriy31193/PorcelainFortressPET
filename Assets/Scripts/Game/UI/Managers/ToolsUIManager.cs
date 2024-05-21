using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public sealed class ToolsUIManager : MonoBehaviour
{
    public EntityType GetCurrentTool => _currentTool;
    private static readonly EntityType[] TOOLS =
    {
        EntityType.DirectionLeft,
        EntityType.DirectionRight,
        EntityType.Void
    };

    [SerializeField] private HorizontalLayoutGroup _parent;

    private Dictionary<EntityType, Button> _tools;
    private EntitiesCollection _entitiesCollection;
    private EntityType _currentTool = EntityType.Void;



    private void Start()
    {
        _entitiesCollection = DIContainer.Resolve<EntitiesCollection>();

        Populate();
    }

    private void Populate()
    {
        var forbiddenTools = Enum.GetValues(typeof(EntityType))
                                .Cast<EntityType>()
                                .Except(TOOLS)
                                .ToArray();

        var entities = _entitiesCollection.GetAll(ignore: forbiddenTools).Select(x => x.Type).ToArray();

        _tools = ToolsUIBuilder.Build(entities, parent: _parent.transform);

        ProcessTools();
    }
    private void OnToolClick(EntityType toolType)
    {
        _currentTool = toolType;
    }
    private void ProcessTools()
    {
        for (int i = 0; i < _tools.Count; i++)
        {
            var tool = _tools.ElementAt(i);
            tool.Value.onClick.AddListener(() => OnToolClick(tool.Key));
        }
    }
}
