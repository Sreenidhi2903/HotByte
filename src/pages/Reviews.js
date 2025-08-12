import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

const Review = () => {
  const [menuItems, setMenuItems] = useState([]);
  const [reviews, setReviews] = useState({});
  const [userEmail, setUserEmail] = useState('');

  const token = localStorage.getItem('token');
  const branchId = localStorage.getItem('branchId');

  useEffect(() => {
    if (!token) return;

    // Decode JWT to get user email
    const decoded = jwtDecode(token);
    setUserEmail(decoded.email);

    // Fetch menu items for the selected branch
    axios
      .get(`https://localhost:7194/api/Menu/${branchId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
      .then((res) => setMenuItems(res.data))
      .catch((err) => console.error('Failed to fetch menu items:', err));
  }, [token, branchId]);

  const handleInputChange = (menuItemId, field, value) => {
    setReviews((prev) => ({
      ...prev,
      [menuItemId]: {
        ...prev[menuItemId],
        [field]: value,
      },
    }));
  };

  const handleSubmit = (menuItemId) => {
    const review = reviews[menuItemId];

    if (!review || !review.rating || !review.comment) {
      alert('Please provide both rating and comment');
      return;
    }

    axios
      .post(
        'https://localhost:7194/api/Review',
        {
          menuItemId,
          rating: parseInt(review.rating),
          comment: review.comment,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        }
      )
      .then(() => {
        alert('Review submitted successfully!');
        setReviews((prev) => ({
          ...prev,
          [menuItemId]: { rating: '', comment: '' },
        }));
      })
      .catch((err) => {
        console.error('Error submitting review:', err);
        alert('Failed to submit review');
      });
  };

  return (
    <div className="container mt-4">
      <h2 className="text-center mb-4">Submit Your Review</h2>

      {menuItems.length === 0 ? (
        <p className="text-center">No menu items found.</p>
      ) : (
        <div className="row">
          {menuItems.map((item) => (
            <div key={item.id} className="col-md-6 mb-4">
              <div className="card shadow-sm">
                <div className="card-body">
                  <h5 className="card-title">{item.name}</h5>
                  <p className="card-text">Price: ₹{item.price}</p>

                  <div className="mb-2">
                    <label className="form-label">Rating (1–5)</label>
                    <input
                      type="number"
                      min="1"
                      max="5"
                      className="form-control"
                      value={reviews[item.id]?.rating || ''}
                      onChange={(e) =>
                        handleInputChange(item.id, 'rating', e.target.value)
                      }
                    />
                  </div>

                  <div className="mb-2">
                    <label className="form-label">Comment</label>
                    <textarea
                      className="form-control"
                      rows="2"
                      value={reviews[item.id]?.comment || ''}
                      onChange={(e) =>
                        handleInputChange(item.id, 'comment', e.target.value)
                      }
                    />
                  </div>

                  <button
                    className="btn btn-success mt-2"
                    onClick={() => handleSubmit(item.id)}
                  >
                    Submit Review
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default Review;
