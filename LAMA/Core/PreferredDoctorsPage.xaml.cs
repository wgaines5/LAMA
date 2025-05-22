using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace LAMA.Core
{
    public partial class PreferredDoctorsPage : ContentPage
    {
        public ObservableCollection<Doctor> PreferredDoctors { get; set; } = new();
        public ObservableCollection<Doctor> AllDoctors { get; set; } = new();
        public ObservableCollection<Doctor> FilteredDoctors { get; set; } = new();

        public bool IsSuggestionsVisible { get; set; }

        private string searchQuery = "";

        public PreferredDoctorsPage()
        {
            InitializeComponent();
            BindingContext = this;

            SearchSuggestionsList.SelectionChanged += (s, e) =>
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

                    SearchEntry.Text = "";
                    FilteredDoctors.Clear();
                    IsSuggestionsVisible = false;

                    OnPropertyChanged(nameof(FilteredDoctors));
                    OnPropertyChanged(nameof(IsSuggestionsVisible));
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadStaticDoctors();
        }

        private void LoadStaticDoctors()
        {
            AllDoctors.Clear();

            var hardcodedDoctors = new[]
            {
                new Doctor { Id = "1", FirstName = "Alice", LastName = "Smith" },
                new Doctor { Id = "2", FirstName = "Bob", LastName = "Jones" },
                new Doctor { Id = "3", FirstName = "Carol", LastName = "Taylor" },
                new Doctor { Id = "4", FirstName = "David", LastName = "Lee" },
                new Doctor { Id = "5", FirstName = "Eva", LastName = "Brown" },
            };

            foreach (var doc in hardcodedDoctors)
            {
                AllDoctors.Add(doc);
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
                    .Take(5);

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

        private void OnAddDoctorClicked(object sender, EventArgs e)
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
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsSelected { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}