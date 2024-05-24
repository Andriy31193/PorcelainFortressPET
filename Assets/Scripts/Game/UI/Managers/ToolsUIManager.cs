using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public sealed class ToolsUIManager : MonoBehaviour
{
    public EntityType CurrentTool { get; private set; } = EntityType.Void;

    public static event Action<EntityType> OnCurrentToolSwitched;
    private static readonly EntityType[] TOOLS =
    {
        EntityType.DirectionLeft,
        EntityType.DirectionRight,
        EntityType.Void
    };

    [SerializeField] private HorizontalLayoutGroup _parent;

    private Dictionary<EntityType, Button> _tools;


    private void Awake() => DIContainer.Register(this);
    private void Start() => Populate();


    private void Populate()
    {
        _tools = ToolsUIBuilder.Build(TOOLS, parent: _parent.transform);

        SetupTools();
    }
    private void OnToolClick(EntityType toolType)
    {
        CurrentTool = toolType;

        OnCurrentToolSwitched?.Invoke(CurrentTool);
    }
    private void SetupTools()
    {
        for (int i = 0; i < _tools.Count; i++)
        {
            var tool = _tools.ElementAt(i);
            tool.Value.onClick.AddListener(() => OnToolClick(tool.Key));
        }
    }
}
