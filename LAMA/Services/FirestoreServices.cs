using Google.Api;
using Google.Cloud.Firestore;
using LAMA.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LAMA.Core;

namespace LAMA.Services
{
    public class FirestoreServices
    {
        public FirestoreDb db;
        private readonly HttpClient _httpClient;
        private readonly string _url;

        private async Task SetupFirestore()
        {
            if (db == null)
            {
                var stream = await FileSystem.OpenAppPackageFileAsync("admin-sdk.json");
                var reader = new StreamReader(stream);
                var contents = reader.ReadToEnd();

                db = new FirestoreDbBuilder
                {
                    ProjectId = "lama-7054a",

                    ConverterRegistry = new ConverterRegistry
                    {
                        new DateTimeToTimeStampConverter()
                    },
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
            var data = await db
                .Collection("SampleModels")
                .GetSnapshotAsync();

            var sampleModels = data.Documents
                .Select(doc =>
                {
                    var sampleModel = doc.ConvertTo<SampleModel>();
                    sampleModel.Id = doc.Id;
                    return sampleModel;
                }).ToList();
            return sampleModels;
        }

        public async Task SendMessage(ChatMessage message)
        {
            await SetupFirestore();
            DocumentReference docRef = db.Collection("messages").Document();
            await docRef.SetAsync(message);
        }

        public async void ListenForMessages(string userId, Action<List<ChatMessage>> onMessageUpdated)
        {
            await SetupFirestore();
            var query = db.Collection("messages")
                .WhereEqualTo("ReceiverId", userId)
                .OrderBy("SentAt");

            query.Listen(snapshot =>
            {
                var messages = snapshot.Documents.Select(doc => doc.ConvertTo<ChatMessage>()).ToList();

                onMessageUpdated(messages);
            });
        }
    }

        public async Task<List<Doctor>> GetPreferredDoctorsAsync()
        {
            await SetupFirestore();
            var snapshot = await db.Collection("SampleModels").GetSnapshotAsync();

            var doctors = snapshot.Documents.Select(doc =>
            {
                var model = doc.ConvertTo<SampleModel>();
                return new Doctor
                {
                    Name = model.Name,
                    IsSelected = false
                };
            }).ToList();

            return doctors;
        }

        public async Task DeleteDoctorByNameAsync(string name)
        {
            await SetupFirestore();
            var snapshot = await db.Collection("SampleModels")
                .WhereEqualTo("Name", name)
                .GetSnapshotAsync();

            foreach (var doc in snapshot.Documents)
            {
                await doc.Reference.DeleteAsync();
            }
        }
    }
    [FirestoreData]
    public class SampleModel
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string Description { get; set; }
        [FirestoreProperty]
        public DateTime Created { get; set; }
    }

    public class DateTimeToTimeStampConverter : IFirestoreConverter<DateTime>
    {
        public object ToFirestore(DateTime value) => Timestamp.FromDateTime(value.ToUniversalTime());

        public DateTime FromFirestore(object value)
        {
            if (value is Timestamp timestamp)
            {
                return timestamp.ToDateTime();
            }
            throw new ArgumentException("Invalid value");
        }
    }

    public class ChatMessageConverter : IFirestoreConverter<ChatMessage>
    {
        public ChatMessage FromFirestore(object value)
        {
            var data = value as Dictionary<string, object>;
            return new ChatMessage
            {
                SenderId = data["SenderId"] as string,
                ReceiverId = data["ReceiverId"] as string,
                Content = data["Content"] as string,
                IsUserMessage = (bool)data["IsUserMessage"],
            };
        }

        public object ToFirestore(ChatMessage message)
        {
            return new Dictionary<string, object>
        {
            { "SenderId", message.SenderId },
            { "ReceiverId", message.ReceiverId },
            { "Content", message.Content },
            { "IsUserMessage", message.IsUserMessage },
        };
        }
    }


}
