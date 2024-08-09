import { config } from "../config";
import { get, post } from "../utils/fetch";

/**
 * 账号登录
 * @param account 
 * @param password 
 * @returns 
 */
export const login = (account: string, password: string) => {
    return get(`/api/Authorize/Token?account=${account}&pass=${password}`);
};