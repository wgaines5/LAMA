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
using System.Text.Json;

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

        public async Task<List<Doctor>> GetAllMedicalProvidersViaRestAsync(string idToken)
        {
            var doctors = new List<Doctor>();

            string url = $"https://firestore.googleapis.com/v1/projects/lama-60ddc/databases/(default)/documents/medical_providers?access_token={idToken}";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Firestore REST call failed: {response.StatusCode}");
                Console.WriteLine(content);
                return doctors;
            }

            using var jsonDoc = JsonDocument.Parse(content);
            foreach (var doc in jsonDoc.RootElement.GetProperty("documents").EnumerateArray())
            {
                var fields = doc.GetProperty("fields");

                string GetField(string key)
                {
                    return fields.TryGetProperty(key, out var val) && val.TryGetProperty("stringValue", out var str)
                        ? str.GetString()
                        : "";
                }

                doctors.Add(new Doctor
                {
                    Id = doc.GetProperty("name").ToString().Split('/').Last(),
                    FirstName = GetField("firstName"),
                    LastName = GetField("lastName"),
                    IsSelected = false
                });
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
