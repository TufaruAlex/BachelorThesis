using System;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PathFindController : MonoBehaviour
{
    public Transform playerTransform;

    public float maxLength = 5f;

    public Seeker seeker;

    // Update is called once per frame
    void Update()
    {
        GetComponent<AIPath>().enabled = Vector3.Distance(playerTransform.position, gameObject.transform.position) <= maxLength;
        // seeker.StartPath(gameObject.transform.position, playerTransform.position, OnPathCalculated);
    }

    private void OnPathCalculated(Path path)
    {
        if(path.error){
            return;
        }

        var pathLength = path.GetTotalLength();

        if (pathLength <= maxLength)
        {
            GetComponent<AIPath>().enabled = true;
        }
        else if (pathLength > maxLength)
        {
            GetComponent<AIPath>().enabled = false;
        }
    }
}
