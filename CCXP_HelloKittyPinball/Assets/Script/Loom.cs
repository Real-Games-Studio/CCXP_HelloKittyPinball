using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;
using Script;

/// <summary>
/// Multi-thread helper...
/// </summary>
public class Loom : Singleton<Loom>
{
    /* ============================================================
     * CONSTANTS
     * ============================================================*/
    private const int MAX_THREADS = 8;

    /* ============================================================
     * AUXILIAR MEMBERS
     * ============================================================*/
    private int _numThreads;
    private readonly List<Action> _currentActions = new();
    private readonly List<Action> _actions = new();
    private readonly List<DelayedQueueItem> _delayed = new();
    private readonly List<DelayedQueueItem> _currentDelayed = new();

    /* ============================================================
     * PUBLIC STRUCTS
     * ============================================================*/
    public struct DelayedQueueItem
    {
        public float time;
        public Action action;
    }

    /* ============================================================
     * PUBLIC STATIC FUNCTIONS
     * ============================================================*/
    public static void QueueOnMainThread(Action action, float time = 0f)
    {
        if (time != 0f)
        {
            lock (Instance._delayed)
            {
                Instance._delayed.Add(
                    new DelayedQueueItem { time = Time.time + time, action = action }
                );
            }
        }
        else
        {
            lock (Instance._actions)
            {
                Instance._actions.Add(action);
            }
        }
    }

    public static void RunAsync(Action action)
    {
        while (Instance._numThreads >= MAX_THREADS)
        {
            Thread.Sleep(1);
        }
        Interlocked.Increment(ref Instance._numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, action);
    }

    /* ============================================================
     * UNITY MESSAGES
     * ============================================================*/
    private void Update()
    {
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }

        foreach (Action action in _currentActions)
        {
            action.Invoke();
        }

        lock (_delayed)
        {
            _currentDelayed.Clear();
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            foreach (DelayedQueueItem item in _currentDelayed)
            {
                _delayed.Remove(item);
            }
        }

        foreach (DelayedQueueItem delayed in _currentDelayed)
        {
            delayed.action();
        }
    }

    /* ============================================================
     * PRIVATE STATIC FUNCTIONS
     * ============================================================*/
    private static void RunAction(object action)
    {
        try
        {
            ((Action)action).Invoke();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        finally
        {
            Interlocked.Decrement(ref Instance._numThreads);
        }
    }
}
