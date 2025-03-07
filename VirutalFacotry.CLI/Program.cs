// Program.cs
using System;
using VirutalFactoryCore;
class Program
{
    static void Main(string[] args)
    {
        // Create the display controller


        // Create machine with the display
        Machine machine = new Machine();

        bool running = true;

        while (running)
        {

            string command = Console.ReadLine().ToLower().Trim();

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


                    break;

                case "help":
                    DisplayHelp();
                    break;

                case "new job":
                    CreateNewJob(machine);
                    break;

                case "list jobs":
                    machine.JobManager.ListAllJobs();
                    break;

                case "select job":
                    SelectJob(machine);
                    break;

                case "cancel job":
                    CancelJob(machine);
                    break;

                default:

                    break;
            }

            machine.JobManager.SaveJobs();
        }
    }

    private static void DisplayHelp()
    {

    }

    private static void CreateNewJob(Machine machine)
    {

        string productName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(productName))
        {

            return;
        }

        string quantityInput = Console.ReadLine();
        ;

        if (int.TryParse(quantityInput, out int quantity) && quantity > 0)
        {
            machine.JobManager.CreateJob(productName, quantity);
        }
        else
        {

        }
    }

    private static void SelectJob(Machine machine)
    {
        if (machine.CurrentState == Machine.State.Running)
        {

            return;
        }

        string jobIdInput = Console.ReadLine();


        if (int.TryParse(jobIdInput, out int jobId))
        {
            if (machine.JobManager.StartJob(jobId))
            {

            }
        }
        else
        {
            dsad
        }
    }

    private static void CancelJob(Machine machine)
    {
        if (machine.CurrentState == Machine.State.Running)
        {

            return;
        }



        string jobIdInput = Console.ReadLine();




        if (int.TryParse(jobIdInput, out int jobId))
        {
            machine.JobManager.CancelJob(jobId);


        }
        else
        {

        }
    }
}
