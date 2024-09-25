import { ThemeProvider } from '@lobehub/ui'
import './App.css'
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import Home from './pages/home/page';
import Login from './pages/login/page';
import MainLayout from './layouts/main-layout';
import { Suspense } from 'react';
import App from './pages/app/page';
import User from './pages/user/page';
import AppDetail from './pages/app-detail/page';
import Knowledge from './pages/knowledge/page';
import { config } from './config';
import { message } from 'antd';
import Chat from './pages/chat/(desktop)/index';
import KnowledgeDetail from './pages/knowledge/features/KnowledgeDetail'
const router = createBrowserRouter([{
  path: '/',
  element: <Home />
},
{
  element: <MainLayout />,
  children: [
    {
      path: '/chat', element: <Suspense fallback={'加载中'}>
      <Chat/>
      </Suspense>
    },
    {
      path: '/app', element: <Suspense fallback={'加载中'}>
        <App />
      </Suspense>
    },
    {
      path: '/app/:id', element: <Suspense fallback={'加载中'}>
        <AppDetail />
      </Suspense>
    },
    {
      path: '/knowledge', element: <Suspense fallback={'加载中'}>
        <Knowledge />
      </Suspense>
    },
    {
      path: '/KnowledgeDetail/:id', element: <Suspense fallback={'加载中'}>
        <KnowledgeDetail />
      </Suspense>
    },
    {
      path: '/user', element: <Suspense fallback={'加载中'}>
        <User />
      </Suspense>
    },
  ]
},
{
  path: '/login',
  element: <Login />
}]);


// 保存原始的 fetch 函数
const originalFetch = window.fetch;

// 重写 window.fetch
window.fetch = async (url: any, options: any) => {
  const token = localStorage.getItem('token');
  const headers = new Headers(options.headers || {});

  // 添加或更新 Authorization 头
  if (token) {
    headers.append('Authorization', `Bearer ${token}`);
  }

  // 创建新的 options 对象，避免修改原始 options 对象
  const newOptions = { ...options, headers: headers };

  try {
    const baseUrl = config.FAST_API_URL;
    url = `${baseUrl}${url}`.replace(/([^:]\/)\/+/g, '$1');
    const response = await originalFetch(url, newOptions);

    if (response.status >= 200 && response.status < 300) {
      return response;
    }
    if (response.status === 401) {
      window.location.href = '/login';
    }
    if (response.status === 400) {
      const data = await response.json();
      message.error(data.message);
      throw new Error(data);
    } else if (response.status === 404) {
      message.error('请求的资源不存在');
      const data = await response.json();
      message.error(data.message);
      throw new Error(data);
    } else if (response.status === 500) {
      message.error('服务器错误');
      const data = await response.json();
      message.error(data.message);
      throw new Error(data);
    }
    const error = new Error();
    throw error;
  } catch (error) {
    console.error('Fetch error:', error);
    throw error;
  }
};


function AppPage() {
  return (
    <ThemeProvider defaultThemeMode='dark'>
      <RouterProvider router={router} />
    </ThemeProvider>
  )
}

export default AppPage
