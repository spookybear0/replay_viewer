using Godot;
using System;
using System.Collections.Generic;

public partial class HitSlider : Node2D {
    public Line2D line2d;
    public CanvasGroup canvasgroup;
    public HitCircle circle;
    public List<Vector2> points = new List<Vector2>();
    public OsuParsers.Enums.Beatmaps.CurveType curveType;
    public double timeLength;
    public int repeatCount;
    public double pixelLength;

    private float precision = 0.01f;

    public override void _Ready() {
        line2d = GetNode<Line2D>("CanvasGroup/Line2D");
        canvasgroup = GetNode<CanvasGroup>("CanvasGroup");
        circle = GetNode<HitCircle>("HitCircle");

        line2d.Points = new Vector2[] { };

        points.Insert(0, circle.Position);

        if (curveType == OsuParsers.Enums.Beatmaps.CurveType.Linear) {
            foreach (Vector2 point in points) {
                line2d.AddPoint(point);
            }
        }
        else if (curveType == OsuParsers.Enums.Beatmaps.CurveType.Bezier) {
            Bezier(points);
        }
        else if (curveType == OsuParsers.Enums.Beatmaps.CurveType.Catmull) {
            CatmullRom(points);
        }
        else if (curveType == OsuParsers.Enums.Beatmaps.CurveType.PerfectCurve) {
            // TODO: implement perfect curve
            ClassicBezier(points);
        }

        // reverse the line2d points

        Vector2[] reversedPoints = new Vector2[line2d.Points.Length];

        for (int i = 0; i < line2d.Points.Length; i++) {
            reversedPoints[i] = line2d.Points[line2d.Points.Length - 1 - i];
        }

        line2d.Points = reversedPoints;

        // set default opacity to 0

        canvasgroup.SelfModulate = new Color(1, 1, 1, 0);

        // set lengths


    }

    // TODO: implement perfect curve

    /*public void Bezier(List<Vector2> points_) {
        // consider each two points that have equal x or y values as a
        // "stopping point" or a "red point" to split the curve into multiple bezier curves
        // which will create sharp angles in the curve
        // this is the same way osu! does it

        // we're going to have to loop through each point on the list
        // and check if the next point is a red point 
        // if it is, we'll have to create a new bezier curve for the points after the red point
        // if theres two red points in a row, we'll have to create a linear line between them
        // the first and last points can't be red points
        // if theres no red points, we'll just create a bezier curve for the whole list

        // behavior for sliders with no red points

        bool hasRedPoints = false;

        for (int i = 0; i < points_.Count - 1; i++) {
            if (points_[i].X == points_[i + 1].X || points_[i].Y == points_[i + 1].Y) {
                hasRedPoints = true;
                break;
            }
        }

        if (!hasRedPoints) {
            ClassicBezier(points_);
            return;
        }

        // behavior for sliders with red points

        List<Vector2> tempPoints = new List<Vector2>();

        for (int i = 0; i < points_.Count - 1; i++) {
            if (points_[i].X == points_[i + 1].X || points_[i].Y == points_[i + 1].Y) {
                tempPoints.Add(points_[i]);

                ClassicBezier(tempPoints);

                tempPoints.Clear();
            }
            else {
                // linearly interpolate between the two points
                for (float t = 0; t < 1; t += precision) {
                    line2d.AddPoint(points_[i].Lerp(points_[i + 1], t));
                }
            }
        }
    }*/

    public void Bezier(List<Vector2> points_) {
        List<Vector2> curvePoints = new List<Vector2>();
        List<Vector2> currentSegmentPoints = new List<Vector2>();

        for (int i = 0; i < points_.Count - 1; i++) {
            currentSegmentPoints.Add(points_[i]);

            bool isRedPoint = points_[i].X == points_[i + 1].X || points_[i].Y == points_[i + 1].Y;

            if (isRedPoint) {
                if (currentSegmentPoints.Count == 2) {
                    // Handle linear interpolation for two points in the segment
                    LinearInterpolate(currentSegmentPoints[0], currentSegmentPoints[1]);
                } else {
                    // Handle the current segment as a Bezier curve
                    ClassicBezier(currentSegmentPoints);
                }

                // Clear the points list for the next segment
                currentSegmentPoints.Clear();
            }
        }

        // Add the last point of the input list as the last point of the last segment
        currentSegmentPoints.Add(points_[points_.Count - 1]);

        if (currentSegmentPoints.Count == 2) {
            // Handle linear interpolation for the last two points in the segment
            LinearInterpolate(currentSegmentPoints[0], currentSegmentPoints[1]);
        } else {
            // Handle the last segment as a Bezier curve
            ClassicBezier(currentSegmentPoints);
        }
    }

    public void LinearInterpolate(Vector2 start, Vector2 end) {
        int steps = Mathf.FloorToInt(1 / precision);
        for (int i = 0; i < steps; i++) {
            float t = i * precision;
            Vector2 point = start.Lerp(end, t);
            line2d.AddPoint(point);
        }
    }

    public void ClassicBezier(List<Vector2> points_) {
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
            line2d.AddPoint(lastLerped[0]);
        }
    }

    public void CatmullRom(List<Vector2> points_) {
        if (points_.Count < 4) {
            GD.PrintErr("Centripetal Catmull-Rom interpolation requires at least 4 control points, invalid map?");
            return;
        }

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
            line2d.AddPoint(lastInterpolated[0]);
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

    public void PerfectCurve(List<Vector2> points_) {
        if (points_.Count != 3) {
            GD.PrintErr("Perfect Curve interpolation requires 3 control points, invalid map?");
            return;
        }


    }
}