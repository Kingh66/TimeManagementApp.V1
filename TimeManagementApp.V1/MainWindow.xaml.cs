using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TimeManagementApp.V1
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Module> modules;

        public MainWindow()
        {
            InitializeComponent();
            modules = new ObservableCollection<Module>();
            ModulesListView.ItemsSource = modules;
        }

        public class Module : INotifyPropertyChanged
        {
            private static int currentMaxId = 0; 

            public int ID { get; private set; }  

            public Module()
            {
                this.ID = ++currentMaxId;  
            }
            public string Code { get; set; }
            public string Name { get; set; }
            public int Credits { get; set; }
            public int ClassHoursPerWeek { get; set; }
            public int NumWeeks { get; set; }
            
            public List<StudySession> StudySessions { get; set; } = new List<StudySession>();


            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            public double SelfStudyHoursPerWeek => CalculateSelfStudyHours();

            private DateTime _date;

            public DateTime Date
            {
                get => _date;
                set
                {
                    if (_date != value)
                    {
                        _date = value;
                        OnPropertyChanged(nameof(Date));
                    }
                }
            }
            private double CalculateSelfStudyHours()
            {
                if (NumWeeks == 0)
                {
                    return 0;
                }
                return (double)(Credits * 10) / NumWeeks - ClassHoursPerWeek;
            }

            public int TotalHoursSpent
            {
                get
                {
                    return StudySessions.Sum(ss => ss.Hours);
                }
            }

            public string LastStudyDate
            {
                get
                {
                    var lastSession = StudySessions.OrderByDescending(ss => ss.Date).FirstOrDefault();
                    return lastSession?.Date.ToShortDateString() ?? "N/A";
                }
            }

            public void AddStudySession(StudySession session)
            {
                StudySessions.Add(session);
                OnPropertyChanged(nameof(TotalHoursSpent));
                OnPropertyChanged(nameof(LastStudyDate));
                OnPropertyChanged(nameof(HoursStudiedThisWeek));  
                OnPropertyChanged(nameof(SelfStudyHoursRemainingForWeek));  
            }
            public int HoursStudiedThisWeek
            {
                get
                {
                    var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
                    var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);  // till end of Sunday

                    return StudySessions
                           .Where(ss => ss.Date >= startOfWeek && ss.Date <= endOfWeek)
                           .Sum(ss => ss.Hours);
                }
            }
            public double SelfStudyHoursRemainingForWeek => SelfStudyHoursPerWeek - TotalHoursSpent;

        }

        public class StudySession
        {
            public DateTime Date { get; set; }
            public int Hours { get; set; }
        }//getters and setters for date and hours spent by a user

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //method for textbox.
        }

        private void Button_Click(object sender, RoutedEventArgs e) // For adding a module
        {
            if (int.TryParse(ModuleCredits.Text, out int credits) &&
                int.TryParse(ClassHoursPerWeek.Text, out int hours) &&
                int.TryParse(NumWeeks.Text, out int weeks))
            {
                var newModule = new Module
                {
                    Code = ModuleCode.Text,
                    Name = ModuleName.Text,
                    Credits = credits,
                    ClassHoursPerWeek = hours,
                    NumWeeks = weeks
                };

                modules.Add(newModule);

                ModuleCode.Text = "";
                ModuleName.Text = "";
                ModuleCredits.Text = "";
                ClassHoursPerWeek.Text = "";
                NumWeeks.Text = "";
            }
            else
            {
                MessageBox.Show("Please enter valid details for the module.");
            }
        }

        private void ModulesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (ModulesListView.SelectedItem is Module selectedModule)
            {
                MessageBox.Show($"You selected module: {selectedModule.Name}");
                ModuleCode.Text = selectedModule.Code;
                ModuleName.Text = selectedModule.Name;
                ModuleCredits.Text = selectedModule.Credits.ToString();
                ClassHoursPerWeek.Text = selectedModule.ClassHoursPerWeek.ToString();
                NumWeeks.Text = selectedModule.NumWeeks.ToString();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // For adding study hours
        {
            if (ModulesListView.SelectedItem is Module selectedModule)
            {
                var studyDialog = new StudyHoursDialog(modules.ToList());

                if (studyDialog.ShowDialog() == true)
                {
                    selectedModule.AddStudySession(studyDialog.NewStudySession);
                    MessageBox.Show($"Added {studyDialog.NewStudySession.Hours} hours of study on {studyDialog.NewStudySession.Date.ToShortDateString()} for {selectedModule.Name}.");
                    ModulesListView.Items.Refresh(); // auto refresh the list
                }
            }
            else
            {
                MessageBox.Show("Please select a module to add study hours to.");
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) // deleting modules
        {
            if (ModulesListView.SelectedItem is Module selectedModule)
            {
                // Confirm the deletion
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {selectedModule.Name}?",
                                                          "Confirm Deletion",
                                                          MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    modules.Remove(selectedModule);
                    MessageBox.Show($"Module {selectedModule.Name} has been deleted.");
                }
            }
            else
            {
                MessageBox.Show("Please select a module that you wish to delete.");
            }
        }


        private void Button_Click_2(object sender, RoutedEventArgs e) // For Updating a Module Selected by the user
{
    try
    {
        if (ModulesListView.SelectedItem is Module selectedModule)
        {
            var updateDialog = new UpdateModuleDialog(selectedModule);
            
            if (updateDialog.ShowDialog() == true)
            {
                // Refresh or rebind the ModulesListView if necessary
                ModulesListView.Items.Refresh();
            }
        }
        else
        {
            MessageBox.Show("Please select a module that you wish to Update.");
        }
    }
    catch(Exception ex)
    {
        MessageBox.Show("An error occurred: " + ex.Message);
    }
}
        private void ModuleName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ModuleName.Text == "e.g. Programming 2A")
                ModuleName.Text = "";
        }

        private void ModuleName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ModuleName.Text))
                ModuleName.Text = "e.g. Programming 2A";
        }

        private void ModuleCode_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ModuleCode.Text == "e.g. Prog6212")
                ModuleCode.Text = "";
        }

        private void ModuleCode_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ModuleCode.Text))
                ModuleCode.Text = "e.g. Prog6212";
        }

        private void ModuleCredits_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ModuleCredits.Text == "e.g. 15")
                ModuleCredits.Text = "";
        }

        private void ModuleCredits_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ModuleCredits.Text))
                ModuleCredits.Text = "e.g. 15";
        }

        private void ClassHoursPerWeek_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ClassHoursPerWeek.Text == "e.g. 7")
                ClassHoursPerWeek.Text = "";
        }

        private void ClassHoursPerWeek_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ClassHoursPerWeek.Text))
                ClassHoursPerWeek.Text = "e.g. 7";
        }

        private void NumWeeks_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NumWeeks.Text == "e.g. 12")
                NumWeeks.Text = "";
        }

        private void NumWeeks_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NumWeeks.Text))
                NumWeeks.Text = "e.g 12";
        }


    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
