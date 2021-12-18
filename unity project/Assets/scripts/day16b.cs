using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class day16b : MonoBehaviour
{
    public string[] entries;
    bool minAsigned = false;
    bool firstGreat = false;
    bool firstLess = false;
    bool firstEqual = false;
    int recur = 0;
    int actualPosition = 0;

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
            recur = 0;
            actualPosition = 0;

            for (int j = 0; j < entries[i].Length; j++) // characters
            {
                hexaValue += hexaCoding[entries[i][j].ToString()];
            }

            UnityEngine.Debug.Log(hexaValue);
            string packetVersion = hexaValue.Substring(0, 3);

            versionSum += Convert.ToInt32(packetVersion, 2);
            int[] finalAnswer = CalculateOperator(hexaValue, versionSum);
            versionSum = finalAnswer[1];

            UnityEngine.Debug.Log("Version sum: " + versionSum);
            UnityEngine.Debug.Log("Final answer: " + finalAnswer[2]);
            // correct answer: 911945136934
            // my calculated answer: -946527008
        }

        st.Stop();
        UnityEngine.Debug.Log(string.Format("took {0} ms to complete", st.ElapsedMilliseconds));
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
            iterations += 5;
            if (packetBits[j].ToString() == "0") break;
        }

        //UnityEngine.Debug.Log("lit: " + totalLit);
        return new int[] { iterations, totalLit };
    }

    // part B solution
    int[] CalculateOperator(string hexaValue, int versionSum)
    {
        int operatorType = Convert.ToInt32(hexaValue.Substring(actualPosition + 3, 3), 2); // 1-7
        UnityEngine.Debug.Log("operator type: " + operatorType);
        actualPosition += 7;

        // used for operator comparison
        int compA = 0;
        bool boolA = false;

        // can only take place during recursion
        if (actualPosition > hexaValue.Length)
        {
            UnityEngine.Debug.LogError("Start is bigger than hexa length, recursion " + recur);
            return new int[] { 18, versionSum };
        }

        int iteration = 0;
        int pathUpdate = 0;

        int subPacketsLength = 0;
        int curLength = 0; // can not exceed subpacketslength

        int typeBitCounter = 0;
        char[] chars = hexaValue.ToCharArray();
        int localAns = 0;
        bool localFirst = false;

        while (actualPosition < chars.Length)
        {
            if (iteration == 0)
            {
                int count = (chars[actualPosition - 1].ToString() == "0") ? 15 : 11;
                typeBitCounter = (chars[actualPosition - 1].ToString() == "0") ? 0 : 1;

                if (actualPosition + count >= hexaValue.Length)
                {
                    return new int[] { count, versionSum };
                }

                string val = hexaValue.Substring(actualPosition, count);
                subPacketsLength = Convert.ToInt32(val, 2);
                actualPosition += count;
            }
            else
            {
                // typebit 0 = subPacketsLength defines length in bits
                // typebit 1 = subPacketsLength defines count of amount of packets
                bool incrementSingle = (typeBitCounter == 0) ? false : true;

                if (actualPosition + 6 >= hexaValue.Length)
                {
                    UnityEngine.Debug.LogError("Start is bigger or equal to hexa length");
                    UnityEngine.Debug.Log(actualPosition + "/" + hexaValue.Length);
                    return new int[] { 18, versionSum, compA };
                }

                string checkLit = hexaValue.Substring(actualPosition + 3, 3);
                string version = hexaValue.Substring(actualPosition, 3);
                versionSum += Convert.ToInt32(version, 2);
                if (checkLit == "100") // issa literal~!
                {
                    char[] packetBits = hexaValue.Substring(actualPosition + 6).ToCharArray(); // the packets
                    //UnityEngine.Debug.Log("call literal");
                    int[] answer = CalculateLiteralValue(hexaValue, actualPosition);
                    actualPosition += answer[0];

                    // PART B PART
                    switch(operatorType)
                    {
                        case 0:
                            localAns += answer[1];
                            break;

                        case 1:
                            localAns = (localAns == 0) ? answer[1] : localAns *= answer[1];
                            break;

                        case 2:
                            if (!localFirst)
                            {
                                localAns = answer[1];
                                localFirst = true;
                            }
                            if (answer[1] < localAns) localAns = answer[1];
                            break;

                        case 3:
                            if (answer[1] > localAns) localAns = answer[1];
                            break;

                        case 5:
                            if (!localFirst)
                            {
                                localAns = answer[1];
                                localFirst = true;
                            }
                            else
                            {
                                localAns = (localAns > answer[1]) ? 1 : 0;
                            }
                            break;

                        case 6:
                            if (!localFirst)
                            {
                                localAns = answer[1];
                                localFirst = true;
                            }
                            else
                            {
                                localAns = (localAns < answer[1]) ? 1 : 0;
                            }
                            break;

                        case 7:
                            if (!localFirst)
                            {
                                localAns = answer[1];
                                localFirst = true;
                            }
                            else
                            {
                                localAns = (localAns == answer[1]) ? 1 : 0;
                            }
                            break;
                    }

                    if (incrementSingle)
                        curLength++;
                    else
                        curLength += answer[0];

                    if (curLength >= subPacketsLength)
                    {
                        return new int[] { pathUpdate, versionSum, localAns };
                    }
                }
                else // issa operator
                {
                    recur++;
                    if (recur >= 500)
                    {
                        UnityEngine.Debug.LogError("Recursion loop version " + versionSum);
                        return new int[] { pathUpdate, versionSum };
                    }

                    UnityEngine.Debug.Log("call recursion from type " + operatorType);
                    int[] answer = CalculateOperator(hexaValue, versionSum);
                    actualPosition += answer[0];
                    versionSum = answer[1];
                    //UnityEngine.Debug.Log("answer " + answer[2]);

                    // PART B PART
                    switch (operatorType)
                    {
                        case 0:
                            compA += answer[2];
                            break;

                        case 1:
                            compA = (compA == 0) ? answer[2] : compA *= answer[2];
                            break;

                        case 2:
                            if (!boolA)
                            {
                                compA = answer[2];
                                boolA = true;
                            }
                            if (answer[2] < compA) compA = answer[2];
                            break;

                        case 3:
                            if (answer[2] > compA) compA = answer[2];
                            break;

                        case 5:
                            if (!boolA)
                            {
                                compA = answer[2];
                                boolA = true;
                            }
                            else
                            {
                                compA = (compA > answer[2]) ? 1 : 0;
                            }
                            break;

                        case 6:
                            if (!boolA)
                            {
                                compA = answer[2];
                                boolA = true;
                            }
                            else
                            {
                                compA = (compA < answer[2]) ? 1 : 0;
                            }
                            break;

                        case 7:
                            if (!boolA)
                            {
                                compA = answer[2];
                                boolA = true;
                            }
                            else
                            {
                                compA = (compA == answer[2]) ? 1 : 0;
                            }
                            break;
                    }

                    UnityEngine.Debug.Log("comp " + compA + " recur " + recur);
                }
            }
            iteration++;

            if (iteration > 500)
            {
                UnityEngine.Debug.Log("Hard quit");
                break;
            }
        }

        return new int[] { pathUpdate, versionSum, compA };
    }
}