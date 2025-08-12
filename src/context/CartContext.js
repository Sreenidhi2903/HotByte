import React, { createContext, useContext, useState } from "react";

const CartContext = createContext();

export const CartProvider = ({ children }) => {
  const [cartItems, setCartItems] = useState([]);

  // Example addToCart function
const addToCart = (itemToAdd) => {
  setCartItems((prevItems) => {
    const existingItem = prevItems.find(item => item.id === itemToAdd.id);
    if (existingItem) {
      // update quantity
      return prevItems.map(item =>
        item.id === itemToAdd.id
          ? { ...item, quantity: (item.quantity || 1) + (itemToAdd.quantity || 1) }
          : item
      );
    } else {
      // add new item with quantity
      return [...prevItems, { ...itemToAdd, quantity: itemToAdd.quantity || 1 }];
    }
  });
};


  const removeFromCart = (itemId) => {
    setCartItems((prevItems) => prevItems.filter((item) => item.id !== itemId));
  };

  const clearCart = () => {
    setCartItems([]);
  };

  return (
    <CartContext.Provider value={{ cartItems, addToCart, removeFromCart, clearCart }}>
      {children}
    </CartContext.Provider>
  );
};

// ✅ Properly export the custom hook
export const useCart = () => {
  const context = useContext(CartContext);
  if (!context) {
    throw new Error("useCart must be used within a CartProvider");
  }
  return context;
};
// import React, { createContext, useContext, useState } from "react";
// import axiosInstance from "../api/axios"; // Correct import path here

// const CartContext = createContext();

// export const CartProvider = ({ children }) => {
//   const [cartItems, setCartItems] = useState([]);

//   const addToCart = (itemToAdd) => {
//     setCartItems((prevItems) => {
//       const existingItem = prevItems.find((item) => item.id === itemToAdd.id);
//       if (existingItem) {
//         return prevItems.map((item) =>
//           item.id === itemToAdd.id
//             ? { ...item, quantity: (item.quantity || 1) + (itemToAdd.quantity || 1) }
//             : item
//         );
//       } else {
//         return [...prevItems, { ...itemToAdd, quantity: itemToAdd.quantity || 1 }];
//       }
//     });
//   };

// const removeFromCart = async (itemId) => {
//   try {
//     const response = await axiosInstance.delete(`/Cart/remove/${itemId}`);
//     if (response.status !== 200) {
//       throw new Error("Failed to remove item from cart");
//     }
//     setCartItems((prevItems) => prevItems.filter((item) => item.id !== itemId));
//   } catch (error) {
//     console.error("Error removing item from cart:", error);
//   }
// };


//   const clearCart = () => {
//     setCartItems([]);
//   };

//   return (
//     <CartContext.Provider value={{ cartItems, addToCart, removeFromCart, clearCart }}>
//       {children}
//     </CartContext.Provider>
//   );
// };

// export const useCart = () => {
//   const context = useContext(CartContext);
//   if (!context) {
//     throw new Error("useCart must be used within a CartProvider");
//   }
//   return context;
// };
