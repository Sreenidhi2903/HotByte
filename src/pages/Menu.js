
// import React, { useEffect, useState } from "react";
// import { Container, Row, Col, Card, Button, Form } from "react-bootstrap";
// import axios from "../api/axios";
// import { useCart } from "../context/CartContext";

// const Menu = () => {
//   const { addToCart } = useCart();
//   const [menuItems, setMenuItems] = useState([]);
//   const [searchTerm, setSearchTerm] = useState("");
//   const [loading, setLoading] = useState(true);

//   useEffect(() => {
//     const branchId = localStorage.getItem("selectedBranchId");
//     if (!branchId) {
//       console.error("No branch selected.");
//       setLoading(false);
//       return;
//     }

//     const fetchMenuItems = async () => {
//       try {
//         const token = localStorage.getItem("token");
//         const response = await axios.get(`/Menu/branch/${branchId}`, {
//           headers: { Authorization: `Bearer ${token}` },
//         });

//         console.log("Fetched menu items:", response.data);

//         // Combine branchItems + commonItems into one array
//         const combinedItems = [
//           ...(response.data.branchItems || []),
//           ...(response.data.commonItems || []),
//         ];

//         setMenuItems(combinedItems);
//       } catch (error) {
//         console.error("Error fetching menu items:", error);
//       } finally {
//         setLoading(false);
//       }
//     };

//     fetchMenuItems();
//   }, []);

//   const handleAddToCart = (item) => {
//     try {
//       addToCart(item);
//     } catch (error) {
//       console.error("Failed to add to cart:", error);
//     }
//   };

//   const filteredItems = menuItems.filter((item) =>
//     item.name.toLowerCase().includes(searchTerm.toLowerCase())
//   );

//   if (loading) {
//     return <p className="text-center mt-4">Loading menu...</p>;
//   }

//   return (
//     <Container className="mt-4">
//       <h2 className="text-center mb-4">Menu</h2>
//       <Form className="mb-4">
//         <Form.Control
//           type="text"
//           placeholder="Search items..."
//           value={searchTerm}
//           onChange={(e) => setSearchTerm(e.target.value)}
//         />
//       </Form>
//       <Row>
//         {filteredItems.length > 0 ? (
//           filteredItems.map((item) => (
//             <Col key={item.id} sm={12} md={6} lg={4} className="mb-4">
//               <Card className="shadow-sm h-100">
//                 {item.imageUrl && (
//                   <Card.Img
//                     variant="top"
//                     src={item.imageUrl}
//                     alt={item.name}
//                     style={{ height: "200px", objectFit: "cover" }}
//                   />
//                 )}
//                 <Card.Body className="d-flex flex-column">
//                   <Card.Title>{item.name}</Card.Title>
//                   <Card.Text>{item.description}</Card.Text>
//                   <Card.Text className="fw-bold">₹{item.price}</Card.Text>
//                   <Button
//                     variant="primary"
//                     className="mt-auto"
//                     onClick={() => handleAddToCart(item)}
//                   >
//                     Add to Cart
//                   </Button>
//                 </Card.Body>
//               </Card>
//             </Col>
//           ))
//         ) : (
//           <p className="text-center">No items found.</p>
//         )}
//       </Row>
//     </Container>
//   );
// };

// export default Menu;
import React, { useEffect, useState } from "react";
import axios from "../api/axios";
import { useCart } from "../context/CartContext";
import { Container, Row, Col, Card, Button, Form, Toast } from "react-bootstrap";

const Menu = () => {
  const { addToCart } = useCart();
  const [menuItems, setMenuItems] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  
  // quantity state: { [itemId]: quantity }
  const [quantities, setQuantities] = useState({});
  
  // toast state
  const [showToast, setShowToast] = useState(false);
  const [toastMessage, setToastMessage] = useState("");

  const branchId = localStorage.getItem("selectedBranchId");

  useEffect(() => {
    if (!branchId) {
      setError("Please select a branch first.");
      setLoading(false);
      return;
    }

    const fetchMenu = async () => {
      try {
        setLoading(true);
        setError("");

        const response = await axios.get(`/Menu/branch/${branchId}`);
        console.log("Fetched menu items (raw):", response.data);

        const { commonItems = {}, specialItems = {} } = response.data;

        const commonArray = Object.values(commonItems);
        const specialArray = Object.values(specialItems);

        const combined = [...commonArray, ...specialArray];

        setMenuItems(combined);

        // Initialize quantities to 1 for all items
        const initialQuantities = {};
        combined.forEach((item) => {
          initialQuantities[item.id] = 1;
        });
        setQuantities(initialQuantities);
      } catch (err) {
        console.error("Error fetching menu:", err);
        setError("Failed to load menu items.");
      } finally {
        setLoading(false);
      }
    };

    fetchMenu();
  }, [branchId]);

  const handleAddToCart = async (item) => {
    try {
      // Add quantity info to the item before adding to cart
      const quantity = quantities[item.id] || 1;
      await addToCart({ ...item, quantity });

      setToastMessage(`Added ${quantity} x ${item.name} to cart`);
      setShowToast(true);

      // Optional: reset quantity to 1 after adding
      setQuantities((prev) => ({ ...prev, [item.id]: 1 }));
    } catch (err) {
      console.error("Error adding to cart:", err);
      setError("Failed to add item to cart.");
    }
  };

  const filteredItems = menuItems.filter((item) =>
    item.name?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleQuantityChange = (itemId, delta) => {
    setQuantities((prev) => {
      const newQty = (prev[itemId] || 1) + delta;
      if (newQty < 1) return prev; // minimum 1
      return { ...prev, [itemId]: newQty };
    });
  };

  if (loading) {
    return <p className="text-center mt-5">Loading menu...</p>;
  }

  if (error) {
    return <p className="text-center text-danger mt-5">{error}</p>;
  }

  return (
    <Container className="mt-4">
      <h2 className="mb-4">Menu</h2>

      <Form className="mb-4">
        <Form.Control
          type="text"
          placeholder="Search menu items..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
      </Form>

      {filteredItems.length === 0 ? (
        <p>No items found</p>
      ) : (
        <Row>
          {filteredItems.map((item) => (
            <Col key={item.id} sm={12} md={6} lg={4} className="mb-4">
              <Card className="h-100">
                <Card.Body>
                  <Card.Title>{item.name}</Card.Title>
                  <Card.Text>Price: ₹{item.price}</Card.Text>
                  <div className="d-flex align-items-center mb-3">
                    <Button
                      variant="outline-secondary"
                      size="sm"
                      onClick={() => handleQuantityChange(item.id, -1)}
                    >
                      -
                    </Button>
                    <span className="mx-3">{quantities[item.id] || 1}</span>
                    <Button
                      variant="outline-secondary"
                      size="sm"
                      onClick={() => handleQuantityChange(item.id, 1)}
                    >
                      +
                    </Button>
                  </div>
                  <Button
                    variant="primary"
                    onClick={() => handleAddToCart(item)}
                  >
                    Add to Cart
                  </Button>
                </Card.Body>
              </Card>
            </Col>
          ))}
        </Row>
      )}

      <Toast
        onClose={() => setShowToast(false)}
        show={showToast}
        delay={2000}
        autohide
        style={{
          position: "fixed",
          bottom: 20,
          right: 20,
          minWidth: "200px",
        }}
      >
        <Toast.Header>
          <strong className="me-auto">Cart</strong>
        </Toast.Header>
        <Toast.Body>{toastMessage}</Toast.Body>
      </Toast>
    </Container>
  );
};

export default Menu;
