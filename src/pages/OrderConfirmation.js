import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { FaCheckCircle } from "react-icons/fa";

export default function OrderConfirmation() {
  const navigate = useNavigate();

  useEffect(() => {
    const timer = setTimeout(() => {
      navigate("/orders"); // Redirect after 3 seconds
    }, 3000);
    return () => clearTimeout(timer);
  }, [navigate]);

  return (
    <div style={{ textAlign: "center", marginTop: "80px", padding: "20px" }}>
      <FaCheckCircle size={80} color="green" />
      <h2 style={{ marginTop: "20px", color: "green" }}>Order Placed Successfully!</h2>
      <p>You will be redirected to your orders...</p>
    </div>
  );
}

