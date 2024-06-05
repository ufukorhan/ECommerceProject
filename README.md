<img width="462" alt="image" src="https://github.com/ufukorhan/ECommerceProject/assets/91383831/19d1f89b-0abc-4018-b1f2-4c413b9cb0e2">

# ECommerceAPI

ECommerceAPI is an ASP.NET Core Web API project developed for an e-commerce platform. This project provides various services ranging from user registration and authentication to product and category management, cart and payment operations. Additionally, there is a separate PaymentAPI project to handle payment operations.

## Project Architecture

The project consists of two main APIs:
- **ECommerceAPI**: The main e-commerce API
- **PaymentAPI**: A separate API for payment operations

### Services

#### 1. Account Service
- **Applyment**: Seller application process
- **Register**: User registration process
- **Authenticate**: Token generation process

#### 2. Category Service
- **List**: List all categories
- **GetById**: Get details of a specific category
- **Create**: Create a new category
- **Update**: Update an existing category
- **Delete**: Delete a category

#### 3. Product Service
- **List**: List all products
- **GetById**: Get details of a specific product
- **Create**: Create a new product
- **Update**: Update an existing product
- **Delete**: Delete a product

#### 4. Cart Service
- **GetCart**: Retrieve cart details
- **AddToCart**: Add a product to the cart

#### 5. Payment Gateway Service
- **Pay**: Process a payment (fake payment)

### PaymentAPI
- A separate API project for handling payment operations.

  ## Installation and Running

### Requirements
- [.NET Core SDK](https://dotnet.microsoft.com/download) must be installed.
- A SQL Server database.

### Installation Steps

1. **Clone the repository**
    ```sh
    git clone https://github.com/ufukorhan/ECommerceProject.git
    cd ECommerceProject
    ```

2. **Set up the database**
    - Update the connection string in the `appsettings.json` file with your SQL Server settings.

3. **Install dependencies**
    ```sh
    dotnet restore
    ```

4. **Apply database migrations**
    ```sh
    dotnet ef database update
    ```

5. **Run the project**
    ```sh
    dotnet run
    ```
