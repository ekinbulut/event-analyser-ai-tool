using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Toolchains.MonoAotLLVM;
using Microsoft.Extensions.AI;

namespace analyser.console.cli
{
    internal class OllamaAIEngine
    {
        private IChatClient _chatClient;
        private Uri _uri;
        private List<ChatMessage> _chatHistory = new();
        public OllamaAIEngine(string uri, string model)
        {
            _uri = new Uri(uri);
            _chatClient = new OllamaChatClient(_uri, model);
        }

        public async Task<List<ChatMessage>> QueryAsync(string text, CancellationToken token)
        {
            _chatHistory.Add(new ChatMessage(ChatRole.User, text));
            var response = "";

            await foreach (var item in _chatClient.GetStreamingResponseAsync(_chatHistory, null, token))
            {
                //Console.Write(item.Text);
                response += item.Text;
            }
            _chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
            return _chatHistory;
        }

        public async Task<List<ChatMessage>> ChatAsync(string text, CancellationToken token)
        {
            var response = "";
            _chatHistory.Add(new ChatMessage(ChatRole.User, text));
            var chatResponse = await _chatClient.GetResponseAsync(_chatHistory, null,  token);
            response += chatResponse.Text;
            _chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
            return _chatHistory;
        }


    }
}
