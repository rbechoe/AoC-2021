using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class day16 : MonoBehaviour
{
    public string[] entries;
    bool iterationDone = false;

    void Start()
    {
        Stopwatch st = new Stopwatch();
        st.Start();

        FileStream fs = new FileStream(Application.dataPath + "/inputs/sixteen.txt", FileMode.Open);
        string content = "";
        using (StreamReader read = new StreamReader(fs, true))
        {
            content = read.ReadToEnd();
        }
        entries = Regex.Split(content, "\r\n?|\n", RegexOptions.Singleline);
    
        Dictionary<string, string> hexaCoding = new Dictionary<string, string>();
        hexaCoding.Add("0", "0000");
        hexaCoding.Add("1", "0001");
        hexaCoding.Add("2", "0010");
        hexaCoding.Add("3", "0011");
        hexaCoding.Add("4", "0100");
        hexaCoding.Add("5", "0101");
        hexaCoding.Add("6", "0110");
        hexaCoding.Add("7", "0111");
        hexaCoding.Add("8", "1000");
        hexaCoding.Add("9", "1001");
        hexaCoding.Add("A", "1010");
        hexaCoding.Add("B", "1011");
        hexaCoding.Add("C", "1100");
        hexaCoding.Add("D", "1101");
        hexaCoding.Add("E", "1110");
        hexaCoding.Add("F", "1111");

        for (int i = 0; i < entries.Length; i++) // entries
        {
            string hexaValue = "";
            int versionSum = 0;

            for (int j = 0; j < entries[i].Length; j++) // characters
            {
                hexaValue += hexaCoding[entries[i][j].ToString()];
            }

            UnityEngine.Debug.Log(hexaValue);
            string packetVersion = hexaValue.Substring(0, 3);
            versionSum += Convert.ToInt32(packetVersion, 2);

            // is a literal value
            if (hexaValue.Substring(3, 3) == "100") // -- WORKS CORRECT
            {
                SimpleLiteral(hexaValue);
            }
            else // operator
            {
                versionSum = CalculateOperator(hexaValue, versionSum)[1];
            }

            UnityEngine.Debug.Log("Version sum " + versionSum);
        }

        st.Stop();
        UnityEngine.Debug.Log(string.Format("took {0} ms to complete", st.ElapsedMilliseconds));
    }

    void SimpleLiteral(string hexaValue)
    {
        char[] chars = hexaValue.ToCharArray();
        string totalPacket = "";
        for (int j = 6; j < chars.Length; j += 5) // characters
        {
            string packetGroup = hexaValue.Substring(j + 1, 4);
            totalPacket += packetGroup;
            if (chars[j].ToString() == "0")
            {
                UnityEngine.Debug.Log(totalPacket + " translates to " + Convert.ToInt32(totalPacket, 2));
                break;
            }
        }
    }

    int[] CalculateLiteralValue(string hexaValue, int pathPos)
    {
        char[] packetBits = hexaValue.ToCharArray();
        int iterations = 6;
        int totalLit = 0;

        if (pathPos + 11 > hexaValue.Length)
        {
            return new int[] { iterations, totalLit };
        }

        for (int j = pathPos + 6; j < pathPos + 6 + packetBits.Length; j += 5)
        {
            string packetGroup = "";
            packetGroup = hexaValue.Substring(j + 1, 4);
            totalLit += Convert.ToInt32(packetGroup, 2);
            if (packetBits[j].ToString() == "0")
            {
                iterations += 5;
                break;
            }
            iterations += 5;
        }
        
        return new int[] { iterations, totalLit };
    }

    int recur = 0;
    int[] CalculateOperator(string hexaValue, int versionSum, int offset = 0)
    {
        //UnityEngine.Debug.Log("hex: " + hexaValue);
        int pathPos = 7 + offset;
        UnityEngine.Debug.Log("path start: " + pathPos + " version " + versionSum);

        if (pathPos > hexaValue.Length)
        {
            UnityEngine.Debug.Log("Start is bigger than hexa length, version " + versionSum);
            return new int[] { 18, versionSum };
        }

        //UnityEngine.Debug.Log("recursion: " + recur);
        int iteration = 0;
        int pathUpdate = 0;

        int subPacketsLength = 0;
        int curLength = 0; // can not exceed subpacketslength

        int typeBitCounter = 0;
        char[] chars = hexaValue.ToCharArray();
        while (pathPos < chars.Length && !iterationDone)
        {
            if (iteration == 0)
            {
                int count = (chars[6 + offset].ToString() == "0") ? 15 : 11;
                typeBitCounter = (chars[6 + offset].ToString() == "0") ? 0 : 1;
                string val = hexaValue.Substring(pathPos, count);
                subPacketsLength = Convert.ToInt32(val, 2);
                //UnityEngine.Debug.Log("packet length " + subPacketsLength + " binary " + val);
                pathPos += count;
            }
            else
            {
                // typebit 0 = subPacketsLength defines length in bits
                // typebit 1 = subPacketsLength defines count of amount of packets
                bool incrementSingle = (typeBitCounter == 0) ? false : true;

                // if its 4 (binary) its a literal value
                UnityEngine.Debug.Log(pathPos + "/" + hexaValue.Length);
                if (pathPos + 6 >= hexaValue.Length)
                {
                    UnityEngine.Debug.Log("Start is bigger or equal to hexa length, version " + versionSum);
                    iterationDone = true;
                    return new int[] { 18, versionSum };
                }

                string checkLit = hexaValue.Substring(pathPos + 3, 3);
                string version = hexaValue.Substring(pathPos, 3);
                versionSum += Convert.ToInt32(version, 2);
                if (checkLit == "100") // issa literal~!
                {
                    char[] packetBits = hexaValue.Substring(pathPos + 6).ToCharArray(); // the packets
                    int[] answers = CalculateLiteralValue(hexaValue, pathPos);
                    int pBitsCount = answers[0];
                    int totalLit = answers[1];
                    pathPos += pBitsCount;

                    if (incrementSingle)
                        curLength++;
                    else
                        curLength += pBitsCount;

                    if (curLength >= subPacketsLength)
                    {
                        UnityEngine.Debug.Log("Length reached or exceeded, parsing stopped!");
                        pathUpdate = pBitsCount;
                        break;
                    }
                }
                else // issa operator
                {
                    recur++;
                    if (recur >= 10)
                    {
                        UnityEngine.Debug.Log("Recursion loop version " + versionSum);
                        return new int[] { pathUpdate, versionSum };
                    }

                    if (pathPos < hexaValue.Length)
                    {
                        UnityEngine.Debug.Log("Summoning recursion, current version " + versionSum);
                        int[] answer = CalculateOperator(hexaValue, versionSum, pathPos);
                        pathPos += answer[0];
                        versionSum = answer[1];
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Remaining hexavalue is " + (hexaValue.Length - pathPos) + " version " + versionSum);
                        return new int[] { pathUpdate, versionSum };
                    }
                }
            }
            iteration++;

            if (iteration > 500)
            {
                UnityEngine.Debug.Log("Hard quit");
                break;
            }
        }

        return new int[] { pathUpdate, versionSum };
    }
}