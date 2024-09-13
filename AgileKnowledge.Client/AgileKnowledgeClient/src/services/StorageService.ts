import { StorageService } from "./service-proxies";

const prefix = `/api/Storage`;

/**
 * 上传文件
 * @param file 
 * @returns 
 */
export function UploadFile(file: File) {

    var storageService = new StorageService();
    const fileParameter = {
        data: file,
        fileName: file.name // 你可以根据需要设置文件名
    };
    return storageService.uploadFile(fileParameter);
}