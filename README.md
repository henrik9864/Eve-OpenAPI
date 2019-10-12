# Eve-OpenAPI
A library for accessing EVE online's ESI api.

## Getting Started

### Installation
Get the latest version on nuget: https://www.nuget.org/packages/Eve-OpenApi/ <br />
```
Install-Package Eve-OpenApi -Version 0.4.0
```
Get AspNet core support: https://www.nuget.org/packages/Eve-OpenApi.DpendencyInjection/ <br />
```
Install-Package Eve-OpenApi.DpendencyInjection -Version 0.4.0
```

### Setup Login

Eve-OpenAPI does not require a login for it to work on endpoints that does not require a scope so this step is optional dont plan on using any. For programs you plan on distributing Eve-OpenAPI does not require a client secret. Read more at https://github.com/esi/esi-docs.

#### Standard eve login.
```cs
string ClientID = "your eve developer client id";
string Callback = "client callback url";

ILogin login = await new LoginBuilder()
  .WithCredentials(ClientID, Callback)
  .BuildEve();
```
<br />

#### Custom oauth login.
If you need a custom login for your api you can customize it with a ILoginConfig
```cs
string ClientID = "your eve developer client id";
string Callback = "client callback url";
ILoginConfig Config = new LoginConfig();

ILogin login = await new LoginBuilder(Config)
  .WithCredentials(ClientID, Callback)
  .BuildOAuth();
```
<br />

#### Adding tokens.
If you want the library to open the url for you do this.
```cs
IScope Scope = (Scope)"<esi scope>";
await login.AddToken(Scope);
```

If you want a custom way to give users the auth url. This method will setup a web listener in the background to handle the response.
```cs
IScope Scope = (Scope)"<esi scope>";
string url = await login.GetAuthUrl(Scope);

// Do stuff with url
```

#### Saving and loading tokens.
```cs
string ClientID = "your eve developer client id";
string Callback = "client callback url";
string SaveFile = "Path to savefile"

ILogin login = await new LoginBuilder()
  .WithCredentials(ClientID, Callback)
  .FromFile(Savefile)
  .BuildEve();

// Will save and encrypted file with a refresh token for all added tokens.
login.SaveToFile(SaveFile, true);
```
### Setup API

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

### Retrive data from the API
This example is shown using ESI but the interface works the same way for all API's.
```cs
// First you must select a path, this path will be validated to make sure you are using the right EsiVersion
IApiPath path = api.Path("/characters/{character_id}/mail/");
IApiResponse response = await api.Get("Character Name", ("character_id", "character id"));

// If you have a class for the response you can also specify that in the request.
IApiResponse<T> response = await api.Get<T>("Character Name", ("character_id", "character id"));

// If you want to do a batch request to for multiple values use GetBatch
List<object> characterIDs = new List<object>() {"character id", "character id"};
Lis<IApiResponse<T>> response = await path.GetBatch<T>("Character Name", ("character_id", characterIDs));
```
---

EVE Online © 2019 [CCP hf.](https://www.ccpgames.com/)
