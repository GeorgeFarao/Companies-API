# Companies Web API

In this project I have created a REST Web API using .NET Core that performs all CRUD operations on companies, branchies, workstations and employees. The companies have branchies, branchies have workStations and workstations have employees.

# Project's Logic

In the Startup file the services and the HTTP request pipeline are configured.

In the Data folder the classes are repressented as in the database and in the Models as they would be in the client.
Each Model has it's Controller for Post, Get, Put and Delete operations and it's Service to perform the operations needed by the controllers.

The models have Builders, Deleters which are responsible for correct deletion so that data that are connected with foreign key relations will be correctly deleted and Lookups for better Reading operations , using paging and field filtering.

I have also implemented Validators for each Model in order to check if the data received are correct, an error handling Middleware and Internationalization to translate the data requested in French.


