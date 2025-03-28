using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using LAMA.Services;

namespace LAMA.Core
{
    public partial class PreferredDoctorsPage : ContentPage
    {
        private readonly FirestoreServices _firestoreService = new FirestoreServices();

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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadDoctorsFromFirestore();
        }

        private async Task LoadDoctorsFromFirestore()
        {
            var doctors = await _firestoreService.GetPreferredDoctorsAsync();
            PreferredDoctors.Clear();
            foreach (var doc in doctors)
            {
                PreferredDoctors.Add(doc);
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            searchQuery = e.NewTextValue?.Trim() ?? "";
        }

        private async void OnAddDoctorClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var matchingDoctor = AllDoctors.FirstOrDefault(d => d.ToLower().Contains(searchQuery.ToLower()));
                if (matchingDoctor != null && !PreferredDoctors.Any(d => d.Name == matchingDoctor))
                {
                    var doctor = new Doctor { Name = matchingDoctor };
                    PreferredDoctors.Add(doctor);

                    // Save to Firestore
                    var sampleModel = new SampleModel
                    {
                        Name = doctor.Name,
                        Description = "",
                        Created = DateTime.UtcNow
                    };
                    await _firestoreService.InsertSampleModel(sampleModel);
                }
            }
        }

        private async void OnRemoveSelectedDoctorsClicked(object sender, EventArgs e)
        {
            var doctorsToRemove = PreferredDoctors.Where(d => d.IsSelected).ToList();
            foreach (var doctor in doctorsToRemove)
            {
                PreferredDoctors.Remove(doctor);
                // Remove from Firestore by name (assuming name is unique)
                await _firestoreService.DeleteDoctorByNameAsync(doctor.Name);
            }
        }
    }

    public class Doctor
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
