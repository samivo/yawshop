import { BrowserRouter, Route, Routes } from 'react-router-dom'
//import Dashboard from './Pages/Management/Dashboard';
import ProductTemplate from './Pages/Public/Product/ProductTemplate';
import LoginPage from './Pages/Management/Login/LoginPage';
import ProtectedRoute from './Pages/Management/ProtectedRouteWrapper';
import { CheckoutPage } from './Pages/Public/Checkout/CheckoutPage';
import { lazy } from 'react';

const DashboardLazy : any = lazy(() => import('./Pages/Management/Dashboard'));

function App() {

  return (
    <BrowserRouter>
      <Routes>
          <Route path="/dashboard" element={
            <ProtectedRoute>
              <DashboardLazy />
            </ProtectedRoute>
          } />
          <Route path="/product/:code" element={<ProductTemplate />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path='/checkout' element={<CheckoutPage />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
