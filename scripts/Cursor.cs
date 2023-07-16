using Godot;
using System;

public partial class Cursor : Node2D {
    public Sprite2D cursorSprite;
    public Area2D cursorArea;

    public override void _Ready() {
        cursorSprite = GetNode<Sprite2D>("CursorSprite");
        cursorArea = GetNode<Area2D>("CursorArea");
    }

    public override void _Process(double delta) {
        if (GameManager.mode == GameMode.Playing) {
            // if the game is playing
            // have the mouse control the cursor
            Vector2 mousePosition = GetGlobalMousePosition();

            cursorSprite.Position = mousePosition;
        }
    }
}