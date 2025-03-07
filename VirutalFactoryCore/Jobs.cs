using System;
using System.Collections.Generic;
using System.Linq;

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

        public Job(int jobId, string productName, int quantity)
        {
            JobId = jobId;
            ProductName = productName;
            Quantity = quantity;
            QuantityProduced = 0;
            Status = JobStatus.Pending;
        }

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
            _jobs = new List<Job>();
            _nextJobId = 1;
            _currentJob = null;
        }

        public Job CreateJob(string productName, int quantity)
        {
            Job newJob = new Job(_nextJobId++, productName, quantity);
            _jobs.Add(newJob);
            return newJob;
        }

        public bool StartJob(int jobId)
        {
            if (_currentJob != null && _currentJob.Status == Job.JobStatus.InWork)
            {
                return false;
            }

            Job job = _jobs.Find(j => j.JobId == jobId);
            if (job == null || job.Status == Job.JobStatus.Done)
            {
                return false;
            }

            job.Status = Job.JobStatus.InWork;
            _currentJob = job;
            return true;
        }

        public void CompleteCurrentJob()
        {
            if (_currentJob != null && _currentJob.Status == Job.JobStatus.InWork)
            {
                _currentJob.Produce(_currentJob.Quantity);
                _currentJob = null;
            }
        }

        public void UpdateCurrentJobProgress(int progress)
        {
            if (_currentJob != null && _currentJob.Status == Job.JobStatus.InWork)
            {
                _currentJob.Produce(progress);
            }
        }

        public Job GetCurrentJob()
        {
            return _currentJob;
        }

        public List<Job> GetAllJobs()
        {
            return _jobs;
        }

        public Job GetJobStatus(int jobId)
        {
            return _jobs.Find(j => j.JobId == jobId);
        }

        public void CancelJob(int jobId)
        {
            Job job = _jobs.Find(j => j.JobId == jobId);
            if (job != null && job.Status != Job.JobStatus.Done)
            {
                if (job == _currentJob)
                {
                    _currentJob = null;
                }
                _jobs.Remove(job);
            }
        }
    }
}