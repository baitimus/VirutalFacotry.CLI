// Program.cs
using System;

class Program
{
    static void Main(string[] args)
    {
        // Create the display controller
        ConsoleDisplay display = new ConsoleDisplay();

        // Create machine with the display
        Machine machine = new Machine(display);

        bool running = true;

        while (running)
        {
            display.SetCursorToInput();
            string command = Console.ReadLine().ToLower().Trim();
            display.ClearInput();

            if (string.IsNullOrWhiteSpace(command))
                continue;

            switch (command)
            {
                case "start":
                    machine.Start();
                    break;

                case "stop":
                    machine.Stop();
                    break;

                case "exit":
                    running = false;
                    
                    display.UpdateMessage("Shutting down simulation...");
                    break;

                case "help":
                    DisplayHelp(display);
                    break;

                case "new job":
                    CreateNewJob(machine, display);
                    break;

                case "list jobs":
                    machine.JobManager.ListAllJobs();
                    break;

                case "select job":
                    SelectJob(machine, display);
                    break;

                case "cancel job":
                    CancelJob(machine, display);
                    break;

                default:
                    display.UpdateMessage($"Unknown command: '{command}'. Type 'help' for available commands.");
                    break;
            }

            machine.JobManager.SaveJobs();
        }
    }

    private static void DisplayHelp(ConsoleDisplay display)
    {
        display.UpdateMessage("Available commands: 'start', 'stop', 'exit', 'new job', 'list jobs', 'start job', 'cancel job'");
    }

    private static void CreateNewJob(Machine machine, ConsoleDisplay display)
    {
        display.UpdateMessage("Enter product name:");
        display.SetCursorToInput();
        string productName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(productName))
        {
            display.UpdateMessage("Job creation cancelled. Product name cannot be empty.");
            return;
        }
        display.ClearInput();

        display.UpdateMessage($"Enter quantity for {productName}:");
        display.SetCursorToInput();
        string quantityInput = Console.ReadLine();
        display.ClearInput();

        if (int.TryParse(quantityInput, out int quantity) && quantity > 0)
        {
            machine.JobManager.CreateJob(productName, quantity);
        }
        else
        {
            display.UpdateMessage("Invalid quantity. Please enter a positive number.");
        }
    }

    private static void SelectJob(Machine machine, ConsoleDisplay display)
    {
        if (machine.CurrentState == Machine.State.Running)
        {
            display.UpdateMessage("Cannot start a job while machine is running. Stop the machine first.");
            return;
        }

        display.UpdateMessage("Enter job ID to start:");
        display.SetCursorToInput();
        string jobIdInput = Console.ReadLine();
        display.ClearInput();

        if (int.TryParse(jobIdInput, out int jobId))
        {
            if (machine.JobManager.StartJob(jobId))
            {
                display.UpdateMessage($"Job #{jobId} is ready. Use 'start' command to begin processing.");
            }
        }
        else
        {
            display.UpdateMessage("Invalid job ID. Please enter a valid number.");
        }
    }

    private static void CancelJob(Machine machine, ConsoleDisplay display)
    {
        if (machine.CurrentState == Machine.State.Running)
        {
            display.UpdateMessage("Cannot cancel a job while machine is running. Stop the machine first.");
            return;
        }

        display.UpdateMessage("Enter job ID to cancel:");
        display.SetCursorToInput();
        string jobIdInput = Console.ReadLine();
        display.ClearInput();

        if (int.TryParse(jobIdInput, out int jobId))
        {
            machine.JobManager.CancelJob(jobId);
        }
        else
        {
            display.UpdateMessage("Invalid job ID. Please enter a valid number.");
        }
    }
}
