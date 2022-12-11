using UnityEngine;

public class gridCubeNode : MonoBehaviour
{
    public bool visited = false; //bool to check if this node has already been visited or not
    public GameObject previousNode = null; //GameObject of the node that was traversed before getting to this one
    public int distance = int.MaxValue; //distance from start node (Weight/Cost)
    public float distanceToEndNode = 0; //distance from this node to the end 'target' node - needs to be initialized during grid creation
    public float AStarDistance = 100000; //distance+distanceToEndNode
}
