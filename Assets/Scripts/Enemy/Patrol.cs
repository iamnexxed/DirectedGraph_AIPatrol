
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public float speed = 5f;
    public float offsetToPlayerDistance = 1f;
    [SerializeField] Transform patrolPoints;
    [SerializeField] float checkNodeDistance = 0.5f;
    [SerializeField] Enemy current;

    DirectedGraph<Transform> directedGraph;

    int[] nodeOrder;

    Transform currentPatrolPos;
   

    bool shouldDrawGizmos = false;

    

    void Awake()
    {
        nodeOrder = new int[patrolPoints.childCount];
        directedGraph = new DirectedGraph<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize node order
        for(int i = 0; i < patrolPoints.childCount; i++)
        {
            nodeOrder[i] = i;
        }

        // Shuffle node order
        for (int t = 0; t < nodeOrder.Length; t++)
        {
            int tmp = nodeOrder[t];
            int r = Random.Range(t, nodeOrder.Length);
            nodeOrder[t] = nodeOrder[r];
            nodeOrder[r] = tmp;
        }

        string printOrder = "";

        // Print Node Order
        for (int i = 0; i < patrolPoints.childCount; i++)
        {
            printOrder += "  " + nodeOrder[i].ToString();
        }

        Debug.Log("The Node Order is : " + printOrder);

        


        // Add nodes to the graph
        for (int i = 0; i < patrolPoints.childCount; ++i)
        {

            
            directedGraph.AddNode(patrolPoints.GetChild(i));
            
        }

        int lastIndex = 0;
        int currentIndex = nodeOrder[0];

        // Add edges from start to end node
        for(int i = 0; i < nodeOrder.Length - 1; ++i)
        {

            directedGraph.AddEdge(patrolPoints.GetChild(currentIndex), patrolPoints.GetChild(nodeOrder[i + 1]));
            currentIndex = nodeOrder[i + 1];
            lastIndex = currentIndex;
        }

        // Add edge from end node to the start node
        directedGraph.AddEdge(patrolPoints.GetChild(lastIndex), patrolPoints.GetChild(nodeOrder[0]));

        shouldDrawGizmos = true;

        currentPatrolPos = directedGraph.FindNode(patrolPoints.GetChild(nodeOrder[0])).GetData();

        Debug.Log("Graph instantiated with : " + directedGraph.Size() + " nodes");
        Debug.Log("Graph instantiated with : " + directedGraph.edgeCount + " edges");
    }

    // Update is called once per frame
    void Update()
    {
        switch(current.currentState)
        {
            case Enemy.State.Patrol:
                Patrolling();
                break;
            case Enemy.State.Chase:
                Chasing();
                break;
        }

        
    }

    void Patrolling()
    {
        // Move the enemy from one point to another
        // transform.position = Vector3.Lerp(transform.position, currentPatrolPos.position, Time.deltaTime);

        transform.position = Vector3.MoveTowards(transform.position, currentPatrolPos.position, Time.deltaTime * speed);

        Debug.DrawLine(transform.position, currentPatrolPos.position, Color.yellow);

        // Store the currently moving position in a variable so as to resume
        // after enemy has finished chasing the player
        if (Vector3.Distance(transform.position, currentPatrolPos.position) < checkNodeDistance)
        {
            if (directedGraph.FindNode(currentPatrolPos).GetOutgoing().Count > 0)
                currentPatrolPos = directedGraph.FindNode(currentPatrolPos).GetOutgoing()[0].GetData();
        }
    }

    void Chasing()
    {
        if(Vector3.Distance(transform.position, current.playerTransform.position) > offsetToPlayerDistance)
            transform.position = Vector3.MoveTowards(transform.position, current.playerTransform.position, Time.deltaTime * speed);
    }

    void OnDrawGizmos()
    {
        if (!shouldDrawGizmos)
            return;

        int lastIndex = 0;
        int currentIndex = nodeOrder[0];

        for (int i = 0; i < nodeOrder.Length - 1; ++i)
        {
            
                Debug.DrawLine(patrolPoints.GetChild(currentIndex).position, patrolPoints.GetChild(nodeOrder[i + 1]).position, Color.white);
                
                currentIndex = nodeOrder[i + 1];
                lastIndex = currentIndex;
          
        }

        Debug.DrawLine(patrolPoints.GetChild(lastIndex).position, patrolPoints.GetChild(nodeOrder[0]).position, Color.white);
    }
}
