﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DotNext.Threading;

using Tasks.Pooling;
using LinkedValueTaskCompletionSource = Tasks.LinkedValueTaskCompletionSource<bool>;

/// <summary>
/// Represents asynchronous mutually exclusive lock.
/// </summary>
[DebuggerDisplay($"IsLockHeld = {{{nameof(IsLockHeld)}}}")]
public class AsyncExclusiveLock : QueuedSynchronizer, IAsyncDisposable
{
    [StructLayout(LayoutKind.Auto)]
    private struct LockManager : ILockManager<DefaultWaitNode>
    {
        private volatile bool state;

        internal readonly bool Value => state;

        public readonly bool IsLockAllowed => !state;

        public void AcquireLock() => state = true;

        internal void ExitLock() => state = false;

        readonly void ILockManager<DefaultWaitNode>.InitializeNode(DefaultWaitNode node)
        {
            // nothing to do here
        }
    }

    private ValueTaskPool<bool, DefaultWaitNode, Action<DefaultWaitNode>> pool;
    private LockManager manager;

    /// <summary>
    /// Initializes a new asynchronous exclusive lock.
    /// </summary>
    /// <param name="concurrencyLevel">The expected number of concurrent flows.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="concurrencyLevel"/> is less than or equal to zero.</exception>
    public AsyncExclusiveLock(int concurrencyLevel)
    {
        if (concurrencyLevel < 1)
            throw new ArgumentOutOfRangeException(nameof(concurrencyLevel));

        pool = new(OnCompleted, concurrencyLevel);
    }

    /// <summary>
    /// Initializes a new asynchronous exclusive lock.
    /// </summary>
    public AsyncExclusiveLock()
    {
        pool = new(OnCompleted);
    }

    [MethodImpl(MethodImplOptions.Synchronized | MethodImplOptions.NoInlining)]
    private void OnCompleted(DefaultWaitNode node)
    {
        if (node.NeedsRemoval && RemoveNode(node))
            DrainWaitQueue();

        pool.Return(node);
    }

    /// <summary>
    /// Indicates that exclusive lock taken.
    /// </summary>
    public bool IsLockHeld => manager.Value;

