using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace LAMA.Core
{
    public partial class PreferredDoctorsPage : ContentPage
    {
        public ObservableCollection<Doctor> PreferredDoctors { get; set; } = new ObservableCollection<Doctor>();
        public ObservableCollection<string> AllDoctors { get; set; } = new ObservableCollection<string>
        {
            "Dr. John Smith", "Dr. Sarah Johnson", "Dr. Emily Davis",
            "Dr. Michael Brown", "Dr. James Wilson", "Dr. Anna Lee"
        };

        private string searchQuery = "";

        public PreferredDoctorsPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            searchQuery = e.NewTextValue?.Trim() ?? "";
        }

        private void OnAddDoctorClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var matchingDoctor = AllDoctors.FirstOrDefault(d => d.ToLower().Contains(searchQuery.ToLower()));

                if (matchingDoctor != null && !PreferredDoctors.Any(d => d.Name == matchingDoctor))
                {
                    PreferredDoctors.Add(new Doctor { Name = matchingDoctor });
                }
            }
        }

        private void OnRemoveSelectedDoctorsClicked(object sender, EventArgs e)
        {
            var doctorsToRemove = PreferredDoctors.Where(d => d.IsSelected).ToList();
            foreach (var doctor in doctorsToRemove)
            {
                PreferredDoctors.Remove(doctor);
            }
        }
    }

    public class Doctor
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
