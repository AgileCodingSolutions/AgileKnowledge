import { Table } from 'antd';
import { useEffect, useState } from 'react';
import { Button, Dropdown, MenuProps,message } from 'antd';
import { ChangeRoleInputDto, DisableInputDto, RoleType, UserDto, UserService } from '../../../services/service-proxies';

interface IUserListProps {
    keyword: string;
}

export default function UserList({
    keyword
}:IUserListProps) {

    var userService = new UserService();



    const [input, setInput] = useState({
        page: 1,
        pageSize: 10
    });
    const [total, setTotal] = useState(0);
    const [data, setData] = useState([]);
    async function handleTableChange(page: number, pageSize: number) {
        setInput({
            ...input,
            page: page,
            pageSize: pageSize,
        });
        await loadingData();
    }

    async function ResetLoading(){
        setInput({
            ...input,
            page: 1,
            pageSize: 10
        });
        await loadingData();
    }

    async function loadingData() {
        try {
            const result = await userService.getList(keyword, "", input.page, input.pageSize)

            const updatedItems = result.items!.map((item: UserDto, index : number) => {
                return {
                    ...item,
                    key: index + 1
                };
            });
            setData(updatedItems as any);
            setTotal(result.totalCount!);
        } catch (error) {

        }
    }

    useEffect(() => {
        loadingData();
    }, [keyword]);

    useEffect(() => {
        loadingData();
    }, [input]);


    const columns = [
        {
            title: '账户',
            dataIndex: 'account',
            key: 'account',
        },
        {
            title: '昵称',
            dataIndex: 'name',
            key: 'name',
        },
        {
            title: '邮箱',
            dataIndex: 'email',
            key: 'email',
        },
        {
            title: '手机号',
            dataIndex: 'phone',
            key: 'phone',
        },
        {
            title: '是否禁用',
            dataIndex: 'isDisable',
            key: 'isDisable',
            render: (text: boolean) => text ? '是' : '否'
        },
        {
            title: '角色',
            dataIndex: 'roleName',
            key: 'roleName'
        },
        {
            title: '操作',
            dataIndex: 'acting',
            key: 'acting',
            render: (_: any, item: UserDto) => {
                const items: MenuProps['items'] = [];
                items.push({
                    key: '1',
                    label: '编辑',
                    onClick: () => {
                        console.log('编辑');
                    }
                })
                if (item.role === RoleType._1) {
                    items.push({
                        key: '4',
                        label: '取消管理员',
                        onClick: async () => {
                            userService.changeRole(new ChangeRoleInputDto({
                                id:item.id,
                                role: RoleType._0
                            }));
                            message.success('取消成功');
                            await ResetLoading();
                        }
                    })
                } else {

                    if (!item.isDisable) {
                        items.push({
                            key: '3',
                            label: '禁用',
                            onClick: async () => {
                                await userService.disable(new DisableInputDto({
                                    id: item.id,
                                    disable: true
                                }));
                                message.success('禁用成功');
                                await ResetLoading();
                            }
                        })
                    } else {
                        items.push({
                            key: '3',
                            label: '启用',
                            onClick: async () => {
                                await userService.disable(new DisableInputDto({
                                    id: item.id,
                                    disable: false
                                }));
                                message.success('启用成功');
                                await ResetLoading();
                            }
                        })
                    }

                    items.push({
                        key: '4',
                        label: '设为管理员',
                        onClick: async () => {
                            await userService.changeRole(new ChangeRoleInputDto({
                                id: item.id,
                                role: RoleType._1
                            }));
                            message.success('设置成功');
                            await ResetLoading();
                        }
                    })
                }
                return (
                    <>
                        <Dropdown menu={{ items }} trigger={['click']}>
                            <Button>操作</Button>
                        </Dropdown>
                    </>
                )
            }
        },
    ];


    return (
        <Table  pagination={{
            current: input.page,
            pageSize: input.pageSize,
            total: total,
            onChange: handleTableChange,
        }} scroll={{ y: 'calc(100vh - 240px)' }}
         dataSource={data} columns={columns} />
    )
}