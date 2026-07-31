# PrimeCheque

PrimeCheque is a modern Windows desktop application built with WinUI 3 (Windows App SDK) and .NET 8. It provides a comprehensive solution for managing, previewing, and printing bank cheques with customizable templates, role-based access control, and robust auditing.

## Features

- **Cheque Register & Entry**: Keep track of all cheques with a searchable and filterable ledger. Enter new cheques with an intuitive, centered UI.
- **Template Designer**: A drag-and-drop designer for configuring cheque templates. Calibrate print layouts, background images, and data overlays down to the millimeter.
- **Print & PDF Preview**: Generate highly accurate print previews in real-time. PDF rendering is powered by QuestPDF and viewed natively in a responsive WebView2 component.
- **Role-Based Access Control (RBAC)**: Supports discrete user roles (Administrator, ChequePreparer, Approver, Printer, Auditor) enforcing a strict separation of duties. Cheques must be prepared, approved, and then printed by different personnel.
- **Multi-Bank & Company Management**: Organize your workflow across multiple bank accounts and multiple corporate entities.
- **Batch Import**: Efficiently import cheque data in bulk.
- **Audit Log**: A secure ledger tracking all user actions, logins, state transitions, and modifications within the application.
- **SQLite Database**: Lightweight, zero-configuration local data storage utilizing Entity Framework Core.

## Technologies Used

- **Framework**: .NET 8.0
- **UI**: WinUI 3 / Windows App SDK
- **Architecture**: MVVM using `CommunityToolkit.Mvvm`
- **Database**: SQLite with Entity Framework Core (`Microsoft.EntityFrameworkCore.Sqlite`)
- **PDF Generation**: QuestPDF
- **PDF Viewing**: PdfiumViewer / WebView2
- **Logging**: Serilog

## Getting Started

### Prerequisites

- [Visual Studio 2022](https://visualstudio.microsoft.com/) (Version 17.8 or higher) with the following workloads:
  - .NET desktop development
  - Windows application development
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Windows App SDK](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/)

### Building the Project

1. Clone or download the repository.
2. Open `PrimeCheque.sln` in Visual Studio 2022.
3. Ensure the active architecture is set to **x64** (or x86/ARM64 depending on your system).
4. Build the solution (`Ctrl + Shift + B`) to restore NuGet packages and compile the project.
5. Run the application (`F5`).

## How to Use

1. **Login**: Launch the app and log in using your assigned credentials. By default, the admin account can be used to set up the system.
2. **Setup Banks & Companies**: Navigate to the **Banks** and **Companies** sections to set up your primary entities.
3. **Design a Template**: Go to the **Template Designer** to map out where data (Payee Name, Amount, Date) should print on your physical cheque leaf. You can adjust positions down to the millimeter.
4. **Draft a Cheque**: As a Preparer, go to **New Cheque**, fill out the details, select the appropriate template, and click "Save as Draft" or "Send for Approval".
5. **Approve**: Switch to an Approver account, go to the **Cheque Register**, review the pending cheques, and click "Approve".
6. **Print**: Finally, log in as a Printer, select the approved cheque, preview the generated PDF, and click **Print** to send it directly to your physical printer.

## User Roles & Workflow

The cheque lifecycle is strictly governed by user roles:
1. **Preparer**: Drafts new cheques and sends them for approval.
2. **Approver**: Reviews cheques and marks them as Approved or Rejected.
3. **Printer**: Has exclusive rights to physically print Approved cheques.

*Administrators have full access to system settings, user management, and template configuration.*

## Architecture Notes

- **Minimum Window Size**: The application uses Win32 API hooks (`WM_GETMINMAXINFO`) via PInvoke in `MainWindow.xaml.cs` to enforce a minimum screen resolution (1024x768), guaranteeing UI integrity.
- **Navigation**: Controlled by a customized `NavigationView` implementing a `LeftCompact` pane to maximize horizontal screen real estate.
- **PDF Integration**: The app uses `WebView2` for high-fidelity, resizable PDF previews, replacing traditional WinForms viewers to maintain the modern WinUI 3 aesthetic.
