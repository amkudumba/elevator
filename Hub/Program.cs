// See https://aka.ms/new-console-template for more information
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

int topFloor = shaft.Floors.Last().Number;
int bottomFloor = shaft.Floors.First().Number;

bool validPeople = false;
bool validDirection = false;
bool validDestination = false;
bool validCarryOn = false;

bool tripComplete = false;

shaft.RaiseIsMoving += Shaft_RaiseIsMoving;
shaft.RaiseAtFloor += Shaft_RaiseAtFloor;
shaft.RaiseRemaining += Shaft_RaiseRemaining;

Elevator elevator = new Elevator();     //avoid getting null here.  Review

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
        tripComplete=true;
    }
}

void Shaft_RaiseIsMoving(object? sender, MovingEventArgs e)
{
    WriteMessage($"{e.ElevatorId}, is at Floor # {e.CurrentFloor}, going {(e.IsUp ? "Up" : "Down")}, with {e.People}");
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

WriteMessage("Elevator summary!", ConsoleColor.Green);
WriteMessage($"{shaft.Floors.Count} floors, {shaft.Elevators.Count} elevators", ConsoleColor.Green);
WriteMessage($"Bottom floor: {bottomFloor}", ConsoleColor.Green);
WriteMessage($"Top floor: {topFloor}", ConsoleColor.Green);

ShowSummary();

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

void ShowSummary()
{
    WriteMessage($"Current Status".PadLeft(30, '-').PadRight(46, '-'), ConsoleColor.Cyan);
    foreach (var item in shaft.Elevators)
    {
        WriteMessage($":: {item.Id.PadRight(10)}, at floor {item.CurrentFloor.ToString().PadRight(10)}{item.ElevatorStatus.ToString()}".PadRight(46), ConsoleColor.Cyan);
    }
    WriteMessage(new string('-', 46), ConsoleColor.Cyan);
}