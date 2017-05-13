[![Build status](https://ci.appveyor.com/api/projects/status/58fdyc6de81r7t3r/branch/master?svg=true)](https://ci.appveyor.com/project/eswann/cars/branch/master)
[![Coverage Status](https://coveralls.io/repos/github/eswann/cars/badge.svg?branch=master)](https://coveralls.io/github/eswann/cars?branch=master)

# CARS: CQRS + Event Sourcing
A small .Net Core framework to help you with CQRS and Event Sourcing.

Currently CARS supports Mongo, although it has a provider model to allow additional data stores.

## Features

* Unit of Work
* Command dispatcher abstraction
* Event publisher
* Event Store abstraction
* Snapshot (custom strategy implementation)
* Custom events metadata

## Configure development enviroment

1. Install Mongo (Directly or via Docker)

## Event store implementations

* MongoDB: Install-Package Cars.EventStore.MongoDB

