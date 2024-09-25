import { Avatar, GridShowcase, LogoThree, SpotlightCard } from '@lobehub/ui';
import { useEffect, useState } from 'react';
import { Flexbox } from 'react-layout-kit';
import { message, Button, Pagination } from 'antd';
import { useNavigate } from 'react-router-dom';
import { DeleteOutlined } from '@ant-design/icons';
import { KnowledgeBasesDto, KnowledgeService } from '../../../services/service-proxies';
import { config } from '../../../config';


interface IAppListProps {
    input: {
        page: number;
        pageSize: number;
    }
    setInput: (input: any) => void;
}

export function AppList(props: IAppListProps) {
    const navigate = useNavigate();

    const [input, setInput] = useState({
        keyword: '',
        page: 1,
        pageSize: 12
    });

    const [data, setData] = useState<KnowledgeBasesDto[]>([]);
    const [total, setTotal] = useState(0);

    var knowledgeService = new KnowledgeService();

    const render = (item: KnowledgeBasesDto) => (
        <Flexbox align={'flex-start'} gap={8} horizontal style={{ padding: 16, height: 100 }}>

            <Avatar size={50} src={config.FAST_API_URL+'/'+item.icon} style={{ flex: 'none' }} />
            
            <Flexbox onClick={() => {
                openWikiDetail(item.id!);
            }}>
                <div style={{ fontSize: 15, fontWeight: 600 }}>{item.name}</div>
                <div style={{ opacity: 0.7,marginTop: 10}}>
                    QA模型：
                    {item.model}
                </div>
            </Flexbox>
            
            <Button
                style={{
                    float: 'inline-end',
                    position: 'absolute',
                    right: 16,
                }}
                icon={<DeleteOutlined />}
                onClick={() => deleteWiki(item.id!)}
            />
        </Flexbox>
    )
    
    async function deleteWiki(id: string) {
        await knowledgeService.delete(id);
        message.success('删除成功');
        setInput({
            ...input,
            page: 1
        });
        loadingData();
    }

    function openWikiDetail(id: string) {
        //message.success('111');
        navigate(`/KnowledgeDetail/${id}`);
    }

    async function loadingData() {
        try {
            const data = await knowledgeService.getList(input.keyword,"", input.page, input.pageSize);
            console.log(data);
            setData(data.items!);
            setTotal(data.totalCount!);
        } catch (error) {
            console.log(error);
            message.error('获取数据失败');
        }
    }

    useEffect(() => {
        loadingData();
    }, [props.input])

    return (<>
        <GridShowcase style={{ width: '100%' }}>
            <LogoThree size={180} style={{ marginTop: -64 }} />
            <div style={{ fontSize: 48, fontWeight: 600, marginTop: -16 }}>知识库列表</div>
        </GridShowcase>
        <SpotlightCard style={{
            margin: 16,
            borderRadius: 8,
            boxShadow: '0 0 8px 0 rgba(0,0,0,0.1)',
            overflow: 'auto',
            maxHeight: 'calc(100vh - 100px)',
            padding: 0,
        }} size={data.length} renderItem={render} items={data} >
        </SpotlightCard>
        <Pagination onChange={(page) => {
            props.setInput({
                ...props.input,
                page
            });
        }} total={total} />
    </>)
}