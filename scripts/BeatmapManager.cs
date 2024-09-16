using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class BeatmapManager : Node2D {
    public OsuParsers.Beatmaps.Beatmap beatmap;


    string beatmapFolder;
    double songOffset = 0;

    OsuParsers.Replays.Replay replay;

    PackedScene hitCircleScene;
    PackedScene hitSliderScene;
    PackedScene hitSpinnerScene;
    AudioStreamPlayer audioStreamPlayer;
    Cursor cursor;
    
    int hitObjectIndex = 0;
    public double totalDeltaTime = 0;
    int currentCombo = 1;
    bool skipped = false;
    public OsuParsers.Beatmaps.Objects.TimingPoint currentTimingPoint;
    public Node2D currentHitObject;

    int replayFrameIndex = 0;


    public OsuParsers.Replays.Objects.ReplayFrame replayFrame;

    public override void _Ready() {
        hitCircleScene = ResourceLoader.Load<PackedScene>("res://objects/hit_circle.tscn");
        hitSliderScene = ResourceLoader.Load<PackedScene>("res://objects/hit_slider.tscn");
        hitSpinnerScene = ResourceLoader.Load<PackedScene>("res://objects/hit_spinner.tscn");
        audioStreamPlayer = GetNode<AudioStreamPlayer>("Player");
        cursor = GetNode<Cursor>("/root/Scene/Playfield/Cursor");

        //Engine.TimeScale = 0.25f;

        PlayBeatmap("D:/games/osu!/Songs/813569 Laur - Sound Chimera/Laur - Sound Chimera (Nattu) [Chimera].osu");
        GameManager.mode = GameMode.Auto;
        //PlayReplay("D:/games/osu!/Replays/BlackDog5 - Laur - Sound Chimera [Chimera] (2021-03-19) Osu.osr");
        //PlayBeatmap("D:/games/osu!/Songs/437683 Halozy - Kikoku Doukoku Jigokuraku/Halozy - Kikoku Doukoku Jigokuraku (Hollow Wings) [Notch Hell].osu");
        //PlayReplay("D:/games/osu!/Replays/Xilver15 - Halozy - Kikoku Doukoku Jigokuraku [Notch Hell] (2019-07-26) Osu.osr");
        //PlayBeatmap("D:/games/osu!/Songs/41823 The Quick Brown Fox - The Big Black/The Quick Brown Fox - The Big Black (Blue Dragon) [WHO'S AFRAID OF THE BIG BLACK].osu");
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

        GetNode<Playfield>("/root/Scene/Playfield").SetBackground(beatmapFolder + "/" + beatmap.EventsSection.BackgroundImage, 0.8f);


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
        if (replay == null || replayFrameIndex >= replay.ReplayFrames.Count) {
            return;
        }

        replayFrame = replay.ReplayFrames[replayFrameIndex];

        if (replayFrameIndex >= replay.ReplayFrames.Count) {
            return;
        }

        if (replayFrame.Time <= totalDeltaTime * 1000) {
            cursor.Position = OsuConverter.OsuPixelToGodotPixel(replayFrame.X, replayFrame.Y);

            InputEventAction action = new InputEventAction();
            action.Pressed = true;
            if (replayFrame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.K1)) {
                action.Action = "key_1";
                Input.ParseInputEvent(action);
            }
            else if (replayFrame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.K2)) {
                action.Action = "key_2";
                Input.ParseInputEvent(action);
            }
            else if (replayFrame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.M1)) {
                action.Action = "mouse_1";
                Input.ParseInputEvent(action);
            }
            else if (replayFrame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.M2)) {
                action.Action = "mouse_2";
                Input.ParseInputEvent(action);
            }
            else if (replayFrame.StandardKeys.HasFlag(OsuParsers.Enums.Replays.StandardKeys.Smoke)) {
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
            if (!(beatmap.TimingPoints.IndexOf(currentTimingPoint) + 1 >= beatmap.TimingPoints.Count)) {
                currentTimingPoint = beatmap.TimingPoints[beatmap.TimingPoints.IndexOf(currentTimingPoint) + 1];
            }
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
            currentHitObject = hitCircle;

            OsuParsers.Enums.Beatmaps.SampleSet sampleSet = currentTimingPoint.SampleSet;

            if (circle.Extras.SampleSet != OsuParsers.Enums.Beatmaps.SampleSet.None) {
                sampleSet = circle.Extras.SampleSet;
            }

            OsuParsers.Enums.Beatmaps.SampleSet additionSet = circle.Extras.AdditionSet;

            hitCircle.startTime = circle.StartTime;
            hitCircle.setCS(beatmap.DifficultySection.CircleSize);
            hitCircle.setHitsoundSettings(circle.HitSound, sampleSet, additionSet, currentTimingPoint.Volume);
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
            currentHitObject = hitSlider;

            OsuParsers.Enums.Beatmaps.SampleSet sampleSet = currentTimingPoint.SampleSet;

            if (slider.Extras.SampleSet != OsuParsers.Enums.Beatmaps.SampleSet.None) {
                sampleSet = slider.Extras.SampleSet;
            }

            OsuParsers.Enums.Beatmaps.SampleSet additionSet = slider.Extras.AdditionSet;


            hitSlider.circle = hitSlider.GetNode<HitCircle>("HitCircle");
            hitSlider.line2d = hitSlider.GetNode<Line2D>("CanvasGroup/Line2D");
            
            hitSlider.circle.isSlider = true;
            hitSlider.circle.slider = hitSlider;

            hitSlider.circle.setCS(beatmap.DifficultySection.CircleSize);
            hitSlider.circle.setHitsoundSettings(slider.HitSound, sampleSet, additionSet, currentTimingPoint.Volume);
            hitSlider.Position = OsuConverter.OsuPixelToGodotPixel(slider.Position.X, slider.Position.Y);
            hitSlider.circle.setComboNumber(currentCombo);

            hitSlider.line2d.AddPoint(new Vector2(0, 0));
            hitSlider.curveType = slider.CurveType;
            hitSlider.pixelLength = slider.PixelLength;
            hitSlider.repeatCount = slider.Repeats - 1;
            hitSlider.timeLength = slider.TotalTimeSpan.TotalMilliseconds;
            hitSlider.edgeAdditions = slider.EdgeAdditions;
            hitSlider.edgeHitsounds = slider.EdgeHitSounds;
            hitSlider.SetCS(beatmap.DifficultySection.CircleSize);

            foreach (System.Numerics.Vector2 point in slider.SliderPoints) {
                Vector2 vec = OsuConverter.OsuPixelToGodotPixel(point.X, point.Y) - hitSlider.Position;
                hitSlider.points.Add(vec);
                hitSlider.line2d.AddPoint(vec);
            }

            GetNode<Node2D>("../").CallDeferred("add_child", hitSlider);
            hitSlider.circle.fadeIn(beatmap.DifficultySection.ApproachRate);

            currentCombo++;
        }
        else if (obj is OsuParsers.Beatmaps.Objects.Spinner) {
            OsuParsers.Beatmaps.Objects.Spinner spinner = (OsuParsers.Beatmaps.Objects.Spinner)obj;
            // check if spinner is able to be shown

            if (totalDeltaTime*1000 <= spinner.StartTime) {
                return;
            }

            currentCombo = 1;

            HitSpinner hitSpinner = hitSpinnerScene.Instantiate<HitSpinner>();
            currentHitObject = hitSpinner;

            OsuParsers.Enums.Beatmaps.SampleSet sampleSet = currentTimingPoint.SampleSet;

            if (spinner.Extras.SampleSet != OsuParsers.Enums.Beatmaps.SampleSet.None) {
                sampleSet = spinner.Extras.SampleSet;
            }

            OsuParsers.Enums.Beatmaps.SampleSet additionSet = spinner.Extras.AdditionSet;

            hitSpinner.endTime = spinner.EndTime;
            hitSpinner.hitsound = spinner.HitSound;
            hitSpinner.sampleSet = sampleSet;
            hitSpinner.additionSet = additionSet;
            hitSpinner.volume = currentTimingPoint.Volume;

            GetNode<Node2D>("../").CallDeferred("add_child", hitSpinner);
            hitSpinner.fadeIn(beatmap.DifficultySection.ApproachRate);
        }
        hitObjectIndex++;
    }
}
