using System.Collections;
using System.Collections.Generic;

public class UndirectedGraph <Data> 
{
	public class Node
	{

		Data data;
		List<Node> neighbours = new List<Node>();

		public Node(Data newData)
		{
			data = newData;
		}

		public Data GetData()
		{
			return data;
		}

		public List<Node> GetNeighbours()
		{
			return neighbours;
		}			
	}

	List<Node> nodes = new List<Node>();

	public Node AddNode(Data data)
	{
		Node node = new Node(data);
		
		nodes.Add(node);
		
		return node;
	}

	public Node FindNode(Data data)
	{
		for (int i=0; i < nodes.Count; i++)
			if (nodes[i].GetData().Equals(data))
				return nodes[i];
		return null; // not found!
	}

	public void AddEdge(Node node1, Node node2)
	{
		if (node1 == null || node2 == null)
			return;
		node1.GetNeighbours().Add(node2);
		node2.GetNeighbours().Add(node1);
	}

	public void AddEdge(Data node1, Data node2)
	{
		AddEdge(FindNode(node1), FindNode(node2));
	}

}
