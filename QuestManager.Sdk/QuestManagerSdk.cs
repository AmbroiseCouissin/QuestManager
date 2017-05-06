using Newtonsoft.Json;
using QuestManager.ViewModels;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QuestManager.Sdk
{
    public interface IQuestManagerSdk
    {
        Task<ProgressViewModel> GetProgressAsync(ProgressQueryViewModel query);
    }

    public class QuestManagerSdk : IQuestManagerSdk
    {
        public readonly HttpClient _client;
        public QuestManagerSdk() : this(new HttpClient())
        {
        }

        public QuestManagerSdk(HttpClient client)
        {
            _client = client ?? new HttpClient();
        }

        public async Task<ProgressViewModel> GetProgressAsync(ProgressQueryViewModel query)
        {
            string uri = "/api/progress";
            string jsonInString = JsonConvert.SerializeObject(query);
            HttpResponseMessage response = await _client.PostAsync(uri, new StringContent(jsonInString, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            string progressString = await response.Content.ReadAsStringAsync();
            ProgressViewModel progress = JsonConvert.DeserializeObject<ProgressViewModel>(progressString);

            return progress;
        }
    }
}
