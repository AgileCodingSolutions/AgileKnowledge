import { UploadFile } from "../../../services/StorageService";
import { Button, Steps, Upload, UploadProps, Table, Progress, Radio, message, MenuProps, Dropdown } from 'antd';
import { useState } from 'react';
import { InboxOutlined, CloseOutlined } from '@ant-design/icons';
import styled from 'styled-components';
import { bytesToSize } from '../../../utils/stringHelper';
import { CreateKnowledgeDetailsInput, TrainingPatternType } from '../../../services/service-proxies';
import { KnowledgeService } from '../../../services/service-proxies';
import { TextArea } from '@lobehub/ui';

const FileItem = styled.div`
    transition: border-color 0.3s linear;
    border: 1px solid #d9d9d9;
    border-radius: 8px;
    padding: 10px;
    margin-right: 10px;
    display: flex;
    cursor: pointer;
    margin-bottom: 10px;
    &:hover {
        border-color: #1890ff;
        transition: border-color 0.3s linear;
        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
    }
`;

const { Dragger } = Upload;

interface IUploadWikiFileProps {
    id: string;
    onChagePath(key: any): void;
}

export default function UploadWikiFile({ id, onChagePath }: IUploadWikiFileProps) {
    const [current, setCurrent] = useState(0);
    const [fileList, setFileList] = useState<any[]>([]);
    const [trainingPattern, setTrainingPattern] = useState(TrainingPatternType._1);
    const [maxTokensPerParagraph, setMaxTokensPerParagraph] = useState(1000);
    const [maxTokensPerLine, setMaxTokensPerLine] = useState(300);
    const [overlappingTokens, setOverlappingTokens] = useState(100);
    const [qAPromptTemplate, setQAPromptTemplate] = useState(`
    我会给你一段文本，学习它们，并整理学习成果，要求为：
    1. 提出最多 20 个问题。
    2. 给出每个问题的答案。
    3. 答案要详细完整，答案可以包含普通文字、链接、代码、表格、公示、媒体链接等 markdown 元素。
    4. 按格式返回多个问题和答案:

    Q1: 问题。
    A1: 答案。
    Q2:
    A2:
    ……

    我的文本："""{{$input}}"""`); // QA问答模板

    var knowledgeService = new KnowledgeService();
    const props: UploadProps = {
        name: 'file',
        multiple: true,
        showUploadList: false,
        accept: '.md,.pdf,.docs,.txt,.json,.excel,.word,.html',
        beforeUpload: (file: any) => {
            fileList.push(file);
            setFileList([...fileList]);
            return false;
        }
    };
    const columns = [
        {
            title: '文件名',
            dataIndex: 'fileName',
            key: 'fileName',
        },
        {
            title: '文件上传进度',
            dataIndex: 'progress',
            key: 'progress',
            render: (value: number) => {
                return <Progress percent={value} size="small" />
            }
        },
        {
            title: '数据上传进度',
            dataIndex: 'dataProgress',
            key: 'dataProgress',
            render: (value: number) => {
                return <Progress percent={value} size="small" />
            }
        },
        {
            title: '操作',
            dataIndex: 'handler',
            key: 'handler',
            render: (_: any, item: any) => {
                const items: MenuProps['items'] = [];
                items.push({
                    key: '1',
                    label: '删除',
                    onClick: () => {
                        setFileList(fileList.filter((i) => i !== item));
                    }
                })
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

    function saveFile() {
        
        try {
            fileList.forEach(async (file) => {
                await Upload(file)
            });
            message.success('上传成功');
          } catch (error) {
            message.error('上传失败');
          }
    }
    
    async function Upload(file: any) {

        const fileItem = await UploadFile(file);
        file.progress = 100;

        setFileList([...fileList]);

        await knowledgeService.createDetails(new CreateKnowledgeDetailsInput({
            fileId: fileItem.id,
            filePath: fileItem.path,
            knowledgeId: id,
            maxTokensPerParagraph: maxTokensPerParagraph,
            maxTokensPerLine: maxTokensPerLine,
            overlappingTokens: overlappingTokens,
            qaPromptTemplate: qAPromptTemplate,
            trainingPattern: trainingPattern,
        }))

        file.dataProgress = 100;

        setFileList([...fileList]);
    }

    return (<>
        <div >
            <Button onClick={() => {
                onChagePath(1)
            }}>返回</Button>
        </div>

        <Steps
            style={{
                marginTop: '20px',
                marginBottom: '20px',
            }}
            size="small"
            current={current}
            items={[
                {
                    title: '上传文件'
                },
                {
                    title: '数据处理'
                },
            ]}
        />

        {
            current === 0 && <>
                <Dragger {...props} style={{
                    padding: '20px',
                    border: '1px dashed #d9d9d9',
                    borderRadius: '2px',
                    justifyContent: 'center',
                    alignItems: 'center',
                    flexDirection: 'column'
                }} height={200}>
                    <p className="ant-upload-drag-icon">
                        <InboxOutlined />
                    </p>
                    <p className="ant-upload-text">点击或推动文件上传</p>
                    <p className="ant-upload-hint">
                        支持单个或批量上传，支持 .md .pdf .docs .txt .json .excel .word .html等格式,
                        最多支持1000个文件。单文件最大支持100M。
                    </p>
                </Dragger>
                <div style={{
                    padding: '20px',
                    display: 'flex',
                    flexWrap: 'wrap',
                    overflow: 'auto',
                    height: '200px',
                    alignContent: 'flex-start'
                }}>
                    {fileList.length > 0 && fileList.map((item, index) => {
                        return <FileItem key={item.id || `${item.name}-${index}`}>
                            <span>{item.name}</span>
                            <span style={{
                                marginLeft: 10
                            }}>
                                {bytesToSize(item.size || 0)}
                            </span>
                            <span style={{
                                marginLeft: 10
                            }}>
                                <CloseOutlined
                                    onClick={() => {
                                        setFileList(fileList.filter((_, i) => i !== index));
                                    }} />
                            </span>
                        </FileItem>
                    })}
                </div>
                <Button onClick={() => {
                    setCurrent(1);
                }} style={{
                    float: 'right',
                    marginTop: 20,
                }}>下一步</Button>
            </>
        }
        {
            current === 1 && <>
                <div style={{
                    display: 'flex',
                    flexDirection: 'column',
                    height: '55%'
                }}>
                    <div style={{
                        flex: '1',
                        marginBottom: 20
                    }}>
                        <Radio.Group style={{
                            marginBottom: 20
                        }} onChange={(v: any) => {
                            const value = Number(v.target.value);
                            setTrainingPattern(value as TrainingPatternType);
                        }} value={trainingPattern}>
                            <Radio style={{
                                border: '1px solid #d9d9d9',
                                borderRadius: 8,
                                padding: 10,
                                marginRight: 10
                            }} value={TrainingPatternType._0}>文本拆分</Radio>
                            <Radio style={{
                                border: '1px solid #d9d9d9',
                                borderRadius: 8,
                                padding: 10,
                                marginRight: 10
                            }} value={TrainingPatternType._1}>QA问答拆分</Radio>
                        </Radio.Group>
                        
                        {
                            trainingPattern === TrainingPatternType._1 && <>
                                <span>QA问答模板：</span>
                                <TextArea rows={4} value={qAPromptTemplate} onChange={(v) => {
                                    setQAPromptTemplate(v.target.value);
                                }} style={{
                                    minHeight: '50px',
                                    flex: '1'
                                }} />
                            </>
                        }
                    </div>
                    <div style={{
                        flex: '1'
                    }}>
                        <Table
                            rowKey="key"
                            dataSource={fileList.map((item, index) => ({
                                fileName: item.name,
                                progress: item.progress || 0,
                                dataProgress: item.dataProgress || 0,
                                key: `${item.id || item.name}-${index}`
                            }))}
                            columns={columns}
                        />
                        <Button type='primary' onClick={() => {
                        saveFile();}} style={{
                            float: 'right',
                            marginTop: 20,
                            marginLeft: 20,
                        }}>提交数据（{fileList.length}）</Button>
                        <Button onClick={() => {
                            setCurrent(0);
                        }} style={{
                            float: 'right',
                            marginTop: 20,
                        }}>上一步</Button>
                        </div>
                        </div>
                        
                    </>
                }
            </>)
            }
            
            