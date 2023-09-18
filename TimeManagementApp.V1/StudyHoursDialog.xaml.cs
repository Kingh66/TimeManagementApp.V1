using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static TimeManagementApp.V1.MainWindow;

namespace TimeManagementApp.V1
{
    /// <summary>
    /// Interaction logic for StudyHoursDialog.xaml
    /// </summary>
    public partial class StudyHoursDialog : Window
    {
        public Module SelectedModule { get; private set; }
        public StudySession NewStudySession { get; private set; }

        public StudyHoursDialog(List<Module> modules)
        {
            InitializeComponent();

            ModuleSelectionComboBox.ItemsSource = modules;
            ModuleSelectionComboBox.DisplayMemberPath = "Name"; // Display the name of the module.
            ModuleSelectionComboBox.Text = "Please select a module"; // drop down option that allows the user to select any exist module
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedModule = ModuleSelectionComboBox.SelectedItem as Module;

            if (SelectedModule == null)
            {
                MessageBox.Show("Please select a module.");
                return;
            }

            if (!int.TryParse(HoursSpentTextBox.Text, out int hours))
            {
                MessageBox.Show("Please enter a valid number of hours.");
                return;
            }

            if (!StudyDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a date.");
                return;
            }

            NewStudySession = new StudySession
            {
                Date = StudyDate.SelectedDate.Value,
                Hours = hours
            };

            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ModuleSelectionComboBox_SelectionChanged()
        {

        }
    }
}
