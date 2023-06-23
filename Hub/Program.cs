// See https://aka.ms/new-console-template for more information
using Domain;
using Entities;
using System.ComponentModel.Design.Serialization;
using System.Runtime.CompilerServices;

int floor;
int people;
int destination = 0;
bool isUp = true;

Shaft shaft = new Shaft();

int topFloor = shaft.Floors.Last().Number;
int bottomFloor = shaft.Floors.First().Number;

bool validPeople = false;
bool validDirection = false;
bool validDestination = false;

shaft.RaiseIsMoving += Shaft_RaiseIsMoving;
shaft.RaiseAtFloor += Shaft_RaiseAtFloor;
Elevator elevator = new Elevator();     //avoid getting null here.  Review

void Shaft_RaiseAtFloor(object? sender, MovingEventArgs e)
{
    elevator = e.Elevator;
    WriteMessage($"{e.ElevatorId}, arrived at {e.CurrentFloor}", (e.FinalDestination ? ConsoleColor.Green : ConsoleColor.Yellow));
    if (e.FinalDestination == false)
    {
        for (int i = 0; i < e.People; i++)
        {
            elevator.Board(new Person { Destination = destination });
        }
        destination = GetFloor("Enter floor where lift is going:");
        validDestination = true;
    }
}

void Shaft_RaiseIsMoving(object? sender, MovingEventArgs e)
{
    WriteMessage($"{e.ElevatorId}, is at Floor # {e.CurrentFloor}, going {(e.IsUp ? "Up" : "Down")}, with {e.People}");
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
            if (floorValue < bottomFloor || floorValue > topFloor)
            {
                WriteMessage($"Floor input is outside range {bottomFloor} to {topFloor}", ConsoleColor.Red);
                validFloor = false;
            }
        }
    } while (validFloor == false);
    return floorValue;
}

WriteMessage("Elevator summary!", ConsoleColor.Green);
WriteMessage($"{shaft.Floors.Count} floors, {shaft.Elevators.Count} elevators", ConsoleColor.Green);
WriteMessage($"Bottom floor: {bottomFloor}", ConsoleColor.Green);
WriteMessage($"Top floor: {topFloor}", ConsoleColor.Green);

floor = GetFloor("Enter floor where lift is required:");

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
        WriteMessage("Invalid directionr", ConsoleColor.Red);
    }
} while (validDirection == false);

shaft.AddRequest(floor, people, isUp);

do
{
    Thread.Sleep(1000);
} while (validDestination == false);

elevator.Destination = destination;
if (destination > elevator.CurrentFloor)
{
    _ = elevator.MoveUp(true);
}
else
{
    _ = elevator.MoveDown(true);
}
Console.ReadLine();

Console.ReadLine();
