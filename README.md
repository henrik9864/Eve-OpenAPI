# Eve-OpenAPI
A library for accessing EVE online's ESI api.

## Installation
Get the latest version on nuget: https://www.nuget.org/packages/Eve-OpenApi/ <br />
```
Install-Package Eve-OpenApi -Version 0.4.2
```
Optional <br />
Get SSO support: https://www.nuget.org/packages/Eve-OpenApi.Authentication/ <br />
```
Install-Package Eve-OpenApi.Authentication -Version 0.4.2
```

## Setup Login

Eve-OpenAPI does not require a login for it to work on endpoints that does not require a scope so this step is optional dont plan on using any. For programs you plan on distributing Eve-OpenAPI does not require a client secret. Read more at https://github.com/esi/esi-docs.

#### Standard eve login.
```cs
string ClientID = "your eve developer client id";
string Callback = "client callback url";

// Create a preconfigured login for EVE Tranquility SSO.
// Other preconfigurations and base classes to setup custom login can be found here.
ILogin login = await new LoginBuilder().Eve
  .WithCredentials(ClientID, Callback)
  .Build();
```
#### Custom oauth login.
If you need a custom login for your api you can customize it with a ILoginConfig. Eve-OpenAPI comes with two login solutions, OAuth and key. OAuth is the standard OAuth protocol for creating and using access tokens and key is where you manually get a key from the api.
```cs
string ClientID = "your eve developer client id";
string Callback = "client callback url";
ILoginConfig Config = new LoginConfig();

ILogin login = await new LoginBuilder(Config).OAuth
  .WithCredentials(ClientID, Callback)
  .Build();
```
#### Adding tokens with oauth.
If you want the library to open the url for you do this.
```cs
IScope Scope = (Scope)"<esi scope>";
await login.AddToken(Scope);
```
If you want a custom way to give users the auth url with oauth. This method will setup a web listener in the background to handle the response.
```cs
IScope Scope = (Scope)"<esi scope>";
string url = await login.GetAuthUrl(Scope); // Login must be of type IOauthLogin

// Do stuff with url
```
#### Adding tokens with key.
Adding a key is as simple as building the login and giving it a key
```cs
IKeyLogin login = new LoginBuilder().Key
				.Build();

// User is reccomended to keep as the default user set in the API config.
login.AddKey("<Key from API>", "<User>", "<Scope>");
```

#### Saving and loading tokens with OAuth.
```cs
string ClientID = "your eve developer client id";
string Callback = "client callback url";
string SaveFile = "Path to savefile";
bool Override = True;

// NOTE. Make sure you dont load encrypted files as raw text
ILogin login = await new LoginBuilder()
  .WithCredentials(ClientID, Callback)
  .FromFileEncrypted(Savefile)
  .Build();

// Will save and encrypted file with a refresh token for all added tokens.
login.SaveToFileEncrypted(SaveFile, Override);
```
## Setup API
#### Creating API config
To have the API pull from ESI you have to setup its config to do so. This is also where you supply your user agent and feault user. The library will not work without the user agent being set.
```cs
IApiConfig config = new EsiConfig()
{
  UserAgent = "Your cool user agent",
};
```
If you want to pull from an OpenAPI with no preset included you can create your own custom api config.
```cs
IApiConfig config = new ApiConfig()
{
  UserAgent = "Your cool user agent",
  SpecURL = "Link to the swagger spec v2 or v3",
  TokenLocation = "Where your oauth token are put", // This can be one of two values header or query
  TokenName = "Name of the token parameter"
};
```
#### Build the API
Once you have created the config you can now build the api. The login can be omitted if you dont need it.
```cs
IAPI api = new ApiBuilder(config, login).Build();
```
## Get data from the API
This example is shown using ESI but the interface works the same way for all API's.
```cs
// First you must select a path, this path will be validated to make sure you are using the right EsiVersion
IApiPath path = api.Path("/characters/{character_id}/mail/");
IApiResponse response = await api.Get(("character_id", "character id"));

// If you have a class for the response you can also specify that in the request.
// You can also set a other user to ovveride DefaultUser
IApiResponse<T> response = await api.SetUsers("Character ID").Get<T>(("character_id", "character id"));

// If you want to do a batch request to for multiple values use GetBatch
// This also works with characters
List<object> CharacterIDs = new List<object>() {"character id", "character id"};
List<string> EveCharacterIDs = new List<string>() {"Character ID", "Character ID"};
Lis<IApiResponse<T>> response = await path.SetUsers(EveCharacterIDs).GetBatch<T>(("character_id", CharacterIDs));
```
### Interacting with the data
```cs
// First you must select a path, this path will be validated to make sure you are using the right EsiVersion
IApiPath path = api.Path("/characters/{character_id}/mail/");
IApiResponse response = await path.Get(("character_id", "character id");

// If you response does not have any pagination or you only want the first page
response.FirstPage

// IApiResponse extends IEnumerable so can access each page by enumerating over the response.
foreach (string page in response) // page type deafults to string and will be the same as T
{

}

// If you want to flatten the pages into one item you can use System.Linq
response.Aggregate((a, b) => a + b);
```
### Listening for events
This library currrently comes with two events OnChange and OnExpire.
```cs
IApiEventMethod method = api.PathEvent("/characters/{character_id}/location/").Get(("character_id", 96037287));
method.OnChange += Change; // Called each time the value for this endpoint changes between each expire
method.OnExpire += Update; // Called each time the data from this enpoint becomes stale

void Update(IApiResponse a1, IApiResponse a2)
{
  Console.WriteLine("Updated location!");
}

void Change(IApiResponse a1, IApiResponse a2)
{
  Console.WriteLine("	Changed location!");
  Console.WriteLine($"	{a1.FirstPage}");
  Console.WriteLine($"	{a2.FirstPage}");
}
```
---

EVE Online © 2019 [CCP hf.](https://www.ccpgames.com/)
