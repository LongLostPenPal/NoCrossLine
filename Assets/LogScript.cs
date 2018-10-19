using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class LogScript
{

    static LogScript instance;
    public static LogScript Instance
    {
        get
        {
            if(instance ==null)
            {
                instance = new LogScript();
            }
            return instance;
        }
    }

    public void  WriteLog(object obj)
    {
        string str = JsonConvert.SerializeObject(obj);
              //下面将这个字符串写入本地文本
        StreamWriter sw;
        string path = "Assets/test.txt";
        #if UNITY_ANDROID
         path = Application.persistentDataPath + "/test.txt";
        #endif
        FileInfo t = new FileInfo("Assets/test.txt");
        sw = t.CreateText();
//        if(!t.Exists)
//        {
//            sw = t.CreateText();
//        }
//        else
//        {
//            sw = t.AppendText();
//        }
        sw.Write(str);
        sw.Close();
    }
    public class LogClass
    {
        public int AllPointsNum;
        public int circleNum;
        public int insideAndOutSideNum;
        public int leftNum;
        public List<Vector2Int> pointList;
    }
}
