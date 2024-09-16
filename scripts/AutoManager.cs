using Godot;
using OsuParsers.Beatmaps.Objects;
using System;
using System.Collections.Generic;
using System.Geometry;


public partial class AutoManager : Node2D {
    Cursor cursor;
    BeatmapManager beatmapManager;
    Node2D lastHitObject;

    Tween moveToNextCircleTween;

    public override void _Ready(){
        cursor = GetNode<Cursor>("/root/Scene/Playfield/Cursor");
        beatmapManager = GetNode<BeatmapManager>("/root/Scene/BeatmapManager");
    }

    public override void _Process(double delta) {
        // automatically play the map
        if (GameManager.mode == GameMode.Auto) {
            // move mouse to the next circle

            for (int i = 0; i < GetNode<Node2D>("/root/Scene").GetChildren().Count; i++) {
                Node child = GetNode<Node2D>("/root/Scene").GetChild<Node>(i);

                if (child is HitCircle) {
                    HitCircle hitCircle = child as HitCircle;
                    if (hitCircle.fadingIn && !hitCircle.clicked && !hitCircle.fadedIn && !hitCircle.autoFollowing) {
                        //cursor.animationPlayer.Stop();

                        Curve2D goToCircle = new Curve2D();

                        //cursor.Position = Vector2.Zero;

                        goToCircle.AddPoint(cursor.pathFollow2D.GlobalPosition);
                        goToCircle.AddPoint(hitCircle.GlobalPosition);

                        cursor.Curve = goToCircle;

                        cursor.animationPlayer.SpeedScale = 1 / ((float)(hitCircle.startTime - beatmapManager.totalDeltaTime*1000) / 1000);
                        cursor.animationPlayer.Play("followPath");

                        hitCircle.autoFollowing = true;
                    }
                    if (hitCircle.fadedIn && !hitCircle.clicked) {
                        hitCircle.objectHit();
                    }
                } 
                else if (child is HitSlider) {
                    HitSlider hitSlider = child as HitSlider;
                    if (hitSlider.circle.fadedIn && !hitSlider.circle.clicked) {
                        cursor.Position = hitSlider.Position;
                        hitSlider.circle.objectHit();
                        // follow the slider path

                        cursor.Curve = hitSlider.path2d.Curve;

                        // move the cursor along the path

                        cursor.animationPlayer.SpeedScale = 1 / ((float)hitSlider.timeLength / 1000);
                        cursor.animationPlayer.Play("followPath");
                    }
                }
                else if (child is HitSpinner) {
                    HitSpinner hitSpinner = child as HitSpinner;
                    if (hitSpinner.fadedIn && !hitSpinner.autoSpinning) {
                        hitSpinner.autoSpinning = true;

                        // spin around the center of the spinner

                        cursor.Position = hitSpinner.Position;

                        // move the cursor along the path

                        Curve2D spinnerPath = new Curve2D();

                        int radius = 100;
                        int rpm = 477;

                        for (int j = 0; j < (360 * (hitSpinner.endTime - beatmapManager.totalDeltaTime*1000) / 1000) * rpm; j++) {
                            Vector2 point = new Vector2(
                                (float)(radius * Math.Cos(j * Math.PI / 180)),
                                (float)(radius * Math.Sin(j * Math.PI / 180))
                            );

                            spinnerPath.AddPoint(point);
                        }

                        cursor.Curve = spinnerPath;
                        // speed depending on the RPM of auto and how long the spinner will last (hitSpinner.endTime - beatmapManager.totalDeltaTime*1000)
                        cursor.animationPlayer.SpeedScale = 1 / ((float)(hitSpinner.endTime - beatmapManager.totalDeltaTime*1000) / 1000);
                        cursor.animationPlayer.Play("followPath");
                    }
                }
            }
        }
    }
}