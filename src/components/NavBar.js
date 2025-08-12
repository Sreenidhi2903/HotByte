import React from "react";
import { Navbar, Nav, Container } from "react-bootstrap";
import { Link, useNavigate } from "react-router-dom";
import logo from "../assets/logo.png";

const NavBar = () => {
  const role = localStorage.getItem("role");
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.clear();
    navigate("/login");
  };

  return (
    <Navbar bg="dark" variant="dark" expand="lg" collapseOnSelect>
      <Container>
        <Navbar.Brand as={Link} to="/">
          <img
            src={logo}
            alt="HotByte"
            style={{ height: "40px", marginRight: "10px" }}
          />
          HotByte
        </Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          {role && (
            <>
              <Nav className="me-auto">
                <Nav.Link as={Link} to="/branches">
                  Branches
                </Nav.Link>

                {role === "Restaurant" && (
                  <>
                    <Nav.Link as={Link} to="/menu">
                      Menu
                    </Nav.Link>
                    <Nav.Link as={Link} to="/orders">
                      Orders
                    </Nav.Link>
                  </>
                )}

                {role === "User" && (
                  <>
                    <Nav.Link as={Link} to="/cart">
                      Cart
                    </Nav.Link>
                    <Nav.Link as={Link} to="/orders">
                      My Orders
                    </Nav.Link>
                  </>
                )}

                {role === "Admin" && (
                  // If admin has extra links, add here. Currently only Branches.
                  null
                )}
              </Nav>

              <Nav>
                <Nav.Link onClick={handleLogout}>Logout</Nav.Link>
              </Nav>
            </>
          )}
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};

export default NavBar;



