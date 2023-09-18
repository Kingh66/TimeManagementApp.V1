using System;
using System.Windows;
using TimeManagementApp.V1;

namespace TimeManagementApp.V1
{
    public partial class UpdateModuleDialog : Window
    {
        private MainWindow.Module _currentModule;

        public UpdateModuleDialog(MainWindow.Module module)
        {
            InitializeComponent();

            _currentModule = module;

            // Now, bind the current module properties to the dialog's fields.
            UpdateModuleCode.Text = _currentModule.Code;
            UpdateModuleName.Text = _currentModule.Name;
            UpdateModuleCredits.Text = _currentModule.Credits.ToString();
            UpdateClassHoursPerWeek.Text = _currentModule.ClassHoursPerWeek.ToString();
            UpdateNumWeeks.Text = _currentModule.NumWeeks.ToString();
            UpdateDate.SelectedDate = _currentModule.Date;
            UpdateDate.DisplayDate = _currentModule.Date;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate the input
                if (int.TryParse(UpdateModuleCredits.Text, out int credits) &&
                    int.TryParse(UpdateClassHoursPerWeek.Text, out int hours) &&
                    int.TryParse(UpdateNumWeeks.Text, out int weeks))
                {
                    // Update the _currentModule properties
                    _currentModule.Code = UpdateModuleCode.Text;
                    _currentModule.Name = UpdateModuleName.Text;
                    _currentModule.Credits = credits;
                    _currentModule.ClassHoursPerWeek = hours;
                    _currentModule.NumWeeks = weeks;

                    // Ensure a date is selected before trying to access its value.
                    if (UpdateDate.SelectedDate.HasValue)
                    {
                        _currentModule.Date = UpdateDate.SelectedDate.Value;
                    }
                    else
                    {
                        MessageBox.Show("Please select a valid date for the module.");
                        return; // Exit the method without closing the dialog.
                    }

                    // Signal success and close
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please enter valid details for the module.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the module: {ex.Message}");
            }
        }
    }
}
