// components/RequireAuth.js
import React from "react";
import { Navigate } from "react-router-dom";

const RequireAuth = ({ children, allowedRoles = [] }) => {
  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");

  if (!token) {
    // Not logged in
    return <Navigate to="/login" replace />;
  }

  if (allowedRoles.length && !allowedRoles.includes(role)) {
    // Logged in but role not allowed
    return <Navigate to="/unauthorized" replace />;
  }

  return children;
};

export default RequireAuth;
