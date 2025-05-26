using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ProgressManager : MonoBehaviour
{
    public Transform player;
    public List<Transform> waypoints;
    public Slider progressBar;
    public TextMeshProUGUI text;
    private float totalTrackLength;
    private List<float> segmentLengths = new();
    public Transform needle;
    public float minAngle = -90f;
    public float maxAngle = 90f;


    void Start()
    {
        totalTrackLength = 0f;
        segmentLengths.Clear();

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            float segmentLength = Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
            segmentLengths.Add(segmentLength);
            totalTrackLength += segmentLength;
        }
    }

    void Update()
    {
        float progress = Mathf.Clamp01(GetPlayerTrackDistance() / totalTrackLength);
        progressBar.value = progress;
        text.text = (progress * 100f).ToString("F0") + "%";

        float angle = Mathf.Lerp(minAngle, maxAngle, progress);
        needle.localEulerAngles = new Vector3(0f, 0f, angle);

    }

    float GetPlayerTrackDistance()
    {
        float closestDist = float.MaxValue;
        float distanceAlongTrack = 0f;
        float trackSoFar = 0f;

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Vector3 a = waypoints[i].position;
            Vector3 b = waypoints[i + 1].position;

            Vector3 closestPoint = GetClosestPointOnSegment(a, b, player.position);
            float distToPlayer = Vector3.Distance(player.position, closestPoint);

            if (distToPlayer < closestDist)
            {
                closestDist = distToPlayer;
                distanceAlongTrack = trackSoFar + Vector3.Distance(a, closestPoint);
            }

            trackSoFar += segmentLengths[i];
        }

        return distanceAlongTrack;
    }

    Vector3 GetClosestPointOnSegment(Vector3 a, Vector3 b, Vector3 point)
    {
        Vector3 ab = b - a;
        float t = Mathf.Clamp01(Vector3.Dot(point - a, ab) / ab.sqrMagnitude);
        return a + t * ab;
    }
}
