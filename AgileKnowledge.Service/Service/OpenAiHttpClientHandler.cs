using AgileKnowledge.Service.Options;

namespace AgileKnowledge.Service.Service
{
	public class OpenAiHttpClientHandler: HttpClientHandler
	{
		private readonly string _uri;

		public OpenAiHttpClientHandler()
		{
		}

		public OpenAiHttpClientHandler(string uri)
		{
			_uri = uri;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			UriBuilder uriBuilder;
			if (!string.IsNullOrWhiteSpace(OpenAIOption.ChatEndpoint) && request.RequestUri?.LocalPath == "/v1/chat/completions")
			{
				uriBuilder = string.IsNullOrWhiteSpace(_uri) ? new UriBuilder(OpenAIOption.ChatEndpoint.TrimEnd('/') + "/v1/chat/completions") : new UriBuilder(_uri.TrimEnd('/') + "/v1/chat/completions");
				request.RequestUri = uriBuilder.Uri;
			}
			else if (!string.IsNullOrWhiteSpace(OpenAIOption.EmbeddingEndpoint) &&
			         request.RequestUri?.LocalPath == "/v1/embeddings")
			{
				uriBuilder = string.IsNullOrWhiteSpace(_uri) ? new UriBuilder(OpenAIOption.EmbeddingEndpoint.TrimEnd('/') + "/v1/embeddings") : new UriBuilder(_uri.TrimEnd('/') + "/v1/embeddings");
				request.RequestUri = uriBuilder.Uri;
			}
        
			return await base.SendAsync(request, cancellationToken);
		}


	}
}
