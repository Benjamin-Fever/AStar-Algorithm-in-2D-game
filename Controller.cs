using Godot;
using System;

public partial class Controller : Node2D
{
    [Export]
    public TileMap tileMap;
    [Export]
    public int layer;
    public override void _Ready()
    {
        AgentPathFinding.Setup(tileMap, layer);
    }
}
