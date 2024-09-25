import { Modal } from "@lobehub/ui";
import { Form, Input, Button, message , FormInstance} from 'antd';
import { ChatApplicationService, CreateChatDialogInputDto } from "../../../services/service-proxies";
import {useRef } from "react";
interface ICreateAppProps {
    visible: boolean;
    onClose: () => void;
    id: string;
    
}

type CreateAppType = {
    name?: string;
    description :string;
};

export function CreateDialog(props: ICreateAppProps) {

    const formRef = useRef<FormInstance>(null);
    var chatApplicationService = new ChatApplicationService();

    async function onFinish(values: any) {
        try {
            await chatApplicationService.createChatDialog(
                {
                    name: values.name,
                    description: values.description,
                    applicationId: props.id
                } as CreateChatDialogInputDto
);
            message.success('创建成功');
            if (formRef.current) {
                formRef.current.resetFields();
            }
            props.onClose();
        } catch (e) {
            message.error('创建失败');
        }
    }

    function onFinishFailed(errorInfo: any) {
        console.log('Failed:', errorInfo);
    }

    return (
        <Modal
            title="创建对话"
            open={props.visible}
            onCancel={props.onClose}
            width={400}
            footer={null}
        >
            <Form
                ref={formRef}
                name="basic"
                onFinish={onFinish}
                onFinishFailed={onFinishFailed}
                autoComplete="off"
            >
                <Form.Item<CreateAppType>
                    label="对话名称"
                    name="name"
                    rules={[{ required: true, message: '请输入您的对话名称' }]}
                >
                    <Input />
                </Form.Item>
                <Form.Item<CreateAppType>
                    label="对话描述"
                    name="description"
                    rules={[{ required: true, message: '请输入您的对话描述' }]}
                >
                    <Input />
                </Form.Item>

                <Form.Item>
                    <Button block type="primary" htmlType="submit">
                        OK
                    </Button>
                </Form.Item>
            </Form>
        </Modal>
    )
}