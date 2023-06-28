using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public static TreeManager Instance;
    
    [SerializeField] private GameObject _nodeItem;

    private List<TreeItem> _trees = new List<TreeItem>();
    private List<NodeItem> _intersectionNodes;
    
    private void Awake()
    {
        Instance = this;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 20), "Clear"))
        {
            _trees.Clear();
            foreach (Transform trans in transform)
            {
                Destroy(trans.gameObject);
            }
        }

        if (GUI.Button(new Rect(10, 50, 100, 20), "Intersection"))
        {
            if (_trees.Count < 2)
            {
                Debug.Log("Need two trees.");
                return;
            }

            _intersectionNodes = new List<NodeItem>();
            if (_trees[0].RootNode.Label == _trees[1].RootNode.Label)
            {
                _intersectionNodes.Add(_trees[0].RootNode);
                FindIntersection(_trees[0].RootNode, _trees[1].RootNode);
            }
        }
    }

    public GameObject CreateNode(NodeItem parentNode = null)
    {
        var clonedItem = Instantiate(_nodeItem, transform);
        var nodeItem = clonedItem.GetComponent<NodeItem>();

        if (parentNode == null)
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 10;
            clonedItem.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            var treeItem = clonedItem.AddComponent<TreeItem>();
            nodeItem.ParentTree = treeItem;
            treeItem.RootNode = nodeItem;
            nodeItem.UpdateLabel();
            _trees.Add(treeItem);
        }
        else
        {
            clonedItem.transform.position = parentNode.transform.position;
            nodeItem.ParentTree = parentNode.ParentTree;
            nodeItem.UpdateLabel();
            parentNode.OnPositionUpdated += nodeItem.OnParentPositionUpdated;
            parentNode.ChildNodes.Add(nodeItem);
        }

        nodeItem.ParentNode = parentNode;

        return clonedItem;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl))
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 10;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Vector2.zero);

            if (hit.collider == null)
            {
                CreateNode();
            }
        }
    }


    private void FindIntersection(NodeItem node1, NodeItem node2)
    {
        foreach (var child1 in node1.ChildNodes)
        {
            foreach(var child2 in node2.ChildNodes)
            {
                if (child1.Label == child2.Label)
                {
                    child1.UpdateLineColor(Color.red);
                    child2.UpdateLineColor(Color.red);

                    // all intersection nodes are saved in _intersectionNodes
                    _intersectionNodes.Add(child1);
                    FindIntersection(child1, child2);
                }
            }
        }
    }
}
