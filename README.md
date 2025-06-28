# Task Management System

## Project Purpose
The Task Management System is designed to help individuals and teams efficiently organize, track, 
and manage their tasks. It solves the problem of task disorganization by providing a secure, user-friendly 
interface for creating, updating, and monitoring tasks with features like prioritization, due dates, and 
completion tracking.

## Project Description

### Key Features
- **User Authentication**: Secure login and registration system
- **Task Management**: Create, edit, delete, and mark tasks as complete
- **Task Organization**: 
  - Title and description fields
  - Priority levels (Low/Medium/High)
  - Due date tracking
- **Task Filtering**: 
  - Search by keyword
  - Toggle between completed/pending views
- **Command History**: View last 12 hours of user actions
- **Data Persistence**: All data saved locally in text files

### Technologies Used
- **C# 7.0**: Core programming language
- **WPF (Windows Presentation Foundation)**: For the user interface
- **MVVM (Model-View-ViewModel)**: Architectural pattern
- **.NET Framework**: Runtime environment

## Installation Instructions

### Prerequisites
- Windows 7 or later
- .NET Framework 4.7.2 or later
- Visual Studio 2019 or later (for development)

### Setup Steps
1. **Clone the repository**:
   ```bash
   git clone https://github.com/Naoyuki-Christopher-H/task-management-system-cs-v1.git
   ```

2. **Open in Visual Studio**:
   - Launch Visual Studio
   - Select "Open a project or solution"
   - Navigate to the cloned repository and open `task-management-system-cs-v1.sln`

3. **Build the solution**:
   - In Visual Studio, go to Build > Build Solution (Ctrl+Shift+B)

4. **Run the application**:
   - Press F5 or click the Start Debugging button

5. **First-time setup**:
   - Register a new account on first launch
   - The system will automatically create necessary data files

### Dependencies
The project uses the following NuGet packages (automatically restored during build):
- None (pure WPF implementation)

## Citation Guidelines

To cite this repository in academic work or publications, please use the following format:

> Author(s). (Year). *Title of Repository*. Available at: \[URL] (Accessed: \[Date]).

## License Information

This project is released under the **MIT License**.

---
