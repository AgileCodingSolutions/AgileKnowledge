using System.Text;
using System.Text.Json;
using Azure.AI.OpenAI;
using Microsoft.KernelMemory.DataFormats.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AgileKnowledge.Service.Service;

public class OpenAIService
{
	public static async IAsyncEnumerable<string> QaAsync(string prompt, string value, string model, string apiKey,
		string url,
		KnowledgeMemoryService knowledgeMemoryService)
	{
		var kernel = knowledgeMemoryService.CreateFunctionKernel(apiKey, model, url);
		var qaFunction = kernel.CreateFunctionFromPrompt(prompt, functionName: "QA", description: "QA问答");

		var lines = TextChunker.SplitPlainTextLines(value, 299);
		var paragraphs = TextChunker.SplitPlainTextParagraphs(lines, 4000);

		foreach (var paragraph in paragraphs)
		{
			var result = await kernel.InvokeAsync(qaFunction, new KernelArguments()
			{
				{
					"input", paragraph
				}
			});

			yield return result.GetValue<string>();
		}
	}
}