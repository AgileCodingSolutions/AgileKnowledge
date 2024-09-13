import { memo, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { Button, List } from 'antd';

import styled from 'styled-components';
import { KnowledgeService } from '../../../services/service-proxies';
//import { Avatar, Tag, } from '@lobehub/ui';

// import WikiData from '../features/WikiData';
// import UploadWikiFile from '../features/UploadWikiFile';
// import SearchWikiDetail from '../features/SearchWikiDetail';
// import { WikiDto } from '../../../models';
// import WikiInfo from '../features/WikiInfo';
// import UploadWikiWeb from '../features/UploadWikiWeb';
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

    //const [wiki, setWiki] = useState({} as WikiDto);
    const [application, setApplication] = useState({} as any);
    const [tabs, setTabs] = useState([] as any[]);

    const [quantizationState, setQuantizationState] = useState([]) as any;

    const [tab, setTab] = useState() as any;

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
            label: '应用配置'
        }];

        changeTab(tabs[0]);

        //强制刷新
        setTabs([...tabs]);
    }

    function changeTab(key: any) {
        setTab(key);
    }


    return (
        <>
            <div style={{ display: 'flex' }}>
                <LeftTabs>
                    {tabs.map((item, index) => {
                        return <Button key={index} onClick={() => {
                            changeTab(item);
                        }} type={tab?.key === item.key ? 'default' : 'text'} style={{
                            marginBottom: 16,
                            width: '100%'
                        }} size='large'>{item.label}</Button>
                    })}
                </LeftTabs>
            </div>
            <div style={{
                width: '100%',
                padding: 20

            }}>
                {/* {
                   <AppDetailInfo value={application} />
                } */}
            </div>
        </>
    );
})