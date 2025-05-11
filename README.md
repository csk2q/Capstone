# Capstone Project for Computer Science
**Links:** [Github](https://github.com/csk2q/Capstone/) | [Jira](https://directdepositcapstone.atlassian.net/jira/software/projects/DDC/boards/1?selectedIssue=DDC-3) | [Website (ddcapstone.com)](https://ddcapstone.com)   
This repository is for a banking web app, created for as our capstone project in computer science.

## Direct Deposit Team
- [Trinity M.](https://github.com/kirselandise) (tlmqdm@umkc.edu)
- [Raine M.](https://github.com/RMorrigan) (rmorrigan@umkc.edu)
- [Christian S.](https://github.com/csk2q) (csk2q@umkc.edu)
- [Abigail B.](https://github.com/abotts-umkc) (albdmc@umkc.edu)

## Project structure

### Technology used
- ASP.NET / C# - webserver and backend
- Blazor - Frontend framework
- PostgreSQL - database
- Open AI 4o and 4.5 LLM - AI used during development
  - Also used to power the AI budget sim.
- sendgrid - sending emails
- Cloudflare - domain registration
- Cloudflare Tunnel - reverse proxy for the website
- Docker - self-hosting the database and webserver

### Project Structure
- **DDC** - The root folder for the project source.
  - **BeeTesting** - Project containing the unit tests.
  - **ServerBee** - Source code for the website and backend.
  - *pgsql-compose.ymal* - Docker compose file for hosting database in dev.
- *docker-compose.ymal* - Docker compose file for running the project in containers for production.

## Building & Running
Docker is used to containerize and run the postgres database and in production the webserver.
If you do not have Docker installed you can find instructions to install it [here](https://docs.docker.com/get-started/get-docker/).  

Ensure you have docker installed and running before following the instructions to run the project locally.


### Production

To download and run the project:

```sh
git clone --branch main --depth 1 https://github.com/csk2q/DirectDepositCapstone.git
cd DirectDepositCapstone

# Optional: Open the file docker-compose.yaml and insert your tokens for OpenAI, sendgrid, and Cloudflare Tunnel.
# code docker-compose.yaml

docker compose up --build --detach 
```

### Development

To run in development you will need to download and install the [.NET SDK v9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) to your computer.

First start the postgres database, by opening a terminal in the DDC folder and running the following.
```sh
docker compose --file pgsql-compose.yml up --detach
```
With postgress running in docker you can move to the folder DDC/ServerBee and run the asp.net server as you would any .NET project.
```sh
dotnet run # To run in development
dotnet watch # Run in development with hot-reload enabled
dotnet build # (Optional) Use explictly cause a (re)build
```

### Database setup issues

#### Manually creating the database

The asp.net should automatically scaffold and migrate the database on first startup. However should it fail it may be necessary to manually migrate the database.

> Note: ONLY preform the following on a new database. If used in a database with data it WILL cause issues and may cause data to be lost.

If Entity Framework core tools are not installed, you can install it with the following command:
```sh 
dotnet tool install --global dotnet-ef
```
First generate the migration script by running the following in the folder DDC/ServerBee.
```sh
dotnet ef migrations script --idempotent -o ./migration.sql
```
Open the generate script `migration.sql` in a text editor and do the following.   
Using find and replace remove every line that contains references to `DO $EF$`, `BEGIN`, `END $EF$;`, and `COMMIT;`   

Open Adminer and log into the database using the instructions [here](#Adminer).
Once logged in, click on "SQL command" and paste the edited contents of `migration.sql` into the text box. Execute the command and ignore any sql exceptions that occur.

You may need to manually extract the INSERT statements from the script and manually execute each statement. To prevent warning messages.

The database should now be scaffolded out and you should be able to run the project with out any issues.   
Should issues persist, further manual edits to the database schema may be required.

#### Password login failure
This error message is misleading the actual issue likely that is that the sever was unable to contact the database. Ensure that postgress is running in Docker and that the port used `2222` is not being used by another program.



### Adminer

We used Adminer as a human accessible interface for the database and included it in the docker compose files.   
After following either of the above instructions you can access it at http://localhost:8080.
Use the host name `postgres` and the sign in information set in the docker compose file.


