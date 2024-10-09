import { Table, Button, Dropdown, MenuProps, message, Select } from 'antd';
import { useEffect, useState } from 'react';
import {  KnowledgeService } from '../../../services/service-proxies';

import { KnowledgeBaseQuantizationState } from '../../../services/service-proxies';
import DetailFile from '../features/DetailFile'
interface IWikiDataProps {
    id: string;
    onChagePath(key: string): void;
}

export default function WikiData({ id, onChagePath }: IWikiDataProps) {

var knowledgeService = new KnowledgeService();
    const columns = [
        {
            title: '文件名',
            dataIndex: 'name',
            key: 'name',
            render: (text: string, item: any) => {
                return item.isedit ? <input
                    autoFocus
                    style={{
                        width: '100%',
                        border: 'none',
                        outline: 'none',
                        background: 'transparent',
                        fontSize: 14
                    }}
                    onBlur={async (el) => {
                        item.isedit = false;
                        if (el.target.value !== text) {
                            item.fileName = el.target.value;
                        }
                        setData([...data]);

                        try {
                            //await DetailsRenameName(item.id, el.target.value);
                            message.success('修改成功');
                        } catch (error) {
                            message.error('修改失败');
                        }
                    }}
                    defaultValue={text}
                ></input> : <span onDoubleClick={() => handleDoubleClick(item)}>{text}</span>;
            },
        },
        {
            title: '索引数量',
            dataIndex: 'dataCount',
            key: 'dataCount',
        },
        {
            title: '数据类型',
            dataIndex: 'type',
            key: 'dataType',
        },
        {
            title: '数据状态',
            key: 'stateName',
            dataIndex: 'stateName',
        },
        {
            title: '创建时间',
            key: 'creationTime',
            dataIndex: 'creationTime',
            render: (text: string) => {
                if (!text) return '-';
                
                const date = new Date(text);
                return date.toLocaleString(); 
              },
        },
        {
            title: '操作',
            key: 'action',
            render: (_: any, item: any) => {
                const items = [
                    {
                        key: '1',
                        onClick: () => {
                            setOpenItem(item);
                            setVisible(true);
                        },
                        label: (
                            <span>
                                详情
                            </span>
                        ),
                    },
                    {
                        key: '2',
                        onClick: () => {
                            RemoveDeleteWikiDetails(item.id);
                        },
                        label: (
                            <span>
                                删除
                            </span>
                        ),
                    },]
                // 如果失败了则显示量化。
                if (item.state !== 0) {
                    items.push({
                        key: '3',
                        onClick: () => {
                            onRetryVectorDetail(item);
                        },
                        label: (
                            <span>
                                重试
                            </span>
                        ),
                    })
                }
                return (
                    <Dropdown
                        menu={{
                            items: items
                        }}
                        placement="bottomLeft"
                    >
                        <Button>
                            操作
                        </Button>
                    </Dropdown>
                )
            },
        },
    ]
    const [data, setData] = useState([] as any);
    const [visible, setVisible] = useState(false);
    const [openItem, setOpenItem] = useState({} as any);
    const [total, setTotal] = useState(0);
    const [input, setInput] = useState({
        keyword: '',
        page: 1,
        pageSize: 5,
        filter : '',
        state: undefined as KnowledgeBaseQuantizationState | undefined
    });

    const items: MenuProps['items'] = [
        {
            key: '1',
            onClick: () => {
                onChagePath('upload')
            },
            label: (
                <span>
                    上传文件
                </span>
            ),
        },
        {
            key: '2',
            onClick: () => {
                onChagePath('upload-web')
            },
            label: (
                <span>
                    网页链接
                </span>
            ),
        },
        {
            key: '3',
            label: (
                <span>
                    自定义文本
                </span>
            ),
        },
    ];

    function handleDoubleClick(item: any) {
        item.isedit = true;
        setData([...data]);
    }
    async function RemoveDeleteWikiDetails(id: string) {
        try {
            await  knowledgeService.deleteDetails(id);
            message.success('删除成功');
            setInput({
                ...input,
                page: 1
            })
        } catch (error) {
            message.error('删除失败');
        }
    }

    async function onRetryVectorDetail(item: any) {
        try {
            
            await knowledgeService.retryVectorDetail(item.id);
            message.success('成功');
            loadingData()
        } catch (error) {

            message.error('失败');
        }

    }


    function handleTableChange(page: number, pageSize: number) {
        setInput({
            ...input,
            page: page,
            pageSize: pageSize,
        });
    }

    async function loadingData() {
        try {
            const result = await knowledgeService.getDetailsList(id, input.state, input.filter ,input.keyword, input.page, input.pageSize);
            setData(result.items!);
            setTotal(result.totalCount!);
        } catch (error) {
                 
        }
    }

    useEffect(() => {
        loadingData();
    }, [id, input]);

    return (<>
        <header style={{
            padding: 16,
            fontSize: 20,
            fontWeight: 600
        }}>
            文件列表
            <div style={{
                float: 'right'
            }}>
                <Dropdown menu={{ items }} placement="bottomLeft">
                    <Button >上传文件</Button>
                </Dropdown>
            </div>
            <Select
                    value={input.state !== undefined ? input.state.toString() : ''}
                    style={{
                        width: 120,
                        marginLeft: 16,
                        marginRight: 16,
                        float: 'right'
                    }}
                    onChange={(v) => {
                        setInput({
                            ...input,
                            state: v === '' ? undefined : parseInt(v) as KnowledgeBaseQuantizationState,
                            page: 1,
                        });
                        loadingData();
                    }}
                    options={[
                        { value: "", label: '全部' },
                        { value: KnowledgeBaseQuantizationState._0.toString(), label: '处理中' },
                        { value: KnowledgeBaseQuantizationState._1.toString(), label: '完成' },
                        { value: KnowledgeBaseQuantizationState._2.toString(), label: '失败' },
                    ]}
            />
        </header>
        <Table
        dataSource={data}
        pagination={{
          current: input.page,
          pageSize: input.pageSize,
          total: total,
          onChange: handleTableChange,
        }}
        scroll={{ y: 'calc(100vh - 240px)' }}
        columns={columns}
        rowKey="id"  // 指定每行数据的唯一键字段
        style={{ overflow: 'auto', padding: 16, borderRadius: 8 }}
      />
            
        <DetailFile onClose={() => {
            setVisible(false);
        }} wikiDetail={openItem} visible={visible} />
    </>)
}