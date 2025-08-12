import React, { useEffect, useState } from "react";
import axios from "../api/axios";  // your configured axios instance
import { Container, Card, Spinner, Alert, Button } from "react-bootstrap";
import { useLocation, useNavigate } from "react-router-dom";

const Orders = () => {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const token = localStorage.getItem("token");
  const userRole = localStorage.getItem("role"); // "User", "Restaurant", "Admin"

  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchOrders = async () => {
      setLoading(true);
      setError("");

      try {
        let endpoint = "";

        if (userRole === "User") {
          endpoint = "/Order/my";
        } else if (userRole === "Restaurant") {
          endpoint = "/Order/restaurant";
        } else if (userRole === "Admin") {
          // If you have an admin orders endpoint, put it here
          endpoint = "/Order/restaurant"; // or wherever Admin should fetch from
        } else {
          setError("Unknown user role.");
          setLoading(false);
          return;
        }

        const response = await axios.get(endpoint, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        setOrders(response.data);
      } catch (err) {
        console.error(err);
        setError("Failed to fetch orders. Please try again.");
      } finally {
        setLoading(false);
      }
    };

    fetchOrders();
  }, [token, userRole, location]);

  if (loading) {
    return (
      <div className="text-center mt-5">
        <Spinner animation="border" variant="primary" />
        <p>Loading your orders...</p>
      </div>
    );
  }

  if (error) {
    return (
      <Container className="mt-4">
        <Alert variant="danger">{error}</Alert>
      </Container>
    );
  }

  return (
    <Container className="mt-4">
      <h3 className="mb-4">My Orders</h3>

      {orders.length === 0 ? (
        <p>You have no orders yet.</p>
      ) : (
        orders.map((order) => (
          <Card key={order.id} className="mb-4 shadow-sm">
            <Card.Body>
              <Card.Title>Order #{order.id}</Card.Title>
              <Card.Subtitle className="mb-2 text-muted">
                Placed on: {new Date(order.createdAt).toLocaleString()}
              </Card.Subtitle>
              <p>
                <strong>Status:</strong> {order.status}
              </p>
              <p>
                <strong>Total Amount:</strong> ₹{order.totalAmount}
              </p>

              <h6 className="mt-3">Items:</h6>
              <ul className="list-group">
                {order.items.map((item, index) => (
                  <li
                    key={index}
                    className="list-group-item d-flex justify-content-between"
                  >
                    <div>
                      {item.menuItemName} x {item.quantity}
                    </div>
                    <div>₹{item.price}</div>
                  </li>
                ))}
              </ul>

              {order.status === "Delivered" && (
                <Button
                  className="mt-3"
                  onClick={() => navigate(`/review/${order.id}`)}
                >
                  Write Review
                </Button>
              )}
            </Card.Body>
          </Card>
        ))
      )}
    </Container>
  );
};

export default Orders;

