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
    public ulong player1wins;
    public ulong player2wins;
    public ulong winCondition = 21;
    public ulong totalRolls = 0;
    public ulong unFinishedGames = 0;

    // List<ulong[]> - p1, pos1, score1, p2, pos2, score2, gamestate
    // state 0 = ongoing, 1 = finished
    public List<ulong[]> games = new List<ulong[]>();

    void Start()
    {
        Stopwatch st = new Stopwatch();
        st.Start();
        
        // assign starting values
        games.Add(new ulong[] { 0, 4, 0, 1, 6, 0, 0 });

        // starting games
        RollDieA();
        RollDieB();

        StartCoroutine(extraBRRRR());

        st.Stop();
        UnityEngine.Debug.Log(games.Count);
        UnityEngine.Debug.Log(string.Format("took {0} ms to complete", st.ElapsedMilliseconds));
        //UnityEngine.Debug.Log("Answer: " + (totalRolls * scores[losingPlayer]));
    }

    IEnumerator extraBRRRR()
    {
        while (unFinishedGames > 0)
        {
            RollDieA();
            RollDieB();
            yield return new WaitForEndOfFrame();
        }
    }

    void RollDieA()
    {
        // die: 1 - 1 - 1 = 3
        // die: 1 - 2 - 1 = 4
        // die: 2 - 1 - 1 = 4
        // die: 3 - 1 - 3 = 7
        // die: 2 - 3 - 2 = 7

        // copy current lists before making new lists for the dies, once done apply the new lists to the existing ones
        List<ulong[]> newGames = new List<ulong[]>();
        ulong calc = 0;
        unFinishedGames = 0;
        for (ulong i = 0; i < Convert.ToUInt64(games.Count); i++)
        {
            if (games[(int)i][6] == 1) continue;

            // does 3 new rolls per game which results in final die score being 3-9
            // List<ulong[]> { p1, pos1, score1, p2, pos2, score2, gamestate }
            // final die score = 3
            calc = newVal(games[(int)i][1] + 3, 10);
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            // final die score = 4
            calc = newVal(games[(int)i][1] + 4, 10);
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            // final die score = 5
            calc = newVal(games[(int)i][1] + 5, 10);
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            // final die score = 6
            calc = newVal(games[(int)i][1] + 6, 10);
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            // final die score = 7
            calc = newVal(games[(int)i][1] + 7, 10);
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            // final die score = 8
            calc = newVal(games[(int)i][1] + 8, 10);
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            // final die score = 9
            calc = newVal(games[(int)i][1] + 9, 10);
            newGames.Add(new ulong[] { 0, calc, games[(int)i][2] + calc, 1, games[(int)i][4], games[(int)i][5], 0 });
            totalRolls += 15;
        }
        newGames = EvaluateGamesA(newGames);
        games = newGames;
    }

    void RollDieB()
    {
        // die: 1 - 1 - 1 = 3
        // die: 1 - 2 - 1 = 4
        // die: 2 - 1 - 1 = 4
        // die: 3 - 1 - 3 = 7
        // die: 2 - 3 - 2 = 7

        // copy current lists before making new lists for the dies, once done apply the new lists to the existing ones
        List<ulong[]> newGames = new List<ulong[]>();
        ulong calc = 0;
        unFinishedGames = 0;
        for (ulong i = 0; i < Convert.ToUInt64(games.Count); i++)
        {
            if (games[(int)i][6] == 1) continue;

            // does 3 new rolls per game which results in final die score being 3-9
            // List<ulong[]> { p1, pos1, score1, p2, pos2, score2, gamestate }
            // final die score = 3
            calc = newVal(games[(int)i][4] + 3, 10);
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            // final die score = 4
            calc = newVal(games[(int)i][4] + 4, 10);
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            // final die score = 5
            calc = newVal(games[(int)i][4] + 5, 10);
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            // final die score = 6
            calc = newVal(games[(int)i][4] + 6, 10);
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            // final die score = 7
            calc = newVal(games[(int)i][4] + 7, 10);
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            // final die score = 8
            calc = newVal(games[(int)i][4] + 8, 10);
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            // final die score = 9
            calc = newVal(games[(int)i][4] + 9, 10);
            newGames.Add(new ulong[] { 0, games[(int)i][1], games[(int)i][2], 1, calc, games[(int)i][5] + calc, 0 });
            totalRolls += 15;
        }
        newGames = EvaluateGamesB(newGames);
        games = newGames;
    }

    ulong newVal(ulong val, ulong sub)
    {
        while(val > sub) val -= sub;
        if (val == 0) val = 1;
        return val;
    }

    // check for player 1
    List<ulong[]> EvaluateGamesA(List<ulong[]> gamesToCheck)
    {
        for (ulong i = 0; i < Convert.ToUInt64(games.Count); i++)
        {
            if (gamesToCheck[(int)i][2] > winCondition && gamesToCheck[(int)i][6] == 0)
            {
                player1wins++;
                gamesToCheck[(int)i][6] = 1;
            }
            else
            {
                unFinishedGames++;
            }
        }

        return gamesToCheck;
    }

    // check for player 2
    List<ulong[]> EvaluateGamesB(List<ulong[]> gamesToCheck)
    {
        for (ulong i = 0; i < Convert.ToUInt64(games.Count); i++)
        {
            if (gamesToCheck[(int)i][5] > winCondition && gamesToCheck[(int)i][6] == 0)
            {
                player2wins++;
                gamesToCheck[(int)i][6] = 1;
            }
            else
            {
                unFinishedGames++;
            }
        }

        return gamesToCheck;
    }
}