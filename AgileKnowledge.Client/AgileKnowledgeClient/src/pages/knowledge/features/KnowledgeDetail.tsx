import { memo, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { Button , List} from 'antd';
import { Avatar, Tag } from '@lobehub/ui';
import styled from 'styled-components';
//import KnowledgeDetail from '../feautres/KnowledgeDetail';
import { KnowledgeService } from '../../../services/service-proxies';
import WikiData from '../features/CheckQuantizationState'
import UploadWikiFile from '../features/UploadWikiFile'
import SearchDetail from '../features/SearchDetail'
const LeftTabs = styled.div`
    width: 190px;
    min-width: 190px;
    height: 100%;
    border-right: 1px solid #464545;
    display: flex;
    flex-direction: column;
`;


export default memo(() => {
    const { id } = useParams<{ id: string }>();
    if (id === undefined) return (<div>id is undefined</div>)

    const [application, setApplication] = useState({} as any);

    const [tabs, setTabs] = useState([] as any[]);

    const [tab, setTab] = useState() as any;
    const [quantizationState] = useState([]) as any;

    var knowledgeService = new KnowledgeService();

    useEffect(() => {
        loadingApplication();
    }, [id]);

    async function loadingApplication() {
        knowledgeService.get(id as string)
            .then((application) => {
                setApplication(application);
            });
    }

    useEffect(() => {
        loadingTabs();
    }, [application]);

    function loadingTabs() {
        const tabs = [{
            key: 1,
            label: '数据集'
        }, {
            key: 2,
            label: '搜索测试'
        }, {
            key: 3,
            label: '配置'
        }];

        changeTab(tabs[0]);

        //强制刷新
        setTabs([...tabs]);
    }

    function changeTab(key: any) {
        // 如果key是数字
        if (typeof key === 'number') {
            key = tabs.find(item => item.key === key);
        }

        setTab(key);
    }

    return (
<>
            <LeftTabs>
                <div style={{
                    borderBottom: "1px solid #464545",
                    marginBottom: 16,
                    marginTop: 16,
                }}>
                    <Avatar size={50}
                        style={{
                            margin: '0 auto',
                            display: 'block',
                            marginBottom: 16
                        }}
                        src={application.icon} />
                    <div style={{
                        fontSize: 18,
                        overflow: 'hidden',
                        textAlign: 'center',
                        fontWeight: 600,
                        marginBottom: 16
                    }}>{application.name}</div>
                </div>
                <div style={{
                    padding: 8,
                    display: 'flex',
                    flexDirection: 'column',
                }}>
                    {tabs.map((item, index) => {
                        return <Button key={index} onClick={() => {
                            changeTab(item);
                        }} type={tab?.key === item.key ? 'default' : 'text'} style={{
                            marginBottom: 16,
                            width: '100%',

                        }} size='large'>{item.label}</Button>
                    })}
                </div>
                <div style={{
                    position: 'absolute',
                    bottom: 0,
                    padding: 8,
                    height: '20%',
                    overflow: 'auto',
                    width: 190,
                    justifyContent: 'space-around',
                }}>
                    <List
                        itemLayout='vertical'
                        locale={{ emptyText: '暂无量化任务' }}
                        dataSource={quantizationState}
                        renderItem={(item: any, index: number) => (
                            <List.Item style={{
                                height: 45,
                            }}>
                                <List.Item.Meta
                                    title={
                                        <>
                                            <span>{index}：</span>
                                            <span>{item.fileName}</span>
                                            <Tag style={{float: 'right'
                                            }}>量化中</Tag>
                                        </>
                                    }
                                />
                            </List.Item>
                        )}
                    />
                </div>
            </LeftTabs >
            <div style={{
                width: '100%',
                padding: 20,
                overflow: 'auto',

            }}>
                 {
                    tab?.key === 1 && <WikiData onChagePath={key => changeTab(key)} id={id} />
                }
                 {
                     tab?.key === 2 && <SearchDetail onChagePath={key => changeTab(key)} id={id} />
                 }
                 {/* {
                     tab?.key === 3 && <WikiInfo id={id} />
                 } */}
                 {
                     tab === 'upload' && <UploadWikiFile id={id} onChagePath={key => changeTab(key)} />
                 }
                 {/* {
                     tab === 'upload-web' && <UploadWikiWeb id={id} onChagePath={key => changeTab(key)} />
                 }  */}
            </div>
        </>
    );
})