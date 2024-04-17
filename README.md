# MdkRestServerless

Serverless REST API with Azure Functions

## Introduction

This was a learning project to understand how to create a serverless REST API using Azure Functions.

The inspiration for this project was a YouTube video by Codewrinkes (Dan Patrascu):

https://youtu.be/3HZjmYohlgc?si=VoKvw_VhFBYehVRk

While this helped me get started, I had to change a few things and invent some new things to get it to work.

For one, Codewrinkles used the "in-process" model for Azure .NET Functions, but this is being
deprecated/replaced with the "isolated" model, so I used the "isolated" model.

Also, because of my own biases, I implemented the "update" method as a PATCH request instead of a PUT request.

I also had to figure out how to create and apply the database migrations, as this was not covered in the video,
and this was non-trivial. It required creating a DesignTimeDbContextFactory, and a custom IStartup class.
And further, I had to set the environment variable for the connection string in the terminal before running the migration,
because the ef tools do not read the connection string from the local.settings.json file.

Another struggle was getting the connection string for my Azure SQL database. I could not find anywhere in the Azure portal
where the connection string was displayed. I had to construct it myself from the server name, database name, and user name
(GitHub Copilot helped me with this).

When launching the function within VSCode, I was getting a warning message:
> Failed to verify "AzureWebJobsStorage" connection specified in "local.settings.json". Is the local emulator installed and running?

Copilot again came to the rescue, suggesting that I set `AzureWebJobsStorage` to an empty string in the local.settings.json file.

Also a minor struggle is understanding what is returned from `req.CreateResponse`. The documentation just says "creates a new response",
but no details on what is contained in the response. Apparently this method sets the "Content-Type" header, so you try to set
it yourself the code will throw an exception.
