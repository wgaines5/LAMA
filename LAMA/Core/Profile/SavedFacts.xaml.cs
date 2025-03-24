using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LAMA.Core.Profile;

public partial class SavedFacts : ContentPage
{
	public SavedFacts()
	{
		InitializeComponent();
		BindingContext = new SavedFactsViewModel();
	}
}

public class SavedFactsViewModel : BindableObject
{
	private ObservableCollection<MedFact> _savedFacts;
	public ObservableCollection<MedFact> SavedFacts
	{
		get => _savedFacts;
		set
		{
			_savedFacts = value;
			OnPropertyChanged();
		}
	}

	public ICommand RemoveFactCommand { get; }

	public SavedFactsViewModel()
	{
		SavedFacts = new ObservableCollection<MedFact>(LoadFacts());

		RemoveFactCommand = new Command<MedFact>(RemoveFact);
	}

	private void RemoveFact(MedFact medFact)
	{
		if (medFact != null)
		{
			SavedFacts.Remove(medFact);
			SaveFacts();
		} 
	}
	
	private void SaveFacts()
	{

	}

	private ObservableCollection<MedFact> LoadFacts()
	{
		return new ObservableCollection<MedFact>
		{
		new MedFact { Text = "Generic Fact", IsBookmarked = true }
		};
	}
}
