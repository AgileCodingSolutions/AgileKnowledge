import { memo, useEffect, useState } from "react";
import { AutoComplete, Row, Select, Checkbox, Button, Collapse, Col, Slider, message } from 'antd';
import styled from 'styled-components';
import { getModels } from "../../../store/Model";
import { IUpdateChatApplicationInputDto, ChatApplicationService, IChatApplicationDto, KnowledgeService, UpdateChatApplicationInputDto } from "../../../services/service-proxies";

interface IAppDetailInfoProps {
    value: IChatApplicationDto
}

const Container = styled.div`
    display: grid;
    padding: 20px; 
    /* 屏幕居中显示 */
    margin: auto;
    width: 580px;
    overflow: auto;
    height: 100%;

    // 隐藏滚动条
    &::-webkit-scrollbar {
        display: none;
    }
    scrollbar-width: none;

`;

const ListItem = styled.div`
    display: flex;
    justify-content: space-between;
    padding: 20px;
    width: 100%;
`;

const AppDetailInfo = memo(({ value }: IAppDetailInfoProps) => {
    if (value === undefined) return null;

    const [selectChatModel, setSelectChatModel] = useState([] as any[]);
    const [KnowledgeBases, setKnowledgeBases] = useState([] as any[]);
    const [input,] = useState({
        keyword: '',
        page: 1,
        pageSize: 10
    } as any);

    var chatApplicationService = new ChatApplicationService();
    var knowledgeService = new KnowledgeService();

    useEffect(() => {
        getModels()
            .then((models) => {
                setSelectChatModel(models.chatModel.map((item) => {
                    return { label: item.label, value: item.value }
                }));
            });
        loadingWiki();
    }, []);

    function loadingWiki() {
        knowledgeService.getList(input.keyword, "", input.page, 100)
            .then((knowledge) => {
                setKnowledgeBases(knowledge.items!);
            });
    }

    const [application, setApplication] = useState(value);

    useEffect(() => {
        setApplication(value);
    }, [value]);

    function save() {
        const updateData = new UpdateChatApplicationInputDto({
            id: application.id,
            name: application.name,
            prompt: application.prompt,
            chatModel: application.chatModel,
            temperature: application.temperature,
            maxResponseToken: application.maxResponseToken,
            template: application.template,
            opener: application.opener,
            knowledgeIds: application.knowledgeIds
        });

        chatApplicationService.update(updateData).then(() => {
            message.success('保存成功');
        });
    }

    return (
        <Container>
            <ListItem>
                <span style={{
                    fontSize: 20,
                    marginRight: 20
                }}>对话模型</span>
                <AutoComplete
                    defaultValue={application.chatModel}
                    value={application.chatModel}
                    style={{ width: 380 }}
                    onChange={(v: string) => {
                        setApplication({
                            ...application,
                            chatModel: v
                        });
                    }}
                    options={selectChatModel}
                />
            </ListItem>

            <ListItem>
                <span style={{
                    fontSize: 20,
                    marginRight: 20
                }}>开场白</span>
                <textarea value={application?.opener ?? ""}
                    onChange={(e: any) => {
                        setApplication({
                            ...application,
                            opener: e.target.value
                        });
                    }}
                    style={{ width: 380, resize: "none", height: '200px' }}>

                </textarea>
            </ListItem>
            <ListItem>
                <span style={{
                    fontSize: 20,
                    marginRight: 20
                }}>提示词</span>
                <textarea value={application.prompt}
                    defaultValue={application.prompt}
                    onChange={(e: any) => {
                        setApplication({
                            ...application,
                            prompt: e.target.value
                        });
                    }}
                    style={{ width: 380, resize: "none", height: '200px' }}>

                </textarea>
            </ListItem>
            {/* <ListItem>
                <span style={{
                    marginRight: 20
                }}>未找到回复模板</span>
                <textarea value={application.noReplyFoundTemplate ?? ''}
                    defaultValue={application.noReplyFoundTemplate ?? ''}
                    onChange={(e: any) => {
                        setApplication({
                            ...application,
                            noReplyFoundTemplate: e.target.value
                        });
                    }}
                    style={{ width: 380, resize: "none", height: '200px' }}>

                </textarea>
            </ListItem> */}
            <Select
                mode="multiple"
                allowClear
                style={{
                    width: '100%',
                    marginTop: 20,
                    marginBottom: 20
                }}
                placeholder="绑定知识库"
                defaultValue={application.knowledgeIds}
                value={application.knowledgeIds}
                onChange={(v: any) => {
                    setApplication({
                        ...application,
                        knowledgeIds: v
                    });
                }}
                options={KnowledgeBases.map((item) => {
                    return {
                        label: item.name,
                        value: item.id
                    }
                })}
            />
            <Collapse
                items={[{
                    key: '1', label: '高级设置', children: <>

                        <Row style={{
                            marginLeft: 20,
                            marginTop: 20,

                        }}>
                            <span style={{
                                marginRight: 20
                            }}>温度（AI智商）</span>
                            <Col span={12}>
                                <Slider
                                    min={0}
                                    max={1}
                                    step={0.1}
                                    defaultValue={application.temperature}
                                    onChange={(e: any) => {
                                        setApplication({
                                            ...application,
                                            temperature: e
                                        });
                                    }}
                                    value={application.temperature}
                                />
                            </Col>
                        </Row>
                        <Row style={{
                            marginLeft: 20,
                            marginTop: 20,

                        }}>
                            <span style={{
                                marginRight: 20
                            }}>响应token上限</span>
                            <Col span={12}>
                                <Slider
                                    max={128000}
                                    min={100}
                                    defaultValue={application.maxResponseToken}
                                    step={1}
                                    onChange={(e: any) => {
                                        setApplication({
                                            ...application,
                                            maxResponseToken: e
                                        });
                                    }}
                                    value={application.maxResponseToken}
                                />
                            </Col>
                        </Row>

                        <ListItem>
                            <span style={{
                                marginRight: 20
                            }}>引用模板提示词</span>
                            <textarea value={application.template}
                                defaultValue={application.template}
                                onChange={(e: any) => {
                                    setApplication({
                                        ...application,
                                        template: e.target.value
                                    });
                                }}
                                style={{ width: 380, resize: "none", height: '200px' }}>

                            </textarea>
                        </ListItem>
                    </>
                }
                ]}
            />
            <Button block onClick={save} style={{
                marginTop: 20
            }}>
                保存修改
            </Button>
        </Container>
    )
})

export default AppDetailInfo;