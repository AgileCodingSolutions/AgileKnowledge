import { memo, useState } from "react";
import { AppList } from "../features/KnowledgeList";
import { Layout } from "@lobehub/ui";
import Header from "../features/Header";

const DesktopLayout = memo(() => {
    const [input, setInput] = useState({
        page: 1,
        pageSize: 12
    });
    //return 111;
    return (
        <div style={{ height: '100vh', overflow: 'auto',width:'100%' }}>
            <Layout
                footer={[]}
                header={
                    <Header onSucess={()=>{
                        setInput({
                            ...input,
                            page: 1
                        });
                    }}/>
                }
            >
                <AppList setInput={(v)=>{
                    setInput(v);
                }} input={input}/>
            </Layout>
        </div>)
});

export default DesktopLayout;