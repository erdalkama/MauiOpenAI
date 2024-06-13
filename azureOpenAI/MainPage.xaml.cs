using Azure;
using Azure.AI.OpenAI;
using System.Collections.ObjectModel;

namespace azureOpenAI
{
    public partial class MainPage : ContentPage
    {
        const string key = "yourAPIkey";
        const string endpoint = "yourEndpoint";
        const string deploymentName = "yourDeploymentName";
        public ObservableCollection<Message> Messages { get; set; }

        public MainPage()
        {
            InitializeComponent();
            Messages = new ObservableCollection<Message>();
            MessagesListView.ItemsSource = Messages;

        }

        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            string prompt = PromptEntry.Text;
            if (!string.IsNullOrWhiteSpace(prompt))
            {
                // Kullanıcı mesajını listeye ekle
                Messages.Add(new Message { Text = $"You: {prompt}" });

                // OpenAI'dan yanıt al
                string response = await GetContent(prompt);

                // Yanıtı listeye ekle
                Messages.Add(new Message { Text = $"AI: {response}" });

                // Entry alanını temizle
                PromptEntry.Text = string.Empty;
            }
        }

        public async Task<string> GetContent(string prompt)
        {
            var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));

            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = deploymentName,
                Temperature = (float)0.5,
                MaxTokens = 800,
                NucleusSamplingFactor = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            };

            chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(prompt));
            var response = await client.GetChatCompletionsAsync(chatCompletionsOptions);

            return response.Value.Choices[0].Message.Content;
        }
    }

    public class Message
    {
        public string Text { get; set; }
    }
}
