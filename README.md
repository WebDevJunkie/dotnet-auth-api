## Introduction

This is a template dotnet core api with authentication built in.

### Endpoints

/users/register - accepts JSON with a username, email address and password to create a user.

/users/authenticate - accepts JSON with an email and password that passes back a JWT for authorizing.

/users/user - this is a simple endpoint to get the information of the current user logged in. This endpoint requires that the JWT is passed in the Header as an auth bearer token.

## Setup

### Database Migration

After pulling the project set up your database connection string in the appsettings at the AuthDatabase connection string.

Run: add-migration and name your migration
Run: update-database to load the migrations that were just created
