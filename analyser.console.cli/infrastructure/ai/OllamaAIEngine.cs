using Microsoft.Extensions.AI;

namespace analyser.console.cli.infrastructure.ai
{
    public interface IAIEngine
    {
        Task<List<ChatMessage>> ChatWithStreamingAsync(string text, CancellationToken token);
        Task<List<ChatMessage>> ChatAsync(string text, CancellationToken token);
    }

    public class OllamaAIEngine : IAIEngine
    {
        private readonly IChatClient _chatClient;
        private readonly Uri _uri;
        private readonly List<ChatMessage> _chatHistory = new();
        public OllamaAIEngine(string uri, string model)
        {
            _uri = new Uri(uri);
            _chatClient = new OllamaChatClient(_uri, model);
        }

        /// <summary>
        /// Sends a user message to the AI and processes a streaming response.
        /// </summary>
        /// <param name="text">User input prompt.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Updated chat history.</returns>
        public async Task<List<ChatMessage>> ChatWithStreamingAsync(string text, CancellationToken token)
        {
            _chatHistory.Add(new ChatMessage(ChatRole.User, text));
            var response = await GetStreamingResponseAsync(token);
            _chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
            return _chatHistory;
        }

        /// <summary>
        /// Sends a user message to the AI and processes a single response.
        /// </summary>
        /// <param name="text">User input prompt.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Updated chat history.</returns>
        public async Task<List<ChatMessage>> ChatAsync(string text, CancellationToken token)
        {
            var response = "";
            _chatHistory.Add(new ChatMessage(ChatRole.User, text));
            var chatResponse = await _chatClient.GetResponseAsync(_chatHistory, null,  token);
            response += chatResponse.Text;
            _chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
            return _chatHistory;
        }

        private async Task<string> GetStreamingResponseAsync(CancellationToken token)
        {
            var response = "";
            await foreach (var item in _chatClient.GetStreamingResponseAsync(_chatHistory, null, token))
            {
                response += item.Text;
            }
            return response;
        }
    }
}
