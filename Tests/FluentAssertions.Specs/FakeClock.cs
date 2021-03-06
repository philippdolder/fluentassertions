﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions.Common;

namespace FluentAssertions.Specs
{
    /// <summary>
    /// Implementation of <see cref="IClock"/> for testing purposes only.
    /// </summary>
    /// <remarks>
    /// It allows you to control the "current" time.
    /// </remarks>
    internal class FakeClock : IClock
    {
        private readonly TaskCompletionSource<bool> delayTask = new TaskCompletionSource<bool>();

        private TimeSpan elapsedTime = TimeSpan.Zero;

        Task IClock.DelayAsync(TimeSpan delay, CancellationToken cancellationToken)
        {
            elapsedTime += delay;
            return delayTask.Task;
        }

        bool IClock.Wait(Task task, TimeSpan timeout)
        {
            delayTask.Task.GetAwaiter().GetResult();
            return delayTask.Task.Result;
        }

        public ITimer StartTimer() => new TestTimer(() => elapsedTime);

        public void Delay(TimeSpan timeToDelay)
        {
            elapsedTime += timeToDelay;
        }

        public void Complete()
        {
            // the value is not relevant
            delayTask.SetResult(true);
        }

        public void CompleteAfter(TimeSpan timeSpan)
        {
            Delay(timeSpan);

            // the value is not relevant
            delayTask.SetResult(true);
        }

        public void CompletesBeforeTimeout()
        {
            // the value is only relevant when IClock.Wait is involved
            delayTask.SetResult(true);
        }

        public void RunsIntoTimeout()
        {
            // the value is only relevant when IClock.Wait is involved
            delayTask.SetResult(false);
        }
    }
}
