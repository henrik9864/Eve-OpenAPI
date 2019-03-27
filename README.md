# Eve-OpenAPI
A library for accessing EVE online's ESI api.

## Getting Started

### Installation
Get the latest version on nuget: https://www.nuget.org/packages/Eve-OpenApi/ <br />
```
Install-Package Eve-OpenApi -Version 0.1.4
```

### Example

#### Create a EveLogin

Eve-OpenAPI does not require a client secret as it uses the new v2 authorization. It will autmaticly generate a code challenge and a verifier. Read more at https://github.com/esi/esi-docs.
```cs
string ClientID = "your eve developer client id";
string Callback = "client callback url";
string Scope = "esi scopes separated by a space";

EveLogin login = await EveLogin.Login(Scope, ClientID, Callback);
```
<br />

If you need alternative ways to give the user the auth URL, for example on servers.
```cs
string ClientID = "your eve developer client id";
string Callback = "client callback url";
string Scope = "esi scopes separated by a space";

EveLogin login = await EveLogin.Create(ClientID, Callback);
string AuthUrl = await login.Authenticate(Scope);

// Do whatever you want withe URL, here i open it in the browser
OpenUrl(AuthUrl);
```
<br />

Or alternativly if you want to load from a save file.
```cs
string FilePath = "path to your save file";

EveLogin login = await EveLogin.FromFile(FilePath);
```
#### Save EveLogin
There is two ways to save a EveLogin. The SaveToFile method automaticly writes it to a file, if you want more control you can use the ToJson method.

```cs
// Let EveLogin handle file manipulation
string FilePath = "path to your save file";

// Do it yourself
string json = await login.ToJson(FilePath);
```
#### Setup ESI
It is reccomended that you always give a UserAgent, then CCP is less likely to remove your access to ESI.
```cs
EsiConfig config = new EsiConfig {
  UserAgent = "Your user agent",
};

// When you create ESI you must specify both version and datasource, Eve-OpenaAPI will then automaticly downlad the spec for that version.
ESI esi = await ESI.Create(EsiVersion.Latest, Datasource.Tranquility, login, client, config);
```
#### Retrive data from ESI
```cs
// First you must select a path, this path will be validated to make sure you are using the right EsiVersion
EsiPath path = esi.Path("/characters/{character_id}/mail/");

EsiResponse response = await path.Get("Character Name", ("character_id", "character id"));

// If you have a class for the response you can also specify that in the request.
EsiResponse<T> response = await path.Get<T>("Character Name", ("character_id", "character id"));

// If you want to do a batch request to for multiple characters use GetBatch
List<object> characterIDs = new List<object>() {"character id", "character id"};
Lis<EsiResponse<T>> response = await path.GetBatch<T>("Character Name", ("character_id", characterIDs));
```
---

EVE Online © 2019 [CCP hf.](https://www.ccpgames.com/)