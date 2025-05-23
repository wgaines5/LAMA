using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using LAMA.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using LAMA.Auth;

namespace LAMA.Core
{
    public partial class PreferredDoctorsPage : ContentPage
    {
        private readonly FirestoreServices _firestoreService = new FirestoreServices();

        public ObservableCollection<Doctor> PreferredDoctors { get; set; } = new ObservableCollection<Doctor>();
        public ObservableCollection<Doctor> AllDoctors { get; set; } = new ObservableCollection<Doctor>();

        public ObservableCollection<Doctor> FilteredDoctors { get; set; } = new();
        public bool IsSuggestionsVisible { get; set; }


        private string searchQuery = "";

        public PreferredDoctorsPage()
        {
            InitializeComponent();
            BindingContext = this;

            SearchSuggestionsList.SelectionChanged += async (s, e) =>
            {
                if (e.CurrentSelection.FirstOrDefault() is Doctor selectedDoctor)
                {
                    if (!PreferredDoctors.Any(d => d.Id == selectedDoctor.Id))
                    {
                        PreferredDoctors.Add(new Doctor
                        {
                            Id = selectedDoctor.Id,
                            FirstName = selectedDoctor.FirstName,
                            LastName = selectedDoctor.LastName
                        });
                    }

                    // Clear search box & dropdown
                    SearchEntry.Text = "";
                    FilteredDoctors.Clear();
                    IsSuggestionsVisible = false;

                    OnPropertyChanged(nameof(FilteredDoctors));
                    OnPropertyChanged(nameof(IsSuggestionsVisible));
                }
            };

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Console.WriteLine("OnAppearing triggered"); // <- DEBUG
            await LoadDoctorsFromFirestore();
        }

        private async Task LoadDoctorsFromFirestore()
        {
            Console.WriteLine("Fetching doctors via REST...");

            // ✅ Check if user is signed in and token is available
            if (UserSession.Token == null)
            {
                await DisplayAlert("Error", "You must be signed in to load doctors.", "OK");
                return;
            }

            // ✅ Use the cached token for Firestore REST access
            string idToken = UserSession.Token;

            // 🔁 Call the REST-based Firestore fetch method
            var doctors = await _firestoreService.GetAllMedicalProvidersViaRestAsync(idToken);

            Console.WriteLine($"Fetched {doctors.Count} doctors");

            AllDoctors.Clear();
            foreach (var doc in doctors)
            {
                AllDoctors.Add(doc);
                Console.WriteLine($"Loaded doctor: {doc.FullName}");
            }
        }


        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            searchQuery = e.NewTextValue?.Trim() ?? "";

            FilteredDoctors.Clear();
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var matches = AllDoctors
                    .Where(d => d.FullName.ToLower().Contains(searchQuery.ToLower()))
                    .Take(5); // limit

                foreach (var doctor in matches)
                    FilteredDoctors.Add(doctor);

                IsSuggestionsVisible = FilteredDoctors.Any();
            }
            else
            {
                IsSuggestionsVisible = false;
            }

            OnPropertyChanged(nameof(FilteredDoctors));
            OnPropertyChanged(nameof(IsSuggestionsVisible));
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
                        Id = matchingDoctor.Id,
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
                await _firestoreService.DeleteMedicalProviderByIdAsync(doctor.Id);
            }
        }

        private void OnSuggestionSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Doctor selectedDoctor)
            {
                if (!PreferredDoctors.Any(d => d.Id == selectedDoctor.Id))
                {
                    PreferredDoctors.Add(new Doctor
                    {
                        Id = selectedDoctor.Id,
                        FirstName = selectedDoctor.FirstName,
                        LastName = selectedDoctor.LastName
                    });
                }

                // Reset UI
                SearchEntry.Text = "";
                FilteredDoctors.Clear();
                IsSuggestionsVisible = false;

                OnPropertyChanged(nameof(FilteredDoctors));
                OnPropertyChanged(nameof(IsSuggestionsVisible));
            }
        }
    }

    public class Doctor
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsSelected { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
