using System.Collections;
using System.Collections.Generic;

public class DirectedGraph <Data>
{
	public class Node
	{
		Data data;
		List<Node> incoming = new List<Node>();
		List<Node> outgoing = new List<Node>();

		public Node(Data newData)
		{
			data = newData;
		}

		public Data GetData()
		{
			return data;
		}

		public List<Node> GetIncoming()
		{
			return incoming;
		}

		public List<Node> GetOutgoing()
		{
			return outgoing;
		}
			
	}

	List<Node> nodes = new List<Node>();

	public int edgeCount = 0;

	public Node AddNode(Data data)
	{
		Node node = new Node(data);

		nodes.Add(node);
			
		return node;
	}

	public Node FindNode(Data data)
	{
		for (int i = 0; i < nodes.Count; i++)
			if (nodes[i].GetData().Equals(data))
				return nodes[i];

		return null; 
	}

	public bool AddEdge(Node srcNode, Node dstNode)
	{
		if (srcNode == null || dstNode == null)
			return false;

		srcNode.GetOutgoing().Add(dstNode);
		dstNode.GetIncoming().Add(srcNode);
		edgeCount++;
		return true;
	}

	public bool AddEdge(Data src, Data dst)
	{
		return AddEdge(FindNode(src), FindNode(dst));
	}

	

	public int Size()
	{
		return nodes.Count;
	}


}
