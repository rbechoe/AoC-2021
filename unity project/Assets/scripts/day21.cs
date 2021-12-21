using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class day21 : MonoBehaviour
{
    public int[] positions;
    public int[] scores;
    public int players = 2;
    public int winCondition = 21;
    public int dieVal = 1;
    public int totalRolls = 0;
    int losingPlayer = 0;

    bool gameDone = false;

    void Start()
    {
        Stopwatch st = new Stopwatch();
        st.Start();

        // logic here
        positions = new int[players];
        scores = new int[players];

        // assign starting positions
        positions[0] = 4;
        positions[1] = 6;

        while (!gameDone)
        {
            RollDie();
        }

        st.Stop();
        UnityEngine.Debug.Log(string.Format("took {0} ms to complete", st.ElapsedMilliseconds));
        UnityEngine.Debug.Log("Answer: " + (totalRolls * scores[losingPlayer]));
    }

    void RollDie()
    {
        for (int i = 0; i < players; i++)
        {
            int val = 0;
            for (int j = 0; j < 3; j++)
            {
                totalRolls++;
                UnityEngine.Debug.Log("Player " + (i + 1) + " rolled " + dieVal);
                val += dieVal;
                dieVal++;
                dieVal = newVal(dieVal, 100);
                if (dieVal == 0) dieVal = 1;
            }

            positions[i] += val;
            positions[i] = newVal(positions[i], 10);
            if (positions[i] == 0) positions[i] = 1;

            scores[i] += positions[i];
            if (CheckForWin(i)) break;
            UnityEngine.Debug.Log("Player " + (i + 1) + " score total " + scores[i]);
        }
    }

    int newVal(int val, int sub)
    {
        while(val > sub)
        {
            val -= sub;
        }
        return val;
    }

    bool CheckForWin(int player)
    {
        if (scores[player] >= winCondition)
        {
            UnityEngine.Debug.Log("Player " + (player + 1) + " won!");
            if (player == 0) losingPlayer = 1;
            gameDone = true;
            return true;
        }
        return false;
    }
}