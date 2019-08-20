using EveOpenApi.Api;
using EveOpenApi.Authentication;
using EveOpenApi.Enums;
using EveOpenApi.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Eve-OpenApi.Test")]

namespace EveOpenApi.Managers
{
	public delegate void ApiUpdate(IApiResponse now, IApiResponse old);

	internal class EventManager : BaseManager, IEventManager
	{
		public Dictionary<(int, EventType), ApiUpdate> Events { get; }

		SortedSet<(DateTime time, IApiRequest request)> requests;
		SemaphoreSlim trigger;

		bool backroundRunning = false;

		ICacheManager cacheManager;
		IRequestManager requestManager;

		public EventManager(IHttpHandler client, IApiConfig config, ILogin login, ICacheManager cacheManager, IRequestManager requestManager) : base(client, login, config)
		{
			Events = new Dictionary<(int, EventType), ApiUpdate>();

			requests = new SortedSet<(DateTime, IApiRequest)>(new ApiRequestComparer());
			trigger = new SemaphoreSlim(0, 1);

			this.cacheManager = cacheManager;
			this.requestManager = requestManager;
		}

		/// <summary>
		/// Prep a path for an event subscription.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="eventType"></param>
		/// <param name="path"></param>
		/// <param name="parameters"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		public IEnumerable<IApiRequest> GetRequest(OperationType type, EventType eventType, string path, Dictionary<string, List<object>> parameters, List<string> users, OpenApiOperation operation)
		{
			if (!Config.EnableEventQueue)
				throw new Exception("Events has been disabled. Enable them via the 'EnableEventQueue' property in the config.");

			IEnumerable<IApiRequest> requests = requestManager.GetRequest(path, type, parameters, users, operation);
			foreach (IApiRequest request in requests)
			{
				this.requests.Add((DateTime.Now + new TimeSpan(0, 0, 1), request));

				if (!Events.ContainsKey((request.GetHashCode(), eventType)))
					Events.Add((request.GetHashCode(), eventType), null);
			}

			if (trigger.CurrentCount == 0)
				trigger.Release();

			if (!backroundRunning)
				StartBackgroundLoop();

			return requests;
		}

		/// <summary>
		/// Start background event loop and setup event handling.
		/// </summary>
		void StartBackgroundLoop()
		{
			backroundRunning = true;
			Task.Factory.StartNew(
				Loop,
				CancellationToken.None,
				TaskCreationOptions.LongRunning,
				TaskScheduler.Default
			).Unwrap().ContinueWith(tsk =>
			{
				Console.WriteLine(tsk.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted);
		}

		/// <summary>
		/// Background loop that checks events
		/// </summary>
		/// <returns></returns>
		async Task Loop()
		{
			Task semaphore = trigger.WaitAsync();

			while (true)
			{
				(DateTime time, IApiRequest request) = requests.FirstOrDefault();
				Task waitTask = Task.Delay(-1);

				if (request != default && time.CompareTo(DateTime.Now) != -1)
					waitTask = Task.Delay(time - DateTime.Now + new TimeSpan(0, 0, 1));
				else if (request != default)
					waitTask = Task.Delay(1000);

				await Task.WhenAny(semaphore, waitTask);

				if (waitTask.IsCompleted)
				{
					int index = requests.RemoveWhere(x => x.time == time);
					await ProcessRequest(request);
				}

				if (semaphore.IsCompleted)
					semaphore = trigger.WaitAsync();
			}
		}

		/// <summary>
		/// Check if a request path data has changed.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		async Task ProcessRequest(IApiRequest request)
		{
			cacheManager.TryHitCache(request, false, out IApiResponse old);
			IApiResponse now = await cacheManager.GetResponse(request);

			if (now is ApiError)
				throw new Exception(now.FirstPage);

			TryInvokeEvent(EventType.Update, request.GetHashCode(), now, old);

			if (old != null && now.GetHashCode() != old?.GetHashCode())
				TryInvokeEvent(EventType.Change, request.GetHashCode(), now, old);

			requests.Add((now.Expired, request));
			//return now.Expired;
		}

		void TryInvokeEvent(EventType eventType, int id, IApiResponse now, IApiResponse old)
		{
			if (!Events.TryGetValue((id, eventType), out ApiUpdate apiUpdate))
				return;

			apiUpdate(now, old);
		}
	}

	/// <summary>
	/// Not the pretties but it works
	/// </summary>
	class ApiRequestComparer : IComparer<(DateTime time, IApiRequest request)>
	{
		public int Compare([AllowNull] (DateTime time, IApiRequest request) x, [AllowNull] (DateTime time, IApiRequest request) y)
		{
			return x.time.CompareTo(y.time);
		}
	}
}
