using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NavigationAgent : MonoBehaviour {

    //Navigation Variables
    public WaypointGraph graphNodes;
    public List<int> openList = new List<int>();
    public List<int> closedList = new List<int>();
    public List<int> currentPath = new List<int>();
    public List<int> greedyPaintList = new List<int>();
    public int currentPathIndex = 0;
    public int currentNodeIndex = 0;

    public Dictionary<int, int> cameFrom = new Dictionary<int, int>();

    // Use this for initialization
    void Start () {
        //Find waypoint graph
        graphNodes = GameObject.FindGameObjectWithTag("waypoint graph").GetComponent<WaypointGraph>();

        //Initial node index to move to
        currentPath.Add(currentNodeIndex);
    }

    //A-Star Search
    public List<int> AStarSearch(int start, int goal) {
        //clear everything at start
        openList.Clear();
        closedList.Clear();
        cameFrom.Clear();

        //begin
        openList.Add(start);

        float gScore = 0;
        float fScore = 0;

        while (openList.Count > 0)
        {
            //Find node in openList that has lowest fScore value
            int currentNode = bestOpenListFScore(start, goal);

            //Find the end, reconstruct entire path and return
            if (currentNode == goal)
            {
                return ReconstructPath(cameFrom, currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //For each nodes connected to current node
            for (int i = 0; i < graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex.Length; i++)
            {
                int thisNeighbourNode = graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex[i];

                //Ignore if neighbour is attached
                if (!closedList.Contains(thisNeighbourNode))
                {
                    //Distance from current to next node
                    float tentativeGScore = Heuristic(start, currentNode) + Heuristic(currentNode, thisNeighbourNode);

                    //Check to see if in openList or if new GScore is more sensible
                    if (!openList.Contains(thisNeighbourNode) || tentativeGScore < gScore)
                    {
                        openList.Add(thisNeighbourNode);
                    }

                    //Add to Dictionary - this neigbour came from the parent
                    if (!cameFrom.ContainsKey(thisNeighbourNode))
                    {
                        cameFrom.Add(thisNeighbourNode, currentNode);
                    }

                    gScore = tentativeGScore;
                    fScore = Heuristic(start, thisNeighbourNode) + Heuristic(thisNeighbourNode, goal);
                }
            }
        }
        return null;
    }

    public class GreedyChildren : IComparable<GreedyChildren>
    {
        public int childID { get; set; }
        public float childHScore { get; set; }

        public GreedyChildren(int childrenID, float childrenHScore)
        {
            this.childID = childrenID;
            this.childHScore = childrenHScore;
        }
        public int CompareTo(GreedyChildren other)
        {
            return this.childHScore.CompareTo(other.childHScore);
        }

    }

    //Greedy Search
    public List<int> GreedySearch(int currentNode, int goal, List<int> path) {
        if (!greedyPaintList.Contains(currentNode))
            greedyPaintList.Add(currentNode);

        //Make a custom list that stores the current node's children nodes and H scores. Sort them by ascending order of Heuristic
        List<GreedyChildren> thisNodesChildren = new List<GreedyChildren>();
        for (int i = 0; i < graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex.Length; i++)
        {
            //Add to list of child nodes
            thisNodesChildren.Add(new GreedyChildren(graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex[i], Heuristic(graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex[i], goal)));
        }
        //order child node by value
        thisNodesChildren.Sort();

        for(int i=0; i< thisNodesChildren.Count; i++)
        {
            int child = thisNodesChildren[i].childID;
            //Check if child isnt painted
            if (!greedyPaintList.Contains(child))
            {
                //paint it if it isnt
                greedyPaintList.Add(child);
                //check if child node is goal
                if (child == goal)
                {
                    //return the path used to reach goal
                    path.Add(child);
                    return path;
                }
                //use recursive greedysearch on current child node
                path = GreedySearch(child, goal, path);
                if(path.Count != 0)
                {
                    path.Add(child);
                    return path;
                }
                thisNodesChildren.RemoveAt(i);
            }
        }
        return path;
    }

    public float Heuristic(int a, int b)
    {
        return Vector3.Distance(graphNodes.graphNodes[a].transform.position, graphNodes.graphNodes[b].transform.position);
    }


    public int bestOpenListFScore(int start, int goal)
    {

        int bestIndex = 0;

        for (int i = 0; i < openList.Count; i++)
        {

            if ((Heuristic(openList[i], start) + Heuristic(openList[i], goal)) < (Heuristic(openList[bestIndex], start) + Heuristic(openList[bestIndex], goal)))
            {
                bestIndex = i;
            }
        }

        int bestNode = openList[bestIndex];
        return bestNode;
    }


    public List<int> ReconstructPath(Dictionary<int, int> CF, int current)
    {

        List<int> finalPath = new List<int>();

        finalPath.Add(current);

        while (CF.ContainsKey(current))
        {

            current = CF[current];

            finalPath.Add(current);
        }

        finalPath.Reverse();

        return finalPath;
    }


}
