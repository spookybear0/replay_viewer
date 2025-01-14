using Godot;
using System;

public partial class HitCircle : Area2D {
	bool mouseInside = false;
	bool cursorInside = false;
	public bool clicked = false;
	Sprite2D circleOutline;
	Sprite2D circleOverlay;
	Sprite2D circleShader;
	Sprite2D circleCombo;
	Sprite2D circleCombo1;
	Sprite2D circleCombo2;
	Sprite2D approachCircle;
    CollisionShape2D hitbox;
	public AnimationPlayer animationPlayer;
	public double fadeInTime;
	public OsuParsers.Enums.Beatmaps.HitSoundType hitsound;
	public OsuParsers.Enums.Beatmaps.SampleSet sampleSet;
    public OsuParsers.Enums.Beatmaps.SampleSet additionSet;
	public float volume;

    public double startTime;

	public bool isSlider = false;
	public HitSlider slider; // when isSlider is true, this wont be null

    public bool fadingIn = false;
    public bool fadedIn = false;

    // auto

    public bool autoFollowing = false;

	// numbers
	static Texture2D number0;
	static Texture2D number1;
	static Texture2D number2;
	static Texture2D number3;
	static Texture2D number4;
	static Texture2D number5;
	static Texture2D number6;
	static Texture2D number7;
	static Texture2D number8;
	static Texture2D number9;

	public static bool texturesLoaded = false;

	public override void _Ready() {
		Connect("area_entered", new Callable(this, "areaEntered"));
		Connect("area_exited", new Callable(this, "areaExited"));

		loadTextures();

		SetProcessUnhandledInput(true);

		circleOutline = GetNode<Sprite2D>("Outline");
		circleOverlay = GetNode<Sprite2D>("Overlay");
		circleShader = GetNode<Sprite2D>("Shader");
		circleCombo = GetNode<Sprite2D>("Combo");
		circleCombo1 = GetNode<Sprite2D>("Combo1");
		circleCombo2 = GetNode<Sprite2D>("Combo2");
		approachCircle = GetNode<Sprite2D>("ApproachCircle");
        hitbox = GetNode<CollisionShape2D>("Hitbox");


		if (isSlider) {
			animationPlayer = GetNode<AnimationPlayer>("../AnimationPlayer");
		}
		else {
			animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		}

		animationPlayer.Connect("animation_finished", new Callable(this, "AnimationFinished"));

		// set default opacity to 0
		circleOutline.Modulate = new Color(1, 1, 1, 0);
		circleOverlay.Modulate = new Color(1, 1, 1, 0);
		circleShader.Modulate = new Color(1, 1, 1, 0);
		circleCombo.Modulate = new Color(1, 1, 1, 0);
		circleCombo1.Modulate = new Color(1, 1, 1, 0);
		circleCombo2.Modulate = new Color(1, 1, 1, 0);
		approachCircle.Modulate = new Color(1, 1, 1, 0);
	}

    public void areaEntered(Area2D area) {
		if (area.Name == "CursorArea") {
			cursorInside = true;
		}
	}

	public void areaExited(Area2D area) {
		if (area.Name == "CursorArea") {
			cursorInside = false;
		}
	}

	public void objectHit() {
		// play the hit sound
        clicked = true;

        if (isSlider) {
            if (slider.edgeHitsounds != null && slider.edgeAdditions != null) {
                // sliders store hitsounds differently
                HitsoundManager.playHitsound(
                    slider.edgeHitsounds[0],
                    slider.edgeAdditions[0].Item1,
                    slider.edgeAdditions[0].Item2,
                    volume
                );
            }
            else {
                // no hitsounds, this will just play the default hitsound with no additions
                HitsoundManager.playHitsound(hitsound, sampleSet, additionSet, volume);
            }
        }
		else {
            HitsoundManager.playHitsound(hitsound, sampleSet, additionSet, volume);
        }

		// fade out the hitcircle

		animationPlayer.SpeedScale = (1 / ((float)fadeInTime / 1000)) * 4;

        animationPlayer.Play("hit");
		
		if (isSlider) {
			Timer t = new Timer();
			t.WaitTime = slider.timeLength / 1000;
			t.OneShot = true;
			t.Connect("timeout", new Callable(this, "fadeOutSlider"));
			AddChild(t);
			t.Start();
		}
	}

	public static void loadTextures() {
		if (texturesLoaded) return;

		number0 = ResourceLoader.Load<Texture2D>("res://assets/numbers/default-0.png");
		number1 = ResourceLoader.Load<Texture2D>("res://assets/numbers/default-1.png");
		number2 = ResourceLoader.Load<Texture2D>("res://assets/numbers/default-2.png");
		number3 = ResourceLoader.Load<Texture2D>("res://assets/numbers/default-3.png");
		number4 = ResourceLoader.Load<Texture2D>("res://assets/numbers/default-4.png");
		number5 = ResourceLoader.Load<Texture2D>("res://assets/numbers/default-5.png");
		number6 = ResourceLoader.Load<Texture2D>("res://assets/numbers/default-6.png");
		number7 = ResourceLoader.Load<Texture2D>("res://assets/numbers/default-7.png");
		number8 = ResourceLoader.Load<Texture2D>("res://assets/numbers/default-8.png");
		number9 = ResourceLoader.Load<Texture2D>("res://assets/numbers/default-9.png");

		texturesLoaded = true;
	}

