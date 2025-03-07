﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using VirutalFactoryCore;
using System.Windows.Media;

namespace GUI_VirtualFacotry
{
    public partial class MainWindow : Window
    {
        private MachineManager _machineManager;
        private Machine _currentMachine;
        private JobPersistence JobPersistence;
        public MainWindow()
        {
            InitializeComponent();
            InitializeMachineManager();
            UpdateUI();
            JobPersistence.LoadJobs();

            
        }

        private void InitializeMachineManager()
        {
            _machineManager = new MachineManager();
            _currentMachine = new Machine(0); 
            _machineManager.AddMachine(_currentMachine);
        }

        private void UpdateUI()
        {
            UpdateMachineStatus();
            UpdateSignalLight();
            UpdateJobList();
        }

        private void UpdateMachineStatus()
        {
            txtMachineStatus.Text = _currentMachine.CurrentState.ToString();
            txtLastUpdate.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void UpdateSignalLight()
        {
            signalRed.Fill = _currentMachine.CurrentState == Machine.State.Error ? Brushes.Red : Brushes.Gray;
            signalYellow.Fill = _currentMachine.CurrentState == Machine.State.Ready ? Brushes.Yellow : Brushes.Gray;
            signalGreen.Fill = _currentMachine.CurrentState == Machine.State.Running ? Brushes.Green : Brushes.Gray;
        }

        private void UpdateJobList()
        {
            lvJobs.Items.Clear();
            var jobs = _currentMachine.JobManager.GetAllJobs(); // Assuming ListAllJobs() returns List<Job>
            foreach (var job in jobs)
            {
                lvJobs.Items.Add(new
                {
                    Name = $"Job #{job.JobId}",
                    Product = job.ProductName,
                    Produced = job.QuantityProduced,
                    Quantity = job.Quantity,
                    Status = job.Status.ToString()
                });
            }
        }

        private void btnCreateJob_Click(object sender, RoutedEventArgs e)
        {
            string productName = txtProduct.Text;
            if (int.TryParse(txtQuantity.Text, out int quantity))
            {
                _currentMachine.JobManager.CreateJob(productName, quantity);
                UpdateJobList();
                txtConsole.AppendText($"Job created for {productName} with quantity {quantity}\n");
            }
            else
            {
                MessageBox.Show("Invalid quantity. Please enter a valid number.");
            }
        }

        private void btnStartJob_Click(object sender, RoutedEventArgs e)
        {
            if (lvJobs.SelectedItem != null)
            {
                dynamic selectedJob = lvJobs.SelectedItem;
                int jobId = int.Parse(selectedJob.Name.Split('#')[1]);
                // if status is InWork, set status to Pending

                if (_currentMachine.JobManager.GetJobStatus(jobId).Status == Job.JobStatus.InWork)
                {
                   MessageBox.Show("job will be set to Pending");
                    return;
                }

                if (_currentMachine.JobManager.StartJob(jobId))
                {
                    txtConsole.AppendText($"Job #{jobId} selected.\n");
                    UpdateUI();
                }
                

            }
            else
            {
             
            }
        }

        private void btnCheckStatus_Click(object sender, RoutedEventArgs e)
        {
            if (lvJobs.SelectedItem != null)
            {
                dynamic selectedJob = lvJobs.SelectedItem;
                int jobId = int.Parse(selectedJob.Name.Split('#')[1]);
               var job = _currentMachine.JobManager.GetJobStatus(jobId);
               txtConsole.AppendText($"Job #{jobId} status: {job.Status}\n");
            }
            else
            {
                MessageBox.Show("Please select a job to check status.");
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            _currentMachine.Start();

            UpdateUI();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _currentMachine.Stop();
            UpdateUI();
            txtConsole.AppendText("Machine stopped.\n");
        }

        private void btnTriggerError_Click(object sender, RoutedEventArgs e)
        {
            
            UpdateUI();
            txtConsole.AppendText("Error triggered.\n");
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            
            UpdateUI();
            txtConsole.AppendText("Machine reset.\n");
        }

        private void btnClearConsole_Click(object sender, RoutedEventArgs e)
        {
            txtConsole.Clear();
        }
    }

    public class MultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double factor = 1.0;
            if (parameter != null)
                double.TryParse(parameter.ToString(), out factor);

            return (double)value * factor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}