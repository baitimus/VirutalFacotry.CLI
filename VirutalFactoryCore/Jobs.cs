using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirutalFactoryCore
{
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

        // Original constructor
        public Job(int jobId, string productName, int quantity)
        {
            JobId = jobId;
            ProductName = productName;
            Quantity = quantity;
            QuantityProduced = 0;
            Status = JobStatus.Pending;
        }

        // New constructor for loading jobs from file
        public Job(int jobId, string productName, int quantity, int quantityProduced, JobStatus status)
        {
            JobId = jobId;
            ProductName = productName;
            Quantity = quantity;
            QuantityProduced = quantityProduced;
            Status = status;
        }

        public void Produce(int quantity)
        {
            QuantityProduced = quantity;
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
        
        private Job _currentJob;

        public JobManager()
        {
         
            _currentJob = null;

            // Load jobs from file
            _jobs = JobPersistence.LoadJobs();

            // Find highest job ID to set next ID correctly
            _nextJobId = 1;
            if (_jobs.Count > 0)
            {
                _nextJobId = _jobs.Max(j => j.JobId) + 1;
            }

           // _display.UpdateMessage($"Loaded {_jobs.Count} jobs from storage");
        }

        public Job CreateJob(string productName, int quantity)
        {




            Job newJob = new Job(_nextJobId++, productName, quantity);
            _jobs.Add(newJob);
            //  _display.UpdateMessage($"Created {newJob}");

            // Save jobs after creating a new one
            SaveJobs();

            return newJob;
        }

        public void SaveJobs()
        {
            JobPersistence.SaveJobs(_jobs);
        }

        public bool StartJob(int jobId)
        {
            // Check if a job is already running
            if (_currentJob != null && _currentJob.Status == Job.JobStatus.InWork)
            {
               // _display.UpdateMessage($"Cannot start job #{jobId}. Job #{_currentJob.JobId} is already in progress.");
                return false;
            }

            // Find the job with the given ID
            Job job = _jobs.Find(j => j.JobId == jobId);
            if (job == null)
            {
               // _display.UpdateMessage($"Job #{jobId} not found.");
                return false;
            }

            // Check if the job is already completed
            if (job.Status == Job.JobStatus.Done)
            {
                // _display.UpdateMessage($"Job #{jobId} is already completed.");
                return false;
            }

            // Start the job
            job.Status = Job.JobStatus.InWork;
            _currentJob = job;
            //_display.UpdateJobStatus(_currentJob.ToString());

            // Save job status change
            SaveJobs();

            return true;
        }

        public void CompleteCurrentJob()
        {
            if (_currentJob != null && _currentJob.Status == Job.JobStatus.InWork)
            {
                _currentJob.Produce(_currentJob.Quantity - _currentJob.QuantityProduced);
               // _display.UpdateMessage($"Completed {_currentJob}");
                //_display.UpdateJobStatus("No job running");
                _currentJob = null;

                // Save job status change
                SaveJobs();
            }
        }

        public void UpdateCurrentJobProgress(int progress)
        {
            if (_currentJob != null && _currentJob.Status == Job.JobStatus.InWork)
            {
                _currentJob.Produce(progress);

                // Save job progress change
                SaveJobs();
            }
        }

        public void ListAllJobs()
        {
            if (_jobs.Count == 0)
            {
                
                return;
            }

            

            // Group jobs by status
            var completedJobs = _jobs.Where(j => j.Status == Job.JobStatus.Done).OrderBy(j => j.JobId);
            var pendingJobs = _jobs.Where(j => j.Status == Job.JobStatus.Pending).OrderBy(j => j.JobId);
            var inWorkJobs = _jobs.Where(j => j.Status == Job.JobStatus.InWork).OrderBy(j => j.JobId);

            // Add pending jobs
            foreach (Job job in pendingJobs)
            {
               
            }

            // Add in-work jobs
            foreach (Job job in inWorkJobs)
            {
               
            }

            // Add completed jobs
            foreach (Job job in completedJobs)
            {
                
            }

            //_display.UpdateMessage($"Total Jobs: {_jobs.Count} (Pending: {pendingJobs.Count()}, In Work: {inWorkJobs.Count()}, Completed: {completedJobs.Count()})");
        }

        public void GetJobStatus(int jobId)
        {
            Job job = _jobs.Find(j => j.JobId == jobId);
            if (job == null)
            {
                //_display.UpdateMessage($"Job #{jobId} not found.");
                return;
            }

            
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
                
                return;
            }

            if (job.Status == Job.JobStatus.Done)
            {
                
                return;
            }

            if (job == _currentJob)
            {
                _currentJob = null;
                
            }

            _jobs.Remove(job);
         
            // Save after cancelling a job
            SaveJobs();
        }
    }
}
