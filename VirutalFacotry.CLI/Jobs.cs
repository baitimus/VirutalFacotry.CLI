// Jobs.cs
using System;
using System.Collections.Generic;
using System.Linq;

public class Job
{
    public enum JobStatus
    {
        Pending,
        InWork,
        Done
    }

    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public int QuantityProduced { get; private set; }
    public JobStatus Status { get; internal set; }
 

    public int JobId { get; private set; }

    public Job(int jobId, string productName, int quantity)
    {
        JobId = jobId;
        ProductName = productName;
        Quantity = quantity;
        QuantityProduced = 0;
        Status = JobStatus.Pending;
      
    }

    public void Produce(int quantity)
    {
        QuantityProduced += quantity;
        if (QuantityProduced >= Quantity)
        {
            Status = JobStatus.Done;
        }
    }

    public override string ToString()
    {
        return $"Job #{JobId}: {ProductName} (Qty: {Quantity}, Produced: {QuantityProduced}) - Status: {Status}";
    }
}

public class JobManager
{
    private List<Job> _jobs;
    private int _nextJobId;
    private ConsoleDisplay _display;
    private Job _currentJob;

    public JobManager(ConsoleDisplay display)
    {
        _jobs = new List<Job>();
        _nextJobId = 1;
        _display = display;
        _currentJob = null;
    }

    public Job CreateJob(string productName, int quantity)
    {
        Job newJob = new Job(_nextJobId++, productName, quantity);
        _jobs.Add(newJob);
        _display.UpdateMessage($"Created {newJob}");
        return newJob;
    }

    public bool StartJob(int jobId)
    {
        // Check if a job is already running
        if (_currentJob != null && _currentJob.Status == Job.JobStatus.InWork)
        {
            _display.UpdateMessage($"Cannot start job #{jobId}. Job #{_currentJob.JobId} is already in progress.");
            return false;
        }

        // Find the job with the given ID
        Job job = _jobs.Find(j => j.JobId == jobId);
        if (job == null)
        {
            _display.UpdateMessage($"Job #{jobId} not found.");
            return false;
        }

        // Check if the job is already completed
        if (job.Status == Job.JobStatus.Done)
        {
            _display.UpdateMessage($"Job #{jobId} is already completed.");
            return false;
        }

        // Start the job
        job.Status = Job.JobStatus.InWork;
        _currentJob = job;
        _display.UpdateJobStatus(_currentJob.ToString());
        return true;
    }

    public void CompleteCurrentJob()
    {
        if (_currentJob != null && _currentJob.Status == Job.JobStatus.InWork)
        {
            _currentJob.Produce(_currentJob.Quantity - _currentJob.QuantityProduced);
            _display.UpdateMessage($"Completed {_currentJob}");
            _display.UpdateJobStatus("No job running");
            _currentJob = null;
        }
    }

    public void StopCurrentJob(int quantityProduced)
    {
        if (_currentJob != null && _currentJob.Status == Job.JobStatus.InWork)
        {
            _currentJob.Produce(quantityProduced);
            _display.UpdateMessage($"Stopped {_currentJob} with {_currentJob.QuantityProduced} produced.");
            _display.UpdateJobStatus("No job running");
            _currentJob = null;
        }
    }

    public void ListAllJobs()
    {
        if (_jobs.Count == 0)
        {
            _display.UpdateMessage("No jobs have been created yet.");
            return;
        }

        _display.ClearJobList();
        foreach (Job job in _jobs.OrderBy(j => j.JobId))
        {
            _display.AddToJobList(job.ToString());
        }
        _display.UpdateMessage($"Total Jobs: {_jobs.Count}");
    }

    public void GetJobStatus(int jobId)
    {
        Job job = _jobs.Find(j => j.JobId == jobId);
        if (job == null)
        {
            _display.UpdateMessage($"Job #{jobId} not found.");
            return;
        }

        _display.UpdateMessage(job.ToString());
    }

    public Job GetCurrentJob()
    {
        return _currentJob;
    }

    public void CancelJob(int jobId)
    {
        Job job = _jobs.Find(j => j.JobId == jobId);
        if (job == null)
        {
            _display.UpdateMessage($"Job #{jobId} not found.");
            return;
        }

        if (job.Status == Job.JobStatus.Done)
        {
            _display.UpdateMessage($"Job #{jobId} is already completed and cannot be cancelled.");
            return;
        }

        if (job == _currentJob)
        {
            _currentJob = null;
            _display.UpdateJobStatus("No job running");
        }

        _jobs.Remove(job);
        _display.UpdateMessage($"Cancelled and removed Job #{jobId}");
    }
}
