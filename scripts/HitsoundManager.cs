using Godot;
using System;


public partial class HitsoundManager : Node2D {
    static AudioStreamPlayer audioStreamPlayer;

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
        audioStreamPlayer = new AudioStreamPlayer();
        AddChild(audioStreamPlayer);
        audioStreamPlayer.Name = "HitsoundPlayer";
        audioStreamPlayer.VolumeDb = -20;

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

    public static void playHitsound(OsuParsers.Enums.Beatmaps.HitSoundType hitsound, OsuParsers.Enums.Beatmaps.SampleSet sampleSet, float volume) {
        // TODO: volume

        switch (sampleSet) {
            case OsuParsers.Enums.Beatmaps.SampleSet.Drum:
                audioStreamPlayer.Stream = drumHitNormal;
                break;
            case OsuParsers.Enums.Beatmaps.SampleSet.Normal:
                audioStreamPlayer.Stream = normalHitNormal;
                break;
            case OsuParsers.Enums.Beatmaps.SampleSet.Soft:
                audioStreamPlayer.Stream = softHitNormal;
                break;
            default:
                break;
        }

        audioStreamPlayer.Play();

		if (hitsound.HasFlag(OsuParsers.Enums.Beatmaps.HitSoundType.Whistle)) {
            bool playSound = true;
            switch (sampleSet) {
                case OsuParsers.Enums.Beatmaps.SampleSet.Drum:
                    audioStreamPlayer.Stream = drumHitWhistle;
                    break;
                case OsuParsers.Enums.Beatmaps.SampleSet.Normal:
                    audioStreamPlayer.Stream = normalHitWhistle;
                    break;
                case OsuParsers.Enums.Beatmaps.SampleSet.Soft:
                    audioStreamPlayer.Stream = softHitWhistle;
                    break;
                default:
                    playSound = false;
                    break;
            }
            if (playSound) audioStreamPlayer.Play();
		}
		if (hitsound.HasFlag(OsuParsers.Enums.Beatmaps.HitSoundType.Finish)) {
            bool playSound = true;
			switch (sampleSet) {
                case OsuParsers.Enums.Beatmaps.SampleSet.Drum:
                    audioStreamPlayer.Stream = drumHitFinish;
                    break;
                case OsuParsers.Enums.Beatmaps.SampleSet.Normal:
                    audioStreamPlayer.Stream = normalHitFinish;
                    break;
                case OsuParsers.Enums.Beatmaps.SampleSet.Soft:
                    audioStreamPlayer.Stream = softHitFinish;
                    break;
                default:
                    playSound = false;
                    break;
            }
            if (playSound) audioStreamPlayer.Play();
		}
		if (hitsound.HasFlag(OsuParsers.Enums.Beatmaps.HitSoundType.Clap)) {
            bool playSound = true;
			switch (sampleSet) {
                case OsuParsers.Enums.Beatmaps.SampleSet.Drum:
                    audioStreamPlayer.Stream = drumHitClap;
                    break;
                case OsuParsers.Enums.Beatmaps.SampleSet.Normal:
                    audioStreamPlayer.Stream = normalHitClap;
                    break;
                case OsuParsers.Enums.Beatmaps.SampleSet.Soft:
                    audioStreamPlayer.Stream = softHitClap;
                    break;
                default:
                    playSound = false;
                    break;
            }
            if (playSound) audioStreamPlayer.Play();
		}
    }
}