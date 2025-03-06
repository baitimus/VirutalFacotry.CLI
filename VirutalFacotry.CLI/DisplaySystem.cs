// ConsoleDisplay.cs
using System;
using System.Collections.Generic;

public class ConsoleDisplay
{
    private int _statusLinePosition;
    private int _lightLinePosition;
    private int _jobStatusLinePosition;
    private int _messageLinePosition;
    private int _jobListStartPosition;
    private int _inputLinePosition;
    private List<string> _jobListItems = new List<string>();
    private int _maxJobListLines = 10;

    public ConsoleDisplay()
    {
        Console.Clear();
        DrawInitialInterface();
    }



    private void DrawInitialInterface()
    {
        Console.WriteLine("Virtual Factory Machine Simulation");
        Console.WriteLine("=================================");
        Console.WriteLine("Machine Commands: 'start', 'stop', 'exit'");
        Console.WriteLine("Job Commands: 'new job', 'list jobs', 'select job', 'cancel job'");
        Console.WriteLine();

        // Fixed status lines
        Console.WriteLine("Machine State: Ready");
        Console.WriteLine("Signal Light: ●YELLOW●");
        Console.WriteLine("Current Job: None");
        Console.WriteLine("Message: ");
        Console.WriteLine();

      
   

        // Input prompt
        Console.WriteLine("> ");

        // Store the positions of the status lines
        _statusLinePosition = 5;
        _lightLinePosition = 6;
        _jobStatusLinePosition = 7;
        _messageLinePosition = 8;
        _jobListStartPosition = 11;
        _inputLinePosition = _jobListStartPosition + 1;

        SetCursorToInput();
    }

    public void UpdateMachineState(Machine.State state)
    {
        UpdateLine(_statusLinePosition, $"Machine State: {state}");
    }

    public void UpdateSignalLight(SignalLight.LightColor color)
    {
        // Save current cursor position
        int currentLeft = Console.CursorLeft;
        int currentTop = Console.CursorTop;

        // Move to signal light line
        Console.SetCursorPosition(0, _lightLinePosition);
        Console.Write("Signal Light: ");

        // Set color and display light indicator
        ConsoleColor originalColor = Console.ForegroundColor;

        switch (color)
        {
            case SignalLight.LightColor.Green:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("●GREEN●");
                break;
            case SignalLight.LightColor.Yellow:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("●YELLOW●");
                break;
            case SignalLight.LightColor.Red:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("●RED●");
                break;
        }

        // Clear the rest of the line
        Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft - 1));

        // Restore original color and cursor position
        Console.ForegroundColor = originalColor;
        Console.SetCursorPosition(currentLeft, currentTop);
    }

    public void UpdateJobStatus(string status)
    {
        UpdateLine(_jobStatusLinePosition, $"Current Job: {status}");
    }

    public void UpdateMessage(string message)
    {
        UpdateLine(_messageLinePosition, $"Message: {message}");
    }

    public void AddToJobList(string jobInfo)
    {
        _jobListItems.Add(jobInfo);
        RefreshJobList();
    }

    public void ClearJobList()
    {
        _jobListItems.Clear();
        RefreshJobList();
    }

    private void RefreshJobList()
    {
        // Save current cursor position
        int currentLeft = Console.CursorLeft;
        int currentTop = Console.CursorTop;

        // Clear existing job list area
        for (int i = _jobListStartPosition; i < _jobListStartPosition + _maxJobListLines; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write(new string(' ', Console.WindowWidth - 1));
        }

        // Display job items or "no jobs" message
        if (_jobListItems.Count == 0)
        {
            Console.SetCursorPosition(1, _jobListStartPosition);
            Console.Write("(No jobs to display)");
        }
        else
        {
            // Show up to 5 most recent jobs
            int startIndex = Math.Max(0, _jobListItems.Count - _maxJobListLines);
            for (int i = 0; i < Math.Min(_maxJobListLines, _jobListItems.Count); i++)
            {
                Console.SetCursorPosition(1, _jobListStartPosition + i);
                Console.Write(_jobListItems[startIndex + i]);
            }
        }

        // Update input line position
        _inputLinePosition = _jobListStartPosition + Math.Min(_maxJobListLines, Math.Max(1, _jobListItems.Count)) + 1;

        // Update input prompt
        Console.SetCursorPosition(0, _inputLinePosition);
        Console.Write("> " + new string(' ', Console.WindowWidth - 3));

        // Restore cursor position
        Console.SetCursorPosition(currentLeft, currentTop);
    }

    private void UpdateLine(int linePosition, string text)
    {
        // Save current cursor position
        int currentLeft = Console.CursorLeft;
        int currentTop = Console.CursorTop;

        // Move to line position
        Console.SetCursorPosition(0, linePosition);

        // Write new text and clear the rest of the line
        Console.Write(text);
        Console.Write(new string(' ', Console.WindowWidth - text.Length - 1));

        // Restore cursor position
        Console.SetCursorPosition(currentLeft, currentTop);
    }

    public void SetCursorToInput()
    {
        Console.SetCursorPosition(2, _inputLinePosition);
    }

    public void ClearInput()
    {
        // Clear the input line
        Console.SetCursorPosition(0, _inputLinePosition);
        Console.Write("> " + new string(' ', Console.WindowWidth - 3));
        SetCursorToInput();
    }
}