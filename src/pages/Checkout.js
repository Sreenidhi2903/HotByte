// import React, { useState } from "react";
// import { Container, Form, Button, Row, Col } from "react-bootstrap";
// import { useCart } from "../context/CartContext";
// import axios from "../api/axios";
// import { useNavigate } from "react-router-dom";

// const Checkout = () => {
//   const { cartItems, clearCart } = useCart();
//   const [addressLine, setAddressLine] = useState("");
//   const [city, setCity] = useState("");
//   const [pincode, setPincode] = useState("");
//   const [state, setState] = useState("");
//   const [country, setCountry] = useState("");
//   const [paymentMode, setPaymentMode] = useState("COD");
//   const [cardDetails, setCardDetails] = useState({ number: "", expiry: "", cvv: "" });
//   const navigate = useNavigate();

//   const totalPrice = cartItems.reduce(
//     (total, item) => total + item.price * (item.quantity || 1),
//     0
//   );

//   const handlePlaceOrder = async () => {
//     if (!addressLine.trim() || !city.trim() || !pincode.trim() || !state.trim() || !country.trim()) {
//       alert("Please fill all shipping address fields");
//       return;
//     }

//     if (
//       paymentMode === "Card" &&
//       (!cardDetails.number || !cardDetails.expiry || !cardDetails.cvv)
//     ) {
//       alert("Please fill all card details");
//       return;
//     }

//     const paymentMethod = paymentMode === "COD" ? 0 : 1;
//     const branchId = Number(localStorage.getItem("selectedBranchId")) || 101;

//     const orderData = {
//       branchId,
//       paymentMethod,
//       items: cartItems.map((item) => ({
//         menuItemId: item.id,
//         quantity: item.quantity || 1,
//       })),
//       shippingAddress: {
//         addressLine,
//         city,
//         pincode,
//         state,
//         country,
//       },
//     };

//     try {
//       await axios.post("/Order/place", orderData, {
//         headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
//       });
//       clearCart();
//       navigate("/order-confirmation");
//     } catch (err) {
//       console.error("Order placement failed:", err);
//       alert("Order placement failed. Please try again.");
//     }
//   };

//   return (
//     <Container className="mt-4">
//       <h2 className="mb-4">Checkout</h2>
//       <Form>
//         <Row>
//           <Col md={6}>
//             <Form.Group className="mb-3">
//               <Form.Label>Address Line</Form.Label>
//               <Form.Control
//                 type="text"
//                 value={addressLine}
//                 onChange={(e) => setAddressLine(e.target.value)}
//                 placeholder="Street address"
//                 required
//               />
//             </Form.Group>
//           </Col>
//           <Col md={6}>
//             <Form.Group className="mb-3">
//               <Form.Label>City</Form.Label>
//               <Form.Control
//                 type="text"
//                 value={city}
//                 onChange={(e) => setCity(e.target.value)}
//                 placeholder="City"
//                 required
//               />
//             </Form.Group>
//           </Col>
//         </Row>

//         <Row>
//           <Col md={4}>
//             <Form.Group className="mb-3">
//               <Form.Label>Pincode</Form.Label>
//               <Form.Control
//                 type="text"
//                 value={pincode}
//                 onChange={(e) => setPincode(e.target.value)}
//                 placeholder="Pincode"
//                 required
//               />
//             </Form.Group>
//           </Col>
//           <Col md={4}>
//             <Form.Group className="mb-3">
//               <Form.Label>State</Form.Label>
//               <Form.Control
//                 type="text"
//                 value={state}
//                 onChange={(e) => setState(e.target.value)}
//                 placeholder="State"
//                 required
//               />
//             </Form.Group>
//           </Col>
//           <Col md={4}>
//             <Form.Group className="mb-3">
//               <Form.Label>Country</Form.Label>
//               <Form.Control
//                 type="text"
//                 value={country}
//                 onChange={(e) => setCountry(e.target.value)}
//                 placeholder="Country"
//                 required
//               />
//             </Form.Group>
//           </Col>
//         </Row>

