using System.Threading.Tasks;

namespace EveOpenApi.Interfaces
{
	public interface IClientLogin : ILogin
	{
		/// <summary>
		/// Will open a new page in the persons browser.
		/// </summary>
		/// <param name="scope"></param>
		/// <returns></returns>
		Task<IToken> AddToken(IScope scope);

		/// <summary>
		/// Will create an auth url and start a web server listning for a response.
		/// </summary>
		/// <param name="scope"></param>
		/// <returns></returns>
		Task<string> GetAuthUrl(IScope scope);
	}
}
