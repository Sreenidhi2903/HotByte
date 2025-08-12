import React, { useEffect, useState } from "react";
import axios from "../api/axios";
import { useNavigate } from "react-router-dom";

function RestaurantDashboard() {
  const [branches, setBranches] = useState([]);
  const [selectedBranchId, setSelectedBranchId] = useState(null);
  const [orders, setOrders] = useState([]);
  const [menuItems, setMenuItems] = useState([]);
  const [activeTab, setActiveTab] = useState("branches"); // 'branches', 'orders', 'menu'
  const [newMenuItem, setNewMenuItem] = useState({
    name: "",
    description: "",
    category: "",
    price: "",
    quantityAvailable: "",
    isAvailable: true,
  });

  const navigate = useNavigate();
  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");
  const city = localStorage.getItem("city");

  // Auth check and load branches
  useEffect(() => {
    if (!token || role !== "Restaurant") {
      navigate("/login");
      return;
    }
    if (!city) {
      alert("City info missing. Please login again.");
      navigate("/login");
      return;
    }
    fetchBranches();
  }, [navigate, token, role, city]);

  // Fetch restaurant branches filtered by city from backend
  const fetchBranches = async () => {
    try {
      const res = await axios.get("/Restaurant/branches");
      setBranches(res.data);
      // Optionally pre-select first branch
      if (res.data.length > 0) setSelectedBranchId(res.data[0].id);
    } catch (err) {
      console.error("Error fetching branches:", err.response?.data || err.message);
    }
  };

  // Fetch orders for selected branch
  const fetchOrders = async () => {
    if (!selectedBranchId) {
      setOrders([]);
      return;
    }
    try {
      const res = await axios.get("/Restaurant/orders");
      // Filter orders by selectedBranchId
      const filtered = res.data.filter((o) => o.branchId === selectedBranchId);
      setOrders(filtered);
    } catch (err) {
      console.error("Error fetching orders:", err.response?.data || err.message);
      setOrders([]);
    }
  };

  // Fetch menu items for selected branch
  const fetchMenuItems = async () => {
    if (!selectedBranchId) {
      setMenuItems([]);
      return;
    }
    try {
      const res = await axios.get(`/Restaurant/branches/${selectedBranchId}/menu`);
      setMenuItems(res.data);
    } catch (err) {
      console.error("Error fetching menu items:", err.response?.data || err.message);
      setMenuItems([]);
    }
  };

  // Update order status and send email
  const updateOrderStatus = async (orderId, newStatus) => {
    try {
      await axios.put(`/Restaurant/orders/${orderId}/status`, { status: newStatus });
      fetchOrders();
    } catch (err) {
      console.error("Error updating order status:", err.response?.data || err.message);
    }
  };

  // CRUD menu item handlers
  const handleAddMenuItem = async () => {
    if (!selectedBranchId) {
      alert("Select a branch first");
      return;
    }
    try {
      const payload = {
        name: newMenuItem.name,
        description: newMenuItem.description,
        category: newMenuItem.category,
        price: parseFloat(newMenuItem.price),
        quantityAvailable: parseInt(newMenuItem.quantityAvailable),
        isAvailable: newMenuItem.isAvailable,
      };
      await axios.post(`/Restaurant/branches/${selectedBranchId}/menu`, payload);
      setNewMenuItem({ name: "", description: "", category: "", price: "", quantityAvailable: "", isAvailable: true });
      fetchMenuItems();
    } catch (err) {
      console.error("Error adding menu item:", err.response?.data || err.message);
    }
  };

  const handleUpdateMenuItem = async (menuItemId, updatedFields) => {
    try {
      await axios.put(`/Restaurant/menu/${menuItemId}`, updatedFields);
      fetchMenuItems();
    } catch (err) {
      console.error("Error updating menu item:", err.response?.data || err.message);
    }
  };

  const handleDeleteMenuItem = async (menuItemId) => {
    if (!window.confirm("Delete this menu item?")) return;
    try {
      await axios.delete(`/Restaurant/menu/${menuItemId}`);
      fetchMenuItems();
    } catch (err) {
      console.error("Error deleting menu item:", err.response?.data || err.message);
    }
  };

  // When branch changes, refresh orders or menu based on activeTab
  useEffect(() => {
    if (activeTab === "orders") fetchOrders();
    else if (activeTab === "menu") fetchMenuItems();
  }, [selectedBranchId, activeTab]);

  return (
    <div className="container mt-4">
      <h2>Restaurant Dashboard</h2>
      <nav className="mb-3">
        <button className={`btn btn-primary me-2 ${activeTab === "branches" ? "active" : ""}`} onClick={() => setActiveTab("branches")}>Branches</button>
        <button
          className={`btn btn-primary me-2 ${activeTab === "orders" ? "active" : ""}`}
          onClick={() => {
            if (!selectedBranchId) return alert("Select a branch first");
            setActiveTab("orders");
          }}
        >
          Orders
        </button>
        <button
          className={`btn btn-primary ${activeTab === "menu" ? "active" : ""}`}
          onClick={() => {
            if (!selectedBranchId) return alert("Select a branch first");
            setActiveTab("menu");
          }}
        >
          Menu
        </button>
      </nav>

      {/* Branches Tab */}
      {activeTab === "branches" && (
        <>
          <h4>Select Branch</h4>
          {branches.length === 0 ? (
            <p>No branches available.</p>
          ) : (
            <ul className="list-group">
              {branches.map((branch) => (
                <li
                  key={branch.id}
                  className={`list-group-item ${branch.id === selectedBranchId ? "active" : ""}`}
                  style={{ cursor: "pointer" }}
                  onClick={() => setSelectedBranchId(branch.id)}
                >
                  {branch.name} - {branch.location} ({branch.contactNumber})
                </li>
              ))}
            </ul>
          )}
        </>
      )}

      {/* Orders Tab */}
      {activeTab === "orders" && (
        <>
          <h4>Orders for Branch: {branches.find((b) => b.id === selectedBranchId)?.name || "N/A"}</h4>
          {orders.length === 0 ? (
            <p>No orders found.</p>
          ) : (
            <table className="table table-bordered">
              <thead>
                <tr>
                  <th>Order ID</th>
                  <th>Status</th>
                  <th>Items</th>
                  <th>Total</th>
                  <th>Address</th>
                  <th>Update Status</th>
                </tr>
              </thead>
              <tbody>
                {orders.map((order) => (
                  <tr key={order.id}>
                    <td>{order.id}</td>
                    <td>{order.status}</td>
                    <td>
                      <ul className="mb-0">
                        {order.items.map((item) => (
                          <li key={item.menuItemId}>
                            {item.menuItemName || "Item"} - Qty: {item.quantity}
                          </li>
                        ))}
                      </ul>
                    </td>
                    <td>₹{order.totalAmount}</td>
                    <td>
                      {order.shippingAddress?.addressLine}, {order.shippingAddress?.city}
                      <br />
                      {order.shippingAddress?.state}, {order.shippingAddress?.pincode}, {order.shippingAddress?.country}
                    </td>
                    <td>
                      <select
                        className="form-control"
                        value={order.status}
                        onChange={(e) => updateOrderStatus(order.id, e.target.value)}
                      >
                        <option value="Pending">Pending</option>
                        <option value="InProgress">In Progress</option>
                        <option value="OutForDelivery">Out For Delivery</option>
                        <option value="Delivered">Delivered</option>
                      </select>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </>
      )}

      {/* Menu Tab */}
      {activeTab === "menu" && (
        <>
          <h4>Menu for Branch: {branches.find((b) => b.id === selectedBranchId)?.name || "N/A"}</h4>

          <div className="mb-3 border p-3">
            <h5>Add New Menu Item</h5>
            <input
              type="text"
              placeholder="Name"
              value={newMenuItem.name}
              onChange={(e) => setNewMenuItem({ ...newMenuItem, name: e.target.value })}
              className="form-control mb-2"
            />
            <input
              type="text"
              placeholder="Description"
              value={newMenuItem.description}
              onChange={(e) => setNewMenuItem({ ...newMenuItem, description: e.target.value })}
              className="form-control mb-2"
            />
            <input
              type="text"
              placeholder="Category"
              value={newMenuItem.category}
              onChange={(e) => setNewMenuItem({ ...newMenuItem, category: e.target.value })}
              className="form-control mb-2"
            />
            <input
              type="number"
              placeholder="Price"
              value={newMenuItem.price}
              onChange={(e) => setNewMenuItem({ ...newMenuItem, price: e.target.value })}
              className="form-control mb-2"
              min="0"
            />
            <input
              type="number"
              placeholder="Quantity Available"
              value={newMenuItem.quantityAvailable}
              onChange={(e) => setNewMenuItem({ ...newMenuItem, quantityAvailable: e.target.value })}
              className="form-control mb-2"
              min="0"
            />
            <div className="form-check mb-2">
              <input
                type="checkbox"
                checked={newMenuItem.isAvailable}
                onChange={(e) => setNewMenuItem({ ...newMenuItem, isAvailable: e.target.checked })}
                className="form-check-input"
                id="availableCheck"
              />
              <label htmlFor="availableCheck" className="form-check-label">
                Available
              </label>
            </div>
            <button className="btn btn-success" onClick={handleAddMenuItem}>
              Add Item
            </button>
          </div>

          <h5>Existing Menu Items</h5>
          {menuItems.length === 0 ? (
            <p>No menu items found.</p>
          ) : (
            <table className="table table-bordered">
              <thead>
                <tr>
                  <th>Name</th>
                  <th>Description</th>
                  <th>Category</th>
                  <th>Price</th>
                  <th>Qty Available</th>
                  <th>Available</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {menuItems.map((item) => (
                  <MenuItemRow
                    key={item.id}
                    item={item}
                    onUpdate={handleUpdateMenuItem}
                    onDelete={handleDeleteMenuItem}
                  />
                ))}
              </tbody>
            </table>
          )}
        </>
      )}
    </div>
  );
}

function MenuItemRow({ item, onUpdate, onDelete }) {
  const [editItem, setEditItem] = useState({ ...item });
  const [isEditing, setIsEditing] = useState(false);

  const saveChanges = () => {
    const updatedFields = {
      name: editItem.name,
      description: editItem.description,
      category: editItem.category,
      price: parseFloat(editItem.price),
      quantityAvailable: parseInt(editItem.quantityAvailable),
      isAvailable: editItem.isAvailable,
    };
    onUpdate(item.id, updatedFields);
    setIsEditing(false);
  };

  return (
    <tr>
      <td>
        {isEditing ? (
          <input
            type="text"
            value={editItem.name}
            onChange={(e) => setEditItem({ ...editItem, name: e.target.value })}
          />
        ) : (
          item.name
        )}
      </td>
      <td>
        {isEditing ? (
          <input
            type="text"
            value={editItem.description}
            onChange={(e) => setEditItem({ ...editItem, description: e.target.value })}
          />
        ) : (
          item.description
        )}
      </td>
      <td>
        {isEditing ? (
          <input
            type="text"
            value={editItem.category}
            onChange={(e) => setEditItem({ ...editItem, category: e.target.value })}
          />
        ) : (
          item.category
        )}
      </td>
      <td>
        {isEditing ? (
          <input
            type="number"
            value={editItem.price}
            onChange={(e) => setEditItem({ ...editItem, price: e.target.value })}
            min="0"
          />
        ) : (
          `₹${item.price}`
        )}
      </td>
      <td>
        {isEditing ? (
          <input
            type="number"
            value={editItem.quantityAvailable}
            onChange={(e) => setEditItem({ ...editItem, quantityAvailable: e.target.value })}
            min="0"
          />
        ) : (
          item.quantityAvailable
        )}
      </td>
      <td>
        {isEditing ? (
          <input
            type="checkbox"
            checked={editItem.isAvailable}
            onChange={(e) => setEditItem({ ...editItem, isAvailable: e.target.checked })}
          />
        ) : item.isAvailable ? (
          "Yes"
        ) : (
          "No"
        )}
      </td>
      <td>
        {isEditing ? (
          <>
            <button className="btn btn-sm btn-success me-2" onClick={saveChanges}>
              Save
            </button>
            <button
              className="btn btn-sm btn-secondary"
              onClick={() => {
                setEditItem({ ...item });
                setIsEditing(false);
              }}
            >
              Cancel
            </button>
          </>
        ) : (
          <>
            <button className="btn btn-sm btn-primary me-2" onClick={() => setIsEditing(true)}>
              Edit
            </button>
            <button className="btn btn-sm btn-danger" onClick={() => onDelete(item.id)}>
              Delete
            </button>
          </>
        )}
      </td>
    </tr>
  );
}

export default RestaurantDashboard;
