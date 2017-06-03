[![Build status](https://ci.appveyor.com/api/projects/status/58fdyc6de81r7t3r/branch/master?svg=true)](https://ci.appveyor.com/project/eswann/cars/branch/master)
[![Coverage Status](https://coveralls.io/repos/github/eswann/cars/badge.svg?branch=master)](https://coveralls.io/github/eswann/cars?branch=master)

![Cars image](https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSI680XhLv9L5_noib5ax3YrQ57Vf5dpYWXPw8ulO1dL1kRHpJJ)

# Cars: CQRS + Event Sourcing
A small .Net Core framework to help you with CQRS and Event Sourcing.

Currently Cars supports Mongo, although it has a provider model to allow additional data stores.

Cars is forked from [EnjoyCQRS](https://github.com/ircnelson/enjoy.cqrs). 

The project has been updated to VS 2017, and setup has been simplified a bit.  

## What's with the name?
Google: "Did you mean Cars?"

In the days of yore (like 10 years ago) if you searched for CQRS on Google, it thought it was Cars misspelled.

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

