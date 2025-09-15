# Customer/Lead Image Upload Feature

A complete full-stack application for managing customers and leads with image upload functionality.

## 🏗️ Architecture

### Backend (C# .NET 8 Web API)
- **Framework**: ASP.NET Core Web API (.NET 8)
- **Database**: SQL Server with Entity Framework Core
- **Image Storage**: Base64 encoded strings in database
- **API Documentation**: Swagger/OpenAPI

### Frontend (Angular 19)
- **Framework**: Angular 19 with Zoneless change detection
- **UI**: Custom SCSS with responsive design
- **HTTP Client**: Angular HttpClient for API communication

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server or SQL Server Express
- Node.js (for Angular)
- Angular CLI

### Backend Setup

1. **Open the solution**:
   ```bash
   # Using Visual Studio
   start CustomerLeadSolution.sln
   
   # Or using command line
   cd Backend
   dotnet build CustomerLeadSolution.sln
   ```

2. **Update database connection** (if needed):
   - Edit `CustomerLeadApi/appsettings.json`
   - Modify the `DefaultConnection` string

3. **Run migrations**:
   ```bash
   cd CustomerLeadApi
   dotnet ef database update
   ```

4. **Start the API**:
   ```bash
   dotnet run
   ```
   - API will be available at: `https://localhost:7113`
   - Swagger UI: `https://localhost:7113/swagger`

### Frontend Setup

1. **Navigate to frontend**:
   ```bash
   cd Frontend/customer-lead-app
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Start the application**:
   ```bash
   ng serve --open
   ```
   - App will be available at: `http://localhost:4200`

## 📋 Features

### ✅ Backend API Endpoints

#### Contacts
- `GET /api/contacts` - List all contacts
- `POST /api/contacts` - Create new contact
- `GET /api/contacts/{id}` - Get contact details
- `PUT /api/contacts/{id}` - Update contact
- `DELETE /api/contacts/{id}` - Delete contact

#### Images
- `GET /api/contacts/{id}/images` - List contact images
- `POST /api/contacts/{id}/images` - Upload single image
- `POST /api/contacts/{id}/images/batch` - Upload multiple images
- `DELETE /api/contacts/{id}/images/{imageId}` - Delete image

### ✅ Frontend Features

#### Contact Management
- View all contacts in a responsive grid
- Create new contacts (Customer or Lead)
- Delete contacts with confirmation
- Navigate to contact details

#### Image Management
- Upload multiple images per contact
- Maximum 10 images per contact (enforced)
- View images in responsive gallery
- Modal viewer with navigation
- Delete images with confirmation
- Real-time image count display

## 🎯 Key Implementation Highlights

### Image Limit Enforcement
- **Backend**: Validates before allowing uploads
- **Frontend**: Shows remaining slots and prevents over-limit uploads
- **Batch Upload**: Respects limits across multiple files

### Base64 Storage Strategy
- Images stored as Base64 strings in database
- Efficient for small to medium images
- No file system dependencies
- Easy backup and deployment

### User Experience
- Intuitive drag-and-drop feel
- Modal image viewer with keyboard navigation
- Real-time feedback on limits
- Responsive design for all devices
- Loading states and error handling

## 🗄️ Database Schema

### Contacts Table
```sql
Id (int, PK)
Name (nvarchar(100), required)
Email (nvarchar(100), optional)
Phone (nvarchar(20), optional)
Type (int) -- 0=Customer, 1=Lead
```

### ContactImages Table
```sql
Id (int, PK)
ContactId (int, FK)
ImageData (nvarchar(max)) -- Base64 string
FileName (nvarchar(100), optional)
ContentType (nvarchar(50), optional)
UploadedAt (datetime2)
```

## 🔧 Configuration

### Backend Configuration
- **Connection String**: `appsettings.json`
- **CORS**: Configured for Angular app
- **Swagger**: Enabled in development

### Frontend Configuration
- **API URL**: `src/app/services/contact.service.ts`
- **Styles**: SCSS with CSS Grid and Flexbox
- **Routing**: Angular Router with lazy loading

## 📱 Responsive Design

The application is fully responsive and works on:
- Desktop computers
- Tablets
- Mobile phones

## 🛠️ Development Tools

### Backend
- Visual Studio or VS Code
- SQL Server Management Studio
- Postman for API testing

### Frontend
- VS Code with Angular extensions
- Chrome DevTools
- Angular DevTools extension

## 🚀 Deployment

### Backend
- Can be deployed to Azure App Service
- IIS on Windows Server
- Docker containers

### Frontend
- Can be deployed to any static hosting
- Azure Static Web Apps
- Netlify, Vercel, etc.

## 📝 Notes

- Images are limited to 10 per contact for performance
- Base64 storage is suitable for moderate image sizes
- For production, consider cloud storage (Azure Blob, AWS S3)
- Database includes proper foreign key relationships
- All endpoints include error handling
