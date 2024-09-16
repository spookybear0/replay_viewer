using Godot;
using System;


public partial class HitsoundManager : Node2D {
    static AudioStreamPlayer normalPlayer;
    static AudioStreamPlayer whistlePlayer;
    static AudioStreamPlayer finishPlayer;
    static AudioStreamPlayer clapPlayer;

    static AudioStream drumHitClap;
    static AudioStream drumHitFinish;
    static AudioStream drumHitNormal;
    static AudioStream drumHitWhistle;
    static AudioStream drumHitSliderSlide;
    static AudioStream drumHitSliderTick;
    static AudioStream drumHitSliderWhistle;

    static AudioStream normalHitClap;
    static AudioStream normalHitFinish;
    static AudioStream normalHitNormal;
    static AudioStream normalHitWhistle;
    static AudioStream normalHitSliderSlide;
    static AudioStream normalHitSliderTick;
    static AudioStream normalHitSliderWhistle;

    static AudioStream softHitClap;
    static AudioStream softHitFinish;
    static AudioStream softHitNormal;
    static AudioStream softHitWhistle;
    static AudioStream softHitSliderSlide;
    static AudioStream softHitSliderTick;
    static AudioStream softHitSliderWhistle;

    public override void _Ready() {
        normalPlayer = new AudioStreamPlayer();
        AddChild(normalPlayer);
        normalPlayer.Name = "HitsoundPlayer Normal";
        normalPlayer.VolumeDb = -15;

        whistlePlayer = new AudioStreamPlayer();
        AddChild(whistlePlayer);
        whistlePlayer.Name = "HitsoundPlayer Whistle";
        whistlePlayer.VolumeDb = -15;

        finishPlayer = new AudioStreamPlayer();
        AddChild(finishPlayer);
        finishPlayer.Name = "HitsoundPlayer Finish";
        finishPlayer.VolumeDb = -15;

        clapPlayer = new AudioStreamPlayer();
        AddChild(clapPlayer);
        clapPlayer.Name = "HitsoundPlayer Clap";
        clapPlayer.VolumeDb = -15;

        // store hitsounds

        drumHitClap = ResourceLoader.Load<AudioStream>("res://assets/sounds/drum-hitclap.wav");
        drumHitFinish = ResourceLoader.Load<AudioStream>("res://assets/sounds/drum-hitfinish.wav");
        drumHitNormal = ResourceLoader.Load<AudioStream>("res://assets/sounds/drum-hitnormal.wav");
        drumHitWhistle = ResourceLoader.Load<AudioStream>("res://assets/sounds/drum-hitwhistle.wav");
        drumHitSliderSlide = ResourceLoader.Load<AudioStream>("res://assets/sounds/drum-sliderslide.wav");
        drumHitSliderTick = ResourceLoader.Load<AudioStream>("res://assets/sounds/drum-slidertick.wav");
        drumHitSliderWhistle = ResourceLoader.Load<AudioStream>("res://assets/sounds/drum-slidertick.wav");

        normalHitClap = ResourceLoader.Load<AudioStream>("res://assets/sounds/normal-hitclap.wav");
        normalHitFinish = ResourceLoader.Load<AudioStream>("res://assets/sounds/normal-hitfinish.wav");
        normalHitNormal = ResourceLoader.Load<AudioStream>("res://assets/sounds/normal-hitnormal.wav");
        normalHitWhistle = ResourceLoader.Load<AudioStream>("res://assets/sounds/normal-hitwhistle.wav");
        normalHitSliderSlide = ResourceLoader.Load<AudioStream>("res://assets/sounds/normal-sliderslide.wav");
        normalHitSliderTick = ResourceLoader.Load<AudioStream>("res://assets/sounds/normal-slidertick.wav");
        normalHitSliderWhistle = ResourceLoader.Load<AudioStream>("res://assets/sounds/normal-slidertick.wav");
        
        softHitClap = ResourceLoader.Load<AudioStream>("res://assets/sounds/soft-hitclap.wav");
        softHitFinish = ResourceLoader.Load<AudioStream>("res://assets/sounds/soft-hitfinish.wav");
        softHitNormal = ResourceLoader.Load<AudioStream>("res://assets/sounds/soft-hitnormal.wav");
        softHitWhistle = ResourceLoader.Load<AudioStream>("res://assets/sounds/soft-hitwhistle.wav");
        softHitSliderSlide = ResourceLoader.Load<AudioStream>("res://assets/sounds/soft-sliderslide.wav");
        softHitSliderTick = ResourceLoader.Load<AudioStream>("res://assets/sounds/soft-slidertick.wav");
        softHitSliderWhistle = ResourceLoader.Load<AudioStream>("res://assets/sounds/soft-slidertick.wav");
    }

