//from: http://jacksondunstan.com/articles/3241
using System;
using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;
 
/// <summary>
/// Imposes a limit on the maximum number of coroutines that can be running at any given time. Runs
/// coroutines until the limit is reached and then begins queueing coroutines instead. When
/// coroutines finish, queued coroutines are run.
/// </summary>
/// <author>Jackson Dunstan, http://JacksonDunstan.com/articles/3241</author>
public class CoroutineQueue
{
	/// <summary>
	/// Maximum number of coroutines to run at once
	/// </summary>
	private readonly uint maxActive;
 
	/// <summary>
	/// Delegate to start coroutines with
	/// </summary>
	private readonly Func<IEnumerator,Coroutine> coroutineStarter;
 
	/// <summary>
	/// Queue of coroutines waiting to start
	/// </summary>
	private readonly Queue<IEnumerator> queue;
 
	/// <summary>
	/// Number of currently active coroutines
	/// </summary>
	public uint numActive;
 
	/// <summary>
	/// Create the queue, initially with no coroutines
	/// </summary>
	/// <param name="maxActive">
	/// Maximum number of coroutines to run at once. This must be at least one.
	/// </param>
	/// <param name="coroutineStarter">
	/// Delegate to start coroutines with. Normally you'd pass
	/// <see cref="MonoBehaviour.StartCoroutine"/> for this.
	/// </param>
	/// <exception cref="ArgumentException">
	/// If maxActive is zero.
	/// </exception>
	public CoroutineQueue(uint maxActive, Func<IEnumerator,Coroutine> coroutineStarter)
	{
		if (maxActive == 0)
		{
			throw new ArgumentException("Must be at least one", "maxActive");
		}
		this.maxActive = maxActive;
		this.coroutineStarter = coroutineStarter;
		queue = new Queue<IEnumerator>();
	}
 
	/// <summary>
	/// If the number of active coroutines is under the limit specified in the constructor, run the
	/// given coroutine. Otherwise, queue it to be run when other coroutines finish.
	/// </summary>
	/// <param name="coroutine">Coroutine to run or queue</param>
	public void Run(IEnumerator coroutine)
	{
		if (numActive < maxActive)
		{
			var runner = CoroutineRunner(coroutine);
			coroutineStarter(runner);
		}
		else
		{
			queue.Enqueue(coroutine);
		}
	}
 
	/// <summary>
	/// Runs a coroutine then runs the next queued coroutine (via <see cref="Run"/>) if available.
	/// Increments <see cref="numActive"/> before running the coroutine and decrements it after.
	/// </summary>
	/// <returns>Values yielded by the given coroutine</returns>
	/// <param name="coroutine">Coroutine to run</param>
	private IEnumerator CoroutineRunner(IEnumerator coroutine)
	{
		numActive++;
		while (coroutine.MoveNext())
		{
			yield return coroutine.Current;
		}
		numActive--;
		if (queue.Count > 0)
		{
			var next = queue.Dequeue();
			Run(next);
		}
	}
}