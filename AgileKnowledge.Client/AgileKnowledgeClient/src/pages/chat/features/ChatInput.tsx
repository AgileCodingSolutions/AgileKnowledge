import { useState } from "react";
import { generateRandomString } from "../../../utils/stringHelper";
//import { fetchRaw } from "../../../App";
import { ActionIcon, ChatInputActionBar, ChatInputArea, ChatSendButton } from "@lobehub/ui";
import { Flexbox } from 'react-layout-kit';
import { Eraser, Languages } from 'lucide-react';
import React from "react";
import { message } from "antd";
import { KnowledgeService } from "../../../services/service-proxies";
import { ChatApplicationService ,CreateChatDialogHistoryInputDto} from '../../../services/service-proxies';

interface IFastChatInputProps {
    dialog: any;
    application: any;
    id?: string;
    history: any[];
    setHistory: any;
}

export default function FastChatInput({
    dialog,
    application,
    id,
    history,
    setHistory
}: IFastChatInputProps) {
    var chatApplicationService = new ChatApplicationService();
    var knowledgeService = new KnowledgeService();
    const [value, setValue] = useState<string>();
    const [loading, setLoading] = useState(false);
    const ref = React.useRef(null);

    async function sendChat() {
        try {

            // ref获取value
            const data = (ref as any).current?.resizableTextArea.textArea.value ?? value;
            if ((ref as any).current?.resizableTextArea.textArea.value) {
                (ref as any).current.resizableTextArea.textArea.value = '';
            }
            setValue('');

            if (!data || data === '') {
                return;
            }
            if (loading) {
                return;
            }

            if (dialog.id === undefined) {
                message.error('请先选择对话框');
                return;
            }

            setLoading(true);
            const chatlayout = document.getElementById('chat-layout');

            let userChat = {
                content: data,
                createat: new Date().toISOString(),
                extra: {},
                id: generateRandomString(10),
                meta: {
                    avatar: "https://blog-simple.oss-cn-shenzhen.aliyuncs.com/Avatar.jpg",
                    title: "我",
                },
                role: 'user',
            };

            async function SearchVectorQuantity() {
                try {
                    if (value === '') return;
                    const result = await knowledgeService.getSearchVectorQuantity(id, value, 0.4);
                    if(result.init!)
                    setValue(result)
                   
                    
                } catch (error) {
        
                }
            }
            





            // await chatApplicationService.createChatDialogHistory(
            //     {
            //         chatDialogId: dialog.id,
            //         //content :,
            //         type : dialog.type
            //     } as CreateChatDialogHistoryInputDto
//);
             // 发送消息到后端
           //await chatApplicationService.createChatDialogHistory(input);


            history.push(userChat)

            setHistory([...history]);

            let chat = {
                content: '',
                createat: new Date().toISOString(),
                extra: {} as any,
                id: generateRandomString(10),
                meta: {
                    avatar: "https://blog-simple.oss-cn-shenzhen.aliyuncs.com/chatgpt.png",
                    title: "AI助手",
                },
                role: 'assistant',
            };

            setHistory([...history, chat]);

            // 滚动到底部
            if (chatlayout) {
                chatlayout.scrollTop = chatlayout.scrollHeight;
            }

            let requestInput = {
                messages: [

                ],
                max_tokens: 2000,
                temperature: 0.5,
                top_p: 1,
                stream: true,
            } as any;

            // 携带上文消息 用于生成对话
            history.slice(-4).forEach(x => {
                requestInput.messages.push({
                    content: x.content,
                    role: x.role
                });
            });

            let url;

            if (id) {
                url = '/v1/chat/completions?chatId=' + application.id + "&chatDialogId=" + dialog.id + "&chatShareId=" + id;
            } else {
                url = '/v1/chat/completions?chatId=' + application.id + "&chatDialogId=" + dialog.id;
            }
            //console.log(application.id, 11111111,dialog.id)
            let stream = await fetchRaw(url, requestInput);

            for await (const chunk of stream) {
                // 分割chunk，可能包含多条信息，每条信息以 "data: " 开始
                const messages = chunk!.split('\n');

                for (const message of messages) {
                    // 忽略空行
                    if (!message) continue;

                    // 假设每条信息都以 "data: " 开始，移除这个前缀并解析JSON
                    try {
                        const jsonString = message.replace('data: ', '');

                        // 如果是结束标志，停止接收数据
                        if (jsonString === "[DONE]") {
                            setLoading(false);
                            break;
                        }
                        const jsonData = JSON.parse(jsonString);

                        // 提取所需要的内容
                        const content = jsonData.choices[0].delta.content;

                        if(userChat)


                        // 更新用户对话id
                        if(userChat.id !== jsonData.system_fingerprint) {
                            userChat.id = jsonData.system_fingerprint;
                            setHistory([...history, userChat]);
                        }

                        chat.id = jsonData.id;

                        chat.content += content;

                        setHistory([...history, chat]);
                        // 滚动到底部
                        if (chatlayout) {
                            chatlayout.scrollTop = chatlayout.scrollHeight;
                        }
                    } catch (error) {
                        console.error('Error parsing stream data:', error);
                    }
                }
            }


            // 通过对话id获取源数据
        //     const hisotryInfo = await chatApplicationService.getChatDialogHistory(chat.id,'','',input.page,input.pageSize);
        //     console.log(chat.id , 566655225)
        //     chat.extra.referenceFile = hisotryInfo.referenceFile;

        //     setHistory([...history, chat]);
        // } catch (error) {
        //     console.error(error);
        } finally {
            setLoading(false);
        }
    }

    return (
        <Flexbox style={{ flex: 1, position: 'relative', width: '100%', height: "100%" }}>
            <ChatInputArea
                value={value}
                onChange={(e: any) => {
                    setValue(e.target.value);
                }}
                placeholder="请输入您的消息"
                onKeyUpCapture={(e: any) => {
                    if (e.key === 'Enter' && !e.shiftKey && value !== '') {
                        sendChat();
                    }
                }}
                style={{
                    height: '100%',
                }}
                bottomAddons={<ChatSendButton loading={loading} onSend={() => sendChat()} 
                
                />}
                
                topAddons={
                    <ChatInputActionBar
                        leftAddons={
                            <>
                                <ActionIcon icon={Languages} color={undefined} fill={undefined} fillOpacity={undefined} fillRule={undefined} focusable={undefined} />
                                <ActionIcon onClick={() => {
                                    chatApplicationService.deleteChatDialogHistory(dialog.id)

                                        .then(()=>{
                                            setHistory([]);
                                            message.success('清空成功');
                                        });
                                }} icon={Eraser} color={undefined} fill={undefined} fillOpacity={undefined} fillRule={undefined} focusable={undefined} />
                            </>
                        }
                    />
                }
            />
        </Flexbox>
    )
}