using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TimeMeasure
{
    public float PrecisionMs { get; }
    //int TimerResolutionSample { get; set; }
    float RelativeErrorAllowed { get; }
    int AvgSamples { get; }

    float MinDurationMs { get; }

    public TimeMeasure(float precision, float relativeErrorAllowed, int avgSamples)
    {
        PrecisionMs = EstimatePrecision() * 1E6f;
        RelativeErrorAllowed = relativeErrorAllowed;
        MinDurationMs = PrecisionMs * (1f + 1f / RelativeErrorAllowed);
        AvgSamples = avgSamples;
    }

    /// <summary>
    /// The minimum measurable time on this system in nanoseconds. See: 
    /// https://stackoverflow.com/questions/7137121/high-resolution-timer
    /// </summary>
    /// <returns>Number of nanoseconds</returns>
    public float EstimatePrecision()
    {
        //no need to average
        return MeasureSmallestDeltaNs();
    }

    float MeasureSmallestDeltaNs()
    {
        var watch = new Stopwatch();

        return (float)(1E9 / Stopwatch.Frequency);
    }

    /// <summary>
    /// Computes average time of an algorithm on a NavMesh graph.
    /// </summary>
    /// <param name="g">Nav Mesh graph</param>
    /// <param name="algorithm">algorithm function</param>
    /// <returns>average of time taken</returns>
    public float CheckTimetakenAlg(NavMeshGraph g, System.Func<NavMeshGraph, int, int, List<int>> algorithm)
    {
        float[] times = new float[AvgSamples];
        for (int i = 0; i < AvgSamples; i++)
        {
            //TODO random start and end nodes
            //times[i] = CheckTimetakenSingleRun(g, algorithm, start, target);
        }

        return times.Average();
    }

    /// <summary>
    /// Computes time taken of an algorithm on a NavMesh graph.
    /// </summary>
    /// <param name="g">Nav Mesh graph</param>
    /// <param name="algorithm">algorithm function</param>
    /// <param name="start">start node id</param>
    /// <param name="target">target node id</param>
    /// <returns>milliseconds of a single run of the algorithm</returns>
    float CheckTimetakenSingleRun(NavMeshGraph g, System.Func<NavMeshGraph, int, int, List<int>> algorithm, int start, int target)
    {
        var watch = new Stopwatch();

        int runs = 0;
        do
        {
            algorithm(g, start, target);
            runs++;
        } while (watch.ElapsedMilliseconds < MinDurationMs);

        watch.Stop();

        return watch.ElapsedMilliseconds;
    }

}
