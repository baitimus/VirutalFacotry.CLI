// JobPersistence.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class JobPersistence
{
    private const string JobsFileName = "jobs.json";

    // Serializable job class for JSON conversion
    private class SerializableJob
    {
        public int JobId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int QuantityProduced { get; set; }
        public Job.JobStatus Status { get; set; }

        // Convert from Job to SerializableJob
        public static SerializableJob FromJob(Job job)
        {
            return new SerializableJob
            {
                JobId = job.JobId,
                ProductName = job.ProductName,
                Quantity = job.Quantity,
                QuantityProduced = job.QuantityProduced,
                Status = job.Status
            };
        }

        // Convert from SerializableJob to Job
        public Job ToJob()
        {
            return new Job(JobId, ProductName, Quantity, QuantityProduced, Status);
        }
    }

    // Save jobs to file
    public static void SaveJobs(List<Job> jobs)
    {
        try
        {
            // Convert jobs to serializable format
            var serializableJobs = new List<SerializableJob>();
            foreach (var job in jobs)
            {
                serializableJobs.Add(SerializableJob.FromJob(job));
            }

            // Serialize to JSON
            string jsonString = JsonSerializer.Serialize(serializableJobs, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Write to file
            File.WriteAllText(JobsFileName, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving jobs: {ex.Message}");
        }
    }

    // Load jobs from file
    public static List<Job> LoadJobs()
    {
        var jobs = new List<Job>();

        try
        {
            // Check if file exists
            if (!File.Exists(JobsFileName))
            {
                return jobs;
            }

            // Read JSON from file
            string jsonString = File.ReadAllText(JobsFileName);

            // Deserialize JSON
            var serializableJobs = JsonSerializer.Deserialize<List<SerializableJob>>(jsonString);

            // Convert to Job objects
            foreach (var serializedJob in serializableJobs)
            {
                jobs.Add(serializedJob.ToJob());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading jobs: {ex.Message}");
        }

        return jobs;
    }
}