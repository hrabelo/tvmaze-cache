# TVMaze cache

A simple application to scrap TVMaze Api for show with cast, cache it in MongoDB and serve it via API.

## Dependencies
 - [.NET6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
 - [Docker](https://docs.docker.com/get-docker/)

## Setup
 - Start the MongoDB using the Docker compose command:
 ```
 docker-compose up -d
 ```

 - Run the application using dotnet run command from the root folder:
 ```
 dotnet run --project src/TVMazeCache.WebApi/TVMazeCache.WebApi.csproj
 ```

## Usage
This applications consists in a WebApi with and endpoint to serve `shows` following these guidelines:
- The list of shows is paginated;
- The `cast` property of the shows is ordered by `birthday` descending.

## Endpoints

### GET shows
```
/shows?page={page_no}
```
Where `{page_no}` is the page number.

Returns a list of paginated shows. The size of the page is `100`.

Example responses
```
200 Ok
```
```json
[
  {
    "id": 1,
    "name": "Under the Dome",
    "cast": [
        {
        "id": 7,
        "name": "Mackenzie Lintz",
        "birthday": "1996-11-21"
        },
        {
        "id": 5,
        "name": "Colin Ford",
        "birthday": "1996-09-11"
        }
    ]
  },
  {
    "id": 2,
    "name": "Person of Interest",
    "cast": [
        {
        "id": 92,
        "name": "Sarah Shahi",
        "birthday": "1980-01-09"
        },
        {
        "id": 91,
        "name": "Amy Acker",
        "birthday": "1976-12-04"
        }
    ]
  }
]
```
## Decisions
- The scrapping of [TVMazeApi](https://www.tvmaze.com/api) is done via a `BackgroundService` of the Api;
- Because of the rate limit of TVMazeApi, parallelization wouldn't offer much. Therefore, the scrapping is serial;
- MongoDB as a storage for simplicity. NoSQL looks like a good option for this case, since it's basically storing the JSON response of API calls. No need to worry about data modelling and table creation.
- If the application stops midway through the scrapping, then the next time it runs it'll resume from the last processed page. (Also stored in Mongo).
- The Api rate limit mentioned above and possible transient errors when scrapping TVMazeApi are gracefully handled by [Polly](https://github.com/App-vNext/Polly.Extensions.Http/blob/master/README.md).