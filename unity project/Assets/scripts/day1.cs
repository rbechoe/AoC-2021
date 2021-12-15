using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class day1 : MonoBehaviour
{
    public string[] entries;

    void Start()
    {
        Stopwatch st = new Stopwatch();
        st.Start();

        FileStream fs = new FileStream(Application.dataPath + "/inputs/one.txt", FileMode.Open);
        string content = "";
        using (StreamReader read = new StreamReader(fs, true))
        {
            content = read.ReadToEnd();
        }
        entries = Regex.Split(content, "\r\n?|\n", RegexOptions.Singleline);

        // logic here
        int count = 0;
        // skip 0 since there is no prev data
        for (int i = 1; i < entries.Length - 3; i++)
        {
            // no need to add 1 and 2 since they are the same for both
            if (int.Parse(entries[i + 3]) > int.Parse(entries[i])) count++;
        }
        UnityEngine.Debug.Log("increments: " + count);

        st.Stop();
        UnityEngine.Debug.Log(string.Format("took {0} ms to complete", st.ElapsedMilliseconds));
    }
}