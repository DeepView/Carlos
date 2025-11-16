using System;
using System.Text;
using OllamaSharp;
using Microsoft.Extensions.AI;
using System.Collections.Generic;
namespace Carlos.AI.Chat
{
    /// <summary>
    /// 一个可以连接到Ollama本地服务器的轻量级AI聊天客户端。
    /// </summary>
    /// <param name="url">Ollama本地服务器的URL，包含端口。</param>
    /// <param name="model">本地模型的名称，包含参数量，比如说deepseek-r1:8b。</param>
    public class LocalLiteClient(string url, string model)
    {
        /// <summary>
        /// 获取或设置Ollama本地服务器的URL地址。
        /// </summary>
        public string Url { get; set; } = url;
        /// <summary>
        /// 获取或设置使用的模型名称，比如说deepseek-r1:8b。
        /// </summary>
        public string Model { get; set; } = model;
        /// <summary>
        /// 获取当前的聊天客户端实例。
        /// </summary>
        public IChatClient Client { get; private set; } = new OllamaApiClient(new Uri(url), model);
        /// <summary>
        /// 获取当前的聊天历史记录。
        /// </summary>
        public List<ChatMessage> ChatHistory { get; private set; } = [];
        /// <summary>
        /// 重置聊天历史记录。
        /// </summary>
        public void ResetHistory() => ChatHistory.Clear();
        /// <summary>
        /// 重新初始化聊天客户端。
        /// </summary>
        public void ReInitializeClient()
        {
            Client = new OllamaApiClient(new Uri(Url), Model);
            ResetHistory();
        }
        /// <summary>
        /// 获取模型对指定提示的响应，并通过回调函数逐块返回响应内容。
        /// </summary>
        /// <param name="prompt">指定的提示或者提问。</param>
        /// <param name="onReceiveChunk">一个回调函数，获取返回的响应内容。</param>
        public void GetResponse(string prompt, Action<string> onReceiveChunk)
        {
            var chatMessages = new List<ChatMessage>(ChatHistory)
            {
                new(ChatRole.User, prompt)
            };
            var fullResponseBuilder = new StringBuilder();
            var responseStream = Client.GetStreamingResponseAsync(chatMessages);
            var enumerator = responseStream.GetAsyncEnumerator();
            try
            {
                while (enumerator.MoveNextAsync().AsTask().Result)
                {
                    var chunk = enumerator.Current;
                    fullResponseBuilder.Append(chunk.Text);
                    onReceiveChunk?.Invoke(chunk.Text);
                }
            }
            finally
            {
                enumerator.DisposeAsync().AsTask().Wait();
            }
            AddHistory(new ChatMessage(ChatRole.User, prompt));
            AddHistory(new ChatMessage(ChatRole.Assistant, fullResponseBuilder.ToString()));
        }
        /// <summary>
        /// 新增一条聊天记录到历史记录中。
        /// </summary>
        /// <param name="message">一条聊天记录。</param>
        private void AddHistory(ChatMessage message) => ChatHistory.Add(message);
    }
}
