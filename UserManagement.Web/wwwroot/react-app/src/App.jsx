import UsersDisplayComponent from './components/UserDisplay/UsersDisplayComponent.jsx';
import ViewUserComponent from './components/ViewUser/ViewUserComponent.jsx';
import ViewLogsComponent from './components/LogsDisplay/ViewLogsComponent.jsx';
import { createBrowserRouter, createRoutesFromElements, Route, RouterProvider } from 'react-router-dom';
import RootLayout from './layouts/RootLayout.jsx'; 

export default function App() {
    const router = createBrowserRouter(
        createRoutesFromElements(
            <Route path='/' element={<RootLayout />}>
                <Route index element={<UsersDisplayComponent />} />
                <Route path='view' element={<ViewUserComponent />} />
                <Route path='logs' element={<ViewLogsComponent />} />
            </Route>
        )
    )

    return <RouterProvider router={router} />
}
