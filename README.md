# mt-performance-benchmark
## Solution that tests how fast a consumer with an inbox can receive messages.

In order to run the project in Producer and Consumer projects set the correct connection strings for your database (either locally or through Docker).
Besides that run Localstack with default settings.

Run Consumer and Producer projects.

In order to test the solution go to: https://localhost:7013/swagger/index.html where you can call an endpoint specifying the number of messages you want to publish.
