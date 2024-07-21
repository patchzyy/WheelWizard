using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CT_MKWII_WPF.Utilities.RepeatedTasks;

public abstract class RepeatedTaskManager
{
    private readonly int _intervalSeconds;
    private readonly List<IRepeatedTaskListener> _subscribers = new();
    private DispatcherTimer? _timer;

    // Since we are using DispatcherTimer, we cant use CancellationTokenSource, so we just do it with a bool ¯\(°_o)/¯
    private static bool _globalCancellation;

    protected RepeatedTaskManager(int intervalSeconds)
    {
        _intervalSeconds = intervalSeconds;
    }

    protected void NotifySubscribers()
    {
        foreach (var subscriber in _subscribers)
            subscriber.OnUpdate(this);
    }

    public void Start()
    {
        if (_globalCancellation || _timer != null) return;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(_intervalSeconds)
        };
        _timer.Tick += async (_, _) => await ExecuteAndNotifyAsync();
        _timer.Start();

        // Run the initial execution
        Application.Current.Dispatcher.Invoke(async () => await ExecuteAndNotifyAsync());
    }

    public void Stop()
    {
        _timer?.Stop();
        _timer = null;
    }

    public bool Unsubscribe(IRepeatedTaskListener subscriber) => _subscribers.Remove(subscriber);
    public void Subscribe(IRepeatedTaskListener subscriber)
    {
        if (!_subscribers.Contains(subscriber))
            _subscribers.Add(subscriber);
    }

    private async Task ExecuteAndNotifyAsync()
    {
        if (_globalCancellation)
        {
            Stop();
            return;
        }

        await ExecuteTaskAsync();
        NotifySubscribers();
    }

    protected abstract Task ExecuteTaskAsync();

    // This is used to stop all tasks, regardless of what manager it is, this can be used when the application
    // is closing for example.  It is different then the Stop method, because that only stops the current task.
    public static void CancelAllTasks() => _globalCancellation = true;
}
