using Transport.Domain.Common;
using Transport.Domain.Exceptions;

namespace Transport.Domain.Entities
{
    public class SystemSetting : BaseEntity
    {
        public string Key { get; private set; } = string.Empty;
        public string Value { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string Category { get; private set; } = string.Empty;
        public string DataType { get; private set; } = string.Empty;
        public bool IsEditable { get; private set; }

        private SystemSetting() { } // EF

        public SystemSetting(
            string key,
            string value,
            string category,
            string dataType,
            string? description = null,
            bool isEditable = true)
        {
            SetKey(key);
            SetValue(value);
            SetCategory(category);
            SetDataType(dataType);
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            IsEditable = isEditable;
        }

        public void Update(string value, string category, string dataType, string? description, bool isEditable)
        {
            if (!IsEditable)
                throw new DomainException("No se puede actualizar una configuración no editable");

            SetValue(value);
            SetCategory(category);
            SetDataType(dataType);
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            IsEditable = isEditable;
            SetUpdated();
        }

        public void UpdateValue(string value)
        {
            if (!IsEditable)
                throw new DomainException("No se puede actualizar una configuración no editable");

            SetValue(value);
            SetUpdated();
        }

        private void SetKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new DomainException("La clave de configuración es requerida");

            Key = key.Trim();
        }

        private void SetValue(string value)
        {
            if (value is null)
                throw new DomainException("El valor de configuración es requerido");

            Value = value.Trim();
        }

        private void SetCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new DomainException("La categoría de configuración es requerida");

            Category = category.Trim();
        }

        private void SetDataType(string dataType)
        {
            if (string.IsNullOrWhiteSpace(dataType))
                throw new DomainException("El tipo de dato de configuración es requerido");

            DataType = dataType.Trim();
        }
    }
}
