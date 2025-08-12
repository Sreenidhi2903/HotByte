import React, { useEffect, useState } from "react";
import axios from "../api/axios";
import { useNavigate } from "react-router-dom";
import { Button, Form, Table, Modal, Spinner } from "react-bootstrap";

function Branches() {
  const [branches, setBranches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [showModal, setShowModal] = useState(false);
  const [editingBranch, setEditingBranch] = useState(null);
  const [name, setName] = useState("");
  const [location, setLocation] = useState("");
  const [contactNumber, setContactNumber] = useState("");  // NEW
  const [saving, setSaving] = useState(false);
  const [deletingId, setDeletingId] = useState(null);

  const navigate = useNavigate();
  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");

  useEffect(() => {
    if (!token || role !== "Admin") {
      navigate("/login");
      return;
    }
    fetchBranches();
  }, [role, token, navigate]);

  const fetchBranches = async () => {
    setLoading(true);
    try {
      const response = await axios.get("/Admin/branches", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setBranches(response.data);
      setError("");
    } catch (err) {
      console.error(err);
      setError("Failed to fetch branches");
    } finally {
      setLoading(false);
    }
  };

  const openAddModal = () => {
    setEditingBranch(null);
    setName("");
    setLocation("");
    setContactNumber("");  // reset
    setShowModal(true);
  };

  const openEditModal = (branch) => {
    setEditingBranch(branch);
    setName(branch.name);
    setLocation(branch.location);
    setContactNumber(branch.contactNumber || "");  // populate
    setShowModal(true);
  };

  const handleSave = async () => {
    if (!name.trim() || !location.trim() || !contactNumber.trim()) {
      alert("Please enter name, location, and contact number");
      return;
    }

    setSaving(true);
    try {
      if (editingBranch) {
        await axios.put(
          `/Admin/branches/${editingBranch.id}`,
          { name, location, contactNumber },  // send contactNumber
          { headers: { Authorization: `Bearer ${token}` } }
        );
      } else {
        await axios.post(
          "/Admin/branches",
          { name, location, contactNumber },  // send contactNumber
          { headers: { Authorization: `Bearer ${token}` } }
        );
      }
      fetchBranches();
      setShowModal(false);
    } catch (err) {
      console.error(err);
      alert("Failed to save branch");
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (branchId) => {
    if (!window.confirm("Are you sure to delete this branch?")) return;

    setDeletingId(branchId);
    try {
      await axios.delete(`/Admin/branches/${branchId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      fetchBranches();
    } catch (err) {
      console.error(err);
      alert("Failed to delete branch");
    } finally {
      setDeletingId(null);
    }
  };

  if (loading) return <p>Loading branches...</p>;
  if (error) return <p className="text-danger">{error}</p>;

  return (
    <div className="container mt-4">
      <h2>Manage Branches (Admin)</h2>
      <Button className="mb-3" onClick={openAddModal}>
        Add New Branch
      </Button>

      <Table striped bordered hover>
        <thead>
          <tr>
            <th>Name</th>
            <th>Location</th>
            <th>Contact Number</th> {/* NEW column */}
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {branches.map((branch) => (
            <tr key={branch.id}>
              <td>{branch.name}</td>
              <td>{branch.location}</td>
              <td>{branch.contactNumber}</td> {/* display contactNumber */}
              <td>
                <Button
                  variant="warning"
                  size="sm"
                  onClick={() => openEditModal(branch)}
                  className="me-2"
                  disabled={deletingId === branch.id || saving}
                >
                  Edit
                </Button>
                <Button
                  variant="danger"
                  size="sm"
                  onClick={() => handleDelete(branch.id)}
                  disabled={deletingId === branch.id || saving}
                >
                  {deletingId === branch.id ? (
                    <>
                      <Spinner
                        as="span"
                        animation="border"
                        size="sm"
                        role="status"
                        aria-hidden="true"
                      />{" "}
                      Deleting...
                    </>
                  ) : (
                    "Delete"
                  )}
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

      {/* Modal for Add/Edit Branch */}
      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>{editingBranch ? "Edit Branch" : "Add Branch"}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form.Group className="mb-3">
            <Form.Label>Branch Name</Form.Label>
            <Form.Control
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              disabled={saving}
            />
          </Form.Group>
          <Form.Group className="mb-3">
            <Form.Label>Location</Form.Label>
            <Form.Control
              type="text"
              value={location}
              onChange={(e) => setLocation(e.target.value)}
              disabled={saving}
            />
          </Form.Group>
          <Form.Group>
            <Form.Label>Contact Number</Form.Label>
            <Form.Control
              type="text"
              value={contactNumber}
              onChange={(e) => setContactNumber(e.target.value)}
              disabled={saving}
            />
          </Form.Group>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowModal(false)} disabled={saving}>
            Cancel
          </Button>
          <Button variant="primary" onClick={handleSave} disabled={saving}>
            {saving ? (
              <>
                <Spinner
                  as="span"
                  animation="border"
                  size="sm"
                  role="status"
                  aria-hidden="true"
                />{" "}
                Saving...
              </>
            ) : (
              "Save"
            )}
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
}

export default Branches;

