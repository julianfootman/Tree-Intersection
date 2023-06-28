using System.Collections.Generic;
using UnityEngine;

public class TreeItem : MonoBehaviour
{
    public NodeItem RootNode
    {
        get { return _rootNodeItem; }
        set { _rootNodeItem = value; }
    }

    private List<string> _nodeLetters;
    private NodeItem _rootNodeItem;

    private void Awake()
    {
        _nodeLetters = new List<string>();
        for (int i = 0; i < 26; i++)
        {
            _nodeLetters.Add(char.ConvertFromUtf32(65 + i));
        }
    }

    public string GetNextLetter(string currentletter)
    {
        string tmpString = _nodeLetters[0];
        _nodeLetters.RemoveAt(0);
        if (currentletter != string.Empty)
        {
            _nodeLetters.Insert(0, currentletter);
        }
        return tmpString;
    }
}
