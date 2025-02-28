using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LAMA.Core;

public partial class MedFactsPage : ContentPage
{
	public MedFactsPage()
	{
		InitializeComponent();
		BindingContext = new MedFactsViewModel();
	}
}

public class MedFactsViewModel : INotifyPropertyChanged
{
	//
	// This class is the binding context for the Collection View.
	//
	public ObservableCollection<String> medFacts { get; set; }

	public MedFactsViewModel()
	{
		medFacts = new ObservableCollection<String>()
		{
			"The human brain contains about 86 billion neurons.",
			"Your heart beats around 100,000 times a day.",
			"The average adult human body is made up of 60% water.",
			"Bones are about five times stronger than steel of the same density.",
			"The skin is the largest organ of the human body.",
			"According to the World Health Organization, 5% of adults struggle with depression.",
			"According to the World Health Organization an estimated 13% of people aged 15-49 have HSV-2."
		};
	}
	public event PropertyChangedEventHandler PropertyChanged;
}