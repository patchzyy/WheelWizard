# Code Conventions

This document outlines the conventions for organizing code files within the project. Adhering to these conventions helps maintain a consistent and scalable project structure.

## File Locations & Naming

### 1. `Helpers` Directory
- **Purpose:** Contains classes that are purely collections of utility functions designed to simplify coding across the application.
- **Guidelines:**
    - **Static Classes:** Helper classes should be static and stateless to remain independent and easily accessible.
    - **Naming Convention:** End helper class/file names with `Helper` unless the name is already widely recognized as a helper (e.g., `Humanizer`).

### 2. `Models` Directory
- **Purpose:** This directory contains data structures that represent the core models of the application.
- **File Types:** Classes that define properties (and little functionality) for data representation.

### 3. `Resources` Directory
- **Purpose:** Holds non-code resources like fonts, language files, images, and other media.
- **Organization:** Place each resource type in its own subdirectory, such as:
    - `Fonts`
    - `Languages`
    - `Images`

### 4. `Services` Directory
- **Purpose:** Contains code that interacts with external resources or systems (e.g., HTTP requests, file I/O).
- **Guidelines:** Keep this directory organized by grouping related service files.

### 5. `Views` Directory
IMPORTANT: THIS BIT IS OUTDATES SINCE WE MOVED TO AVALONIA. I WILL UPDATE IT SOON.   
BUT ITS BASICALLY THE SAME, ONLY THE STYLES AND CONVERTERS ARE THEIR OWN FOLDER NOW
- **Purpose:** Holds XAML files and associated components for UI.
- **Subdirectories:**
    - **`Components`:** Contains reusable UI components (e.g., buttons, list views).
    - **`Converters`:** Holds converter classes that are specifically designed for XAML binding.
        - **Naming Convention:** End converter file names with `Converter`.
    - **`Pages`:** Contains XAML files representing pages.
        - **Naming Convention:** End page file names with `Page`.
    - **`Popups`:** Contains XAML files for popup windows.
        - **Naming Convention:** End popup file names with `Window`.
    - **`Styles`:** Contains XAML style files.
        - **Naming Convention:** End style file names with `Style` or `Styles`.
- **View Utility Files:** Any code useful across views should be placed in `Views/ViewUtils.cs`.

### 6. `Utilities` Directory
- **Purpose:** Contains any code that does not fit the above criteria (and is not `App.xaml` or `AssemblyInfo.cs`).
- **Guidelines:** Use this as a catch-all for miscellaneous utilities.

---

Following these conventions ensures a clean, modular, and organized codebase that is easy to maintain and scale.
