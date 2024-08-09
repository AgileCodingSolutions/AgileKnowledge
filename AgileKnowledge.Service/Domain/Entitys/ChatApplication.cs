using AgileKnowledge.Service.Domain.BaseEntity;

namespace AgileKnowledge.Service.Domain.Enities
{
    public class ChatApplication : FullAuditedEntity
    {
        public string Name { get; set; }

        public string Prompt { get; set; } = string.Empty;

        public string ChatModel { get; set; } = "gpt-3.5-turbo";

        public double Temperature { get; set; } = 0;

        public int MaxResponseToken { get; set; } = 2000;

        public string Template { get; set; } = string.Empty;

        public string Opener { get; set; }

        public virtual ICollection<KnowledgeBase> KnowledgeBases { get; set; }

        public virtual ICollection<ChatDialog> ChatDialogs { get; set; }

        protected ChatApplication()
        {
        }

        public ChatApplication(string name) 
        {
	        Opener =
		        """
		        FastWiki本项目是一个高性能、基于最新技术栈的知识库系统，专为大规模信息检索和智能搜索设计。
		            利用微软Semantic Kernel进行深度学习和自然语言处理，结合.NET 8和`MasaBlazor`前端框架，后台采用`MasaFramework`，实现了一个高效、易用、可扩展的智能向量搜索平台。
		            我们的目标是提供一个能够理解和处理复杂查询的智能搜索解决方案，帮助用户快速准确地获取所需信息。采用Apache-2.0，您也可以完全商用不会有任何版权纠纷
		        [Github](https://github.com/239573049/fast-wiki)
		        [Gitee](https://gitee.com/hejiale010426/fast-wiki)
		        [项目文档](https://docs.token-ai.cn/)

		        当前AI提供了Avalonia中文文档知识库功能！
		        """;

	        Template =
		        """"
		        使用 <data></data> 标记中的内容作为你的知识:
		            <data>
		            {{quote}}
		            </data>

		        回答要求：
		        - 如果你不清楚答案，你需要澄清。
		        - 避免提及你是从 <data></data> 获取的知识。
		        - 保持答案与 <data></data> 中描述的一致。
		        - 使用 Markdown 语法优化回答格式。
		        - 使用与问题相同的语言回答。
		        - 如果 Markdown中有图片则正常显示。

		        问题:"""{{question}}"""
		        """";

	        Name = name;
        }
    }
}
