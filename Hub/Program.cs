﻿// See https://aka.ms/new-console-template for more information
using Domain;
using Entities;
using System.ComponentModel.Design.Serialization;
using System.Runtime.CompilerServices;

int floor;
int people;
int destination = 0;
bool isUp = true;
bool carryOn = true;

Shaft shaft = new Shaft();

bool validPeople = false;
bool validDirection = false;
bool validDestination = false;
bool validCarryOn = false;

bool tripComplete = false;

shaft.RaiseIsMoving += Shaft_RaiseIsMoving;
shaft.RaiseAtFloor += Shaft_RaiseAtFloor;
shaft.RaiseRemaining += Shaft_RaiseRemaining;

IElevator elevator = new FastElevator();     //avoid getting null here.  Review

void Shaft_RaiseAtFloor(object? sender, MovingEventArgs e)
{
    elevator = e.Elevator;
    WriteMessage($"{e.ElevatorId}, arrived at {e.CurrentFloor}", (e.FinalDestination ? ConsoleColor.Green : ConsoleColor.Yellow));
    if (e.FinalDestination == false)
    {
        elevator.Board();

        destination = GetFloor("Enter floor where lift is going:");
        validDestination = true;
    }
    else
    {
        tripComplete = true;
    }
}

void Shaft_RaiseIsMoving(object? sender, MovingEventArgs e)
{
    var elevator = e.Elevator;

    string elevatorType = "Slow Elevator";
    if (elevator is FastElevator)
    {
        elevatorType = "Express Elevator";
    }
    
    WriteMessage($"{elevatorType} {e.ElevatorId}, is at Floor # {e.CurrentFloor}, going {(e.IsUp ? "Up" : "Down")}, with {e.People}, to floor {e.Elevator.NextFloor}");
}


void Shaft_RaiseRemaining(object? sender, LoadingEventArgs e)
{
    WriteMessage($"{e.ElevatorId} left behind {e.Remaining} people at floor {e.Floor}", ConsoleColor.Yellow);
}

void WriteMessage(string message, ConsoleColor? color = null)
{
    if (color != null)
    {
        Console.ForegroundColor = color.Value;
    }
    Console.WriteLine(message);
    if (color != null)
    {
        Console.ResetColor();
    }
}

//input a floor.  Validate if valid
int GetFloor(string prompt)
{
    int floorValue;
    bool validFloor = false;
    do
    {
        WriteMessage(prompt);
        validFloor = int.TryParse(Console.ReadLine(), out floorValue);
        if (!validFloor)
        {
            WriteMessage("Invalid floor entered", ConsoleColor.Red);

        }
        else
        {
            if (!shaft.FloorInRange(floorValue))
            {
                WriteMessage($"Floor input is outside range {shaft.LowestFloor} to {shaft.HighestFloor}", ConsoleColor.Red);
                validFloor = false;
            }
        }
    } while (validFloor == false);
    return floorValue;
}

//input number of people.  Validate if valid
void GetNumberOfPeople()
{
    do
    {
        WriteMessage($"Number of people waiting:");
        validPeople = int.TryParse(Console.ReadLine(), out people);
        if (validPeople == false || people <= 0)
        {
            WriteMessage("Invalid number of people", ConsoleColor.Red);
            validPeople = false;
        }
    } while (validPeople == false);
}

//input direction and validate
void GetDirection()
{
    do
    {
        validDirection = false;
        WriteMessage($"Desired direction: Up/Down");
        string direction = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(direction))
        {
            if (direction.Trim().ToLower() == "up")
            {
                isUp = true;
                validDirection = true;
            }
            if (direction.Trim().ToLower() == "down")
            {
                isUp = false;
                validDirection = true;
            }
        }

        if (validDirection == false)
        {
            WriteMessage("Invalid direction", ConsoleColor.Red);
        }
    } while (validDirection == false);
}

//allow user to add new request
void PromptForNewTrip()
{
    do
    {
        validCarryOn = false;
        WriteMessage($"Do you want to enter another trip? (Yes|Y / No|N)", ConsoleColor.Red);
        string carryOnAnswer = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(carryOnAnswer))
        {
            if (carryOnAnswer.Trim().ToLower() == "yes" || carryOnAnswer.Trim().ToLower() == "y")
            {
                carryOn = true;
                validCarryOn = true;
            }
            if (carryOnAnswer.Trim().ToLower() == "no" || carryOnAnswer.Trim().ToLower() == "n")
            {
                carryOn = false;
                validCarryOn = true;
            }
        }

        if (validCarryOn == false)
        {
            WriteMessage("Invalid answer", ConsoleColor.Red);
        }
    } while (validCarryOn == false);
    tripComplete = false;
    validDestination = false;
}

//display summary

void ShowSummary()
{
    WriteMessage($"Current Status".PadLeft(30, '-').PadRight(46, '-'), ConsoleColor.Cyan);
    foreach (var item in shaft.Elevators)
    {
        WriteMessage($":: {item.Id.PadRight(10)}, at floor {item.CurrentFloor.ToString().PadRight(10)}{item.ElevatorStatus.ToString()}".PadRight(46), ConsoleColor.Cyan);
    }
    WriteMessage(new string('-', 46), ConsoleColor.Cyan);
}

WriteMessage("Elevator summary!", ConsoleColor.Green);
WriteMessage($"{shaft.Floors.Count} floors, {shaft.Elevators.Count} elevators", ConsoleColor.Green);
WriteMessage($"Bottom floor: {shaft.LowestFloor}", ConsoleColor.Green);
WriteMessage($"Top floor: {shaft.HighestFloor}", ConsoleColor.Green);

ShowSummary();

//loop until user has no more requests
do
{

    floor = GetFloor("Enter floor where lift is required:");

    GetNumberOfPeople();

    GetDirection();

    shaft.AddRequest(floor, people, isUp);

    do
    {
        Thread.Sleep(1000);
    } while (validDestination == false);

    shaft.ExecuteRequest(elevator, destination);

    do
    {
        Thread.Sleep(1000);
    } while (tripComplete == false);

    ShowSummary();

    PromptForNewTrip();
} while (carryOn);

