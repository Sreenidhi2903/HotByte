// import React from "react";
// import { useCart } from "../context/CartContext";
// import { Container, Table, Button } from "react-bootstrap";
// import { useNavigate } from "react-router-dom";

// const Cart = () => {
//   const { cartItems, removeFromCart } = useCart();
//   const navigate = useNavigate();

//   const totalPrice = cartItems.reduce(
//     (total, item) => total + item.price * (item.quantity || 1),
//     0
//   );

//   return (
//     <Container className="mt-4">
//       <h2>Your Cart</h2>
//       {cartItems.length === 0 ? (
//         <p>Your cart is empty.</p>
//       ) : (
//         <>
//           <Table striped bordered hover>
//             <thead>
//               <tr>
//                 <th>Item</th>
//                 <th>Quantity</th> {/* Added Quantity column */}
//                 <th>Price</th>
//                 <th>Action</th>
//               </tr>
//             </thead>
//             <tbody>
//               {cartItems.map((item, idx) => (
//                 <tr key={idx}>
//                   <td>{item.name}</td>
//                   <td>{item.quantity || 1}</td>
//                   <td>₹{item.price * (item.quantity || 1)}</td>
//                   <td>
//                     <Button variant="danger" onClick={() => removeFromCart(item)}>
//                       Remove
//                     </Button>
//                   </td>
//                 </tr>
//               ))}
//             </tbody>
//           </Table>
//           <h4>Total: ₹{totalPrice}</h4>
//           <Button variant="success" onClick={() => navigate("/checkout")}>
//             Proceed to Checkout
//           </Button>
//         </>
//       )}
//     </Container>
//   );
// };

// export default Cart;
import React from "react";
import { useCart } from "../context/CartContext";
import { Container, Table, Button } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

const Cart = () => {
  const { cartItems, removeFromCart } = useCart();
  const navigate = useNavigate();

  const totalPrice = cartItems.reduce(
    (total, item) => total + item.price * (item.quantity || 1),
    0
  );

  return (
    <Container className="mt-4" style={{ maxWidth: "600px", margin: "auto" }}>
      <h2 className="mb-4 text-center">Your Cart</h2>
      {cartItems.length === 0 ? (
        <p className="text-center">Your cart is empty.</p>
      ) : (
        <>
          <Table striped bordered hover>
            <thead>
              <tr>
                <th>Item</th>
                <th>Quantity</th>
                <th>Price</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {cartItems.map((item) => (
                <tr key={item.id}>
                  <td>{item.name}</td>
                  <td>{item.quantity || 1}</td>
                  <td>₹{item.price * (item.quantity || 1)}</td>
                  <td>
                    <Button
                      variant="danger"
                      onClick={() => removeFromCart(item.id)}
                    >
                      Remove
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>
          <h4 className="text-end">Total: ₹{totalPrice}</h4>
          <div className="d-flex justify-content-center">
            <Button variant="success" onClick={() => navigate("/checkout")}>
              Proceed to Checkout
            </Button>
          </div>
        </>
      )}
    </Container>
  );
};

export default Cart;





