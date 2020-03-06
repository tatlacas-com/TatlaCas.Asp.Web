using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using TatlaCas.Asp.Domain.Resources;
using TatlaCas.Asp.Utils.HtmlForms;

namespace TatlaCas.Asp.Web.ViewModels
{
    public class FormsModel
    {
        public List<Field> Fields { get; set; }
        public string SubmitText { get; set; }
        public bool HasCancel { get; set; }
        public string CancelText { get; set; }
        public string SubmitToController { get; set; }
        public string SubmitToAction { get; set; }
        public PostResult Result { get; set; }

        public FormsModel(IResource resource)
        {
            Fields = new List<Field>();
            var properties = resource.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Field addAnotherCheckbox = null;
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<HtmlFormIgnoreAttribute>() != null)
                {
                    if (resource.ShowAddAnotherAfterSave && property.Name == nameof(resource.AddAnotherAfterSave))
                    {
                        var dsp = property.GetCustomAttribute<DisplayNameAttribute>();
                        addAnotherCheckbox = new Field
                        {
                            FieldType = FieldTypes.Checkbox,
                            DisplayName = dsp != null ? dsp.DisplayName : property.Name,
                            Name = property.Name,
                            Required = property.GetCustomAttribute<RequiredAttribute>() != null ? "true" : "false",
                            Value = (bool) property.GetValue(resource, null) ? "checked" : "",
                            ShowTopDivider = true
                        };
                    }

                    continue;
                }

                var field = new Field
                {
                    FieldType = FieldTypes.Text,
                    DisplayName = property.Name,
                    Placeholder = property.Name,
                    Name = property.Name,
                    Value = (property.GetValue(resource, null) ?? "").ToString(),
                    Required = property.GetCustomAttribute<RequiredAttribute>() != null ? "true" : "false"
                };
                var displayNameAttr = property.GetCustomAttribute<DisplayNameAttribute>();
                if (property.GetCustomAttribute<DisabledFieldAttribute>() != null)
                    field.Disabled = "disabled='disabled'";
                if (property.GetCustomAttribute<ReadOnlyAttribute>() is { } readOnlyAttribute &&
                    readOnlyAttribute.IsReadOnly)
                    field.ReadOnly = "readonly";
                if (property.GetCustomAttribute<MinLengthAttribute>() is { } minLengthAttribute)
                    field.MinLength = minLengthAttribute.Length;
                if (property.GetCustomAttribute<MaxLengthAttribute>() is { } maxLengthAttribute)
                    field.MaxLength = maxLengthAttribute.Length;

                if (property.GetCustomAttribute<EmailAddressAttribute>() != null)
                    field.FieldType = FieldTypes.Email;
                else if (property.GetCustomAttribute<PhoneAttribute>() != null)
                    field.FieldType = FieldTypes.Phone;
                else if (property.GetCustomAttribute<HiddenFieldAttribute>() != null)
                    field.FieldType = FieldTypes.Hidden;
                else
                {
                    if (property.GetCustomAttribute<DropDownAttribute>() != null)
                    {
                        field.FieldType = FieldTypes.DropDown;
                        field.Options = resource.Options[property.Name];
                    }
                    else if (property.PropertyType == typeof(int))
                        field.FieldType = FieldTypes.Integer;
                    else if (property.PropertyType == typeof(bool))
                    {
                        field.FieldType = FieldTypes.Checkbox;
                        field.Value = (bool) property.GetValue(resource, null) ? "checked" : "";
                    }
                }

                if (displayNameAttr != null)
                {
                    field.DisplayName = displayNameAttr.DisplayName;
                    if (displayNameAttr is HtmlFormDisplayNameAttribute fd)
                        field.Placeholder = fd.Placeholder;
                    else field.Placeholder = displayNameAttr.DisplayName;
                }

                Fields.Add(field);
            }

            if (addAnotherCheckbox != null)
                Fields.Add(addAnotherCheckbox);
        }
    }

    public class Field
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public FieldTypes FieldType { get; set; }
        public string Required { get; set; }
        public string ReadOnly { get; set; } = "";
        public string Disabled { get; set; } = "";
        public string Placeholder { get; set; }
        public List<IResourceOption> Options { get; set; }
        public bool ShowTopDivider { get; set; }
        public bool ShowBottomDivider { get; set; }
        public int MinLength { get; set; } = -1;
        public int MaxLength { get; set; } = -1;
    }

    public enum FieldTypes
    {
        Text,
        Email,
        Phone,
        Integer,
        Checkbox,
        RadioButton,
        DropDown,
        Hidden
    }
}