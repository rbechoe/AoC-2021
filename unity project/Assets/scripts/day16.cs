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
                versionSum = CalculateOperator(hexaValue, versionSum);
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
        for (int j = pathPos + 6; j < pathPos + 6 + packetBits.Length; j += 5)
        {
            string packetGroup = hexaValue.Substring(j + 1, 4);
            totalLit += Convert.ToInt32(packetGroup, 2);
            if (packetBits[j].ToString() == "0")
            {
                UnityEngine.Debug.Log("Total literal " + totalLit);
                iterations += 5;
                break;
            }
            iterations += 5;
        }

        return new int[] { iterations, totalLit };
    }

    int recur = 0;
    int CalculateOperator(string hexaValue, int versionSum)
    {
        int pathPos = 7;
        int iteration = 0;

        int subPacketsLength = 0;
        int curLength = 0; // can not exceed subpacketslength

        int typeBitCounter = 0;
        char[] chars = hexaValue.ToCharArray();
        while (pathPos < chars.Length)
        {
            if (iteration == 0)
            {
                int count = (chars[6].ToString() == "0") ? 15 : 11;
                typeBitCounter = (chars[6].ToString() == "0") ? 0 : 1;
                string val = hexaValue.Substring(pathPos, count);
                subPacketsLength = Convert.ToInt32(val, 2);
                UnityEngine.Debug.Log("packet length " + subPacketsLength);
                pathPos += count;
            }
            else
            {
                if (typeBitCounter == 0)
                {
                    // subPacketsLength defines length in bits -- WORKS CORRECT

                    // SAMPLE: 11010001010
                    // if its 4 (binary) its a literal value
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
                        curLength += pBitsCount;

                        if (curLength == subPacketsLength)
                        {
                            UnityEngine.Debug.Log("Length reached, parsing stopped!");
                            break;
                        }
                        if (curLength > subPacketsLength)
                        {
                            UnityEngine.Debug.Log("Length exceeded sub!");
                            break;
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.Log(":(");
                        break;
                    }
                }
                else
                {
                    // subPacketsLength defines count of amount of packets -- WORKS CORRECT

                    // if its 4 (binary) its a literal value
                    string checkLit = hexaValue.Substring(pathPos + 3, 3);
                    string version = hexaValue.Substring(pathPos, 3);
                    versionSum += Convert.ToInt32(version, 2);
                    if (checkLit == "100")
                    {
                        char[] packetBits = hexaValue.Substring(pathPos + 6).ToCharArray(); // the packets -> 01010
                        int[] answers = CalculateLiteralValue(hexaValue, pathPos);
                        int pBitsCount = answers[0];
                        int totalLit = answers[1];
                        pathPos += pBitsCount;
                        curLength++;

                        if (curLength == subPacketsLength)
                        {
                            UnityEngine.Debug.Log("Count reached, parsing stopped!");
                            break;
                        }
                        if (curLength > subPacketsLength)
                        {
                            UnityEngine.Debug.Log("Count exceeded sub!");
                            break;
                        }
                    }
                    else // operator
                    {
                        recur++;
                        if (recur >= 10) return 0;
                        versionSum = CalculateOperator(hexaValue.Substring(pathPos), versionSum);
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

        return versionSum;
    }
}