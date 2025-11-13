using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.Json;

namespace Fadebook.Models;

public abstract class AModel
{
    // public void Update(AModel otherModel)
    // {
    //     var properties = this.GetType()
    //         .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //         .Where(prop =>
    //             prop.CanRead && prop.CanWrite &&
    //             prop.GetCustomAttribute<RequiredAttribute>() != null &&
    //             prop.GetCustomAttribute<KeyAttribute>() == null);

    //     foreach (var prop in properties)
    //     {
    //         var value = prop.GetValue(otherModel);
    //         if (value is not null && !(prop.PropertyType.IsValueType && Equals(value, Activator.CreateInstance(prop.PropertyType))))
    //         {
    //             prop.SetValue(this, value);
    //         }
    //     }
    // }

    public bool AreAllValuesNotNull(bool ignorePrimaryKey = true)
    {
        var properties = this.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(prop =>
                prop.CanRead && prop.CanWrite &&
                // Ignore mapped navigation properties (those with [ForeignKey] attribute on another property)
                prop.GetCustomAttribute<ForeignKeyAttribute>() == null &&
                // Optionally ignore primary key
                (!ignorePrimaryKey || prop.GetCustomAttribute<KeyAttribute>() == null));

        foreach (var prop in properties)
        {
            var value = prop.GetValue(this);
            if (prop.PropertyType.IsValueType)
            {
                if (Equals(value, Activator.CreateInstance(prop.PropertyType)))
                    return false;
            }
            else
            {
                if (value is null)
                    return false;
            }
        }
        return true;
    }

    // Returns a json object of column values
    public string ToJson(bool ignorePrimaryKey = true)
    {
        var properties = this.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(prop =>
                prop.CanRead &&
                prop.GetCustomAttribute<RequiredAttribute>() != null &&
                (ignorePrimaryKey || prop.GetCustomAttribute<KeyAttribute>() == null));

        var dict = new Dictionary<string, object?>();
        foreach (var prop in properties)
        {
            dict[prop.Name] = prop.GetValue(this);
        }
        return JsonSerializer.Serialize(dict);
    }
}