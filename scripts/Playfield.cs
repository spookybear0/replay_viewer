using Godot;
using System;
using System.Collections.Generic;


public partial class Playfield : Node2D {
    public float playfieldWidth;
    public float playfieldHeight;
    public float playfieldLeft;
    public float playfieldTop;

    Sprite2D background;

    public override void _Ready() {
        GetNode<Node2D>("Canvas/PlayfieldBorder").Connect("draw", new Callable(this, "draw"));
        background = GetNode<Sprite2D>("Canvas/Background");
    }

    public override void _EnterTree() {
        // 80% of camera height
        playfieldHeight = OsuConverter.PlayfieldHeight();

        // (4/3) * playfieldHeight
        playfieldWidth = OsuConverter.PlayfieldWidth();

        // center the playfield
        // middle of screen
        // then the playfield y position is offset down by 2% of the playfield height

        playfieldLeft = OsuConverter.PlayfieldLeft();
        playfieldTop = OsuConverter.PlayfieldTop();
    }

    public void draw() {
        // draw the playfield grey, it's a border
        GetNode<Node2D>("Canvas/PlayfieldBorder").DrawRect(new Rect2(playfieldLeft, playfieldTop, playfieldWidth, playfieldHeight), new Color(0.5f, 0.5f, 0.5f), false, 2);
    }

    public void SetBackground(string backgroundPath) {
        background = GetNode<Sprite2D>("Canvas/Background");

        // load from local file system
        Image img = Image.LoadFromFile(backgroundPath);

        background.Texture = ImageTexture.CreateFromImage(img);
    }
}