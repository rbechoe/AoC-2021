using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class day3 : MonoBehaviour
{
    public string[] entries;
    public List<string> oxygen;
    public List<string> scrubber;

    void Start()
    {
        Stopwatch st = new Stopwatch();
        st.Start();

        FileStream fs = new FileStream(Application.dataPath + "/inputs/three.txt", FileMode.Open);
        string content = "";
        using (StreamReader read = new StreamReader(fs, true))
        {
            content = read.ReadToEnd();
        }
        entries = Regex.Split(content, "\r\n?|\n", RegexOptions.Singleline);

        // part 1
        /*string gammaRate = "";
        string epsilonRate = "";
        
        for (int i = 0; i < entries[0].Length; i++)
        {
            int trueCount = 0;
            for (int j = 0; j < entries.Length; j++)
            {
                if (int.Parse(entries[j][i].ToString()) == 1) trueCount++;
            }
            if (trueCount > entries.Length / 2f)
            {
                gammaRate += "1";
                epsilonRate += "0";
            }
            else
            {
                gammaRate += "0";
                epsilonRate += "1";
            }
        }
        UnityEngine.Debug.Log("Gamma: " + gammaRate + " Epsilon: " + epsilonRate);
        int gammaDec = Convert.ToInt32(gammaRate, 2);
        int epsiDec = Convert.ToInt32(epsilonRate, 2);
        UnityEngine.Debug.Log("Dec A: " + gammaDec + " Dec B: " + epsiDec);
        UnityEngine.Debug.Log("Answer: " + (gammaDec * epsiDec));*/

        // part 2
        oxygen = entries.ToList();
        for (int i = 0; i < entries[0].Length; i++)
        {
            List<string> newOxygen = new List<string>();
            int trueCount = 0;
            for (int j = 0; j < oxygen.Count; j++)
            {
                if (int.Parse(oxygen[j][i].ToString()) == 1) trueCount++;
            }

            if (trueCount == oxygen.Count / 2f)
            {
                for (int j = 0; j < oxygen.Count; j++)
                {
                    if (int.Parse(oxygen[j][i].ToString()) == 1)
                    {
                        newOxygen.Add(oxygen[j]);
                    }
                }
            }

            if (newOxygen.Count == 1)
            {
                oxygen = newOxygen;
                break;
            }

            if (trueCount > oxygen.Count / 2f)
            {
                for (int j = 0; j < oxygen.Count; j++)
                {
                    if (int.Parse(oxygen[j][i].ToString()) == 1)
                    {
                        newOxygen.Add(oxygen[j]);
                    }
                }
            }

            if (trueCount < oxygen.Count / 2f)
            {
                for (int j = 0; j < oxygen.Count; j++)
                {
                    if (int.Parse(oxygen[j][i].ToString()) == 0)
                    {
                        newOxygen.Add(oxygen[j]);
                    }
                }
            }

            oxygen = newOxygen;
        }
        UnityEngine.Debug.Log("Oxygen: " + oxygen[0]);


        scrubber = entries.ToList();
        for (int i = 0; i < entries[0].Length; i++)
        {
            List<string> newScrubber = new List<string>();
            int trueCount = 0;
            for (int j = 0; j < scrubber.Count; j++)
            {
                if (int.Parse(scrubber[j][i].ToString()) == 1) trueCount++;
            }

            if (trueCount == scrubber.Count / 2f)
            {
                for (int j = 0; j < scrubber.Count; j++)
                {
                    if (int.Parse(scrubber[j][i].ToString()) == 0)
                    {
                        newScrubber.Add(scrubber[j]);
                    }
                }
                scrubber = newScrubber;
                continue;
            }

            // no clue why the second if statement doesnt get triggered
            if (i == 8)
            {
                break;
            }
            if (scrubber.Count == 1)
            {
                scrubber = newScrubber;
                break;
            }

            if (trueCount > scrubber.Count / 2f)
            {
                for (int j = 0; j < scrubber.Count; j++)
                {
                    if (int.Parse(scrubber[j][i].ToString()) == 0)
                    {
                        newScrubber.Add(scrubber[j]);
                    }
                }
            }

            if (trueCount < scrubber.Count / 2f)
            {
                for (int j = 0; j < scrubber.Count; j++)
                {
                    if (int.Parse(scrubber[j][i].ToString()) == 1)
                    {
                        newScrubber.Add(scrubber[j]);
                    }
                }
            }

            scrubber = newScrubber;
        }
        UnityEngine.Debug.Log("Scrubber: " + scrubber[0]);

        int oxy = Convert.ToInt32(oxygen[0], 2);
        int scrubby = Convert.ToInt32(scrubber[0], 2);
        UnityEngine.Debug.Log("Answer: " + (oxy * scrubby));

         st.Stop();
        UnityEngine.Debug.Log(string.Format("took {0} ms to complete", st.ElapsedMilliseconds));
    }
}