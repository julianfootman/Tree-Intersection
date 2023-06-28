using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeItem : MonoBehaviour
{
    public TreeItem ParentTree 
    { 
        get { return _treeItem; }
        set { _treeItem = value; }
    }

    public string Label => _label.text;

    public NodeItem ParentNode
    {
        get { return _parentItem; }
        set { _parentItem = value; }
    }

    public List<NodeItem> ChildNodes
    {
        get { return _childItems; }
        set { _childItems = value; }
    }

    [SerializeField] private TextMesh _label;

    public Action<Vector3> OnPositionUpdated;
    
    private KeyCode _labelChangeKey = KeyCode.LeftShift;
    private KeyCode _cloneKey = KeyCode.LeftControl;
    private GameObject _clonedItem;
    private LineRenderer _clonedLineRenderer;
    private Vector3 _originMousePosition;

    // parent tree
    private TreeItem _treeItem;
    private NodeItem _parentItem;
    private List<NodeItem> _childItems = new List<NodeItem>();


    private void GenerateChildNode()
    {
        _clonedItem = TreeManager.Instance.CreateNode(this);
        _clonedLineRenderer = _clonedItem.AddComponent<LineRenderer>();
        _clonedLineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));        
        _clonedLineRenderer.positionCount = 2;
        _clonedLineRenderer.startWidth = 0.1f;
        _clonedLineRenderer.endWidth = 0.1f;
        _clonedLineRenderer.startColor = Color.black;
        _clonedLineRenderer.endColor = Color.black;
        _clonedLineRenderer.sortingOrder = -1;
        _clonedLineRenderer.SetPosition(0, transform.position);
    }

    public void UpdateLineColor(Color newColor)
    {
        var lineRenderer =  GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.startColor = newColor;
            lineRenderer.endColor = newColor;
        }
    }

    public void UpdateLabel()
    {
        _label.text = _treeItem.GetNextLetter(_label.text);
    }

    private void OnMouseDown()
    {
        if (Input.GetKey(_labelChangeKey))
        {
            UpdateLabel();
        }
        else if (Input.GetKey(_cloneKey))
        {
            GenerateChildNode();
        }
        else
        {
            _originMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void OnParentPositionUpdated(Vector3 updatedPosition)
    {
        var lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, updatedPosition);
        }
    }

    private void OnMouseDrag()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = 10;

        if (Input.GetKey(_labelChangeKey))
        {

        }
        else if(Input.GetKey(_cloneKey))
        {
            if (_clonedItem == null)
            {
                GenerateChildNode();
            }
            _clonedItem.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            _clonedLineRenderer.SetPosition(1, _clonedItem.transform.position);
        }
        else
        {
            transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            OnPositionUpdated?.Invoke(transform.position);
            var lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(1, transform.position);
            }
        }
    }

    private void OnMouseUp()
    {
        if (_clonedItem != null)
        {
            _clonedItem = null;
        }

        if (_clonedLineRenderer  != null)
        {
            _clonedLineRenderer = null;
        }
    }
}
