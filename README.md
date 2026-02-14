# Beauty Studio System

This is a **Beauty Studio System** project developed for the **SoftUni ASP.NET course**.

The application is designed to help beauty studios manage their clients, services, and reservations through an integrated booking system.

---

## First Run (Database Seeding, configuration)

You need to change the default connection string in `appsettings.json` to point to your local SQL Server instance.
On the first run, the application automatically seeds:

### ASP.NET Identity Roles
- **Admin**
- **Client**

#### Admin role
The **Admin** has full control over the system:
- View and manage all clients
- Edit client information
- Delete reservations
- Add, edit, and delete services
- Manage bookings

This role is intended for **studio employees or managers**.

#### Client role
The **Client** is the standard user:
- Can create reservations
- Can view and delete their own reservations
- Has access only to client-related features

---

## Seeded Users

The database includes **4 dummy users**:
- 1 Admin
- 3 Clients

It also seeds:
- 3 Services
- 3 Reservations

### Login Information

**Admin account**

Email: admin@admin.com
Password: Admin123!


**Client accounts**


Email: vanq.petrova@gmail.com
Password: Client123!

Email: anna.vasileva@abv.bg
Password: Client123!

Email: kristina.hristova@gmail.com
Password: Client123!


Each time you run the application, Program.cs will reseed the very same data.
---

## Main Application Functionality

The system is designed to serve both **beauty studios** and **their clients** with a built-in booking platform.

### General access (non-registered users)
- Can view the **Services** page
- Can view the **Contacts** page
- Can see service prices

### Registered users
- Can register and log in
- Can create reservations
- Can delete their own reservations

### Admin users
- Can search clients by **name or email**
- Can add, edit, and delete services
- Can update service prices
- Can create and delete reservations
- Can delete client accounts

---

## Architecture

The application uses a **layered architecture**:

- **Controllers** → thin, handle HTTP requests
- **Services** → contain business logic
- **Repositories** → handle database access
- **Data layer** → Entity Framework + Identity

Repositories are used to add an **extra abstraction layer**, so services do not directly depend on the `DbContext`.

The goal is to keep:
- Controllers **clean**
- Business logic inside **services**
- Data access inside **repositories**

---

## Future Plans

Planned improvements:

- Calendar view showing booked time slots
- Unit tests
- More responsive and modern UI
- Additional booking features

The long-term goal is to evolve this project from a student exercise into a **real, monetizable product**.

---

## AI Usage

Views, UI and this README were generated with the help of **AI tools and Bootstrap**, with additional manual adjustments.
