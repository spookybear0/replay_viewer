using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class BeatmapManager : Node2D {
    OsuParsers.Beatmaps.Beatmap beatmap;
    string beatmapFolder;
    double songOffset = 0;

    OsuParsers.Replays.Replay replay;

    PackedScene hitCircleScene;
    PackedScene hitSliderScene;
    AudioStreamPlayer audioStreamPlayer;
    Cursor cursor;
    
    int hitObjectIndex = 0;
    double totalDeltaTime = 0;
    int currentCombo = 1;
    bool skipped = false;
    OsuParsers.Beatmaps.Objects.TimingPoint currentTimingPoint;

    int replayFrameIndex = 0;

    public override void _Ready() {
        hitCircleScene = ResourceLoader.Load<PackedScene>("res://objects/hit_circle.tscn");
        hitSliderScene = ResourceLoader.Load<PackedScene>("res://objects/hit_slider.tscn");
        audioStreamPlayer = GetNode<AudioStreamPlayer>("Player");
        cursor = GetNode<Cursor>("/root/Scene/Playfield/Cursor");

        PlayBeatmap("D:/games/osu!/Songs/833927 Weird Al Yankovic - Hardware Store/Weird Al Yankovic - Hardware Store (Mr Moseby) [Matching Salt And Pepper Shakers].osu");
        PlayReplay("D:/games/osu!/Replays/BlackDog5 - Weird Al Yankovic - Hardware Store [Matching Salt And Pepper Shakers] (2020-10-11) Osu.osr");
    }

    public void PlayBeatmap(string beatmapPath) {
        beatmapFolder = Path.GetDirectoryName(beatmapPath).Replace("\\", "/");

        beatmap = OsuParsers.Decoders.BeatmapDecoder.Decode(beatmapPath);

        currentTimingPoint = beatmap.TimingPoints[0];

        songOffset = beatmap.GeneralSection.AudioLeadIn / 1000;

        if (songOffset < 0) {
            Timer t = new Timer();
            t.WaitTime = (float)-songOffset;
            t.OneShot = true;
            t.Connect("timeout", new Callable(this, nameof(StartBeatmapAudio)));
            AddChild(t);
            t.Start();
        }
        else {
            StartBeatmapAudio();
            audioStreamPlayer.Seek((float)songOffset);
            totalDeltaTime = songOffset;
        }

        // check if skip button is needed
        // if songOffset + the first circle's time is less than 5000ms, then skip button is not needed

        if (songOffset + beatmap.HitObjects[0].StartTime >= 5000) {
            SkipButton skipButton = GetNode<SkipButton>("/root/Scene/Playfield/Canvas/SkipButton");
            skipButton.EnableSkipping(new Callable(this, nameof(Skipped)));
        }

        // background image

        //GetNode<Playfield>("/root/Scene/Playfield").SetBackground(beatmapFolder + "/" + beatmap.EventsSection.BackgroundImage);
    }

    public void PlayReplay(string replayPath) {
        replay = OsuParsers.Decoders.ReplayDecoder.Decode(replayPath);
        GameManager.mode = GameMode.Replay;
    }

    public void Skipped() {
        SkipButton button = GetNode<SkipButton>("/root/Scene/Playfield/Canvas/SkipButton");
        
        button.Visible = false;
        button.Disabled = true;

        totalDeltaTime = (beatmap.HitObjects[0].StartTime / 1000) - 5;
        audioStreamPlayer.Seek((float)totalDeltaTime);

        skipped = true;
    }

    public void StartBeatmapAudio() {
        audioStreamPlayer.Stream = LoadMP3(beatmapFolder + "/" + beatmap.GeneralSection.AudioFilename);
        audioStreamPlayer.Play();
    }    

    public AudioStreamMP3 LoadMP3(string path) {
        Godot.FileAccess file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
        var sound = new AudioStreamMP3();
        sound.Data = file.GetBuffer((long)file.GetLength());
        return sound;
    }

    public override void _Process(double delta) {
        double time = audioStreamPlayer.GetPlaybackPosition() + AudioServer.GetTimeSinceLastMix();
        // compensate for output latency.
        time -= AudioServer.GetOutputLatency();

        ProcessBeatmap(delta);
        ProcessReplay(delta);
    }

    public void ProcessReplay(double delta) {
        if (replay == null) {
            return;
        }

        OsuParsers.Replays.Objects.ReplayFrame frame = replay.ReplayFrames[replayFrameIndex];

        if (replayFrameIndex >= replay.ReplayFrames.Count) {
            return;
        }

        if (frame.Time <= totalDeltaTime * 1000) {
            cursor.Position = OsuConverter.OsuPixelToGodotPixel(frame.X, frame.Y);

            InputEventAction action = new InputEventAction();
            action.Pressed = true;
            if (frame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.K1)) {
                action.Action = "key_1";
                Input.ParseInputEvent(action);
            }
            else if (frame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.K2)) {
                action.Action = "key_2";
                Input.ParseInputEvent(action);
            }
            else if (frame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.M1)) {
                action.Action = "mouse_1";
                Input.ParseInputEvent(action);
            }
            else if (frame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.M2)) {
                action.Action = "mouse_2";
                Input.ParseInputEvent(action);
            }
            else if (frame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.Smoke)) {
                //Input.ActionPress("key_smoke");
            }

            replayFrameIndex++;
        }
    }

    public void ProcessBeatmap(double delta) {
        if (beatmap == null) {
            return;
        }

        totalDeltaTime += delta;
        if (hitObjectIndex >= beatmap.HitObjects.Count) {
            return;
        }

        if (!skipped && (totalDeltaTime > ((beatmap.HitObjects[0].StartTime / 1000)))) {
            SkipButton button = GetNode<SkipButton>("/root/Scene/Playfield/Canvas/SkipButton");
            
            button.Visible = false;
            button.Disabled = true;
            skipped = true;
        }

        // get the current sample set
        

        if (currentTimingPoint.Offset <= totalDeltaTime * 1000) {
            currentTimingPoint = beatmap.TimingPoints[beatmap.TimingPoints.IndexOf(currentTimingPoint) + 1];
        }

        if (currentTimingPoint.SampleSet == OsuParsers.Enums.Beatmaps.SampleSet.None) {
            currentTimingPoint.SampleSet = beatmap.GeneralSection.SampleSet;
        }

        OsuParsers.Beatmaps.Objects.HitObject obj = beatmap.HitObjects[hitObjectIndex];

        if (obj is OsuParsers.Beatmaps.Objects.HitCircle) {
            OsuParsers.Beatmaps.Objects.HitCircle circle = (OsuParsers.Beatmaps.Objects.HitCircle)obj;
            // check if circle is able to be shown

            double preemptTime = OsuConverter.ARToPreemptTime(beatmap.DifficultySection.ApproachRate);

            if (totalDeltaTime*1000 <= circle.StartTime - preemptTime) {
                return;
            }

            if (circle.IsNewCombo) {
                currentCombo = 1;
            }

            HitCircle hitCircle = hitCircleScene.Instantiate<HitCircle>();

            hitCircle.setCS(beatmap.DifficultySection.CircleSize, GetNode<Playfield>("../Playfield"));
            hitCircle.setHitsoundSettings(circle.HitSound, currentTimingPoint.SampleSet, currentTimingPoint.Volume);
            hitCircle.Position = OsuConverter.OsuPixelToGodotPixel(circle.Position);
            hitCircle.setComboNumber(currentCombo);

            GetNode<Node2D>("../").CallDeferred("add_child", hitCircle);

            hitCircle.fadeIn(beatmap.DifficultySection.ApproachRate);

            currentCombo++;
        }
        else if (obj is OsuParsers.Beatmaps.Objects.Slider) {
            OsuParsers.Beatmaps.Objects.Slider slider = (OsuParsers.Beatmaps.Objects.Slider)obj;

            double preemptTime = OsuConverter.ARToPreemptTime(beatmap.DifficultySection.ApproachRate);

            if (totalDeltaTime*1000 <= slider.StartTime - preemptTime) {
                return;
            }

            if (slider.IsNewCombo) {
                currentCombo = 1;
            }

            HitSlider hitSlider = hitSliderScene.Instantiate<HitSlider>();

            hitSlider.circle = hitSlider.GetNode<HitCircle>("HitCircle");
            hitSlider.line2d = hitSlider.GetNode<Line2D>("Line2D");
            
            hitSlider.circle.isSlider = true;

            hitSlider.circle.setCS(beatmap.DifficultySection.CircleSize, GetNode<Playfield>("../Playfield"));
            hitSlider.circle.setHitsoundSettings(slider.HitSound, currentTimingPoint.SampleSet, currentTimingPoint.Volume);
            hitSlider.Position = OsuConverter.OsuPixelToGodotPixel(slider.Position.X, slider.Position.Y);
            hitSlider.circle.setComboNumber(currentCombo);

            hitSlider.line2d.AddPoint(new Vector2(0, 0));
            hitSlider.curveType = slider.CurveType;

            foreach (System.Numerics.Vector2 point in slider.SliderPoints) {
                Vector2 vec = OsuConverter.OsuPixelToGodotPixel(point.X, point.Y) - hitSlider.Position;
                hitSlider.points.Add(vec);
                hitSlider.line2d.AddPoint(vec);
            }

            GetNode<Node2D>("../").CallDeferred("add_child", hitSlider);
            hitSlider.circle.fadeIn(beatmap.DifficultySection.ApproachRate);

            currentCombo++;
        }
        hitObjectIndex++;
    }
}
