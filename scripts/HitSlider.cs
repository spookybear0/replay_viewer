using Godot;
using System;
using System.Collections.Generic;

public partial class HitSlider : Node2D {
    public Line2D line2d;
    public HitCircle circle;
    public List<Vector2> points = new List<Vector2>();
    public OsuParsers.Enums.Beatmaps.CurveType curveType;

    public override void _Ready() {
        line2d = GetNode<Line2D>("Line2D");
        circle = GetNode<HitCircle>("HitCircle");

        line2d.Points = new Vector2[] { };

        points.Insert(0, circle.Position);

        if (curveType == OsuParsers.Enums.Beatmaps.CurveType.Linear) {
            foreach (Vector2 point in points) {
                line2d.AddPoint(OsuConverter.OsuPixelToGodotPixel(point.X, point.Y));
            }
        }
        else if (curveType == OsuParsers.Enums.Beatmaps.CurveType.Bezier) {
            Bezier(points);
        }
        else if (curveType == OsuParsers.Enums.Beatmaps.CurveType.Catmull) {
            CatmullRom(points, 10);
        }
        else if (curveType == OsuParsers.Enums.Beatmaps.CurveType.PerfectCurve) {
            Bezier(points);
        }

        // set default opacity to 0

        line2d.Modulate = new Color(1, 1, 1, 0);
    }

    // TODO: implement perfect circle

    public void Bezier(List<Vector2> points_) {
        float precision = 0.001f;

        for (float t = 0; t < 1; t += precision) {
            List<Vector2> lastLerped = points_;

            while (lastLerped.Count > 1) {
                List<Vector2> tempList = new List<Vector2>();

                for (int i = 0; i < lastLerped.Count - 1; i++) {
                    if (lastLerped.Count - 1 >= i) {
                        tempList.Add(lastLerped[i].Lerp(lastLerped[i + 1], t));
                    }
                }
                lastLerped = tempList;
            }
            points_.Add(lastLerped[0]);
        }
    }

    public void CatmullRom(List<Vector2> points_, int resolution) {
        if (points_.Count < 4) {
            GD.PrintErr("Centripetal Catmull-Rom interpolation requires at least 4 control points, invalid map?");
            return;
        }

        float precision = 1f / resolution;

        for (float t = 0; t < 1; t += precision) {
            List<Vector2> lastInterpolated = points_;

            while (lastInterpolated.Count > 1) {
                List<Vector2> tempList = new List<Vector2>();

                for (int i = 0; i < lastInterpolated.Count - 3; i++) {
                    Vector2 p0 = lastInterpolated[i];
                    Vector2 p1 = lastInterpolated[i + 1];
                    Vector2 p2 = lastInterpolated[i + 2];
                    Vector2 p3 = lastInterpolated[i + 3];

                    float t0 = 0;
                    float t1 = GetCatmullRomT(t, p0, p1);
                    float t2 = GetCatmullRomT(t, p1, p2);
                    float t3 = GetCatmullRomT(t, p2, p3);

                    Vector2 interpolatedPoint = InterpolateCentripetalCatmullSegment(p0, p1, p2, p3, t0, t1, t2, t3);
                    tempList.Add(interpolatedPoint);
                }
                lastInterpolated = tempList;
            }
            points_.Add(lastInterpolated[0]);
        }
    }

    private float GetCatmullRomT(float t, Vector2 p0, Vector2 p1) {
        float alpha = 0.5f;
        float d = Mathf.Pow(p0.DistanceTo(p1), alpha) + Mathf.Epsilon;
        float tPower = Mathf.Pow(t, alpha);
        return tPower / d;
    }

    private Vector2 InterpolateCentripetalCatmullSegment(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t0, float t1, float t2, float t3) {
        float t02 = t0 * t0;
        float t03 = t02 * t0;
        float t12 = t1 * t1;
        float t13 = t12 * t1;
        float t22 = t2 * t2;
        float t23 = t22 * t2;
        float t32 = t3 * t3;
        float t33 = t32 * t3;

        Vector2 interpolatedPoint =
            0.5f * ((2.0f * p1) +
            (-p0 + p2) * t2 +
            (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t22 +
            (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t32);

        return interpolatedPoint;
    }
}