using Firebase.Auth.Requests;
using Google.Api;
using Google.Cloud.Firestore;
using LAMA.Core.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LAMA.Core;

namespace LAMA.Services
{
    public class FirestoreServices
    {
        private FirestoreDb db;

        private async Task SetupFirestore()
        {
            if (db == null)
            {
                var stream = await FileSystem.OpenAppPackageFileAsync("admin-sdk.json");
                using var reader = new StreamReader(stream);
                var contents = await reader.ReadToEndAsync();

                db = new FirestoreDbBuilder
                {
                    ProjectId = "lama-60ddc",
                    JsonCredentials = contents
                }.Build();
            }
        }

        public async Task InsertSampleModel(SampleModel sample)
        {
            await SetupFirestore();
            await db.Collection("SampleModels").AddAsync(sample);
        }

        public async Task<List<SampleModel>> GetSampleModel()
        {
            await SetupFirestore();
            var data = await db.Collection("SampleModels").GetSnapshotAsync();

            return data.Documents.Select(doc =>
            {
                var sampleModel = doc.ConvertTo<SampleModel>();
                sampleModel.Id = doc.Id;
                return sampleModel;
            }).ToList();
        }

        public async Task SendMessage(Message message)
        {
            await SetupFirestore();
            DocumentReference docRef = db.Collection("messages").Document();
            await docRef.SetAsync(message);
        }

        public async Task ListenForMessages(string userId, Action<List<Message>> onMessageUpdated)
        {
            await SetupFirestore();
            var query = db.Collection("messages")
                .WhereEqualTo("ReceiverId", userId)
                .OrderBy("SentAt");

            query.Listen(snapshot =>
            {
                var messages = snapshot.Documents.Select(doc => doc.ConvertTo<Message>()).ToList();
                onMessageUpdated(messages);
            });
        }

        public async Task<List<Doctor>> GetAllMedicalProvidersAsync()
        {
            await SetupFirestore();
            var snapshot = await db.Collection("medical_providers").GetSnapshotAsync();

            var doctors = new List<Doctor>();

            foreach (var doc in snapshot.Documents)
            {
                var data = doc.ToDictionary();

                try
                {
                    var firstName = data.ContainsKey("firstName") ? data["firstName"].ToString() : null;
                    var lastName = data.ContainsKey("lastName") ? data["lastName"].ToString() : null;

                    if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                    {
                        doctors.Add(new Doctor
                        {
                            Id = doc.Id,
                            FirstName = firstName,
                            LastName = lastName,
                            IsSelected = false
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing doctor doc {doc.Id}: {ex.Message}");
                }
            }

            return doctors;
        }

        public async Task DeleteMedicalProviderAsync(string firstName, string lastName)
        {
            await SetupFirestore();
            var snapshot = await db.Collection("medical_providers")
                .WhereEqualTo("firstName", firstName)
                .WhereEqualTo("lastName", lastName)
                .GetSnapshotAsync();

            foreach (var doc in snapshot.Documents)
            {
                await doc.Reference.DeleteAsync();
            }
        }

        public async Task DeleteMedicalProviderByIdAsync(string doctorId)
        {
            await SetupFirestore();
            await db.Collection("medical_providers").Document(doctorId).DeleteAsync();
        }
    }

    [FirestoreData]
    public class SampleModel
    {
        [FirestoreProperty] public string Id { get; set; }
        [FirestoreProperty] public string Name { get; set; }
        [FirestoreProperty] public string Description { get; set; }
        [FirestoreProperty] public DateTime Created { get; set; }
    }
}
