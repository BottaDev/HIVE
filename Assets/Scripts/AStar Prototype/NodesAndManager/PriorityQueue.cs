using System.Collections.Generic;

public class PriorityQueue
{
    private Dictionary<Node, float> _allNodes = new Dictionary<Node, float>();

    public void Put(Node key, float value)
    {
        if (_allNodes.ContainsKey(key))
            _allNodes[key] = value;
        else
            _allNodes.Add(key, value);
    }

    public int Count()
    {
        return _allNodes.Count;
    }

    public Node Get()
    {
        Node node = null;
        foreach (var item in _allNodes)
        {
            if (node == null)
                node = item.Key;
            if (item.Value < _allNodes[node])
                node = item.Key;
        }
        _allNodes.Remove(node);

        return node;
    }
}
