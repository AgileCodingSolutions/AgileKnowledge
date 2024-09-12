const prefix = `/api/Storage`;

/**
 * 上传文件
 * @param file 
 * @returns 
 */
export function UploadFile(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return fetch(`${prefix}/UploadFile`, {
        method: 'POST',
        body: formData
    });
}