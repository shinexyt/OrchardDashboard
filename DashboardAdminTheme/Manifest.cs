using OrchardCore.DisplayManagement.Manifest;

[assembly: Theme(
    Id = "DashboardAdminTheme",
    Name = "Dashboard Admin Theme",
    Author = "OrchardDashboard",
    Website = "https://orchardcore.net",
    Version = "1.0.0",
    Description = "Custom admin theme for OrchardDashboard.",
    BaseTheme = "TheAdmin",
    Dependencies = new[] { "OrchardCore.Themes", "TheAdmin" },
    Tags = new[] { "admin" }
)]
