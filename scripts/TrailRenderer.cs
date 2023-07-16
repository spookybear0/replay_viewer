using Godot;
using System;
using System.Collections.Generic;

public partial class TrailRenderer : Node2D {
    Cursor cursor;
    Texture2D trailTexture;
    int maxLength = 10;
    List<Sprite2D> trailSprites = new List<Sprite2D>();
    [Export]
    bool enabled = true;

    int currentLength = 0;

    public override void _Ready() {
        cursor = GetNode<Cursor>("../Cursor");
        trailTexture = ResourceLoader.Load<Texture2D>("res://assets/cursortrail.png");
    }

    public override void _Process(double delta) {
        if (cursor == null || !enabled) {
            return;
        }

        if (currentLength >= maxLength) {
            Sprite2D lastSprite = trailSprites[0];
            lastSprite.QueueFree();
            trailSprites.RemoveAt(0);
            currentLength--;
        }

        // get the cursor's position then add a new sprite to that position

        Vector2 cursorPosition = cursor.cursorSprite.GlobalPosition;

        Sprite2D trailSprite = new Sprite2D();

        trailSprite.Texture = trailTexture;
        trailSprite.GlobalPosition = cursorPosition;
        trailSprite.ZIndex = 3;
        
        AddChild(trailSprite);
        currentLength++;

        trailSprites.Add(trailSprite);
    }
}
