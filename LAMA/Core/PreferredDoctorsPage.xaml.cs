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
        public ObservableCollection<Doctor> AllDoctors { get; set; } = new ObservableCollection<Doctor>();

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
            var doctors = await _firestoreService.GetAllMedicalProvidersAsync();
            AllDoctors.Clear();
            foreach (var doc in doctors)
            {
                AllDoctors.Add(doc);
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
                var matchingDoctor = AllDoctors.FirstOrDefault(d =>
                    d.FullName.ToLower().Contains(searchQuery.ToLower()));

                if (matchingDoctor != null && !PreferredDoctors.Any(d => d.FullName == matchingDoctor.FullName))
                {
                    PreferredDoctors.Add(new Doctor
                    {
                        FirstName = matchingDoctor.FirstName,
                        LastName = matchingDoctor.LastName
                    });
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
                await _firestoreService.DeleteMedicalProviderAsync(doctor.FirstName, doctor.LastName);

            }
        }
    }

    public class Doctor
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsSelected { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }

}
