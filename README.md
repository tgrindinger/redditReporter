# RedditReporter
The objective of this application is to report both the posts with the most upvotes and users with the most posts in a given subreddit while the application is running. This is provided through a simple REST API written in ASP.NET. The requirements are somewhat ambiguous, so after requesting additional clarification, here is how I am defining these two requirements:

* Posts with the most upvotes: Reddit's definition of "top" uses upvotes to determine which posts are "top". Since the requirement does not explicitly state that the upvotes must occur while the application is running, and it would be practically impossible to watch for upvotes on older posts, this application provides a simple report of all posts that have the most upvotes. If this data changes while the application is running, the user will get the updated data on their next API request.
* Users with the most posts: This application reports the users that have made the most posts while the application is running. Most subreddits do not move fast enough to be able to see this value change much, though, unless it is left running for several hours or days. It is not possible to report the all-time top posters to a given subreddit, unlike the previously-discussed posts with the most upvotes. Keep in mind that a reddit "post" is the top-level entity that you see in list format when you browse to a given subreddit, and these usually do not change rapidly.

## Running the app

You should be able to run the app through Visual Studio with IISExpress or Docker. The requirement stated that .NET 6 or 7 are preferred, so this app requires .NET 7 to be installed. Additionally, there are a few nuget packages that will need to be installed, but nothing that requires additional configuration.

In order to access Reddit's API, appropriate values for appId, refreshToken, and accessToken will need to be provided in appsettings.json. Details on how to procure these values are outside the scope of this project but should be available from Reddit's documentation. The /Posts endpoint should immediately show results, but if not, check the output log to see if there are problems authenticating with Reddit. Additionally, there are a few values that may be adjusted depending on your preferences:
* TargetSub: by default, it is set to "funny", but this may be set to any public subreddit
* SeedTopPosts: by default, it is set to "true", and that is likely to be the preferred setting by any users. This will cause the application to retrieve the current top posts and make them available to user requests, immediately. If this is set to false, the set of top posts will only contain top posts that have been updated since the application started.
* SeedNewPosts: by default, it is set to "false". By the definition for "users with the most posts" given above, it is inferred that this data should only encompass new posts made since the application started. This means that requests for users with the most posts will be mostly empty, unless a lot of users make a lot of posts within a small window of time after the application starts. By setting this value to "true", more data is available for users to request, but the results fall outside the time window of the application.

## Using the app
The app provides a user-friendly Swagger interface, which displays the two endpoints for the app: /Posts and /Users

It is recommended to use Swagger for interfacing with the application, but other mechanisms such as curl or RestSharp should work just fine. The endpoints require no parameters and will simply return responses reflecting the defined values discussed above. A more fully-featured app might enforce paging to constrain the size of responses.

## Testing
Unit tests are provided for controllers and data access. This encompasses the majority of the app functionality, aside from communication with Reddit's API. Adding tests for the latter would fall under integration or functional testing, which are needed in a real production system, but are not provided in this short project.

It's worth pointing out that these unit tests are written as "sociable" tests, meaning that "unit" is defined as "unit of behavior", not as "unit of code". This means that different app components may be instantiated and communicate with each other within a single unit test, minimizing the need for mocks (there are actually no mocks in this project) and making it more tolerant (or resistant) to refactoring.

Sociable unit tests are a great way to minimize implementation detail within unit tests, keep the tests behavior-oriented, and more efficiently cover app behavior with fewer tests. In a more elaborate system, components like the in-memory data repositories may serve as "stubs" for the database components, etc.

Additionally, the builder pattern is used to provide a mechanism for isolating construction of the system under test from the unit tests, themselves. This further decouples the individual unit tests from the implementation details of the system under test.

One shortcoming of MSTest, used in this project, is a lackluster assertion library. FluentAssertions alleviates this inconvenience and also greatly enhances the readability of the assertions. This is preferable to using a heavier-weight external unit test library, especially for fairly small projects.

## Implementation details
This app uses a simple ConcurrentDictionary to store post and user data. This allows the service that monitors Reddit data to be decoupled from the client-facing API, but it would be unacceptable for a production system. The requirements stated that in-memory storage is fine for this project, with the acknowledgement that a database would be preferential for a real production system.

Communication with Reddit happens through the Reddit.Net nuget package, which manages Reddit API data limits automatically and drastically reduces the amount of code needed for this project. On the other hand, the Reddit.Net project is not very active, so a production system would need to find a better long-term solution, whether it is forking Reddit.Net or rolling a custom Reddit API client.

The afore-mentioned communication with Reddit occurs within a background service that is decoupled from the user-facing API. This allows individual requests to be fulfilled more quickly by keeping the state of the data within the app up to date with the data from Reddit. Instead of having to ping Reddit every time a user request occurs, the app can simply return its current knowledge, which should already be up to date, since the background service is continuously monitoring Reddit for changes.
