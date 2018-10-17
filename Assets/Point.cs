using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Point : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public int id;
    public Dictionary<Vector2Int,Line> SelfLinesDic = new Dictionary<Vector2Int,Line>();

    public Vector3 Position
    {
        get { return transform.position; }
        set
        {
            transform.position = value;
            if(OnMove!=null)
            {
                OnMove(this);
            }
        }
    }
    public Action<Point> OnMove;

    public void Init()
    {
        id = -1;
        OnMove = null;
        SelfLinesDic.Clear();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
//        Debug.LogError(Input.mousePosition);
//        Debug.LogError(transform.localPosition);
//        Debug.LogError(RectTransformUtility.WorldToScreenPoint(Camera.main,transform.position));
//        Vector3 pos = new Vector3();
//        RectTransformUtility.ScreenPointToWorldPointInRectangle(MCanvas.transform as RectTransform, Input.mousePosition,
//            Camera.main,out pos);
//        Debug.LogError(""+pos);
//        Debug.LogError(transform.position);
    }

    Vector3 _pos;
    public void OnDrag(PointerEventData eventData)
    {
        var changePos= RectTransformUtility.ScreenPointToWorldPointInRectangle(GameMgr.Instance.MainCanvas.transform as RectTransform,Input.mousePosition,
                Camera.main,out _pos);
        if (changePos)
        {
            this.Position = _pos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
