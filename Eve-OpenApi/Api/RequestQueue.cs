using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EveOpenApi.Api
{
	internal class RequestQueueAsync<T1, T2>
	{
		Dictionary<int, TaskCompletionSource<T2>> requestDone;
		Queue<(int id, T1 request)> requestQueue;

		Func<T1, Task<T2>> processMethod;

		SemaphoreSlim requestAdded;
		int requestId;

		CancellationTokenSource cancelSource;
		Task loopTask;

		public RequestQueueAsync(Func<T1, Task<T2>> processMethod)
		{
			requestAdded = new SemaphoreSlim(0, 1);
			requestQueue = new Queue<(int id, T1 item)>();
			requestDone = new Dictionary<int, TaskCompletionSource<T2>>();
			cancelSource = new CancellationTokenSource();
			this.processMethod = processMethod;

			Task<Task> task = Task.Factory.StartNew(
				Loop,
				cancelSource.Token,
				TaskCreationOptions.LongRunning,
				TaskScheduler.Default
			);

			loopTask = task.Unwrap();
		}

		async Task Loop()
		{
			while (true)
			{
				if (requestQueue.Count == 0 || requestAdded.CurrentCount == 1)
					await requestAdded.WaitAsync();

				if (cancelSource.IsCancellationRequested)
					break;

				(int id, T1 request) item;
				lock (requestQueue)
				{
					item = requestQueue.Dequeue();
				}

				try
				{
					var response = await processMethod(item.request);
					requestDone[item.id].SetResult(response);
				}
				catch (Exception e)
				{
					requestDone[item.id].SetException(e);
				}
			}
		}

		/// <summary>
		/// Add request to be processed once completed.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public int AddRequest(T1 request)
		{
			var tcs = new TaskCompletionSource<T2>();

			lock (requestAdded)
			lock (requestQueue)
			{
				requestQueue.Enqueue((requestId, request));
				if (!requestDone.TryAdd(requestId, tcs))
					requestDone[requestId] = tcs;

				if (requestQueue.Count <= 1)
					requestAdded.Release();
			}

			requestId++;
			return requestId - 1;
		}

		/// <summary>
		/// Wait for response to finish.
		/// </summary>
		/// <param name="id">Response id.</param>
		/// <returns></returns>
		public Task<T2> AwaitResponse(int id)
		{
			return requestDone[id].Task;
		}

		/// <summary>
		/// Stop request loop and wait for loop to stop.
		/// </summary>
		/// <returns></returns>
		public Task Stop()
		{
			cancelSource.Cancel();

			if (requestQueue.Count == 1)
				requestAdded.Release();

			return loopTask;
		}
	}
}
