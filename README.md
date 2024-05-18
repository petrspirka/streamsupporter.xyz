# streamsupporter.xyz
A web application used for Twitch and YouTube reward system. Allows reactions to superchats on YouTube, subscriptions, channel point redemptions and bit donations on Twitch.

# Building
First clone the repository with `git clone https://github.com/petrspirka/streamsupporter.xyz`
Navigate to the folder with the repository and run `dotnet build -c Release` in the cloned repository to build the app
Navigate to `bin/Release` and edit the appsettings.Empty.json file to match your configuration
Rename appsettings.Empty.json to appsettings.json and run `dotnet ./NewStreamSupporter.dll`. If an error arises about invalid URI, check the callback value in appsettings.json
