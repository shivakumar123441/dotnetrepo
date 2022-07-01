# InvestTrackerWebApi
Web Api for Investment Tracker Application

## Admin Credentials

Email: rootadmin@investtracker.com

Password: rootadmin

## Ethereal Credentials

https://ethereal.email/

Email Address: susana.gislason79@ethereal.email

Password: 1sfGfsrmF8CEEeJJMr

## Hangfire credentials

https://investtrackerwebapi.herokuapp.com/jobs

Username: admin

Password: admin

## Setting up local environment

- Install Postgress Database Server
- Create user with username as "postgres" and password as "myPassw0rd" (modify the password if already exists)
- Setup DbMigrator as startup project and run. This would create InvestTracker database in the Postgres server along with fake data
- Setup InvestTrackerWebApi.Host as startup project and run.
- This would display existing APIs in Swagger.
- Use token end point to get token and use it in swagger for futhur API calls