//         <Form.Group className="mb-3">
//           <Form.Label>Payment Mode</Form.Label>
//           <Form.Select
//             value={paymentMode}
//             onChange={(e) => setPaymentMode(e.target.value)}
//           >
//             <option value="COD">Cash on Delivery</option>
//             <option value="Card">Prepaid Card</option>
//           </Form.Select>
//         </Form.Group>

//         {paymentMode === "Card" && (
//           <>
//             <Form.Group className="mb-3">
//               <Form.Label>Card Number</Form.Label>
//               <Form.Control
//                 type="text"
//                 maxLength="16"
//                 value={cardDetails.number}
//                 onChange={(e) =>
//                   setCardDetails({ ...cardDetails, number: e.target.value })
//                 }
//                 placeholder="1234 5678 9012 3456"
//               />
//             </Form.Group>
//             <Form.Group className="mb-3">
//               <Form.Label>Expiry Date</Form.Label>
//               <Form.Control
//                 type="text"
//                 placeholder="MM/YY"
//                 value={cardDetails.expiry}
//                 onChange={(e) =>
//                   setCardDetails({ ...cardDetails, expiry: e.target.value })
//                 }
//               />
//             </Form.Group>
//             <Form.Group className="mb-3">
//               <Form.Label>CVV</Form.Label>
//               <Form.Control
//                 type="password"
//                 maxLength="3"
//                 value={cardDetails.cvv}
//                 onChange={(e) =>
//                   setCardDetails({ ...cardDetails, cvv: e.target.value })
//                 }
//               />
//             </Form.Group>
//           </>
//         )}

//         <h4 className="mt-3 mb-4">Total Amount: ₹{totalPrice}</h4>
//         <Button
//           variant="primary"
//           onClick={handlePlaceOrder}
//           disabled={cartItems.length === 0}
//         >
//           Place Order
//         </Button>
//       </Form>
//     </Container>
//   );
// };

// export default Checkout;

import React, { useState } from "react";
import { Container, Form, Button, Row, Col, Spinner } from "react-bootstrap";
import { useCart } from "../context/CartContext";
import axios from "../api/axios";
import { useNavigate } from "react-router-dom";

