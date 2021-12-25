using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System;

public class day21b : MonoBehaviour
{
    public ulong player1wins = 0;
    public ulong player2wins = 0;
    public int winCondition = 21;
    public int startP1 = 4;
    public int startP2 = 8;

    Dictionary<int, ulong> possibleTotals = new Dictionary<int, ulong>();

    void Start()
    {
        Stopwatch st = new Stopwatch();
        st.Start();

        // starting games
        RollOutcomes();
        PlayGame(0, 0, startP1, startP2, 1, 1);

        UnityEngine.Debug.Log(player1wins);
        UnityEngine.Debug.Log(player2wins);

        st.Stop();
        UnityEngine.Debug.Log(string.Format("took {0} ms to complete", st.ElapsedMilliseconds));
    }

    void RollOutcomes()
    {
        // all possible rolls per die, some totals happen more often
        // eg 2+2+1 = 1+3+1 = 1+2+2 etc.
        for (int die1 = 1; die1 <= 3; die1++)
        {
            for (int die2 = 1; die2 <= 3; die2++)
            {
                for (int die3 = 1; die3 <= 3; die3++)
                {
                    int totalRoll = die1 + die2 + die3;
                    if (possibleTotals.ContainsKey(totalRoll))
                    {
                        possibleTotals[totalRoll] += 1;
                    }
                    else
                    {
                        possibleTotals.Add(totalRoll, 1);
                    }
                }
            }
        }
    }

    // recursive games
    void PlayGame(int p1points, int p2points, int p1pos, int p2pos, int turn, ulong games)
    {
        if (p1points > winCondition || p2points > winCondition) return;

        if (turn == 1)
        {
            // player 1
            foreach (KeyValuePair<int, ulong> roll in possibleTotals)
            {
                int newPoints = newPos(p1pos, roll.Key);

                if (p1points + newPoints >= winCondition)
                {
                    // player 1 won in all games that have the same rolls
                    player1wins += games * roll.Value;
                }
                else
                {
                    PlayGame(p1points + newPoints, p2points, newPoints, p2pos, 2, roll.Value * games);
                }
            }
        }
        else
        {
            // player 2
            foreach (KeyValuePair<int, ulong> roll in possibleTotals)
            {
                int newPoints = newPos(p2pos, roll.Key);

                if (p2points + newPoints >= winCondition)
                {
                    // player 2 won in all games that have the same rolls
                    player2wins += games * roll.Value;
                }
                else
                {
                    PlayGame(p1points, p2points + newPoints, p1pos, newPoints, 1, roll.Value * games);
                }
            }
        }
    }

    int newPos(int position, int roll)
    {
        int val = position + roll;
        while (val > 10) val -= 10;
        return val;
    }
}