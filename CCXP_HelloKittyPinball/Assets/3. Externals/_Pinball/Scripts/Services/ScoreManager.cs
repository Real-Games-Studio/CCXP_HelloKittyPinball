using UnityEngine;
using System;
using System.Collections;

namespace SgLib
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        public int Score { get; private set; }

        public bool HasNewHighScore { get; private set; }

        public static event Action<int> ScoreUpdated = delegate {};


        void Awake()
        {
            if (Instance)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            Reset();
        }

        public void Reset()
        {
            // Initialize score
            Score = 0;

            // Initialize highscore
            HasNewHighScore = false;
        }

        public void AddScore(int amount)
        {
            Score += amount;

            // Fire event
            ScoreUpdated(Score);
        }
    }
}
