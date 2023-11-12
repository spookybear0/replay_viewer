using Godot;
using System;

public partial class OsuConverter : Node2D {
    static Viewport _viewport;

    public override void _EnterTree() {
        _viewport = GetViewport();
    }

    public static Vector2 OsuPixelToGodotPixel(Vector2 osuPixel) {
        return new Vector2(osuPixel.X*1.33333333333333f+234, osuPixel.Y*1.33333333333333f+64.8f);
    }

    public static Vector2 OsuPixelToGodotPixel(float x, float y) {
        return new Vector2(x*1.33333333333333f+234, y*1.33333333333333f+64.8f);
    }

    public static Vector2 OsuPixelToGodotPixel(System.Numerics.Vector2 osuPixel) {
        return new Vector2(osuPixel.X*1.33333333333333f+234, osuPixel.Y*1.33333333333333f+64.8f);
    }

    public static Vector2 GodotPixelToOsuPixel(Vector2 godotPixel) {
        return new Vector2((godotPixel.X-234)/1.33333333333333f, (godotPixel.Y-64.8f)/1.33333333333333f);
    }

    public static Vector2 GodotPixelToOsuPixel(float x, float y) {
        return new Vector2((x-234)/1.33333333333333f, (y-64.8f)/1.33333333333333f);
    }

    public static Vector2 GodotPixelToOsuPixel(System.Numerics.Vector2 godotPixel) {
        return new Vector2((godotPixel.X-234)/1.33333333333333f, (godotPixel.Y-64.8f)/1.33333333333333f);
    }

    public static float PlayfieldHeight() {
        return _viewport.GetCamera2D().GetViewportRect().Size.Y * 0.8f;
    }

    public static float PlayfieldWidth() {
        return PlayfieldHeight() * (4f/3f);
    }

    public static float PlayfieldLeft() {
        return (_viewport.GetCamera2D().GetViewportRect().Size.X - PlayfieldWidth()) / 2f;
    }

    public static float PlayfieldTop() {
        return (_viewport.GetCamera2D().GetViewportRect().Size.Y - PlayfieldHeight()) / 2f + PlayfieldHeight() * 0.02f;
    }

    public static float PlayfieldRight() {
        return PlayfieldLeft() + PlayfieldWidth();
    }

    public static float PlayfieldBottom() {
        return PlayfieldTop() + PlayfieldHeight();
    }

    public static double ARToPreemptTime(float ar) {
        double preemptTime; // time before circle appears in ms
        if (ar < 5) {
            preemptTime = 1200 + 600 * (5 - ar) / 5;
        }
        else if (ar > 5) {
            preemptTime = 1200 - 750 * (ar - 5) / 5;
        } // is 5
        else {
            preemptTime = 1200;
        }

        return preemptTime;
    }

    public static double ARToCompleteFadeInTime(float ar) {
        double fadeInTime; // time before circle appears in ms
        if (ar < 5) {
            fadeInTime = 800 + 400 * (5 - ar) / 5;
        }
        else if (ar > 5) {
            fadeInTime = 800 - 500 * (ar - 5) / 5;
        } // is 5
        else {
            fadeInTime = 800;
        }

        return fadeInTime;
    }

    public static float CSToScale(float cs) {
        return (1-(0.7f*(cs-5)/5))*(PlayfieldWidth()/16f)*2;
    }
}