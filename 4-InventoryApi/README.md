Description

An IMS (Inventory Management System) is a system used to rent organization equipment to clients for a specific number of days with price calculated per day.

The system supports authentication, equipment management, borrowing workflow, payments, and reporting.

Auth Role
------------------------------
Admin
Client

Admin Login
-------------------------------
email
password

Client Login / Register
-------------------------------
email
password

UserData
-------------------------------
id
name
email
password
gender
address
phone_no
role (ADMIN / CLIENT)
status (active / inactive)
created_at

Admin Operations
=============================
1) User Management

Create User
View All Users
View User By Id
Update User
Deactivate User
Soft Delete User

2) Equipment Management

Add Equipment
Update Equipment
Delete Equipment (soft delete)
View All Equipment
View Equipment By Id
Update Condition
Update Price Per Day
Change Status (Available / Borrowed / Maintenance)

3) Borrow Management

Approve Borrow Request
Reject Borrow Request
Assign Equipment
Set Release Date
Mark Returned
Add Pre Remarks
Add Post Remarks

4) Payment Management

Generate Bill
Update Paid Amount
Calculate Due Amount
Select Payment Mode
Mark Fully Paid

5) Reports

Daily Rentals
Monthly Revenue
Client Borrow History
Pending Dues
Available Inventory
Most Borrowed Equipment

6) Client Operations

Register
Login
View Profile
Update Profile

7) Equipment

View Available Equipment
Search Equipment

8) Borrow

Request Borrow
View My Borrow History
Cancel Request (before approval)

9) Payment

View Bill
Pay Amount

10) Equipment Data

id
name
equipment_code
description
category
condition
price_per_day
status
created_at

11) Borrow Data

id
user_id
equipment_id
borrowed_days (single_day / recurrence)
price
paid
due_amount
payment_mode
assigned_date
release_date
pre_remarks
post_remarks
borrow_status (requested / approved / rejected / returned)

12) Payment Data (Optional Table)

id
borrow_id
total_amount
paid_amount
due_amount
payment_mode
payment_status
payment_date

13) System Workflow

Client registers

Admin approves client

Client requests equipment

Admin assigns equipment

System calculates price

Client pays amount

Admin marks equipment returned

Admin Storage Design (Best Practice)

Recommended Approach:

Single Users Table with Role-Based Access

Users table contains both Admin and Client using role column:

role = ADMIN
role = CLIENT

Advantages:
• Clean architecture
• Easy scalability
• Role-based authorization
• Secure admin APIs
• Simple maintenance

Admin routes protected using middleware.

Security

JWT Authentication
Role Middleware
Soft Deletes
Admin Activity Logs
Password Hashing

Future Scalability

Add Roles: Super Admin, Staff
Add Invoice Generation
Add Notifications
Add Dashboard Analytics