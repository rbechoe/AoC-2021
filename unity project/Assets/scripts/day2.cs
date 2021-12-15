using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class day2 : MonoBehaviour
{
    public string[] entries;

    void Start()
    {
        Stopwatch st = new Stopwatch();
        st.Start();

        FileStream fs = new FileStream(Application.dataPath + "/inputs/two.txt", FileMode.Open);
        string content = "";
        using (StreamReader read = new StreamReader(fs, true))
        {
            content = read.ReadToEnd();
        }
        entries = Regex.Split(content, "\r\n?|\n", RegexOptions.Singleline);

        // logic here
        int horPos = 0;
        int depthPos = 0;
        int aim = 0;

        for (int i = 0; i < entries.Length; i++)
        {
            string val = entries[i];
            if (val.Contains("forward"))
            {
                val = val.Replace("forward", "");
                horPos += int.Parse(val);
                depthPos += aim * int.Parse(val);
            }
            if (val.Contains("up"))
            {
                val = val.Replace("up", "");
                aim -= int.Parse(val);
            }
            if (val.Contains("down"))
            {
                val = val.Replace("down", "");
                aim += int.Parse(val);
            }
        }
        UnityEngine.Debug.Log("Hor: " + horPos + " Depth: " + depthPos + " Aim: " + aim + " Total: " + (horPos * depthPos));

        st.Stop();
        UnityEngine.Debug.Log(string.Format("took {0} ms to complete", st.ElapsedMilliseconds));
    }
}