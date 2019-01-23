using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EsiNet
{
	internal class RequestQueueAsync<T1, T2>
	{
		Dictionary<int, TaskCompletionSource<T2>> requestDone;
		Queue<(int id, T1 request)> requestQueue;

		Func<T1, Task<T2>> processMethod;
		SemaphoreSlim requestAdded;
		int requestId;

		public RequestQueueAsync(Func<T1, Task<T2>> processMethod)
		{
			requestAdded = new SemaphoreSlim(0, 1);
			requestQueue = new Queue<(int id, T1 item)>();
			requestDone = new Dictionary<int, TaskCompletionSource<T2>>();
			this.processMethod = processMethod;

			Loop();
		}

		async void Loop()
		{
			requestAdded = new SemaphoreSlim(0, 1);

			while (true)
			{
				if (requestQueue.Count == 0)
				{
					requestId = 0;
					await requestAdded.WaitAsync();
				}

				var item = requestQueue.Dequeue();
				var response = await processMethod(item.request);

				requestDone[item.id].SetResult(response);
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

			requestQueue.Enqueue((requestId, request));
			if (!requestDone.TryAdd(requestId, tcs))
				requestDone[requestId] = tcs;

			requestAdded.Release();
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
	}
}
