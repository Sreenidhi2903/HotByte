import React from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import NavBar from "./components/NavBar";
import Login from "./pages/Login";
import BranchSelector from "./pages/BranchSelector";
import Menu from "./pages/Menu";
import Cart from "./pages/Cart";
import Checkout from "./pages/Checkout";
import Orders from "./pages/Orders";
import Review from "./pages/Reviews";
import OrderConfirmation from "./pages/OrderConfirmation";
import { CartProvider } from "./context/CartContext";
import "bootstrap/dist/css/bootstrap.min.css";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import Branches from "./pages/Branches";
import RequireAuth from "./components/RequireAuth";
import Unauthorized from "./pages/Unauthorized";
import RestaurantDashboard from "./pages/RestaurantDashboard"; // Add this import

function PrivateRoute({ children, allowedRoles }) {
  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");

  if (!token) {
    return <Navigate to="/login" replace />;
  }

  if (allowedRoles && !allowedRoles.includes(role)) {
    return <Navigate to="/unauthorized" replace />;
  }

  return children;
}

function App() {
  return (
    <CartProvider>
      <Router>
        <NavBar />
        <Routes>
          {/* Public Routes */}
          <Route path="/login" element={<Login />} />

          {/* Unauthorized */}
          <Route path="/unauthorized" element={<Unauthorized />} />

          {/* Admin Routes */}
          <Route
            path="/branches"
            element={
              <RequireAuth allowedRoles={["Admin"]}>
                <Branches />
              </RequireAuth>
            }
          />

          {/* Branch selector accessible to all logged in roles */}
          <Route
            path="/branch-selector"
            element={
              <PrivateRoute allowedRoles={["Admin", "Restaurant", "User"]}>
                <BranchSelector />
              </PrivateRoute>
            }
          />

          {/* Menu accessible to Restaurant and User */}
          <Route
            path="/menu"
            element={
              <PrivateRoute allowedRoles={["Restaurant", "User"]}>
                <Menu />
              </PrivateRoute>
            }
          />

          {/* Orders for Restaurant (managing) */}
          <Route path="/restaurant/dashboard" element={<RestaurantDashboard />} />


          {/* Orders for User (view own orders) and Restaurant (optionally) */}
          <Route
            path="/orders"
            element={
              <PrivateRoute allowedRoles={["User", "Restaurant"]}>
                <Orders />
              </PrivateRoute>
            }
          />

          {/* User Routes */}
          <Route
            path="/cart"
            element={
              <PrivateRoute allowedRoles={["User"]}>
                <Cart />
              </PrivateRoute>
            }
          />
          <Route
            path="/checkout"
            element={
              <PrivateRoute allowedRoles={["User"]}>
                <Checkout />
              </PrivateRoute>
            }
          />
          <Route
            path="/review"
            element={
              <PrivateRoute allowedRoles={["User"]}>
                <Review />
              </PrivateRoute>
            }
          />
          <Route path="/order-confirmation" element={<OrderConfirmation />} />

          {/* Default Redirect */}
          <Route path="/" element={<Navigate to="/login" replace />} />
        </Routes>

        <ToastContainer position="top-right" autoClose={2000} />
      </Router>
    </CartProvider>
  );
}

export default App;