    public override void _Input(InputEvent @event) {
		if (cursorInside && !clicked) {
			// check if key_1, key_2, mouse_1, or mouse_2 is pressed
			if ((@event.IsActionPressed("key_1") || @event.IsActionPressed("key_2") ||
				@event.IsActionPressed("mouse_1") || @event.IsActionPressed("mouse_2")) &&
				!clicked) {
				clicked = true;
				//objectHit();
				GetViewport().SetInputAsHandled();
			}
		}
	}


	public void setCS(float cs) {
		circleOverlay = GetNode<Sprite2D>("Overlay");
        hitbox = GetNode<CollisionShape2D>("Hitbox");

		float csScale = OsuConverter.CSToScale(cs);

		Scale = new Vector2(csScale/circleOverlay.Texture.GetSize().X, csScale/circleOverlay.Texture.GetSize().Y);
        hitbox.Scale = new Vector2(csScale / circleOverlay.Texture.GetSize().X, csScale / circleOverlay.Texture.GetSize().Y);
	}

	public void fadeIn(float ar) {
		if (isSlider) {
			animationPlayer = GetNode<AnimationPlayer>("../AnimationPlayer");
		}
		else {
			animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		}

        fadingIn = true;

		fadeInTime = OsuConverter.ARToPreemptTime(ar);

		animationPlayer.SpeedScale = 1 / ((float)fadeInTime / 1000);

		animationPlayer.Play("fadein");
	}

	public void fadeOut() {
		animationPlayer.Play("fadeout");
	}

	public void fadeOutSlider() {
		animationPlayer.Play("fadeoutslider");

        if (isSlider) { // this should ALWAYS be ture
            // slider end hitsound

            // sliders do hitsounds differently
            // lets get the last edge hitsound

            if (slider.edgeHitsounds != null && slider.edgeAdditions != null) {
                HitsoundManager.playHitsound(
                    slider.edgeHitsounds[slider.edgeHitsounds.Count - 1],
                    slider.edgeAdditions[slider.edgeAdditions.Count - 1].Item1,
                    slider.edgeAdditions[slider.edgeAdditions.Count - 1].Item2,
                    volume
                );
            }
            else {
                // no hitsounds, this will just play the default hitsound with no additions
                HitsoundManager.playHitsound(hitsound, sampleSet, additionSet, volume);
            }
        }
	}

	public void AnimationFinished(string name) {
		if (name == "fadeout" || name == "fadeoutslider" || name == "hit") {
            // remove the slider from the scene tree
            if (isSlider) {
                if (name != "hit") {
                    slider.QueueFree();
                }
            }
            else {
                QueueFree();
            }
		}

		if (name == "fadein") {
            fadedIn = true;
            if (isSlider) {
                Timer t = new Timer();
                t.WaitTime = slider.timeLength / 1000;
                t.OneShot = true;
                t.Connect("timeout", new Callable(this, "fadeOut"));
                AddChild(t);
                t.Start();
            }
            else {
                //fadeOut();
            }
            if (GameManager.mode != GameMode.Auto) {
                /**/objectHit();/**/
            }
		}
	}

	public Texture2D getTextureFromNumber(int number) {
		loadTextures();
		Texture2D texture;

		switch (number) {
			case 0:
				texture = number0;
				break;
			case 1:
				texture = number1;
				break;
			case 2:
				texture = number2;
				break;
			case 3:
				texture = number3;
				break;
			case 4:
				texture = number4;
				break;
			case 5:
				texture = number5;
				break;
			case 6:
				texture = number6;
				break;
			case 7:
				texture = number7;
				break;
			case 8:
				texture = number8;
				break;
			case 9:
				texture = number9;
				break;
			default:
				texture = number0;
				break;
		}

		return texture;
	}

	public void setComboNumber(int number) {
		circleCombo = GetNode<Sprite2D>("Combo");
		circleCombo1 = GetNode<Sprite2D>("Combo1");
		circleCombo2 = GetNode<Sprite2D>("Combo2");

		if (number < 10) { // 1 digit
			circleCombo.Texture = getTextureFromNumber(number);

			circleCombo.Visible = true;
			circleCombo1.Visible = false;
			circleCombo2.Visible = false;
		}
		else { // 2 digits
			if (number > 99) {
				number = 99;
			}

			circleCombo1.Texture = getTextureFromNumber(number / 10);
			circleCombo2.Texture = getTextureFromNumber(number % 10);

			circleCombo.Visible = false;
			circleCombo1.Visible = true;
			circleCombo2.Visible = true;
		}
	}

	public void setHitsoundSettings(
        OsuParsers.Enums.Beatmaps.HitSoundType _hitsound,
        OsuParsers.Enums.Beatmaps.SampleSet sampleSet_,
        OsuParsers.Enums.Beatmaps.SampleSet additionSet_,
        float volume_
    ) {
		hitsound = _hitsound;
		sampleSet = sampleSet_;
        additionSet = additionSet_;
		volume = volume_;
	}
}