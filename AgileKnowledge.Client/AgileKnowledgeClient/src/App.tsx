import { ThemeProvider } from '@lobehub/ui'
import './App.css'
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import Home from './pages/home/page';
import Login from './pages/login/page';
import MainLayout from './layouts/main-layout';
import { Suspense } from 'react';
import App from './pages/app/page';
import User from './pages/user/page';


const router = createBrowserRouter([{
  path: '/',
  element: <Home />
},
{
  element: <MainLayout />,
  children: [
    {
      path: '/app', element: <Suspense fallback={'加载中'}>
        <App />
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




function AppPage() {
  return (
    <ThemeProvider defaultThemeMode='dark'>
      <RouterProvider router={router} />
    </ThemeProvider>
  )
}

export default AppPage
