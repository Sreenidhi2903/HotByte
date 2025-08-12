// import React, { useEffect, useState } from 'react';
// import axios from '../api/axios';
// import { useNavigate } from 'react-router-dom';

// function RestaurantOrders() {
//   const [orders, setOrders] = useState([]);
//   const navigate = useNavigate();

//   const fetchOrders = async () => {
//     try {
//       const token = localStorage.getItem('token');
//       const response = await axios.get('https://localhost:7194/api/Order/placed', {
//         headers: {
//           Authorization: `Bearer ${token}`,
//         },
//       });
//       setOrders(response.data);
//     } catch (error) {
//       console.error('Error fetching orders:', error);
//     }
//   };

//   const updateOrderStatus = async (orderId, newStatus) => {
//     try {
//       const token = localStorage.getItem('token');
//       await axios.put(
//         `https://localhost:7194/api/Order/${orderId}/status`,
//         `"${newStatus}"`,
//         {
//           headers: {
//             'Content-Type': 'application/json',
//             Authorization: `Bearer ${token}`,
//           },
//         }
//       );
//       fetchOrders(); // Refresh orders after update
//     } catch (error) {
//       console.error('Error updating order status:', error);
//     }
//   };

//   useEffect(() => {
//     const role = localStorage.getItem('role');
//     if (!role || role !== 'Restaurant') {
//       navigate('/login');
//     } else {
//       fetchOrders();
//     }
//   }, []);

//   return (
//     <div className="container mt-4">
//       <h2 className="mb-4">Placed Orders</h2>
//       {orders.length === 0 ? (
//         <p>No orders found.</p>
//       ) : (
//         <div className="table-responsive">
//           <table className="table table-bordered table-striped">
//             <thead className="thead-dark">
//               <tr>
//                 <th>Order ID</th>
//                 <th>Status</th>
//                 <th>Items</th>
//                 <th>Total</th>
//                 <th>Address</th>
//                 <th>Update Status</th>
//               </tr>
//             </thead>
//             <tbody>
//               {orders.map(order => (
//                 <tr key={order.id}>
//                   <td>{order.id}</td>
//                   <td>{order.status}</td>
//                   <td>
//                     <ul className="mb-0">
//                       {order.items.map(item => (
//                         <li key={item.menuItemId}>
//                           {item.menuItemName || 'Item'} - Qty: {item.quantity}
//                         </li>
//                       ))}
//                     </ul>
//                   </td>
//                   <td>₹{order.totalAmount}</td>
//                   <td>
//                     {order.shippingAddress?.addressLine}, {order.shippingAddress?.city}<br />
//                     {order.shippingAddress?.state}, {order.shippingAddress?.pincode}, {order.shippingAddress?.country}
//                   </td>
//                   <td>
//                     <select
//                       className="form-control"
//                       value={order.status}
//                       onChange={(e) => updateOrderStatus(order.id, e.target.value)}
//                     >
//                       <option value="Pending">Pending</option>
//                       <option value="InProgress">In Progress</option>
//                       <option value="OutForDelivery">Out For Delivery</option>
//                       <option value="Delivered">Delivered</option>
//                     </select>
//                   </td>
//                 </tr>
//               ))}
//             </tbody>
//           </table>
//         </div>
//       )}
//     </div>
//   );
// }

// export default RestaurantOrders;