    /// <summary>
    /// Attempts to obtain exclusive lock synchronously without blocking caller thread.
    /// </summary>
    /// <returns><see langword="true"/> if lock is taken successfuly; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool TryAcquire()
    {
        ThrowIfDisposed();
        return TryAcquire(ref manager);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private BooleanValueTaskFactory WaitNoTimeout(TimeSpan timeout, CancellationToken token)
        => WaitNoTimeout(ref manager, ref pool, timeout, token);

    /// <summary>
    /// Tries to enter the lock in exclusive mode asynchronously, with an optional time-out.
    /// </summary>
    /// <param name="timeout">The interval to wait for the lock.</param>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns><see langword="true"/> if the caller entered exclusive mode; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    /// <exception cref="PendingTaskInterruptedException">The operation has been interrupted manually.</exception>
    public ValueTask<bool> TryAcquireAsync(TimeSpan timeout, CancellationToken token = default)
        => WaitNoTimeout(timeout, token).Create();

    [MethodImpl(MethodImplOptions.Synchronized)]
    private ValueTaskFactory WaitWithTimeout(TimeSpan timeout, CancellationToken token)
        => WaitWithTimeout(ref manager, ref pool, timeout, token);

    /// <summary>
    /// Enters the lock in exclusive mode asynchronously.
    /// </summary>
    /// <param name="timeout">The interval to wait for the lock.</param>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns>The task representing lock acquisition operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="TimeoutException">The lock cannot be acquired during the specified amount of time.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    /// <exception cref="PendingTaskInterruptedException">The operation has been interrupted manually.</exception>
    public ValueTask AcquireAsync(TimeSpan timeout, CancellationToken token = default)
        => WaitWithTimeout(timeout, token).Create();

    [MethodImpl(MethodImplOptions.Synchronized)]
    private ValueTaskFactory WaitNoTimeout(CancellationToken token)
        => WaitNoTimeout(ref manager, ref pool, token);

    /// <summary>
    /// Enters the lock in exclusive mode asynchronously.
    /// </summary>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns>The task representing lock acquisition operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    /// <exception cref="PendingTaskInterruptedException">The operation has been interrupted manually.</exception>
    public ValueTask AcquireAsync(CancellationToken token = default)
        => WaitNoTimeout(token).Create();

    [MethodImpl(MethodImplOptions.Synchronized)]
    private BooleanValueTaskFactory StealNoTimeout(object? reason, TimeSpan timeout, CancellationToken token)
    {
        Interrupt(reason);
        return WaitNoTimeout(ref manager, ref pool, timeout, token);
    }

    /// <summary>
    /// Interrupts all pending callers in the queue and acquires the lock.
    /// </summary>
    /// <remarks>
    /// <see exception="LockStolenException"/> will be thrown for each suspended caller in the queue.
    /// The method cannot interrupt the caller that has already acquired the lock. If there is no suspended callers
    /// in the queue, this method is equivalent to <see cref="TryAcquireAsync(TimeSpan, CancellationToken)"/>.
    /// </remarks>
    /// <param name="reason">The reason for lock steal.</param>
    /// <param name="timeout">The interval to wait for the lock.</param>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns><see langword="true"/> if the caller entered exclusive mode; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    /// <seealso cref="PendingTaskInterruptedException"/>
    public ValueTask<bool> TryStealAsync(object? reason, TimeSpan timeout, CancellationToken token = default)
        => StealNoTimeout(reason, timeout, token).Create();

    [MethodImpl(MethodImplOptions.Synchronized)]
    private ValueTaskFactory StealWithTimeout(object? reason, TimeSpan timeout, CancellationToken token)
    {
        Interrupt(reason);
        return WaitWithTimeout(ref manager, ref pool, timeout, token);
    }

    /// <summary>
    /// Interrupts all pending callers in the queue and acquires the lock.
    /// </summary>
    /// <remarks>
    /// <see exception="LockStolenException"/> will be thrown for each suspended caller in the queue.
    /// The method cannot interrupt the caller that has already acquired the lock. If there is no suspended callers
    /// in the queue, this method is equivalent to <see cref="TryAcquireAsync(TimeSpan, CancellationToken)"/>.
    /// </remarks>
    /// <param name="reason">The reason for lock steal.</param>
    /// <param name="timeout">The interval to wait for the lock.</param>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns>The task representing lock acquisition operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="TimeoutException">The lock cannot be acquired during the specified amount of time.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    /// <seealso cref="PendingTaskInterruptedException"/>
    public ValueTask StealAsync(object? reason, TimeSpan timeout, CancellationToken token = default)
        => StealWithTimeout(reason, timeout, token).Create();

    [MethodImpl(MethodImplOptions.Synchronized)]
    private ValueTaskFactory StealNoTimeout(object? reason, CancellationToken token)
    {
        Interrupt(reason);
        return WaitNoTimeout(ref manager, ref pool, token);
    }

    /// <summary>
    /// Interrupts all pending callers in the queue and acquires the lock.
    /// </summary>
    /// <remarks>
    /// <see exception="LockStolenException"/> will be thrown for each suspended caller in the queue.
    /// The method cannot interrupt the caller that has already acquired the lock. If there is no suspended callers
    /// in the queue, this method is equivalent to <see cref="TryAcquireAsync(TimeSpan, CancellationToken)"/>.
    /// </remarks>
    /// <param name="reason">The reason for lock steal.</param>
    /// <param name="token">The token that can be used to abort lock acquisition.</param>
    /// <returns>The task representing lock acquisition operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Time-out value is negative.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The operation has been canceled.</exception>
    /// <seealso cref="PendingTaskInterruptedException"/>
    public ValueTask StealAsync(object? reason = null, CancellationToken token = default)
        => StealNoTimeout(reason, token).Create();

    private void DrainWaitQueue()
    {
        Debug.Assert(Monitor.IsEntered(this));

        for (LinkedValueTaskCompletionSource? current = first, next; current is not null; current = next)
        {
            next = current.Next;

            if (!manager.IsLockAllowed)
                break;

            // skip dead node
            if (RemoveAndSignal(current))
            {
                manager.AcquireLock();
                break;
            }
        }
    }

    /// <summary>
    /// Releases previously acquired exclusive lock.
    /// </summary>
    /// <exception cref="SynchronizationLockException">The caller has not entered the lock.</exception>
    /// <exception cref="ObjectDisposedException">This object has been disposed.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Release()
    {
        ThrowIfDisposed();
        if (!manager.Value)
            throw new SynchronizationLockException(ExceptionMessages.NotInLock);

        manager.ExitLock();
        DrainWaitQueue();

        if (IsDisposing && IsReadyToDispose)
            Dispose(true);
    }

    private protected sealed override bool IsReadyToDispose => manager.Value is false && first is null;
}