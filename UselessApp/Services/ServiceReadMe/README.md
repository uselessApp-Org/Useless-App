# Migration #

You will need to use the migration in MongodbUser to add any new field to the databse

# How To use the migration #

1. update user.cs in the model folder
2. update the filter and update in MongodbUser UpdateUserAsync
    * var filter = Builders<User>.Filter.Or(
            Builders<User>.Filter.Exists("<YourChangeHere>", false),
            Builders<User>.Filter.Exists("<YourChangeHere>", false)
    );

    * var update = Builders<User>.Update
            .Set(u => u.<YourChangeHere>, "")
            .Unset(u => u.<YourChangeHere>, "");
3. in the Terminal run this command
  *  `dotnet run --project .\UselessApp\UselessApp.csproj -- --update-users`

# Notes #

You can not have the website running locally at the same time.
  * You will get a lot of warnings when you run the command, its fine.
  * .Set adds a field 
  * .Unset removes a field