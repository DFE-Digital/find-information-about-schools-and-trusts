# Docker

- [Docker](#docker)
  - [Run the web application locally in Docker](#run-the-web-application-locally-in-docker)
    - [Why?](#why)
    - [How?](#how)
      - [Github Token](#github-token)
    - [Known issues (and their solutions)](#known-issues-and-their-solutions)
      - [On Windows](#on-windows)
      - [On Macs](#on-macs)

## Run the web application locally in Docker

### Why?

As Find information about academies and trusts is deployed in a Docker container built from `/docker/Dockerfile`, running the application from Docker gives you the most live-like experience you can get on your local machine.

This can be particularly useful when:

- Making sure styling changes are up to date and work on a clean environment
- Writing automated UI tests against a branch that's not deployed

### How?

First:

- ensure the Docker engine is running
- navigate to the `docker` directory
- copy the `.env.example` file, save as `.env` and populate the application secrets within

To start the application and have it automatically update after any changes to the source files:

```bash
# Make sure you are in the docker directory
docker compose watch
```

#### Github Token

To build the docker image a token will need to be created to [add a personal access token](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens#creating-a-personal-access-token-classic) to you get hub account with the `read:packages` permissions. This needs to be a classic token.

### Known issues (and their solutions)

#### On Windows

If your container doesn't start or you see an error like this `/bin/bash^M: bad interpreter: No such file or directory` it may be caused by line ending differences in the bash shell script (which we've been unable to fix in the remote repository).

1. Open git bash (this will not work in PowerShell)
2. Navigate to the docker directory
3. Rewrite your line endings by running:

   ```bash
   sed -i -e 's/\r$//' web-docker-entrypoint.sh
   ```

4. Stage your changes (they will disappear after being staged)
5. Rebuild and run the Docker container - it should now work

#### On Macs

If you are running on Apple M1 chip the Test Containers SQL Server image ([used by the FIAT db unit tests][adr-15]) may not work. This can be fixed by:

- Docker Settings > General: [X] Use virtualization framework and
- Docker Settings > Features in Development: [X] Use Rosetta...

[adr-15]: adrs\0015-use-test-containers-to-unit-test-fiat-database.md
