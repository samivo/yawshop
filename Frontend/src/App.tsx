import { BrowserRouter, Route, Routes } from 'react-router-dom'
//import Dashboard from './Pages/Management/Dashboard';
import ProductTemplate from './Pages/Public/Product/ProductTemplate';
import LoginPage from './Pages/Management/Login/LoginPage';
import ProtectedRoute from './Pages/Management/ProtectedRouteWrapper';
import { CheckoutPage } from './Pages/Public/Checkout/CheckoutPage';
import { lazy } from 'react';
import { CheckoutComplete } from './Pages/Public/Checkout/CheckoutComplete';
import { GiftcardInfo } from './Pages/Public/Giftcard/GiftcardInfo';
import OrderList from './Pages/Management/Orders/OrderList';

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
          <Route path="/giftcard/:code" element={<GiftcardInfo />} />
          <Route path="/lahjakortti/:code" element={<GiftcardInfo />} />
          <Route path='/checkout/cancel' element={<CheckoutComplete success={false} redirectUrl='https://klu.fi'/>} />
          <Route path='/checkout/success' element={<CheckoutComplete success={true} redirectUrl='https://klu.fi'/>} />
          <Route path="/orders" element={
            <ProtectedRoute>
              <OrderList />
            </ProtectedRoute>
          } />
      </Routes>
    </BrowserRouter>
  )
}

export default App
