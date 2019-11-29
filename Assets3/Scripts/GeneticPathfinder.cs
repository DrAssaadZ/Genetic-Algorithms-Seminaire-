using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticPathfinder : MonoBehaviour
{
    public static float elapsed = 0;
    public float timeStepsPenalty;
    public float creatureSpeed;
    public float pathMultiplier; //size of the step taken by the agent
    public float rotationSpeed;
    int pathIndex = 0; //index of the step
    public DNA dna;
    public bool hasFinished = false;

    public LayerMask obstacleLayer; //this variable is used for interaction with the obstacles

    bool hasBeenInitialized = false;
    bool hasCrashed = false; //if the agent has a contact with an obstacle
    List<Vector2> travelledPath = new List<Vector2>(); //list that contains the  the path taken in term of steps
    Vector2 target;
    Vector2 nextPoint;
    Quaternion targetRotation; // smoothing the rotation animation
    LineRenderer lr; // drawing the traveled path on the screen

    public void InitCreature(DNA newDna, Vector2 _target)
    {
        travelledPath.Add(transform.position);
        lr = GetComponent<LineRenderer>(); // binding the line renderer with the object
        dna = newDna;
        target = _target;
        nextPoint = transform.position; //setting then next point as the current agent's position
        travelledPath.Add(nextPoint);
        hasBeenInitialized = true;
        elapsed = 0;
    }

    //mainly used for movements
    private void Update()
    {
        if (hasBeenInitialized && !hasFinished) //keep moving
        {   
            //if the agent has reached the goal or his possible moves are over
            if(pathIndex == dna.genes.Count || Vector2.Distance(transform.position, target) < 0.5f)
            {
                hasFinished = true;
            }
            if((Vector2)transform.position == nextPoint) //calculating the next step in case the agent reached the planed next point
            {
                nextPoint = (Vector2)transform.position + dna.genes[pathIndex] * pathMultiplier;
                travelledPath.Add(nextPoint);
                targetRotation = LookAt2D(nextPoint);
                pathIndex++;
            }
            else
            {
                //moving the agent to the next position
                transform.position = Vector2.MoveTowards(transform.position, nextPoint, creatureSpeed * Time.deltaTime);
                elapsed += Time.deltaTime; 
            }
            if(transform.rotation != targetRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            RenderLine();
        }
    }

    //a function that draws the travelled path
    public void RenderLine()
    {
        List<Vector3> linePoints = new List<Vector3>();
        if(travelledPath.Count > 2)
        {
            for (int i = 0; i < travelledPath.Count - 1; i++)
            {
                linePoints.Add(travelledPath[i]);
            }
            linePoints.Add(transform.position);
        }
        else
        {
            linePoints.Add(travelledPath[0]);
            linePoints.Add(transform.position);
        }
        lr.positionCount = linePoints.Count;
        lr.SetPositions(linePoints.ToArray());
        
    }
    //fit function that uses the distance and how many obstacles are on the way to the destination
    public float fitness
    {
        get
        {
            float dist = Vector2.Distance(transform.position, target); // the distance between the agent and the destination
            if(dist == 0) // normalisation of the distance for calculation purposes
            {
                dist = 0.0001f;
            }
            //returting the obstances that are between the agent and the destination
            RaycastHit2D[] obstacles = Physics2D.RaycastAll(transform.position, target, obstacleLayer);
            //adding a penalty for each obstacles between the agent and the destination
            float obstacleMultiplier = 1f - (0.1f * obstacles.Length);
            //return the fitness value, give a penalty for : longer distances, if the agent has crashed on not, and the obstacles count penalty
            return (60/dist) * (hasCrashed ? 0.5f : 1f) * obstacleMultiplier;
        }
    }

    //fit function that only uses the distance from the destination
    // public float fitness
    // {
    //     get
    //     {
    //         float dist = Vector2.Distance(transform.position, target); // the distance between the agent and the destination
    //         if(dist == 0) // normalisation of the distance for calculation purposes
    //         {
    //             dist = 0.0001f;
    //         }
            
    //         return 1/dist;
    //     }
    // }

    //fit function that uses the distance to the destination and a penalty
    // public float fitness
    // {
    //     get
    //     {
    //         float dist = Vector2.Distance(transform.position, target); // the distance between the agent and the destination
    //         if(dist == 0) // normalisation of the distance for calculation purposes
    //         {
    //             dist = 0.0001f;
    //         }
            
    //         timeStepsPenalty = (1/(dist));
    //         // timeStepsPenalty = Mathf.Pow(fitness, 4);
    //         if(hasCrashed) timeStepsPenalty*= 0.1f;
    //         if(dist <= 0.5f) timeStepsPenalty*= 3f; 
    //         return timeStepsPenalty;
    //     }
    // }

    //function that smoothes the rotation of the agent
    public Quaternion LookAt2D(Vector2 target, float angleOffset = -90)
    {
        Vector2 fromTo = (target - (Vector2)transform.position).normalized;
        float zRotation = Mathf.Atan2(fromTo.y, fromTo.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, zRotation + angleOffset);
    }

    //triggering on a collision with an obstacle
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            hasFinished = true;
            hasCrashed = true;
        }
    }
}
