using Godot;
using System;
using System.Collections.Generic;

public enum GameMode {
    Playing,
    Replay,
}

public partial class GameManager : Node2D {
    public static GameManager _gameManager;

    public static GameMode mode = GameMode.Playing;
    public static List<ulong> eventsHandled = new List<ulong>();

    // change fullscreen on f11
    public override void _Input(InputEvent @event) {
        if (Input.IsActionJustPressed("fullscreen")) {
            DisplayServer.WindowSetMode(DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen ? DisplayServer.WindowMode.Windowed : DisplayServer.WindowMode.Fullscreen);
        }
    }
}