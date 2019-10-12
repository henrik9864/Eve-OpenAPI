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

### Example

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
#### Setup ESI specific API
It is reccomended that you always give a UserAgent, then CCP is less likely to remove your access to ESI. This library will not work without one.
```cs
ApiConfig config = new ApiConfig {
  UserAgent = "Your user agent",
};

// When you create the ESI interface you must specify both version and datasource, Eve-OpenaAPI will then automaticly downlad the spec for that version.
API api = await API.CreateEsi(EsiVersion.Latest, Datasource.Tranquility, login, client, config);
```
#### Setup API
```cs
ApiConfig config = new ApiConfig {
  UserAgent = "Your user agent",
};

// Connect the API interface to an arbitrary api.
API api = API.Create("Specification URL", config: config);

// If the API require authentication you can pass in the login as the second argument, like for example ESI or SeAT.
API api = API.Create("Specification URL", login, config: config);
```
#### Retrive data from API interface
This example is shown using ESI but the interface works the same way for all API's.
```cs
// First you must select a path, this path will be validated to make sure you are using the right EsiVersion
ApiPath path = esi.Path("/characters/{character_id}/mail/");

ApiResponse response = await path.Get("Character Name", ("character_id", "character id"));

// If you have a class for the response you can also specify that in the request.
ApiResponse<T> response = await path.Get<T>("Character Name", ("character_id", "character id"));

// If you want to do a batch request to for multiple characters use GetBatch
List<object> characterIDs = new List<object>() {"character id", "character id"};
Lis<ApiResponse<T>> response = await path.GetBatch<T>("Character Name", ("character_id", characterIDs));
```
---

EVE Online © 2019 [CCP hf.](https://www.ccpgames.com/)
