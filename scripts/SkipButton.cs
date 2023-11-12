using Godot;
using System;

public partial class SkipButton : TextureButton {
    public override void _Ready() {
        
    }

    public void EnableSkipping(Callable callback) {
        Visible = true;
        Disabled = false;
        Connect("pressed", callback);
    }
}