    public static void playHitsound(
        OsuParsers.Enums.Beatmaps.HitSoundType hitsound,
        OsuParsers.Enums.Beatmaps.SampleSet sampleSet,
        OsuParsers.Enums.Beatmaps.SampleSet additionSet,
        float volume
    ) {
        // TODO: volume

        switch (sampleSet) {
            case OsuParsers.Enums.Beatmaps.SampleSet.Drum:
                normalPlayer.Stream = drumHitNormal;
                break;
            case OsuParsers.Enums.Beatmaps.SampleSet.Normal:
                normalPlayer.Stream = normalHitNormal;
                break;
            case OsuParsers.Enums.Beatmaps.SampleSet.Soft:
                normalPlayer.Stream = softHitNormal;
                break;
            default:
                break;
        }

        normalPlayer.Play();

		if (hitsound.HasFlag(OsuParsers.Enums.Beatmaps.HitSoundType.Whistle)) {
            bool playSound = true;

            OsuParsers.Enums.Beatmaps.SampleSet actualSet = sampleSet;

            if (additionSet != OsuParsers.Enums.Beatmaps.SampleSet.None) {
                actualSet = additionSet;
            }

            switch (actualSet) {
                case OsuParsers.Enums.Beatmaps.SampleSet.Drum:
                    whistlePlayer.Stream = drumHitWhistle;
                    break;
                case OsuParsers.Enums.Beatmaps.SampleSet.Normal:
                    whistlePlayer.Stream = normalHitWhistle;
                    break;
                case OsuParsers.Enums.Beatmaps.SampleSet.Soft:
                    whistlePlayer.Stream = softHitWhistle;
                    break;
                default:
                    playSound = false;
                    break;
            }
            if (playSound) whistlePlayer.Play();
		}
		if (hitsound.HasFlag(OsuParsers.Enums.Beatmaps.HitSoundType.Finish)) {
            bool playSound = true;

            OsuParsers.Enums.Beatmaps.SampleSet actualSet = sampleSet;

            if (additionSet != OsuParsers.Enums.Beatmaps.SampleSet.None) {
                actualSet = additionSet;
            }

			switch (actualSet) {
                case OsuParsers.Enums.Beatmaps.SampleSet.Drum:
                    finishPlayer.Stream = drumHitFinish;
                    break;
                case OsuParsers.Enums.Beatmaps.SampleSet.Normal:
                    finishPlayer.Stream = normalHitFinish;
                    break;
                case OsuParsers.Enums.Beatmaps.SampleSet.Soft:
                    finishPlayer.Stream = softHitFinish;
                    break;
                default:
                    playSound = false;
                    break;
            }
            if (playSound) finishPlayer.Play();
		}
		if (hitsound.HasFlag(OsuParsers.Enums.Beatmaps.HitSoundType.Clap)) {
            bool playSound = true;

            OsuParsers.Enums.Beatmaps.SampleSet actualSet = sampleSet;

            if (additionSet != OsuParsers.Enums.Beatmaps.SampleSet.None) {
                actualSet = additionSet;
            }

			switch (actualSet) {
                case OsuParsers.Enums.Beatmaps.SampleSet.Drum:
                    clapPlayer.Stream = drumHitClap;
                    break;
                case OsuParsers.Enums.Beatmaps.SampleSet.Normal:
                    clapPlayer.Stream = normalHitClap;
                    break;
                case OsuParsers.Enums.Beatmaps.SampleSet.Soft:
                    clapPlayer.Stream = softHitClap;
                    break;
                default:
                    playSound = false;
                    break;
            }
            if (playSound) clapPlayer.Play();
		}
    }
}