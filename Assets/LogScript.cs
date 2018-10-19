using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;//https://blog.csdn.net/qq992817263/article/details/43539809
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
         FileInfo t = new FileInfo(path);
        sw = t.CreateText();
        sw.Write(str);
        sw.Close();
    }

    public LogClass ReadLog()
    {
        LogClass ret = null;
        string path = Application.dataPath+"/test.txt";
        #if UNITY_ANDROID
//         path = Application.persistentDataPath + "/test.txt";
        #endif
        FileStream t = File.Open(path,FileMode.Open);
        if (t!=Stream.Null)
        {
            int fsLen = (int)t.Length;
            byte[] heByte = new byte[fsLen];
            t.Read(heByte,0,heByte.Length);
            string myStr = System.Text.Encoding.UTF8.GetString(heByte);
            ret = JsonConvert.DeserializeObject<LogClass>(myStr);
        }
        return ret;
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
