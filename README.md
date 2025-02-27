# 🛒 EcommerceAPI  

EcommerceAPI is a RESTful API built with ASP.NET Core and Entity Framework Core for managing an e-commerce platform. It supports user authentication, product management, orders, and payments.  

## 🚀 Features  

✅ User authentication & role-based authorization  
✅ Product catalog management  
✅ Order processing & order-product relationships  
✅ Payment processing  
✅ SQLite database with EF Core  
✅ Unit tests using xUnit  
✅ CI/CD pipeline with GitHub Actions  
✅ Deployment-ready for Azure  

---

## 🛠️ **Technologies Used**  

- **Backend**: ASP.NET Core (.NET 9)  
- **Database**: SQLite  
- **Authentication**: ASP.NET Identity + JWT  
- **Testing**: xUnit  
- **Deployment**: Azure App Service (CI/CD via GitHub Actions)  

---

## 📂 **Project Structure**  

EcommerceAPI/ │── Controllers/ # API Controllers
│── Data/ # Database Context & Config
│── Models/ # Entity Models
│── Services/ # Business Logic Services
│── Migrations/ # EF Core Migrations
│── EcommerceAPI.Tests/ # Unit Tests
│── .github/workflows/ # CI/CD Pipeline
│── Program.cs # Main Application Entry
│── appsettings.json # App Configuration
│── ecommerce.db # SQLite Database
│── README.md # Project Documentation

---

## 🔑 **Authentication**  

The API uses JWT authentication. To access protected routes, include a **Bearer Token** in the request header:  

Authorization: Bearer <your_token>

### User Roles:  
- **Admin**: Full access to manage products, orders, and users  
- **Customer**: Can browse products, place orders, and make payments  

---

## 🔗 **API Endpoints**  

### 🛠️ **User Management** (`/api/user`)  
- `POST /api/user/register` → Register new user  
- `POST /api/user/login` → Authenticate & get JWT token  
- `GET /api/user/{id}` → Get user by ID  

### 🛍️ **Product Management** (`/api/products`)  
- `GET /api/products` → Get all products  
- `POST /api/products` → Add new product *(Admin only)*  

### 📦 **Order Processing** (`/api/orders`)  
- `GET /api/orders` → Get all orders *(Admin only)*  
- `POST /api/orders` → Create a new order  

### 💳 **Payments** (`/api/payments`)  
- `POST /api/payments` → Process a payment  

---

## ✅ **Running the Project**  

### 1️⃣ **Clone the Repository**  
```sh
git clone https://github.com/yourusername/EcommerceAPI.git
cd EcommerceAPI
dotnet run
