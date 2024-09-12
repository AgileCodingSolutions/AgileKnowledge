import { message } from "antd";
import { AuthorizeService } from "../../../services/service-proxies";

export function handleLogin(user: string, password: string, onSuccess: () => void) {

    var loginService = new AuthorizeService();

    loginService.token(user,password).then((value) => {
            message.success('登录成功');
            if (value.token) {
                localStorage.setItem('token', value.token);
                onSuccess()
            }
        }).catch(() => {
        });
}