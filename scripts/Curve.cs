using Godot;

public partial class Curve : Node2D {
    private Curve2D curve;

    public override void _Ready() {
        curve = GetNode<Path2D>("Path2D").Curve;
    }
}