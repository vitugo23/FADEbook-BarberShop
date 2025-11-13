# Fadebook - Barbershop Booking System

A modern, full-featured barbershop appointment booking system built with Next.js 15, React 19, and TypeScript. Features a beautiful UI with dark mode support, real-time updates, and comprehensive admin tools.

## Features

### Customer Features
- **User Authentication** - Sign up and sign in with username-based authentication
- **Book Appointments** - Easy-to-use booking interface with service and barber selection
- **My Appointments** - View all your upcoming and past appointments
- **Browse Barbers** - View available barbers and their specialties
- **Browse Services** - See all available services with pricing

### Admin Features
- **Barber Management** - Add, view, and delete barbers with service assignments
- **Service Management** - Create and manage services with pricing
- **Customer Overview** - View all registered customers with search functionality
- **Appointment Monitoring** - Filter and view appointments by date and status

### UI/UX Features
- **Dark Mode** - Full dark mode support with system preference detection
- **Responsive Design** - Mobile-first design that works on all devices
- **Modern UI** - Built with shadcn/ui components and Tailwind CSS
- **Real-time Updates** - Optimistic updates with React Query
- **Form Validation** - Client-side validation with helpful error messages

## 🛠️ Tech Stack

### Core
- **[Next.js 15](https://nextjs.org/)** - React framework with App Router
- **[React 19](https://react.dev/)** - UI library
- **[TypeScript](https://www.typescriptlang.org/)** - Type safety

### UI & Styling
- **[Tailwind CSS](https://tailwindcss.com/)** - Utility-first CSS framework
- **[shadcn/ui](https://ui.shadcn.com/)** - High-quality React components
- **[Lucide Icons](https://lucide.dev/)** - Beautiful icon library
- **[next-themes](https://github.com/pacocoursey/next-themes)** - Dark mode support

### State Management & Data Fetching
- **[TanStack Query (React Query)](https://tanstack.com/query)** - Server state management
- **[Axios](https://axios-http.com/)** - HTTP client

### Development Tools
- **[ESLint](https://eslint.org/)** - Code linting
- **[PostCSS](https://postcss.org/)** - CSS processing

## 📋 Prerequisites

Before you begin, ensure you have the following installed:
- **Node.js** 18.x or higher
- **npm** 9.x or higher
- **Backend API** running on `http://localhost:5288`

## 🚦 Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd fadebook-frontend
```

### 2. Install Dependencies

```bash
npm install
```

### 3. Configure Environment

The application expects the backend API to be running on `http://localhost:5288`. If your API runs on a different port, update the `baseURL` in `src/lib/axios.ts`.

### 4. Run Development Server

```bash
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) in your browser.

### 5. Build for Production

```bash
npm run build
npm start
```

## 📁 Project Structure

```
fadebook-frontend/
├── src/
│   ├── app/                      # Next.js App Router pages
│   │   ├── admin/                # Admin dashboard
│   │   ├── barbers/              # Barbers listing page
│   │   ├── book/                 # Appointment booking page
│   │   ├── my-appointments/      # User's appointments page
│   │   ├── signin/               # Sign in page
│   │   ├── signup/               # Sign up page
│   │   ├── success/              # Booking success page
│   │   ├── layout.tsx            # Root layout with providers
│   │   ├── page.tsx              # Home page
│   │   └── globals.css           # Global styles
│   │
│   ├── components/               # React components
│   │   ├── admin/                # Admin-specific components
│   │   │   ├── AppointmentsTab.tsx
│   │   │   ├── BarbersTab.tsx
│   │   │   ├── CustomersTab.tsx
│   │   │   └── ServicesTab.tsx
│   │   ├── ui/                   # shadcn/ui components
│   │   ├── Navigation.tsx        # Main navigation bar
│   │   ├── theme-provider.tsx    # Theme context provider
│   │   └── theme-toggle.tsx      # Dark mode toggle
│   │
│   ├── hooks/                    # Custom React hooks
│   │   ├── useAppointments.ts    # Appointment data hooks
│   │   ├── useBarbers.ts         # Barber data hooks
│   │   ├── useCustomers.ts       # Customer data hooks
│   │   └── useServices.ts        # Service data hooks
│   │
│   ├── lib/                      # Utility libraries
│   │   ├── api/                  # API client functions
│   │   │   ├── appointments.ts
│   │   │   ├── barbers.ts
│   │   │   └── customers.ts
│   │   ├── axios.ts              # Axios instance configuration
│   │   └── utils.ts              # Utility functions
│   │
│   ├── providers/                # React context providers
│   │   └── QueryProvider.tsx    # React Query provider
│   │
│   └── types/                    # TypeScript type definitions
│       └── api.ts                # API DTOs and interfaces
│
├── public/                       # Static assets
├── package.json                  # Dependencies and scripts
├── tsconfig.json                 # TypeScript configuration
├── tailwind.config.ts            # Tailwind CSS configuration
├── next.config.ts                # Next.js configuration
└── README.md                     # This file
```

## 🎨 UI Components

This project uses [shadcn/ui](https://ui.shadcn.com/) components. Installed components include:

- `badge` - Status indicators and labels
- `button` - Interactive buttons
- `card` - Content containers
- `checkbox` - Multi-select inputs
- `dialog` - Modal dialogs
- `dropdown-menu` - Dropdown menus
- `form` - Form components
- `input` - Text inputs
- `label` - Form labels
- `select` - Dropdown selects
- `table` - Data tables
- `tabs` - Tabbed interfaces

### Adding New Components

```bash
npx shadcn@latest add [component-name]
```

## 🔌 API Integration

The frontend communicates with a .NET Core backend API. All API calls are made through Axios with a configured base URL.

### API Endpoints Used

#### Authentication
- `POST /api/customeraccount/signup` - Create new customer account
- `POST /api/customeraccount/login` - Sign in existing customer

#### Customers
- `GET /api/customer/customers` - Get all customers (admin)
- `GET /api/customer/services` - Get all services
- `GET /api/customer/barbers-by-service/{serviceId}` - Get barbers by service
- `POST /api/customer/request-appointment` - Create appointment

#### Barbers
- `GET /api/barber` - Get all barbers
- `POST /api/barber` - Create barber with services (admin)
- `DELETE /api/barber/{id}` - Delete barber (admin)

#### Services
- `GET /api/service` - Get all services
- `POST /api/service` - Create service (admin)
- `DELETE /api/service/{id}` - Delete service (admin)

#### Appointments
- `GET /api/appointment/by-username/{username}` - Get user's appointments
- `GET /api/appointment/by-date?date={date}` - Get appointments by date (admin)
- `POST /api/appointment` - Create appointment
- `PUT /api/appointment/{id}` - Update appointment
- `DELETE /api/appointment/{id}` - Delete appointment

## 🎯 Key Features Explained

### Authentication System
- Username-based authentication (no passwords for demo purposes)
- Session stored in localStorage
- Automatic redirect to sign in for protected pages
- Sign out clears all session data

### Appointment Booking Flow
1. User signs in or signs up
2. Selects a service from available options
3. Chooses a barber who offers that service
4. Picks date and time
5. Confirms booking
6. Views confirmation on success page

### Admin Dashboard
Four-tab interface for complete system management:
- **Barbers Tab** - Add/remove barbers, assign services
- **Services Tab** - Create/delete services, set pricing
- **Customers Tab** - View all customers, search functionality
- **Appointments Tab** - Filter by date/status, monitor bookings

### Dark Mode
- Three modes: Light, Dark, System
- Persists across sessions
- Smooth transitions
- Respects system preferences

## 🔧 Configuration

### Axios Configuration
Located in `src/lib/axios.ts`:

```typescript
export const axiosInstance = axios.create({
  baseURL: 'http://localhost:5288',
  headers: {
    'Content-Type': 'application/json',
  },
});
```

Update `baseURL` if your backend runs on a different port.

### Theme Configuration
Located in `src/app/layout.tsx`:

```typescript
<ThemeProvider
  attribute="class"
  defaultTheme="system"
  enableSystem
  disableTransitionOnChange
>
```

## 📱 Pages Overview

### Public Pages
- **`/`** - Home page with service overview
- **`/signin`** - Customer sign in
- **`/signup`** - Customer registration
- **`/barbers`** - Browse available barbers

### Protected Pages (Require Authentication)
- **`/book`** - Book new appointment
- **`/my-appointments`** - View user's appointments
- **`/success`** - Booking confirmation

### Admin Pages
- **`/admin`** - Admin dashboard with management tools

## 🧪 Development

### Available Scripts

```bash
# Start development server
npm run dev

# Build for production
npm run build

# Start production server
npm start

# Run linter
npm run lint

# Type check
npx tsc --noEmit
```

### Code Style
- **TypeScript** - Strict mode enabled
- **ESLint** - Configured for Next.js
- **Prettier** - Code formatting (if configured)

## 🔐 Authentication Flow

```
┌─────────────┐
│   Sign Up   │──────┐
└─────────────┘      │
                     ▼
┌─────────────┐   ┌──────────────┐
│   Sign In   │──▶│  localStorage │
└─────────────┘   │  - username   │
                  │  - customerId │
                  │  - isAuth     │
                  └──────────────┘
                         │
                         ▼
                  ┌──────────────┐
                  │ Protected    │
                  │ Pages        │
                  └──────────────┘
```

## 🎨 Styling

### Tailwind CSS
Custom configuration in `tailwind.config.ts` with:
- Custom color palette
- Dark mode support
- Typography utilities
- Responsive breakpoints

### CSS Variables
Theme colors defined in `globals.css`:
- Light mode colors
- Dark mode colors
- Semantic color tokens

## 📦 Dependencies

### Production Dependencies
```json
{
  "@tanstack/react-query": "^5.x",
  "axios": "^1.x",
  "lucide-react": "^0.x",
  "next": "15.x",
  "next-themes": "^0.x",
  "react": "19.x",
  "react-dom": "19.x"
}
```

### Development Dependencies
```json
{
  "@types/node": "^20",
  "@types/react": "^19",
  "@types/react-dom": "^19",
  "eslint": "^9",
  "eslint-config-next": "15.x",
  "postcss": "^8",
  "tailwindcss": "^3.4",
  "typescript": "^5"
}
```

## 🚀 Deployment

### Vercel (Recommended)
1. Push code to GitHub
2. Import project in Vercel
3. Configure environment variables (if needed)
4. Deploy

### Docker
```bash
# Build image
docker build -t fadebook-frontend .

# Run container
docker run -p 3000:3000 fadebook-frontend
```

### Manual Deployment
```bash
npm run build
npm start
```

## 🐛 Troubleshooting

### Common Issues

**Issue**: "Cannot connect to server"
- **Solution**: Ensure backend API is running on `http://localhost:5288`

**Issue**: "Module not found" errors
- **Solution**: Run `npm install` to install all dependencies

**Issue**: Dark mode not working
- **Solution**: Clear browser cache and localStorage

**Issue**: Authentication not persisting
- **Solution**: Check browser localStorage is enabled

**Issue**: Components not found
- **Solution**: Install missing shadcn components:
  ```bash
  npx shadcn@latest add [component-name]
  ```

## 📝 Environment Variables

Currently, the application uses hardcoded API URL. For production, consider using environment variables:

Create `.env.local`:
```env
NEXT_PUBLIC_API_URL=http://localhost:5288
```

Update `src/lib/axios.ts`:
```typescript
baseURL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5288'
```

## 🧪 Testing

### Manual Testing Checklist
- [ ] Sign up with new account
- [ ] Sign in with existing account
- [ ] Book appointment
- [ ] View appointments
- [ ] Browse barbers
- [ ] Admin: Add barber
- [ ] Admin: Add service
- [ ] Admin: View customers
- [ ] Admin: Filter appointments
- [ ] Dark mode toggle
- [ ] Sign out

### Browser Console
Open browser DevTools (F12) to view:
- API request/response logs
- Authentication state changes
- Error messages

## 🤝 Contributing

### Development Workflow
1. Create feature branch
2. Make changes
3. Test thoroughly
4. Submit pull request

### Code Standards
- Use TypeScript for type safety
- Follow existing component patterns
- Use shadcn/ui components when possible
- Keep components under 300 lines
- Write meaningful commit messages

## 📄 License

This project is part of the Night Owls team project for Revature training.

## 👥 Team

**The Night Owls** - Revature .NET Training Cohort

## 🔗 Related Projects

- **Backend API** - Located in `../api` directory
- **Database** - SQL Server with Entity Framework Core

## 📞 Support

For issues or questions:
1. Check the troubleshooting section above
2. Review browser console for errors
3. Verify backend API is running
4. Check API endpoint documentation

## 🎓 Learning Resources

- [Next.js Documentation](https://nextjs.org/docs)
- [React Documentation](https://react.dev/)
- [Tailwind CSS Documentation](https://tailwindcss.com/docs)
- [shadcn/ui Documentation](https://ui.shadcn.com/)
- [TanStack Query Documentation](https://tanstack.com/query/latest)

## 🔄 Version History

### Current Version
- Full authentication system
- Complete booking flow
- Admin dashboard with 4 management tabs
- Dark mode support
- Responsive design
- Real-time data updates

## 🚧 Future Enhancements

Potential features for future development:
- [ ] Email notifications
- [ ] SMS reminders
- [ ] Payment integration
- [ ] Barber availability calendar
- [ ] Customer reviews and ratings
- [ ] Photo gallery
- [ ] Multi-location support
- [ ] Appointment rescheduling
- [ ] Cancellation policies
- [ ] Loyalty program
- [ ] Gift cards
- [ ] Social media integration

## 📊 Performance

- **Lighthouse Score**: Optimized for performance
- **Code Splitting**: Automatic with Next.js
- **Image Optimization**: Next.js Image component
- **Font Optimization**: Geist font family with next/font

## 🔒 Security Notes

⚠️ **Important**: Current authentication is for development/demo purposes only.

### For Production:
- Implement proper JWT authentication
- Use HTTP-only cookies
- Add password hashing
- Implement CSRF protection
- Use HTTPS only
- Add rate limiting
- Implement proper session management
- Add input sanitization
- Use environment variables for sensitive data

---

**Built with ❤️ by The Night Owls Team**
