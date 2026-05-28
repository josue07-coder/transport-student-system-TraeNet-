using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Transport.Domain.Entities;
using Transport.Infrastructure.Persistence.Context;

namespace Transport.Infrastructure.Security
{
    public static class SystemSettingSeeder
    {
        private static readonly (string Key, string Value, string Description, string Category, string DataType, bool IsEditable)[] BaseSettings =
        {
            ("General.SystemName", "Transport Student System", "Nombre del sistema", "General", "String", false),
            ("General.SchoolName", "Nombre de la institución", "Nombre de la institución principal", "General", "String", true),
            ("Branding.LogoUrl", "", "URL del logo institucional", "Branding", "String", true),
            ("Branding.PrimaryColor", "#2563EB", "Color primario de la interfaz", "Branding", "String", true),
            ("GPS.UpdateIntervalSeconds", "15", "Intervalo de actualización GPS en segundos", "GPS", "Integer", true),
            ("GPS.SpeedLimitKmH", "80", "Límite de velocidad para alertas", "GPS", "Decimal", true),
            ("GPS.EnableSpeedAlerts", "true", "Activa alertas de exceso de velocidad", "GPS", "Boolean", true),
            ("Notifications.EnableInApp", "true", "Activa notificaciones internas", "Notifications", "Boolean", true),
            ("Notifications.EnableEmail", "false", "Activa notificaciones por email", "Notifications", "Boolean", true),
            ("Notifications.EnableSms", "false", "Activa notificaciones por SMS", "Notifications", "Boolean", true),
            ("Notifications.EnableWhatsApp", "false", "Activa notificaciones por WhatsApp", "Notifications", "Boolean", true),
            ("Notifications.EnablePush", "false", "Activa notificaciones push", "Notifications", "Boolean", true),
            ("Trips.MaxRouteDurationMinutes", "240", "Duración máxima permitida de una ruta", "Trips", "Integer", true),
            ("Trips.MinRouteDurationMinutes", "10", "Duración mínima permitida de una ruta", "Trips", "Integer", true),
            ("Reports.DefaultDateRangeDays", "30", "Rango por defecto para reportes", "Reports", "Integer", true),
            ("Security.PasswordMinLength", "6", "Longitud mínima de contraseña", "Security", "Integer", true),
            ("Backup.EnableAutoBackup", "false", "Activa respaldo automático", "Backup", "Boolean", true),
            ("Backup.LocalPath", "backups", "Ruta local para archivos de backup", "Backup", "String", true),
            ("Backup.RetentionDays", "30", "Días de retención de backups", "Backup", "Integer", true),
            ("Maps.DefaultAverageSpeedKmH", "35", "Velocidad promedio para estimaciones de mapas", "Maps", "Decimal", true),
            ("Storage.Provider", "Mock", "Proveedor de almacenamiento de archivos", "Storage", "String", true),
            ("Storage.LocalPath", "wwwroot/uploads", "Ruta local para almacenamiento de archivos", "Storage", "String", true)
        };

        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!await TableExistsAsync(context))
                return;

            foreach (var setting in BaseSettings)
            {
                var exists = await context.SystemSettings.AnyAsync(x => x.Key == setting.Key);
                if (exists)
                    continue;

                await context.SystemSettings.AddAsync(new SystemSetting(
                    setting.Key,
                    setting.Value,
                    setting.Category,
                    setting.DataType,
                    setting.Description,
                    setting.IsEditable));
            }

            await context.SaveChangesAsync();
        }

        private static async Task<bool> TableExistsAsync(AppDbContext context)
        {
            var connection = context.Database.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT CASE WHEN OBJECT_ID(N'[SystemSettings]', N'U') IS NULL THEN 0 ELSE 1 END";
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) == 1;
        }
    }
}
