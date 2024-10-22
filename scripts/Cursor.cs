using Godot;
using System;

public partial class Cursor : Path2D {
    public Sprite2D cursorSprite;
    public Area2D cursorArea;
    public PathFollow2D pathFollow2D;
    public AnimationPlayer animationPlayer;

    public override void _Ready() {
        cursorSprite = GetNode<Sprite2D>("PathFollow2D/CursorSprite");
        cursorArea = GetNode<Area2D>("PathFollow2D/CursorArea");
        pathFollow2D = GetNode<PathFollow2D>("PathFollow2D");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Process(double delta) {
        if (GameManager.mode == GameMode.Playing) {
            // if the game is playing
            // have the mouse control the cursor
            Vector2 mousePosition = GetGlobalMousePosition();

            Position = mousePosition;
        }
    }
}