using Godot;
using System;
using System.Collections.Generic;

public partial class BetterTrailRenderer : Line2D {
    Cursor cursor;
    Texture2D trailTexture;
    [Export]
    int maxLength = 20;
    List<Sprite2D> trailSprites = new List<Sprite2D>();
    [Export]
    bool enabled = true;

    int currentLength = 0;

    public override void _Ready() {
        cursor = GetNode<Cursor>("../Cursor");
    }

    public override void _Process(double delta) {
        if (cursor == null || !enabled) {
            return;
        }

        Vector2 point = cursor.cursorSprite.GlobalPosition;

        AddPoint(point);

        while (GetPointCount() > maxLength) {
            RemovePoint(0);
        }
    }
}
