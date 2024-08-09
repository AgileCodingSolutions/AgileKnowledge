import { RoleType } from "../pages/models";
import { del, get, post, put, putJson } from "../utils/fetch";

const prefix = `/api/User`;

export const GetUsers = (keyword: string, page: number, pageSize: number) => {
    return get(`${prefix}/GetList?PageNumber=${page}&PageSize=${pageSize}&Filter=${keyword}`)
}

export const DisableUser = (id: string, disable: boolean) => {
    return putJson(`${prefix}/Disable`,{
        id:id,
        disable:disable
    })
}

export const UpdateUserRole = (id: string, role: RoleType) => {
    return putJson(`${prefix}/ChangeRole`, {
        id:id,
        role:role
    })
}







