using Godot;
using System;
using System.Collections.Generic;

public partial class HitSpinner : Node2D {
    AnimationPlayer animationPlayer;
    BeatmapManager beatmap_mgr;
    GameManager game_mgr;
    Cursor cursor;
    double fadeInTime;
    bool fadingOut = false;

    public double endTime;

    public OsuParsers.Enums.Beatmaps.HitSoundType hitsound;
    public OsuParsers.Enums.Beatmaps.SampleSet sampleSet;
    public float volume;

    private float rotationSpeed = 0.0f;

    public override void _Ready() {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        beatmap_mgr = GetNode<BeatmapManager>("../BeatmapManager");
        cursor = GetNode<Cursor>("../Playfield/Cursor");

        animationPlayer.Connect("animation_finished", new Callable(this, "AnimationFinished"));

        Position = OsuConverter.OsuPixelToGodotPixel(256, 192);
    }

    public override void _Process(double delta) {
        // fade out

        if (beatmap_mgr.totalDeltaTime*1000 >= endTime && !fadingOut) {
            // output
            animationPlayer.Play("fadeout");
            fadingOut = true;
        }

        // rotation

        if (
            (beatmap_mgr.replayFrame != null && ((beatmap_mgr.replayFrame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.K1) ||
            beatmap_mgr.replayFrame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.K2) ||
            beatmap_mgr.replayFrame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.M1) ||
            beatmap_mgr.replayFrame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.M2)) && GameManager.mode == GameMode.Replay)) ||
            // input
            ((Input.IsActionPressed("key_1") ||
            Input.IsActionPressed("key_2") ||
            Input.IsActionPressed("mouse_1") ||
            Input.IsActionPressed("mouse_2")) && GameManager.mode == GameMode.Playing)
        ) {
            // Get the mouse position
            Vector2 cursorPosition = cursor.GlobalPosition;

            Vector2 direction = cursorPosition - GlobalPosition;

            // calculate the angle of rotation
            float rotation = Mathf.Atan2(direction.Y, direction.X);

            Rotation = rotation;
        } else {
            // reset spinner
            Rotation += rotationSpeed * (float)delta;
        }
    }

	public void fadeIn(float ar) {
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		fadeInTime = OsuConverter.ARToPreemptTime(ar);

		animationPlayer.SpeedScale = 1 / ((float)fadeInTime / 1000);

		animationPlayer.Play("fadein");
	}

	public void AnimationFinished(string name) {
		if (name == "fadeout") {
			QueueFree();
		}

        if (name == "fadein") {

            // approach circle

            // animation needs to finish at endTime
            animationPlayer.SpeedScale = 1 / (((float)(endTime - beatmap_mgr.totalDeltaTime*1000) / 1000) - (((float)fadeInTime / 1000)));
            animationPlayer.Play("approach");
        }

        if (name == "approach") {
            animationPlayer.SpeedScale = (1 / ((float)fadeInTime / 1000)) * 3;
            animationPlayer.Play("fadeout");
            fadingOut = true;
        }
	}
}