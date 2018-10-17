using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public Canvas MainCanvas;
    public Point PointPrefab;
    public Transform GamePanel;
    public Material LineRendererMat;
    public static GameMgr Instance;
    public int pointNum;
    Point[] AllUsePoints;
    private List<Point> pointPoolList= new List<Point>();
    private List<Line> linePoolList = new List<Line>();
    /// <summary>
    /// key值 id小的point id 赋值在 x , 大的赋值给 y 
    /// </summary>
    Dictionary<Vector2Int,Line> linesInUseDic = new Dictionary<Vector2Int,Line>();
    private void Awake()
    {
        Instance = this;
    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.J))
//        {
//            if (IsCross(new Vector2Int(0,1),new Vector2Int(2,3)))
//            {
//                Debug.LogError("shi");
//            }
//            else
//            {
//                Debug.LogError("fou");
//            }
//        }
//    }

    // Use this for initialization
	void Start ()
	{
	    CreatPoints(pointNum);
	}

    public void SetDataOld()
    {
        if(AllUsePoints!=null)
        {
            for(int i = 0;i < AllUsePoints.Length;i++)
            {
                AllUsePoints[i].gameObject.SetActive(false);
                pointPoolList.Add(AllUsePoints[i]);
            }    
        }
        
        foreach(var lineRenderer in linesInUseDic)
        {
            lineRenderer.Value.gameObject.SetActive(false);
            linePoolList.Add(lineRenderer.Value);
        }
        AllUsePoints = null;
        linesInUseDic.Clear();
    }

    private void OnPointMove(Point point)
    {
        foreach(var lineRenderer in point.SelfLinesDic)
        {
            lineRenderer.Value.SetPosition(0,AllUsePoints[lineRenderer.Key.x].Position);
            lineRenderer.Value.SetPosition(1,AllUsePoints[lineRenderer.Key.y].Position);
            foreach (var lineRenderer2 in linesInUseDic)
            {
                if(!point.SelfLinesDic.ContainsValue(lineRenderer2.Value))
                {
                    bool isCross = IsCross(lineRenderer.Key, lineRenderer2.Key);
//                    Debug.LogError("isCross "+isCross+lineRenderer.Key+"  "+lineRenderer2.Key);
                    if (isCross)
                    {
                        if(!lineRenderer.Value.CrossLine.Contains(lineRenderer2.Value))
                        {
                            lineRenderer.Value.CrossLine.Add(lineRenderer2.Value);
                        }
                        if(!lineRenderer2.Value.CrossLine.Contains(lineRenderer.Value))
                        {
                            lineRenderer2.Value.CrossLine.Add(lineRenderer.Value);
                        }
                    }
                    else
                    {
                        if(lineRenderer.Value.CrossLine.Contains(lineRenderer2.Value))
                        {
                            lineRenderer.Value.CrossLine.Remove(lineRenderer2.Value);
                        }
                        if(lineRenderer2.Value.CrossLine.Contains(lineRenderer.Value))
                        {
                            lineRenderer2.Value.CrossLine.Remove(lineRenderer.Value);
                        }
                    }
                }
            }
        }

        foreach (var line in linesInUseDic)
        {
            if (line.Value.CrossLine.Count>0)
            {
                line.Value.SetColor(Color.red);
            }
            else
            {
                line.Value.SetColor(Color.green);
            }
        }       
    }

    private void CreatPoints(int pointNum)
    {
        SetDataOld();
        AllUsePoints = new Point[pointNum];
        for(int i = 0;i < pointNum;i++)
        {
            Point point;
            if(pointPoolList.Count > 0)
            {
                point = pointPoolList[0];
                pointPoolList.Remove(point);
            }
            else
            {
                point = Instantiate(PointPrefab);
            }
            point.Init();
            point.gameObject.SetActive(true);
            point.id = i;
            AllUsePoints[i] = point;
            point.transform.SetParent(GamePanel);
            point.transform.localPosition = Vector3.zero;
            point.transform.localScale = Vector3.one;
            point.OnMove = OnPointMove;
        }
        CreatLine(pointNum);
    }

    private void CreatLine(int pointNum)
    {
        if(pointNum <= 1)
        {
            return;
        }
        AddLineRenderer2InUseList(GetLinePositionByPointNum(pointNum));
        foreach(var lineRenderer in linesInUseDic)
        {
            lineRenderer.Value.SetPositions(new Vector3[] { AllUsePoints[lineRenderer.Key.x].Position,AllUsePoints[lineRenderer.Key.y].Position });
        }
    }


    private void AddLineRenderer2InUseList( List<Vector2Int> posList)
    {
        if(posList==null)
        {
            return;
        }
        foreach (var vector2Int in posList)
        {
            if(vector2Int.x >= vector2Int.y)
            {
                Debug.LogError("startId 要大于 endId 避免两点置换点重复添加！");
                continue;
            }
            Line mLine = GetLineFromPool();
            linesInUseDic.Add(new Vector2Int(vector2Int.x,vector2Int.y),mLine);
            AllUsePoints[vector2Int.x].SelfLinesDic.Add(new Vector2Int(vector2Int.x,vector2Int.y),mLine);
            AllUsePoints[vector2Int.y].SelfLinesDic.Add(new Vector2Int(vector2Int.x,vector2Int.y),mLine);
        }
    }
    private Line GetLineFromPool()
    {
        Line mLineRenderer;
        if(linePoolList.Count > 0)
        {
            mLineRenderer = linePoolList[0];
            mLineRenderer.gameObject.SetActive(true);
            linePoolList.Remove(mLineRenderer);
        }
        else
        {
            GameObject lineRendererGo = new GameObject();
            mLineRenderer = lineRendererGo.AddComponent<Line>();
        }
        mLineRenderer.SetLineMaterial(LineRendererMat);
        return mLineRenderer;
    }
    /// <summary>
    /// 获取所有连线（起点、终点的集合） 
    /// N点做相连的位置
    /// </summary>
    /// <param name="pointNum">点数</param>
    /// <returns></returns>
    private List<Vector2Int> GetLinePositionByPointNum(int pointNum)
    {
        if(pointNum <= 1)
        {
            return null;
        }
        bool largerThan3 = pointNum > 3;
        if(largerThan3)
        {
            //移除最大点至圈内
            pointNum--;
        }
        List<Vector2Int> ret = new List<Vector2Int>();
        for (int i = 0; i < pointNum; i++)
        {
            Vector2Int temp = new Vector2Int(0, i);
            //  做首尾相连
            if (i+1<pointNum)
            {
                ret.Add(new Vector2Int(i,i + 1));
                if (i>1)//起始点0 的左邻居 与 右邻居（下面的最大点）去除  做外部链接
                {
                    ret.Add(temp);
                }
            }
            //移除最大点后的 最大点与起始点相连
            else if(!ret.Contains(temp))//主要是2点的时候 其他不会重复
            {
                ret.Add(temp);
            }
            //大于3点， 将最大点拿出 至于内部 分别于其他点相连
            if(largerThan3)
            {
                ret.Add(new Vector2Int(i,pointNum));
            }
        }
        return ret;
    }


    private bool IsCross(Vector2Int line1,Vector2Int line2)
    {
        //https://blog.csdn.net/qq826309057/article/details/70942061
        //线段1起点
        float line1StartX = AllUsePoints[line1.x].transform.localPosition.x;
        float line1StartY = AllUsePoints[line1.x].transform.localPosition.y;
        //线段1终点
        float line1EndX = AllUsePoints[line1.y].transform.localPosition.x;
        float line1EndY = AllUsePoints[line1.y].transform.localPosition.y;
        //线段2起点
        float line2StartX = AllUsePoints[line2.x].transform.localPosition.x;
        float line2StartY = AllUsePoints[line2.x].transform.localPosition.y;
        //线段2终点
        float line2EndX = AllUsePoints[line2.y].transform.localPosition.x;
        float line2EndY = AllUsePoints[line2.y].transform.localPosition.y;
        if(Mathf.Max(line1StartX,line1EndX)<Mathf.Min(line2StartX,line2EndX)
        || Mathf.Max(line1StartY,line1EndY) < Mathf.Min(line2StartY,line2EndY)
        || Mathf.Max(line2StartX,line2EndX) < Mathf.Min(line1StartX,line1EndX)
        || Mathf.Max(line2StartY,line2EndY) < Mathf.Min(line1StartY,line1EndY)
        )
        {
            return false;
        }
        //向量叉乘可以表示方向，分别以 line2的首尾到line1的起点连线做向量，当这两个向量与line1叉乘小于0时代表 line2的首尾在line1的两侧
        //线段1
        Vector3 v3Line1 = new Vector3(line1EndX-line1StartX,line1EndY-line1StartY,0);
        //线段2起点减去线段1起点
        Vector3 v3Line2Start = new Vector3(line2StartX-line1StartX,line2StartY-line1StartY,0);
        //线段2终点减去线段1起点
        Vector3 v3line2End = new Vector3(line2EndX-line1StartX,line2EndY-line1StartY,0);

        Vector3 v1 = Vector3.Cross(v3Line1,v3Line2Start);
        Vector3 v2 = Vector3.Cross(v3Line1,v3line2End);

        //以上只能判断 线段2 的两点是否在线段1两边， 还需判断 线段1两点是否在线段2两端
        Vector3 v3Line2 = new Vector3(line2EndX - line2StartX,line2EndY - line2StartY,0);
        Vector3 v3Line1Start = new Vector3(line1StartX - line2StartX,line1StartY - line2StartY,0);
        Vector3 v3line1End = new Vector3(line1EndX - line2StartX,line1EndY - line2StartY,0);

        Vector3 v3 = Vector3.Cross(v3Line2,v3Line1Start);
        Vector3 v4 = Vector3.Cross(v3Line2,v3line1End);

        return (v1.z * v2.z < 0) && (v3.z*v4.z<0);
    }
}
