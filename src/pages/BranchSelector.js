// src/pages/BranchSelector.js
import React, { useEffect, useState } from "react";
import axios from "../api/axios";
import { useNavigate } from "react-router-dom";

function BranchSelector() {
  const [branches, setBranches] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchBranches = async () => {
      const token = localStorage.getItem("token");
      try {
        const response = await axios.get("/Branch", {
          headers: { Authorization: `Bearer ${token}` },
        });
        setBranches(response.data);
      } catch (error) {
        console.error("Failed to fetch branches", error);
      }
    };

    fetchBranches();
  }, []);

  const handleSelect = (branch) => {
    localStorage.setItem("selectedBranchId", branch.id);
    localStorage.setItem("selectedBranchName", branch.name);
    navigate("/menu");
  };

  return (
    <div className="container mt-5">
      <h3 className="text-center mb-4">Select a Branch</h3>
      <div className="row">
        {branches.map((branch) => (
          <div className="col-md-4 mb-3" key={branch.id}>
            <div
              className="card p-3 shadow-sm text-center"
              onClick={() => handleSelect(branch)}
              style={{ cursor: "pointer" }}
            >
              <h5>{branch.name}</h5>
              <p>{branch.location}</p>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

export default BranchSelector;
