using Godot;
using System;

public partial class SkipButton : TextureButton {
    public override void _Ready() {
        
    }

    public override void _Process(double delta) {

    }

    public void EnableSkipping(Callable callback) {
        Visible = true;
        Disabled = false;
        Connect("pressed", callback);
    }
}
