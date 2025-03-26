using Google.Cloud.Firestore.V1;
using System.Collections.ObjectModel;
using LAMA.Services;
using System.Windows.Input;

namespace LAMA;

public partial class Sample : ContentPage
{
	public Sample()
	{
		InitializeComponent();
		FirestoreServices firestoreServices = new FirestoreServices();
		BindingContext = new SampleViewModel(firestoreServices);
	}
}

public class SampleViewModel
{
	LAMA.Services.FirestoreServices FirestoreServices;

	public ObservableCollection<SampleModel> SampleData { get; set; } = [];

    public ICommand GetSampleDataCommand { get; }
    public ICommand SaveSampleDataCommand { get; }

    public SampleViewModel(FirestoreServices firestoreServices)
	{
		this.FirestoreServices = firestoreServices;

        GetSampleDataCommand = new Command(async () => await GetSampleData());
        SaveSampleDataCommand = new Command(async () => await SaveSampleData());
    }

	public async Task GetSampleData()
	{
		var x = await FirestoreServices.GetSampleModel();
		foreach (var model in x)
		{
			SampleData.Add(model);
		}
	}

	public async Task SaveSampleData()
	{
		var sample = new SampleModel
		{
			Name = "SampleName",
			Description = "Description",
			Created = DateTime.Now
		};
		await FirestoreServices.InsertSampleModel(sample);
	}
}