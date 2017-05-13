[![Build status](https://ci.appveyor.com/api/projects/status/58fdyc6de81r7t3r/branch/master?svg=true)](https://ci.appveyor.com/project/eswann/cars/branch/master)
[![Coverage Status](https://coveralls.io/repos/github/eswann/cars/badge.svg?branch=master)](https://coveralls.io/github/eswann/cars?branch=master)

# eNJoy CQRS + Event Sourcing
This framework can help you with two things together and easy. 
First, your entities could use event sourcing technique. The second one you could use Command Query Segregation Responsability (CQRS) pattern. 

Any suggestion is welcome.

## Features

* Unit of Work
    - You can work with BASE or ACID
* Command dispatcher abstraction
* Event publisher
* Event Store abstraction
* Snapshot (custom strategy implementation)
* Custom events metadata

## Configure development enviroment

1. Install Docker
2. Pull mongo image. (See https://hub.docker.com/_/mongo/)
	* e.g.: docker run --name srv-mongo -p 27017:27017 -d mongo


* Discovering docker ip:
	* unix: $(ifconfig en0 | awk '/ *inet /{print $2}')


## Event store implementations

* MongoDB: Install-Package Cars.EventStore.MongoDB

## Architecture

![CQRS high level architecture](http://s32.postimg.org/ty18uww45/090615_1544_introductio1_png_w_604.png)

## Concept

[![CQRS concept](http://www.conceptmaps.io/maps/0acfabc1-5e39-4dd7-9590-3b32c2918ec8.png)](http://www.conceptmaps.io/maps/0acfabc1-5e39-4dd7-9590-3b32c2918ec8/detail)

### [See wiki for more details](https://github.com/ircnelson/enjoy.cqrs/wiki)