const Checkout = () => {
  const { cartItems, clearCart } = useCart();
  const [addressLine, setAddressLine] = useState("");
  const [city, setCity] = useState("");
  const [pincode, setPincode] = useState("");
  const [state, setState] = useState("");
  const [country, setCountry] = useState("");
  const [paymentMode, setPaymentMode] = useState("COD");
  const [cardDetails, setCardDetails] = useState({ number: "", expiry: "", cvv: "" });
  const [loading, setLoading] = useState(false);  // <---- loading state
  const navigate = useNavigate();

  const totalPrice = cartItems.reduce(
    (total, item) => total + item.price * (item.quantity || 1),
    0
  );

  const handlePlaceOrder = async () => {
    if (!addressLine.trim() || !city.trim() || !pincode.trim() || !state.trim() || !country.trim()) {
      alert("Please fill all shipping address fields");
      return;
    }

    if (
      paymentMode === "Card" &&
      (!cardDetails.number || !cardDetails.expiry || !cardDetails.cvv)
    ) {
      alert("Please fill all card details");
      return;
    }

    setLoading(true);  // Disable button and show spinner

    const paymentMethod = paymentMode === "COD" ? 0 : 1;
    const branchId = Number(localStorage.getItem("selectedBranchId")) || 101;

    const orderData = {
      branchId,
      paymentMethod,
      items: cartItems.map((item) => ({
        menuItemId: item.id,
        quantity: item.quantity || 1,
      })),
      shippingAddress: {
        addressLine,
        city,
        pincode,
        state,
        country,
      },
    };

    try {
      await axios.post("/Order/place", orderData, {
        headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
      });
      clearCart();
      navigate("/order-confirmation");
    } catch (err) {
      console.error("Order placement failed:", err);
      alert("Order placement failed. Please try again.");
    } finally {
      setLoading(false);  // Re-enable button if error occurs
    }
  };

  return (
    <Container className="mt-4">
      <h2 className="mb-4">Checkout</h2>
      <Form>
        {/* Address inputs */}
        <Row>
          <Col md={6}>
            <Form.Group className="mb-3">
              <Form.Label>Address Line</Form.Label>
              <Form.Control
                type="text"
                value={addressLine}
                onChange={(e) => setAddressLine(e.target.value)}
                placeholder="Street address"
                required
                disabled={loading}
              />
            </Form.Group>
          </Col>
          <Col md={6}>
            <Form.Group className="mb-3">
              <Form.Label>City</Form.Label>
              <Form.Control
                type="text"
                value={city}
                onChange={(e) => setCity(e.target.value)}
                placeholder="City"
                required
                disabled={loading}
              />
            </Form.Group>
          </Col>
        </Row>

        <Row>
          <Col md={4}>
            <Form.Group className="mb-3">
              <Form.Label>Pincode</Form.Label>
              <Form.Control
                type="text"
                value={pincode}
                onChange={(e) => setPincode(e.target.value)}
                placeholder="Pincode"
                required
                disabled={loading}
              />
            </Form.Group>
          </Col>
          <Col md={4}>
            <Form.Group className="mb-3">
              <Form.Label>State</Form.Label>
              <Form.Control
                type="text"
                value={state}
                onChange={(e) => setState(e.target.value)}
                placeholder="State"
                required
                disabled={loading}
              />
            </Form.Group>
          </Col>
          <Col md={4}>
            <Form.Group className="mb-3">
              <Form.Label>Country</Form.Label>
              <Form.Control
                type="text"
                value={country}
                onChange={(e) => setCountry(e.target.value)}
                placeholder="Country"
                required
                disabled={loading}
              />
            </Form.Group>
          </Col>
        </Row>

        {/* Payment Mode */}
        <Form.Group className="mb-3">
          <Form.Label>Payment Mode</Form.Label>
          <Form.Select
            value={paymentMode}
            onChange={(e) => setPaymentMode(e.target.value)}
            disabled={loading}
          >
            <option value="COD">Cash on Delivery</option>
            <option value="Card">Prepaid Card</option>
          </Form.Select>
        </Form.Group>

        {paymentMode === "Card" && (
          <>
            <Form.Group className="mb-3">
              <Form.Label>Card Number</Form.Label>
              <Form.Control
                type="text"
                maxLength="16"
                value={cardDetails.number}
                onChange={(e) =>
                  setCardDetails({ ...cardDetails, number: e.target.value })
                }
                placeholder="1234 5678 9012 3456"
                disabled={loading}
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Expiry Date</Form.Label>
              <Form.Control
                type="text"
                placeholder="MM/YY"
                value={cardDetails.expiry}
                onChange={(e) =>
                  setCardDetails({ ...cardDetails, expiry: e.target.value })
                }
                disabled={loading}
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>CVV</Form.Label>
              <Form.Control
                type="password"
                maxLength="3"
                value={cardDetails.cvv}
                onChange={(e) =>
                  setCardDetails({ ...cardDetails, cvv: e.target.value })
                }
                disabled={loading}
              />
            </Form.Group>
          </>
        )}

        <h4 className="mt-3 mb-4">Total Amount: ₹{totalPrice}</h4>
        <Button
          variant="primary"
          onClick={handlePlaceOrder}
          disabled={cartItems.length === 0 || loading}
        >
          {loading ? (
            <>
              <Spinner animation="border" size="sm" /> Placing order...
            </>
          ) : (
            "Place Order"
          )}
        </Button>
      </Form>
    </Container>
  );
};

export default Checkout;
