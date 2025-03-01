﻿namespace Scoreboard.Templates.DigitSets;

public class FancyDigitSet : IDigitSet
{
    public Dictionary<int, int[,]> GetDigitPattern()
    {
        return new Dictionary<int, int[,]>
        {
            {
                0, new int[,]
                {
                    { 0, 1, 1, 0 },
                    { 1, 0, 0, 1 },
                    { 1, 0, 0, 1 },
                    { 1, 0, 0, 1 },
                    { 1, 0, 0, 1 },
                    { 1, 0, 0, 1 },
                    { 0, 1, 1, 0 },
                }
            },
            {
                1, new int[,]
                {
                    { 0, 0, 1, 0 },
                    { 0, 1, 1, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 0, 1, 0 },
                    { 0, 1, 1, 1 },
                }
            },
            {
                2, new int[,]
                {
                    { 0, 1, 1, 0 },
                    { 1, 0, 0, 1 },
                    { 0, 0, 0, 1 },
                    { 0, 0, 1, 0 },
                    { 0, 1, 0, 0 },
                    { 1, 0, 0, 0 },
                    { 1, 1, 1, 1 },
                }
            },
            {
                3, new int[,]
                {
                    { 0, 1, 1, 0 },
                    { 1, 0, 0, 1 },
                    { 0, 0, 0, 1 },
                    { 0, 1, 1, 0 },
                    { 0, 0, 0, 1 },
                    { 1, 0, 0, 1 },
                    { 0, 1, 1, 0 },
                }
            },
            {
                4, new int[,]
                {
                    { 0, 0, 0, 1 },
                    { 0, 0, 1, 1 },
                    { 0, 1, 0, 1 },
                    { 1, 1, 1, 1 },
                    { 0, 0, 0, 1 },
                    { 0, 0, 0, 1 },
                    { 0, 0, 0, 1 },
                }
            },
            {
                5, new int[,]
                {
                    { 1, 1, 1, 1 },
                    { 1, 0, 0, 0 },
                    { 1, 0, 0, 0 },
                    { 0, 1, 1, 0 },
                    { 0, 0, 0, 1 },
                    { 1, 0, 0, 1 },
                    { 0, 1, 1, 0 },
                }
            },
            {
                6, new int[,]
                {
                    { 0, 1, 1, 0 },
                    { 1, 0, 0, 1 },
                    { 1, 0, 0, 0 },
                    { 1, 1, 1, 0 },
                    { 1, 0, 0, 1 },
                    { 1, 0, 0, 1 },
                    { 0, 1, 1, 0 },
                }
            },
            {
                7, new int[,]
                {
                    { 1, 1, 1, 1 },
                    { 0, 0, 0, 1 },
                    { 0, 0, 0, 1 },
                    { 0, 0, 1, 0 },
                    { 0, 1, 0, 0 },
                    { 1, 0, 0, 0 },
                    { 1, 0, 0, 0 },
                }
            },
            {
                8, new int[,]
                {
                    { 0, 1, 1, 0 },
                    { 1, 0, 0, 1 },
                    { 1, 0, 0, 1 },
                    { 0, 1, 1, 0 },
                    { 1, 0, 0, 1 },
                    { 1, 0, 0, 1 },
                    { 0, 1, 1, 0 },
                }
            },
            {
                9, new int[,]
                {
                    { 0, 1, 1, 0 },
                    { 1, 0, 0, 1 },
                    { 1, 0, 0, 1 },
                    { 0, 1, 1, 1 },
                    { 0, 0, 0, 1 },
                    { 1, 0, 0, 1 },
                    { 0, 1, 1, 0 },
                }
            },
            {
                -1, new int[,]
                {
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                    { 0, 0, 0, 0 },
                }
            }
        };
    }
}