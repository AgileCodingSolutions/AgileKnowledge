
interface ChatModel {
    label: string;
    value: string;
}

interface EmbeddingModel {
    label: string;
    value: string;
}

interface Models {
    chatModel: ChatModel[];
    embeddingModel: EmbeddingModel[];
}

let models: Models;

async function loadingModel() {
    let options_: RequestInit = {
        method: "GET",
        headers: {
            
            "Accept": "text/plain"
        }
    };
    const response = await fetch('/model.json',options_);
    models = await response.json();

}

export async function getModels() {
    if (!models) {
        await loadingModel();
    }
    return models;
}