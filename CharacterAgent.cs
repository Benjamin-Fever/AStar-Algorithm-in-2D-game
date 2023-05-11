using Godot;
using Godot.Collections;

public partial class CharacterAgent : CharacterBody2D
{
    [Export]
    private States state = States.Idle;
    [Export]
    private Node2D goalPosition;
    [Export]
    private float speed = 1;
    private Vector2 oldGoalPosition;
    private enum States{Idle, Move}
    private Array<Vector2> path = new Array<Vector2>();

    public override void _Process(double delta)
    {
        Velocity = Vector2.Zero;
        if (state == States.Idle) return; // Check state
        if (Position.Equals(goalPosition.Position)) return; // Check if agent is already at goal
        if (!AgentPathFinding.HasPath(Position, goalPosition.Position)) return; // Check if there a path between to positions
        
        if (path.Count == 0 || oldGoalPosition != goalPosition.Position){ // Check if the path is empty or change in goal position
            path = AgentPathFinding.FindAgentPath(Position, goalPosition.Position); // Get a list of positions to traverse
            oldGoalPosition = goalPosition.Position;
        }

        if (path[0].DistanceTo(Position) <= 1){ // Check's if the desired node has been achived
            Position = path[0];
            path.Remove(path[0]);
            return;
        }

        Vector2 currentNode = path[0];
        Vector2 direction = Position.DirectionTo(currentNode).Normalized();
        Velocity = direction * speed;
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndCollide(Velocity * (float)delta);
    }
}
