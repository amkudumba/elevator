# Elevator Challenge

The task is to create a console application in C# using OOP principles, which simulates the movement of elevators, with the goal being to move people as efficiently as possible.

The console application should provide the following:
* show the status of the elevators, including which floor they are on, whether they are moving and in which direction, as well as how many peopl they are carrying
* provide a way to interact with the elevators, including calling them to a specific floor and setting the number of people waiting on each floor

The console application should be able to offer the following:
* support for multiple floors
* support for multiple elevators

**Hints**
Given a pool of elevators, the program should send the nearest available elevator to that person.

**Extra Credit**
For extra credit, allow a weight limit, expressed as a number of people, to be imposed on the elevators.  You can assume every elevator in the simulation has the same weight limit

**********************************************************************************************************
# Implementation Notes
* Language : C#
* Framework : .Net 6.0
* Structure : Hub -> Domain -> Entities
* Unit tests : used to test validations, moving elevators, initialization, and elevator rules