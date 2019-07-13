using System.Net.Http;
using EveOpenApi.Api;
using EveOpenApi.Enums;
using EveOpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace EveOpenApi
{
	public interface IAPI
	{
		/// <summary>
		/// Default API user to use when no else is supplied.
		/// </summary>
		string DefaultUser { get; }

		/// <summary>
		/// Login to api.
		/// </summary>
		ILogin Login { get; }

		/// <summary>
		/// API specification following the Open API v2 specification.
		/// </summary>
		OpenApiDocument Spec { get; }

		/// <summary>
		/// Change login for this API
		/// </summary>
		/// <param name="login"></param>
		void ChangeLogin(ILogin login);
		
		/// <summary>
		/// Select a path from the API to prefrom an action on.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		IApiPath Path(string path);

		/// <summary>
		/// Select a pat from the API to listen for an event on.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		IApiEventPath PathEvent(string path);
	}
}