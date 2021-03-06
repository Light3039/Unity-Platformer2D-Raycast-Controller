﻿using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
/*
 * TODO: Go through platforms
 */
{
    // Controller
    [SerializeField] private PlatformController controller;

    // Waypoints
    [SerializeField] private List<Vector2> localWayPoints = new List<Vector2>();

    private List<Vector2> globalWayPoints = new List<Vector2>();
    private int toWayPoint = 0;

    // Properties
    [SerializeField] private float speed = 1f;
    [SerializeField] private bool isCyclic = false;
    [SerializeField] private LeanTweenType easeType = LeanTweenType.notUsed;

    // Current state
    private Vector2 tweenPosition;

    private bool isCyclingBack = false;
    private bool isHold = false;

    #region UnityEvents

    private void Awake()
    {
        globalWayPoints.Add(transform.position);

        foreach(Vector2 wayPoint in localWayPoints)
            globalWayPoints.Add((Vector2)transform.position + wayPoint);

        tweenPosition = transform.position;
        Debug.Log(tweenPosition);

    }

    private void FixedUpdate()
    {
        if (isHold)
            return;

        if (transform.position == (Vector3)globalWayPoints[toWayPoint])
        {
            if(isCyclic)
            {
                if (isCyclingBack)
                {
                    toWayPoint--;
                    if (toWayPoint == 0)
                        isCyclingBack = false;
                }
                else
                {
                    toWayPoint++;
                    if (toWayPoint == globalWayPoints.Count - 1)
                        isCyclingBack = true;
                }
            }
            else
                toWayPoint = (toWayPoint + 1) % globalWayPoints.Count;

            LeanTween.value(gameObject, tweenPosition, globalWayPoints[toWayPoint], 1.33F)
                .setOnUpdate(new System.Action<Vector2>((value) =>
                {
                    controller.Move(value - tweenPosition, ForceDir.Self);
                    tweenPosition = value;
                })).setEase(easeType);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (Application.isPlaying)
            for (int i = 0; i < globalWayPoints.Count; i++)
            {
                {
                    Gizmos.DrawLine(globalWayPoints[i] + (Vector2.up * .5f), globalWayPoints[i] - (Vector2.up * .5f));
                    Gizmos.DrawLine(globalWayPoints[i] + (Vector2.right * .5f), globalWayPoints[i] - (Vector2.right * .5f));
                }
            }
        else
            for (int i = 0; i < localWayPoints.Count; i++)
            {
                {
                    Gizmos.DrawLine(((Vector2)transform.position + localWayPoints[i]) + Vector2.up * (.5f), ((Vector2)transform.position + localWayPoints[i]) - Vector2.up * (.5f));
                    Gizmos.DrawLine(((Vector2)transform.position + localWayPoints[i]) + Vector2.right * (.5f), ((Vector2)transform.position + localWayPoints[i]) - Vector2.right * (.5f));
                }
            }
    }

    #endregion // __UNITY_EVENTS__

    public void SetHold(bool hold, bool toggle = false)
    {
        isHold = toggle ? !isHold : hold;
    }

}